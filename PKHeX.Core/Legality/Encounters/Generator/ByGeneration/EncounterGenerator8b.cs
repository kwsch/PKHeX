using System.Collections.Generic;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterEggGenerator;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core
{
    internal static class EncounterGenerator8b
    {
        public static IEnumerable<IEncounterable> GetEncounters(PKM pkm, EvoCriteria[] chain)
        {
            if (pkm is PK8)
                yield break;
            int ctr = 0;
            var game = (GameVersion)pkm.Version;

            if (pkm.FatefulEncounter)
            {
                foreach (var z in GetValidGifts(pkm, chain, game))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            if (Locations.IsEggLocationBred8b(pkm.Egg_Location))
            {
                foreach (var z in GenerateEggs(pkm, 8))
                { yield return z; ++ctr; }
                if (ctr == 0) yield break;
            }

            IEncounterable? cache = null;
            EncounterMatchRating rating = None;

            // Trades
            if (!pkm.IsEgg && pkm.Met_Location == Locations.LinkTrade6NPC)
            {
                foreach (var z in GetValidEncounterTrades(pkm, chain, game))
                {
                    var match = z.GetMatchRating(pkm);
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
            var encs = GetValidStaticEncounter(pkm, chain, game);
            foreach (var z in encs)
            {
                var match = z.GetMatchRating(pkm);
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

            foreach (var z in GetValidWildEncounters(pkm, chain, game))
            {
                var match = z.GetMatchRating(pkm);
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

        public static IEnumerable<IEncounterable> GetEncountersFuzzy(PKM pkm, EvoCriteria[] chain, GameVersion game)
        {
            int ctr = 0;

            if (pkm.FatefulEncounter)
            {
                foreach (var z in GetValidGifts(pkm, chain, game))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            if (pkm.Met_Level == 1)
            {
                foreach (var z in GenerateEggs(pkm, 8))
                { yield return z; ++ctr; }
                if (ctr == 0) yield break;
            }

            IEncounterable? cache = null;
            EncounterMatchRating rating = None;

            // Trades
            if (!pkm.IsEgg)
            {
                foreach (var z in GetValidEncounterTrades(pkm, chain, game))
                {
                    var match = z.GetMatchRating(pkm);
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
            var encs = GetValidStaticEncounter(pkm, chain, game);
            foreach (var z in encs)
            {
                var match = z.GetMatchRating(pkm);
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

            foreach (var z in GetValidWildEncounters(pkm, chain, game))
            {
                var match = z.GetMatchRating(pkm);
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
}
