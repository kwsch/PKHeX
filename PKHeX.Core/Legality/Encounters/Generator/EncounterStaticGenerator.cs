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
    public static class EncounterStaticGenerator
    {
        public static IEnumerable<EncounterStatic> GetPossible(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            int gen = pkm.GenNumber;
            int maxID = gen == 2 ? MaxSpeciesID_2 : gen == 1 ? MaxSpeciesID_1 : -1;
            var dl = EvolutionChain.GetValidPreEvolutions(pkm, maxID);
            return GetPossible(pkm, dl, gameSource);
        }

        public static IEnumerable<EncounterStatic> GetPossible(PKM pkm, IReadOnlyList<DexLevel> vs, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var encs = GetStaticEncounters(pkm, vs, gameSource);
            return encs.Where(e => ParseSettings.AllowGBCartEra || !GameVersion.GBCartEraOnly.Contains(e.Version));
        }

        public static IEnumerable<EncounterStatic> GetValidStaticEncounter(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            var poss = GetPossible(pkm, gameSource: gameSource);

            int lvl = GetMinLevelEncounter(pkm);
            if (lvl < 0)
                return Enumerable.Empty<EncounterStatic>();

            // Back Check against pkm
            return GetMatchingStaticEncounters(pkm, poss, lvl);
        }

        public static IEnumerable<EncounterStatic> GetValidStaticEncounter(PKM pkm, IReadOnlyList<DexLevel> vs, GameVersion gameSource)
        {
            var poss = GetPossible(pkm, vs, gameSource: gameSource);

            int lvl = GetMinLevelEncounter(pkm);
            if (lvl < 0)
                return Enumerable.Empty<EncounterStatic>();

            // Back Check against pkm
            return GetMatchingStaticEncounters(pkm, poss, lvl);
        }

        private static IEnumerable<EncounterStatic> GetMatchingStaticEncounters(PKM pkm, IEnumerable<EncounterStatic> poss, int lvl)
        {
            // check for petty rejection scenarios that will be flagged by other legality checks
            var deferred = new List<EncounterStatic>();
            foreach (EncounterStatic e in poss)
            {
                if (!GetIsMatchStatic(pkm, e, lvl))
                    continue;

                if (e.IsMatchDeferred(pkm))
                    deferred.Add(e);
                else
                    yield return e;
            }
            foreach (var e in deferred)
                yield return e;
        }

        private static bool GetIsMatchStatic(PKM pkm, EncounterStatic e, int lvl)
        {
            if (!e.IsMatch(pkm, lvl))
                return false;

            if (pkm is PK1 pk1 && pk1.Gen1_NotTradeback && !IsValidCatchRatePK1(e, pk1))
                return false;

            if (!ParseSettings.AllowGBCartEra && GameVersion.GBCartEraOnly.Contains(e.Version))
                return false;

            return true;
        }

        private static IEnumerable<EncounterStatic> GetStaticEncounters(PKM pkm, IReadOnlyList<DexLevel> dl, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var table = GetEncounterStaticTable(pkm, gameSource);
            return table.Where(e => dl.Any(d => d.Species == e.Species));
        }

        internal static IEncounterable GetVCStaticTransferEncounter(PKM pkm)
        {
            if (pkm.VC1)
                return GetRBYStaticTransfer(pkm.Species, pkm.Met_Level);
            if (pkm.VC2)
                return GetGSStaticTransfer(pkm.Species, pkm.Met_Level);
            return new EncounterInvalid(pkm);
        }

        private static EncounterStatic GetRBYStaticTransfer(int species, int pkmMetLevel)
        {
            bool mew = species == (int)Species.Mew;
            return new EncounterStatic
            {
                Species = species,
                Gift = true, // Forces Poké Ball
                Ability = TransferSpeciesDefaultAbility_1.Contains(species) ? 1 : 4, // Hidden by default, else first
                Shiny = mew ? Shiny.Never : Shiny.Random,
                Fateful = mew,
                Location = Transfer1,
                EggLocation = 0,
                Level = pkmMetLevel,
                Generation = 7,
                Version = GameVersion.RBY,
                FlawlessIVCount = mew ? 5 : 3,
            };
        }

        private static EncounterStatic GetGSStaticTransfer(int species, int pkmMetLevel)
        {
            bool mew = species == (int) Species.Mew;
            bool fateful = mew || species == (int) Species.Celebi;
            return new EncounterStatic
            {
                Species = species,
                Gift = true, // Forces Poké Ball
                Ability = TransferSpeciesDefaultAbility_2.Contains(species) ? 1 : 4, // Hidden by default, else first
                Shiny = mew ? Shiny.Never : Shiny.Random,
                Fateful = fateful,
                Location = Transfer2,
                EggLocation = 0,
                Level = pkmMetLevel,
                Generation = 7,
                Version = GameVersion.GSC,
                FlawlessIVCount = fateful ? 5 : 3
            };
        }

        internal static EncounterStatic? GetStaticLocation(PKM pkm, int species = -1)
        {
            switch (pkm.GenNumber)
            {
                case 1:
                    return GetRBYStaticTransfer(species, pkm.Met_Level);
                case 2:
                    return GetGSStaticTransfer(species, pkm.Met_Level);
                default:
                    var dl = EvolutionChain.GetValidPreEvolutions(pkm, lvl: 100, skipChecks: true);
                    return GetPossible(pkm, dl).FirstOrDefault();
            }
        }

        internal static bool IsVCStaticTransferEncounterValid(PKM pkm, EncounterStatic e)
        {
            return pkm.Met_Location == e.Location && pkm.Egg_Location == e.EggLocation;
        }

        private static bool IsValidCatchRatePK1(EncounterStatic e, PK1 pk1)
        {
            var catch_rate = pk1.Catch_Rate;
            // Pure gen 1, trades can be filter by catch rate
            if (pk1.Species == (int)Species.Pikachu || pk1.Species == (int)Species.Raichu)
            {
                if (catch_rate == 190) // Red Blue Pikachu, is not a static encounter
                    return false;
                if (catch_rate == 163 && e.Level == 5) // Light Ball (Yellow) starter
                    return true;
            }

            if (e.Version == GameVersion.Stadium)
            {
                // Amnesia Psyduck has different catch rates depending on language
                if (e.Species == (int)Species.Psyduck)
                    return catch_rate == (pk1.Japanese ? 167 : 168);
                return GBRestrictions.Stadium_CatchRate.Contains(catch_rate);
            }

            // Encounters can have different Catch Rates (RBG vs Y)
            var table = e.Version == GameVersion.Y ? PersonalTable.Y : PersonalTable.RB;
            var rate = table[e.Species].CatchRate;
            return catch_rate == rate;
        }

        // Generation Specific Fetching
        private static IEnumerable<EncounterStatic> GetEncounterStaticTable(PKM pkm, GameVersion gameSource = GameVersion.Any)
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
                    return StaticRBY;

                case GameVersion.GSC:
                case GameVersion.GD:
                case GameVersion.SV:
                case GameVersion.C:
                    return GetEncounterStaticTableGSC(pkm);

                case GameVersion.R: return StaticR;
                case GameVersion.S: return StaticS;
                case GameVersion.E: return StaticE;
                case GameVersion.FR: return StaticFR;
                case GameVersion.LG: return StaticLG;
                case GameVersion.CXD: return Encounter_CXD;

                case GameVersion.D: return StaticD;
                case GameVersion.P: return StaticP;
                case GameVersion.Pt: return StaticPt;
                case GameVersion.HG: return StaticHG;
                case GameVersion.SS: return StaticSS;

                case GameVersion.B: return StaticB;
                case GameVersion.W: return StaticW;
                case GameVersion.B2: return StaticB2;
                case GameVersion.W2: return StaticW2;

                case GameVersion.X: return StaticX;
                case GameVersion.Y: return StaticY;
                case GameVersion.AS: return StaticA;
                case GameVersion.OR: return StaticO;

                case GameVersion.SN: return StaticSN;
                case GameVersion.MN: return StaticMN;
                case GameVersion.US: return StaticUS;
                case GameVersion.UM: return StaticUM;

                case GameVersion.GP: return StaticGP;
                case GameVersion.GE: return StaticGE;

                case GameVersion.SW: return StaticSW;
                case GameVersion.SH: return StaticSH;

                default: return Enumerable.Empty<EncounterStatic>();
            }
        }

        private static IEnumerable<EncounterStatic> GetEncounterStaticTableGSC(PKM pkm)
        {
            if (!ParseSettings.AllowGen2Crystal(pkm))
                return StaticGS;
            if (pkm.Format != 2)
                return StaticGSC;

            if (pkm.HasOriginalMetLocation)
                return StaticC;
            return StaticGSC;
        }
    }
}
