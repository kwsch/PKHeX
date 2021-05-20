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
                else if (pkm.Format <= 6 && !(z is IEncounterTypeTile t ? t.TypeEncounter.Contains(pkm.EncounterType) : pkm.EncounterType == 0))
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
            if (pkm.FatefulEncounter)
            {
                int ctr = 0;
                foreach (var z in GetValidGifts(pkm, chain))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }
            if (pkm.WasBredEgg)
            {
                foreach (var z in GenerateEggs(pkm, 4))
                    yield return z;
            }
            foreach (var z in GetValidEncounterTrades(pkm, chain))
                yield return z;

            IEncounterable? deferred = null;
            IEncounterable? partial = null;

            bool sport = pkm.Ball == (int)Ball.Sport; // never static encounters (conflict with non bcc / bcc)
            bool safari = pkm.Ball == (int)Ball.Safari; // never static encounters
            bool safariSport = safari || sport;
            if (!safariSport)
            {
                foreach (var z in GetValidStaticEncounter(pkm, chain))
                {
                    var match = z.GetMatchRating(pkm);
                    if (match == PartialMatch)
                        partial ??= z;
                    else
                        yield return z;
                }
            }

            var slots = FrameFinder.GetFrames(info.PIDIV, pkm).ToList();
            foreach (var z in GetValidWildEncounters34(pkm, chain))
            {
                var match = z.GetMatchRating(pkm);
                if (match == PartialMatch)
                {
                    partial ??= z;
                    continue;
                }

                var frame = slots.Find(s => s.IsSlotCompatibile((EncounterSlot4)z, pkm));
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

            foreach (var z in GetValidStaticEncounter(pkm, chain))
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
