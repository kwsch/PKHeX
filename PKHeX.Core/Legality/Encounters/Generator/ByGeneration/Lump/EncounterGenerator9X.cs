using System.Collections.Generic;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public sealed class EncounterGenerator9X : IEncounterGenerator
{
    public static readonly EncounterGenerator9X Instance = new();
    public bool CanGenerateEggs => false;

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups) => game switch
    {
        ZA => EncounterGenerator9a.Instance.GetPossible(pk, chain, game, groups),
        SL or VL => EncounterGenerator9.Instance.GetPossible(pk, chain, game, groups),
        _ => [],
    };

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 9, pk.Context);
        if (chain.Length == 0)
            return [];

        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info) => pk.Version switch
    {
        ZA => EncounterGenerator9a.Instance.GetEncounters(pk, chain, info),
        SL or VL => EncounterGenerator9.Instance.GetEncounters(pk, chain, info),
        0 when pk.IsEgg => EncounterGenerator9.Instance.GetEncounters(pk, chain, info), // 0 for eggs
        SW or SH => EncounterGenerator8X.Instance.GetEncounters(pk, chain, info), // Backwards transfers to SW/SH that are recognized as being from Gen9
        _ => [],
    };
}
