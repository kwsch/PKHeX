using System;

namespace PKHeX.Core
{
    public sealed class Misc8 : SaveBlock
    {
        public Misc8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

        public int Badges
        {
            get => Data[Offset + 0x00];
            set => Data[Offset + 0x00] = (byte)value;
        }

        public uint Money
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x04);
            set
            {
                if (value > 9999999)
                    value = 9999999;
                SAV.SetData(Data, BitConverter.GetBytes(value), Offset + 0x04);
            }
        }

        public int BP
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x11C);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x11C);
        }
    }
}