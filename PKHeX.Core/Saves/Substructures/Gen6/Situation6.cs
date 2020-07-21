using System;

namespace PKHeX.Core
{
    public sealed class Situation6 : SaveBlock
    {
        public Situation6(SaveFile SAV, int offset) : base(SAV) => Offset = offset;

        public int M
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x02);
            set
            {
                var val = BitConverter.GetBytes((ushort)value);
                val.CopyTo(Data, Offset + 0x02);
                val.CopyTo(Data, Offset + 0x02 + 0xF4);
            }
        }

        public float X
        {
            get => BitConverter.ToSingle(Data, Offset + 0x10) / 18;
            set
            {
                var val = BitConverter.GetBytes(value * 18);
                val.CopyTo(Data, Offset + 0x10);
                val.CopyTo(Data, Offset + 0x10 + 0xF4);
            }
        }

        public float Z
        {
            get => BitConverter.ToSingle(Data, Offset + 0x14);
            set
            {
                var val = BitConverter.GetBytes(value);
                val.CopyTo(Data, Offset + 0x14);
                val.CopyTo(Data, Offset + 0x14 + 0xF4);
            }
        }

        public float Y
        {
            get => BitConverter.ToSingle(Data, Offset + 0x18) / 18;
            set
            {
                var val = BitConverter.GetBytes(value * 18);
                val.CopyTo(Data, Offset + 0x18);
                val.CopyTo(Data, Offset + 0x18 + 0xF4);
            }
        }

        // xy only
        public int Style
        {
            get => Data[Offset + 0x14D];
            set => Data[Offset + 0x14D] = (byte)value;
        }
    }
}