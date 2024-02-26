using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SecretBase6GoodPlacement(Memory<byte> raw)
{
    public const int SIZE = 12;

    private Span<byte> Data => raw.Span;

    public ushort Good { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort X { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }
    public ushort Y { get => ReadUInt16LittleEndian(Data[4..]); set => WriteUInt16LittleEndian(Data[4..], value); }

    public byte Rotation { get => Data[6]; set => Data[6] = value; }
    // byte unused

    public ushort Param1 { get => ReadUInt16LittleEndian(Data[8..]); set => WriteUInt16LittleEndian(Data[8..], value); }
    public ushort Param2 { get => ReadUInt16LittleEndian(Data[10..]); set => WriteUInt16LittleEndian(Data[10..], value); }
}
