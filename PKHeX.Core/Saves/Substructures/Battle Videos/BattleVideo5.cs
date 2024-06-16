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
    private const int SIZE = 0x1900;
    private const int SIZE_FOOTER = 0x14;
    public const int SIZE_USED = SIZE + SIZE_FOOTER;
    //public const int SIZE_BLOCK = 0x2000;
    //bw is 0x2000 between each video, b2w2 is 0x1A00

    private Span<byte> Data => Raw.Span[..SIZE_USED];

    public bool IsDecrypted;
    private Span<byte> CryptoData => Data[0xC4..0x18A0];
    private Span<byte> Footer => Data.Slice(SIZE, SIZE_FOOTER);

    public Span<byte> DecryptedChecksumRegion => CryptoData; // ccitt16 -> 0x18A0
    public Span<byte> EncryptedChecksumRegion => Data[..0x18A4]; // ccitt16 -> 0x18A6
    public Span<byte> FooterChecksumRegion => Footer[..4]; // ccitt16 -> 0x12

    public byte Generation => 5;
    public IEnumerable<PKM> Contents => GetTeam(0).Concat(GetTeam(1)); // don't bother with multi-battles
    public static bool IsValid(ReadOnlySpan<byte> data) => data.Length == SIZE_USED && ReadUInt32LittleEndian(data[0x1908..]) == SIZE_USED;

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
    // 0x178C: 0x44 trainer data (u32 flag, char[8] OT, remainder idc)
    // 0x17D0: 0x44 trainer data (u32 flag, char[8] OT, remainder idc)
    // 0x1814: 0x44 trainer data (u32 flag, char[8] OT, remainder idc)
    // 0x1858: 0x44 trainer data (u32 flag, char[8] OT, remainder idc)
    // 0x189C: 0xC bytes ???

    public const int SizeVideoPoke = 0x70;

    public uint Key { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }

    /// <summary> <see cref="CalculateDecryptedChecksum"/> </summary>
    public ushort Seed { get => ReadUInt16LittleEndian(Data[0x18A0..]); set => WriteUInt16LittleEndian(Data[0x18A0..], value); }
    // 0000: ^ seed is truncated to u16.
    public ushort EncryptedCount { get => ReadUInt16LittleEndian(Data[0x18A4..]); set => WriteUInt16LittleEndian(Data[0x18A4..], value); }
    public ushort EncryptedChecksum { get => ReadUInt16LittleEndian(Data[0x18A6..]); set => WriteUInt16LittleEndian(Data[0x18A6..], value); }
    // 0xFF padding(?) to 0x1900

    public void Decrypt() => SetDecryptedState(true);
    public void Encrypt() => SetDecryptedState(false);
    public void SetDecryptedState(bool state)
    {
        if (state == IsDecrypted)
            return;
        PokeCrypto.CryptArray(CryptoData, GetEncryptionSeed());
        IsDecrypted = state;
    }

    /// <summary>
    /// Same as Gen4!
    /// </summary>
    /// <returns></returns>
    public uint GetEncryptionSeed()
    {
        uint seed = Seed;
        return seed | (~seed << 16);
    }

    #region Footer
    public bool SizeValid => BlockSize == SIZE;
    public bool ChecksumValid => FooterChecksum == CalculateFooterChecksum();

    public ushort EncryptedChecksumCopy { get => ReadUInt16LittleEndian(Footer); set => WriteUInt16LittleEndian(Footer, value); }
    // 0000
    public uint FooterCount { get => ReadUInt32LittleEndian(Footer[0x4..]); set => WriteUInt32LittleEndian(Footer[0x4..], value); }
    public int BlockSize { get => ReadInt32LittleEndian(Footer[0x8..]); set => WriteInt32LittleEndian(Footer[0x8..], value); }
    public uint BlockID { get => ReadUInt32LittleEndian(Footer[0xC..]); set => WriteUInt32LittleEndian(Footer[0xC..], value); }
    // 0000
    public ushort FooterChecksum { get => ReadUInt16LittleEndian(Footer[0x12..]); set => WriteUInt16LittleEndian(Footer[0x12..], value); }

    private ushort CalculateEncryptedChecksum()
    {
        if (IsDecrypted)
            throw new InvalidOperationException("Cannot calculate checksum when decrypted.");
        return Checksums.CRC16_CCITT(EncryptedChecksumRegion);
    }

    private ushort CalculateDecryptedChecksum()
    {
        if (!IsDecrypted)
            throw new InvalidOperationException("Cannot calculate checksum when encrypted.");
        return Checksums.CRC16_CCITT(DecryptedChecksumRegion);
    }

    private ushort CalculateFooterChecksum() => Checksums.CRC16_CCITT(FooterChecksumRegion);

    public void RefreshChecksums()
    {
        var state = IsDecrypted;
        Decrypt();
        Seed = CalculateDecryptedChecksum();
        Encrypt();
        EncryptedChecksum = CalculateEncryptedChecksum();
        FooterChecksum = CalculateFooterChecksum();
        SetDecryptedState(state);
    }

    #endregion

    #region Conversion

    private const int TrainerCount = 4;
    private const int TrainerLength = 0x2A4;
    public Span<byte> Trainer1 => Data.Slice(0x0CFC, TrainerLength);
    public Span<byte> Trainer2 => Data.Slice(0x0FA0, TrainerLength);
    public Span<byte> Trainer3 => Data.Slice(0x1244, TrainerLength);
    public Span<byte> Trainer4 => Data.Slice(0x14E8, TrainerLength);

    public PK5[] GetTeam(int trainer)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)trainer, TrainerCount);
        var span = trainer switch
        {
            0 => Trainer1,
            1 => Trainer2,
            2 => Trainer3,
            3 => Trainer4,
            _ => throw new ArgumentOutOfRangeException(nameof(trainer)),
        };

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
}
