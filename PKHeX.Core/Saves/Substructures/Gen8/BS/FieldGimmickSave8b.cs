using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// size: 0xC
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class FieldGimmickSave8b : SaveBlock
    {
        public FieldGimmickSave8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public int Value0 { get => BitConverter.ToInt32(Data, Offset + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x00); }
        public int Value1 { get => BitConverter.ToInt32(Data, Offset + 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x04); }
        public int Value2 { get => BitConverter.ToInt32(Data, Offset + 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x08); }
    }
}
