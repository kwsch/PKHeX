using System.Collections.Generic;
using static PKHeX.Core.EncounterTypeGroup;

namespace PKHeX.Core;

public sealed class EncounterGenerator8GO : IEncounterGenerator
{
    public static readonly EncounterGenerator8GO Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

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
        var iterator = new EncounterEnumerator8GO(pk, chain);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }
}
