using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class RTC3(byte[] Data)
{
    public readonly byte[] Data = Data;
    public const int Size = 8;

    public int Day { get => ReadUInt16LittleEndian(Data.AsSpan(0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(0x00), (ushort)value); }
    public int Hour { get => Data[2]; set => Data[2] = (byte)value; }
    public int Minute { get => Data[3]; set => Data[3] = (byte)value; }
    public int Second { get => Data[4]; set => Data[4] = (byte)value; }
}
