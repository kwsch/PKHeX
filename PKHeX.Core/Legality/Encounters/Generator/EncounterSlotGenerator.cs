using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EncounterSlotGenerator
    {

        // EncounterSlot
        private static IEnumerable<EncounterSlot> GetRawEncounterSlots(PKM pkm, int lvl, GameVersion gameSource = GameVersion.Any)
        {
            int maxspeciesorigin = -1;
            if (gameSource == GameVersion.RBY) maxspeciesorigin = MaxSpeciesID_1;
            else if (GameVersion.GSC.Contains(gameSource)) maxspeciesorigin = MaxSpeciesID_2;

            var vs = GetValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin);
            return GetEncounterAreas(pkm, gameSource).SelectMany(area => GetValidEncounterSlots(pkm, area, vs, DexNav: pkm.AO, lvl: lvl));
        }

        public static IEnumerable<EncounterSlot> GetValidWildEncounters(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            int lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();
            var s = GetRawEncounterSlots(pkm, lvl, gameSource);
            bool IsSafariBall = pkm.Ball == 5;
            bool IsSportsBall = pkm.Ball == 0x18;
            bool IsHidden = pkm.AbilityNumber == 4; // hidden Ability
            int species = pkm.Species;

            bool IsDeferred(EncounterSlot slot)
            {
                if (slot.Species == 265 && species != 265 && !IsWurmpleEvoValid(pkm))
                    return true; // bad wurmple evolution
                if (IsHidden ^ IsHiddenAbilitySlot(slot))
                    return true; // ability mismatch
                if (IsSafariBall ^ IsSafariSlot(slot.Type))
                    return true; // Safari Zone only ball
                if (IsSportsBall ^ slot.Type == SlotType.BugContest)
                    return true;
                return false; // BCC only ball
            }
            return s.OrderBy(IsDeferred); // non-deferred first
        }

        public static IEnumerable<EncounterSlot> GetValidFriendSafari(PKM pkm)
        {
            if (!pkm.XY || pkm.Met_Location != 148 || pkm.Met_Level != 30) // Friend Safari
                return Enumerable.Empty<EncounterSlot>();
            var vs = GetValidPreEvolutions(pkm).Where(d => d.Level >= 30);
            return vs.SelectMany(z => Encounters6.FriendSafari[z.Species]);
        }

        public static IEnumerable<EncounterSlot> GetValidEncounterSlots(PKM pkm, EncounterArea loc, IEnumerable<DexLevel> vs, bool DexNav = false, int lvl = -1, bool ignoreLevel = false)
        {
            if (pkm.WasEgg)
                return Enumerable.Empty<EncounterSlot>();
            if (lvl < 0)
                lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();

            int gen = pkm.GenNumber;
            if (gen < 3)
                return GetValidEncounterSlots12(pkm, loc, vs, lvl, ignoreLevel);

            const int fluteBoost = 4;
            const int dexnavBoost = 30;
            int df = DexNav ? fluteBoost : 0;
            int dn = DexNav ? fluteBoost + dexnavBoost : 0;

            // Get Valid levels
            var encounterSlots = GetValidEncounterSlotsByEvoLevel(pkm, loc.Slots, lvl, ignoreLevel, vs, df, dn);

            // Return enumerable of slots pkm might have originated from
            if (gen <= 5)
                return GetFilteredSlotsByForm(pkm, encounterSlots);
            if (DexNav && gen == 6)
                return GetFilteredSlots6DexNav(pkm, lvl, encounterSlots, fluteBoost);
            return GetFilteredSlots67(pkm, encounterSlots);
        }
        private static IEnumerable<EncounterSlot> GetValidEncounterSlots12(PKM pkm, EncounterArea loc, IEnumerable<DexLevel> vs, int lvl = -1, bool ignoreLevel = false)
        {
            if (lvl < 0)
                lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();

            var Gen1Version = GameVersion.RBY;
            bool RBDragonair = false;
            if (!ignoreLevel && !FilterGBSlotsCatchRate(pkm, ref vs, ref Gen1Version, ref RBDragonair))
                return Enumerable.Empty<EncounterSlot>();

            var encounterSlots = GetValidEncounterSlotsByEvoLevel(pkm, loc.Slots, lvl, ignoreLevel, vs);
            return GetFilteredSlots12(pkm, pkm.GenNumber, Gen1Version, encounterSlots, RBDragonair).OrderBy(slot => slot.LevelMin); // prefer lowest levels
        }
        private static IEnumerable<EncounterSlot> GetValidEncounterSlotsByEvoLevel(PKM pkm, IEnumerable<EncounterSlot> slots, int lvl, bool ignoreLevel, IEnumerable<DexLevel> vs, int df = 0, int dn = 0)
        {
            // Get slots where pokemon can exist with respect to the evolution chain
            if (ignoreLevel)
                return slots.Where(slot => vs.Any(evo => evo.Species == slot.Species)).ToList();

            slots = slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin - df));
            // Get slots where pokemon can exist with respect to level constraints
            if (pkm.HasOriginalMetLocation)
                return slots.Where(slot => slot.LevelMin - df <= lvl && lvl <= slot.LevelMax + (slot.Permissions.AllowDexNav ? dn : df)).ToList();
            // check for any less than current level
            return slots.Where(slot => slot.LevelMin <= lvl).ToList();
        }
        private static IEnumerable<EncounterSlot> GetFilteredSlotsByForm(PKM pkm, IEnumerable<EncounterSlot> encounterSlots)
        {
            return WildForms.Contains(pkm.Species)
                ? encounterSlots.Where(slot => slot.Form == pkm.AltForm)
                : encounterSlots;
        }
        private static IEnumerable<EncounterSlot> GetFilteredSlots67(PKM pkm, IEnumerable<EncounterSlot> encounterSlots)
        {
            int species = pkm.Species;
            int form = pkm.AltForm;

            // Edge Case Handling
            switch (species)
            {
                case 744 when form == 1:
                case 745 when form == 2:
                    yield break;
            }

            var slots = new List<EncounterSlot>();
            if (AlolanVariantEvolutions12.Contains(species)) // match form if same species, else form 0.
            {
                foreach (var slot in encounterSlots)
                {
                    if (species == slot.Species ? slot.Form == form : slot.Form == 0)
                        yield return slot;
                    slots.Add(slot);
                }
            }
            else if (ShouldMatchSlotForm()) // match slot form
            {
                foreach (var slot in encounterSlots)
                {
                    if (slot.Form == form)
                        yield return slot;
                    slots.Add(slot);
                }
            }
            else
            {
                foreach (var slot in encounterSlots)
                {
                    yield return slot; // no form checking
                    slots.Add(slot);
                }
            }

            // Filter for Form Specific
            // Pressure Slot
            EncounterSlot slotMax = slots.OrderByDescending(slot => slot.LevelMax).FirstOrDefault();
            if (slotMax == null)
                yield break; // yield break;

            if (AlolanVariantEvolutions12.Contains(species)) // match form if same species, else form 0.
            {
                if (species == slotMax.Species ? slotMax.Form == form : slotMax.Form == 0)
                    yield return GetPressureSlot(slotMax, pkm);
            }
            else if (ShouldMatchSlotForm()) // match slot form
            {
                if (slotMax.Form == form)
                    yield return GetPressureSlot(slotMax, pkm);
            }
            else
                yield return GetPressureSlot(slotMax, pkm);

            bool ShouldMatchSlotForm() => WildForms.Contains(species) || AlolanOriginForms.Contains(species) || FormConverter.IsTotemForm(species, form);
        }
        private static IEnumerable<EncounterSlot> GetFilteredSlots6DexNav(PKM pkm, int lvl, IEnumerable<EncounterSlot> encounterSlots, int fluteBoost)
        {
            var slots = new List<EncounterSlot>();
            foreach (EncounterSlot s in encounterSlots)
            {
                if (WildForms.Contains(pkm.Species) && s.Form != pkm.AltForm)
                {
                    slots.Add(s);
                    continue;
                }
                bool nav = s.Permissions.AllowDexNav && (pkm.RelearnMove1 != 0 || pkm.AbilityNumber == 4);
                EncounterSlot slot = s.Clone();
                slot.Permissions.DexNav = nav;

                if (slot.LevelMin > lvl)
                    slot.Permissions.WhiteFlute = true;
                if (slot.LevelMax + 1 <= lvl && lvl <= slot.LevelMax + fluteBoost)
                    slot.Permissions.BlackFlute = true;
                if (slot.LevelMax != lvl && slot.Permissions.AllowDexNav)
                    slot.Permissions.DexNav = true;
                yield return slot;
                slots.Add(s);
            }
            // Pressure Slot
            EncounterSlot slotMax = slots.OrderByDescending(slot => slot.LevelMax).FirstOrDefault();
            if (slotMax != null)
                yield return GetPressureSlot(slotMax, pkm);
        }
        private static EncounterSlot GetPressureSlot(EncounterSlot s, PKM pkm)
        {
            var max = s.Clone();
            max.Permissions.Pressure = true;
            max.Form = pkm.AltForm;
            return max;
        }

        private static bool FilterGBSlotsCatchRate(PKM pkm, ref IEnumerable<DexLevel> vs, ref GameVersion Gen1Version, ref bool RBDragonair)
        {
            if (!(pkm is PK1 pk1) || !pkm.Gen1_NotTradeback)
                return true;

            // Pure gen 1, slots can be filter by catch rate
            var rate = pk1.Catch_Rate;
            switch (pkm.Species)
            {
                // Pikachu
                case 25 when rate == 163:
                case 26 when rate == 163:
                    return false; // Yellow Pikachu is not a wild encounter

                // Kadabra (YW)
                case 64 when rate == 96:
                case 65 when rate == 96:
                    vs = vs.Where(s => s.Species == 64);
                    Gen1Version = GameVersion.YW;
                    return true;

                // Kadabra (RB)
                case 64 when rate == 100:
                case 65 when rate == 100:
                    vs = vs.Where(s => s.Species == 64);
                    Gen1Version = GameVersion.RB;
                    return true;

                // Dragonair (YW)
                case 148 when rate == 27:
                case 149 when rate == 27:
                    vs = vs.Where(s => s.Species == 148); // Yellow Dragonair, ignore Dratini encounters
                    Gen1Version = GameVersion.YW;
                    return true;

                // Dragonair (RB)
                case 148:
                case 149:
                    // Red blue dragonair have the same catch rate as dratini, it could also be a dratini from any game
                    vs = vs.Where(s => rate == PersonalTable.RB[s.Species].CatchRate);
                    RBDragonair = true;
                    return true;

                default:
                    vs = vs.Where(s => rate == PersonalTable.RB[s.Species].CatchRate);
                    return true;
            }
        }
        private static IEnumerable<EncounterSlot> GetFilteredSlots12(PKM pkm, int gen, GameVersion Gen1Version, IEnumerable<EncounterSlot> slots, bool RBDragonair)
        {
            switch (gen)
            {
                case 1:
                    if (Gen1Version != GameVersion.RBY)
                        slots = slots.Where(slot => Gen1Version.Contains(((EncounterSlot1)slot).Version));

                    // Red Blue dragonair or dratini from any gen 1 games
                    if (RBDragonair)
                        return slots.Where(slot => GameVersion.RB.Contains(((EncounterSlot1)slot).Version) || slot.Species == 147);

                    return slots;

                case 2:
                    if (pkm is PK2 pk2 && pk2.Met_Day != 0)
                        return slots.Where(slot => ((EncounterSlot1)slot).Time.Contains(pk2.Met_Day));
                    return slots;

                default:
                    return slots;
            }
        }

        public static IEnumerable<EncounterArea> GetEncounterSlots(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            return GetEncounterTable(pkm, gameSource);
        }
        private static IEnumerable<EncounterArea> GetEncounterAreas(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var slots = GetEncounterSlots(pkm, gameSource: gameSource);
            bool noMet = !pkm.HasOriginalMetLocation || pkm.Format == 2 && gameSource != GameVersion.C;
            return noMet ? slots : slots.Where(area => area.Location == pkm.Met_Location);
        }

        private static bool IsWurmpleEvoValid(PKM pkm)
        {
            uint evoVal = PKX.GetWurmpleEvoVal(pkm.EncryptionConstant);
            int wIndex = Array.IndexOf(WurmpleEvolutions, pkm.Species) / 2;
            return evoVal == wIndex;
        }

        private static bool IsHiddenAbilitySlot(EncounterSlot slot)
        {
            return slot.Permissions.DexNav || slot.Type == SlotType.FriendSafari || slot.Type == SlotType.Horde || slot.Type == SlotType.SOS;
        }

        internal static bool IsSafariSlot(SlotType t)
        {
            return t.HasFlag(SlotType.Safari);
        }

        internal static bool IsDexNavValid(PKM pkm)
        {
            if (!pkm.AO || !pkm.InhabitedGeneration(6))
                return false;

            var vs = GetValidPreEvolutions(pkm);
            IEnumerable<EncounterArea> locs = GetDexNavAreas(pkm);
            var d_areas = locs.Select(loc => GetValidEncounterSlots(pkm, loc, vs, DexNav: true));
            return d_areas.Any(slots => slots.Any(slot => slot.Permissions.AllowDexNav && slot.Permissions.DexNav));
        }
        internal static EncounterArea GetCaptureLocation(PKM pkm)
        {
            var vs = GetValidPreEvolutions(pkm);
            return (from area in GetEncounterSlots(pkm)
                let slots = GetValidEncounterSlots(pkm, area, vs, DexNav: pkm.AO, ignoreLevel: true).ToArray()
                where slots.Length != 0
                select new EncounterArea
                {
                    Location = area.Location,
                    Slots = slots,
                }).OrderBy(area => area.Slots.Min(x => x.LevelMin)).FirstOrDefault();
        }
    }
}
