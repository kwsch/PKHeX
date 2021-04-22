using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from the <see cref="GameVersion.SWSH"/> games.
    /// </summary>
    public sealed class PersonalInfoSWSH : PersonalInfo
    {
        public const int SIZE = 0xB0;

        public PersonalInfoSWSH(byte[] data) : base(data)
        {
            TMHM = new bool[200];
            for (var i = 0; i < 100; i++)
            {
                TMHM[i]       = FlagUtil.GetFlag(Data, 0x28 + (i >> 3), i);
                TMHM[i + 100] = FlagUtil.GetFlag(Data, 0x3C + (i >> 3), i);
            }

            // 0x38-0x3B type tutors, but only 8 bits are valid flags.
            var typeTutors = new bool[8];
            for (int i = 0; i < typeTutors.Length; i++)
                typeTutors[i] = FlagUtil.GetFlag(Data, 0x38, i);
            TypeTutors = typeTutors;

            // 0xA8-0xAF are armor type tutors, one bit for each type
            var armorTutors = new bool[18];
            for (int i = 0; i < armorTutors.Length; i++)
                armorTutors[i] = FlagUtil.GetFlag(Data, 0xA8 + (i >> 3), i);
            SpecialTutors = new[]
            {
                armorTutors,
            };
        }

        public override byte[] Write()
        {
            for (var i = 0; i < 100; i++)
            {
                FlagUtil.SetFlag(Data, 0x28 + (i >> 3), i, TMHM[i]);
                FlagUtil.SetFlag(Data, 0x3C + (i >> 3), i, TMHM[i + 100]);
            }
            for (int i = 0; i < TypeTutors.Length; i++)
                FlagUtil.SetFlag(Data, 0x38, i, TypeTutors[i]);
            for (int i = 0; i < SpecialTutors[0].Length; i++)
                FlagUtil.SetFlag(Data, 0xA8 + (i >> 3), i, SpecialTutors[0][i]);
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
        public override int FormSprite { get => BitConverter.ToUInt16(Data, 0x1E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1E); } // ???
        public override int FormCount { get => Data[0x20]; set => Data[0x20] = (byte)value; }
        public override int Color { get => Data[0x21] & 0x3F; set => Data[0x21] = (byte)((Data[0x21] & 0xC0) | (value & 0x3F)); }
        public bool IsPresentInGame { get => ((Data[0x21] >> 6) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x40) | (value ? 0x40 : 0)); }
        public bool SpriteForm { get => ((Data[0x21] >> 7) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x80) | (value ? 0x80 : 0)); }
        public override int BaseEXP { get => BitConverter.ToUInt16(Data, 0x22); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x22); }
        public override int Height { get => BitConverter.ToUInt16(Data, 0x24); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x24); }
        public override int Weight { get => BitConverter.ToUInt16(Data, 0x26); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x26); }

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

        public int Species { get => BitConverter.ToUInt16(Data, 0x4C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x4C); }

        public int HatchSpecies { get => BitConverter.ToUInt16(Data, 0x56); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x56); }
        public int LocalFormIndex { get => BitConverter.ToUInt16(Data, 0x58); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x58); } // local region base form
        public ushort RegionalFlags { get => BitConverter.ToUInt16(Data, 0x5A); set => BitConverter.GetBytes(value).CopyTo(Data, 0x5A); }
        public bool IsRegionalForm { get => (RegionalFlags & 1) == 1; set => BitConverter.GetBytes((ushort)((RegionalFlags & 0xFFFE) | (value ? 1 : 0))).CopyTo(Data, 0x5A); }
        public int PokeDexIndex { get => BitConverter.ToUInt16(Data, 0x5C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5C); }
        public int RegionalFormIndex { get => BitConverter.ToUInt16(Data, 0x5E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5E); } // form index of this entry
        public int ArmorDexIndex { get => BitConverter.ToUInt16(Data, 0xAC); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xAC); }
        public int CrownDexIndex { get => BitConverter.ToUInt16(Data, 0xAE); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xAE); }

        /// <summary>
        /// Gets the Forme that any offspring will hatch with, assuming it is holding an Everstone.
        /// </summary>
        public int HatchFormIndexEverstone => IsRegionalForm ? RegionalFormIndex : LocalFormIndex;

        /// <summary>
        /// Checks if the entry shows up in any of the built-in Pokédex.
        /// </summary>
        public bool IsInDex => PokeDexIndex != 0 || ArmorDexIndex != 0 || CrownDexIndex != 0;

        public bool HasHiddenAbility => AbilityH != Ability1;
    }
}
