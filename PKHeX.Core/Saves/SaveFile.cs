using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Base Class for Save Files
/// </summary>
public abstract class SaveFile : ITrainerInfo, IGameValueLimit, IGeneration, IVersion, IStringConverter
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

    protected SaveFile([ConstantExpected] int size = 0) : this(size == 0 ? [] : new byte[size], false) { }

    protected internal abstract string ShortSummary { get; }
    public abstract string Extension { get; }

    protected abstract SaveFile CloneInternal();

    public SaveFile Clone()
    {
        var sav = CloneInternal();
        sav.Metadata = Metadata with {SAV = sav};
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
    public virtual bool IsVersionValid() => true;
    public abstract GameVersion Version { get; set; }
    public abstract bool ChecksumsValid { get; }
    public abstract string ChecksumInfo { get; }
    public abstract byte Generation { get; }
    public abstract EntityContext Context { get; }
    #endregion

    #region Savedata Container Handling
    public void SetData(ReadOnlySpan<byte> input, int offset) => SetData(Data.AsSpan(offset), input);

    public void SetData(Span<byte> dest, ReadOnlySpan<byte> input)
    {
        input.CopyTo(dest);
        State.Edited = true;
    }

    public abstract string GetString(ReadOnlySpan<byte> data);
    public abstract int LoadString(ReadOnlySpan<byte> data, Span<char> text);
    public abstract int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option);
    #endregion

    public virtual void CopyChangesFrom(SaveFile sav) => SetData(sav.Data, 0);

    // Offsets

    #region Stored PKM Limits
    public abstract IPersonalTable Personal { get; }
    public abstract int MaxStringLengthTrainer { get; }
    public abstract int MaxStringLengthNickname { get; }
    public abstract ushort MaxMoveID { get; }
    public abstract ushort MaxSpeciesID { get; }
    public abstract int MaxAbilityID { get; }
    public abstract int MaxItemID { get; }
    public abstract int MaxBallID { get; }
    public abstract GameVersion MaxGameID { get; }
    public virtual GameVersion MinGameID => 0;
    #endregion

    /// <summary>
    /// Gets the <see cref="bool"/> status of the Flag at the specified offset and index.
    /// </summary>
    /// <param name="offset">Offset to read from</param>
    /// <param name="bitIndex">Bit index to read</param>
    /// <returns>Flag is Set (true) or not Set (false)</returns>
    public virtual bool GetFlag(int offset, int bitIndex) => GetFlag(Data, offset, bitIndex);

    /// <summary>
    /// Sets the <see cref="bool"/> status of the Flag at the specified offset and index.
    /// </summary>
    /// <param name="offset">Offset to read from</param>
    /// <param name="bitIndex">Bit index to read</param>
    /// <param name="value">Flag status to set</param>
    /// <remarks>Flag is Set (true) or not Set (false)</remarks>
    public virtual void SetFlag(int offset, int bitIndex, bool value) => SetFlag(Data, offset, bitIndex, value);

    public bool GetFlag(Span<byte> data, int offset, int bitIndex) => FlagUtil.GetFlag(data, offset, bitIndex);
    public void SetFlag(Span<byte> data, int offset, int bitIndex, bool value)
    {
        FlagUtil.SetFlag(data, offset, bitIndex, value);
        State.Edited = true;
    }

    public virtual IReadOnlyList<InventoryPouch> Inventory { get => []; set { } }

    #region Player Info
    public virtual byte Gender { get; set; }
    public virtual int Language { get => -1; set { } }
    public virtual uint ID32 { get; set; }
    public virtual ushort TID16 { get; set; }
    public virtual ushort SID16 { get; set; }
    public virtual string OT { get; set; } = "PKHeX";
    public virtual int PlayedHours { get; set; }
    public virtual int PlayedMinutes { get; set; }
    public virtual int PlayedSeconds { get; set; }
    public virtual uint SecondsToStart { get; set; }
    public virtual uint SecondsToFame { get; set; }
    public virtual uint Money { get; set; }
    public abstract int BoxCount { get; }
    public virtual int SlotCount => BoxCount * BoxSlotCount;
    public virtual int MaxMoney => 9999999;
    public virtual int MaxCoins => 9999;

    public TrainerIDFormat TrainerIDDisplayFormat => this.GetTrainerIDFormat();
    public uint TrainerTID7 { get => this.GetTrainerTID7(); set => this.SetTrainerTID7(value); }
    public uint TrainerSID7 { get => this.GetTrainerSID7(); set => this.SetTrainerSID7(value); }
    public uint DisplayTID { get => this.GetDisplayTID(); set => this.SetDisplayTID(value); }
    public uint DisplaySID { get => this.GetDisplaySID(); set => this.SetDisplaySID(value); }

    #endregion

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
                data[i] = GetPartySlotAtIndex(i);
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
                SetPartySlotAtIndex(exist, ctr++);
            PartyCount = ctr;
            for (int i = ctr; i < 6; i++)
                SetPartySlotAtIndex(BlankPKM, i);
        }
    }
    #endregion

    // Varied Methods
    protected abstract void SetChecksums();

    private Span<byte> GetPartySpan(int index) => PartyBuffer[GetPartyOffset(index)..];
    public PKM GetPartySlotAtIndex(int index) => GetPartySlot(GetPartySpan(index));

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

        SetPartySlot(pk, GetPartySpan(index), trade, dex);
    }

    public void SetSlotFormatParty(PKM pk, Span<byte> data, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
    {
        if (pk.GetType() != PKMType)
            throw new ArgumentException($"PKM Format needs to be {PKMType} when setting to this Save File.");

        UpdatePKM(pk, isParty: true, trade, dex);
        SetPartyValues(pk, isParty: true);
        WritePartySlot(pk, data);
    }

    public void SetSlotFormatStored(PKM pk, Span<byte> data, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
    {
        if (pk.GetType() != PKMType)
            throw new ArgumentException($"PKM Format needs to be {PKMType} when setting to this Save File.");

        UpdatePKM(pk, isParty: false, trade, dex);
        SetPartyValues(pk, isParty: false);
        WriteSlotFormatStored(pk, data);
    }

    public void SetPartySlot(PKM pk, Span<byte> data, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault) => SetSlotFormatParty(pk, data, trade, dex);

    public void SetBoxSlot(PKM pk, Span<byte> data, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
    {
        if (pk.GetType() != PKMType)
            throw new ArgumentException($"PKM Format needs to be {PKMType} when setting to this Save File.");

        UpdatePKM(pk, isParty: false, trade, dex);
        SetPartyValues(pk, isParty: false);
        WriteBoxSlot(pk, data);
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
    public static PKMImportSetting SetUpdateRecords { protected get; set; } = PKMImportSetting.Update;

    public abstract Type PKMType { get; }
    protected abstract PKM GetPKM(byte[] data);
    protected abstract byte[] DecryptPKM(byte[] data);
    public abstract PKM BlankPKM { get; }
    protected abstract int SIZE_STORED { get; }
    protected abstract int SIZE_PARTY { get; }
    public virtual int SIZE_BOXSLOT => SIZE_STORED;
    public abstract int MaxEV { get; }
    public virtual int MaxIV => 31;
    public abstract ReadOnlySpan<ushort> HeldItems { get; }
    protected virtual Span<byte> BoxBuffer => Data;
    protected virtual Span<byte> PartyBuffer => Data;
    public virtual bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresent(data);
    public virtual PKM GetDecryptedPKM(byte[] data) => GetPKM(DecryptPKM(data));
    public virtual PKM GetPartySlot(ReadOnlySpan<byte> data) => GetDecryptedPKM(data[..SIZE_PARTY].ToArray());
    public virtual PKM GetStoredSlot(ReadOnlySpan<byte> data) => GetDecryptedPKM(data[..SIZE_STORED].ToArray());
    public virtual PKM GetBoxSlot(int offset) => GetStoredSlot(BoxBuffer[offset..]);

    public virtual byte[] GetDataForFormatStored(PKM pk) => pk.EncryptedBoxData;
    public virtual byte[] GetDataForFormatParty(PKM pk) => pk.EncryptedPartyData;
    public virtual byte[] GetDataForParty(PKM pk) => pk.EncryptedPartyData;
    public virtual byte[] GetDataForBox(PKM pk) => pk.EncryptedBoxData;

    public virtual void WriteSlotFormatStored(PKM pk, Span<byte> data) => SetData(data, GetDataForFormatStored(pk));
    public virtual void WriteSlotFormatParty(PKM pk, Span<byte> data) => SetData(data, GetDataForFormatParty(pk));
    public virtual void WritePartySlot(PKM pk, Span<byte> data) => SetData(data, GetDataForParty(pk));
    public virtual void WriteBoxSlot(PKM pk, Span<byte> data) => SetData(data, GetDataForBox(pk));

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
    /// <param name="trade">Setting on whether to adapt</param>
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
    public virtual bool HasPokeDex => false;
    public virtual bool GetSeen(ushort species) => false;
    public virtual void SetSeen(ushort species, bool seen) { }
    public virtual bool GetCaught(ushort species) => false;
    public virtual void SetCaught(ushort species, bool caught) { }
    public int SeenCount
    {
        get
        {
            int ctr = 0;
            for (ushort i = 1; i <= MaxSpeciesID; i++)
            {
                if (GetSeen(i))
                    ctr++;
            }
            return ctr;
        }
    }

    /// <summary> Count of unique Species Caught (Owned) </summary>
    public int CaughtCount
    {
        get
        {
            int ctr = 0;
            for (ushort i = 1; i <= MaxSpeciesID; i++)
            {
                if (GetCaught(i))
                    ctr++;
            }
            return ctr;
        }
    }
    public decimal PercentSeen => (decimal) SeenCount / MaxSpeciesID;
    public decimal PercentCaught => (decimal)CaughtCount / MaxSpeciesID;
    #endregion

    public bool HasBox => Box > -1;
    public virtual int BoxSlotCount => 30;
    public virtual int BoxesUnlocked { get => -1; set { } }
    public virtual byte[] BoxFlags { get => []; set { } }
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
            ArgumentOutOfRangeException.ThrowIfNotEqual(value.Count, BoxCount * BoxSlotCount);

            for (int b = 0; b < BoxCount; b++)
                SetBoxData(value, b, b * BoxSlotCount);
        }
    }

    public int SetBoxData(IList<PKM> value, int box, int index = 0)
    {
        int skipped = 0;
        for (int slot = 0; slot < BoxSlotCount; slot++)
        {
            var flags = GetBoxSlotFlags(box, slot);
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
    protected int[] TeamSlots = [];

    /// <summary>
    /// Slot indexes that are protected from overwriting.
    /// </summary>
    protected virtual IList<int>[] SlotPointers => [ TeamSlots ];
    public virtual StorageSlotSource GetBoxSlotFlags(int index) => StorageSlotSource.None;
    public StorageSlotSource GetBoxSlotFlags(int box, int slot) => GetBoxSlotFlags((box * BoxSlotCount) + slot);
    public bool IsBoxSlotLocked(int box, int slot) => GetBoxSlotFlags(box, slot).HasFlag(StorageSlotSource.Locked);
    public bool IsBoxSlotLocked(int index) => GetBoxSlotFlags(index).HasFlag(StorageSlotSource.Locked);
    public bool IsBoxSlotOverwriteProtected(int box, int slot) => GetBoxSlotFlags(box, slot).IsOverwriteProtected();
    public bool IsBoxSlotOverwriteProtected(int index) => GetBoxSlotFlags(index).IsOverwriteProtected();

    private const int StorageFullValue = -1;
    public bool IsStorageFull => NextOpenBoxSlot() == StorageFullValue;

    public int NextOpenBoxSlot(int lastKnownOccupied = -1)
    {
        var storage = BoxBuffer;
        int count = SlotCount;
        for (int i = lastKnownOccupied + 1; i < count; i++)
        {
            int offset = GetBoxSlotOffset(i);
            // overwrite protect is only true if there is already data in slot
            if (IsPKMPresent(storage[offset..]))
                continue;
            return i;
        }
        return StorageFullValue;
    }

    protected virtual bool IsSlotSwapProtected(int box, int slot) => false;

    private bool IsRegionOverwriteProtected(int min, int max)
    {
        var ptrs = SlotPointers;
        if (ptrs.Length == 0)
            return false;

        foreach (var arrays in ptrs)
        {
            foreach (int slotIndex in arrays)
            {
                if (!GetBoxSlotFlags(slotIndex).IsOverwriteProtected())
                    continue;
                if (min <= slotIndex && slotIndex < max)
                    return true;
            }
        }

        return false;
    }

    public bool IsAnySlotLockedInBox(int BoxStart, int BoxEnd)
    {
        var ptrs = SlotPointers;
        if (ptrs.Length == 0)
            return false;

        var min = BoxStart * BoxSlotCount;
        var max = (BoxEnd + 1) * BoxSlotCount;

        foreach (var arrays in ptrs)
        {
            foreach (int slotIndex in arrays)
            {
                if (!GetBoxSlotFlags(slotIndex).HasFlag(StorageSlotSource.Locked))
                    continue;
                if (min <= slotIndex && slotIndex < max)
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
        => SetBoxSlot(pk, BoxBuffer[GetBoxSlotOffset(box, slot)..], trade, dex);

    public void SetBoxSlotAtIndex(PKM pk, int index, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
        => SetBoxSlot(pk, BoxBuffer[GetBoxSlotOffset(index)..], trade, dex);
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

    private void MoveBox(int box, int insertBeforeBox, Span<byte> storage)
    {
        int pos1 = BoxSlotCount * box;
        int pos2 = BoxSlotCount * insertBeforeBox;
        int min = Math.Min(pos1, pos2);
        int max = Math.Max(pos1, pos2);

        int len = BoxSlotCount * SIZE_BOXSLOT;
        byte[] boxdata = storage.Slice(GetBoxOffset(0), len * BoxCount).ToArray(); // get all boxes

        if (this is IBoxDetailWallpaper w)
            w.MoveWallpaper(box, insertBeforeBox);
        if (this is IBoxDetailName n)
            n.MoveBoxName(box, insertBeforeBox);

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

            boxdata.AsSpan(len * i, len).CopyTo(storage[GetBoxOffset(b)..]);
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
        if (this is IBoxDetailName n)
            n.SwapBoxName(box1, box2);

        // Wallpaper
        if (this is IBoxDetailWallpaper w)
            w.SwapWallpaper(box1, box2);

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

        Func<int, bool> skip = IsBoxSlotOverwriteProtected;
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
        var storage = BoxBuffer;

        if ((uint)BoxEnd >= BoxCount)
            BoxEnd = BoxCount - 1;

        var blank = GetDataForBox(BlankPKM);
        int deleted = 0;
        for (int i = BoxStart; i <= BoxEnd; i++)
        {
            for (int p = 0; p < BoxSlotCount; p++)
            {
                if (IsBoxSlotOverwriteProtected(i, p))
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

                SetData(storage[ofs..], blank);
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

        var storage = BoxBuffer;
        int modified = 0;
        for (int b = BoxStart; b <= BoxEnd; b++)
        {
            for (int s = 0; s < BoxSlotCount; s++)
            {
                if (IsBoxSlotOverwriteProtected(b, s))
                    continue;
                var ofs = GetBoxSlotOffset(b, s);
                var dest = storage[ofs..];
                if (!IsPKMPresent(dest))
                    continue;
                var pk = GetBoxSlotAtIndex(b, s);
                action(pk);
                ++modified;
                SetBoxSlot(pk, dest, PKMImportSetting.Skip, PKMImportSetting.Skip);
            }
        }
        return modified;
    }
    #endregion

    #region Box Binaries
    public byte[] GetPCBinary() => BoxData.SelectMany(GetDataForBox).ToArray();
    public byte[] GetBoxBinary(int box) => GetBoxData(box).SelectMany(GetDataForBox).ToArray();

    public bool SetPCBinary(ReadOnlySpan<byte> data)
    {
        if (IsRegionOverwriteProtected(0, SlotCount))
            return false;

        int expectLength = SlotCount * SIZE_BOXSLOT;
        return SetConcatenatedBinary(data, expectLength);
    }

    public bool SetBoxBinary(ReadOnlySpan<byte> data, int box)
    {
        int start = box * BoxSlotCount;
        int end = start + BoxSlotCount;

        if (IsRegionOverwriteProtected(start, end))
            return false;

        int expectLength = BoxSlotCount * SIZE_BOXSLOT;
        return SetConcatenatedBinary(data, expectLength, start);
    }

    private bool SetConcatenatedBinary(ReadOnlySpan<byte> data, int expectLength, int start = 0)
    {
        if (data.Length != expectLength)
            return false;

        var entryLength = SIZE_BOXSLOT;
        for (int i = 0, ctr = start; i < data.Length; i += entryLength)
        {
            if (IsBoxSlotOverwriteProtected(ctr))
                continue;
            var src = data.Slice(i, entryLength);
            var arr = src.ToArray();
            var pk = GetPKM(arr);
            SetBoxSlotAtIndex(pk, ctr++);
        }
        return true;
    }
    #endregion
}
