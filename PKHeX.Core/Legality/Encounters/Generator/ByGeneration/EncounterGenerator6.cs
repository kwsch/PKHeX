using System.Collections.Generic;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterEggGenerator;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core
{
    internal static class EncounterGenerator6
    {
        public static IEnumerable<IEncounterable> GetEncounters(PKM pkm)
        {
            int ctr = 0;

            var chain = EncounterOrigin.GetOriginChain(pkm);

            IEncounterable? deferred = null;
            IEncounterable? partial = null;

            if (pkm.WasEvent || pkm.WasEventEgg || pkm.WasLink)
            {
                foreach (var z in GetValidGifts(pkm, chain))
                {
                    var match = z.GetMatchRating(pkm);
                    switch (match)
                    {
                        case Match: yield return z; break;
                        case Deferred: deferred ??= z; break;
                        case PartialMatch: partial ??= z; break;
                    }
                    ++ctr;
                }

                if (ctr != 0)
                {
                    if (deferred != null)
                        yield return deferred;

                    if (partial != null)
                        yield return partial;

                    yield break;
                }
            }

            if (pkm.WasBredEgg)
            {
                foreach (var z in GenerateEggs(pkm, 6))
                { yield return z; ++ctr; }
                if (ctr == 0) yield break;
            }

            foreach (var z in GetValidStaticEncounter(pkm, chain))
            {
                var match = z.GetMatchRating(pkm);
                switch (match)
                {
                    case Match: yield return z; ++ctr; break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
            }
            if (ctr != 0) yield break;

            foreach (var z in GetValidWildEncounters(pkm, chain))
            {
                var match = z.GetMatchRating(pkm);
                switch (match)
                {
                    case Match: yield return z; ++ctr; break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
            }
            if (ctr != 0) yield break;

            foreach (var z in GetValidEncounterTrades(pkm, chain))
            {
                var match = z.GetMatchRating(pkm);
                switch (match)
                {
                    case Match: yield return z; /*++ctr*/ break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
            }

            if (deferred != null)
                yield return deferred;

            if (partial != null)
                yield return partial;
        }
    }
}
