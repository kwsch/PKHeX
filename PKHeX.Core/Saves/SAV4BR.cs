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

    private readonly int SavePartition;
    /// <summary> Amount of times the primary save has been saved </summary>
    private readonly uint SaveCount;
    private readonly Memory<byte> Container;

    public readonly IReadOnlyList<string> SaveNames = new string[SAVE_COUNT];
    private int _currentSlot = -1;

    public int CurrentSlot
    {
        get => _currentSlot;
        set
        {
            LoadSlot(value);
            _currentSlot = value;
        }
    }

    public SAV4BR() : base(SIZE_SLOT)
    {
        Container = new byte[SIZE_HALF];
        _currentSlot = 0;
        ClearBoxes();
    }

    public SAV4BR(Memory<byte> data, bool decrypt = true) : base(new byte[SIZE_SLOT])
    {
        Container = data;
        var span = data.Span;
        if (decrypt)
            Decrypt(span);

        (SavePartition, SaveCount) = DetectPartition(span);

        InitializeData();
    }

    private SAV4BR(SAV4BR other) : base(new byte[SIZE_SLOT])
    {
        Container = other.Container.ToArray();
        SavePartition = other.SavePartition;
        SaveCount = other.SaveCount;
        CurrentSlot = other.CurrentSlot;

        other.Data.CopyTo(Data);
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
            span.Clear();
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
    protected override int SIZE_PARTY => PokeCrypto.SIZE_4STORED + 84;
    public override BK4 BlankPKM => new();
    public override Type PKMType => typeof(BK4);

    public override ushort MaxMoveID => 467;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_4;
    public override int MaxAbilityID => Legal.MaxAbilityID_4;
    public override int MaxItemID => Legal.MaxItemID_4_HGSS;
    public override int MaxBallID => Legal.MaxBallID_4;
    public override GameVersion MaxGameID => GameVersion.BATREV;

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
                var ofs = GetPartyOffset(i);
                var span = Data[ofs..];
                if (IsPKMPresent(span))
                    ctr++;
            }
            return ctr;
        }
        protected set
        {
            // Ignore, value is calculated
        }
    }

    #region Checksums
    protected override void SetChecksums()
    {
        var span = Container.Span[(SavePartition * SIZE_HALF)..];
        SetChecksum(span, 0x0000000, 0x0000100, 0x000008);
        SetChecksum(span, 0x0000000, SIZE_HALF, SIZE_HALF - 0x80);
    }

    public override bool ChecksumsValid => IsChecksumsValid(Container.Span);
    public override string ChecksumInfo => $"Checksums valid: {ChecksumsValid}.";

    public static bool IsChecksumsValid(Span<byte> sav) => IsChecksumValid(sav, 0x0000000) || IsChecksumValid(sav, SIZE_HALF);

    private static bool IsChecksumValid(Span<byte> sav, int offset)
    {
        return VerifyChecksum(sav, offset, 0x0000100, offset + 0x000008)
               && VerifyChecksum(sav, offset, SIZE_HALF, offset + SIZE_HALF - 0x80);
    }
    #endregion

    #region Trainer Info
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

    private Span<byte> GetOriginalTrainerSpan(int slot) => (slot == CurrentSlot ? Data : GetSlot(slot)).Slice(0x390, 16);
    private string GetOTName(int slot) => GetString(GetOriginalTrainerSpan(slot));
    private void SetOTName(int slot, ReadOnlySpan<char> name) => SetString(GetOriginalTrainerSpan(slot), name, 7, StringConverterOption.ClearZero);
    public string CurrentOT { get => GetOTName(CurrentSlot); set => SetOTName(CurrentSlot, value); }

    public bool GearShinyGroudonOutfit { get => FlagUtil.GetFlag(Data, 0x434, 0); set => FlagUtil.SetFlag(Data, 0x434, 0, value); }
    public bool GearShinyLucarioOutfit { get => FlagUtil.GetFlag(Data, 0x434, 1); set => FlagUtil.SetFlag(Data, 0x434, 1, value); }
    public bool GearShinyElectivireOutfit { get => FlagUtil.GetFlag(Data, 0x434, 2); set => FlagUtil.SetFlag(Data, 0x434, 2, value); }
    public bool GearShinyKyogreOutfit { get => FlagUtil.GetFlag(Data, 0x434, 3); set => FlagUtil.SetFlag(Data, 0x434, 3, value); }
    public bool GearShinyRoseradeOutfit { get => FlagUtil.GetFlag(Data, 0x434, 4); set => FlagUtil.SetFlag(Data, 0x434, 4, value); }
    public bool GearShinyPachirisuOutfit { get => FlagUtil.GetFlag(Data, 0x434, 5); set => FlagUtil.SetFlag(Data, 0x434, 5, value); }

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

    /// <inheritdoc cref="SaveFile.TID16"/>
    /// <remarks>Copied from the connected DS game.</remarks>
    public override ushort TID16
    {
        get => (ushort)((Data[0x12867] << 8) | Data[0x12860]);
        set
        {
            Data[0x12867] = (byte)(value >> 8);
            Data[0x12860] = (byte)(value & 0xFF);
        }
    }

    /// <inheritdoc cref="SaveFile.SID16"/>
    /// <remarks>Copied from the connected DS game.</remarks>
    public override ushort SID16
    {
        get => (ushort)((Data[0x12865] << 8) | Data[0x12866]);
        set
        {
            Data[0x12865] = (byte)(value >> 8);
            Data[0x12866] = (byte)(value & 0xFF);
        }
    }

    public Span<byte> BirthMonthTrash => Data.Slice(0x3B0, 0x8);
    public string BirthMonth
    {
        get => StringConverter4GC.GetStringUnicodeBR(BirthMonthTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, BirthMonthTrash);
    }

    public Span<byte> BirthDayTrash => Data.Slice(0x3B8, 0x8);
    public string BirthDay
    {
        get => StringConverter4GC.GetStringUnicodeBR(BirthDayTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, BirthDayTrash);
    }

    public int Country { get => ReadUInt16BigEndian(Data[0x3C0..]); set => WriteUInt16BigEndian(Data[0x578..], (ushort)value); }
    public int Region { get => ReadUInt16BigEndian(Data[0x3C2..]); set => WriteUInt16BigEndian(Data[0x57A..], (ushort)value); }

    public Span<byte> SelfIntroductionTrash => Data.Slice(0x3C4, 0x6C);

    /// <summary>
    /// The self-introduction in the player's profile.
    /// </summary>
    /// <remarks>
    /// Western games prefix the text with \uFFFF\uF013 to indicate that
    /// it should be displayed with a proportional font in Japanese games.
    /// </remarks>
    public string SelfIntroduction
    {
        get => StringConverter4GC.GetStringUnicodeBR(SelfIntroductionTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, SelfIntroductionTrash);
    }

    public uint RecordTotalBattles
    {
        get => (uint)((Data[0x1286A] << 16) | (Data[0x1286B] << 8) | Data[0x12864]);
        set
        {
            Data[0x1286A] = (byte)((value >> 16) & 0xFF);
            Data[0x1286B] = (byte)((value >> 8) & 0xFF);
            Data[0x12864] = (byte)(value & 0xFF);
        }
    }

    public uint RecordColosseumBattles
    {
        get => (uint)((Data[0x1286F] << 16) | (Data[0x12868] << 8) | Data[0x12869]);
        set
        {
            Data[0x1286F] = (byte)((value >> 16) & 0xFF);
            Data[0x12868] = (byte)((value >> 8) & 0xFF);
            Data[0x12869] = (byte)(value & 0xFF);
        }
    }

    public uint RecordFreeBattles
    {
        get => (uint)((Data[0x1286C] << 16) | (Data[0x1286D] << 8) | Data[0x1286E]);
        set
        {
            Data[0x1286C] = (byte)((value >> 16) & 0xFF);
            Data[0x1286D] = (byte)((value >> 8) & 0xFF);
            Data[0x1286E] = (byte)(value & 0xFF);
        }
    }

    public uint RecordWiFiBattles
    {
        get => (uint)((Data[0x12871] << 16) | (Data[0x12872] << 8) | Data[0x12873]);
        set
        {
            Data[0x12871] = (byte)((value >> 16) & 0xFF);
            Data[0x12872] = (byte)((value >> 8) & 0xFF);
            Data[0x12873] = (byte)(value & 0xFF);
        }
    }

    public byte RecordGatewayColosseumClears { get => Data[0x12870]; set => Data[0x12870] = value; }
    public byte RecordMainStreetColosseumClears { get => Data[0x12877]; set => Data[0x12877] = value; }
    public byte RecordWaterfallColosseumClears { get => Data[0x12876]; set => Data[0x12876] = value; }
    public byte RecordNeonColosseumClears { get => Data[0x12875]; set => Data[0x12875] = value; }
    public byte RecordCrystalColosseumClears { get => Data[0x12874]; set => Data[0x12874] = value; }
    public byte RecordSunnyParkColosseumClears { get => Data[0x1287B]; set => Data[0x1287B] = value; }
    public byte RecordMagmaColosseumClears { get => Data[0x1287A]; set => Data[0x1287A] = value; }
    public byte RecordCourtyardColosseumClears { get => Data[0x12879]; set => Data[0x12879] = value; }
    public byte RecordSunsetColosseumClears { get => Data[0x12878]; set => Data[0x12878] = value; }
    public byte RecordStargazerColosseumClears { get => Data[0x1287F]; set => Data[0x1287F] = value; }

    public bool UnlockedGatewayColosseum { get => FlagUtil.GetFlag(Data, 0x12889, 4); set => FlagUtil.SetFlag(Data, 0x12889, 4, value); }
    public bool UnlockedMainStreetColosseum { get => FlagUtil.GetFlag(Data, 0x12889, 5); set => FlagUtil.SetFlag(Data, 0x12889, 5, value); }
    public bool UnlockedWaterfallColosseum { get => FlagUtil.GetFlag(Data, 0x12889, 6); set => FlagUtil.SetFlag(Data, 0x12889, 6, value); }
    public bool UnlockedNeonColosseum { get => FlagUtil.GetFlag(Data, 0x12889, 7); set => FlagUtil.SetFlag(Data, 0x12889, 7, value); }
    public bool UnlockedCrystalColosseum { get => FlagUtil.GetFlag(Data, 0x12888, 0); set => FlagUtil.SetFlag(Data, 0x12888, 0, value); }
    public bool UnlockedSunnyParkColosseum { get => FlagUtil.GetFlag(Data, 0x12888, 1); set => FlagUtil.SetFlag(Data, 0x12888, 1, value); }
    public bool UnlockedMagmaColosseum { get => FlagUtil.GetFlag(Data, 0x12888, 2); set => FlagUtil.SetFlag(Data, 0x12888, 2, value); }
    public bool UnlockedCourtyardColosseum { get => FlagUtil.GetFlag(Data, 0x12888, 3); set => FlagUtil.SetFlag(Data, 0x12888, 3, value); }
    public bool UnlockedSunsetColosseum { get => FlagUtil.GetFlag(Data, 0x12888, 4); set => FlagUtil.SetFlag(Data, 0x12888, 4, value); }
    public bool UnlockedStargazerColosseum { get => FlagUtil.GetFlag(Data, 0x12888, 5); set => FlagUtil.SetFlag(Data, 0x12888, 5, value); }
    public bool UnlockedPostGame { get => FlagUtil.GetFlag(Data, 0x12891, 6); set => FlagUtil.SetFlag(Data, 0x12891, 6, value); }

    /// <summary>
    /// Used to identify which save file created a given Battle Pass.
    /// </summary>
    /// <remarks>
    /// If the value in the save file is 0, a value is instead calculated for the saved Battle Pass.
    /// </remarks>
    public ulong PlayerID
    {
        get => (
            ((ulong)Data[0x128CA] << 56) | ((ulong)Data[0x128CC] << 48) | ((ulong)Data[0x128CE] << 40) | ((ulong)Data[0x128C0] << 32) |
            ((ulong)Data[0x128D2] << 24) | ((ulong)Data[0x128D4] << 16) | ((ulong)Data[0x128D6] << 8) | Data[0x128C8]
        );
        set
        {
            Data[0x128CA] = (byte)((value >> 56) & 0xFF);
            Data[0x128CC] = (byte)((value >> 48) & 0xFF);
            Data[0x128CE] = (byte)((value >> 40) & 0xFF);
            Data[0x128C0] = (byte)((value >> 32) & 0xFF);
            Data[0x128D2] = (byte)((value >> 24) & 0xFF);
            Data[0x128D4] = (byte)((value >> 16) & 0xFF);
            Data[0x128D6] = (byte)((value >> 8) & 0xFF);
            Data[0x128C8] = (byte)(value & 0xFF);
        }
    }
    #endregion

    #region Storage
    public override bool HasParty => true;
    public override int GetPartyOffset(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(slot, 6);
        return 0x44C + (SIZE_PARTY * slot);
    }

    public override bool HasBox => true;
    public override int GetBoxOffset(int box)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(box, BoxCount);
        return 0x978 + (SIZE_STORED * box * 30);
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

    /// <summary>
    /// Finds the location of a PKM in the party or boxes.
    /// </summary>
    /// <param name="pk">The PKM to find.</param>
    /// <returns>Where the PKM was found, or (255, 255) otherwise</returns>
    public (byte Box, byte Slot) FindSlot(PKM pk)
    {
        var party = PartyData;
        for (byte slot = 0; slot < PartyCount; slot++)
        {
            PKM other = party[slot];
            if (pk.PID == other.PID && pk.DecryptedBoxData.AsSpan().SequenceEqual(other.DecryptedBoxData))
                return (0, slot);
        }

        var boxes = BoxData;
        for (byte box = 0; box < BoxCount; box++)
        {
            for (byte slot = 0; slot < BoxSlotCount; slot++)
            {
                PKM other = boxes[(box * BoxSlotCount) + slot];
                if (pk.PID == other.PID && pk.DecryptedBoxData.AsSpan().SequenceEqual(other.DecryptedBoxData))
                    return (++box, slot);
            }
        }

        return (255, 255); // disappeared
    }
    #endregion

    public BattlePassAccessor BattlePasses => new(this);
    public GearUnlock GearUnlock => new(Buffer.Slice(0x584EC, GearUnlock.Size));

    #region Encryption
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
    #endregion

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter4GC.GetStringUnicodeBR(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter4GC.LoadStringUnicodeBR(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter4GC.SetStringUnicodeBR(value, destBuffer, maxLength, option);
}
