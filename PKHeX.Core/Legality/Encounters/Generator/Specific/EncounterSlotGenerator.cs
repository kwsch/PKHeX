using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;
using static PKHeX.Core.Encounters1;
using static PKHeX.Core.Encounters2;
using static PKHeX.Core.Encounters3;
using static PKHeX.Core.Encounters3GC;
using static PKHeX.Core.Encounters4;
using static PKHeX.Core.Encounters5;
using static PKHeX.Core.Encounters6;
using static PKHeX.Core.Encounters7;
using static PKHeX.Core.Encounters7b;
using static PKHeX.Core.Encounters8;
using static PKHeX.Core.EncountersGO;

using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    public static class EncounterSlotGenerator
    {
        public static IEnumerable<EncounterSlot> GetPossible(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource = GameVersion.Any)
        {
            var possibleAreas = GetEncounterSlots(pkm, gameSource);
            return possibleAreas.SelectMany(area => area.Slots).Where(z => chain.Any(v => v.Species == z.Species));
        }

        private static IEnumerable<EncounterSlot> GetRawEncounterSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain, GameVersion gameSource)
        {
            if (pkm.Egg_Location != 0 || pkm.IsEgg)
                yield break;

            var possibleAreas = GetEncounterAreas(pkm, gameSource);
            foreach (var area in possibleAreas)
            {
                var slots = area.GetMatchingSlots(pkm, chain);
                foreach (var s in slots)
                    yield return s;
            }
        }

        public static IEnumerable<EncounterSlot> GetValidWildEncounters34(PKM pkm, IReadOnlyList<EvoCriteria> chain, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var slots = GetRawEncounterSlots(pkm, chain, gameSource);

            return slots; // defer deferrals to the method consuming this collection
        }

        public static IEnumerable<EncounterSlot> GetValidWildEncounters12(PKM pkm, IReadOnlyList<EvoCriteria> chain, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            return GetRawEncounterSlots(pkm, chain, gameSource);
        }

        public static IEnumerable<EncounterSlot> GetValidWildEncounters(PKM pkm, IReadOnlyList<EvoCriteria> chain, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            return GetRawEncounterSlots(pkm, chain, gameSource);
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

        internal static EncounterSlot? GetCaptureLocation(PKM pkm)
        {
            var chain = EvolutionChain.GetValidPreEvolutions(pkm, maxLevel: 100, skipChecks: true);
            return GetPossible(pkm, chain)
                .OrderBy(z => !chain.Any(s => s.Species == z.Species && s.Form == z.Form))
                .ThenBy(z => z.LevelMin)
                .FirstOrDefault();
        }

        private static IEnumerable<EncounterArea> GetEncounterTable(PKM pkm, GameVersion game) => game switch
        {
            RBY or RD or BU or GN or YW => pkm.Japanese ? SlotsRGBY : SlotsRBY,

            GSC or GD or SV or C => GetEncounterTableGSC(pkm),

            R => SlotsR,
            S => SlotsS,
            E => SlotsE,
            FR => SlotsFR,
            LG => SlotsLG,
            CXD => SlotsXD,

            D => SlotsD,
            P => SlotsP,
            Pt => SlotsPt,
            HG => SlotsHG,
            SS => SlotsSS,

            B => SlotsB,
            W => SlotsW,
            B2 => SlotsB2,
            W2 => SlotsW2,

            X => SlotsX,
            Y => SlotsY,
            AS => SlotsA,
            OR => SlotsO,

            SN => SlotsSN,
            MN => SlotsMN,
            US => SlotsUS,
            UM => SlotsUM,
            GP => SlotsGP,
            GE => SlotsGE,

            GO => GetEncounterTableGO(pkm),
            SW => SlotsSW,
            SH => SlotsSH,
            _ => Array.Empty<EncounterArea>()
        };

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
            if (pkm.Species > MaxSpeciesID_1 && !EvolutionLegality.FutureEvolutionsGen1.Contains(pkm.Species))
                return SlotsGS;

            // Encounter could be any gen 2 game, it can have empty met location for have a g/s origin
            // or it can be a Crystal pokemon that lost met location after being tradeback to gen 1 games
            return SlotsGSC;
        }

        private static IEnumerable<EncounterArea> GetEncounterTableGO(PKM pkm)
        {
            if (pkm.Format < 8)
                return SlotsGO_GG;

            // If we know the met location, return the specific area list.
            // If we're just getting all encounters (lack of met location is kinda bad...), just return everything.
            var met = pkm.Met_Location;
            return met switch
            {
                Locations.GO8 => SlotsGO,
                Locations.GO7 => SlotsGO_GG,
                _ => SlotsGO_GG.Concat<EncounterArea>(SlotsGO),
            };
        }
    }
}
