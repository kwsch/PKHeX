using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Encounter Generator for <see cref="GameVersion.GP"/> &amp; <see cref="GameVersion.GE"/>
/// </summary>
public sealed class EncounterGenerator7GG : IEncounterGenerator
{
    public static readonly EncounterGenerator7GG Instance = new();
    public bool CanGenerateEggs => false;

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible7GG(chain, groups, game);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var iterator = new EncounterEnumerator7GG(pk, chain, pk.Version);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }
}
