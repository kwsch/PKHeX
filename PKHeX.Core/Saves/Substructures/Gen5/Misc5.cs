using System;

namespace PKHeX.Core
{
    public sealed class Misc5 : SaveBlock
    {
        public Misc5(SAV5BW sav, int offset) : base(sav) => Offset = offset;
        public Misc5(SAV5B2W2 sav, int offset) : base(sav) => Offset = offset;

        public uint Money
        {
            get => BitConverter.ToUInt32(Data, Offset);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset);
        }

        public int Badges
        {
            get => Data[Offset + 0x4];
            set => Data[Offset + 0x4] = (byte)value;
        }
    }
}