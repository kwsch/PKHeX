using System;

namespace PKHeX.Core
{
    public sealed class PlayerData5 : SaveBlock
    {
        public PlayerData5(SAV5BW sav, int offset) : base(sav) => Offset = offset;
        public PlayerData5(SAV5B2W2 sav, int offset) : base(sav) => Offset = offset;

        public string OT
        {
            get => SAV.GetString(Offset + 0x4, 0x10);
            set => SAV.SetString(value, SAV.OTLength).CopyTo(Data, Offset + 0x4);
        }

        public int TID
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x14 + 0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x14 + 0);
        }

        public int SID
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x14 + 2);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x14 + 2);
        }

        public int Language
        {
            get => Data[Offset + 0x1E];
            set => Data[Offset + 0x1E] = (byte)value;
        }

        public int Game
        {
            get => Data[Offset + 0x1F];
            set => Data[Offset + 0x1F] = (byte)value;
        }

        public int Gender
        {
            get => Data[Offset + 0x21];
            set => Data[Offset + 0x21] = (byte)value;
        }

        // 22,23 ??

        public int PlayedHours
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x24);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x24);
        }

        public int PlayedMinutes
        {
            get => Data[Offset + 0x24 + 2];
            set => Data[Offset + 0x24 + 2] = (byte)value;
        }

        public int PlayedSeconds
        {
            get => Data[Offset + 0x24 + 3];
            set => Data[Offset + 0x24 + 3] = (byte)value;
        }

        public int M
        {
            get => BitConverter.ToInt32(Data, Offset + 0x180);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x180);
        }

        public int X
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x186);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x186);
        }

        public int Z
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x18A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x18A);
        }

        public int Y
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x18E);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x18E);
        }
    }
}