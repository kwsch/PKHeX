using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EncounterGeneratorGO : IEncounterGenerator
{
    public static readonly EncounterGeneratorGO Instance = new();
    public bool CanGenerateEggs => false;

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var loc = pk.MetLocation;
        if (loc == Locations.GO7)
            return EncounterGenerator7GO.Instance.GetEncounters(pk, chain, info);
        if (loc == Locations.GO8 && pk is not PB7)
            return EncounterGenerator8GO.Instance.GetEncounters(pk, chain, info);
        return [];
    }

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        var lgpe = EncounterGenerator7GO.Instance.GetPossible(pk, chain, game, groups);
        foreach (var enc in lgpe)
            yield return enc;

        if (pk is PB7)
            yield break;

        var home = EncounterGenerator8GO.Instance.GetPossible(pk, chain, game, groups);
        foreach (var enc in home)
            yield return enc;
    }
}
