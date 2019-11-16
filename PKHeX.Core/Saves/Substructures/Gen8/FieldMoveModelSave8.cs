using System;

namespace PKHeX.Core
{
    public sealed class FieldMoveModelSave8 : SaveBlock
    {
        public FieldMoveModelSave8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

        public int M { get => BitConverter.ToUInt16(Data, 0x00); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x00); }
        public float X { get => BitConverter.ToSingle(Data, 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, 0x08); }
        public float Z { get => BitConverter.ToSingle(Data, 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, 0x10); }
        public float Y { get => (int)BitConverter.ToSingle(Data, 0x18); set => BitConverter.GetBytes(value).CopyTo(Data, 0x18); }
        public float R { get => (int)BitConverter.ToSingle(Data, 0x20); set => BitConverter.GetBytes(value).CopyTo(Data, 0x20); }
    }
}