using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterTypeGroup;

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
            foreach (var enc in GetPossibleAll(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            if (game is GameVersion.RD or GameVersion.GN)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters1.TradeGift_RB))
                    yield return enc;
            }
            else if (game is GameVersion.BU)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters1.TradeGift_BU))
                    yield return enc;
            }
            else if (game is GameVersion.YW)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters1.TradeGift_YW))
                    yield return enc;
            }
            else
            {
                foreach (var enc in GetPossibleAll(chain, Encounters1.TradeGift_RB))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters1.TradeGift_YW))
                    yield return enc;
                if (pk.Japanese)
                {
                    foreach (var enc in GetPossibleAll(chain, Encounters1.TradeGift_BU))
                        yield return enc;
                }
            }
        }
        if (groups.HasFlag(Static))
        {
            foreach (var enc in GetPossibleAll(chain, Encounters1.StaticRBY))
                yield return enc;
            if (game is GameVersion.RD or GameVersion.GN)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters1.StaticRB))
                    yield return enc;
            }
            else if (game is GameVersion.BU)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters1.StaticBU))
                    yield return enc;
            }
            else if (game is GameVersion.YW)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters1.StaticYW))
                    yield return enc;
            }
            else
            {
                foreach (var enc in GetPossibleAll(chain, Encounters1.StaticRB))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters1.StaticYW))
                    yield return enc;
                if (pk.Japanese)
                {
                    foreach (var enc in GetPossibleAll(chain, Encounters1.StaticBU))
                        yield return enc;
                }
            }
        }
        if (groups.HasFlag(Slot))
        {
            if (pk.Japanese)
            {
                foreach (var enc in GetPossibleSlots<EncounterArea1, EncounterSlot1>(chain, Encounters1.SlotsBU))
                    yield return enc;
            }
            foreach (var enc in GetPossibleSlots<EncounterArea1, EncounterSlot1>(chain, Encounters1.SlotsRD))
                yield return enc;
            foreach (var enc in GetPossibleSlots<EncounterArea1, EncounterSlot1>(chain, Encounters1.SlotsGN))
                yield return enc;
            foreach (var enc in GetPossibleSlots<EncounterArea1, EncounterSlot1>(chain, Encounters1.SlotsYW))
                yield return enc;
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
        return GetEncounters(pk, chain);
    }

    private static IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain)
    {
        var iterator = new EncounterEnumerator1(pk, chain);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    private static EncounterGift1[] GetGifts()
    {
        if (!ParseSettings.AllowGBVirtualConsole3DS)
            return Encounters1GBEra.Gifts;
        return Encounters1VC.Gifts;
    }
}
