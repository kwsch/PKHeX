using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 <see cref="SaveFile"/> object that reads from Pokémon Bank savedata (stored on AWS or locally if patched).
/// </summary>
public sealed class Bank7 : BulkStorage, IBoxDetailName
{
    /// <summary>
    /// Creates a new instance of the save file from the bankdata.bin.
    /// Upgrades Gen6 bank files to the most recent (EOL) format by appending everything needed.
    /// </summary>
    public static Bank7 GetBank7(Memory<byte> data)
    {
        if (data.Length == SaveUtil.SIZE_G7BANK_1)
            data = UpgradeFormatFrom1To2(data.Span);
        return new Bank7(data, typeof(PK7), BoxStart);
    }

    public Bank7(Memory<byte> data, Type t, [ConstantExpected] int start, int slotsPerBox = 30)
        : base(data, t, start, slotsPerBox) => Version = GameVersion.USUM;
    public override string PlayTimeString => $"{Year:00}-{Month:00}-{Day:00} {Hours:00}ː{Minutes:00}";
    protected internal override string ShortSummary => PlayTimeString;

    private const int GroupNameCount = 10;
    private const int GroupNameSize = 0x20;
    private const int BankNameSize = 0x22;
    private const int GroupNameSpacing = GroupNameSize + 2;
    private const int BankNameSpacing = BankNameSize + 4;

    // bankdata header.
    private const int BoxStart = 0x17C;

    // 100 Bank Boxes, followed by the Transfer Box and auxiliary metadata.
    private const int TransferBoxStart = 0xAAF14;
    private const int TransferBoxSlotCount = 30;
    private const int TransferBoxSize = TransferBoxSlotCount * 0xE8;

    // One format tag per Bank slot (100 boxes * 30 slots).
    private const int BankFormatTagsStart = 0xACA44;
    private const int BankFormatTagCount = 3000;

    // One format tag per Transfer Box slot.
    private const int TransferBoxFormatTagsStart = 0xAD5FC;
    private const int TransferBoxFormatTagCount = TransferBoxSlotCount;

    // Unused alignment/reserved bytes immediately following the Transfer Box tags.
    private const int ReservedStart = 0xAD61A;
    private const int ReservedSize = 2;

    // Eight source-game summaries, each 0x44 bytes.
    private const int SourceGameSummariesStart = 0xAD61C;
    private const int SourceGameSummaryCount = 8; // x/y, or/as, s/m, us/um
    private const int SourceGameSummarySize = BankSourceGameSummary.SIZE; // 0x44

    // Pokedex-like aggregate data.
    private const int PokedexDataStart = 0xAD83C;
    private const int PokedexDataSize = 0x7260;
    private static ReadOnlySpan<byte> PokedexMagic => "NKZT"u8; // Turtle ZuKaN

    // 4 bytes deposit/withdraw counters

    // One source-software ID per Bank slot.
    private const int SourceSoftwareIDsStart = 0xB4AA0;
    private const int SourceSoftwareIDCount = 3000;

    // One 64-bit update timestamp per Bank slot.
    private const int UpdateTimestampsStart = 0xB5658;
    private const int UpdateTimestampCount = 3000;
    private const int UpdateTimestampSize = 8;

    // Tail flags / reserved bytes.
    private const int TailStart = 0xBB418;
    private const int TailSize = 0x100;

    public override GameVersion Version { get => GameVersion.USUM; set { } }
    public override PersonalTable7 Personal => PersonalTable.USUM;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_SM;
    protected override PK7 GetPKM(Memory<byte> data) => new(data);
    protected override void DecryptPKM(Span<byte> data) => PokeCrypto.Decrypt67(data);
    protected override Bank7 CloneInternal() => new(Data.ToArray(), PKMType, BoxStart, SlotsPerBox);

    public override int BoxCount => BankCount + 1; // 100 + 1 transfer box

    /// <summary>
    /// Can be zero if offline-patched.
    /// </summary>
    public ulong NexUniqueID => ReadUInt64LittleEndian(Data);

    public string GetGroupName(int group)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)group, GroupNameCount);
        int offset = 0x8 + (GroupNameSpacing * group) + 2; // skip over " "
        return GetString(Data.Slice(offset, GroupNameSize / 2));
    }

    public BankRevision Revision
    {
        get => (BankRevision)ReadUInt16LittleEndian(Data[0x15C..]);
        set => WriteUInt16LittleEndian(Data[0x15C..], (ushort)value);
    }

    /// <summary>
    /// Total number of bank boxes available. Always 100.
    /// </summary>
    /// <remarks>
    /// The i=100 box is used to access the Transporter box.
    /// </remarks>
    public const int FixedBoxCount = 100;

    public ushort BankCount // always 100
    {
        get => ReadUInt16LittleEndian(Data[0x15E..]);
        set => WriteUInt16LittleEndian(Data[0x15E..], value);
    }

    private int Year => ReadUInt16LittleEndian(Data[0x160..]);
    private int Month => Data[0x162];
    private int Day => Data[0x163];
    private int Hours => Data[0x164];
    private int Minutes => Data[0x165];
    // unused 0x166-0x167

    public uint Unknown168 { get => ReadUInt32LittleEndian(Data[0x168..]); set => WriteUInt32LittleEndian(Data[0x168..], value); }

    /// <summary> Gift based on a limited time availability window. 3=Regis </summary>
    public uint LastGiftTimedID { get => ReadUInt32LittleEndian(Data[0x16C..]); set => WriteUInt32LittleEndian(Data[0x16C..], value); }

    /// <summary> PokéMiles Balance </summary>
    public uint Pokemiles { get => ReadUInt32LittleEndian(Data[0x170..]); set => WriteUInt32LittleEndian(Data[0x170..], value); }

    /// <summary> Battle Point (BP) Balance </summary>
    public uint BP { get => ReadUInt32LittleEndian(Data[0x174..]); set => WriteUInt32LittleEndian(Data[0x174..], Math.Min(ushort.MaxValue, value)); }
    public bool HasAcquiredFirstGift { get => Data[0x178] == 1; set => Data[0x178] = value ? (byte)1 : (byte)0; }
    public byte Flag2 { get => Data[0x179]; set => Data[0x179] = value; }
    public byte Flag3 { get => Data[0x17A]; set => Data[0x17A] = value; }

    /// <summary> Unlocks when OR/AS is connected. </summary>
    public byte IsAvailableDexORAS { get => Data[0x17B]; set => Data[0x17B] = value; }

    private int BoxDataSize => (SlotsPerBox * SIZE_STORED) + BankNameSpacing;
    public override int GetBoxOffset(int box)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan<uint>((uint)box, FixedBoxCount);
        return Box + (BoxDataSize * box);
    }

    public string GetBoxName(int box)
    {
        if (box == FixedBoxCount)
            return "Transfer";
        return GetString(GetBoxNameSpan(box));
    }

    private Span<byte> GetBoxNameSpan(int box) => Data.Slice(GetBoxNameOffset(box), BankNameSize);

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        if (box == FixedBoxCount)
            return;

        var span = GetBoxNameSpan(box);
        SetString(span, value, BankNameSize / 2, StringConverterOption.ClearZero);
    }

    public byte GetBoxWallpaper(int box)
    {
        if (box >= 100)
            return 0;
        return Data[GetBoxNameOffset(box) + BankNameSize];
    }

    public byte GetBoxGroup(int box)
    {
        if (box >= 100)
            return 0;
        return Data[GetBoxNameOffset(box) + BankNameSize + 1];
    }

    public ushort GetBoxOrder(int box)
    {
        if (box == 100)
            return 0;
        return ReadUInt16LittleEndian(Data[(GetBoxNameOffset(box) + BankNameSize + 2)..]);
    }

    public int GetBoxNameOffset(int box) => GetBoxOffset(box) + (SlotsPerBox * SIZE_STORED);
    public int GetBoxIndex(int box) => ReadUInt16LittleEndian(Data[(GetBoxNameOffset(box) + BankNameSize)..]);

    // https://github.com/Wokann/bank_and_mover_offline_patch/blob/ae9560c8bcefb7223c4da749b8c184c6b324d302/bank/docs/code-analysis.md#bankobject-and-bankdata

    /// <summary>Raw bankdata header (0x000000-0x00017B).</summary>
    public Span<byte> Header => Data[..BoxStart];

    /// <summary>Raw Transfer Box slots (30 x 0xE8 bytes).</summary>
    public Span<byte> TransferBox => Data.Slice(TransferBoxStart, TransferBoxSize);

    /// <summary>One-byte format tag for each of the 3000 Bank slots.</summary>
    public Span<byte> BankFormatTags => Data.Slice(BankFormatTagsStart, BankFormatTagCount);

    /// <summary>One-byte format tag for each of the 30 Transfer Box slots.</summary>
    public Span<byte> TransferBoxFormatTags => Data.Slice(TransferBoxFormatTagsStart, TransferBoxFormatTagCount);

    /// <summary>Reserved/alignment bytes immediately following the Transfer Box tags.</summary>
    public Span<byte> Reserved => Data.Slice(ReservedStart, ReservedSize);

    /// <summary>Eight source-game summary records (8 x 0x44 bytes).</summary>
    public Span<byte> SourceGameSummaries => Data.Slice(SourceGameSummariesStart, SourceGameSummaryCount * SourceGameSummarySize);

    /// <summary>Pokedex-like aggregate data stored in the Bank file.</summary>
    public Span<byte> PokedexData => Data.Slice(PokedexDataStart, PokedexDataSize);

    /// <summary> Count of slots deposited into the Bank in the active session. At rest, this is a record of the last session's deposit count. </summary>
    public ushort CountDeposited
    {
        get => ReadUInt16LittleEndian(Data[0xB4A9C..]);
        set => WriteUInt16LittleEndian(Data[0xB4A9C..], value);
    }

    /// <summary> Count of slots withdrawn from the Bank in the active session. At rest, this is a record of the last session's withdrawal count. </summary>
    public ushort CountWithdrawn
    {
        get => ReadUInt16LittleEndian(Data[0xB4A9E..]);
        set => WriteUInt16LittleEndian(Data[0xB4A9E..], value);
    }

    /// <summary>One source-software ID for each Bank slot.</summary>
    public GameVersion GetSlotSoftware(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slot, SourceSoftwareIDCount);
        return (GameVersion)Data[SourceSoftwareIDsStart + slot];
    }

    public void SetSlotSoftware(int slot, GameVersion value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slot, SourceSoftwareIDCount);
        Data[SourceSoftwareIDsStart + slot] = (byte)value;
    }

    /// <summary>One 64-bit update timestamp for each Bank slot.</summary>
    public Span<byte> UpdateTimestamps => Data.Slice(UpdateTimestampsStart, UpdateTimestampCount * UpdateTimestampSize);

    /// <summary>Tail flags and reserved bytes at the end of the bankdata file.</summary>
    public Span<byte> Tail => Data.Slice(TailStart, TailSize);


    /// <summary> Unlocks when US/UM is connected. </summary>
    public byte IsAvailableDexUSUM { get => Tail[0]; set => Tail[0] = value; }

    /// <summary>Gets one Transfer Box stored-PKM slot span.</summary>
    public Span<byte> GetTransferBoxSlot(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slot, TransferBoxSlotCount);
        return TransferBox.Slice(slot * SIZE_STORED, SIZE_STORED);
    }

    /// <summary>Gets one Bank slot's one-byte format tag.</summary>
    public Span<byte> GetBankFormatTag(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slot, BankFormatTagCount);
        return BankFormatTags.Slice(slot, 1);
    }

    /// <summary>Gets one Transfer Box slot's one-byte format tag.</summary>
    public Span<byte> GetTransferBoxFormatTag(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slot, TransferBoxFormatTagCount);
        return TransferBoxFormatTags.Slice(slot, 1);
    }

    /// <summary>Gets one source-game summary record.</summary>
    public Span<byte> GetSourceGameSummary(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, SourceGameSummaryCount);
        return SourceGameSummaries.Slice(index * SourceGameSummarySize, SourceGameSummarySize);
    }

    /// <summary>Gets one Bank slot's 64-bit update timestamp span.</summary>
    public Span<byte> GetUpdateTimestamp(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slot, UpdateTimestampCount);
        return UpdateTimestamps.Slice(slot * UpdateTimestampSize, UpdateTimestampSize);
    }

    /// <summary>
    /// Upgrades a Gen6-era Bank dump to the latest format (supporting Gen7=>EOL).
    /// </summary>
    /// <param name="data">The input Bank7 format revision 1 data.</param>
    /// <returns>The upgraded Bank7 format revision 2 data.</returns>
    /// <exception cref="ArgumentException">Thrown when the input data is not a valid Bank7 format revision 1 file.</exception>
    public static byte[] UpgradeFormatFrom1To2(ReadOnlySpan<byte> data)
    {
        // Sanity check the input.
        if (data.Length != SaveUtil.SIZE_G7BANK_1)
            throw new ArgumentException($"Input data is not a valid Bank7 format revision 1 file. Expected length {SaveUtil.SIZE_G7BANK_1}, got {data.Length}.", nameof(data));
        // Box Count should be 100, and the revision should be 1.
        if (data.Length < 0x17C || ReadUInt16LittleEndian(data[0x15E..]) != 100 || ReadUInt16LittleEndian(data[0x15C..]) is not ((ushort)BankRevision.Gen6))
            throw new ArgumentException("Input data is not a valid Bank7 format revision 1 file.", nameof(data));

        // Upgrade revision to 2.
        var result = new byte[SaveUtil.SIZE_G7BANK_2];
        var span = result.AsSpan();
        data.CopyTo(span);

        var gameSummary = span[SourceGameSummariesStart..];
        for (int i = 0; i < SourceGameSummaryCount; i++)
        {
            // initialize 0x1A to 2 (genderless)
            gameSummary[(i * SourceGameSummarySize) + 0x1A] = 2;
        }

        // Initialize the Pokédex
        PokedexMagic.CopyTo(span[PokedexDataStart..]);
        span[0xB2814] = 1; // dex flag?
        return result;
    }

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        pk.HeldItem = 0;
        base.SetPKM(pk, isParty);
    }
}

public enum BankRevision : ushort
{
    None = 0,
    Gen6 = 1, // 2013~2016
    Gen7 = 2, // ~2017+
}

public ref struct BankSourceGameSummary(Span<byte> data)
{
    public const int SIZE = 0x44;
    public const int MaxStringLengthTrainer = 12;
    private readonly Span<byte> _data = data;
    public readonly Span<byte> TrainerNameTrash => _data[..((MaxStringLengthTrainer + 1) * 2)];

    public string TrainerName
    {
        readonly get => StringConverter7.GetString(TrainerNameTrash);
        set => StringConverter7.SetString(TrainerNameTrash, value, MaxStringLengthTrainer, 0); // should just copy trash from save file directly, Language is not stored
    }

    public byte Gender { readonly get => _data[0x1A]; set => _data[0x1A] = value; }
    // 0x1B unused
    public uint ID32 { readonly get => ReadUInt32LittleEndian(_data[0x1C..]); set => WriteUInt32LittleEndian(_data[0x1C..], value); }
    public ushort TID16 { readonly get => ReadUInt16LittleEndian(_data[0x1C..]); set => WriteUInt16LittleEndian(_data[0x1C..], value); }
    public ushort SID16 { readonly get => ReadUInt16LittleEndian(_data[0x1E..]); set => WriteUInt16LittleEndian(_data[0x1E..], value); }

    // 9 u32 records? @ 0x20-0x44
    public const int RecordCount = 9;
    public const uint RecordValueMax = 999_999_999u;
    private readonly Span<byte> GetRecordSpan(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, RecordCount);
        return _data[(0x20 + (index * 4))..];
    }

    public readonly uint GetRecord(int index) => ReadUInt32LittleEndian(GetRecordSpan(index));
    public void SetRecord(int index, uint value) => WriteUInt32LittleEndian(GetRecordSpan(index), Math.Min(RecordValueMax, value));

    public static ReadOnlySpan<GameVersion> Indexes =>
    [
        GameVersion.X, GameVersion.Y, GameVersion.OR, GameVersion.AS,
        GameVersion.SN, GameVersion.MN, GameVersion.US, GameVersion.UM,
    ];

    public void Clear()
    {
        _data.Clear();
        Gender = 2;
    }

    public readonly bool IsEmpty => Gender == 2;
}
