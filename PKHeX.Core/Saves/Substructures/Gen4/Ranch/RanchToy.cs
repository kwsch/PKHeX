using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class RanchToy(byte[] Data)
{
    public const int SIZE = 0x08;
    public readonly byte[] Data = Data;

    public RanchToyType ToyType { get => (RanchToyType)Data[0]; set => Data[0] = (byte)value; }
    // 1,2,3 alignment
    public uint ToyMetadata { get => ReadUInt32BigEndian(Data.AsSpan(4)); set => WriteUInt32BigEndian(Data.AsSpan(4), value); }
}
