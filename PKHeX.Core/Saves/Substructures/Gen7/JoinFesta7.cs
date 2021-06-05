using System;

namespace PKHeX.Core
{
    public sealed class JoinFesta7 : SaveBlock
    {
        public JoinFesta7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public JoinFesta7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        public int FestaCoins
        {
            get => BitConverter.ToInt32(Data, Offset + 0x508);
            set
            {
                if (value > 9999999)
                    value = 9999999;
                BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x508);

                TotalFestaCoins = ((SAV7)SAV).GetRecord(038) + value; // UsedFestaCoins
            }
        }

        public int TotalFestaCoins
        {
            get => BitConverter.ToInt32(Data, Offset + 0x50C);
            set
            {
                if (value > 9999999)
                    value = 9999999;
                BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x50C);
            }
        }

        public string FestivalPlazaName
        {
            get => StringConverter.GetString7(Data, Offset + 0x510, 0x2A);
            set => StringConverter.SetString7(value, 20, 21).CopyTo(Data, Offset + 0x510);
        }

        public ushort FestaRank { get => BitConverter.ToUInt16(Data, Offset + 0x53A); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x53A); }
        public ushort GetFestaMessage(int index) => BitConverter.ToUInt16(Data, Offset + (index * 2));
        public void SetFestaMessage(int index, ushort value) => BitConverter.GetBytes(value).CopyTo(Data, Offset + (index * 2));
        public bool GetFestaPhraseUnlocked(int index) => Data[Offset + 0x2A50 + index] != 0; //index: 0 to 105:commonPhrases, 106:Lv100!

        public void SetFestaPhraseUnlocked(int index, bool value)
        {
            if (GetFestaPhraseUnlocked(index) != value)
                Data[Offset + 0x2A50 + index] = value ? (byte)1 : (byte)0;
        }

        public byte GetFestPrizeReceived(int index) => Data[Offset + 0x53C + index];
        public void SetFestaPrizeReceived(int index, byte value) => Data[Offset + 0x53C + index] = value;
        private int FestaYear { get => BitConverter.ToInt32(Data, Offset + 0x2F0); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x2F0); }
        private int FestaMonth { get => BitConverter.ToInt32(Data, Offset + 0x2F4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x2F4); }
        private int FestaDay { get => BitConverter.ToInt32(Data, Offset + 0x2F8); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x2F8); }
        private int FestaHour { get => BitConverter.ToInt32(Data, Offset + 0x300); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x300); }
        private int FestaMinute { get => BitConverter.ToInt32(Data, Offset + 0x304); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x304); }
        private int FestaSecond { get => BitConverter.ToInt32(Data, Offset + 0x308); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x308); }

        public DateTime? FestaDate
        {
            get => FestaYear >= 0 && FestaMonth > 0 && FestaDay > 0 && FestaHour >= 0 && FestaMinute >= 0 && FestaSecond >= 0 && DateUtil.IsDateValid(FestaYear, FestaMonth, FestaDay)
                ? new DateTime(FestaYear, FestaMonth, FestaDay, FestaHour, FestaMinute, FestaSecond)
                : null;
            set
            {
                if (value.HasValue)
                {
                    DateTime dt = value.Value;
                    FestaYear = dt.Year;
                    FestaMonth = dt.Month;
                    FestaDay = dt.Day;
                    FestaHour = dt.Hour;
                    FestaMinute = dt.Minute;
                    FestaSecond = dt.Second;
                }
            }
        }
    }
}