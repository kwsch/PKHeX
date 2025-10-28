using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EncounterGenerator9a : IEncounterGenerator
{
    public static readonly EncounterGenerator9a Instance = new();
    public bool CanGenerateEggs => false;

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion __, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible9a(chain, groups);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var iterator = new EncounterEnumerator9a(pk, chain);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }
}
