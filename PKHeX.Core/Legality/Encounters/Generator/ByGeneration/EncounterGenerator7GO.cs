using System.Collections.Generic;
using static PKHeX.Core.EncounterTypeGroup;

namespace PKHeX.Core;

public sealed class EncounterGenerator7GO : IEncounterGenerator
{
    public static readonly EncounterGenerator7GO Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

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
        var iterator = new EncounterEnumerator7GO(pk, chain);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }
}
