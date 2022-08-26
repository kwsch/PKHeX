using System.Collections.Generic;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterEggGenerator;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

internal static class EncounterGenerator8b
{
    public static IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain)
    {
        if (pk is PK8)
            yield break;
        int ctr = 0;
        var game = (GameVersion)pk.Version;

        if (pk.FatefulEncounter)
        {
            foreach (var z in GetValidGifts(pk, chain, game))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
        }

        if (Locations.IsEggLocationBred8b(pk.Egg_Location))
        {
            foreach (var z in GenerateEggs(pk, 8))
            { yield return z; ++ctr; }
            if (ctr == 0) yield break;
        }

        IEncounterable? cache = null;
        EncounterMatchRating rating = MaxNotMatch;

        // Trades
        if (!pk.IsEgg && pk.Met_Location == Locations.LinkTrade6NPC)
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

    public static IEnumerable<IEncounterable> GetEncountersFuzzy(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        int ctr = 0;

        if (pk.FatefulEncounter)
        {
            foreach (var z in GetValidGifts(pk, chain, game))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
        }

        var wasEgg = pk.Egg_Location switch
        {
            Locations.HOME_SWSHBDSPEgg => true, // Regular hatch location (not link trade)
            Locations.HOME_SWBD => pk.Met_Location == Locations.HOME_SWBD, // Link Trade transferred over must match Met Location
            Locations.HOME_SHSP => pk.Met_Location == Locations.HOME_SHSP, // Link Trade transferred over must match Met Location
            _ => false,
        };
        if (wasEgg && pk.Met_Level == 1)
        {
            foreach (var z in GenerateEggs(pk, 8))
            { yield return z; ++ctr; }
            if (ctr == 0) yield break;
        }

        IEncounterable? cache = null;
        EncounterMatchRating rating = MaxNotMatch;

        // Trades
        if (!pk.IsEgg)
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

        // Only yield if Safari and Marsh encounters match.
        bool safari = pk is PK8 { Ball: (int)Ball.Safari };
        foreach (var z in GetValidWildEncounters(pk, chain, game))
        {
            var marsh = Locations.IsSafariZoneLocation8b(z.Location);
            var match = z.GetMatchRating(pk);
            if (safari != marsh)
                match = DeferredErrors;
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
