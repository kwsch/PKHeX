using System;

namespace PKHeX.Core
{
    public sealed class MyStatus8 : SaveBlock
    {
        public MyStatus8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

        public int TID
        {
            get => BitConverter.ToUInt16(Data, 0xA0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA0);
        }

        public int SID
        {
            get => BitConverter.ToUInt16(Data, 0xA2);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA2);
        }

        public int Game
        {
            get => Data[0xA4];
            set => Data[0xA4] = (byte)value;
        }

        public int Gender
        {
            get => Data[0xA5];
            set => Data[0xA5] = (byte)value;
        }

        // A6
        public int Language
        {
            get => Data[Offset + 0xA7];
            set => Data[Offset + 0xA7] = (byte)value;
        }

        public string OT
        {
            get => SAV.GetString(Data, 0xB0, 0x1A);
            set => SAV.SetData(Data, SAV.SetString(value, SAV.OTLength), 0xB0);
        }
    }
}