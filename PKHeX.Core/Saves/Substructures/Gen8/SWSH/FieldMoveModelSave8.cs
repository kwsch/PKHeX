using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class FieldMoveModelSave8(SAV8SWSH sav, SCBlock block) : SaveBlock<SAV8SWSH>(sav, block.Data)
{
    public int M { get => ReadUInt16LittleEndian(Data.AsSpan(0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(0x00), (ushort)value); }
    public float X { get => ReadSingleLittleEndian(Data.AsSpan(0x08)); set => WriteSingleLittleEndian(Data.AsSpan(0x08), value); }
    public float Z { get => ReadSingleLittleEndian(Data.AsSpan(0x10)); set => WriteSingleLittleEndian(Data.AsSpan(0x10), value); }
    public float Y { get => (int)ReadSingleLittleEndian(Data.AsSpan(0x18)); set => WriteSingleLittleEndian(Data.AsSpan(0x18), value); }
    public float R { get => (int)ReadSingleLittleEndian(Data.AsSpan(0x20)); set => WriteSingleLittleEndian(Data.AsSpan(0x20), value); }
}
