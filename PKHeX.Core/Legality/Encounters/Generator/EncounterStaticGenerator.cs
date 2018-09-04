using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;

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
            return encs.Where(e => AllowGBCartEra || !GameVersion.GBCartEraOnly.Contains(e.Version));
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

                if (GetIsMatchDeferred(pkm, e))
                    deferred.Add(e);
                else
                    yield return e;
            }
            foreach (var e in deferred)
                yield return e;
        }

        private static bool GetIsMatchDeferred(PKM pkm, EncounterStatic e)
        {
            if (pkm.FatefulEncounter != e.Fateful)
                return true;
            if (e.Ability == 4 && pkm.AbilityNumber != 4) // BW/2 Jellicent collision with wild surf slot, resolved by duplicating the encounter with any abil
                return true;
            return false;
        }

        private static bool GetIsMatchStatic(PKM pkm, EncounterStatic e, int lvl)
        {
            if (e.Nature != Nature.Random && pkm.Nature != (int)e.Nature)
                return false;
            if (pkm.WasEgg != e.EggEncounter && pkm.Egg_Location == 0 && pkm.Format > 3 && pkm.GenNumber > 3 && !pkm.IsEgg)
                return false;
            if (e is EncounterStaticPID p && p.PID != pkm.PID)
                return false;

            if (pkm.Gen3 && e.EggLocation != 0) // Gen3 Egg
            {
                if (pkm.Format == 3 && pkm.IsEgg && e.EggLocation != pkm.Met_Location)
                    return false;
            }
            else if (pkm.VC || (pkm.GenNumber <= 2 && e.EggLocation != 0)) // Gen2 Egg
            {
                if (pkm.Format <= 2)
                {
                    if (pkm.IsEgg)
                    {
                        if (pkm.Met_Location != 0 && pkm.Met_Level != 0)
                            return false;
                    }
                    else
                    {
                        switch (pkm.Met_Level)
                        {
                            case 0 when pkm.Met_Location != 0:
                                return false;
                            case 1 when pkm.Met_Location == 0:
                                return false;
                            default:
                                if (pkm.Met_Location == 0 && pkm.Met_Level != 0)
                                    return false;
                                break;
                        }
                    }
                    if (pkm.Met_Level == 1)
                        lvl = 5; // met @ 1, hatch @ 5.
                }
            }
            else if (e.EggLocation != pkm.Egg_Location)
            {
                if (pkm.IsEgg) // unhatched
                {
                    if (e.EggLocation != pkm.Met_Location)
                        return false;
                    if (pkm.Egg_Location != 0)
                        return false;
                }
                else if (pkm.Gen4)
                {
                    if (pkm.Egg_Location != 2002) // Link Trade
                    {
                        // check Pt/HGSS data
                        if (pkm.Format <= 4)
                            return false; // must match
                        if (e.EggLocation >= 3000 || e.EggLocation <= 2010) // non-Pt/HGSS egg gift
                            return false;

                        // transferring 4->5 clears pt/hgss location value and keeps Faraway Place
                        if (pkm.Egg_Location != 3002) // Faraway Place
                            return false;
                    }
                }
                else
                {
                    if (pkm.Egg_Location != 30002) // Link Trade
                        return false;
                }
            }
            else if (e.EggLocation != 0 && pkm.Gen4)
            {
                // Check the inverse scenario for 4->5 eggs
                if (e.EggLocation < 3000 && e.EggLocation > 2010) // Pt/HGSS egg gift
                {
                    if (pkm.Format > 4)
                        return false; // locations match when it shouldn't
                }
            }

            if (pkm.HasOriginalMetLocation)
            {
                if (!e.EggEncounter && e.Location != 0 && e.Location != pkm.Met_Location)
                    return false;
                if (e.Level != lvl)
                {
                    if (!(pkm.Format == 3 && e.EggEncounter && lvl == 0))
                        return false;
                }
            }
            else if (e.Level > lvl)
            {
                return false;
            }

            if (e.Gender != -1 && e.Gender != pkm.Gender)
                return false;
            if (e.Form != pkm.AltForm && !e.SkipFormCheck && !IsFormChangeable(pkm, e.Species))
                return false;
            if (e.EggLocation == 60002 && e.Relearn.Length == 0 && pkm.RelearnMoves.Any(z => z != 0)) // gen7 eevee edge case
                return false;

            if (e.IVs != null && (e.Generation > 2 || pkm.Format <= 2)) // 1,2->7 regenerates IVs, only check if original IVs still exist
            {
                for (int i = 0; i < 6; i++)
                {
                    if (e.IVs[i] != -1 && e.IVs[i] != pkm.IVs[i])
                        return false;
                }
            }

            if (pkm is IContestStats s && s.IsContestBelow(e))
                return false;

            // Defer to EC/PID check
            // if (e.Shiny != null && e.Shiny != pkm.IsShiny)
            // continue;

            // Defer ball check to later
            // if (e.Gift && pkm.Ball != 4) // PokéBall
            // continue;

            if (pkm is PK1 pk1 && pk1.Gen1_NotTradeback && !IsValidCatchRatePK1(e, pk1))
                return false;

            if (!AllowGBCartEra && GameVersion.GBCartEraOnly.Contains(e.Version))
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
            var enc = new EncounterStatic
            {
                Species = species,
                Gift = true, // Forces Poké Ball
                Ability = TransferSpeciesDefaultAbility_1.Contains(species) ? 1 : 4, // Hidden by default, else first
                Shiny = species == 151 ? Shiny.Never : Shiny.Random,
                Fateful = species == 151,
                Location = Transfer1,
                EggLocation = 0,
                Level = pkmMetLevel,
                Version = GameVersion.RBY
            };
            enc.FlawlessIVCount = enc.Fateful ? 5 : 3;
            return enc;
        }

        private static EncounterStatic GetGSStaticTransfer(int species, int pkmMetLevel)
        {
            var enc = new EncounterStatic
            {
                Species = species,
                Gift = true, // Forces Poké Ball
                Ability = TransferSpeciesDefaultAbility_2.Contains(species) ? 1 : 4, // Hidden by default, else first
                Shiny = species == 151 ? Shiny.Never : Shiny.Random,
                Fateful = species == 151 || species == 251,
                Location = Transfer2,
                EggLocation = 0,
                Level = pkmMetLevel,
                Version = GameVersion.GSC
            };
            enc.FlawlessIVCount = enc.Fateful ? 5 : 3;
            return enc;
        }

        internal static EncounterStatic GetStaticLocation(PKM pkm, int species = -1)
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
            if (pk1.Species == 25 || pk1.Species == 26)
            {
                if (catch_rate == 190) // Red Blue Pikachu, is not a static encounter
                    return false;
                if (catch_rate == 163 && e.Level == 5) // Light Ball (Yellow) starter
                    return true;
            }

            if (e.Version == GameVersion.Stadium)
            {
                // Amnesia Psyduck has different catch rates depending on language
                if (e.Species == 054)
                    return catch_rate == (pk1.Japanese ? 167 : 168);
                return Stadium_CatchRate.Contains(catch_rate);
            }

            // Encounters can have different Catch Rates (RBG vs Y)
            var table = e.Version == GameVersion.Y ? PersonalTable.Y : PersonalTable.RB;
            var rate = table[e.Species].CatchRate;
            return catch_rate == rate;
        }
    }
}
