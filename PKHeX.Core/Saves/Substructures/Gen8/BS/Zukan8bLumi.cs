using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    /// <summary>
    /// Pokédex structure used for Brilliant Diamond &amp; Shining Pearl.
    /// </summary>
    /// <remarks>size 0x30B8, struct_name ZUKAN_WORK</remarks>
    public sealed class Zukan8bLumi : Zukan8b
    {
        /* Structure Notes:
            u32 [493] state: None/HeardOf/Seen/Captured
            bool[493] maleShiny
            bool[493] femaleShiny
            bool[493] male
            bool[493] female
            bool[28] Unown Form
            bool[28] Unown FormShiny
            bool[4] Castform
            bool[4] Castform
            bool[4] Deoxys
            bool[4] Deoxys
            bool[3] Burmy
            bool[3] Burmy
            bool[3] Wormadam
            bool[3] Wormadam
            bool[3] Wormadam
            bool[3] Wormadam
            bool[3] Mothim
            bool[3] Mothim
            bool[2] Cherrim
            bool[2] Cherrim
            bool[2] Shellos
            bool[2] Shellos
            bool[2] Gastrodon
            bool[2] Gastrodon
            bool[6] Rotom
            bool[6] Rotom
            bool[2] Giratina
            bool[2] Giratina
            bool[2] Shaymin
            bool[2] Shaymin
            bool[18] Arceus
            bool[18] Arceus
            u32 [493] language flags
            bool regional dex obtained
            bool national dex obtained
         */

        private const int OFS_STATE = 0;

        private static PersonalTable8BDSP Personal => PersonalTable.BDSPLUMI;

        public Zukan8bLumi(SAV8BSLuminescent sav, int dex) : base(sav, dex) { }

        private int GetStateStructOffset(int species)
        {
            if ((uint)species > (uint)Species.MAX_COUNT - 1)
                throw new ArgumentOutOfRangeException(nameof(species));
            return OFS_STATE + (species / 2);
        }

        private int GetBooleanStructOffset(int index, int baseOffset)
        {
            if ((uint)index > (uint)Species.MAX_COUNT - 1)
                throw new ArgumentOutOfRangeException(nameof(index));
            return baseOffset + (index / 8);
        }

        private void SetNibble(ref byte bitFlag, byte bitIndex, byte nibbleValue)
        {
            bitFlag = (byte)(bitFlag & ~(0xF << bitIndex) | (nibbleValue << bitIndex));
        }

        private void SetBit(ref byte bitFlag, byte bitIndex, bool bitValue)
        {
            bitFlag = (byte)(bitFlag & ~(0xF << bitIndex) | ((bitValue ? 1 : 0) << bitIndex));
        }

        public override ZukanState8b GetState(ushort species) => (ZukanState8b)(SAV.Data[PokeDex + GetStateStructOffset(species)] >> ((species & 1) * 4) & 0xF);

        public override void SetState(ushort species, ZukanState8b state) => SetNibble(ref SAV.Data[PokeDex + GetStateStructOffset(species)], (byte)((species & 1) * 4), (byte)state);

        public override bool GetBoolean(int index, int baseOffset) => (SAV.Data[PokeDex + GetBooleanStructOffset(index, baseOffset)] >> (index % 8) & 1) == 1;

        public override void SetBoolean(int index, int baseOffset, bool value) => SetBit(ref SAV.Data[PokeDex + GetBooleanStructOffset(index, baseOffset)], (byte)(index % 8), value);

        public override bool GetLanguageFlag(ushort species, int language)
        {
            if ((uint)species > (uint)Species.MAX_COUNT - 1)
                throw new ArgumentOutOfRangeException(nameof(species));
            // Lang flags in 1.3.0 Lumi Revision 1 Save hasn't been changed to bitfields
            else if ((uint)species > Legal.MaxSpeciesID_4)
                return false;

            var languageBit = GetLanguageBit(language);
            if (languageBit == -1)
                return false;

            var index = species - 1;
            var offset = OFS_LANGUAGE + (sizeof(int) * index);
            var current = ReadInt32LittleEndian(SAV.Data.AsSpan(PokeDex + offset));
            return (current & (1 << languageBit)) != 0;
        }

        public override void SetLanguageFlag(ushort species, int language, bool value)
        {
            if ((uint)species > (uint)Species.MAX_COUNT - 1)
                throw new ArgumentOutOfRangeException(nameof(species));
            // Lang flags in 1.3.0 Lumi Revision 1 Save hasn't been changed to bitfields
            else if ((uint)species > Legal.MaxSpeciesID_4)
                return;

            var languageBit = GetLanguageBit(language);
            if (languageBit == -1)
                return;

            var index = species - 1;
            var offset = OFS_LANGUAGE + (sizeof(int) * index);
            var current = ReadInt32LittleEndian(SAV.Data.AsSpan(PokeDex + offset));
            var mask = (1 << languageBit);
            var update = value ? current | mask : current & ~(mask);
            WriteInt32LittleEndian(SAV.Data.AsSpan(PokeDex + offset), update);
        }

        public override void SetLanguageFlags(ushort species, int value)
        {
            if ((uint)species > (uint)Species.MAX_COUNT - 1)
                throw new ArgumentOutOfRangeException(nameof(species));
            // Lang flags in 1.3.0 Lumi Revision 1 Save hasn't been changed to bitfields
            else if ((uint)species > Legal.MaxSpeciesID_4)
                return;

            var index = species - 1;
            var offset = OFS_LANGUAGE + (sizeof(int) * index);
            WriteInt32LittleEndian(SAV.Data.AsSpan(PokeDex + offset), value);
        }

        public override void CaughtAll(bool shinyToo = false)
        {
            var pt = Personal;
            for (ushort species = 1; species <= (uint)Species.MAX_COUNT - 1; species++)
            {
                SetState(species, ZukanState8b.Caught);
                var pi = pt[species];
                var m = !pi.OnlyFemale;
                var f = !pi.OnlyMale;
                SetGenderFlags(species, m, f, m && shinyToo, f && shinyToo);
                SetLanguageFlag(species, SAV.Language, true);
            }
        }

        public override void SetAllSeen(bool value = true, bool shinyToo = false)
        {
            var pt = Personal;
            for (ushort species = 1; species <= (uint)Species.MAX_COUNT - 1; species++)
            {
                if (value)
                {
                    if (!GetSeen(species))
                        SetState(species, ZukanState8b.Seen);
                    var pi = pt[species];
                    var m = !pi.OnlyFemale;
                    var f = !pi.OnlyMale;
                    SetGenderFlags(species, m, f, m && shinyToo, f && shinyToo);
                }
                else
                {
                    ClearDexEntryAll(species);
                }
            }
        }

        public override void SetDexEntryAll(ushort species, bool shinyToo = false)
        {
            SetState(species, ZukanState8b.Caught);

            var pt = Personal;
            var pi = pt[species];
            var m = !pi.OnlyFemale;
            var f = !pi.OnlyMale;
            SetGenderFlags(species, m, f, m && shinyToo, f && shinyToo);

            var formCount = GetFormCount(species);
            if (formCount is not 0)
            {
                for (byte form = 0; form < formCount; form++)
                {
                    SetHasFormFlag(species, form, false, true);
                    if (shinyToo)
                        SetHasFormFlag(species, form, true, true);
                }
            }

            SetLanguageFlags(species, LANGUAGE_ALL);
        }

        public override void ClearDexEntryAll(ushort species)
        {
            SetState(species, ZukanState8b.None);
            SetGenderFlags(species, false, false, false, false);

            var formCount = GetFormCount(species);
            if (formCount is not 0)
            {
                for (byte form = 0; form < formCount; form++)
                {
                    SetHasFormFlag(species, form, false, false);
                    SetHasFormFlag(species, form, true, false);
                }
            }

            SetLanguageFlags(species, LANGUAGE_NONE);
        }
    }
}
