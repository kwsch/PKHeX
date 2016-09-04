using System.Linq;

namespace PKHeX
{
    public class PersonalInfoG2 : PersonalInfo
    {
        protected PersonalInfoG2() { }
        public const int SIZE = 0x20;
        public PersonalInfoG2(byte[] data)
        {
            if (data.Length != SIZE)
                return;

            Data = data;
            TMHM = getBits(Data.Skip(0x18).Take(0x8).ToArray());
        }
        public override byte[] Write()
        {
            setBits(TMHM).CopyTo(Data, 0x18);
            return Data;
        }

        public int DEX_ID { get { return Data[0x00]; } set { Data[0x00] = (byte)value; } }
        public override int HP { get { return Data[0x01]; } set { Data[0x01] = (byte)value; } }
        public override int ATK { get { return Data[0x02]; } set { Data[0x02] = (byte)value; } }
        public override int DEF { get { return Data[0x03]; } set { Data[0x03] = (byte)value; } }
        public override int SPE { get { return Data[0x04]; } set { Data[0x04] = (byte)value; } }
        public override int SPA { get { return Data[0x05]; } set { Data[0x05] = (byte)value; } }
        public override int SPD { get { return Data[0x06]; } set { Data[0x06] = (byte)value; } }
        public override int[] Types
        {
            get { return new int[] { Data[0x07], Data[0x08] }; }
            set
            {
                if (value?.Length != 2) return;
                Data[0x07] = (byte)value[0];
                Data[0x08] = (byte)value[1];
            }
        }
        public override int CatchRate { get { return Data[0x09]; } set { Data[0x09] = (byte)value; } }
        public override int BaseEXP { get { return Data[0x0A]; } set { Data[0x0A] = (byte)value; } }

        public override int[] Items
        {
            get { return new int[] { Data[0xB], Data[0xC] }; }
            set
            {
                if (value?.Length != 2) return;
                Data[0xB] = (byte) value[0];
                Data[0xC] = (byte) value[1];
            }
        }
        public override int Gender { get { return Data[0xD]; } set { Data[0xD] = (byte)value; } }
        public override int HatchCycles { get { return Data[0xF]; } set { Data[0xF] = (byte)value; } }
        public override int EXPGrowth { get { return Data[0x16]; } set { Data[0x16] = (byte)value; } }
        public override int[] EggGroups
        {
            get { return new[] { Data[0x17] >> 4, Data[0x17] & 0xF }; }
            set
            {
                if (value?.Length != 2) return;
                Data[0x17] = (byte)(((value[0] & 0xF) << 4) | (value[1] & 0xF));
            }
        }

        // EV Yields are just aliases for base stats in Gen I
        public override int EV_HP { get { return HP; } set { } }
        public override int EV_ATK { get { return ATK; } set { } }
        public override int EV_DEF { get { return DEF; } set { } }
        public override int EV_SPE { get { return SPE; } set { } }
        public override int EV_SPA { get { return SPA; } set { } }
        public override int EV_SPD { get { return SPD; } set { } }

        // Future game values, unused
        public override int[] Abilities { get { return new[] { 0, 0 }; } set { } }
        public override int BaseFriendship { get { return 70; } set { } }
        public override int EscapeRate { get { return 0; } set { } }
        public override int Color { get { return 0; } set { } }
    }
}
