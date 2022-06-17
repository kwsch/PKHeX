using System.Collections.Generic;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterEggGenerator;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

internal static class EncounterGenerator6
{
    public static IEnumerable<IEncounterable> GetEncounters(PKM pk)
    {
        int ctr = 0;

        var chain = EncounterOrigin.GetOriginChain(pk);
        var game = (GameVersion)pk.Version;

        IEncounterable? deferred = null;
        IEncounterable? partial = null;

        if (pk.FatefulEncounter || pk.Met_Location == Locations.LinkGift6)
        {
            foreach (var z in GetValidGifts(pk, chain, game))
            {
                var match = z.GetMatchRating(pk);
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

        if (Locations.IsEggLocationBred6(pk.Egg_Location))
        {
            foreach (var z in GenerateEggs(pk, 6))
            { yield return z; ++ctr; }
            if (ctr == 0) yield break;
        }

        foreach (var z in GetValidStaticEncounter(pk, chain, game))
        {
            var match = z.GetMatchRating(pk);
            switch (match)
            {
                case Match: yield return z; ++ctr; break;
                case Deferred: deferred ??= z; break;
                case PartialMatch: partial ??= z; break;
            }
        }
        if (ctr != 0) yield break;

        foreach (var z in GetValidWildEncounters(pk, chain, game))
        {
            var match = z.GetMatchRating(pk);
            switch (match)
            {
                case Match: yield return z; ++ctr; break;
                case Deferred: deferred ??= z; break;
                case PartialMatch: partial ??= z; break;
            }
        }
        if (ctr != 0) yield break;

        foreach (var z in GetValidEncounterTrades(pk, chain, game))
        {
            var match = z.GetMatchRating(pk);
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
