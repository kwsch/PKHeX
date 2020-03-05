using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public class Zukan8 : ZukanBase
    {
        private readonly SCBlock Block;
        public static readonly IReadOnlyDictionary<int, int> DexLookup = GetDexLookup(PersonalTable.SWSH);

        public Zukan8(SAV8SWSH sav, SCBlock block) : base(sav, 0) => Block = block;

        private bool GetFlag(int ofs, int bitIndex) => FlagUtil.GetFlag(Block.Data, ofs + (bitIndex >> 3), bitIndex);
        private void SetFlag(int ofs, int bitIndex, bool value = true) => FlagUtil.SetFlag(Block.Data, ofs + (bitIndex >> 3), bitIndex, value);

        private static Dictionary<int, int> GetDexLookup(PersonalTable pt)
        {
            var lookup = new Dictionary<int, int>();
            for (int i = 1; i <= pt.MaxSpeciesID; i++)
            {
                var p = (PersonalInfoSWSH) pt[i];
                var index = p.PokeDexIndex;
                if (index != 0)
                    lookup.Add(i, index);
            }
            return lookup;
        }

        private static int GetDexLangFlag(int lang)
        {
            if (lang > 10 || lang == 6 || lang <= 0)
                return -1; // invalid language

            if (lang >= 7) // skip over langID 6 (unused)
                lang--;
            lang--; // skip over langID 0 (unused) => [0-8]
            return lang;
        }

        public static IList<string> GetEntryNames(IReadOnlyList<string> Species)
        {
            var dex = new List<string>();
            foreach (var d in DexLookup)
            {
                var spec = d.Key;
                var index = d.Value;
                var name = $"{index:000} - {Species[spec]}";
                dex.Add(name);
            }
            dex.Sort();
            return dex;
        }

        #region Structure
        private const int EntrySize = 0x30;

        // First 0x20 bytes are for seen flags, allocated as 4 QWORD values.
        private const int SeenRegionCount = 4;
        private const int SeenRegionSize = sizeof(ulong);
        // not_shiny_gender_0,
        // not_shiny_gender_1,
        // shiny_gender_0,
        // shiny_gender_1
        // Each QWORD stores the following bits:
        // - FormsSeen[63], default form is index 0.
        // - Gigantimax:1

        // Next 4 bytes are for obtained info (u32)
        private const int OFS_CAUGHT = 0x20;
        // Owned:1 (posessed by player)
        // OwnedGigantamax:1 (Gigantamaxed by player in battle)
        // LanguagesObtained:2-14 (flags)
        // DisplayFormID:15-27 (value)
        // DisplayGigantamaxInstead:28 (flag)
        // DisplayGender:29/30 (m/f)
        // DisplayShiny:31 (flag)

        // Next 4 bytes are Battled Count (u32)
        private const int OFS_BATTLED = 0x24;

        // Next 4 bytes are Unused(?)
        private const int OFS_UNK1 = 0x28;
        // Next 4 bytes are Unused(?)
        private const int OFS_UNK2 = 0x2C;

        public static int GetOffsetEntry(int species)
        {
            if (!DexLookup.TryGetValue(species, out var index))
                return -1;
            if (index < 1)
                throw new IndexOutOfRangeException();

            return (index - 1) * EntrySize;
        }

        public override bool GetSeen(int species)
        {
            var index = GetOffsetEntry(species);
            if (index < 0)
                return false;

            for (int i = 0; i < SeenRegionCount; i++)
            {
                var ofs = index + (SeenRegionSize * i);
                if (BitConverter.ToUInt64(Block.Data, ofs) != 0)
                    return true;
            }

            return false;
        }

        public bool GetSeenRegion(int species, int form, int region)
        {
            if ((uint)region >= SeenRegionCount)
                throw new ArgumentException(nameof(region));
            if ((uint)form > 63)
                return false;

            var index = GetOffsetEntry(species);
            if (index < 0)
                return false;

            var ofs = SeenRegionSize * region;
            return GetFlag(index + ofs, form);
        }

        public void SetSeenRegion(int species, int form, int region, bool value = true)
        {
            if ((uint)region >= SeenRegionCount)
                throw new ArgumentException(nameof(region));
            if ((uint)form > 63)
                return;

            var index = GetOffsetEntry(species);
            if (index < 0)
                return;

            var ofs = SeenRegionSize * region;
            SetFlag(index + ofs, form, value);
        }

        public override bool GetCaught(int species) => GetCaughtFlagID(species, 0);
        public void SetCaught(int species, bool value = true) => SetCaughtFlagID(species, 0, value);
        public bool GetCaughtGigantamaxed(int species) => GetCaughtFlagID(species, 1);
        public void SetCaughtGigantamax(int species, bool value = true) => SetCaughtFlagID(species, 1, value);
        public bool GetIsLanguageIndexObtained(int species, int langIndex) => GetCaughtFlagID(species, 2 + langIndex);
        public void SetIsLanguageIndexObtained(int species, int langIndex, bool value = true) => SetCaughtFlagID(species, 2 + langIndex, value);

        private bool GetCaughtFlagID(int species, int id)
        {
            var index = GetOffsetEntry(species);
            if (index < 0)
                return false;

            return GetFlag(index + OFS_CAUGHT, id);
        }

        private void SetCaughtFlagID(int species, int id, bool value = true)
        {
            var index = GetOffsetEntry(species);
            if (index < 0)
                return;

            SetFlag(index + OFS_CAUGHT, id, value);
        }

        public bool GetIsLanguageObtained(int species, int language)
        {
            int langIndex = GetDexLangFlag(language);
            if (langIndex < 0)
                return false;

            return GetIsLanguageIndexObtained(species, langIndex);
        }

        public void SetIsLanguageObtained(int species, int language, bool value = true)
        {
            int langIndex = GetDexLangFlag(language);
            if (langIndex < 0)
                return;

            SetIsLanguageIndexObtained(species, langIndex, value);
        }

        public uint GetAltFormDisplayed(int species)
        {
            var index = GetOffsetEntry(species);
            if (index < 0)
                return 0;

            var val = BitConverter.ToUInt32(Block.Data, index + OFS_CAUGHT);
            return (val >> 15) & 0x1FFF; // (0x1FFF is really overkill, GameFreak)
        }

        public void SetAltFormDisplayed(int species, uint value = 0)
        {
            var index = GetOffsetEntry(species);
            if (index < 0)
                return;

            var val = BitConverter.ToUInt32(Block.Data, index + OFS_CAUGHT);
            uint nv = (val & ~(0x1FFFu << 15)) | ((value & 0x1FFF) << 15);
            BitConverter.GetBytes(nv).CopyTo(Block.Data, index + OFS_CAUGHT);
        }

        public uint GetGenderDisplayed(int species)
        {
            var index = GetOffsetEntry(species);
            if (index < 0)
                return 0;

            var val = BitConverter.ToUInt32(Block.Data, index + OFS_CAUGHT);
            return (val >> 29) & 3;
        }

        public void SetGenderDisplayed(int species, uint value = 0)
        {
            var index = GetOffsetEntry(species);
            if (index < 0)
                return;

            var val = BitConverter.ToUInt32(Block.Data, index + OFS_CAUGHT);
            uint nv = (val & ~(3u << 29)) | ((value & 3) << 29);
            BitConverter.GetBytes(nv).CopyTo(Block.Data, index + OFS_CAUGHT);
        }

        public bool GetDisplayDynamaxInstead(int species) => GetCaughtFlagID(species, 28);
        public void SetDisplayDynamaxInstead(int species, bool value = true) => SetCaughtFlagID(species, 28, value);
        public bool GetDisplayShiny(int species) => GetCaughtFlagID(species, 31);
        public void SetDisplayShiny(int species, bool value = true) => SetCaughtFlagID(species, 31, value);

        public void SetCaughtFlags32(int species, uint value) => SetU32(species, value, OFS_CAUGHT);
        public uint GetBattledCount(int species) => GetU32(species, OFS_BATTLED);
        public void SetBattledCount(int species, uint value) => SetU32(species, value, OFS_BATTLED);
        public uint GetUnk1Count(int species) => GetU32(species, OFS_UNK1);
        public void SetUnk1Count(int species, uint value) => SetU32(species, value, OFS_UNK1);
        public uint GetUnk2Count(int species) => GetU32(species, OFS_UNK2);
        public void SetUnk2Count(int species, uint value) => SetU32(species, value, OFS_UNK2);

        private uint GetU32(int species, int ofs)
        {
            var index = GetOffsetEntry(species);
            if (index < 0)
                return 0;

            return BitConverter.ToUInt32(Block.Data, index + ofs);
        }

        private void SetU32(int species, uint value, int ofs)
        {
            var index = GetOffsetEntry(species);
            if (index < 0)
                return;

            BitConverter.GetBytes(value).CopyTo(Block.Data, index + ofs);
        }
        #endregion

        #region Inherited
        public override void SetDex(PKM pkm)
        {
            int species = pkm.Species;
            var index = GetOffsetEntry(species);
            if (index < 0)
                return;

            bool owned = GetCaught(species);

            var g = pkm.Gender == 1 ? 1 : 0;
            bool shiny = pkm.IsShiny;
            var s = shiny ? 2 : 0;
            int form = pkm.AltForm;
            if (species == (int)Species.Alcremie)
            {
                form *= 7;
                form += (int)((PK8)pkm).FormArgument; // alteration byte
            }
            else if (species == (int) Species.Eternatus && pkm.AltForm == 1)
            {
                form = 0;
                SetSeenRegion(species, 63, g | s);
            }

            SetSeenRegion(species, form, g | s);
            SetCaught(species);
            SetIsLanguageObtained(species, pkm.Language);
            if (!owned)
            {
                SetAltFormDisplayed(species, (byte)form);
                if (shiny)
                    SetDisplayShiny(species);
            }

            var count = GetBattledCount(species);
            if (count == 0)
                SetBattledCount(species, 1);
        }

        public override void SeenNone()
        {
            Array.Clear(Block.Data, 0, DexLookup.Count * EntrySize);
        }

        public override void CaughtNone()
        {
            foreach (var kvp in DexLookup)
                CaughtNone(kvp.Key);
        }

        private void CaughtNone(int species)
        {
            SetCaughtFlags32(species, 0);
            SetUnk1Count(species, 0);
            SetUnk2Count(species, 0);
        }

        public override void SeenAll(bool shinyToo = false)
        {
            SetAllSeen(true, shinyToo);
        }

        private void SeenAll(int species, int fc, bool shinyToo, bool value = true)
        {
            var pt = PersonalTable.SWSH;
            for (int form = 0; form < fc; form++)
            {
                var pi = pt.GetFormeEntry(species, form);
                SeenAll(species, form, value, pi, shinyToo);
            }

            if (!value)
                ClearGigantamaxFlags(species);
        }

        private void SeenAll(int species, int bitIndex, bool value, PersonalInfo pi, bool shinyToo)
        {
            if (pi.IsDualGender || !value)
            {
                SetSeenRegion(species, bitIndex, 0, value);
                SetSeenRegion(species, bitIndex, 1, value);
                if (!shinyToo && value)
                    return;
                SetSeenRegion(species, bitIndex, 2, value);
                SetSeenRegion(species, bitIndex, 3, value);
            }
            else
            {
                var index = pi.OnlyFemale ? 1 : 0;
                SetSeenRegion(species, bitIndex, 0 + index);
                if (!shinyToo)
                    return;
                SetSeenRegion(species, bitIndex, 2 + index);
            }
        }

        private void ClearGigantamaxFlags(int species)
        {
            SetSeenRegion(species, 63, 0, false);
            SetSeenRegion(species, 63, 1, false);
            SetSeenRegion(species, 63, 2, false);
            SetSeenRegion(species, 63, 3, false);
        }

        public override void CompleteDex(bool shinyToo = false)
        {
            foreach (var kvp in DexLookup)
                SetDexEntryAll(kvp.Key, shinyToo);
        }

        public override void CaughtAll(bool shinyToo = false)
        {
            SeenAll(shinyToo);
            foreach (var kvp in DexLookup)
            {
                var species = kvp.Key;
                SetAllCaught(species, true, shinyToo);
            }
        }

        private void SetAllCaught(int species, bool value = true, bool shinyToo = false)
        {
            SetCaught(species);
            for (int i = 0; i < 11; i++)
                SetIsLanguageObtained(species, i, value);

            if (value)
            {
                var pi = PersonalTable.SWSH[species];
                if (shinyToo)
                    SetDisplayShiny(species);

                SetGenderDisplayed(species, (uint)pi.RandomGender());
            }
            else
            {
                SetDisplayShiny(species, false);
                SetDisplayDynamaxInstead(species, false);
                SetGenderDisplayed(species, 0);
            }
        }

        public override void SetAllSeen(bool value = true, bool shinyToo = false)
        {
            foreach (var kvp in DexLookup)
            {
                var species = kvp.Key;
                SetAllSeen(species, value, shinyToo);
            }
        }

        private void SetAllSeen(int species, bool value = true, bool shinyToo = false)
        {
            var pi = PersonalTable.SWSH[species];
            var fc = pi.FormeCount;
            if (species == (int) Species.Eternatus)
                fc = 1; // ignore gigantamax
            SeenAll(species, fc, shinyToo, value);

            if (species == (int) Species.Alcremie)
            {
                // Alcremie forms
                const int deco = 7;
                const int forms = 9;
                for (int i = 0; i < deco * forms; i++) // 0-62
                    SeenAll(species, i, value, pi, shinyToo);
            }

            if (SpeciesWithGigantamaxData.Contains(species))
            {
                SeenAll(species, 63, value, pi, shinyToo);
                SetCaughtGigantamax(species);
            }
        }

        public override void SetDexEntryAll(int species, bool shinyToo = false)
        {
            SetAllSeen(species, true, shinyToo);
            SetAllCaught(species, true);
        }

        public override void ClearDexEntryAll(int species)
        {
            var ofs = GetOffsetEntry(species);
            if (ofs < 0)
                return;
            Array.Clear(Block.Data, ofs, EntrySize);
        }

        public void SetAllBattledCount(uint count = 500)
        {
            foreach (var kvp in DexLookup)
            {
                var species = kvp.Key;
                SetBattledCount(species, count);
            }
        }

        private static readonly HashSet<int> SpeciesWithGigantamaxData = new HashSet<int>
        {
            (int)Species.Charizard,
            (int)Species.Butterfree,
            (int)Species.Pikachu,
            (int)Species.Meowth,
            (int)Species.Machamp,
            (int)Species.Gengar,
            (int)Species.Kingler,
            (int)Species.Lapras,
            (int)Species.Eevee,
            (int)Species.Snorlax,
            (int)Species.Garbodor,
            (int)Species.Corviknight,
            (int)Species.Orbeetle,
            (int)Species.Drednaw,
            (int)Species.Coalossal,
            (int)Species.Flapple,
            (int)Species.Appletun,
            (int)Species.Sandaconda,
            (int)Species.Toxtricity,
            (int)Species.Centiskorch,
            (int)Species.Hatterene,
            (int)Species.Grimmsnarl,
            (int)Species.Alcremie,
            (int)Species.Copperajah,
            (int)Species.Duraludon,
            (int)Species.Eternatus,
        };
        #endregion
    }
}