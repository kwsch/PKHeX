using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class FieldMoveModelSave8(SAV8SWSH sav, SCBlock block) : SaveBlock<SAV8SWSH>(sav, block.Data)
{
    public int M { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, (ushort)value); }
    public float X { get => ReadSingleLittleEndian(Data[0x08..]); set => WriteSingleLittleEndian(Data[0x08..], value); }
    public float Z { get => ReadSingleLittleEndian(Data[0x10..]); set => WriteSingleLittleEndian(Data[0x10..], value); }
    public float Y { get => (int)ReadSingleLittleEndian(Data[0x18..]); set => WriteSingleLittleEndian(Data[0x18..], value); }
    public float R { get => (int)ReadSingleLittleEndian(Data[0x20..]); set => WriteSingleLittleEndian(Data[0x20..], value); }
}
