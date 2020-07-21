namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from Generation 1 games.
    /// </summary>
    public sealed class PersonalInfoG1 : PersonalInfo
    {
        public const int SIZE = 0x1C;

        public PersonalInfoG1(byte[] data) : base(data)
        {
            TMHM = GetBits(Data, 0x14, 0x8);
        }

        public override byte[] Write()
        {
            SetBits(TMHM).CopyTo(Data, 0x14);
            return Data;
        }

        public int DEX_ID { get => Data[0x00]; set => Data[0x00] = (byte)value; }
        public override int HP { get => Data[0x01]; set => Data[0x01] = (byte)value; }
        public override int ATK { get => Data[0x02]; set => Data[0x02] = (byte)value; }
        public override int DEF { get => Data[0x03]; set => Data[0x03] = (byte)value; }
        public override int SPE { get => Data[0x04]; set => Data[0x04] = (byte)value; }
        public int SPC { get => Data[0x05]; set => Data[0x05] = (byte)value; }
        public override int SPA { get => SPC; set => SPC = value; }
        public override int SPD { get => SPC; set => SPC = value; }
        public override int Type1 { get => Data[0x06]; set => Data[0x06] = (byte)value; }
        public override int Type2 { get => Data[0x07]; set => Data[0x07] = (byte)value; }
        public override int CatchRate { get => Data[0x08]; set => Data[0x08] = (byte)value; }
        public override int BaseEXP { get => Data[0x09]; set => Data[0x09] = (byte)value; }
        public int Move1 { get => Data[0x0F]; set => Data[0x0F] = (byte)value; }
        public int Move2 { get => Data[0x10]; set => Data[0x10] = (byte)value; }
        public int Move3 { get => Data[0x11]; set => Data[0x11] = (byte)value; }
        public int Move4 { get => Data[0x12]; set => Data[0x12] = (byte)value; }
        public override int EXPGrowth { get => Data[0x13]; set => Data[0x13] = (byte)value; }

        // EV Yields are just aliases for base stats in Gen I
        public override int EV_HP { get => HP; set { } }
        public override int EV_ATK { get => ATK; set { } }
        public override int EV_DEF { get => DEF; set { } }
        public override int EV_SPE { get => SPE; set { } }
        public int EV_SPC => SPC;
        public override int EV_SPA { get => EV_SPC; set { } }
        public override int EV_SPD { get => EV_SPC; set { } }

        // Future game values, unused
        public override int[] Items { get => new[] { 0, 0 }; set { } }
        public override int EggGroup1 { get => 0; set { } }
        public override int EggGroup2 { get => 0; set { } }
        public override int[] Abilities { get => new[] { 0, 0 }; set { } }
        public override int Gender { get; set; }
        public override int HatchCycles { get => 0; set { } }
        public override int BaseFriendship { get => 0; set { } }
        public override int EscapeRate { get => 0; set { } }
        public override int Color { get => 0; set { } }

        public int[] Moves
        {
            get => new[] { Move1, Move2, Move3, Move4 };
            set
            {
                if (value.Length != 4) return;
                Move1 = value[0];
                Move2 = value[1];
                Move3 = value[2];
                Move4 = value[3];
            }
        }
    }
}
