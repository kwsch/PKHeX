using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public class RecordMixing3Gift
{
    /// <summary>
    /// 0x8: Total Size of this object
    /// </summary>
    public const int SIZE = 8;

    public readonly byte[] Data;

    public RecordMixing3Gift(byte[] data)
    {
        if (data.Length != SIZE)
            throw new ArgumentException("Invalid size.", nameof(data));

        Data = data;
    }

    public bool IsChecksumValid() => Checksum == Checksums.CheckSum16(Data);
    public void FixChecksum() => Checksum = Checksums.CheckSum16(Data);

    public ushort Checksum { get => ReadUInt16LittleEndian(Data.AsSpan(0)); set => WriteUInt16LittleEndian(Data.AsSpan(0), value); }
    public byte Max { get => Data[4]; set => Data[4] = value; }
    public byte Count { get => Data[5]; set => Data[5] = value; }
    public ushort Item { get => ReadUInt16LittleEndian(Data.AsSpan(6)); set => WriteUInt16LittleEndian(Data.AsSpan(6), value); }
}
