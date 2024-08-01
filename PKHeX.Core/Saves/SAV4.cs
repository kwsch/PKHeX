using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 abstract <see cref="SaveFile"/> object.
/// </summary>
/// <remarks>
/// Storage data is stored in one contiguous block, and the remaining data is stored in another block.
/// </remarks>
public abstract class SAV4 : SaveFile, IEventFlag37, IDaycareStorage, IDaycareRandomState<uint>, IDaycareExperience, IDaycareEggState, IMysteryGiftStorageProvider
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {PlayTimeString}";
    public sealed override string Extension => ".sav";

    // Blocks & Offsets
    private const int PartitionSize = 0x40000;

    // SaveData is chunked into two pieces.
    private readonly Memory<byte> StorageBuffer;
    protected internal readonly Memory<byte> GeneralBuffer;
    protected Span<byte> Storage => StorageBuffer.Span;
    public Span<byte> General => GeneralBuffer.Span;
    protected sealed override Span<byte> BoxBuffer => Storage;
    protected sealed override Span<byte> PartyBuffer => General;

    private readonly Memory<byte> BackupStorageBuffer;
    private readonly Memory<byte> BackupGeneralBuffer;
    private Span<byte> BackupStorage => BackupStorageBuffer.Span;
    private Span<byte> BackupGeneral => BackupGeneralBuffer.Span;
    protected abstract IReadOnlyList<BlockInfo4> ExtraBlocks { get; }

    public abstract Zukan4 Dex { get; }
    public sealed override bool HasPokeDex => true;

    protected abstract int EventFlag { get; }
    protected abstract int EventWork { get; }
    public sealed override bool GetFlag(int offset, int bitIndex) => GetFlag(General, offset, bitIndex);
    public sealed override void SetFlag(int offset, int bitIndex, bool value) => SetFlag(General, offset, bitIndex, value);

    protected SAV4([ConstantExpected] int gSize, [ConstantExpected] int sSize)
    {
        GeneralBuffer = new byte[gSize];
        StorageBuffer = new byte[sSize];
        BackupGeneralBuffer = new byte[gSize];
        BackupStorageBuffer = new byte[sSize];
        ClearBoxes();
    }

    protected SAV4(byte[] data, [ConstantExpected] int gSize, [ConstantExpected] int sSize, [ConstantExpected] int sStart) : base(data)
    {
        var GeneralBlockPosition = GetActiveBlock(data, 0, gSize);
        var StorageBlockPosition = GetActiveBlock(data, sStart, sSize);

        var gbo = (GeneralBlockPosition == 0 ? 0 : PartitionSize);
        var sbo = (StorageBlockPosition == 0 ? 0 : PartitionSize) + sStart;
        GeneralBuffer = Data.AsMemory(gbo, gSize);
        StorageBuffer = Data.AsMemory(sbo, sSize);

        var gboBackup = (GeneralBlockPosition != 0 ? 0 : PartitionSize);
        var sboBackup = (StorageBlockPosition != 0 ? 0 : PartitionSize) + sStart;
        BackupGeneralBuffer = Data.AsMemory(gboBackup, gSize);
        BackupStorageBuffer = Data.AsMemory(sboBackup, sSize);
    }

    // Configuration
    protected sealed override SAV4 CloneInternal()
    {
        var sav = CloneInternal4();
        SetData(sav.General, General);
        SetData(sav.Storage, Storage);
        return sav;
    }

    protected abstract SAV4 CloneInternal4();

    public sealed override void CopyChangesFrom(SaveFile sav)
    {
        SetData(sav.Data, 0);
        var s4 = (SAV4)sav;
        SetData(General, s4.General);
        SetData(Storage, s4.Storage);
    }

    protected sealed override int SIZE_STORED => PokeCrypto.SIZE_4STORED;
    protected sealed override int SIZE_PARTY => PokeCrypto.SIZE_4PARTY;
    public sealed override PK4 BlankPKM => new();
    public sealed override Type PKMType => typeof(PK4);

    public sealed override int BoxCount => 18;
    public sealed override int MaxEV => EffortValues.Max255;
    public sealed override byte Generation => 4;
    public sealed override EntityContext Context => EntityContext.Gen4;
    public int EventFlagCount => 0xB60; // 2912
    public int EventWorkCount => (EventFlag - EventWork) >> 1;
    public sealed override int MaxStringLengthTrainer => 7;
    public sealed override int MaxStringLengthNickname => 10;
    public sealed override int MaxMoney => 999999;
    public sealed override int MaxCoins => 50_000;

    public sealed override ushort MaxMoveID => Legal.MaxMoveID_4;
    public sealed override ushort MaxSpeciesID => Legal.MaxSpeciesID_4;
    // MaxItemID
    public sealed override int MaxAbilityID => Legal.MaxAbilityID_4;
    public sealed override int MaxBallID => Legal.MaxBallID_4;
    public sealed override GameVersion MaxGameID => Legal.MaxGameID_4; // Colo/XD

    #region Checksums
    protected abstract int FooterSize { get; }
    private ushort CalcBlockChecksum(ReadOnlySpan<byte> data) => Checksums.CRC16_CCITT(data[..^FooterSize]);
    private static ushort GetBlockChecksumSaved(ReadOnlySpan<byte> data) => ReadUInt16LittleEndian(data[^2..]);
    private bool GetBlockChecksumValid(ReadOnlySpan<byte> data) => CalcBlockChecksum(data) == GetBlockChecksumSaved(data);

    public const uint MAGIC_JAPAN_INTL = 0x20060623;
    public const uint MAGIC_KOREAN = 0x20070903;
    public uint Magic { get => ReadUInt32LittleEndian(General[^8..^4]); set => SetMagics(value); }

    protected void SetMagics(uint magic)
    {
        WriteUInt32LittleEndian(General[^8..^4], magic);
        WriteUInt32LittleEndian(Storage[^8..^4], magic);
        if (ReadUInt32LittleEndian(BackupGeneral[^8..^4]) != 0xFFFFFFFF)
            WriteUInt32LittleEndian(BackupGeneral[^8..^4], magic);
        if (ReadUInt32LittleEndian(BackupStorage[^8..^4]) != 0xFFFFFFFF)
            WriteUInt32LittleEndian(BackupStorage[^8..^4], magic);
        ExtraBlocks.SetMagics(Data.AsSpan(), magic);
        ExtraBlocks.SetMagics(Data.AsSpan(PartitionSize..), magic);
    }

    protected sealed override void SetChecksums()
    {
        WriteUInt16LittleEndian(General[^2..], CalcBlockChecksum(General));
        WriteUInt16LittleEndian(Storage[^2..], CalcBlockChecksum(Storage));
        if (ReadUInt32LittleEndian(BackupGeneral[^8..^4]) != 0xFFFFFFFF)
            WriteUInt16LittleEndian(BackupGeneral[^2..], CalcBlockChecksum(BackupGeneral));
        if (ReadUInt32LittleEndian(BackupStorage[^8..^4]) != 0xFFFFFFFF)
            WriteUInt16LittleEndian(BackupStorage[^2..], CalcBlockChecksum(BackupStorage));
        ExtraBlocks.SetChecksums(Data.AsSpan());
        ExtraBlocks.SetChecksums(Data.AsSpan(PartitionSize..));
    }

    public sealed override bool ChecksumsValid
    {
        get
        {
            if (!GetBlockChecksumValid(General))
                return false;
            if (!GetBlockChecksumValid(Storage))
                return false;
            if (!ExtraBlocks.GetChecksumsValid(Data.AsSpan()))
                return false;
            if (!ExtraBlocks.GetChecksumsValid(Data.AsSpan(PartitionSize..)))
                return false;

            return true;
        }
    }

    public sealed override string ChecksumInfo
    {
        get
        {
            var list = new List<string>();
            if (!GetBlockChecksumValid(General))
                list.Add("Small block checksum is invalid");
            if (!GetBlockChecksumValid(Storage))
                list.Add("Large block checksum is invalid");
            if (!ExtraBlocks.GetChecksumsValid(Data.AsSpan()))
                list.Add(ExtraBlocks.GetChecksumInfo(Data.AsSpan()));
            if (!ExtraBlocks.GetChecksumsValid(Data.AsSpan(PartitionSize..)))
                list.Add(ExtraBlocks.GetChecksumInfo(Data.AsSpan(PartitionSize..)));

            return list.Count != 0 ? string.Join(Environment.NewLine, list) : "Checksums are valid.";
        }
    }

    private static int GetActiveBlock(ReadOnlySpan<byte> data, [ConstantExpected] int begin, [ConstantExpected] int length)
    {
        int offset = begin + length - 0x14;
        return SAV4BlockDetection.CompareFooters(data, offset, offset + PartitionSize);
    }

    private int GetActiveExtraBlock(BlockInfo4 block)
    {
        int index = (int)block.ID;

        // Hall of Fame
        if (index == 0)
            return SAV4BlockDetection.CompareExtra(Data, Data.AsSpan(PartitionSize), block);

        // Battle Hall/Battle Videos
        var KeyOffset = Extra;
        var KeyBackupOffset = Extra + (0x4 * (ExtraBlocks.Count - 1));
        var PreferOffset = Extra + (2 * 0x4 * (ExtraBlocks.Count - 1));
        var key = ReadUInt32LittleEndian(General[(KeyOffset + (0x4 * (index - 1)))..]);
        var keyBackup = ReadUInt32LittleEndian(General[(KeyBackupOffset + (0x4 * (index - 1)))..]);
        var prefer = General[PreferOffset + (index - 1)];
        return SAV4BlockDetection.CompareExtra(Data, Data.AsSpan(PartitionSize), block, key, keyBackup, prefer);
    }
    #endregion

    public BattleVideo4? GetBattleVideo(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, 4);
        index = 2 + index; // 2-5, skip HoF and Battle Hall
        if (ExtraBlocks.Count < index)
            return null; // not in D/P

        var block = ExtraBlocks[index];
        var active = GetActiveExtraBlock(block);
        return active == -1 ? null : new BattleVideo4(Data.AsMemory((active == 0 ? 0 : PartitionSize) + block.Offset, BattleVideo4.SIZE_USED));
    }

    public Hall4? GetHall()
    {
        if (ExtraBlocks.Count < 1)
            return null; // not in D/P

        var block = ExtraBlocks[1];
        var active = GetActiveExtraBlock(block);
        return active == -1 ? null : new Hall4(Data.AsMemory((active == 0 ? 0 : PartitionSize) + block.Offset, Hall4.SIZE_USED));
    }

    protected int WondercardFlags = int.MinValue;
    protected int AdventureInfo = int.MinValue;
    protected int Seal = int.MinValue;
    public int Geonet { get; protected set; } = int.MinValue;
    protected int Extra = int.MinValue;
    protected int Trainer1;
    public int GTS { get; protected set; } = int.MinValue;

    protected int FashionCase = int.MinValue;
    private int OFS_AccessoryMultiCount => FashionCase; // 4 bits each
    private int OFS_AccessorySingleCount => FashionCase + 0x20; // 1 bit each
    private int OFS_Backdrop => FashionCase + 0x28;

    protected int OFS_Chatter = int.MinValue;
    public Chatter4 Chatter => new(this, Data.AsMemory(OFS_Chatter));

    protected int OFS_Record = int.MinValue;
    public Record4 Records => new(this, Data.AsMemory(OFS_Record, Record4.GetSize(this)));

    // Storage
    public override int PartyCount
    {
        get => General[Party - 4];
        protected set => General[Party - 4] = (byte)value;
    }

    public sealed override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);

    #region Trainer Info
    public override string OT
    {
        get => GetString(General.Slice(Trainer1, 16));
        set => SetString(General.Slice(Trainer1, 16), value, MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    public override uint ID32
    {
        get => ReadUInt32LittleEndian(General[(Trainer1 + 0x10)..]);
        set => WriteUInt32LittleEndian(General[(Trainer1 + 0x10)..], value);
    }

    public override ushort TID16
    {
        get => ReadUInt16LittleEndian(General[(Trainer1 + 0x10)..]);
        set => WriteUInt16LittleEndian(General[(Trainer1 + 0x10)..], value);
    }

    public override ushort SID16
    {
        get => ReadUInt16LittleEndian(General[(Trainer1 + 0x12)..]);
        set => WriteUInt16LittleEndian(General[(Trainer1 + 0x12)..], value);
    }

    public override uint Money
    {
        get => ReadUInt32LittleEndian(General[(Trainer1 + 0x14)..]);
        set => WriteUInt32LittleEndian(General[(Trainer1 + 0x14)..], value);
    }

    public override byte Gender
    {
        get => General[Trainer1 + 0x18];
        set => General[Trainer1 + 0x18] = value;
    }

    public override int Language
    {
        get => General[Trainer1 + 0x19];
        set => General[Trainer1 + 0x19] = (byte)value;
    }

    public int Badges
    {
        get => General[Trainer1 + 0x1A];
        set { if (value < 0) return; General[Trainer1 + 0x1A] = (byte)value; }
    }

    public int Sprite
    {
        get => General[Trainer1 + 0x1B];
        set { if (value < 0) return; General[Trainer1 + 0x1B] = (byte)value; }
    }

    public uint Coin
    {
        get => ReadUInt16LittleEndian(General[(Trainer1 + 0x20)..]);
        set => WriteUInt16LittleEndian(General[(Trainer1 + 0x20)..], (ushort)value);
    }

    public override int PlayedHours
    {
        get => ReadUInt16LittleEndian(General[(Trainer1 + 0x22)..]);
        set => WriteUInt16LittleEndian(General[(Trainer1 + 0x22)..], (ushort)value);
    }

    public override int PlayedMinutes
    {
        get => General[Trainer1 + 0x24];
        set => General[Trainer1 + 0x24] = (byte)value;
    }

    public override int PlayedSeconds
    {
        get => General[Trainer1 + 0x25];
        set => General[Trainer1 + 0x25] = (byte)value;
    }

    public abstract int M { get; set; }
    public abstract int X { get; set; }
    public abstract int Y { get; set; }

    public string Rival
    {
        get => GetString(RivalTrash);
        set => SetString(RivalTrash, value, MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    public abstract Span<byte> RivalTrash { get; set; }

    public abstract int X2 { get; set; }
    public abstract int Y2 { get; set; }
    public abstract int Z { get; set; }

    public override uint SecondsToStart { get => ReadUInt32LittleEndian(General[(AdventureInfo + 0x34)..]); set => WriteUInt32LittleEndian(General[(AdventureInfo + 0x34)..], value); }
    public override uint SecondsToFame { get => ReadUInt32LittleEndian(General[(AdventureInfo + 0x3C)..]); set => WriteUInt32LittleEndian(General[(AdventureInfo + 0x3C)..], value); }

    public bool GeonetGlobalFlag { get => General[Geonet] != 0; set => General[Geonet] = (byte)(value ? 1 : 0); }

    public int Country
    {
        get => General[Geonet + 1];
        set { if (value < 0) return; General[Geonet + 1] = (byte)value; }
    }

    public int Region
    {
        get => General[Geonet + 2];
        set { if (value < 0) return; General[Geonet + 2] = (byte)value; }
    }
    #endregion

    protected sealed override PK4 GetPKM(byte[] data) => new(data);
    protected sealed override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray45(data);

    protected sealed override void SetPKM(PKM pk, bool isParty = false)
    {
        var pk4 = (PK4)pk;
        // Apply to this Save File
        pk4.UpdateHandler(this);
        pk.RefreshChecksum();
    }

    #region Daycare
    public int DaycareSlotCount => 2;
    private const int DaycareSlotSize = PokeCrypto.SIZE_4PARTY;
    protected abstract int DaycareOffset { get; }
    public Memory<byte> GetDaycareSlot(int slot) => GeneralBuffer.Slice(DaycareOffset + (slot * DaycareSlotSize), PokeCrypto.SIZE_4STORED);

    // EXP: Last 4 bytes of each slot
    public uint GetDaycareEXP(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, 2u);
        int ofs = DaycareOffset + DaycareSlotSize - 4 + (index * DaycareSlotSize);
        return ReadUInt32LittleEndian(General[ofs..]);
    }

    public void SetDaycareEXP(int index, uint value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, 2u);
        int ofs = DaycareOffset + DaycareSlotSize - 4 + (index * DaycareSlotSize);
        WriteUInt32LittleEndian(General[ofs..], value);
    }

    public bool IsDaycareOccupied(int index) => GetStoredSlot(GetDaycareSlot(index).Span).Species != 0;

    public void SetDaycareOccupied(int index, bool occupied)
    {
        if (occupied)
            return; // how would we even set this? just ignore and assume they'll set the slot data via other means.
        GetDaycareSlot(index).Span.Clear();
    }

    private int DaycareEnd => DaycareOffset + (2 * DaycareSlotSize);

    uint IDaycareRandomState<uint>.Seed
    {
        get => ReadUInt32LittleEndian(General[DaycareEnd..]);
        set => WriteUInt32LittleEndian(General[DaycareEnd..], value);
    }

    public byte DaycareStepCounter { get => General[DaycareEnd + 4]; set => General[DaycareEnd + 4] = value; }

    public bool IsEggAvailable
    {
        get => ((IDaycareRandomState<uint>)this).Seed != 0;
        set
        {
            if (!value)
                ((IDaycareRandomState<uint>)this).Seed = 0;
            else if (((IDaycareRandomState<uint>)this).Seed == 0)
                ((IDaycareRandomState<uint>)this).Seed = (uint)Util.Rand.Next(1, int.MaxValue);
        }
    }
    #endregion

    // Mystery Gift
    public bool IsMysteryGiftUnlocked { get => (General[72] & 1) == 1; set => General[72] = (byte)((General[72] & 0xFE) | (value ? 1 : 0)); }

    protected sealed override void SetDex(PKM pk) => Dex.SetDex(pk);
    public sealed override bool GetCaught(ushort species) => Dex.GetCaught(species);
    public sealed override bool GetSeen(ushort species) => Dex.GetSeen(species);

    public int DexUpgraded
    {
        get
        {
            switch (Version)
            {
                case GameVersion.DP:
                    if (General[0x1413] != 0) return 4;
                    if (General[0x1415] != 0) return 3;
                    if (General[0x1404] != 0) return 2;
                    if (General[0x1414] != 0) return 1;
                    return 0;
                case GameVersion.HGSS:
                    if (General[0x15ED] != 0) return 3;
                    if (General[0x15EF] != 0) return 2;
                    if (General[0x15EE] != 0 && (General[0x10D1] & 8) != 0) return 1;
                    return 0;
                case GameVersion.Pt:
                    if (General[0x1641] != 0) return 4;
                    if (General[0x1643] != 0) return 3;
                    if (General[0x1640] != 0) return 2;
                    if (General[0x1642] != 0) return 1;
                    return 0;
                default: return 0;
            }
        }
        set
        {
            switch (Version)
            {
                case GameVersion.DP:
                    General[0x1413] = value == 4 ? (byte)1 : (byte)0;
                    General[0x1415] = value >= 3 ? (byte)1 : (byte)0;
                    General[0x1404] = value >= 2 ? (byte)1 : (byte)0;
                    General[0x1414] = value >= 1 ? (byte)1 : (byte)0;
                    break;
                case GameVersion.HGSS:
                    General[0x15ED] = value == 3 ? (byte)1 : (byte)0;
                    General[0x15EF] = value >= 2 ? (byte)1 : (byte)0;
                    General[0x15EE] = value >= 1 ? (byte)1 : (byte)0;
                    General[0x10D1] = (byte)((General[0x10D1] & ~8) | (value >= 1 ? 8 : 0));
                    break;
                case GameVersion.Pt:
                    General[0x1641] = value == 4 ? (byte)1 : (byte)0;
                    General[0x1643] = value >= 3 ? (byte)1 : (byte)0;
                    General[0x1640] = value >= 2 ? (byte)1 : (byte)0;
                    General[0x1642] = value >= 1 ? (byte)1 : (byte)0;
                    break;
                default: return;
            }
        }
    }

    public sealed override string GetString(ReadOnlySpan<byte> data)
        => StringConverter4.GetString(data);
    public sealed override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter4.LoadString(data, destBuffer);
    public sealed override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter4.SetString(destBuffer, value, maxLength, Language, option);

    #region Event Flag/Event Work
    public bool GetEventFlag(int flagNumber)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to get ({flagNumber}) is greater than max ({EventFlagCount}).");
        return GetFlag(EventFlag + (flagNumber >> 3), flagNumber & 7);
    }

    public void SetEventFlag(int flagNumber, bool value)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to set ({flagNumber}) is greater than max ({EventFlagCount}).");
        SetFlag(EventFlag + (flagNumber >> 3), flagNumber & 7, value);
    }

    public ushort GetWork(int index) => ReadUInt16LittleEndian(General[(EventWork + (index * 2))..]);
    public void SetWork(int index, ushort value) => WriteUInt16LittleEndian(General[EventWork..][(index * 2)..], value);
    #endregion

    #region Seals
    public const byte SealMaxCount = 99;

    public Span<byte> GetSealCase() => General.Slice(Seal, (int)Seal4.MAX);
    public void SetSealCase(ReadOnlySpan<byte> value) => SetData(General.Slice(Seal, (int)Seal4.MAX), value);

    public byte GetSealCount(Seal4 id) => General[Seal + (int)id];
    public void SetSealCount(Seal4 id, byte count) => General[Seal + (int)id] = Math.Min(SealMaxCount, count);
    #endregion

    #region Accessories

    public byte GetAccessoryOwnedCount(Accessory4 accessory)
    {
        if (accessory.IsMultiple())
        {
            byte enumIdx = (byte)accessory;
            byte val = General[OFS_AccessoryMultiCount + (enumIdx / 2)];
            if (enumIdx % 2 == 0)
                return (byte)(val & 0x0F);
            return (byte)(val >> 4);
        }

        // Otherwise, it's a single-count accessory
        var flagIdx = accessory.GetSingleBitIndex();
        if (GetFlag(OFS_AccessorySingleCount + (flagIdx >> 3), flagIdx & 7))
            return 1;
        return 0;
    }

    public void SetAccessoryOwnedCount(Accessory4 accessory, byte count)
    {
        if (accessory.IsMultiple())
        {
            if (count > 9)
                count = 9;

            var enumIdx = (byte)accessory;
            var addr = OFS_AccessoryMultiCount + (enumIdx / 2);

            if (enumIdx % 2 == 0)
            {
                General[addr] &= 0xF0;  // Reset old count to 0
                General[addr] |= count; // Set new count
            }
            else
            {
                General[addr] &= 0x0F;  // Reset old count to 0
                General[addr] |= (byte)(count << 4); // Set new count
            }
        }
        else
        {
            var flagIdx = accessory.GetSingleBitIndex();
            SetFlag(OFS_AccessorySingleCount + (flagIdx >> 3), flagIdx & 7, count != 0);
        }

        State.Edited = true;
    }
    #endregion

    #region Backdrops
    public byte GetBackdropPosition(Backdrop4 backdrop)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)backdrop, (uint)Backdrop4.Unset);
        return General[OFS_Backdrop + (byte)backdrop];
    }

    public bool GetBackdropUnlocked(Backdrop4 backdrop) => GetBackdropPosition(backdrop) != (byte)Backdrop4.Unset;
    public void RemoveBackdrop(Backdrop4 backdrop) => SetBackdropPosition(backdrop, (byte)Backdrop4.Unset);

    /// <summary>
    /// Sets the position of a backdrop.
    /// </summary>
    /// <remarks>
    /// Every unlocked backdrop must have a different position.
    /// Use <see cref="RemoveBackdrop"/> to remove a backdrop.
    /// </remarks>
    public void SetBackdropPosition(Backdrop4 backdrop, byte position)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)backdrop, (uint)Backdrop4.Unset);
        General[OFS_Backdrop + (byte)backdrop] = Math.Min(position, (byte)Backdrop4.Unset);
        State.Edited = true;
    }
    #endregion

    public int GetMailOffset(int index)
    {
        int ofs = (index * Mail4.SIZE);
        return Version switch
        {
            GameVersion.DP => (ofs + 0x4BEC),
            GameVersion.Pt => (ofs + 0x4E80),
            _ => (ofs + 0x3FA8),
        };
    }

    public byte[] GetMailData(int ofs) => General.Slice(ofs, Mail4.SIZE).ToArray();

    public Mail4 GetMail(int mailIndex)
    {
        int ofs = GetMailOffset(mailIndex);
        return new Mail4(GetMailData(ofs), ofs);
    }

    public abstract uint SwarmSeed { get; set; }
    public abstract uint SwarmMaxCountModulo { get; }

    public uint SwarmIndex
    {
        get => SwarmSeed % SwarmMaxCountModulo;
        set
        {
            value %= SwarmMaxCountModulo;
            while (SwarmIndex != value)
                ++SwarmSeed;
        }
    }

    public abstract int BP { get; set; }
    public abstract BattleFrontierFacility4 MaxFacility { get; }

    public abstract MysteryBlock4 Mystery { get; }
    IMysteryGiftStorage IMysteryGiftStorageProvider.MysteryGiftStorage => Mystery;
}

public sealed class MysteryBlock4DP(SAV4DP sav, Memory<byte> raw) : MysteryBlock4(sav, raw)
{
    // 0x100 Flags
    // 11 u32 IsActive sentinels
    // 8 PGT
    // 3 PCD
    public const int Size = (MaxReceivedFlag / 8) + (SentinelCount * sizeof(uint)) + (MaxCountPGT * PGT.Size) + (MaxCountPCD * PCD.Size);
    protected override int CardStart => FlagStart + FlagRegionSize + (11 * sizeof(uint));
    private const int SentinelCount = 11;

    // reverse crc32 polynomial, nice!
    private const uint MysteryGiftDPSlotActive = 0xEDB88320;

    private Span<byte> SentinelSpan => Data.Slice(FlagStart + FlagRegionSize, 11 * sizeof(uint));

    public uint GetMysteryGiftReceivedSentinel(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, SentinelCount);
        return ReadUInt32LittleEndian(SentinelSpan[(index * sizeof(uint))..]);
    }

    public void SetMysteryGiftReceivedSentinel(int index, uint value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, SentinelCount);
        WriteUInt32LittleEndian(SentinelSpan[(index * sizeof(uint))..], value);
    }

    public override void SetMysteryGift(int index, PGT pgt)
    {
        base.SetMysteryGift(index, pgt);
        SetMysteryGiftReceivedSentinel(index, pgt.Empty ? 0 : MysteryGiftDPSlotActive);
    }

    public override void SetMysteryGift(int index, PCD pcd)
    {
        base.SetMysteryGift(index, pcd);
        SetMysteryGiftReceivedSentinel(index, pcd.Empty ? 0 : MysteryGiftDPSlotActive);
    }
}

public sealed class MysteryBlock4Pt(SAV4Pt sav, Memory<byte> raw) : MysteryBlock4(sav, raw)
{
    // 0x100 Flags
    // 8 PGT
    // 3 PCD
    public const int Size = (MaxReceivedFlag / 8) + (MaxCountPGT * PGT.Size) + (MaxCountPCD * PCD.Size);
    protected override int CardStart => FlagStart + FlagRegionSize;
}

public sealed class MysteryBlock4HGSS(SAV4HGSS sav, Memory<byte> raw) : MysteryBlock4(sav, raw)
{
    // 0x100 Flags
    // 8 PGT
    // 3 PCD0x100
    public const int Size = (MaxReceivedFlag / 8) + (MaxCountPGT * PGT.Size) + (MaxCountPCD * PCD.Size);
    protected override int CardStart => FlagStart + FlagRegionSize;
}

public abstract class MysteryBlock4(SAV4 sav, Memory<byte> raw) : SaveBlock<SAV4>(sav, raw), IMysteryGiftStorage, IMysteryGiftFlags
{
    protected const int FlagStart = 0;
    protected const int MaxReceivedFlag = 2048;
    protected const int MaxCountPGT = 8;
    protected const int MaxCountPCD = 3;
    protected const int MaxCardsPresent = MaxCountPGT + MaxCountPCD;
    protected const int FlagRegionSize = (MaxReceivedFlag / 8); // 0x100
    protected abstract int CardStart { get; }
    private const int FlagDeliveryManActive = 2047;

    private int CardRegionPGTStart => CardStart;
    private int CardRegionPCDStart => CardRegionPGTStart + (MaxCountPGT * PGT.Size);

    public bool IsDeliveryManActive
    {
        get => GetMysteryGiftReceivedFlag(FlagDeliveryManActive);
        set
        {
            if (value && !SAV.IsMysteryGiftUnlocked)
                SAV.IsMysteryGiftUnlocked = true; // be nice to the user and unlock the Mystery Gift menu feature.
            SetMysteryGiftReceivedFlag(FlagDeliveryManActive, value);
        }
    }

    public int GiftCountMax => MaxCardsPresent;
    DataMysteryGift IMysteryGiftStorage.GetMysteryGift(int index)
    {
        if ((uint)index < MaxCountPGT)
            return GetMysteryGiftPGT(index);
        if ((uint)index < MaxCardsPresent)
            return GetMysteryGiftPCD(index - MaxCountPGT);
        throw new ArgumentOutOfRangeException(nameof(index));
    }

    void IMysteryGiftStorage.SetMysteryGift(int index, DataMysteryGift gift)
    {
        if ((uint) index < MaxCountPGT)
            SetMysteryGift(index, (PGT)gift);
        else if ((uint)index < MaxCardsPresent)
            SetMysteryGift(index - MaxCountPGT, (PCD)gift);
        else throw new ArgumentOutOfRangeException(nameof(index));
    }

    public int MysteryGiftReceivedFlagMax => FlagDeliveryManActive; // ignore the delivery man flag when populating flags
    private Span<byte> FlagRegion => Data[..CardStart]; // 0x100

    public void ClearReceivedFlags() => FlagRegion.Clear();

    private int GetGiftOffsetPGT(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxCountPGT);
        return CardRegionPGTStart + (index * PGT.Size);
    }

    private int GetGiftOffsetPCD(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxCountPCD);
        return CardRegionPCDStart + (index * PCD.Size);
    }

    private Span<byte> GetCardSpanPGT(int index) => Data.Slice(GetGiftOffsetPGT(index), PGT.Size);
    private Span<byte> GetCardSpanPCD(int index) => Data.Slice(GetGiftOffsetPCD(index), PCD.Size);
    public PGT GetMysteryGiftPGT(int index) => new(GetCardSpanPGT(index).ToArray());
    public PCD GetMysteryGiftPCD(int index) => new(GetCardSpanPCD(index).ToArray());

    public virtual void SetMysteryGift(int index, PGT pgt)
    {
        if ((uint)index > MaxCardsPresent)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (pgt.Data.Length != PGT.Size)
            throw new InvalidCastException(nameof(pgt));
        pgt.VerifyPKEncryption();
        SAV.SetData(GetCardSpanPGT(index), pgt.Data);
    }

    public virtual void SetMysteryGift(int index, PCD pcd)
    {
        if ((uint)index > MaxCardsPresent)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (pcd.Data.Length != PCD.Size)
            throw new InvalidCastException(nameof(pcd));
        var gift = pcd.Gift;
        if (gift.VerifyPKEncryption())
            pcd.Gift = gift; // ensure data is encrypted in the object
        SAV.SetData(GetCardSpanPCD(index), pcd.Data);
    }

    public bool GetMysteryGiftReceivedFlag(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxReceivedFlag);
        return FlagUtil.GetFlag(Data, index); // offset 0
    }

    public void SetMysteryGiftReceivedFlag(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxReceivedFlag);
        FlagUtil.SetFlag(Data, index, value); // offset 0
    }

    // HG/SS 0,1,2,[3]; D/P/Pt 1,2,3,[4]
    private const byte NoPCDforPGT = 3; // 4 for DPPt; handle this manually.

    /// <summary>
    /// Each PGT points to a PCD slot if it is correlated.
    /// </summary>
    public static void UpdateSlotPGT(ReadOnlySpan<DataMysteryGift> value, bool hgss)
    {
        var arrPGT = value[..MaxCountPGT];
        var arrPCD = value.Slice(MaxCountPGT, MaxCountPCD);
        UpdateSlotPGT(hgss, arrPGT, arrPCD);
    }

    public static void UpdateSlotPGT(bool hgss, ReadOnlySpan<DataMysteryGift> arrPGT, ReadOnlySpan<DataMysteryGift> arrPCD)
    {
        foreach (var gift in arrPGT)
        {
            var pgt = (PGT)gift;
            if (pgt.CardType == 0) // empty
            {
                pgt.Slot = 0;
                continue;
            }

            var index = FindIndexPCD(pgt, arrPCD);
            if (!hgss)
                index++;
            pgt.Slot = index;
        }
    }

    private static byte FindIndexPCD(PGT pgt, ReadOnlySpan<DataMysteryGift> arrPCD)
    {
        for (byte i = 0; i < arrPCD.Length; i++)
        {
            var pcd = (PCD)arrPCD[i];
            // Check if data matches (except Slot @ 0x02)
            if (pcd.GiftEquals(pgt))
                return i;
        }
        return NoPCDforPGT;
    }
}
