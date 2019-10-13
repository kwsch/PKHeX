using System;

namespace PKHeX.Core
{
    public sealed class Misc7b : SaveBlock
    {
        public Misc7b(SAV7b sav, int offset) : base(sav) => Offset = offset;

        public uint Money
        {
            get => BitConverter.ToUInt32(Data, Offset + 4);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 4);
        }

        public string Rival
        {
            get => SAV.GetString(Offset + 0x200, 0x1A);
            set => SAV.SetString(value, SAV.OTLength).CopyTo(Data, Offset + 0x200);
        }
    }
}