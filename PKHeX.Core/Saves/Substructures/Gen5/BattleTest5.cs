using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BattleTest5(Memory<byte> Raw)
{
    public const int SIZE = 0x5C8;
    public const string Extension = "bt5";

    private Span<byte> Data => Raw.Span;
    public bool IsUninitialized => !Data.ContainsAnyExcept<byte>(0xFF, 0);

    public const ushort Sentinel = 0x0D68; // 3432

    /// <summary>
    /// The game will validate the CRC, then update the magic and reapply the CRC.
    /// </summary>
    public const ushort SentinelPost = 0x07DA; // 2010

    // Should be equal to Sentinel otherwise the data is not valid.
    public ushort Magic { get => ReadUInt16LittleEndian(Data[0x5C2..]); set => WriteUInt16LittleEndian(Data[0x5C2..], value); }
    public ushort Flags { get => ReadUInt16LittleEndian(Data[0x5C4..]); set => WriteUInt16LittleEndian(Data[0x5C4..], value); }
    public ushort Checksum { get => ReadUInt16LittleEndian(Data[0x5C6..]); set => WriteUInt16LittleEndian(Data[0x5C6..], value); }

    public bool IsDoubles
    {
        get => (Flags & 0x8000) != 0;
        set
        {
            if (value)
                Flags |= 0x8000;
            else
                Flags &= 0x7FFF;
        }
    }

    public ushort CalculateChecksum() => Checksums.CRC16_CCITT(Data[..0x5C6]);
    public void RefreshChecksums() => Checksum = CalculateChecksum();

    public void SetAsUnplayable()
    {
        // The game verifies the checksums first (clears it if fails & aborts).
        // If valid, updates the magic and re-applies the checksum.
        // It is assumed that this is the after-test state that prevents replaying.
        Magic = SentinelPost;
        RefreshChecksums();
    }

    // Script command 0x1F2 in B2/W2
    public ushort GetScriptResultIsValid()
    {
        var chk = CalculateChecksum();
        if (chk != Checksum)
            return 0;
        if (Magic != Sentinel)
            return 0;
        return IsDoubles ? (ushort)2 : (ushort)1;
    }
}
