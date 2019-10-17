using System;

namespace PKHeX.Core
{
    public sealed class Misc6XY : SaveBlock, IMisc6
    {
        public Misc6XY(SAV6XY sav, int offset) : base(sav) => Offset = offset;

        public uint Money
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x8);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x8);
        }

        public int Badges
        {
            get => Data[Offset + 0xC];
            set => Data[Offset + 0xC] = (byte)value;
        }

        public int BP
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x3C);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x3C);
        }

        public int Vivillon
        {
            get => Data[Offset + 0x50];
            set => Data[Offset + 0x50] = (byte)value;
        }
    }
}