using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Record Mixing Data for Generation 3 <see cref="SAV3"/> games.
/// </summary>
public sealed class RecordMixing3Gift
{
    /// <summary>
    /// 0x8: Total Size of this object
    /// </summary>
    public const int SIZE = 8;

    public readonly byte[] Data;

    public RecordMixing3Gift(byte[] data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, SIZE);
        Data = data;
    }

    public bool IsChecksumValid() => Checksum == ComputeChecksum();
    public void FixChecksum() => Checksum = ComputeChecksum();

    private ushort ComputeChecksum() => Checksums.CheckSum16(Data.AsSpan(4));

    public ushort Checksum { get => ReadUInt16LittleEndian(Data.AsSpan(0)); set => WriteUInt16LittleEndian(Data.AsSpan(0), value); }
    public byte Max { get => Data[4]; set => Data[4] = value; }
    public byte Count { get => Data[5]; set => Data[5] = value; }
    public ushort Item { get => ReadUInt16LittleEndian(Data.AsSpan(6)); set => WriteUInt16LittleEndian(Data.AsSpan(6), value); }
}
