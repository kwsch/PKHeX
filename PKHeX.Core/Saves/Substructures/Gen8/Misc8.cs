using System;

namespace PKHeX.Core
{
    public sealed class Misc8 : SaveBlock
    {
        public Misc8(SAV8SWSH sav, int offset) : base(sav) => Offset = offset;

        public uint Money
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x4);
            set
            {
                if (value > 9999999)
                    value = 9999999;
                SAV.SetData(BitConverter.GetBytes(value), Offset + 0x4);
            }
        }

        public int Vivillon { get; set; } // todo
    }
}