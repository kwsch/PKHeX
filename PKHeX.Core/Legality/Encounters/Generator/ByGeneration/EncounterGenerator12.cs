using System.Collections.Generic;

using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterEggGenerator2;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

/// <summary>
/// This class is essentially a sub-class of <see cref="EncounterGenerator"/> specialized for Gen1 & Gen2 encounters.
/// </summary>
internal static class EncounterGenerator12
{
    internal static IEnumerable<IEncounterable> GetEncounters12(PKM pk, LegalInfo info)
    {
        foreach (var z in GenerateFilteredEncounters12(pk))
        {
            info.StoreMetadata(z.Generation);
            yield return z;
        }
    }

    private static IEnumerable<IEncounterable> GenerateRawEncounters12(PKM pk, GameVersion game)
    {
        // Since encounter matching is super weak due to limited stored data in the structure
        // Calculate all 3 at the same time and pick the best result (by species).
        // Favor special event move gifts as Static Encounters when applicable
        var chain = EncounterOrigin.GetOriginChain12(pk, game);

        IEncounterable? deferred = null;
        foreach (var t in GetValidEncounterTradesVC(pk, chain, game))
        {
            // Gen2 trades are strictly matched (OT/Nick), while Gen1 trades allow for deferral (shrug).
            if (t is EncounterTrade1 t1 && t1.GetMatchRating(pk) != Match)
            {
                deferred ??= t;
                continue;
            }
            yield return t;
        }
        foreach (var s in GetValidStaticEncounter(pk, chain, game))
        {
            yield return s;
        }
        foreach (var e in GetValidWildEncounters12(pk, chain, game))
        {
            yield return e;
        }
        if (game != GameVersion.RBY)
        {
            foreach (var e in GenerateEggs(pk, chain))
                yield return e;
        }

        foreach (var s in GenerateGBEvents(pk, chain, game))
        {
            yield return s;
        }

        if (deferred != null)
            yield return deferred;
    }

    private static IEnumerable<EncounterStatic> GenerateGBEvents(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        if (pk.Korean) // only GS; no events
            yield break;

        foreach (var e in GetValidGBGifts(pk, chain, game))
        {
            foreach (var evo in chain)
            {
                if (e.IsMatchExact(pk, evo))
                    yield return e;
            }
        }
    }

    private static IEnumerable<IEncounterable> GenerateFilteredEncounters12(PKM pk)
    {
        // If the current data indicates that it must have originated from Crystal, only yield encounter data from Crystal.
        bool crystal = (pk is ICaughtData2 {CaughtData: not 0}) || (pk.Format >= 7 && pk.OT_Gender == 1);
        if (crystal)
            return GenerateRawEncounters12(pk, GameVersion.C);

        var visited = GBRestrictions.GetTradebackStatusInitial(pk);
        switch (visited)
        {
            case PotentialGBOrigin.Gen1Only:
                return GenerateRawEncounters12(pk, GameVersion.RBY);
            case PotentialGBOrigin.Gen2Only:
                return GenerateRawEncounters12(pk, GameVersion.GSC);
            default:
                if (pk.Korean)
                    return GenerateFilteredEncounters12BothKorean(pk);
                return GenerateFilteredEncounters12Both(pk);
        }
    }

    private static IEnumerable<IEncounterable> GenerateFilteredEncounters12BothKorean(PKM pk)
    {
        // Korean origin PK1/PK2 can only originate from GS, but since we're nice we'll defer & yield matches from other games.
        // Yield GS first, then Crystal, then RBY. Anything other than GS will be flagged by later checks.

        var deferred = new List<IEncounterable>();
        var get2 = GenerateRawEncounters12(pk, GameVersion.GSC);
        foreach (var enc in get2)
        {
            if (enc.Version == GameVersion.C)
                deferred.Add(enc);
            else
                yield return enc;
        }

        foreach (var enc in deferred)
            yield return enc;

        var get1 = GenerateRawEncounters12(pk, GameVersion.RBY);
        foreach (var enc in get1)
            yield return enc;
    }

    private static IEnumerable<IEncounterable> GenerateFilteredEncounters12Both(PKM pk)
    {
        // Iterate over both games, consuming from one list at a time until the other list has higher priority encounters
        // Buffer the encounters so that we can consume each iterator separately
        var get1 = GenerateRawEncounters12(pk, GameVersion.RBY);
        var get2 = GenerateRawEncounters12(pk, GameVersion.GSC);
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
}
