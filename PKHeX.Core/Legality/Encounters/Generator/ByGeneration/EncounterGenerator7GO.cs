using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EncounterGenerator7GO : IEncounterGenerator
{
    public static readonly EncounterGenerator7GO Instance = new();
    public bool CanGenerateEggs => false;

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion __, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible7GO(chain, groups);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var iterator = new EncounterEnumerator7GO(pk, chain);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }
}
