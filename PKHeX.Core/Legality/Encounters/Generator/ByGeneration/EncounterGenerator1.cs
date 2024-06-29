using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EncounterGenerator1 : IEncounterGenerator
{
    public static readonly EncounterGenerator1 Instance = new();
    public bool CanGenerateEggs => false;

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible1(chain, groups, game);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        throw new ArgumentException("Generator does not support direct calls to this method.");
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, GameVersion game)
    {
        // Since encounter matching is super weak due to limited stored data in the structure
        // Calculate all 3 at the same time and pick the best result (by species).
        // Favor special event move gifts as Static Encounters when applicable
        var chain = EncounterOrigin.GetOriginChain12(pk, game);
        if (chain.Length == 0)
            return [];
        return GetEncounters(pk, chain);
    }

    private static IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain)
    {
        var iterator = new EncounterEnumerator1(pk, chain);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }
}
