using System;
using System.Collections.Generic;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Extra-data block for Battle Video records.
/// </summary>
/// <param name="Raw">Chunk of memory storing the structure.</param>
public sealed class BattleVideo5(Memory<byte> Raw) : IBattleVideo
{
    public const int SIZE = 0x18A4;
    public const string Extension = "bv5";

    private Span<byte> Data => Raw.Span;
    public bool IsUninitialized => !Data.ContainsAnyExcept<byte>(0xFF, 0);

    public bool IsDecrypted { get; set; }
    private Span<byte> CryptoData => Data[0xC4..0x18A0];

    public Span<byte> DecryptedChecksumRegion => CryptoData; // ccitt16 -> 0x18A0

    public byte Generation => 5;
    public IEnumerable<PKM> Contents => GetTeam(0).Concat(GetTeam(1)); // don't bother with multi-battles
    public static bool IsValid(ReadOnlySpan<byte> data) => data.Length == SIZE && ReadUInt16LittleEndian(data[^2..]) == 0;

    // Structure:
    // 0xC4 - 0x18A0: encrypted region
    // 0x18A0: u16 cryptoSeed ~~ inflate to 32 via seed |= (seed ^ 0xFFFF) << 16;
    // footer

    // Encrypted Region:
    // 0xC4... ???
    // 0x0CFC: u16 count Max, u16 count Present, (6 * sizeof(0x70)) pokemon -- 0x2A4
    // 0x0FA0: u16 count Max, u16 count Present, (6 * sizeof(0x70)) pokemon -- 0x2A4
    // 0x1244: u16 count Max, u16 count Present, (6 * sizeof(0x70)) pokemon -- 0x2A4
    // 0x14E8: u16 count Max, u16 count Present, (6 * sizeof(0x70)) pokemon -- 0x2A4

    // Assuming participant are "real" humans, and not NPCs:
    // 0x178C: 0x44 trainer data (u32 flag, char[8] OT, remainder idc)
    // 0x17D0: 0x44 trainer data (u32 flag, char[8] OT, remainder idc)
    // 0x1814: 0x44 trainer data (u32 flag, char[8] OT, remainder idc)
    // 0x1858: 0x44 trainer data (u32 flag, char[8] OT, remainder idc)
    // If a participant is an NPC, the flag is `2` and Trainer name is +0x10 from where it normally is.

    // 0x189C: 0xC bytes ???

    public const int SizeVideoPoke = 0x70;

    public string VideoName
    {
        get => StringConverter5.GetString(Data[..0x10]);
        set => StringConverter5.SetString(Data[..0x10], value, 8, 0, StringConverterOption.ClearZeroSafeTerminate);
    }

    // Other Data?

    public byte TrainerBirthMonth { get => Data[0x15]; set => Data[0x15] = value; }
    public byte TrainerAvatar { get => Data[0x16]; set => Data[0x16] = value; }
    public byte TrainerNation { get => Data[0x17]; set => Data[0x17] = value; }
    public byte TrainerRegion { get => Data[0x18]; set => Data[0x18] = value; }
    public ushort TrainerSpecies { get => ReadUInt16LittleEndian(Data[0x1C..]); set => WriteUInt16LittleEndian(Data[0x1C..], value); }

    public ushort Team1Species1 { get => ReadUInt16LittleEndian(Data[0x80..]); set => WriteUInt16LittleEndian(Data[0x80..], value); }
    public ushort Team1Species2 { get => ReadUInt16LittleEndian(Data[0x82..]); set => WriteUInt16LittleEndian(Data[0x82..], value); }
    public ushort Team1Species3 { get => ReadUInt16LittleEndian(Data[0x84..]); set => WriteUInt16LittleEndian(Data[0x84..], value); }
    public ushort Team1Species4 { get => ReadUInt16LittleEndian(Data[0x86..]); set => WriteUInt16LittleEndian(Data[0x86..], value); }
    public ushort Team1Species5 { get => ReadUInt16LittleEndian(Data[0x88..]); set => WriteUInt16LittleEndian(Data[0x88..], value); }
    public ushort Team1Species6 { get => ReadUInt16LittleEndian(Data[0x8A..]); set => WriteUInt16LittleEndian(Data[0x8A..], value); }

    public ushort Team2Species1 { get => ReadUInt16LittleEndian(Data[0x8C..]); set => WriteUInt16LittleEndian(Data[0x8C..], value); }
    public ushort Team2Species2 { get => ReadUInt16LittleEndian(Data[0x8E..]); set => WriteUInt16LittleEndian(Data[0x8E..], value); }
    public ushort Team2Species3 { get => ReadUInt16LittleEndian(Data[0x90..]); set => WriteUInt16LittleEndian(Data[0x90..], value); }
    public ushort Team2Species4 { get => ReadUInt16LittleEndian(Data[0x92..]); set => WriteUInt16LittleEndian(Data[0x92..], value); }
    public ushort Team2Species5 { get => ReadUInt16LittleEndian(Data[0x94..]); set => WriteUInt16LittleEndian(Data[0x94..], value); }
    public ushort Team2Species6 { get => ReadUInt16LittleEndian(Data[0x96..]); set => WriteUInt16LittleEndian(Data[0x96..], value); }

    public ushort BattleNumber { get => ReadUInt16LittleEndian(Data[0xA4..]); set => WriteUInt16LittleEndian(Data[0xA4..], value); }
    public byte BattleMode { get => Data[0xA6]; set => Data[0xA6] = value; }
    public byte BattleType { get => Data[0xA7]; set => Data[0xA7] = value; } // Launcher
    public ushort StartSentinel { get => ReadUInt16LittleEndian(Data[0xA8..]); set => WriteUInt16LittleEndian(Data[0xA8..], value); }
    public byte Boolean { get => Data[0xAA]; set => Data[0xAA] = value; } // 0 or 1
    public byte OneHundred { get => Data[0xAB]; set => Data[0xAB] = value; } // 100

    public ulong BattleVideoUploadId { get => ReadUInt64LittleEndian(Data[0xB8..]); set => WriteUInt64LittleEndian(Data[0xB8..], value); }

    // Encrypted Section
    public ulong RandomSeed { get => ReadUInt64LittleEndian(Data[0xC4..]); set => WriteUInt64LittleEndian(Data[0xC4..], value); }
    public ulong RandomMult { get => ReadUInt64LittleEndian(Data[0xCC..]); set => WriteUInt64LittleEndian(Data[0xCC..], value); }
    public ulong RandomAdd { get => ReadUInt64LittleEndian(Data[0xD4..]); set => WriteUInt64LittleEndian(Data[0xD4..], value); }
    public uint BattlePad1 { get => ReadUInt32LittleEndian(Data[0xDC..]); set => WriteUInt32LittleEndian(Data[0xDC..], value); }
    public uint BattlePad2 { get => ReadUInt32LittleEndian(Data[0xE0..]); set => WriteUInt32LittleEndian(Data[0xE0..], value); }

    private const int TeamCount = 4;
    private const int TeamLength = 0x2A4;
    public Span<byte> Team1 => Data.Slice(0x0CFC, TeamLength);
    public Span<byte> Team2 => Data.Slice(0x0FA0, TeamLength);
    public Span<byte> Team3 => Data.Slice(0x1244, TeamLength);
    public Span<byte> Team4 => Data.Slice(0x14E8, TeamLength);

    private const int TrainerLength = 0x44;
    public Span<byte> Trainer1 => Data.Slice(0x178C, TrainerLength); // + 0x10 something else
    public Span<byte> Trainer2 => Data.Slice(0x17D0, TrainerLength);
    public Span<byte> Trainer3 => Data.Slice(0x1814, TrainerLength);
    public Span<byte> Trainer4 => Data.Slice(0x1858, TrainerLength);

    // should always be 0xE281
    public ushort EndSentinel { get => ReadUInt16LittleEndian(Data[0x189C..]); set => WriteUInt16LittleEndian(Data[0x189C..], value); }
    public ushort Unused { get => ReadUInt16LittleEndian(Data[0x189E..]); set => WriteUInt16LittleEndian(Data[0x189E..], value); }

    /// <summary> <see cref="CalculateDecryptedChecksum"/> </summary>
    public ushort Seed { get => ReadUInt16LittleEndian(Data[0x18A0..]); set => WriteUInt16LittleEndian(Data[0x18A0..], value); }
    // 0000: ^ seed is truncated to u16.

    /// <summary> Decrypts the object. If already decrypted, does nothing. </summary>
    public void Decrypt() => SetDecryptedState(true);
    /// <summary> Encrypts the object. If already encrypted, does nothing. </summary>
    public void Encrypt() => SetDecryptedState(false);
    private void SetDecryptedState(bool state)
    {
        if (state == IsDecrypted)
            return;
        PokeCrypto.CryptArray(CryptoData, GetEncryptionSeed());
        IsDecrypted = state;
    }

    /// <summary>
    /// Same as Gen4!
    /// </summary>
    public uint GetEncryptionSeed()
    {
        uint seed = Seed;
        return seed | (~seed << 16);
    }

    private ushort CalculateDecryptedChecksum()
    {
        if (!IsDecrypted)
            throw new InvalidOperationException("Cannot calculate checksum when encrypted.");
        return Checksums.CRC16_CCITT(DecryptedChecksumRegion);
    }

    public void RefreshChecksums()
    {
        var state = IsDecrypted;
        Decrypt();
        Seed = CalculateDecryptedChecksum();
        Encrypt();
        SetDecryptedState(state);
    }

    #region Conversion

    public PK5[] GetTeam(int trainer)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)trainer, TeamLength);
        var span = GetTrainer(trainer);

        var state = IsDecrypted;
        Decrypt();
        int count = span[2];
        if (count > 6)
            count = 6;
        var result = new PK5[count];
        for (int i = 0; i < count; i++)
        {
            var ofs = 4 + (i * SizeVideoPoke);
            var segment = span.Slice(ofs, SizeVideoPoke);
            var entity = new PK5();
            InflateToPK5(segment, entity.Data);
            result[i] = entity;
        }
        SetDecryptedState(state);
        return result;
    }

    private Span<byte> GetTrainer(int trainer) => trainer switch
    {
        0 => Trainer1,
        1 => Trainer2,
        2 => Trainer3,
        3 => Trainer4,
        _ => throw new ArgumentOutOfRangeException(nameof(trainer)),
    };

    public string GetTrainerName(int trainer)
    {
        var state = IsDecrypted;
        Decrypt();
        var data = GetTrainer(trainer);
        uint flag = ReadUInt32LittleEndian(data);
        data = data[4..];
        if (flag != 1) // not human?
            data = data[0x10..];
        var buffer = data[..(7 * 2)];
        var result = !buffer.ContainsAnyExcept<byte>(0) ? string.Empty : StringConverter5.GetString(buffer);
        SetDecryptedState(state);
        return result;
    }

    /// <summary> <see cref="BattleVideo4.InflateToPK4"/> </summary>
    public static void InflateToPK5(ReadOnlySpan<byte> video, Span<byte> entity)
    {
        VerifySpanSizes(entity, video);

        video[..4].CopyTo(entity[..6]); // PID & Sanity -- skip checksum.
        video[6..0xA].CopyTo(entity[8..0xC]); // Species & Held Item
        // 10,11 unused alignment
        video[0xC..0x16].CopyTo(entity[0xC..0x16]); // OTID, Experience, Friendship, Ability
        // Skip PK4 Marking and Language
        video[0x16..0x1C].CopyTo(entity[0x18..0x1E]); // EVs
        // Skip PK4 contest stats and first u32 ribbon.
        video[0x1C..0x30].CopyTo(entity[0x28..0x3C]); // Moves & IVs
        // Skip second u32 ribbon.
        entity[0x40] = video[0x30]; // Fateful & OT Gender & Form
        // 0x31 alignment, skip shiny leaf & extended met location fields
        video[0x32..0x48].CopyTo(entity[0x48..0x5E]); // Nickname (0x16)
        // Skip Version and Ribbons
        video[0x48..0x58].CopyTo(entity[0x68..0x78]); // OT Name (0x10)
        entity[0x41] = (byte)(video[4] >> 3);
        // other bits are likely flags (N sparkle / pokestar?)

        // Ball
        entity[0x83] = video[0x58];
        // Language
        entity[0x17] = video[0x59];
        // u16 alignment

        // Battle Stats
        video[0x5C..0x70].CopyTo(entity[0x88..0x9C]);

        // We're still missing things like Version and Met Location, but this is all we can recover.
        // Recalculate checksum.
        ushort checksum = Checksums.Add16(entity[8..PokeCrypto.SIZE_5STORED]);
        WriteUInt16LittleEndian(entity[6..], checksum);
    }

    /// <summary> <see cref="BattleVideo4.DeflateFromPK4"/> </summary>
    public static void DeflateFromPK5(ReadOnlySpan<byte> entity, Span<byte> video)
    {
        VerifySpanSizes(entity, video);

        entity[..6].CopyTo(video[..6]); // PID & Sanity -- skip checksum.
        // 10,11 unused alignment
        entity[0xC..0x16].CopyTo(video[0xC..0x16]); // OTID, Experience, Friendship, Ability
        // Skip PK4 Marking and Language
        entity[0x18..0x1E].CopyTo(video[0x16..0x1C]); // EVs
        // Skip PK4 contest stats and first u32 ribbon.
        entity[0x28..0x3C].CopyTo(video[0x1C..0x30]); // Moves & IVs
        // Skip second u32 ribbon.
        video[0x30] = entity[0x40]; // Fateful & OT Gender & Form
        // 0x31 alignment, skip shiny leaf & extended met location fields
        entity[0x48..0x5E].CopyTo(video[0x32..0x48]); // Nickname (0x16)
        // Skip Version and Ribbons
        entity[0x68..0x78].CopyTo(video[0x48..0x58]); // OT Name (0x10)
        video[4] |= (byte)(entity[0x41] << 3);
        // other bits are likely flags (N sparkle / pokestar?)

        // Ball
        video[0x58] = entity[0x83];
        // Language
        video[0x59] = entity[0x17];
        // u16 alignment

        // Battle Stats
        entity[0x88..0x9C].CopyTo(video[0x5C..0x70]);
    }

    private static void VerifySpanSizes(ReadOnlySpan<byte> entity, ReadOnlySpan<byte> video)
    {
        if (video.Length < SizeVideoPoke)
            throw new ArgumentOutOfRangeException(nameof(video), "Video size is too small.");
        if (entity.Length < PokeCrypto.SIZE_5STORED)
            throw new ArgumentOutOfRangeException(nameof(entity), "Entity size is too small.");
    }
    #endregion

    public static bool GetIsDecrypted(ReadOnlySpan<byte> data)
    {
        if (data.Length < SIZE)
            return false;
        if (ReadUInt64LittleEndian(data[0xCC..]) != LCRNG64.Mult) // mul
            return false;
        if (ReadUInt64LittleEndian(data[0xD4..]) != LCRNG64.Add) // add
            return false;
        return true;
    }

    public string GetTrainerNames()
    {
        var state = IsDecrypted;
        var tr1 = GetTrainerName(0);
        var tr2 = GetTrainerName(1);
        var tr3 = GetTrainerName(2);
        var tr4 = GetTrainerName(3);
        SetDecryptedState(state);
        if (string.IsNullOrWhiteSpace(tr3))
            return $"{tr1} vs {tr2}";
        return $"{tr1} & {tr2} vs {tr3} & {tr4}";
    }
}
