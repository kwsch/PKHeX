using System;
using System.Collections.Generic;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Extra-data block for Battle Video records.
/// </summary>
/// <param name="Raw">Chunk of memory storing the structure.</param>
public sealed class BattleVideo4(Memory<byte> Raw) : IBattleVideo
{
    private const int SIZE = 0x1D50;
    private const int SIZE_FOOTER = 0x10;
    public const int SIZE_USED = SIZE + SIZE_FOOTER;
  //public const int SIZE_BLOCK = 0x2000;

    private Span<byte> Data => Raw.Span[..SIZE_USED];

    public bool IsDecrypted;
    private Span<byte> CryptoData => Data[0xE8..0x1D4C];
    private Span<byte> Footer => Data.Slice(SIZE, SIZE_FOOTER);

    public Span<byte> DecryptedChecksumRegion => CryptoData; // ccitt16 -> 0x1D4C

    public byte Generation => 4;
    public IEnumerable<PKM> Contents => GetTeam(0).Concat(GetTeam(1)); // don't bother with multi-battles
    public static bool IsValid(ReadOnlySpan<byte> data) => data.Length == SIZE_USED && ReadUInt32LittleEndian(data[^8..]) == SIZE;

    // Structure:
    // 0x00: u32 Key
    // 0x04... ???
    // 0xE8 - 0x1D4C: encrypted region
    // 0x1D4C: u16 cryptoSeed ~~ inflate to 32 via seed |= (seed ^ 0xFFFF) << 16;
    // u16 alignment
    // extdata 0x10 byte footer

    // Encrypted Region:
    // 0xE8... ???
    // 0x1238: u16 count Max, u16 count Present, (6 * sizeof(0x70)) pokemon -- 0x2A4
    // 0x14DC: u16 count Max, u16 count Present, (6 * sizeof(0x70)) pokemon -- 0x2A4
    // 0x1780: u16 count Max, u16 count Present, (6 * sizeof(0x70)) pokemon -- 0x2A4
    // 0x1A24: u16 count Max, u16 count Present, (6 * sizeof(0x70)) pokemon -- 0x2A4
    // 0x1CC8: char[8] OT, 16 bytes trainer info
    // 0x1CE8: char[8] OT, 16 bytes trainer info
    // 0x1D08: char[8] OT, 16 bytes trainer info
    // 0x1D28: char[8] OT, 16 bytes trainer info
    // 0x1D48: 4 bytes ???

    public const int SizeVideoPoke = 0x70;

    public uint Key { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }

    /// <summary> <see cref="CalculateDecryptedChecksum"/> </summary>
    public ushort Seed { get => ReadUInt16LittleEndian(Data[0x1D4C..]); set => WriteUInt16LittleEndian(Data[0x1D4C..], value); }

    public void Decrypt() => SetDecryptedState(true);
    public void Encrypt() => SetDecryptedState(false);
    public void SetDecryptedState(bool state)
    {
        if (state == IsDecrypted)
            return;
        PokeCrypto.CryptArray(CryptoData, GetEncryptionSeed());
        IsDecrypted = state;
    }

    public uint GetEncryptionSeed()
    {
        uint seed = Seed;
        return seed | (~seed << 16);
    }

    #region Footer
    public bool SizeValid => BlockSize == SIZE;
    public bool ChecksumValid => Checksum == CalculateFooterChecksum();

    public uint Magic { get => ReadUInt32LittleEndian(Footer); set => WriteUInt32LittleEndian(Footer, value); }
    public uint Revision { get => ReadUInt32LittleEndian(Footer[0x4..]); set => WriteUInt32LittleEndian(Footer[0x4..], value); }
    public int BlockSize { get => ReadInt32LittleEndian(Footer[0x8..]); set => WriteInt32LittleEndian(Footer[0x8..], value); }
    public ushort BlockID { get => ReadUInt16LittleEndian(Footer[0xC..]); set => WriteUInt16LittleEndian(Footer[0xC..], value); }
    public ushort Checksum { get => ReadUInt16LittleEndian(Footer[0xE..]); set => WriteUInt16LittleEndian(Footer[0xE..], value); }

    private ushort CalculateFooterChecksum()
    {
        if (IsDecrypted)
            throw new InvalidOperationException("Cannot calculate checksum when decrypted.");
        return Checksums.CRC16_CCITT(Data[..^2]);
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
        Checksum = CalculateFooterChecksum();
        SetDecryptedState(state);
    }

    #endregion

    #region Conversion

    private const int TrainerCount = 4;
    private const int TrainerLength = 0x2A4;
    public Span<byte> Trainer1 => Data.Slice(0x1238, TrainerLength);
    public Span<byte> Trainer2 => Data.Slice(0x14DC, TrainerLength);
    public Span<byte> Trainer3 => Data.Slice(0x1780, TrainerLength);
    public Span<byte> Trainer4 => Data.Slice(0x1A24, TrainerLength);

    public PK4[] GetTeam(int trainer)
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
        var result = new PK4[count];
        for (int i = 0; i < count; i++)
        {
            var ofs = 4 + (i * SizeVideoPoke);
            var segment = span.Slice(ofs, SizeVideoPoke);
            var entity = new PK4();
            InflateToPK4(segment, entity.Data);
            result[i] = entity;
        }
        SetDecryptedState(state);
        return result;
    }

    public static void InflateToPK4(ReadOnlySpan<byte> video, Span<byte> entity)
    {
        VerifySpanSizes(entity, video);

        video[..6].CopyTo(entity[..6]); // PID & Sanity -- skip checksum.
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

        // Ball
        entity[0x83] = video[0x58];
        // Language
        entity[0x17] = video[0x59];
        // u16 alignment

        // Battle Stats
        video[0x5C..0x70].CopyTo(entity[0x88..0x9C]);

        // We're still missing things like Version and Met Location, but this is all we can recover.
        // Recalculate checksum.
        ushort checksum = Checksums.Add16(entity[8..PokeCrypto.SIZE_4STORED]);
        WriteUInt16LittleEndian(entity[6..], checksum);
    }

    public static void DeflateFromPK4(ReadOnlySpan<byte> entity, Span<byte> video)
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
        if (entity.Length < PokeCrypto.SIZE_4STORED)
            throw new ArgumentOutOfRangeException(nameof(entity), "Entity size is too small.");
    }
    #endregion
}
