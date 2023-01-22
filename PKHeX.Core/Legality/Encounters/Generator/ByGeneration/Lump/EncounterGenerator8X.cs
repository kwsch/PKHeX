using System.Collections.Generic;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.Locations;

namespace PKHeX.Core;

public sealed class EncounterGenerator8X : IEncounterGenerator
{
    public static readonly EncounterGenerator8X Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups) => game switch
    {
        GO => EncounterGenerator8GO.Instance.GetPossible(pk, chain, game, groups),
        PLA => EncounterGenerator8a.Instance.GetPossible(pk, chain, game, groups),
        BD or SP => EncounterGenerator8b.Instance.GetPossible(pk, chain, game, groups),
        _ => EncounterGenerator8.Instance.GetPossible(pk, chain, game, groups),
    };

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info) => (GameVersion)pk.Version switch
    {
        GO => EncounterGenerator8GO.Instance.GetEncounters(pk, chain, info),
        PLA => EncounterGenerator8a.Instance.GetEncounters(pk, chain, info),
        BD or SP => EncounterGenerator8b.Instance.GetEncounters(pk, chain, info),
        SW when pk.Met_Location == HOME_SWLA => EncounterGenerator8a.Instance.GetEncounters(pk, chain, info),
        SW when pk.Met_Location == HOME_SWBD => EncounterGenerator8b.Instance.GetEncountersSWSH(pk, chain, BD),
        SH when pk.Met_Location == HOME_SHSP => EncounterGenerator8b.Instance.GetEncountersSWSH(pk, chain, SP),
        _ => EncounterGenerator8.Instance.GetEncounters(pk, chain, info),
    };
}
