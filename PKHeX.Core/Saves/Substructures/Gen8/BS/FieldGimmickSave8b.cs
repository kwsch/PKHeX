using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    /// <summary>
    /// size: 0xC
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class FieldGimmickSave8b : SaveBlock<SAV8BS>
    {
        public FieldGimmickSave8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public int Value0 { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x00)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x00), value); }
        public int Value1 { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x04)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x04), value); }
        public int Value2 { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x08)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x08), value); }
    }
}
