using System;
using System.Buffers;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 <see cref="SaveFile"/> object for Pokémon Battle Revolution saves.
/// </summary>
public sealed class SAV4BR : SaveFile, IBoxDetailName
{
    protected internal override string ShortSummary => $"{Version} #{SaveCount:0000}";
    public override string Extension => string.Empty;
    public override PersonalTable4 Personal => PersonalTable.DP;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_DP;

    private const int SAVE_COUNT = 4;
    public const int SIZE_HALF = 0x1C0000;

    private const int SIZE_SLOT = 0x6FF00;

    /// <summary> Amount of times the primary save has been saved </summary>
    private readonly int SavePartition;
    private readonly uint SaveCount;
    private readonly Memory<byte> Container;

    public readonly IReadOnlyList<string> SaveNames = new string[SAVE_COUNT];
    private int _currentSlot = -1;

    public int CurrentSlot
    {
        get => _currentSlot;
        set => LoadSlot(_currentSlot = value);
    }

    public SAV4BR() : base(SIZE_SLOT) => ClearBoxes();

    public SAV4BR(Memory<byte> data, bool decrypt = true) : base(new byte[SIZE_SLOT])
    {
        Container = data;
        var span = data.Span;
        if (decrypt)
            Decrypt(span);

        (SavePartition, SaveCount) = DetectPartition(span);

        InitializeData();
    }

    private SAV4BR(SAV4BR other) : base(other.Data.ToArray())
    {
        Container = other.Container.ToArray();
        SavePartition = other.SavePartition;
        SaveCount = other.SaveCount;
        CurrentSlot = other.CurrentSlot;

        InitializeData();
    }

    private Span<byte> GetSlot(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slot, SAVE_COUNT);

        var ofs = (SavePartition * SIZE_HALF) + (slot * SIZE_SLOT);
        return Container.Span.Slice(ofs, SIZE_SLOT);
    }

    private void LoadSlot(int slot)
    {
        if (_currentSlot == slot)
            return;
        if (_currentSlot != -1)
            SaveSlot(_currentSlot);

        GetSlot(slot).CopyTo(Data);
    }

    private void SaveSlot(int slot) => Data.CopyTo(GetSlot(slot));

    public static bool IsValidSaveFile(ReadOnlySpan<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, SaveUtil.SIZE_G4BR);

        var pool = ArrayPool<byte>.Shared;
        var rent = pool.Rent(SaveUtil.SIZE_G4BR);
        var span = rent.AsSpan(0, SaveUtil.SIZE_G4BR);
        try
        {
            data.CopyTo(span);
            Decrypt(span);
            var partition = DetectPartition(span);
            return partition.Partition != -1;
        }
        finally
        {
            pool.Return(rent);
        }
    }

    private static (int Partition, uint SaveCount) DetectPartition(Span<byte> data)
    {
        var first = ReadUInt32BigEndian(data[0x00004C..]);
        var second = ReadUInt32BigEndian(data[(SIZE_HALF + 0x00004C)..]);
        var firstValid = IsChecksumValid(data, 0x0000000);
        var secondValid = IsChecksumValid(data, SIZE_HALF);
        if (secondValid && (!firstValid || second > first))
            return (1, second);
        if (firstValid)
            return (0, first);
        return (-1, 0);
    }

    private void InitializeData()
    {
        var names = (string[]) SaveNames;
        for (int i = 0; i < SAVE_COUNT; i++)
        {
            var name = GetOTName(i);
            if (string.IsNullOrWhiteSpace(name))
                name = $"Empty {i + 1}";
            else if (CurrentSlot == -1)
                CurrentSlot = i;
            names[i] = name;
        }

        // Ensure one slot is loaded
        if (CurrentSlot == -1)
            CurrentSlot = 0;
    }

    protected override Memory<byte> GetFinalData()
    {
        SaveSlot(CurrentSlot);
        SetChecksums();
        var result = Container.ToArray();
        Encrypt(result);
        return result;
    }

    // Configuration
    protected override SAV4BR CloneInternal() => new(this);

    protected override int SIZE_STORED => PokeCrypto.SIZE_4STORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_4STORED + 4;
    public override BK4 BlankPKM => new();
    public override Type PKMType => typeof(BK4);

    public override ushort MaxMoveID => 467;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_4;
    public override int MaxAbilityID => Legal.MaxAbilityID_4;
    public override int MaxItemID => Legal.MaxItemID_4_HGSS;
    public override int MaxBallID => Legal.MaxBallID_4;
    public override GameVersion MaxGameID => Legal.MaxGameID_4;

    public override int MaxEV => EffortValues.Max255;
    public override byte Generation => 4;
    public override EntityContext Context => EntityContext.Gen4;
    public override int MaxStringLengthTrainer => 7;
    public override int MaxStringLengthNickname => 10;
    public override int MaxMoney => 999999;

    public override int BoxCount => 18;

    public override int PartyCount
    {
        get
        {
            int ctr = 0;
            for (int i = 0; i < 6; i++)
            {
                if (Data[GetPartyOffset(i) + 4] != 0) // sanity
                    ctr++;
            }
            return ctr;
        }
        protected set
        {
            // Ignore, value is calculated
        }
    }

    // Checksums
    protected override void SetChecksums()
    {
        var span = Container.Span[(SavePartition * SIZE_HALF)..];
        SetChecksum(span, 0x0000000, 0x0000100, 0x000008);
        SetChecksum(span, 0x0000000, SIZE_HALF, SIZE_HALF - 0x80);
    }

    public override bool ChecksumsValid => IsChecksumsValid(Data);
    public override string ChecksumInfo => $"Checksums valid: {ChecksumsValid}.";

    public static bool IsChecksumsValid(Span<byte> sav) => IsChecksumValid(sav, 0x0000000) || IsChecksumValid(sav, SIZE_HALF);

    private static bool IsChecksumValid(Span<byte> sav, int offset)
    {
        return VerifyChecksum(sav, offset, 0x0000100, offset + 0x000008)
               && VerifyChecksum(sav, offset, SIZE_HALF, offset + SIZE_HALF - 0x80);
    }

    // Trainer Info
    public override GameVersion Version { get => GameVersion.BATREV; set { } }

    public bool Japanese { get => !FlagUtil.GetFlag(Data, 0x57, 0); set => FlagUtil.SetFlag(Data, 0x57, 0, !value); }
    public LanguageBR BRLanguage { get => (LanguageBR)Data[0x384]; set => Data[0x384] = (byte)(value); }
    public override int Language
    {
        get => (int)(BRLanguage == LanguageBR.JapaneseOrEnglish && Japanese ? LanguageID.Japanese : BRLanguage.ToLanguageID());
        set
        {
            Japanese = value == (int)LanguageID.Japanese;
            BRLanguage = ((LanguageID)value).ToLanguageBR();
        }
    }

    private TimeSpan PlayedSpan
    {
        get => TimeSpan.FromSeconds(ReadDoubleBigEndian(Data.Slice(0x388, 16)));
        set => WriteDoubleBigEndian(Data.Slice(0x388, 16), value.TotalSeconds);
    }

    public override int PlayedHours
    {
        get => (ushort)PlayedSpan.TotalHours;
        set { var time = PlayedSpan; PlayedSpan = time - TimeSpan.FromHours(time.TotalHours) + TimeSpan.FromHours(value); }
    }

    public override int PlayedMinutes
    {
        get => (byte)PlayedSpan.Minutes;
        set { var time = PlayedSpan; PlayedSpan = time - TimeSpan.FromMinutes(time.Minutes) + TimeSpan.FromMinutes(value); }
    }

    public override int PlayedSeconds
    {
        get => (byte)PlayedSpan.Seconds;
        set { var time = PlayedSpan; PlayedSpan = time - TimeSpan.FromSeconds(time.Seconds) + TimeSpan.FromSeconds(value); }
    }

    private Span<byte> GetOriginalTrainerSpan(int slot) => Container.Span.Slice((SIZE_SLOT * slot) + 0x390, 16);
    private string GetOTName(int slot) => GetString(GetOriginalTrainerSpan(slot));
    private void SetOTName(int slot, ReadOnlySpan<char> name) => SetString(GetOriginalTrainerSpan(slot), name, 7, StringConverterOption.ClearZero);
    public string CurrentOT { get => GetOTName(CurrentSlot); set => SetOTName(CurrentSlot, value); }

    // Storage
    public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
    public override int GetBoxOffset(int box) => Box + (SIZE_STORED * box * 30);

    public override uint Money
    {
        get => (uint)((Data[0x12861] << 16) | (Data[0x12862] << 8) | Data[0x12863]);
        set
        {
            Data[0x12861] = (byte)((value >> 16) & 0xFF);
            Data[0x12862] = (byte)((value >> 8) & 0xFF);
            Data[0x12863] = (byte)(value & 0xFF);
        }
    }

    public override ushort TID16
    {
        get => (ushort)((Data[0x12867] << 8) | Data[0x12860]);
        set
        {
            Data[0x12867] = (byte)(value >> 8);
            Data[0x12860] = (byte)(value & 0xFF);
        }
    }

    public override ushort SID16
    {
        get => (ushort)((Data[0x12865] << 8) | Data[0x12866]);
        set
        {
            Data[0x12865] = (byte)(value >> 8);
            Data[0x12866] = (byte)(value & 0xFF);
        }
    }

    // Save file does not have Wallpaper info
    private const int BoxName = 0x58674;
    private const int BoxNameLength = 0x28;

    private Span<byte> GetBoxNameSpan(int box)
    {
        int ofs = BoxName + (box * BoxNameLength);
        return Data.Slice(ofs, BoxNameLength);
    }

    public string GetBoxName(int box)
    {
        var span = GetBoxNameSpan(box);
        var result = GetString(span);
        if (result.Length == 0)
            result = BoxDetailNameExtensions.GetDefaultBoxNameCaps(box);
        return result;
    }

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        var span = GetBoxNameSpan(box);
        SetString(span, value, BoxNameLength / 2, StringConverterOption.ClearZero);
    }

    protected override BK4 GetPKM(byte[] data)
    {
        if (data.Length != SIZE_STORED)
            Array.Resize(ref data, SIZE_STORED);
        return BK4.ReadUnshuffle(data);
    }

    protected override byte[] DecryptPKM(byte[] data) => data;

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        var pk4 = (BK4)pk;
        // Apply to this Save File
        pk4.UpdateHandler(this);
        pk.RefreshChecksum();
    }

    protected override void SetPartyValues(PKM pk, bool isParty)
    {
        if (pk is G4PKM g4)
            g4.Sanity = isParty ? (ushort)0xC000 : (ushort)0x4000;
    }

    public static void Decrypt(Span<byte> input)
    {
        for (int offset = 0; offset < SaveUtil.SIZE_G4BR; offset += SIZE_HALF)
        {
            var inSlice = input.Slice(offset, SIZE_HALF);
            GeniusCrypto.Decrypt(inSlice, new Range(0, 8), new Range(8, SIZE_HALF));
        }
    }

    private static void Encrypt(Span<byte> input)
    {
        for (int offset = 0; offset < SaveUtil.SIZE_G4BR; offset += SIZE_HALF)
        {
            var inSlice = input.Slice(offset, SIZE_HALF);
            GeniusCrypto.Encrypt(inSlice, new Range(0, 8), new Range(8, SIZE_HALF));
        }
    }

    public static bool VerifyChecksum(Span<byte> input, int offset, int len, int chkOffset)
    {
        // Read original checksum data, and clear it for recomputing
        Span<uint> originalChecksums = stackalloc uint[16];
        var checkSpan = input.Slice(chkOffset, 4 * originalChecksums.Length);
        for (int i = 0; i < originalChecksums.Length; i++)
        {
            var chk = checkSpan.Slice(i * 4, 4);
            originalChecksums[i] = ReadUInt32BigEndian(chk);
        }
        checkSpan.Clear();

        // Compute current checksum of the specified span
        Span<uint> checksums = stackalloc uint[16];
        var span = input.Slice(offset, len);
        ComputeChecksums(span, checksums);

        // Restore original checksums
        WriteChecksums(checkSpan, originalChecksums);

        // Check if they match
        return checksums.SequenceEqual(originalChecksums);
    }

    private static void SetChecksum(Span<byte> input, int offset, int len, int chkOffset)
    {
        // Wipe Checksum region.
        var checkSpan = input.Slice(chkOffset, 4 * 16);
        checkSpan.Clear();

        // Compute current checksum of the specified span
        Span<uint> checksums = stackalloc uint[16];
        var span = input.Slice(offset, len);
        ComputeChecksums(span, checksums);

        WriteChecksums(checkSpan, checksums);
    }

    private static void WriteChecksums(Span<byte> span, Span<uint> checksums)
    {
        for (int i = 0; i < checksums.Length; i++)
        {
            var dest = span[(i * 4)..];
            WriteUInt32BigEndian(dest, checksums[i]);
        }
    }

    private static void ComputeChecksums(Span<byte> span, Span<uint> checksums)
    {
        for (int i = 0; i < span.Length; i += 2)
        {
            uint value = ReadUInt16BigEndian(span[i..]);
            for (int c = 0; c < checksums.Length; c++)
                checksums[c] += ((value >> c) & 1);
        }
    }

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter4GC.GetStringUnicode(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter4GC.LoadStringUnicode(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter4GC.SetStringUnicode(value, destBuffer, maxLength, option);
}
