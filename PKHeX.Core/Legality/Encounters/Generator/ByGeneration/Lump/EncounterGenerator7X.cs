using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EncounterGenerator7X : IEncounterGenerator
{
    public static readonly EncounterGenerator7X Instance = new();
    public bool CanGenerateEggs => false;

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion version, EncounterTypeGroup groups) => pk.Version switch
    {
        GameVersion.GO => EncounterGenerator7GO.Instance.GetPossible(pk, chain, version, groups),
        > GameVersion.GO => EncounterGenerator7GG.Instance.GetPossible(pk, chain, version, groups),
        _ => EncounterGenerator7.Instance.GetPossible(pk, chain, version, groups),
    };

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var context = pk.Version.IsGen7() ? EntityContext.Gen7 : EntityContext.Gen7b;
        var chain = EncounterOrigin.GetOriginChain(pk, 7, context);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info) => pk.Version switch
    {
        GameVersion.GO => EncounterGenerator7GO.Instance.GetEncounters(pk, chain, info),
        > GameVersion.GO => EncounterGenerator7GG.Instance.GetEncounters(pk, chain, info),
        _ => EncounterGenerator7.Instance.GetEncounters(pk, chain, info),
    };
}
