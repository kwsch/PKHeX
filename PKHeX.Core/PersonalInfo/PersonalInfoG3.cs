using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from Generation 3 games.
    /// </summary>
    public class PersonalInfoG3 : PersonalInfo
    {
        public const int SIZE = 0x1C;

        public PersonalInfoG3(byte[] data) : base(data)
        {
        }

        public override byte[] Write() => Data;

        public sealed override int HP { get => Data[0x00]; set => Data[0x00] = (byte)value; }
        public sealed override int ATK { get => Data[0x01]; set => Data[0x01] = (byte)value; }
        public sealed override int DEF { get => Data[0x02]; set => Data[0x02] = (byte)value; }
        public sealed override int SPE { get => Data[0x03]; set => Data[0x03] = (byte)value; }
        public sealed override int SPA { get => Data[0x04]; set => Data[0x04] = (byte)value; }
        public sealed override int SPD { get => Data[0x05]; set => Data[0x05] = (byte)value; }
        public sealed override int Type1 { get => Data[0x06]; set => Data[0x06] = (byte)value; }
        public sealed override int Type2 { get => Data[0x07]; set => Data[0x07] = (byte)value; }
        public sealed override int CatchRate { get => Data[0x08]; set => Data[0x08] = (byte)value; }
        public sealed override int BaseEXP { get => Data[0x09]; set => Data[0x09] = (byte)value; }
        private int EVYield { get => BitConverter.ToUInt16(Data, 0x0A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0A); }
        public sealed override int EV_HP { get => EVYield >> 0 & 0x3; set => EVYield = (EVYield & ~(0x3 << 0)) | (value & 0x3) << 0; }
        public sealed override int EV_ATK { get => EVYield >> 2 & 0x3; set => EVYield = (EVYield & ~(0x3 << 2)) | (value & 0x3) << 2; }
        public sealed override int EV_DEF { get => EVYield >> 4 & 0x3; set => EVYield = (EVYield & ~(0x3 << 4)) | (value & 0x3) << 4; }
        public sealed override int EV_SPE { get => EVYield >> 6 & 0x3; set => EVYield = (EVYield & ~(0x3 << 6)) | (value & 0x3) << 6; }
        public sealed override int EV_SPA { get => EVYield >> 8 & 0x3; set => EVYield = (EVYield & ~(0x3 << 8)) | (value & 0x3) << 8; }
        public sealed override int EV_SPD { get => EVYield >> 10 & 0x3; set => EVYield = (EVYield & ~(0x3 << 10)) | (value & 0x3) << 10; }
        public int Item1 { get => BitConverter.ToInt16(Data, 0xC); set => BitConverter.GetBytes((short)value).CopyTo(Data, 0xC); }
        public int Item2 { get => BitConverter.ToInt16(Data, 0xE); set => BitConverter.GetBytes((short)value).CopyTo(Data, 0xE); }
        public sealed override int Gender { get => Data[0x10]; set => Data[0x10] = (byte)value; }
        public sealed override int HatchCycles { get => Data[0x11]; set => Data[0x11] = (byte)value; }
        public sealed override int BaseFriendship { get => Data[0x12]; set => Data[0x12] = (byte)value; }
        public sealed override int EXPGrowth { get => Data[0x13]; set => Data[0x13] = (byte)value; }
        public sealed override int EggGroup1 { get => Data[0x14]; set => Data[0x14] = (byte)value; }
        public sealed override int EggGroup2 { get => Data[0x15]; set => Data[0x15] = (byte)value; }
        public int Ability1 { get => Data[0x16]; set => Data[0x16] = (byte)value; }
        public int Ability2 { get => Data[0x17]; set => Data[0x17] = (byte)value; }
        public sealed override int EscapeRate { get => Data[0x18]; set => Data[0x18] = (byte)value; }
        public sealed override int Color { get => Data[0x19] & 0x7F; set => Data[0x19] = (byte)((Data[0x19] & 0x80) | value); }
        public bool NoFlip { get => Data[0x19] >> 7 == 1; set => Data[0x19] = (byte)(Color | (value ? 0x80 : 0)); }

        public sealed override IReadOnlyList<int> Items
        {
            get => new[] { Item1, Item2 };
            set
            {
                if (value.Count != 2) return;
                Item1 = value[0];
                Item2 = value[1];
            }
        }

        public sealed override IReadOnlyList<int> Abilities
        {
            get => new[] { Ability1, Ability2 };
            set
            {
                if (value.Count != 2) return;
                Ability1 = (byte)value[0];
                Ability2 = (byte)value[1];
            }
        }

        public sealed override int GetAbilityIndex(int abilityID) => abilityID == Ability1 ? 0 : abilityID == Ability2 ? 1 : -1;
        public int GetAbility(bool second) => second && HasSecondAbility ? Ability2 : Ability1;

        public bool HasSecondAbility => Ability1 != Ability2;
    }
}
