using System;

namespace PKHeX.Core
{
    public sealed class FestaFacility
    {
        private const int SIZE = 0x48;
        private readonly byte[] Data;
        private readonly int Language;
        private readonly int ofs;

        public FestaFacility(SAV7 sav, int index)
        {
            ofs = (index * SIZE) + sav.Festa.Offset + 0x310;
            Data = sav.GetData(ofs, SIZE);
            Language = sav.Language;
        }

        public void CopyTo(SAV7 sav) => sav.SetData(Data, ofs);

        public int Type { get => Data[0x00]; set => Data[0x00] = (byte)value; }
        public int Color { get => Data[0x01]; set => Data[0x01] = (byte)value; }
        public bool IsIntroduced { get => Data[0x02] != 0; set => Data[0x02] = (byte)(value ? 1 : 0); }
        public int Gender { get => Data[0x03]; set => Data[0x03] = (byte)value; }
        public string OT_Name { get => StringConverter.GetString7(Data, 0x04, 0x1A); set => StringConverter.SetString7(value, 12, Language).CopyTo(Data, 0x04); }

        private int MessageMeet { get => BitConverter.ToUInt16(Data, 0x1E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1E); }
        private int MessagePart { get => BitConverter.ToUInt16(Data, 0x20); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x20); }
        private int MessageMoved { get => BitConverter.ToUInt16(Data, 0x22); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x22); }
        private int MessageDisappointed { get => BitConverter.ToUInt16(Data, 0x24); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x24); }
        public int UsedLuckyRank { get => Data[0x26]; set => Data[0x26] = (byte)value; } // 1:a bit, 2:a whole lot, 3:a whole ton
        public int UsedLuckyPlace { get => Data[0x27]; set => Data[0x27] = (byte)value; } // 1:GTS, ... 7:haunted house
        public uint UsedFlags { get => BitConverter.ToUInt32(Data, 0x28); set => BitConverter.GetBytes(value).CopyTo(Data, 0x28); }
        public uint UsedRandStat { get => BitConverter.ToUInt32(Data, 0x2C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x2C); }

        public int NPC { get => Math.Max(0, BitConverter.ToInt32(Data, 0x30)); set => BitConverter.GetBytes(Math.Max(0, value)).CopyTo(Data, 0x30); }
        public byte[] TrainerFesID { get => Data.Slice(0x34, 0xC); set => value.CopyTo(Data, 0x34); }
        public int ExchangeLeftCount { get => Data[0x40]; set => Data[0x40] = (byte)value; } // used when Type=Exchange

        public int GetMessage(int index)
        {
            return index switch
            {
                0 => MessageMeet,
                1 => MessagePart,
                2 => MessageMoved,
                3 => MessageDisappointed,
                _ => 0
            };
        }

        public void SetMessage(int index, ushort value)
        {
            switch (index)
            {
                case 0: MessageMeet = value; break;
                case 1: MessagePart = value; break;
                case 2: MessageMoved = value; break;
                case 3: MessageDisappointed = value; break;
                default: return;
            }
        }
    }
}
