using System.Collections.Generic;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterEggGenerator;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

internal static class EncounterGenerator9
{
    public static IEnumerable<IEncounterable> GetEncounters(PKM pk)
    {
        var chain = EncounterOrigin.GetOriginChain(pk);
        return GetEncounters(pk, chain);
    }

    private static IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain)
    {
        int ctr = 0;
        var game = (GameVersion)pk.Version;

        if (pk.FatefulEncounter)
        {
            foreach (var z in GetValidGifts(pk, chain, game))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
        }

        // While an unhatched picnic egg, the Version remains 0.
        if (Locations.IsEggLocationBred9(pk.Egg_Location) && !(pk.IsEgg && pk.Version != 0))
        {
            foreach (var z in GenerateEggs(pk, 9))
            { yield return z; ++ctr; }
            if (ctr == 0) yield break;
        }

        IEncounterable? cache = null;
        EncounterMatchRating rating = MaxNotMatch;

        // Trades
        if (pk.Met_Location == Locations.LinkTrade6NPC)
        {
            foreach (var z in GetValidEncounterTrades(pk, chain, game))
            {
                var match = z.GetMatchRating(pk);
                if (match == Match)
                {
                    yield return z;
                }
                else if (match < rating)
                {
                    cache = z;
                    rating = match;
                }
            }

            if (cache != null)
                yield return cache;
            yield break;
        }

        // Static Encounters can collide with wild encounters (close match); don't break if a Static Encounter is yielded.
        var encs = GetValidStaticEncounter(pk, chain, game);
        foreach (var z in encs)
        {
            var match = z.GetMatchRating(pk);
            if (match == Match)
            {
                yield return z;
            }
            else if (match < rating)
            {
                cache = z;
                rating = match;
            }
        }

        foreach (var z in GetValidWildEncounters(pk, chain, game))
        {
            var match = z.GetMatchRating(pk);
            if (match == Match)
            {
                yield return z;
            }
            else if (match < rating)
            {
                cache = z;
                rating = match;
            }
        }

        if (cache != null)
            yield return cache;
    }
}
