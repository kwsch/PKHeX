using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from the <see cref="GameVersion.BDSP"/> games.
    /// </summary>
    public sealed class PersonalInfoBDSP : PersonalInfo
    {
        public const int SIZE = 0x44;
        private const int CountTM = 100;

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
        public int Item1 { get => BitConverter.ToInt16(Data, 0x0C); set => BitConverter.GetBytes((short)value).CopyTo(Data, 0x0C); }
        public int Item2 { get => BitConverter.ToInt16(Data, 0x0E); set => BitConverter.GetBytes((short)value).CopyTo(Data, 0x0E); }
        public int Item3 { get => BitConverter.ToInt16(Data, 0x10); set => BitConverter.GetBytes((short)value).CopyTo(Data, 0x10); }
        public override int Gender { get => Data[0x12]; set => Data[0x12] = (byte)value; }
        public override int HatchCycles { get => Data[0x13]; set => Data[0x13] = (byte)value; }
        public override int BaseFriendship { get => Data[0x14]; set => Data[0x14] = (byte)value; }
        public override int EXPGrowth { get => Data[0x15]; set => Data[0x15] = (byte)value; }
        public override int EggGroup1 { get => Data[0x16]; set => Data[0x16] = (byte)value; }
        public override int EggGroup2 { get => Data[0x17]; set => Data[0x17] = (byte)value; }
        public int Ability1 { get => BitConverter.ToUInt16(Data, 0x18); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x18); }
        public int Ability2 { get => BitConverter.ToUInt16(Data, 0x1A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1A); }
        public int AbilityH { get => BitConverter.ToUInt16(Data, 0x1C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1C); }
        public override int EscapeRate { get => 0; set { } } // moved?
        protected internal override int FormStatsIndex { get => BitConverter.ToUInt16(Data, 0x1E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1E); }
        public override int FormSprite { get => 0; set { } } // No longer defined in personal
        public override int FormCount { get => Data[0x20]; set => Data[0x20] = (byte)value; }
        public override int Color { get => Data[0x21] & 0x3F; set => Data[0x21] = (byte)((Data[0x21] & 0xC0) | (value & 0x3F)); }
        public bool IsPresentInGame { get => ((Data[0x21] >> 6) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x40) | (value ? 0x40 : 0)); }
        public bool SpriteForm { get => false; set { } } // Unspecified in table
        public override int BaseEXP { get => BitConverter.ToUInt16(Data, 0x22); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x22); }
        public override int Height { get => BitConverter.ToUInt16(Data, 0x24); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x24); }
        public override int Weight { get => BitConverter.ToUInt16(Data, 0x26); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x26); }

      //public uint TM1 { get => BitConverter.ToUInt32(Data, 0x28); set => BitConverter.GetBytes(value).CopyTo(Data, 0x28); }
      //public uint TM2 { get => BitConverter.ToUInt32(Data, 0x2C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x2C); }
      //public uint TM3 { get => BitConverter.ToUInt32(Data, 0x30); set => BitConverter.GetBytes(value).CopyTo(Data, 0x30); }
      //public uint TM4 { get => BitConverter.ToUInt32(Data, 0x34); set => BitConverter.GetBytes(value).CopyTo(Data, 0x34); }
      //public uint Tutor { get => BitConverter.ToUInt32(Data, 0x38); set => BitConverter.GetBytes(value).CopyTo(Data, 0x38); }

        public int Species { get => BitConverter.ToUInt16(Data, 0x3C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x3C); }
        public int HatchSpecies { get => BitConverter.ToUInt16(Data, 0x3E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x3E); }
        public int HatchFormIndex { get => BitConverter.ToUInt16(Data, 0x40); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x40); }
        public int PokeDexIndex { get => BitConverter.ToUInt16(Data, 0x42); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x42); }

        public PersonalInfoBDSP(byte[] data) : base(data)
        {
            TMHM = new bool[CountTM];
            for (var i = 0; i < CountTM; i++)
                TMHM[i] = FlagUtil.GetFlag(Data, 0x28 + (i >> 3), i);

            // 0x38-0x3B type tutors, but only 8 bits are valid flags.
            var typeTutors = new bool[8];
            for (int i = 0; i < typeTutors.Length; i++)
                typeTutors[i] = FlagUtil.GetFlag(Data, 0x38, i);
            TypeTutors = typeTutors;
        }

        public override byte[] Write()
        {
            for (var i = 0; i < CountTM; i++)
                FlagUtil.SetFlag(Data, 0x28 + (i >> 3), i, TMHM[i]);
            for (int i = 0; i < TypeTutors.Length; i++)
                FlagUtil.SetFlag(Data, 0x38, i, TypeTutors[i]);
            return Data;
        }

        public override IReadOnlyList<int> Items
        {
            get => new[] { Item1, Item2, Item3 };
            set
            {
                if (value.Count != 3) return;
                Item1 = value[0];
                Item2 = value[1];
                Item3 = value[2];
            }
        }

        public override IReadOnlyList<int> Abilities
        {
            get => new[] { Ability1, Ability2, AbilityH };
            set
            {
                if (value.Count != 3) return;
                Ability1 = value[0];
                Ability2 = value[1];
                AbilityH = value[2];
            }
        }

        public override int GetAbilityIndex(int abilityID) => abilityID == Ability1 ? 0 : abilityID == Ability2 ? 1 : abilityID == AbilityH ? 2 : -1;

        /// <summary>
        /// Checks if the entry shows up in any of the built-in Pokédex.
        /// </summary>
        public bool IsInDex => PokeDexIndex != 0;
    }
}
