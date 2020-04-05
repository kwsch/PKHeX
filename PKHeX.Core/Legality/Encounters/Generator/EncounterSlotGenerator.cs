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
using static PKHeX.Core.Encounters8;

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

        private static IEnumerable<EncounterSlot> GetRawEncounterSlots(PKM pkm, int lvl, IReadOnlyList<EvoCriteria> vs, GameVersion gameSource)
        {
            if (pkm.Egg_Location != 0)
                yield break;

            var possibleAreas = GetEncounterAreas(pkm, gameSource);
            foreach (var area in possibleAreas)
            {
                var slots = area.GetMatchingSlots(pkm, vs, lvl);
                foreach (var s in slots)
                    yield return s;
            }
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
            int lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();

            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            int maxspeciesorigin = GetMaxSpecies(gameSource);
            var vs = EvolutionChain.GetValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin);
            var s = GetRawEncounterSlots(pkm, lvl, vs, gameSource);

            return s; // defer deferrals to the method consuming this collection
        }

        public static IEnumerable<EncounterSlot> GetValidWildEncounters12(PKM pkm, IReadOnlyList<EvoCriteria> vs, GameVersion gameSource = GameVersion.Any)
        {
            int lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();

            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

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
            int maxspeciesorigin = GetMaxSpecies(gameSource);
            var vs = EvolutionChain.GetValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin);
            return GetValidWildEncounters(pkm, vs, gameSource);
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

        private static bool IsDeferredWurmple(this IEncounterable slot, int currentSpecies, PKM pkm) => slot.Species == (int)Species.Wurmple && currentSpecies != (int)Species.Wurmple && !WurmpleUtil.IsWurmpleEvoValid(pkm);
        private static bool IsDeferredSafari(this EncounterSlot slot, bool IsSafariBall) => IsSafariBall != ((slot.Type & SlotType.Safari) != 0);
        private static bool IsDeferredSport(this EncounterSlot slot, bool IsSportBall) => IsSportBall != ((slot.Type & SlotType.BugContest) != 0);
        private static bool IsDeferredHiddenAbility(this EncounterSlot slot, bool IsHidden) => IsHidden != slot.IsHiddenAbilitySlot();

        private static IEnumerable<EncounterSlot> GetValidEncounterSlots(PKM pkm, EncounterArea loc, IReadOnlyList<EvoCriteria> vs, int lvl)
        {
            if (pkm.Egg_Location != 0)
                return Enumerable.Empty<EncounterSlot>();
            return loc.GetMatchingSlots(pkm, vs, lvl);
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
            if (noMet)
                return slots;
            var metLocation = pkm.Met_Location;
            return slots.Where(z => z.IsMatchLocation(metLocation));
        }

        private static bool IsHiddenAbilitySlot(this EncounterSlot slot)
        {
            return slot.Permissions.DexNav || slot.Type == SlotType.FriendSafari || slot.Type == SlotType.Horde || slot.Type == SlotType.SOS;
        }

        internal static EncounterArea? GetCaptureLocation(PKM pkm)
        {
            var vs = EvolutionChain.GetValidPreEvolutions(pkm);
            return (from area in GetEncounterSlots(pkm)
                let slots = GetValidEncounterSlots(pkm, area, vs, lvl: 0).ToArray()
                where slots.Length != 0
                select new EncounterAreaFake
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

                case GameVersion.SW: return SlotsSW;
                case GameVersion.SH: return SlotsSH;

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
