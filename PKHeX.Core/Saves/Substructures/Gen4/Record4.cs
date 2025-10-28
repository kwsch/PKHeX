using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Records
/// </summary>
/// <remarks>
/// https://github.com/pret/pokediamond/blob/9428132cb0a6f6404c277f1d5c9259e0996d4312/arm9/src/unk_02029FB0.c
/// https://github.com/pret/pokeplatinum/blob/d8b8dd7d9de3722c7561f854d04260cabf1eb008/src/unk_0202CD50.c
/// https://github.com/pret/pokeheartgold/blob/83b71617452eb3e2d8d28d289962deafafd638c2/asm/game_stats.s
/// </remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Record4(SAV4 SAV, Memory<byte> raw) : SaveBlock<SAV4>(SAV, raw)
{
    public static int GetSize(SAV4 SAV) => SAV switch
    {
        SAV4DP =>   (Record32DP   * sizeof(uint)) +(Record16 * sizeof(ushort)),
        SAV4Pt =>   (Record32Pt   * sizeof(uint)) +(Record16 * sizeof(ushort)) + (3 * sizeof(ushort)),
        SAV4HGSS => (Record32HGSS * sizeof(uint)) +(Record16 * sizeof(ushort)) + (3 * sizeof(ushort)),
        _ => throw new ArgumentOutOfRangeException(nameof(SAV)),
    };

    private Span<byte> DataRegion => Data[..^((SAV is SAV4DP) ? 0 : 6)];
    private Span<byte> CryptoData => Data[CryptoOffset..^4]; // most of DataRegion + Padding

    /// <summary>
    /// Padding value. Should always be 0 after decryption.
    /// </summary>
    /// <remarks>
    /// Only present in Pt/HG/SS.
    /// </remarks>
    public ushort Padding
    {
        get => (SAV is SAV4DP) ? (ushort)0 : ReadUInt16LittleEndian(Data[^6..]);
        set { if (SAV is not SAV4DP) WriteUInt16LittleEndian(Data[^6..], value); }
    }

    /// <summary>
    /// Variable portion of the crypto seed. Checksum value based on the current record data.
    /// </summary>
    /// <remarks>
    /// Only present in Pt/HG/SS.
    /// </remarks>
    public ushort Checksum
    {
        get => (SAV is SAV4DP) ? (ushort)0 : ReadUInt16LittleEndian(Data[^4..]);
        set { if (SAV is not SAV4DP) WriteUInt16LittleEndian(Data[^4..], value); }
    }

    /// <summary>
    /// Constant portion of the crypto seed. Initialized when the save file is created.
    /// </summary>
    /// <remarks>
    /// Only present in Pt/HG/SS.
    /// </remarks>
    public ushort InitSeed
    {
        get => (SAV is SAV4DP) ? (ushort)0 : ReadUInt16LittleEndian(Data[^2..]);
        set { if (SAV is not SAV4DP) WriteUInt16LittleEndian(Data[^2..], value); }
    }

    /// <summary>
    /// Seed used to encrypt and decrypt the record data.
    /// </summary>
    /// <remarks>
    /// Only present in Pt/HG/SS.
    /// </remarks>
    public uint CryptoSeed
    {
        get => (SAV is SAV4DP) ? 0 : ReadUInt32LittleEndian(Data[^4..]);
        set { if (SAV is not SAV4DP) WriteUInt32LittleEndian(Data[^4..], value); }
    }

    #region Encryption
    private bool IsDecrypted;
    public void EndAccess() => EnsureDecrypted(false);
    private void EnsureDecrypted(bool state = true)
    {
        if (SAV is SAV4DP || IsDecrypted == state) // DP does not encrypt records
            return;
        if (IsDecrypted && !state)
            RefreshChecksum(); // refresh only on encrypt
        PokeCrypto.CryptArray(CryptoData, CryptoSeed);
        IsDecrypted = state;
    }
    private void RefreshChecksum() => Checksum = Checksums.CheckSum16(CryptoData);
    private int CryptoOffset => ScoreIndex * sizeof(uint); // Pt/HGSS do not encrypt records before Score
    #endregion

    private const int Record32DP = 44;
    private const int Record32Pt = 71;
    private const int Record32HGSS = 72;
    public int Record32 => SAV switch
    {
        SAV4DP => Record32DP,
        SAV4Pt => Record32Pt,
        SAV4HGSS => Record32HGSS,
        _ => throw new ArgumentOutOfRangeException(nameof(SAV)),
    };
    public const int Record16 = 77;
    private int Partition2 => Record32 * sizeof(uint);
    private Span<byte> Record32Data => DataRegion[..Partition2];
    private Span<byte> Record16Data => DataRegion[Partition2..];

    #region Bounds
    private const uint Max32Large = 999_999_999;
    private const uint Max32Small = 999_999;
    private const ushort Max16Large = 0xFFFF;
    private const ushort Max16Small = 9999;
    private const uint MaxScore = 99_999_999;

    private static ReadOnlySpan<byte> IsLargeDP =>
    [
        // u32
        1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0,

        // u16
        1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    ];

    private static ReadOnlySpan<byte> IsLargePt =>
    [
        // u32
        1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 0,

        // u16
        1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    ];

    private static ReadOnlySpan<byte> IsLargeHGSS =>
    [
        // u32
        1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 0,

        // u16
        1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    ];

    private bool IsLarge(int index) => SAV switch
    {
        SAV4DP => IsLargeDP[index] == 1,
        SAV4Pt => IsLargePt[index] == 1,
        SAV4HGSS => IsLargeHGSS[index] == 1,
        _ => throw new NotSupportedException(),
    };

    public uint GetMax32(int index) => IsLarge(index) ? (index == ScoreIndex ? MaxScore : Max32Large) : Max32Small;
    public ushort GetMax16(int index) => IsLarge(Record32 + index) ? Max16Large : Max16Small;
    #endregion

    public uint GetRecord32(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)Record32);
        EnsureDecrypted();
        return ReadUInt32LittleEndian(Record32Data[(index * 4)..]);
    }

    public void SetRecord32(int index, uint value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)Record32);
        EnsureDecrypted();
        WriteUInt32LittleEndian(Record32Data[(index * 4)..], Math.Min(GetMax32(index), value));
    }

    public ushort GetRecord16(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, Record16);
        EnsureDecrypted();
        return ReadUInt16LittleEndian(Record16Data[(index * 2)..]);
    }

    public void SetRecord16(int index, ushort value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, Record16);
        EnsureDecrypted();
        WriteUInt16LittleEndian(Record16Data[(index * 2)..], Math.Min(GetMax16(index), value));
    }

    #region Enums
    public int ScoreIndex => SAV switch
    {
        SAV4DP => (int)Record4DPIndex.Score,
        SAV4Pt => (int)Record4PtIndex.Score,
        SAV4HGSS => (int)Record4HGSSIndex.Score,
        _ => throw new NotSupportedException(),
    };

    /// <summary>
    /// Record IDs for <see cref="GameVersion.D"/> and <see cref="GameVersion.P"/>
    /// </summary>
    public enum Record4DPIndex
    {
        // u32
        Score = 0,

        // u16
        FirstU16 = Record32DP,
    }

    /// <summary>
    /// Record IDs for <see cref="GameVersion.Pt"/>
    /// </summary>
    public enum Record4PtIndex
    {
        // u32
        Score = 1,

        // u16
        FirstU16 = Record32Pt,
    }

    /// <summary>
    /// Record IDs for <see cref="GameVersion.HG"/> and <see cref="GameVersion.SS"/>
    /// </summary>
    public enum Record4HGSSIndex
    {
        // u32
        ApricornGet = 1,
        Score = 2,
        BadgeGet = 22,
        BattlePoints = 69,

        // u16
        FirstU16 = Record32HGSS,
    }
    #endregion
}
