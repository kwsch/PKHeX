using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterEggGenerator;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core
{
    internal static class EncounterGenerator4
    {
        public static IEnumerable<IEncounterable> GetEncounters(PKM pkm, LegalInfo info)
        {
            info.PIDIV = MethodFinder.Analyze(pkm);
            var deferredPIDIV = new List<IEncounterable>();
            var deferredEType = new List<IEncounterable>();

            foreach (var z in GenerateRawEncounters4(pkm, info))
            {
                if (!info.PIDIV.Type.IsCompatible4(z, pkm))
                    deferredPIDIV.Add(z);
                else if (pkm is IGroundTile e && !(z is IGroundTypeTile t ? t.GroundTile.Contains(e.GroundTile) : e.GroundTile == 0))
                    deferredEType.Add(z);
                else
                    yield return z;
            }

            foreach (var z in deferredEType)
                yield return z;

            if (deferredPIDIV.Count == 0)
                yield break;

            info.PIDIVMatches = false;
            foreach (var z in deferredPIDIV)
                yield return z;
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters4(PKM pkm, LegalInfo info)
        {
            var chain = EncounterOrigin.GetOriginChain(pkm);
            var game = (GameVersion)pkm.Version;
            if (pkm.FatefulEncounter)
            {
                int ctr = 0;
                foreach (var z in GetValidGifts(pkm, chain, game))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }
            if (Locations.IsEggLocationBred4(pkm.Egg_Location, game))
            {
                foreach (var z in GenerateEggs(pkm, 4))
                    yield return z;
            }
            foreach (var z in GetValidEncounterTrades(pkm, chain, game))
                yield return z;

            IEncounterable? deferred = null;
            IEncounterable? partial = null;

            bool safariSport = pkm.Ball is (int)Ball.Sport or (int)Ball.Safari; // never static encounters
            if (!safariSport)
            {
                foreach (var z in GetValidStaticEncounter(pkm, chain, game))
                {
                    var match = z.GetMatchRating(pkm);
                    if (match == PartialMatch)
                        partial ??= z;
                    else
                        yield return z;
                }
            }

            var slots = FrameFinder.GetFrames(info.PIDIV, pkm).ToList();
            foreach (var slot in GetValidWildEncounters(pkm, chain, game))
            {
                var z = (EncounterSlot4)slot;
                var match = z.GetMatchRating(pkm);
                if (match == PartialMatch)
                {
                    partial ??= z;
                    continue;
                }

                // Can use Radar to force the encounter slot to stay consistent across encounters.
                if (z.CanUseRadar)
                {
                    yield return slot;
                    continue;
                }

                var frame = slots.Find(s => s.IsSlotCompatibile(z, pkm));
                if (frame == null)
                {
                    deferred ??= z;
                    continue;
                }
                yield return z;
            }

            info.FrameMatches = false;
            if (deferred is EncounterSlot4 x)
                yield return x;

            if (partial is EncounterSlot4 y)
            {
                var frame = slots.Find(s => s.IsSlotCompatibile(y, pkm));
                info.FrameMatches = frame != null;
                yield return y;
            }

            // do static encounters if they were deferred to end, spit out any possible encounters for invalid pkm
            if (!safariSport)
                yield break;

            foreach (var z in GetValidStaticEncounter(pkm, chain, game))
            {
                var match = z.GetMatchRating(pkm);
                if (match == PartialMatch)
                    partial ??= z;
                else
                    yield return z;
            }

            if (partial is not null)
                yield return partial;
        }
    }
}
