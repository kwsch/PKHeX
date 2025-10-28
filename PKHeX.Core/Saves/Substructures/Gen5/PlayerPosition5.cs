using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class PlayerPosition5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    public int M { get => ReadInt32LittleEndian(Data[0x80..]); set => WriteUInt16LittleEndian(Data[0x80..], (ushort)value); }
    public int X { get => ReadUInt16LittleEndian(Data[0x86..]); set => WriteUInt16LittleEndian(Data[0x86..], (ushort)value); }
    public int Z { get => ReadUInt16LittleEndian(Data[0x8A..]); set => WriteUInt16LittleEndian(Data[0x8A..], (ushort)value); }
    public int Y { get => ReadUInt16LittleEndian(Data[0x8E..]); set => WriteUInt16LittleEndian(Data[0x8E..], (ushort)value); }
}
