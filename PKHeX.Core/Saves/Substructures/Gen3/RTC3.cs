using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class RTC3(Memory<byte> Raw)
{
    public Span<byte> Data => Raw.Span;
    public const int Size = 8;

    public int Day { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, (ushort)value); }
    public int Hour { get => Data[2]; set => Data[2] = (byte)value; }
    public int Minute { get => Data[3]; set => Data[3] = (byte)value; }
    public int Second { get => Data[4]; set => Data[4] = (byte)value; }
}
