using System;

namespace PKHeX.Core
{
    public sealed class MyStatus7b : SaveBlock
    {
        private readonly SaveFile SAV;

        public MyStatus7b(SaveFile sav) : base(sav)
        {
            SAV = sav;
            Offset = ((SAV7b)sav).GetBlockOffset(BelugaBlockIndex.MyStatus);
        }

        // Player Information

        // idb uint8 offset: 0x58

        public int TID
        {
            get => BitConverter.ToUInt16(Data, Offset + 0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0);
        }

        public int SID
        {
            get => BitConverter.ToUInt16(Data, Offset + 2);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 2);
        }

        public int Game
        {
            get => Data[Offset + 4];
            set => Data[Offset + 4] = (byte)value;
        }

        public int Gender
        {
            get => Data[Offset + 5];
            set => Data[Offset + 5] = (byte)value;
        }

        public int Language
        {
            get => Data[Offset + 0x35];
            set => Data[Offset + 0x35] = (byte)value;
        }

        public string OT
        {
            get => SAV.GetString(Offset + 0x38, 0x1A);
            set => SAV.SetString(value, SAV.OTLength).CopyTo(Data, Offset + 0x38);
        }
    }
}