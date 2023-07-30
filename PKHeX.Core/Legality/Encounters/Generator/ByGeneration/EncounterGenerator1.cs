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
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            var table = GetGifts();
            foreach (var enc in GetPossible(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            var table = GetTrade(game);
            foreach (var enc in GetPossible(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            var table = Encounters1.StaticRBY;
            foreach (var enc in GetPossible(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = GetAreas(game, pk.Japanese);
            foreach (var area in GetPossibleSlots<EncounterArea1, EncounterSlot1>(chain, areas))
                yield return area;
        }
    }

    private static IEnumerable<T> GetPossible<T>(EvoCriteria[] chain, T[] table) where T : IEncounterTemplate
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

    private static IEnumerable<TSlot> GetPossibleSlots<TArea,TSlot>(EvoCriteria[] chain, TArea[] areas) where TArea : IEncounterArea<TSlot> where TSlot : IEncounterTemplate
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
        if (chain.Length == 0)
            return Array.Empty<IEncounterable>();
        return GetEncounters(pk, chain, game);
    }

    private static IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, GameVersion game)
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
        foreach (var enc in Encounters1.StaticRBY)
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

    private static EncounterGift1[] GetGifts()
    {
        if (!ParseSettings.AllowGBVirtualConsole3DS)
            return Encounters1GBEra.Gifts;
        return Encounters1VC.Gifts;
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
    
    private static EncounterTrade1[] GetTrade(GameVersion game) => game switch
    {
        _ => Encounters1.TradeGift_RBY,
    };
}
