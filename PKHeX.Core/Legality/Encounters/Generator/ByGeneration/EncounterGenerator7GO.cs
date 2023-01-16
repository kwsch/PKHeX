using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator7GO : IEncounterGenerator
{
    public static readonly EncounterGenerator7GO Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (groups.HasFlag(Slot))
        {
            var areas = EncountersGO.SlotsGO_GG;
            foreach (var enc in GetPossibleSlots(chain, areas))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea7g[] areas)
    {
        foreach (var area in areas)
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
        if (!CanBeWildEncounter(pk))
            yield break;

        IEncounterable? deferred = null;
        IEncounterable? partial = null;

        bool yielded = false;
        foreach (var area in EncountersGO.SlotsGO_GG)
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
