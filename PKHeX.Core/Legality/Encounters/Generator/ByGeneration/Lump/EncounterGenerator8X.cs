using System.Collections.Generic;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public sealed class EncounterGenerator8X : IEncounterGenerator
{
    public static readonly EncounterGenerator8X Instance = new();
    public bool CanGenerateEggs => false;

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups) => game switch
    {
        GO => EncounterGenerator8GO.Instance.GetPossible(pk, chain, game, groups),
        PLA => EncounterGenerator8a.Instance.GetPossible(pk, chain, game, groups),
        BD or SP => EncounterGenerator8b.Instance.GetPossible(pk, chain, game, groups),
        _ => EncounterGenerator8.Instance.GetPossible(pk, chain, game, groups),
    };

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 8);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info) => pk.Version switch
    {
        GO => EncounterGeneratorGO.Instance.GetEncounters(pk, chain, info),
        PLA      when pk is not PK8 => EncounterGenerator8a.Instance.GetEncounters(pk, chain, info),
        BD or SP when pk is not PK8 => EncounterGenerator8b.Instance.GetEncounters(pk, chain, info),
        SW when pk.MetLocation == LocationsHOME.SWLA => EncounterGenerator8a.Instance.GetEncounters(pk, chain, info),
        SW when pk.MetLocation == LocationsHOME.SWBD => EncounterGenerator8b.Instance.GetEncountersSWSH(pk, chain, BD),
        SH when pk.MetLocation == LocationsHOME.SHSP => EncounterGenerator8b.Instance.GetEncountersSWSH(pk, chain, SP),
        SW when pk.MetLocation == LocationsHOME.SWSL => EncounterGenerator9.Instance.GetEncountersSWSH(pk, chain, SL),
        SH when pk.MetLocation == LocationsHOME.SHVL => EncounterGenerator9.Instance.GetEncountersSWSH(pk, chain, VL),
        SW or SH => EncounterGenerator8.Instance.GetEncounters(pk, chain, info),
        _ => [],
    };
}
