using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class FieldMoveModelSave7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    //public int Unknown { get => ReadUInt16LittleEndian(Data.Slice(0x00)); set => WriteUInt16LittleEndian(Data.Slice(0x00), (ushort)value); } // related to Ride Pokémon
    public float X { get => ReadSingleLittleEndian(Data[0x08..]); set => WriteSingleLittleEndian(Data[0x08..], value); }
    public float Z { get => ReadSingleLittleEndian(Data[0x0C..]); set => WriteSingleLittleEndian(Data[0x0C..], value); }
    public float Y { get => ReadSingleLittleEndian(Data[0x10..]); set => WriteSingleLittleEndian(Data[0x10..], value); }
    public float RX { get => ReadSingleLittleEndian(Data[0x14..]); set => WriteSingleLittleEndian(Data[0x14..], value); }
    public float RZ { get => ReadSingleLittleEndian(Data[0x18..]); set => WriteSingleLittleEndian(Data[0x18..], value); }
    public float RY { get => ReadSingleLittleEndian(Data[0x1C..]); set => WriteSingleLittleEndian(Data[0x1C..], value); }
    public float RW { get => ReadSingleLittleEndian(Data[0x20..]); set => WriteSingleLittleEndian(Data[0x20..], value); }
}
