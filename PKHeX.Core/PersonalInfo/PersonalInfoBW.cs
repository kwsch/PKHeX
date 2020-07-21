using System;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from the Black &amp; White games.
    /// </summary>
    public class PersonalInfoBW : PersonalInfo
    {
        public const int SIZE = 0x3C;

        public PersonalInfoBW(byte[] data) : base(data)
        {
            // Unpack TMHM & Tutors
            TMHM = GetBits(Data, 0x28, 0x10);
            TypeTutors = GetBits(Data, 0x38, 0x4);
        }

        public override byte[] Write()
        {
            SetBits(TMHM).CopyTo(Data, 0x28);
            SetBits(TypeTutors).CopyTo(Data, 0x38);
            return Data;
        }

        public override int HP { get => Data[0x00]; set => Data[0x00] = (byte)value; }
        public override int ATK { get => Data[0x01]; set => Data[0x01] = (byte)value; }
        public override int DEF { get => Data[0x02]; set => Data[0x02] = (byte)value; }
        public override int SPE { get => Data[0x03]; set => Data[0x03] = (byte)value; }
        public override int SPA { get => Data[0x04]; set => Data[0x04] = (byte)value; }
        public override int SPD { get => Data[0x05]; set => Data[0x05] = (byte)value; }
        public override int Type1 { get => Data[0x06]; set => Data[0x06] = (byte)value; }
        public override int Type2 { get => Data[0x07]; set => Data[0x07] = (byte)value; }
        public override int CatchRate { get => Data[0x08]; set => Data[0x08] = (byte)value; }
        public override int EvoStage { get => Data[0x09]; set => Data[0x09] = (byte)value; }
        private int EVYield { get => BitConverter.ToUInt16(Data, 0x0A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0A); }
        public override int EV_HP { get => EVYield >> 0 & 0x3; set => EVYield = (EVYield & ~(0x3 << 0)) | (value & 0x3) << 0; }
        public override int EV_ATK { get => EVYield >> 2 & 0x3; set => EVYield = (EVYield & ~(0x3 << 2)) | (value & 0x3) << 2; }
        public override int EV_DEF { get => EVYield >> 4 & 0x3; set => EVYield = (EVYield & ~(0x3 << 4)) | (value & 0x3) << 4; }
        public override int EV_SPE { get => EVYield >> 6 & 0x3; set => EVYield = (EVYield & ~(0x3 << 6)) | (value & 0x3) << 6; }
        public override int EV_SPA { get => EVYield >> 8 & 0x3; set => EVYield = (EVYield & ~(0x3 << 8)) | (value & 0x3) << 8; }
        public override int EV_SPD { get => EVYield >> 10 & 0x3; set => EVYield = (EVYield & ~(0x3 << 10)) | (value & 0x3) << 10; }
        public bool Telekenesis { get => (EVYield >> 12 & 1) == 1; set => EVYield = (EVYield & ~(0x1 << 12)) | (value ? 1 : 0) << 12; }
        public int Item1 { get => BitConverter.ToInt16(Data, 0x0C); set => BitConverter.GetBytes((short)value).CopyTo(Data, 0x0C); }
        public int Item2 { get => BitConverter.ToInt16(Data, 0x0E); set => BitConverter.GetBytes((short)value).CopyTo(Data, 0x0E); }
        public int Item3 { get => BitConverter.ToInt16(Data, 0x10); set => BitConverter.GetBytes((short)value).CopyTo(Data, 0x10); }
        public override int Gender { get => Data[0x12]; set => Data[0x12] = (byte)value; }
        public override int HatchCycles { get => Data[0x13]; set => Data[0x13] = (byte)value; }
        public override int BaseFriendship { get => Data[0x14]; set => Data[0x14] = (byte)value; }
        public override int EXPGrowth { get => Data[0x15]; set => Data[0x15] = (byte)value; }
        public override int EggGroup1 { get => Data[0x16]; set => Data[0x16] = (byte)value; }
        public override int EggGroup2 { get => Data[0x17]; set => Data[0x17] = (byte)value; }
        public int Ability1 { get => Data[0x18]; set => Data[0x18] = (byte)value; }
        public int Ability2 { get => Data[0x19]; set => Data[0x19] = (byte)value; }
        public int AbilityH { get => Data[0x1A]; set => Data[0x1A] = (byte)value; }
        public override int EscapeRate { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
        protected internal override int FormStatsIndex { get => BitConverter.ToUInt16(Data, 0x1C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1C); }
        public override int FormeSprite { get => BitConverter.ToUInt16(Data, 0x1E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1E); }
        public override int FormeCount { get => Data[0x20]; set => Data[0x20] = (byte)value; }
        public override int Color { get => Data[0x21] & 0x3F; set => Data[0x21] = (byte)((Data[0x21] & 0xC0) | (value & 0x3F)); }
        public bool SpriteFlip { get => ((Data[0x21] >> 6) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x40) | (value ? 0x40 : 0)); }
        public bool SpriteForme { get => ((Data[0x21] >> 7) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x80) | (value ? 0x80 : 0)); }

        public override int BaseEXP { get => BitConverter.ToUInt16(Data, 0x22); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x22); }
        public override int Height { get => BitConverter.ToUInt16(Data, 0x24); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x24); }
        public override int Weight { get => BitConverter.ToUInt16(Data, 0x26); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x26); }

        public override int[] Items
        {
            get => new[] { Item1, Item2, Item3 };
            set
            {
                if (value.Length != 3) return;
                Item1 = value[0];
                Item2 = value[1];
                Item3 = value[2];
            }
        }

        public override int[] Abilities
        {
            get => new[] { Ability1, Ability2, AbilityH };
            set
            {
                if (value.Length != 3) return;
                Ability1 = (byte)value[0];
                Ability2 = (byte)value[1];
                AbilityH = (byte)value[2];
            }
        }

        public bool HasHiddenAbility => AbilityH != Ability1;
    }
}
