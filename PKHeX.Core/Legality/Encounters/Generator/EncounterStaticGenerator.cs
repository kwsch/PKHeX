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

namespace PKHeX.Core
{
    public static class EncounterStaticGenerator
    {
        public static IEnumerable<EncounterStatic> GetPossible(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var table = GetEncounterStaticTable(pkm, gameSource);
            return table.Where(e => chain.Any(d => d.Species == e.Species));
        }

        public static IEnumerable<EncounterStatic> GetPossibleGBGifts(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            static IEnumerable<EncounterStatic> GetEvents(GameVersion g)
            {
                if (g == GameVersion.RBY)
                    return !ParseSettings.AllowGBCartEra ? Encounters1.StaticEventsVC : Encounters1.StaticEventsGB;

                return !ParseSettings.AllowGBCartEra ? Encounters2.StaticEventsVC : Encounters2.StaticEventsGB;
            }

            var table = GetEvents(gameSource);
            return table.Where(e => chain.Any(d => d.Species == e.Species));
        }

        public static IEnumerable<EncounterStatic> GetValidStaticEncounter(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource = GameVersion.Any)
        {
            var poss = GetPossible(pkm, chain, gameSource: gameSource);

            // Back Check against pkm
            return GetMatchingStaticEncounters(pkm, poss, chain);
        }

        public static IEnumerable<EncounterStatic> GetValidGBGifts(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var poss = GetPossibleGBGifts(pkm, chain, gameSource: gameSource);
            foreach (EncounterStatic e in poss)
            {
                foreach (var dl in chain)
                {
                    if (dl.Species != e.Species)
                        continue;
                    if (!e.IsMatch(pkm, dl))
                        continue;

                    yield return e;
                }
            }
        }

        private static IEnumerable<EncounterStatic> GetMatchingStaticEncounters(PKM pkm, IEnumerable<EncounterStatic> poss, IReadOnlyList<DexLevel> evos)
        {
            // check for petty rejection scenarios that will be flagged by other legality checks
            var deferred = new List<EncounterStatic>();
            foreach (EncounterStatic e in poss)
            {
                foreach (var dl in evos)
                {
                    if (dl.Species != e.Species)
                        continue;
                    if (!e.IsMatch(pkm, dl))
                        continue;

                    if (e.IsMatchDeferred(pkm))
                        deferred.Add(e);
                    else
                        yield return e;
                    break;
                }
            }
            foreach (var e in deferred)
                yield return e;
        }

        internal static EncounterStatic7 GetVCStaticTransferEncounter(PKM pkm, IEncounterable enc)
        {
            var species = pkm.Species;
            var met = pkm.Met_Level;
            if (pkm.VC1)
                return EncounterStatic7.GetVC1(species > MaxSpeciesID_1 ? enc.Species : species, met);
            if (pkm.VC2)
                return EncounterStatic7.GetVC2(species > MaxSpeciesID_2 ? enc.Species : species, met);

            // Should never reach here.
            throw new ArgumentException(nameof(pkm.Version));
        }

        internal static EncounterStatic? GetStaticLocation(PKM pkm, int species = -1)
        {
            switch (pkm.Generation)
            {
                case 1:
                    return EncounterStatic7.GetVC1(species, pkm.Met_Level);
                case 2:
                    return EncounterStatic7.GetVC2(species, pkm.Met_Level);
                default:
                    var chain = EvolutionChain.GetValidPreEvolutions(pkm, maxLevel: 100, skipChecks: true);
                    return GetPossible(pkm, chain)
                        .OrderBy(z => !chain.Any(s => s.Species == z.Species && s.Form == z.Form))
                        .ThenBy(z => z.LevelMin)
                        .FirstOrDefault();
            }
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
