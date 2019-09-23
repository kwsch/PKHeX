using System;

namespace PKHeX.Core
{
    public class MyStatus8 : SaveBlock
    {
        public MyStatus8(SAV8 sav, int offset) : base(sav) => Offset = offset;

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

        public int SubRegion
        {
            get => Data[Offset + 0x2E];
            set => Data[Offset + 0x2E] = (byte)value;
        }

        public int Country
        {
            get => Data[Offset + 0x2F];
            set => Data[Offset + 0x2F] = (byte)value;
        }

        public int ConsoleRegion
        {
            get => Data[Offset + 0x34];
            set => Data[Offset + 0x34] = (byte)value;
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