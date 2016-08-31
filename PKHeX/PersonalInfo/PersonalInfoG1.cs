using System.Linq;

namespace PKHeX
{
    public class PersonalInfoG1 : PersonalInfo
    {
        protected PersonalInfoG1() { }
        public const int SIZE = 0x1C;
        public PersonalInfoG1(byte[] data)
        {
            if (data.Length != SIZE)
                return;

            Data = data;
            TMHM = getBits(Data.Skip(0x14).Take(0x8).ToArray());
        }
        public override byte[] Write()
        {
            setBits(TMHM).CopyTo(Data, 0x14);
            return Data;
        }

        public int DEX_ID { get { return Data[0x00]; } set { Data[0x00] = (byte)value; } }
        public override int HP { get { return Data[0x01]; } set { Data[0x01] = (byte)value; } }
        public override int ATK { get { return Data[0x02]; } set { Data[0x02] = (byte)value; } }
        public override int DEF { get { return Data[0x03]; } set { Data[0x03] = (byte)value; } }
        public override int SPE { get { return Data[0x04]; } set { Data[0x04] = (byte)value; } }
        public int SPC { get { return Data[0x05]; } set { Data[0x05] = (byte)value; } }
        public override int SPA { get { return SPC; } set { SPC = value; } }
        public override int SPD { get { return SPC; } set { SPC = value; } }
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
        public override int BaseEXP { get { return Data[0x09]; } set { Data[0x09] = (byte)value; } }
        public int Move1 { get { return Data[0x0A]; } set { Data[0x0A] = (byte)value; } }
        public int Move2 { get { return Data[0x0B]; } set { Data[0x0B] = (byte)value; } }
        public int Move3 { get { return Data[0x0C]; } set { Data[0x0C] = (byte)value; } }
        public int Move4 { get { return Data[0x0D]; } set { Data[0x0D] = (byte)value; } }
        public override int EXPGrowth { get { return Data[0x13]; } set { Data[0x13] = (byte)value; } }

        // EV Yields are just aliases for base stats in Gen I
        public override int EV_HP { get { return HP; } set { } }
        public override int EV_ATK { get { return ATK; } set { } }
        public override int EV_DEF { get { return DEF; } set { } }
        public override int EV_SPE { get { return SPE; } set { } }
        public int EV_SPC { get { return SPC; } set { } }
        public override int EV_SPA { get { return EV_SPC; } set { } }
        public override int EV_SPD { get { return EV_SPC; } set { } }

        // Future game values, unused
        public override int[] Items { get { return new[] { 0, 0 }; } set { } }
        public override int[] EggGroups { get { return new[] { 0, 0 }; } set { } }
        public override int[] Abilities { get { return new[] { 0, 0 }; } set { } }
        public override int Gender { get { return 0; } set { } }
        public override int HatchCycles { get { return 0; } set { } }
        public override int BaseFriendship { get { return 0; } set { } }
        public override int EscapeRate { get { return 0; } set { } }
        public override int Color { get { return 0; } set { } }
    }
}
