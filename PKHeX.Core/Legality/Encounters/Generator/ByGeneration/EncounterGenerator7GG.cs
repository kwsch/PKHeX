using System.Collections.Generic;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterTypeGroup;

namespace PKHeX.Core;

/// <summary>
/// Encounter Generator for <see cref="GameVersion.GP"/> &amp; <see cref="GameVersion.GE"/>
/// </summary>
public sealed class EncounterGenerator7GG : IEncounterGenerator
{
    public static readonly EncounterGenerator7GG Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            foreach (var enc in GetPossibleAll(chain, EncounterEvent.MGDB_G7GG))
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            foreach (var enc in GetPossibleAll(chain, Encounters7GG.Encounter_GG))
                yield return enc;
            var table = game == GameVersion.GP ? Encounters7GG.StaticGP : Encounters7GG.StaticGE;
            foreach (var enc in GetPossibleAll(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var table = game == GameVersion.GP ? Encounters7GG.SlotsGP : Encounters7GG.SlotsGE;
            foreach (var enc in GetPossibleSlots<EncounterArea7b, EncounterSlot7b>(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            foreach (var enc in GetPossibleAll(chain, Encounters7GG.TradeGift_GG))
                yield return enc;
            var specific = game == GameVersion.GP ? Encounters7GG.TradeGift_GP : Encounters7GG.TradeGift_GE;
            foreach (var enc in GetPossibleAll(chain, specific))
                yield return enc;
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var iterator = new EncounterEnumerator7GG(pk, chain, (GameVersion)pk.Version);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }
}
