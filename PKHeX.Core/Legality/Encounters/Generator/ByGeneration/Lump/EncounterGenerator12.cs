using System.Collections.Generic;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

/// <summary>
/// Dual-generator implementation to yield from multiple sets of games at the same time.
/// </summary>
public sealed class EncounterGenerator12 : IEncounterGenerator
{
    public static readonly EncounterGenerator12 Instance = new();
    public bool CanGenerateEggs => false;

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        foreach (var z in GetEncounters(pk))
        {
            info.StoreMetadata(z.Generation);
            yield return z;
        }
    }

    private static IEnumerable<IEncounterable> GetEncounters(PKM pk)
    {
        // If the current data indicates that it must have originated from Crystal, only yield encounter data from Crystal.
        bool crystal = pk is ICaughtData2 { CaughtData: not 0 } or { Format: >= 7, OriginalTrainerGender: 1 };
        if (crystal)
            return EncounterGenerator2.Instance.GetEncounters(pk, GameVersion.C);

        var visited = GBRestrictions.GetTradebackStatusInitial(pk);
        return visited switch
        {
            PotentialGBOrigin.Gen1Only => EncounterGenerator1.Instance.GetEncounters(pk, GameVersion.RBY),
            PotentialGBOrigin.Gen2Only => EncounterGenerator2.Instance.GetEncounters(pk, GameVersion.GSC),
            _ when pk.Korean => GenerateFilteredEncounters12BothKorean(pk),
            _ => GenerateFilteredEncounters12Both(pk),
        };
    }

    private static IEnumerable<IEncounterable> GenerateFilteredEncounters12BothKorean(PKM pk)
    {
        // Korean origin PK1/PK2 can only originate from GS, but since we're nice we'll defer & yield matches from other games.
        // Yield GS first, then Crystal, then RBY. Anything other than GS will be flagged by later checks.

        var deferred = new List<IEncounterable>();
        var get2 = EncounterGenerator2.Instance.GetEncounters(pk, GameVersion.GSC);
        foreach (var enc in get2)
        {
            if (enc.Version == GameVersion.C)
                deferred.Add(enc);
            else
                yield return enc;
        }

        foreach (var enc in deferred)
            yield return enc;

        var get1 = EncounterGenerator1.Instance.GetEncounters(pk, GameVersion.RBY);
        foreach (var enc in get1)
            yield return enc;
    }

    private static IEnumerable<IEncounterable> GenerateFilteredEncounters12Both(PKM pk)
    {
        // Iterate over both games, consuming from one list at a time until the other list has higher priority encounters
        // Buffer the encounters so that we can consume each iterator separately
        var get1 = EncounterGenerator1.Instance.GetEncounters(pk, GameVersion.RBY);
        var get2 = EncounterGenerator2.Instance.GetEncounters(pk, GameVersion.GSC);
        using var g1i = new PeekEnumerator<IEncounterable>(get1);
        using var g2i = new PeekEnumerator<IEncounterable>(get2);
        while (g2i.PeekIsNext() || g1i.PeekIsNext())
        {
            var iter = PickPreferredIterator(pk, g1i, g2i);
            yield return iter.Current;
            iter.MoveNext();
        }
    }

    private static PeekEnumerator<IEncounterable> PickPreferredIterator(PKM pk, PeekEnumerator<IEncounterable> g1i, PeekEnumerator<IEncounterable> g2i)
    {
        if (!g1i.PeekIsNext())
            return g2i;
        if (!g2i.PeekIsNext())
            return g1i;
        var p1 = GetGBEncounterPriority(pk, g1i.Current);
        var p2 = GetGBEncounterPriority(pk, g2i.Current);
        return p1 > p2 ? g1i : g2i;
    }

    private static GBEncounterPriority GetGBEncounterPriority(PKM pk, IEncounterTemplate enc) => enc switch
    {
        EncounterTrade1 t1 when t1.GetMatchRating(pk) != Match => GBEncounterPriority.Least,
        EncounterTrade1 => GBEncounterPriority.TradeEncounterG1,
        EncounterTrade2 => GBEncounterPriority.TradeEncounterG2,
        EncounterSlot1 or EncounterSlot2 => GBEncounterPriority.WildEncounter,
        EncounterEgg => GBEncounterPriority.EggEncounter,
        _ => GBEncounterPriority.StaticEncounter,
    };

    /// <summary>
    /// Generation 1/2 Encounter Data type, which serves as a 'best match' priority rating when returning from a list.
    /// </summary>
    private enum GBEncounterPriority
    {
        Least,
        EggEncounter,
        WildEncounter,
        StaticEncounter,
        TradeEncounterG1,
        TradeEncounterG2,
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        // Don't call this method.
        return GetEncounters(pk, info);
    }

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        // Don't call this method.
        if (game.GetGeneration() == 1)
            return EncounterGenerator1.Instance.GetPossible(pk, chain, game, groups);
        return EncounterGenerator2.Instance.GetPossible(pk, chain, game, groups);
    }
}
