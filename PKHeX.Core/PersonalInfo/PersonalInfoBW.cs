using System;
using System.Linq;

namespace PKHeX.Core
{
    public class PersonalInfoBW : PersonalInfo
    {
        protected PersonalInfoBW() { }
        public const int SIZE = 0x3C;
        public PersonalInfoBW(byte[] data)
        {
            if (data.Length != SIZE)
                return;
            Data = data;

            // Unpack TMHM & Tutors
            TMHM = getBits(Data.Skip(0x28).Take(0x10).ToArray());
            TypeTutors = getBits(Data.Skip(0x38).Take(0x4).ToArray());
        }
        public override byte[] Write()
        {
            setBits(TMHM).CopyTo(Data, 0x28);
            setBits(TypeTutors).CopyTo(Data, 0x38);
            return Data;
        }

        public override int HP { get { return Data[0x00]; } set { Data[0x00] = (byte)value; } }
        public override int ATK { get { return Data[0x01]; } set { Data[0x01] = (byte)value; } }
        public override int DEF { get { return Data[0x02]; } set { Data[0x02] = (byte)value; } }
        public override int SPE { get { return Data[0x03]; } set { Data[0x03] = (byte)value; } }
        public override int SPA { get { return Data[0x04]; } set { Data[0x04] = (byte)value; } }
        public override int SPD { get { return Data[0x05]; } set { Data[0x05] = (byte)value; } }
        public override int[] Types
        {
            get { return new int[] { Data[0x06], Data[0x07] }; }
            set
            {
                if (value?.Length != 2) return;
                Data[0x06] = (byte)value[0];
                Data[0x07] = (byte)value[1];
            }
        }
        public override int CatchRate { get { return Data[0x08]; } set { Data[0x08] = (byte)value; } }
        public override int EvoStage { get { return Data[0x09]; } set { Data[0x09] = (byte)value; } }
        private int EVYield { get { return BitConverter.ToUInt16(Data, 0x0A); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0A); } }
        public override int EV_HP { get { return EVYield >> 0 & 0x3; } set { EVYield = (EVYield & ~(0x3 << 0)) | (value & 0x3) << 0; } }
        public override int EV_ATK { get { return EVYield >> 2 & 0x3; } set { EVYield = (EVYield & ~(0x3 << 2)) | (value & 0x3) << 2; } }
        public override int EV_DEF { get { return EVYield >> 4 & 0x3; } set { EVYield = (EVYield & ~(0x3 << 4)) | (value & 0x3) << 4; } }
        public override int EV_SPE { get { return EVYield >> 6 & 0x3; } set { EVYield = (EVYield & ~(0x3 << 6)) | (value & 0x3) << 6; } }
        public override int EV_SPA { get { return EVYield >> 8 & 0x3; } set { EVYield = (EVYield & ~(0x3 << 8)) | (value & 0x3) << 8; } }
        public override int EV_SPD { get { return EVYield >> 10 & 0x3; } set { EVYield = (EVYield & ~(0x3 << 10)) | (value & 0x3) << 10; } }
        public override int[] Items
        {
            get { return new int[] { BitConverter.ToInt16(Data, 0xC), BitConverter.ToInt16(Data, 0xE), BitConverter.ToInt16(Data, 0x10) }; }
            set
            {
                if (value?.Length != 3) return;
                BitConverter.GetBytes((short)value[0]).CopyTo(Data, 0xC);
                BitConverter.GetBytes((short)value[1]).CopyTo(Data, 0xE);
                BitConverter.GetBytes((short)value[2]).CopyTo(Data, 0x10);
            }
        }
        public override int Gender { get { return Data[0x12]; } set { Data[0x12] = (byte)value; } }
        public override int HatchCycles { get { return Data[0x13]; } set { Data[0x13] = (byte)value; } }
        public override int BaseFriendship { get { return Data[0x14]; } set { Data[0x14] = (byte)value; } }
        public override int EXPGrowth { get { return Data[0x15]; } set { Data[0x15] = (byte)value; } }
        public override int[] EggGroups
        {
            get { return new int[] { Data[0x16], Data[0x17] }; }
            set
            {
                if (value?.Length != 2) return;
                Data[0x16] = (byte)value[0];
                Data[0x17] = (byte)value[1];
            }
        }
        public override int[] Abilities
        {
            get { return new int[] { Data[0x18], Data[0x19], Data[0x1A] }; }
            set
            {
                if (value?.Length != 3) return;
                Data[0x18] = (byte)value[0];
                Data[0x19] = (byte)value[1];
                Data[0x1A] = (byte)value[2];
            }
        }
        public override int EscapeRate { get { return Data[0x1B]; } set { Data[0x1B] = (byte)value; } }
        protected internal override int FormStatsIndex { get { return BitConverter.ToUInt16(Data, 0x1C); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1C); } }
        public override int FormeSprite { get { return BitConverter.ToUInt16(Data, 0x1E); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1E); } }
        public override int FormeCount { get { return Data[0x20]; } set { Data[0x20] = (byte)value; } }
        public override int Color { get { return Data[0x21]; } set { Data[0x21] = (byte)value; } }
        public override int BaseEXP { get { return BitConverter.ToUInt16(Data, 0x22); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x22); } }
        public override int Height { get { return BitConverter.ToUInt16(Data, 0x24); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x24); } }
        public override int Weight { get { return BitConverter.ToUInt16(Data, 0x26); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x26); } }
    }
}
