using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;
using static PKHeX.Core.Encounters1;
using static PKHeX.Core.Encounters2;
using static PKHeX.Core.Encounters3;
using static PKHeX.Core.Encounters4;
using static PKHeX.Core.Encounters5;
using static PKHeX.Core.Encounters6;
using static PKHeX.Core.Encounters7;
using static PKHeX.Core.Encounters7b;

namespace PKHeX.Core
{
    public static class EncounterSlotGenerator
    {
        public static IEnumerable<EncounterSlot> GetPossible(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            int maxspeciesorigin = GetMaxSpecies(gameSource);
            var vs = EvolutionChain.GetValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin);
            return GetPossible(pkm, vs, gameSource);
        }

        public static IEnumerable<EncounterSlot> GetPossible(PKM pkm, IReadOnlyList<DexLevel> vs, GameVersion gameSource = GameVersion.Any)
        {
            var possibleAreas = GetEncounterSlots(pkm, gameSource);
            return possibleAreas.SelectMany(area => area.Slots).Where(z => vs.Any(v => v.Species == z.Species));
        }

        private static IEnumerable<EncounterSlot> GetRawEncounterSlots(PKM pkm, int lvl, GameVersion gameSource)
        {
            int maxspeciesorigin = GetMaxSpecies(gameSource);
            var vs = EvolutionChain.GetValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin);
            return GetRawEncounterSlots(pkm, lvl, vs, gameSource);
        }

        private static IEnumerable<EncounterSlot> GetRawEncounterSlots(PKM pkm, int lvl, IReadOnlyList<EvoCriteria> vs, GameVersion gameSource)
        {
            var possibleAreas = GetEncounterAreas(pkm, gameSource);
            return possibleAreas.SelectMany(area => GetValidEncounterSlots(pkm, area, vs, DexNav: pkm.AO, lvl: lvl));
        }

        private static int GetMaxSpecies(GameVersion gameSource)
        {
            if (gameSource == GameVersion.RBY)
                return MaxSpeciesID_1;
            if (GameVersion.GSC.Contains(gameSource))
                return MaxSpeciesID_2;
            return -1;
        }

        public static IEnumerable<EncounterSlot> GetValidWildEncounters34(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            int lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();
            var s = GetRawEncounterSlots(pkm, lvl, gameSource);

            return s; // defer deferrals to the method consuming this collection
        }

        public static IEnumerable<EncounterSlot> GetValidWildEncounters12(PKM pkm, IReadOnlyList<EvoCriteria> vs, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            int lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();
            return GetRawEncounterSlots(pkm, lvl, vs, gameSource);
        }

        public static IEnumerable<EncounterSlot> GetValidWildEncounters(PKM pkm, IReadOnlyList<EvoCriteria> vs, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            int lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();
            var s = GetRawEncounterSlots(pkm, lvl, vs, gameSource);

            bool IsSafariBall = pkm.Ball == (int)Ball.Safari;
            bool IsSportBall = pkm.Ball == (int)Ball.Sport;
            bool IsHidden = pkm.AbilityNumber == 4; // hidden Ability
            int species = pkm.Species;

            return s.DeferByBoolean(slot => slot.IsDeferred(species, pkm, IsSafariBall, IsSportBall, IsHidden)); // non-deferred first
        }

        public static IEnumerable<EncounterSlot> GetValidWildEncounters(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            int lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();
            var s = GetRawEncounterSlots(pkm, lvl, gameSource);

            bool IsSafariBall = pkm.Ball == (int)Ball.Safari;
            bool IsSportBall = pkm.Ball == (int)Ball.Sport;
            bool IsHidden = pkm.AbilityNumber == 4; // hidden Ability
            int species = pkm.Species;

            return s.DeferByBoolean(slot => slot.IsDeferred(species, pkm, IsSafariBall, IsSportBall, IsHidden)); // non-deferred first
        }

        public static bool IsDeferred3(this EncounterSlot slot, int currentSpecies, PKM pkm, bool IsSafariBall)
        {
            return slot.IsDeferredWurmple(currentSpecies, pkm)
                || slot.IsDeferredSafari(IsSafariBall);
        }

        public static bool IsDeferred4(this EncounterSlot slot, int currentSpecies, PKM pkm, bool IsSafariBall, bool IsSportBall)
        {
            return slot.IsDeferredWurmple(currentSpecies, pkm)
                || slot.IsDeferredSafari(IsSafariBall)
                || slot.IsDeferredSport(IsSportBall);
        }

        private static bool IsDeferred(this EncounterSlot slot, int currentSpecies, PKM pkm, bool IsSafariBall, bool IsSportBall, bool IsHidden)
        {
            return slot.IsDeferredWurmple(currentSpecies, pkm)
                || slot.IsDeferredHiddenAbility(IsHidden)
                || slot.IsDeferredSafari(IsSafariBall)
                || slot.IsDeferredSport(IsSportBall);
        }

        private static bool IsDeferredWurmple(this IEncounterable slot, int currentSpecies, PKM pkm) => slot.Species == 265 && currentSpecies != 265 && !WurmpleUtil.IsWurmpleEvoValid(pkm);
        private static bool IsDeferredSafari(this EncounterSlot slot, bool IsSafariBall) => IsSafariBall != ((slot.Type & SlotType.Safari) != 0);
        private static bool IsDeferredSport(this EncounterSlot slot, bool IsSportBall) => IsSportBall != ((slot.Type & SlotType.BugContest) != 0);
        private static bool IsDeferredHiddenAbility(this EncounterSlot slot, bool IsHidden) => IsHidden != slot.IsHiddenAbilitySlot();

        public static IEnumerable<EncounterSlot> GetValidFriendSafari(PKM pkm)
        {
            if (!pkm.XY || pkm.Met_Location != 148 || pkm.Met_Level != 30 || pkm.Egg_Location != 0) // Friend Safari
                return Enumerable.Empty<EncounterSlot>();
            var vs = EvolutionChain.GetValidPreEvolutions(pkm).Where(d => d.Level >= 30);
            return vs.SelectMany(z => Encounters6.FriendSafari[z.Species]);
        }

        private static IEnumerable<EncounterSlot> GetValidEncounterSlots(PKM pkm, EncounterArea loc, IEnumerable<DexLevel> vs, bool DexNav = false, int lvl = -1, bool ignoreLevel = false)
        {
            if (pkm.Egg_Location != 0)
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
            const int comboLureBonus = 1; // +1 if combo/lure?
            int df = DexNav ? fluteBoost : IsCatchCombo(pkm) ? comboLureBonus : 0;
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

        private static bool IsCatchCombo(PKM pkm)
        {
            var ver = pkm.Version;
            return ver == (int) GameVersion.GP || ver == (int) GameVersion.GE; // ignore GO Transfers
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
                return slots.Where(slot => vs.Any(evo => evo.Species == slot.Species));

            slots = slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin - df));
            // Get slots where pokemon can exist with respect to level constraints
            if (pkm.HasOriginalMetLocation)
                return slots.Where(slot => slot.LevelMin - df <= lvl && lvl <= slot.LevelMax + (slot.Permissions.AllowDexNav ? dn : df));
            // check for any less than current level
            return slots.Where(slot => slot.LevelMin <= lvl);
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
                case 744 when form == 1: // Rockruff Event
                case 745 when form == 2: // Lycanroc Event
                    yield break;
            }

            EncounterSlot slotMax = null;
            void CachePressureSlot(EncounterSlot s)
            {
                if (slotMax != null && s.LevelMax > slotMax.LevelMax)
                    slotMax = s;
            }

            if (AlolanVariantEvolutions12.Contains(species)) // match form if same species, else form 0.
            {
                foreach (var slot in encounterSlots)
                {
                    if (species == slot.Species ? slot.Form == form : slot.Form == 0)
                        yield return slot;
                    CachePressureSlot(slot);
                }
            }
            else if (ShouldMatchSlotForm()) // match slot form
            {
                foreach (var slot in encounterSlots)
                {
                    if (slot.Form == form)
                        yield return slot;
                    CachePressureSlot(slot);
                }
            }
            else
            {
                foreach (var slot in encounterSlots)
                {
                    yield return slot; // no form checking
                    CachePressureSlot(slot);
                }
            }

            // Filter for Form Specific
            // Pressure Slot
            if (slotMax == null)
                yield break;

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
            {
                yield return GetPressureSlot(slotMax, pkm);
            }

            bool ShouldMatchSlotForm() => WildForms.Contains(species) || AlolanOriginForms.Contains(species) || FormConverter.IsTotemForm(species, form);
        }

        private static IEnumerable<EncounterSlot> GetFilteredSlots6DexNav(PKM pkm, int lvl, IEnumerable<EncounterSlot> encounterSlots, int fluteBoost)
        {
            EncounterSlot slotMax = null;
            foreach (EncounterSlot s in encounterSlots)
            {
                if (WildForms.Contains(pkm.Species) && s.Form != pkm.AltForm)
                {
                    CachePressureSlot(s);
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

                CachePressureSlot(slot);
            }

            void CachePressureSlot(EncounterSlot s)
            {
                if (slotMax != null && s.LevelMax > slotMax.LevelMax)
                    slotMax = s;
            }
            // Pressure Slot
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
                    if (pkm is PK2 pk2 && pk2.Met_TimeOfDay != 0)
                        return slots.Where(slot => ((EncounterSlot1)slot).Time.Contains(pk2.Met_TimeOfDay));
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
            bool noMet = !pkm.HasOriginalMetLocation || (pkm.Format == 2 && gameSource != GameVersion.C);
            return noMet ? slots : slots.Where(area => area.Location == pkm.Met_Location);
        }

        private static bool IsHiddenAbilitySlot(this EncounterSlot slot)
        {
            return slot.Permissions.DexNav || slot.Type == SlotType.FriendSafari || slot.Type == SlotType.Horde || slot.Type == SlotType.SOS;
        }

        internal static bool IsDexNavValid(PKM pkm)
        {
            if (!pkm.AO || !pkm.InhabitedGeneration(6))
                return false;

            var vs = EvolutionChain.GetValidPreEvolutions(pkm);
            var table = pkm.Version == (int) GameVersion.AS ? SlotsA : SlotsO;
            int loc = pkm.Met_Location;
            var areas = table.Where(l => l.Location == loc);
            var d_areas = areas.Select(area => GetValidEncounterSlots(pkm, area, vs, DexNav: true));
            return d_areas.Any(slots => slots.Any(slot => slot.Permissions.AllowDexNav && slot.Permissions.DexNav));
        }

        internal static EncounterArea GetCaptureLocation(PKM pkm)
        {
            var vs = EvolutionChain.GetValidPreEvolutions(pkm);
            return (from area in GetEncounterSlots(pkm)
                let slots = GetValidEncounterSlots(pkm, area, vs, DexNav: pkm.AO, ignoreLevel: true).ToArray()
                where slots.Length != 0
                select new EncounterArea
                {
                    Location = area.Location,
                    Slots = slots,
                }).OrderBy(area => area.Slots.Min(x => x.LevelMin)).FirstOrDefault();
        }

        private static IEnumerable<EncounterArea> GetEncounterTable(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            switch (gameSource)
            {
                case GameVersion.RBY:
                case GameVersion.RD:
                case GameVersion.BU:
                case GameVersion.GN:
                case GameVersion.YW:
                    return SlotsRBY;

                case GameVersion.GSC:
                case GameVersion.GD:
                case GameVersion.SV:
                case GameVersion.C:
                    return GetEncounterTableGSC(pkm);

                case GameVersion.R: return SlotsR;
                case GameVersion.S: return SlotsS;
                case GameVersion.E: return SlotsE;
                case GameVersion.FR: return SlotsFR;
                case GameVersion.LG: return SlotsLG;
                case GameVersion.CXD: return SlotsXD;

                case GameVersion.D: return SlotsD;
                case GameVersion.P: return SlotsP;
                case GameVersion.Pt: return SlotsPt;
                case GameVersion.HG: return SlotsHG;
                case GameVersion.SS: return SlotsSS;

                case GameVersion.B: return SlotsB;
                case GameVersion.W: return SlotsW;
                case GameVersion.B2: return SlotsB2;
                case GameVersion.W2: return SlotsW2;

                case GameVersion.X: return SlotsX;
                case GameVersion.Y: return SlotsY;
                case GameVersion.AS: return SlotsA;
                case GameVersion.OR: return SlotsO;

                case GameVersion.SN: return SlotsSN;
                case GameVersion.MN: return SlotsMN;
                case GameVersion.US: return SlotsUS;
                case GameVersion.UM: return SlotsUM;

                case GameVersion.GP: return SlotsGP;
                case GameVersion.GE: return SlotsGE;
                case GameVersion.GO: return SlotsGO_GG;

                default: return Enumerable.Empty<EncounterArea>();
            }
        }

        private static IEnumerable<EncounterArea> GetEncounterTableGSC(PKM pkm)
        {
            if (!ParseSettings.AllowGen2Crystal(pkm))
                return SlotsGS;

            // Gen 2 met location is lost outside gen 2 games
            if (pkm.Format != 2)
                return SlotsGSC;

            // Format 2 with met location, encounter should be from Crystal
            if (pkm.HasOriginalMetLocation)
                return SlotsC;

            // Format 2 without met location but pokemon could not be tradeback to gen 1,
            // encounter should be from gold or silver
            if (pkm.Species > 151 && !FutureEvolutionsGen1.Contains(pkm.Species))
                return SlotsGS;

            // Encounter could be any gen 2 game, it can have empty met location for have a g/s origin
            // or it can be a Crystal pokemon that lost met location after being tradeback to gen 1 games
            return SlotsGSC;
        }
    }
}
