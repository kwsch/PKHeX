using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator8GO : IEncounterGenerator
{
    public static readonly EncounterGenerator8GO Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (groups.HasFlag(Slot))
        {
            var table = EncountersGO.SlotsGO;
            foreach (var enc in GetPossibleSlots(chain, table))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea8g[] table)
    {
        foreach (var area in table)
        {
            foreach (var evo in chain)
            {
                if (area.Species != evo.Species)
                    continue;

                foreach (var slot in area.Slots)
                    yield return slot;
                break;
            }
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        if (pk.TSV == 0) // HOME doesn't assign TSV=0 to accounts.
            yield break;
        if (!CanBeWildEncounter(pk))
            yield break;

        IEncounterable? deferred = null;
        IEncounterable? partial = null;

        bool yielded = false;
        foreach (var area in EncountersGO.SlotsGO)
        {
            foreach (var evo in chain)
            {
                if (area.Species != evo.Species)
                    continue;

                var slots = area.GetMatchingSlots(pk, evo);
                foreach (var z in slots)
                {
                    var match = z.GetMatchRating(pk);
                    switch (match)
                    {
                        case Match: yield return z; yielded = true; break;
                        case Deferred: deferred ??= z; break;
                        case PartialMatch: partial ??= z; break;
                    }
                }
                break;
            }
        }
        if (yielded)
            yield break;

        if (deferred != null)
            yield return deferred;
        else if (partial != null)
            yield return partial;
    }
}
