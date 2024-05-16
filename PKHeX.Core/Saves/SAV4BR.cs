using System;
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

    public SAV4BR() : base(SaveUtil.SIZE_G4BR)
    {
        ClearBoxes();
    }

    public SAV4BR(byte[] data) : base(data)
    {
        InitializeData(data);
    }

    private void InitializeData(ReadOnlySpan<byte> data)
    {
        Data = DecryptPBRSaveData(data);

        // Detect active save
        var first  = ReadUInt32BigEndian(Data.AsSpan(0x00004C));
        var second = ReadUInt32BigEndian(Data.AsSpan(0x1C004C));
        SaveCount = Math.Max(second, first);
        if (second > first)
        {
            // swap halves
            byte[] tempData = new byte[SIZE_HALF];
            Array.Copy(Data, 0, tempData, 0, SIZE_HALF);
            Array.Copy(Data, SIZE_HALF, Data, 0, SIZE_HALF);
            tempData.CopyTo(Data, SIZE_HALF);
        }

        var names = (string[]) SaveNames;
        for (int i = 0; i < SAVE_COUNT; i++)
        {
            var name = GetOTName(i);
            if (string.IsNullOrWhiteSpace(name))
                name = $"Empty {i + 1}";
            else if (_currentSlot == -1)
                _currentSlot = i;
            names[i] = name;
        }

        if (_currentSlot == -1)
            _currentSlot = 0;

        CurrentSlot = _currentSlot;
    }

    /// <summary> Amount of times the primary save has been saved </summary>
    private uint SaveCount;

    protected override byte[] GetFinalData()
    {
        SetChecksums();
        return EncryptPBRSaveData(Data);
    }

    // Configuration
    protected override SAV4BR CloneInternal() => new(Write());

    public readonly IReadOnlyList<string> SaveNames = new string[SAVE_COUNT];

    private int _currentSlot = -1;
    private const int SIZE_SLOT = 0x6FF00;

    public int CurrentSlot
    {
        get => _currentSlot;
        // 4 save slots, data reading depends on current slot
        set
        {
            _currentSlot = value;
            var ofs = SIZE_SLOT * _currentSlot;
            Box = ofs + 0x978;
            Party = ofs + 0x13A54; // first team slot after boxes
            BoxName = ofs + 0x58674;
        }
    }

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
    public override int Language => (int)LanguageID.English; // prevent KOR from inhabiting

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
        SetChecksum(Data, 0x0000000, 0x0000100, 0x000008);
        SetChecksum(Data, 0x0000000, SIZE_HALF, SIZE_HALF - 0x80);
        SetChecksum(Data, SIZE_HALF, 0x0000100, SIZE_HALF + 0x000008);
        SetChecksum(Data, SIZE_HALF, SIZE_HALF, SIZE_HALF + SIZE_HALF - 0x80);
    }

    public override bool ChecksumsValid => IsChecksumsValid(Data);
    public override string ChecksumInfo => $"Checksums valid: {ChecksumsValid}.";

    public static bool IsChecksumsValid(Span<byte> sav)
    {
        return VerifyChecksum(sav, 0x0000000, 0x0000100, 0x000008)
               && VerifyChecksum(sav, 0x0000000, SIZE_HALF, SIZE_HALF - 0x80)
               && VerifyChecksum(sav, SIZE_HALF, 0x0000100, SIZE_HALF + 0x000008)
               && VerifyChecksum(sav, SIZE_HALF, SIZE_HALF, SIZE_HALF + SIZE_HALF - 0x80);
    }

    // Trainer Info
    public override GameVersion Version { get => GameVersion.BATREV; set { } }

    private string GetOTName(int slot)
    {
        var ofs = 0x390 + (SIZE_SLOT * slot);
        var span = Data.AsSpan(ofs, 16);
        return GetString(span);
    }

    private void SetOTName(int slot, ReadOnlySpan<char> name)
    {
        var ofs = 0x390 + (SIZE_SLOT * slot);
        var span = Data.AsSpan(ofs, 16);
        SetString(span, name, 7, StringConverterOption.ClearZero);
    }

    public string CurrentOT { get => GetOTName(_currentSlot); set => SetOTName(_currentSlot, value); }

    // Storage
    public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
    public override int GetBoxOffset(int box) => Box + (SIZE_STORED * box * 30);

    public override uint Money
    {
        get => (uint)((Data[(_currentSlot * SIZE_SLOT) + 0x12861] << 16) | (Data[(_currentSlot * SIZE_SLOT) + 0x12862] << 8) | Data[(_currentSlot * SIZE_SLOT) + 0x12863]);
        set
        {
            Data[(_currentSlot * SIZE_SLOT) + 0x12861] = (byte)((value >> 16) & 0xFF);
            Data[(_currentSlot * SIZE_SLOT) + 0x12862] = (byte)((value >> 8) & 0xFF);
            Data[(_currentSlot * SIZE_SLOT) + 0x12863] = (byte)(value & 0xFF);
        }
    }

    public override ushort TID16
    {
        get => (ushort)((Data[(_currentSlot * SIZE_SLOT) + 0x12867] << 8) | Data[(_currentSlot * SIZE_SLOT) + 0x12860]);
        set
        {
            Data[(_currentSlot * SIZE_SLOT) + 0x12867] = (byte)(value >> 8);
            Data[(_currentSlot * SIZE_SLOT) + 0x12860] = (byte)(value & 0xFF);
        }
    }

    public override ushort SID16
    {
        get => (ushort)((Data[(_currentSlot * SIZE_SLOT) + 0x12865] << 8) | Data[(_currentSlot * SIZE_SLOT) + 0x12866]);
        set
        {
            Data[(_currentSlot * SIZE_SLOT) + 0x12865] = (byte)(value >> 8);
            Data[(_currentSlot * SIZE_SLOT) + 0x12866] = (byte)(value & 0xFF);
        }
    }

    // Save file does not have Box Name / Wallpaper info
    private int BoxName = -1;
    private const int BoxNameLength = 0x28;

    private Span<byte> GetBoxNameSpan(int box)
    {
        int ofs = BoxName + (box * BoxNameLength);
        return Data.AsSpan(ofs, BoxNameLength);
    }

    public string GetBoxName(int box)
    {
        if (BoxName < 0)
            return BoxDetailNameExtensions.GetDefaultBoxNameCaps(box);

        var span = GetBoxNameSpan(box);
        if (ReadUInt16BigEndian(span) == 0)
            return BoxDetailNameExtensions.GetDefaultBoxNameCaps(box);
        return GetString(span);
    }

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        if (BoxName < 0)
            return;

        var span = GetBoxNameSpan(box);
        if (ReadUInt16BigEndian(span) == 0)
            return;

        SetString(span, value, BoxNameLength / 2, StringConverterOption.ClearZero);
    }

    protected override BK4 GetPKM(byte[] data)
    {
        if (data.Length != SIZE_STORED)
            Array.Resize(ref data, SIZE_STORED);
        return BK4.ReadUnshuffle(data);
    }

    protected override byte[] DecryptPKM(byte[] data) => data;

    protected override void SetDex(PKM pk) { /* There's no PokéDex */ }

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

    public static byte[] DecryptPBRSaveData(ReadOnlySpan<byte> input)
    {
        byte[] output = new byte[input.Length];
        Span<ushort> keys = stackalloc ushort[4];
        for (int offset = 0; offset < SaveUtil.SIZE_G4BR; offset += SIZE_HALF)
        {
            var inSlice = input.Slice(offset, SIZE_HALF);
            var outSlice = output.AsSpan(offset, SIZE_HALF);

            // First 8 bytes are the encryption keys for this chunk.
            var keySlice = inSlice[..(keys.Length * 2)];
            GeniusCrypto.ReadKeys(keySlice, keys);

            // Copy over the keys to the result.
            keySlice.CopyTo(outSlice);

            // Decrypt the input, result stored in output.
            Range r = new(8, SIZE_HALF);
            GeniusCrypto.Decrypt(inSlice[r], outSlice[r], keys);
        }
        return output;
    }

    private static byte[] EncryptPBRSaveData(ReadOnlySpan<byte> input)
    {
        byte[] output = new byte[input.Length];
        Span<ushort> keys = stackalloc ushort[4];
        for (int offset = 0; offset < SaveUtil.SIZE_G4BR; offset += SIZE_HALF)
        {
            var inSlice = input.Slice(offset, SIZE_HALF);
            var outSlice = output.AsSpan(offset, SIZE_HALF);

            // First 8 bytes are the encryption keys for this chunk.
            var keySlice = inSlice[..(keys.Length * 2)];
            GeniusCrypto.ReadKeys(keySlice, keys);

            // Copy over the keys to the result.
            keySlice.CopyTo(outSlice);

            // Decrypt the input, result stored in output.
            Range r = new(8, SIZE_HALF);
            GeniusCrypto.Encrypt(inSlice[r], outSlice[r], keys);
        }
        return output;
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
