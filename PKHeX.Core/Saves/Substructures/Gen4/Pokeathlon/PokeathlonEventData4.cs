using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public struct PokeathlonEventData4(Memory<byte> Raw)
{
    public const int SIZE = 0x2C;
    public const uint MaxAttempts = 9_999_999;
    public const uint MaxRecord = 5;

    public Span<byte> Data => Raw.Span;

    public PokeathlonEventRecord4 GetRecord(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, MaxRecord);
        return new(Raw.Slice(index * PokeathlonEventRecord4.SIZE, PokeathlonEventRecord4.SIZE));
    }

    public uint Attempts { get => ReadUInt32LittleEndian(Data[0x28..]); set => WriteUInt32LittleEndian(Data[0x28..], Math.Min(MaxAttempts, value)); }
}
