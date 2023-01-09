using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

/// <summary>
/// Encounter Generator for <see cref="GameVersion.GP"/> &amp; <see cref="GameVersion.GE"/>
/// </summary>
public sealed class EncounterGenerator7GG : IEncounterGenerator
{
    public static readonly EncounterGenerator7GG Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (groups.HasFlag(Mystery))
        {
            var table = EncounterEvent.MGDB_G7GG;
            foreach (var enc in GetPossibleMystery(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            var table = game == GameVersion.GP ? Encounters7GG.StaticGP : Encounters7GG.StaticGE;
            foreach (var enc in GetPossibleStatic(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var table = game == GameVersion.GP ? Encounters7GG.SlotsGP : Encounters7GG.SlotsGE;
            foreach (var enc in GetPossibleSlot(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            var table = Encounters7GG.TradeGift_GG;
            foreach (var enc in GetPossibleTrade(chain, table))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleMystery(EvoCriteria[] chain, IReadOnlyList<WB7> table)
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleStatic(EvoCriteria[] chain, EncounterStatic7b[] table)
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleSlot(EvoCriteria[] chain, EncounterArea7b[] areas)
    {
        foreach (var area in areas)
        {
            foreach (var slot in area.Slots)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != slot.Species)
                        continue;
                    yield return slot;
                    break;
                }
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleTrade(EvoCriteria[] chain, EncounterTrade7b[] table)
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (enc.Species != evo.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        bool yielded = false;
        if (pk.FatefulEncounter)
        {
            foreach (var z in EncounterEvent.MGDB_G7GG)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != z.Species)
                        continue;

                    if (z.IsMatchExact(pk, evo))
                    {
                        yield return z;
                        yielded = true;
                    }
                    break;
                }
            }
            if (yielded)
                yield break;
        }

        var game = (GameVersion)pk.Version;
        IEncounterable? deferred = null;
        IEncounterable? partial = null;

        var encStatic = game == GameVersion.GP ? Encounters7GG.StaticGP : Encounters7GG.StaticGE;
        foreach (var z in encStatic)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != z.Species)
                    continue;
                if (!z.IsMatchExact(pk, evo))
                    break;

                var match = z.GetMatchRating(pk);
                switch (match)
                {
                    case Match: yield return z; yielded = true; break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
                break;
            }
        }
        if (yielded)
            yield break;

        if (CanBeWildEncounter(pk))
        {
            var location = pk.Met_Location;
            var areas = game == GameVersion.GP ? Encounters7GG.SlotsGP : Encounters7GG.SlotsGE;
            foreach (var area in areas)
            {
                if (!area.IsMatchLocation(location))
                    continue;

                foreach (var slot in area.Slots)
                {
                    foreach (var evo in chain)
                    {
                        if (evo.Species != slot.Species)
                            continue;

                        if (!slot.IsMatchExact(pk, evo))
                            break;

                        var match = slot.GetMatchRating(pk);
                        switch (match)
                        {
                            case Match: yield return slot; yielded = true; break;
                            case Deferred: deferred ??= slot; break;
                            case PartialMatch: partial ??= slot; break;
                        }
                        break;
                    }
                }
            }
            if (yielded)
                yield break;
        }

        foreach (var z in Encounters7GG.TradeGift_GG)
        {
            if (z.Version != GameVersion.GG && z.Version != game)
                continue;

            foreach (var evo in chain)
            {
                if (evo.Species != z.Species)
                    continue;
                if (!z.IsMatchExact(pk, evo))
                    break;

                var match = z.GetMatchRating(pk);
                switch (match)
                {
                    case Match: yield return z; break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
                break;
            }
        }

        if (deferred != null)
            yield return deferred;
        else if (partial != null)
            yield return partial;
    }
}
