using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Base Class for Save Files
/// </summary>
public abstract class SaveFile : ITrainerInfo, IGameValueLimit, IBoxDetailWallpaper, IBoxDetailName, IGeneration, IVersion
{
    // General Object Properties
    public byte[] Data;

    public SaveFileState State { get; }
    public SaveFileMetadata Metadata { get; private set; }

    protected SaveFile(byte[] data, bool exportable = true)
    {
        Data = data;
        State = new SaveFileState(exportable);
        Metadata = new SaveFileMetadata(this);
    }

    protected SaveFile(int size = 0) : this(size == 0 ? Array.Empty<byte>() : new byte[size], false) { }

    protected internal abstract string ShortSummary { get; }
    public abstract string Extension { get; }

    protected abstract SaveFile CloneInternal();

    public SaveFile Clone()
    {
        var sav = CloneInternal();
        sav.Metadata = Metadata;
        return sav;
    }

    public virtual string PlayTimeString => $"{PlayedHours}ː{PlayedMinutes:00}ː{PlayedSeconds:00}"; // not :

    public virtual IReadOnlyList<string> PKMExtensions => Array.FindAll(PKM.Extensions, f =>
    {
        int gen = f[^1] - 0x30;
        return 3 <= gen && gen <= Generation;
    });

    // General SAV Properties
    public byte[] Write(BinaryExportSetting setting = BinaryExportSetting.None)
    {
        byte[] data = GetFinalData();
        return Metadata.Finalize(data, setting);
    }

    protected virtual byte[] GetFinalData()
    {
        SetChecksums();
        return Data;
    }

    #region Metadata & Limits
    public virtual string MiscSaveInfo() => string.Empty;
    public virtual GameVersion Version { get; protected set; }
    public abstract bool ChecksumsValid { get; }
    public abstract string ChecksumInfo { get; }
    public abstract int Generation { get; }
    public abstract EntityContext Context { get; }
    #endregion

    #region Savedata Container Handling
    public byte[] GetData(int offset, int length) => GetData(Data, offset, length);
    protected static byte[] GetData(byte[] data, int offset, int length) => data.Slice(offset, length);
    public void SetData(byte[] input, int offset) => SetData(Data, input, offset);

    public void SetData(Span<byte> dest, Span<byte> input, int offset)
    {
        input.CopyTo(dest[offset..]);
        State.Edited = true;
    }

    public abstract string GetString(ReadOnlySpan<byte> data);
    public string GetString(int offset, int length) => GetString(Data.AsSpan(offset, length));
    public abstract int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option);
    #endregion

    public virtual void CopyChangesFrom(SaveFile sav) => SetData(sav.Data, 0);

    // Offsets

    #region Stored PKM Limits
    public abstract IPersonalTable Personal { get; }
    public abstract int OTLength { get; }
    public abstract int NickLength { get; }
    public abstract int MaxMoveID { get; }
    public abstract int MaxSpeciesID { get; }
    public abstract int MaxAbilityID { get; }
    public abstract int MaxItemID { get; }
    public abstract int MaxBallID { get; }
    public abstract int MaxGameID { get; }
    public virtual int MinGameID => 0;
    #endregion

    /// <summary>
    /// Gets the <see cref="bool"/> status of the Flag at the specified offset and index.
    /// </summary>
    /// <param name="offset">Offset to read from</param>
    /// <param name="bitIndex">Bit index to read</param>
    /// <returns>Flag is Set (true) or not Set (false)</returns>
    public virtual bool GetFlag(int offset, int bitIndex) => FlagUtil.GetFlag(Data, offset, bitIndex);

    /// <summary>
    /// Sets the <see cref="bool"/> status of the Flag at the specified offset and index.
    /// </summary>
    /// <param name="offset">Offset to read from</param>
    /// <param name="bitIndex">Bit index to read</param>
    /// <param name="value">Flag status to set</param>
    /// <remarks>Flag is Set (true) or not Set (false)</remarks>
    public virtual void SetFlag(int offset, int bitIndex, bool value) => FlagUtil.SetFlag(Data, offset, bitIndex, value);

    public virtual IReadOnlyList<InventoryPouch> Inventory { get => Array.Empty<InventoryPouch>(); set { } }

    #region Mystery Gift
    protected virtual int GiftCountMax { get; } = int.MinValue;
    protected virtual int GiftFlagMax { get; } = 0x800;
    protected int WondercardData { get; set; } = int.MinValue;
    public bool HasWondercards => WondercardData > -1;
    protected virtual bool[] MysteryGiftReceivedFlags { get => Array.Empty<bool>(); set { } }
    protected virtual DataMysteryGift[] MysteryGiftCards { get => Array.Empty<DataMysteryGift>(); set { } }

    public virtual MysteryGiftAlbum GiftAlbum
    {
        get => new(MysteryGiftCards, MysteryGiftReceivedFlags);
        set
        {
            MysteryGiftReceivedFlags = value.Flags;
            MysteryGiftCards = value.Gifts;
        }
    }
    #endregion

    #region Player Info
    public virtual int Gender { get; set; }
    public virtual int Language { get => -1; set { } }
    public virtual int Game { get => -1; set { } }
    public virtual int TID { get; set; }
    public virtual int SID { get; set; }
    public virtual string OT { get; set; } = "PKHeX";
    public virtual int PlayedHours { get; set; }
    public virtual int PlayedMinutes { get; set; }
    public virtual int PlayedSeconds { get; set; }
    public virtual uint SecondsToStart { get; set; }
    public virtual uint SecondsToFame { get; set; }
    public virtual uint Money { get; set; }
    public abstract int BoxCount { get; }
    public virtual int SlotCount => BoxCount * BoxSlotCount;
    public int TrainerID7 { get => (int)((uint)(TID | (SID << 16)) % 1000000); set => SetID7(TrainerSID7, value); }
    public int TrainerSID7 { get => (int)((uint)(TID | (SID << 16)) / 1000000); set => SetID7(value, TrainerID7); }
    public virtual int MaxMoney => 9999999;
    public virtual int MaxCoins => 9999;

    public int DisplayTID
    {
        get => Generation >= 7 ? TrainerID7 : TID;
        set { if (Generation >= 7) TrainerID7 = value; else TID = value; }
    }

    public int DisplaySID
    {
        get => Generation >= 7 ? TrainerSID7 : SID;
        set { if (Generation >= 7) TrainerSID7 = value; else SID = value; }
    }
    #endregion

    private void SetID7(int sid7, int tid7)
    {
        var oid = (sid7 * 1_000_000) + (tid7 % 1_000_000);
        TID = (ushort)oid;
        SID = oid >> 16;
    }

    #region Party
    public virtual int PartyCount { get; protected set; }
    protected int Party { get; set; } = int.MinValue;
    public virtual bool HasParty => Party > -1;
    public abstract int GetPartyOffset(int slot);

    public bool IsPartyAllEggs(int except = -1)
    {
        if (!HasParty)
            return false;

        for (int i = 0; i < MaxPartyCount; i++)
        {
            if (i == except)
                continue;

            if (IsPartySlotNotEggOrEmpty(i))
                return false;
        }

        return true;
    }

    private bool IsPartySlotNotEggOrEmpty(int index)
    {
        var slot = GetPartySlotAtIndex(index);
        return !slot.IsEgg && slot.Species != 0;
    }

    private const int MaxPartyCount = 6;

    public IList<PKM> PartyData
    {
        get
        {
            var count = PartyCount;
            if ((uint)count > MaxPartyCount)
                count = MaxPartyCount;

            PKM[] data = new PKM[count];
            for (int i = 0; i < data.Length; i++)
                data[i] = GetPartySlot(PartyBuffer, GetPartyOffset(i));
            return data;
        }
        set
        {
            if (value.Count is 0 or > MaxPartyCount)
                throw new ArgumentOutOfRangeException(nameof(value), $"Expected 1-6, got {value.Count}");
#if DEBUG
            if (value[0].Species == 0)
                System.Diagnostics.Debug.WriteLine($"Empty first slot, received {value.Count}.");
#endif
            int ctr = 0;
            foreach (var exist in value.Where(pk => pk.Species != 0))
                SetPartySlot(exist, PartyBuffer, GetPartyOffset(ctr++));
            for (int i = ctr; i < 6; i++)
                SetPartySlot(BlankPKM, PartyBuffer, GetPartyOffset(i));
        }
    }
    #endregion

    // Varied Methods
    protected abstract void SetChecksums();

    #region Daycare
    public bool HasDaycare => DaycareOffset > -1;
    protected int DaycareOffset { get; set; } = int.MinValue;
    public virtual int DaycareSeedSize { get; } = 0;
    public int DaycareIndex = 0;
    public virtual bool HasTwoDaycares => false;
    public virtual int GetDaycareSlotOffset(int loc, int slot) => -1;
    public virtual uint? GetDaycareEXP(int loc, int slot) => null;
    public virtual string GetDaycareRNGSeed(int loc) => string.Empty;
    public virtual bool? IsDaycareHasEgg(int loc) => null;
    public virtual bool? IsDaycareOccupied(int loc, int slot) => null;

    public virtual void SetDaycareEXP(int loc, int slot, uint EXP) { }
    public virtual void SetDaycareRNGSeed(int loc, string seed) { }
    public virtual void SetDaycareHasEgg(int loc, bool hasEgg) { }
    public virtual void SetDaycareOccupied(int loc, int slot, bool occupied) { }
    #endregion

    public PKM GetPartySlotAtIndex(int index) => GetPartySlot(PartyBuffer, GetPartyOffset(index));

    public void SetPartySlotAtIndex(PKM pk, int index, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
    {
        // update party count
        if ((uint)index > 5)
            throw new ArgumentOutOfRangeException(nameof(index));

        int currentCount = PartyCount;
        if (pk.Species != 0)
        {
            if (currentCount <= index)
                PartyCount = index + 1;
        }
        else if (currentCount > index)
        {
            PartyCount = index;
        }

        int offset = GetPartyOffset(index);
        SetPartySlot(pk, PartyBuffer, offset, trade, dex);
    }

    public void SetSlotFormatParty(PKM pk, byte[] data, int offset, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
    {
        if (pk.GetType() != PKMType)
            throw new ArgumentException($"PKM Format needs to be {PKMType} when setting to this Save File.");

        UpdatePKM(pk, isParty: true, trade, dex);
        SetPartyValues(pk, isParty: true);
        WritePartySlot(pk, data, offset);
    }

    public void SetPartySlot(PKM pk, byte[] data, int offset, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
    {
        if (pk.GetType() != PKMType)
            throw new ArgumentException($"PKM Format needs to be {PKMType} when setting to this Save File.");

        UpdatePKM(pk, isParty: true, trade, dex);
        SetPartyValues(pk, isParty: true);
        WritePartySlot(pk, data, offset);
    }

    public void SetSlotFormatStored(PKM pk, Span<byte> data, int offset, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
    {
        if (pk.GetType() != PKMType)
            throw new ArgumentException($"PKM Format needs to be {PKMType} when setting to this Save File.");

        UpdatePKM(pk, isParty: false, trade, dex);
        SetPartyValues(pk, isParty: false);
        WriteSlotFormatStored(pk, data, offset);
    }

    public void SetBoxSlot(PKM pk, Span<byte> data, int offset, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
    {
        if (pk.GetType() != PKMType)
            throw new ArgumentException($"PKM Format needs to be {PKMType} when setting to this Save File.");

        UpdatePKM(pk, isParty: false, trade, dex);
        SetPartyValues(pk, isParty: false);
        WriteBoxSlot(pk, data, offset);
    }

    public void DeletePartySlot(int slot)
    {
        int newEmpty = PartyCount - 1;
        if ((uint)slot > newEmpty) // beyond party range (or empty data already present)
            return;
        // Move all party slots down one
        for (int i = slot + 1; i <= newEmpty; i++) // Slide slots down
        {
            var current = GetPartySlotAtIndex(i);
            SetPartySlotAtIndex(current, i - 1, PKMImportSetting.Skip, PKMImportSetting.Skip);
        }
        SetPartySlotAtIndex(BlankPKM, newEmpty, PKMImportSetting.Skip, PKMImportSetting.Skip);
        // PartyCount will automatically update via above call. Do not adjust.
    }

    #region Slot Storing
    public static PKMImportSetting SetUpdateDex { protected get; set; } = PKMImportSetting.Update;
    public static PKMImportSetting SetUpdatePKM { protected get; set; } = PKMImportSetting.Update;

    public abstract Type PKMType { get; }
    protected abstract PKM GetPKM(byte[] data);
    protected abstract byte[] DecryptPKM(byte[] data);
    public abstract PKM BlankPKM { get; }
    protected abstract int SIZE_STORED { get; }
    protected abstract int SIZE_PARTY { get; }
    public virtual int SIZE_BOXSLOT => SIZE_STORED;
    public abstract int MaxEV { get; }
    public virtual int MaxIV => 31;
    public abstract IReadOnlyList<ushort> HeldItems { get; }
    protected virtual byte[] BoxBuffer => Data;
    protected virtual byte[] PartyBuffer => Data;
    public virtual bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresent(data);
    public virtual PKM GetDecryptedPKM(byte[] data) => GetPKM(DecryptPKM(data));
    public virtual PKM GetPartySlot(byte[] data, int offset) => GetDecryptedPKM(GetData(data, offset, SIZE_PARTY));
    public virtual PKM GetStoredSlot(byte[] data, int offset) => GetDecryptedPKM(GetData(data, offset, SIZE_STORED));
    public virtual PKM GetBoxSlot(int offset) => GetStoredSlot(BoxBuffer, offset);

    public virtual byte[] GetDataForFormatStored(PKM pk) => pk.EncryptedBoxData;
    public virtual byte[] GetDataForFormatParty(PKM pk) => pk.EncryptedPartyData;
    public virtual byte[] GetDataForParty(PKM pk) => pk.EncryptedPartyData;
    public virtual byte[] GetDataForBox(PKM pk) => pk.EncryptedBoxData;

    public virtual void WriteSlotFormatStored(PKM pk, Span<byte> data, int offset) => SetData(data, GetDataForFormatStored(pk), offset);
    public virtual void WriteSlotFormatParty(PKM pk, Span<byte> data, int offset) => SetData(data, GetDataForFormatParty(pk), offset);
    public virtual void WritePartySlot(PKM pk, Span<byte> data, int offset) => SetData(data, GetDataForParty(pk), offset);
    public virtual void WriteBoxSlot(PKM pk, Span<byte> data, int offset) => SetData(data, GetDataForBox(pk), offset);

    protected virtual void SetPartyValues(PKM pk, bool isParty)
    {
        if (!isParty)
            return;
        if (pk.PartyStatsPresent) // Stats already present
            return;
        pk.ResetPartyStats();
    }

    /// <summary>
    /// Conditions a <see cref="pk"/> for this save file as if it was traded to it.
    /// </summary>
    /// <param name="pk">Entity to adapt</param>
    /// <param name="party">Entity exists in party format</param>
    /// <param name="trade">Setting on whether or not to adapt</param>
    public void AdaptPKM(PKM pk, bool party = true, PKMImportSetting trade = PKMImportSetting.UseDefault)
    {
        if (GetTradeUpdateSetting(trade))
            SetPKM(pk, party);
    }

    protected void UpdatePKM(PKM pk, bool isParty, PKMImportSetting trade, PKMImportSetting dex)
    {
        AdaptPKM(pk, isParty, trade);
        if (GetDexUpdateSetting(dex))
            SetDex(pk);
    }

    private static bool GetTradeUpdateSetting(PKMImportSetting trade = PKMImportSetting.UseDefault)
    {
        if (trade == PKMImportSetting.UseDefault)
            trade = SetUpdatePKM;
        return trade == PKMImportSetting.Update;
    }

    private static bool GetDexUpdateSetting(PKMImportSetting trade = PKMImportSetting.UseDefault)
    {
        if (trade == PKMImportSetting.UseDefault)
            trade = SetUpdateDex;
        return trade == PKMImportSetting.Update;
    }

    protected virtual void SetPKM(PKM pk, bool isParty = false) { }
    protected virtual void SetDex(PKM pk) { }
    #endregion

    #region Pokédex
    public int PokeDex { get; protected set; } = int.MinValue;
    public bool HasPokeDex => PokeDex > -1;
    public virtual bool GetSeen(int species) => false;
    public virtual void SetSeen(int species, bool seen) { }
    public virtual bool GetCaught(int species) => false;
    public virtual void SetCaught(int species, bool caught) { }
    public int SeenCount => HasPokeDex ? Enumerable.Range(1, MaxSpeciesID).Count(GetSeen) : 0;
    public int CaughtCount => HasPokeDex ? Enumerable.Range(1, MaxSpeciesID).Count(GetCaught) : 0;
    public decimal PercentSeen => (decimal) SeenCount / MaxSpeciesID;
    public decimal PercentCaught => (decimal)CaughtCount / MaxSpeciesID;
    #endregion

    public bool HasBox => Box > -1;
    public virtual int BoxSlotCount => 30;
    public virtual int BoxesUnlocked { get => -1; set { } }
    public virtual byte[] BoxFlags { get => Array.Empty<byte>(); set { } }
    public virtual int CurrentBox { get; set; }

    #region BoxData
    protected int Box { get; set; } = int.MinValue;

    public IList<PKM> BoxData
    {
        get
        {
            PKM[] data = new PKM[BoxCount * BoxSlotCount];
            for (int box = 0; box < BoxCount; box++)
                AddBoxData(data, box, box * BoxSlotCount);
            return data;
        }
        set
        {
            if (value.Count != BoxCount * BoxSlotCount)
                throw new ArgumentException($"Expected {BoxCount * BoxSlotCount}, got {value.Count}");

            for (int b = 0; b < BoxCount; b++)
                SetBoxData(value, b, b * BoxSlotCount);
        }
    }

    public int SetBoxData(IList<PKM> value, int box, int index = 0)
    {
        int skipped = 0;
        for (int slot = 0; slot < BoxSlotCount; slot++)
        {
            var flags = GetSlotFlags(box, slot);
            if (!flags.IsOverwriteProtected())
                SetBoxSlotAtIndex(value[index + slot], box, slot);
            else
                ++skipped;
        }

        return skipped;
    }

    public PKM[] GetBoxData(int box)
    {
        var data = new PKM[BoxSlotCount];
        AddBoxData(data, box, 0);
        return data;
    }

    public void AddBoxData(IList<PKM> data, int box, int index)
    {
        for (int slot = 0; slot < BoxSlotCount; slot++)
        {
            int i = slot + index;
            data[i] = GetBoxSlotAtIndex(box, slot);
        }
    }
    #endregion

    #region Storage Health & Metadata
    protected int[] TeamSlots = Array.Empty<int>();

    /// <summary>
    /// Slot indexes that are protected from overwriting.
    /// </summary>
    protected virtual IList<int>[] SlotPointers => new[] { TeamSlots };
    public virtual StorageSlotSource GetSlotFlags(int index) => StorageSlotSource.None;
    public StorageSlotSource GetSlotFlags(int box, int slot) => GetSlotFlags((box * BoxSlotCount) + slot);
    public bool IsSlotLocked(int box, int slot) => GetSlotFlags(box, slot).HasFlagFast(StorageSlotSource.Locked);
    public bool IsSlotLocked(int index) => GetSlotFlags(index).HasFlagFast(StorageSlotSource.Locked);
    public bool IsSlotOverwriteProtected(int box, int slot) => GetSlotFlags(box, slot).IsOverwriteProtected();
    public bool IsSlotOverwriteProtected(int index) => GetSlotFlags(index).IsOverwriteProtected();

    private const int StorageFullValue = -1;
    public bool IsStorageFull => NextOpenBoxSlot() == StorageFullValue;

    public int NextOpenBoxSlot(int lastKnownOccupied = -1)
    {
        var storage = BoxBuffer.AsSpan();
        int count = BoxSlotCount * BoxCount;
        for (int i = lastKnownOccupied + 1; i < count; i++)
        {
            int offset = GetBoxSlotOffset(i);
            if (!IsPKMPresent(storage[offset..]))
                return i;
        }
        return StorageFullValue;
    }

    protected virtual bool IsSlotSwapProtected(int box, int slot) => false;

    private bool IsRegionOverwriteProtected(int min, int max)
    {
        foreach (var arrays in SlotPointers)
        {
            foreach (int slotIndex in arrays)
            {
                if (!GetSlotFlags(slotIndex).IsOverwriteProtected())
                    continue;
                if (ArrayUtil.WithinRange(slotIndex, min, max))
                    return true;
            }
        }

        return false;
    }

    public bool IsAnySlotLockedInBox(int BoxStart, int BoxEnd)
    {
        foreach (var arrays in SlotPointers)
        {
            foreach (int slotIndex in arrays)
            {
                if (!GetSlotFlags(slotIndex).HasFlagFast(StorageSlotSource.Locked))
                    continue;
                if (ArrayUtil.WithinRange(slotIndex, BoxStart * BoxSlotCount, (BoxEnd + 1) * BoxSlotCount))
                    return true;
            }
        }
        return false;
    }
    #endregion

    #region Storage Offsets and Indexing
    public abstract int GetBoxOffset(int box);
    public int GetBoxSlotOffset(int box, int slot) => GetBoxOffset(box) + (slot * SIZE_BOXSLOT);
    public PKM GetBoxSlotAtIndex(int box, int slot) => GetBoxSlot(GetBoxSlotOffset(box, slot));

    public void GetBoxSlotFromIndex(int index, out int box, out int slot)
    {
        box = index / BoxSlotCount;
        if ((uint)box >= BoxCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        slot = index % BoxSlotCount;
    }

    public PKM GetBoxSlotAtIndex(int index)
    {
        GetBoxSlotFromIndex(index, out int box, out int slot);
        return GetBoxSlotAtIndex(box, slot);
    }

    public int GetBoxSlotOffset(int index)
    {
        GetBoxSlotFromIndex(index, out int box, out int slot);
        return GetBoxSlotOffset(box, slot);
    }

    public void SetBoxSlotAtIndex(PKM pk, int box, int slot, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
        => SetBoxSlot(pk, BoxBuffer, GetBoxSlotOffset(box, slot), trade, dex);

    public void SetBoxSlotAtIndex(PKM pk, int index, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
        => SetBoxSlot(pk, BoxBuffer, GetBoxSlotOffset(index), trade, dex);
    #endregion

    #region Storage Manipulations

    public bool MoveBox(int box, int insertBeforeBox)
    {
        if (box == insertBeforeBox) // no movement required
            return true;
        if ((uint)box >= BoxCount || (uint)insertBeforeBox >= BoxCount) // invalid box positions
            return false;

        MoveBox(box, insertBeforeBox, BoxBuffer);
        return true;
    }

    private void MoveBox(int box, int insertBeforeBox, byte[] storage)
    {
        int pos1 = BoxSlotCount * box;
        int pos2 = BoxSlotCount * insertBeforeBox;
        int min = Math.Min(pos1, pos2);
        int max = Math.Max(pos1, pos2);

        int len = BoxSlotCount * SIZE_BOXSLOT;
        byte[] boxdata = storage.Slice(GetBoxOffset(0), len * BoxCount); // get all boxes
        string[] boxNames = Get(GetBoxName, BoxCount);
        int[] boxWallpapers = Get(GetBoxWallpaper, BoxCount);

        static T[] Get<T>(Func<int, T> act, int count)
        {
            T[] result = new T[count];
            for (int i = 0; i < result.Length; i++)
                result[i] = act(i);
            return result;
        }

        min /= BoxSlotCount;
        max /= BoxSlotCount;

        // move all boxes within range to final spot
        for (int i = min, ctr = min; i < max; i++)
        {
            int b = insertBeforeBox; // if box is the moved box, move to insertion point, else move to unused box.
            if (i != box)
            {
                if (insertBeforeBox == ctr)
                    ++ctr;
                b = ctr++;
            }

            Buffer.BlockCopy(boxdata, len * i, storage, GetBoxOffset(b), len);
            SetBoxName(b, boxNames[i]);
            SetBoxWallpaper(b, boxWallpapers[i]);
        }

        SlotPointerUtil.UpdateMove(box, insertBeforeBox, BoxSlotCount, SlotPointers);
    }

    public bool SwapBox(int box1, int box2)
    {
        if (box1 == box2) // no movement required
            return true;
        if ((uint)box1 >= BoxCount || (uint)box2 >= BoxCount) // invalid box positions
            return false;

        if (!IsBoxAbleToMove(box1) || !IsBoxAbleToMove(box2))
            return false;

        SwapBox(box1, box2, BoxBuffer);
        return true;
    }

    private void SwapBox(int box1, int box2, Span<byte> boxData)
    {
        int b1o = GetBoxOffset(box1);
        int b2o = GetBoxOffset(box2);
        int len = BoxSlotCount * SIZE_BOXSLOT;
        Span<byte> b1 = stackalloc byte[len];
        boxData.Slice(b1o, len).CopyTo(b1);
        boxData.Slice(b2o, len).CopyTo(boxData[b1o..]);
        b1.CopyTo(boxData[b2o..]);

        // Name
        string b1n = GetBoxName(box1);
        SetBoxName(box1, GetBoxName(box2));
        SetBoxName(box2, b1n);

        // Wallpaper
        int b1w = GetBoxWallpaper(box1);
        SetBoxWallpaper(box1, GetBoxWallpaper(box2));
        SetBoxWallpaper(box2, b1w);

        // Pointers
        SlotPointerUtil.UpdateSwap(box1, box2, BoxSlotCount, SlotPointers);
    }

    private bool IsBoxAbleToMove(int box)
    {
        int min = BoxSlotCount * box;
        int max = min + BoxSlotCount;
        return !IsRegionOverwriteProtected(min, max);
    }

    /// <summary>
    /// Sorts all <see cref="PKM"/> present within the range specified by <see cref="BoxStart"/> and <see cref="BoxEnd"/> with the provied <see cref="sortMethod"/>.
    /// </summary>
    /// <param name="BoxStart">Starting box; if not provided, will iterate from the first box.</param>
    /// <param name="BoxEnd">Ending box; if not provided, will iterate to the end.</param>
    /// <param name="sortMethod">Sorting logic required to order a <see cref="PKM"/> with respect to its peers; if not provided, will use a default sorting method.</param>
    /// <param name="reverse">Reverse the sorting order</param>
    /// <returns>Count of repositioned <see cref="PKM"/> slots.</returns>
    public int SortBoxes(int BoxStart = 0, int BoxEnd = -1, Func<IEnumerable<PKM>, int, IEnumerable<PKM>>? sortMethod = null, bool reverse = false)
    {
        var BD = BoxData;
        int start = BoxSlotCount * BoxStart;
        var Section = BD.Skip(start);
        if (BoxEnd >= BoxStart)
            Section = Section.Take(BoxSlotCount * (BoxEnd - BoxStart + 1));

        Func<int, bool> skip = IsSlotOverwriteProtected;
        Section = Section.Where((_, i) => !skip(start + i));
        var method = sortMethod ?? ((z, _) => z.OrderBySpecies());
        var Sorted = method(Section, start);
        if (reverse)
            Sorted = Sorted.ReverseSort();

        var result = Sorted.ToArray();
        var boxclone = new PKM[BD.Count];
        BD.CopyTo(boxclone, 0);
        int count = result.CopyTo(boxclone, skip, start);

        SlotPointerUtil.UpdateRepointFrom(boxclone, BD, 0, SlotPointers);

        for (int i = 0; i < boxclone.Length; i++)
        {
            var pk = boxclone[i];
            SetBoxSlotAtIndex(pk, i, PKMImportSetting.Skip, PKMImportSetting.Skip);
        }
        return count;
    }

    /// <summary>
    /// Compresses the <see cref="BoxData"/> by pulling out the empty storage slots and putting them at the end, retaining all existing data.
    /// </summary>
    /// <param name="storedCount">Count of actual <see cref="PKM"/> stored.</param>
    /// <param name="slotPointers">Important slot pointers that need to be re-pointed if a slot moves.</param>
    /// <returns>True if <see cref="BoxData"/> was updated, false if no update done.</returns>
    public bool CompressStorage(out int storedCount, Span<int> slotPointers) => this.CompressStorage(BoxBuffer, out storedCount, slotPointers);

    /// <summary>
    /// Removes all <see cref="PKM"/> present within the range specified by <see cref="BoxStart"/> and <see cref="BoxEnd"/> if the provided <see cref="deleteCriteria"/> is satisfied.
    /// </summary>
    /// <param name="BoxStart">Starting box; if not provided, will iterate from the first box.</param>
    /// <param name="BoxEnd">Ending box; if not provided, will iterate to the end.</param>
    /// <param name="deleteCriteria">Criteria required to be satisfied for a <see cref="PKM"/> to be deleted; if not provided, will clear if possible.</param>
    /// <returns>Count of deleted <see cref="PKM"/> slots.</returns>
    public int ClearBoxes(int BoxStart = 0, int BoxEnd = -1, Func<PKM, bool>? deleteCriteria = null)
    {
        var storage = BoxBuffer.AsSpan();

        if ((uint)BoxEnd >= BoxCount)
            BoxEnd = BoxCount - 1;

        var blank = GetDataForBox(BlankPKM);
        int deleted = 0;
        for (int i = BoxStart; i <= BoxEnd; i++)
        {
            for (int p = 0; p < BoxSlotCount; p++)
            {
                if (IsSlotOverwriteProtected(i, p))
                    continue;
                var ofs = GetBoxSlotOffset(i, p);
                if (!IsPKMPresent(storage[ofs..]))
                    continue;
                if (deleteCriteria != null)
                {
                    var pk = GetBoxSlotAtIndex(i, p);
                    if (!deleteCriteria(pk))
                        continue;
                }

                SetData(storage, blank, ofs);
                ++deleted;
            }
        }
        return deleted;
    }

    /// <summary>
    /// Modifies all <see cref="PKM"/> present within the range specified by <see cref="BoxStart"/> and <see cref="BoxEnd"/> with the modification routine provided by <see cref="action"/>.
    /// </summary>
    /// <param name="action">Modification to perform on a <see cref="PKM"/></param>
    /// <param name="BoxStart">Starting box; if not provided, will iterate from the first box.</param>
    /// <param name="BoxEnd">Ending box (inclusive); if not provided, will iterate to the end.</param>
    /// <returns>Count of modified <see cref="PKM"/> slots.</returns>
    public int ModifyBoxes(Action<PKM> action, int BoxStart = 0, int BoxEnd = -1)
    {
        if ((uint)BoxEnd >= BoxCount)
            BoxEnd = BoxCount - 1;

        var storage = BoxBuffer.AsSpan();
        int modified = 0;
        for (int b = BoxStart; b <= BoxEnd; b++)
        {
            for (int s = 0; s < BoxSlotCount; s++)
            {
                if (IsSlotOverwriteProtected(b, s))
                    continue;
                var ofs = GetBoxSlotOffset(b, s);
                if (!IsPKMPresent(storage[ofs..]))
                    continue;
                var pk = GetBoxSlotAtIndex(b, s);
                action(pk);
                ++modified;
                SetBoxSlot(pk, storage, ofs, PKMImportSetting.Skip, PKMImportSetting.Skip);
            }
        }
        return modified;
    }
    #endregion

    #region Storage Name & Decoration
    public virtual bool HasBoxWallpapers => GetBoxWallpaperOffset(0) > -1;
    public virtual bool HasNamableBoxes => HasBoxWallpapers;

    public abstract string GetBoxName(int box);
    public abstract void SetBoxName(int box, string value);
    protected virtual int GetBoxWallpaperOffset(int box) => -1;

    public virtual int GetBoxWallpaper(int box)
    {
        int offset = GetBoxWallpaperOffset(box);
        if (offset < 0 || (uint)box > BoxCount)
            return box;
        return Data[offset];
    }

    public virtual void SetBoxWallpaper(int box, int value)
    {
        int offset = GetBoxWallpaperOffset(box);
        if (offset < 0 || (uint)box > BoxCount)
            return;
        Data[offset] = (byte)value;
    }
    #endregion

    #region Box Binaries
    public byte[] GetPCBinary() => BoxData.SelectMany(GetDataForBox).ToArray();
    public byte[] GetBoxBinary(int box) => GetBoxData(box).SelectMany(GetDataForBox).ToArray();

    public bool SetPCBinary(byte[] data)
    {
        if (IsRegionOverwriteProtected(0, SlotCount))
            return false;

        int expectLength = SlotCount * SIZE_BOXSLOT;
        return SetConcatenatedBinary(data, expectLength);
    }

    public bool SetBoxBinary(byte[] data, int box)
    {
        int start = box * BoxSlotCount;
        int end = start + BoxSlotCount;

        if (IsRegionOverwriteProtected(start, end))
            return false;

        int expectLength = BoxSlotCount * SIZE_BOXSLOT;
        return SetConcatenatedBinary(data, expectLength, start);
    }

    private bool SetConcatenatedBinary(byte[] data, int expectLength, int start = 0)
    {
        if (data.Length != expectLength)
            return false;

        var BD = BoxData;
        var entryLength = SIZE_BOXSLOT;
        var pkdata = ArrayUtil.EnumerateSplit(data, entryLength);

        pkdata.Select(GetPKM).CopyTo(BD, IsSlotOverwriteProtected, start);
        BoxData = BD;
        return true;
    }
    #endregion
}

public static class StorageUtil
{
    public static bool CompressStorage(this SaveFile sav, Span<byte> storage, out int storedCount, Span<int> slotPointers)
    {
        // keep track of empty slots, and only write them at the end if slots were shifted (no need otherwise).
        var empty = new List<byte[]>();
        bool shiftedSlots = false;

        ushort ctr = 0;
        int size = sav.SIZE_BOXSLOT;
        int count = sav.BoxSlotCount * sav.BoxCount;
        for (int i = 0; i < count; i++)
        {
            int offset = sav.GetBoxSlotOffset(i);
            if (sav.IsPKMPresent(storage[offset..]))
            {
                if (ctr != i) // copy required
                {
                    shiftedSlots = true; // appending empty slots afterwards is now required since a rewrite was done
                    int destOfs = sav.GetBoxSlotOffset(ctr);
                    storage[offset..(offset + size)].CopyTo(storage[destOfs..(destOfs + size)]);
                    SlotPointerUtil.UpdateRepointFrom(ctr, i, slotPointers);
                }

                ctr++;
                continue;
            }

            // pop out an empty slot; save all unused data & preserve order
            var data = storage.Slice(offset, size).ToArray();
            empty.Add(data);
        }

        storedCount = ctr;

        if (!shiftedSlots)
            return false;

        for (int i = ctr; i < count; i++)
        {
            var data = empty[i - ctr];
            int offset = sav.GetBoxSlotOffset(i);
            data.CopyTo(storage[offset..]);
        }

        return true;
    }
}
