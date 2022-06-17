using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SecretBase6GoodPlacement
{
    public const int SIZE = 12;

    public ushort Good { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public byte Rotation { get; set; }
    // byte unused

    public ushort Param1 { get; set; }
    public ushort Param2 { get; set; }

    public SecretBase6GoodPlacement(ReadOnlySpan<byte> data)
    {
        Good = ReadUInt16LittleEndian(data);
        X = ReadUInt16LittleEndian(data[2..]);
        Y = ReadUInt16LittleEndian(data[4..]);
        Rotation = data[6];

        Param1 = ReadUInt16LittleEndian(data[8..]);
        Param2 = ReadUInt16LittleEndian(data[10..]);
    }

    public void Write(Span<byte> data)
    {
        WriteUInt16LittleEndian(data, Good);
        WriteUInt16LittleEndian(data[2..], X);
        WriteUInt16LittleEndian(data[4..], Y);
        data[6] = Rotation;

        WriteUInt16LittleEndian(data[8..], Param1);
        WriteUInt16LittleEndian(data[10..], Param2);
    }
}
