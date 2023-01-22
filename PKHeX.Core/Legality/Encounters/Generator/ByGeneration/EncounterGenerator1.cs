using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator1 : IEncounterGenerator
{
    public static readonly EncounterGenerator1 Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (groups.HasFlag(Mystery))
        {
            var table = GetGifts();
            foreach (var enc in GetPossibleGifts(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            var table = GetTrade(game);
            foreach (var enc in GetPossibleTrades(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            var table = GetStatic(game);
            foreach (var enc in GetPossibleStatic(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = GetAreas(game, pk.Japanese);
            foreach (var area in GetPossibleSlots(chain, areas))
                yield return area;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, EncounterStatic1E[] table)
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

    private static IEnumerable<IEncounterable> GetPossibleTrades(EvoCriteria[] chain, EncounterTrade1[] table)
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

    private static IEnumerable<IEncounterable> GetPossibleStatic(EvoCriteria[] chain, EncounterStatic1[] table)
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

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea1[] areas)
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

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        throw new ArgumentException("Generator does not support direct calls to this method.");
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, GameVersion game)
    {
        // Since encounter matching is super weak due to limited stored data in the structure
        // Calculate all 3 at the same time and pick the best result (by species).
        // Favor special event move gifts as Static Encounters when applicable
        var chain = EncounterOrigin.GetOriginChain12(pk, game);
        return GetEncounters(pk, chain, game);
    }

    private IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        IEncounterable? deferred = null;

        foreach (var enc in GetTrade(game))
        {
            if (!(enc.Version.Contains(game) || game.Contains(enc.Version)))
                continue;

            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match != Match)
                    deferred = enc;
                else
                    yield return enc;
                break;
            }
        }
        foreach (var enc in GetStatic(game))
        {
            if (!(enc.Version.Contains(game) || game.Contains(enc.Version)))
                continue;

            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (enc.IsMatchExact(pk, evo))
                    yield return enc;
                break;
            }
        }
        if (CanBeWildEncounter(pk))
        {
            foreach (var area in GetAreas(game, pk.Japanese))
            {
                var slots = area.GetMatchingSlots(pk, chain);
                foreach (var slot in slots)
                    yield return slot;
            }
        }
        foreach (var enc in GetGifts())
        {
            if (!(enc.Version.Contains(game) || game.Contains(enc.Version)))
                continue;

            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (enc.IsMatchExact(pk, evo))
                    yield return enc;
                break;
            }
        }

        if (deferred != null)
            yield return deferred;
    }

    private static EncounterStatic1E[] GetGifts()
    {
        if (!ParseSettings.AllowGBCartEra)
            return Encounters1.StaticEventsVC;
        return Encounters1.StaticEventsGB;
    }

    private static EncounterArea1[] GetAreas(GameVersion game, bool japanese) => game switch
    {
        GameVersion.RD => Encounters1.SlotsRD,
        GameVersion.GN => Encounters1.SlotsGN,
        GameVersion.BU => japanese ? Encounters1.SlotsBU : Array.Empty<EncounterArea1>(),
        GameVersion.YW => Encounters1.SlotsYW,

        _ when japanese => Encounters1.SlotsRGBY,
        _ => Encounters1.SlotsRBY,
    };

    private static EncounterStatic1[] GetStatic(GameVersion game) => game switch
    {
        _ => Encounters1.StaticRBY,
    };

    private static EncounterTrade1[] GetTrade(GameVersion game) => game switch
    {
        _ => Encounters1.TradeGift_RBY,
    };
}
