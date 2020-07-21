using System;

namespace PKHeX.Core
{
    public sealed class PlayTime8 : SaveBlock
    {
        public PlayTime8(SaveFile sav, SCBlock block) : base(sav, block.Data) { }

        public int PlayedHours
        {
            get => BitConverter.ToUInt16(Data, Offset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset);
        }

        public int PlayedMinutes
        {
            get => Data[Offset + 2];
            set => Data[Offset + 2] = (byte)value;
        }

        public int PlayedSeconds
        {
            get => Data[Offset + 3];
            set => Data[Offset + 3] = (byte)value;
        }

        // TODO
        private uint LastSaved { get => BitConverter.ToUInt32(Data, Offset + 0x4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x4); }
        private int LastSavedYear { get => (int)(LastSaved & 0xFFF) + 1900; set => LastSaved = (LastSaved & 0xFFFFF000) | (uint)(value - 1900); }
        private int LastSavedMonth { get => (int)(LastSaved >> 12 & 0xF) + 1; set => LastSaved = (LastSaved & 0xFFFF0FFF) | ((uint)(value - 1) & 0xF) << 12; }
        private int LastSavedDay { get => (int)(LastSaved >> 16 & 0x1F); set => LastSaved = (LastSaved & 0xFFE0FFFF) | ((uint)value & 0x1F) << 16; }
        private int LastSavedHour { get => (int)(LastSaved >> 21 & 0x1F); set => LastSaved = (LastSaved & 0xFC1FFFFF) | ((uint)value & 0x1F) << 21; }
        private int LastSavedMinute { get => (int)(LastSaved >> 26 & 0x3F); set => LastSaved = (LastSaved & 0x03FFFFFF) | ((uint)value & 0x3F) << 26; }
        public string LastSavedTime => $"{LastSavedYear:0000}-{LastSavedMonth:00}-{LastSavedDay:00} {LastSavedHour:00}ː{LastSavedMinute:00}"; // not :

        public DateTime? LastSavedDate
        {
            get => !Util.IsDateValid(LastSavedYear, LastSavedMonth, LastSavedDay)
                ? (DateTime?)null
                : new DateTime(LastSavedYear, LastSavedMonth, LastSavedDay, LastSavedHour, LastSavedMinute, 0);
            set
            {
                // Only update the properties if a value is provided.
                if (value.HasValue)
                {
                    var dt = value.Value;
                    LastSavedYear = dt.Year;
                    LastSavedMonth = dt.Month;
                    LastSavedDay = dt.Day;
                    LastSavedHour = dt.Hour;
                    LastSavedMinute = dt.Minute;
                }
                else // Clear the date.
                {
                    // If code tries to access MetDate again, null will be returned.
                    LastSavedYear = 0;
                    LastSavedMonth = 0;
                    LastSavedDay = 0;
                    LastSavedHour = 0;
                    LastSavedMinute = 0;
                }
            }
        }
    }
}