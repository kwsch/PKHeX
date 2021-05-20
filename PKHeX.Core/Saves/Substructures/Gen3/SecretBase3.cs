using System;

namespace PKHeX.Core
{
    public sealed class SecretBase3
    {
        private readonly byte[] Data;
        private readonly int Offset;
        private bool Japanese => Language == (int) LanguageID.Japanese;

        public SecretBase3(byte[] data, int offset)
        {
            Data = data;
            Offset = offset;
        }

        public int SecretBaseLocation { get => Data[Offset + 0]; set => Data[Offset + 0] = (byte) value; }

        public int OT_Gender
        {
            get => (Data[Offset + 1] >> 4) & 1;
            set => Data[Offset + 1] = (byte) ((Data[Offset + 1] & 0xEF) | (value & 1) << 4);
        }

        public bool BattledToday
        {
            get => ((Data[Offset + 1] >> 5) & 1) == 1;
            set => Data[Offset + 1] = (byte)((Data[Offset + 1] & 0xDF) | (value ? 1 : 0) << 5);
        }

        public int RegistryStatus
        {
            get => (Data[Offset + 1] >> 6) & 3;
            set => Data[Offset + 1] = (byte)((Data[Offset + 1] & 0x3F) | (value & 3) << 6);
        }

        public string OT_Name
        {
            get => StringConverter3.GetString3(Data, Offset + 2, 7, Japanese);
            set => StringConverter3.SetString3(value, 7, Japanese, 7).CopyTo(Data, Offset + 2);
        }

        public uint OT_ID
        {
            get => BitConverter.ToUInt32(Data, Offset + 9);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 9);
        }

        public int OT_Class => Data[Offset + 9] % 5;
        public int Language { get => Data[Offset + 0x0D]; set => Data[Offset + 0x0D] = (byte)value; }

        public ushort SecretBasesReceived
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x0E);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x0E);
        }

        public byte TimesEntered { get => Data[Offset + 0x10]; set => Data[Offset + 0x10] = value; }
        public int Unused11  { get => Data[Offset + 0x11]; set => Data[Offset + 0x11] = (byte)value; } // alignment padding

        public byte[] GetDecorations() => Data.Slice(Offset + 0x12, 0x10);
        public void SetDecorations(byte[] value) => value.CopyTo(Data, Offset + 0x12);

        public byte[] GetDecorationCoordinates() => Data.Slice(Offset + 0x22, 0x10);
        public void SetDecorationCoordinates(byte[] value) => value.CopyTo(Data, Offset + 0x22);

        public SecretBase3Team Team
        {
            get => new(Data.Slice(Offset + 50, 72));
            set => value.Write().CopyTo(Data, Offset + 50);
        }

        public int TID
        {
            get => (ushort)OT_ID;
            set => OT_ID = (ushort)(SID | (ushort)value);
        }

        public int SID
        {
            get => (ushort)OT_ID >> 8;
            set => OT_ID = (ushort)(((ushort)value << 16) | TID);
        }
    }
}
