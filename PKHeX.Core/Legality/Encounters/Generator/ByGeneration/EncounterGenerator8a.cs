using System.Collections.Generic;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterTypeGroup;

namespace PKHeX.Core;

public sealed class EncounterGenerator8a : IEncounterGenerator
{
    public static readonly EncounterGenerator8a Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            var table = EncounterEvent.MGDB_G8A;
            foreach (var enc in GetPossibleAll(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            var table = Encounters8a.StaticLA;
            foreach (var enc in GetPossibleAll(chain, table))
                yield return enc;
        }

        if (groups.HasFlag(Slot))
        {
            var areas = Encounters8a.SlotsLA;
            foreach (var enc in GetPossibleSlots(chain, areas))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea8a[] areas)
    {
        foreach (var area in areas)
        {
            foreach (var slot in area.Slots)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != slot.Species)
                        continue;
                    yield return slot;
                    break;
                }
            }
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var iterator = new EncounterEnumerator8a(pk, chain);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }
}
