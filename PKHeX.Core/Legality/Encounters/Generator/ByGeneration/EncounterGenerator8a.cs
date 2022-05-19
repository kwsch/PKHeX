using System.Collections.Generic;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

internal static class EncounterGenerator8a
{
    public static IEnumerable<IEncounterable> GetEncounters(PKM pkm, EvoCriteria[] chain)
    {
        if (pkm is PK8 { SWSH: false })
            yield break;
        if (pkm.IsEgg)
            yield break;

        int ctr = 0;
        if (pkm.FatefulEncounter)
        {
            foreach (var z in GetValidGifts(pkm, chain, GameVersion.PLA))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
        }

        IEncounterable? cache = null;
        EncounterMatchRating rating = None;

        // Static Encounters can collide with wild encounters (close match); don't break if a Static Encounter is yielded.
        var encs = GetValidStaticEncounter(pkm, chain, GameVersion.PLA);
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

        foreach (var z in GetValidWildEncounters(pkm, chain, GameVersion.PLA))
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
