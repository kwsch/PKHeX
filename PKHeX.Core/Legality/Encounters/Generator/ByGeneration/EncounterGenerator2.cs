using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator2 : IEncounterGenerator
{
    public static readonly EncounterGenerator2 Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        bool korean = pk.Korean;
        if (groups.HasFlag(Mystery))
        {
            var table = GetGifts(korean);
            foreach (var enc in GetPossibleGifts(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            var table = GetTrade(game);
            foreach (var enc in GetPossibleTrades(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            var eggs = GetEggs(pk, chain);
            foreach (var egg in eggs)
                yield return egg;
        }
        if (groups.HasFlag(Static))
        {
            var table = GetStatic(game, korean);
            foreach (var enc in GetPossibleStatic(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = GetAreas(game, korean);
            foreach (var enc in GetPossibleSlots(chain, areas, pk))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, EncounterStatic2E[] table)
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

    private static IEnumerable<IEncounterable> GetPossibleTrades(EvoCriteria[] chain, EncounterTrade2[] table)
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

    private static IEnumerable<IEncounterable> GetPossibleStatic(EvoCriteria[] chain, EncounterStatic2[] table)
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

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea2[] areas, ITrainerID16 pk)
    {
        foreach (var area in areas)
        {
            foreach (var slot in area.Slots)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != slot.Species)
                        continue;

                    if (slot.IsHeadbutt && !slot.IsTreeAvailable(pk.TID16))
                        break;
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
        var chain = EncounterOrigin.GetOriginChain12(pk, game);
        return GetEncounters(pk, chain, game);
    }

    private IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        // Since encounter matching is super weak due to limited stored data in the structure
        // Calculate all 3 at the same time and pick the best result (by species).
        // Favor special event move gifts as Static Encounters when applicable
        IEncounterable? deferred = null;

        bool korean = pk.Korean;
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
        foreach (var enc in GetStatic(game, korean))
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
            foreach (var area in GetAreas(game, korean))
            {
                var slots = area.GetMatchingSlots(pk, chain);
                foreach (var slot in slots)
                    yield return slot;
            }
        }
        if (GetCanBeEgg(pk))
        {
            foreach (var e in GetEggs(pk, chain))
               yield return e;
        }
        foreach (var enc in GetGifts(korean))
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

    private static EncounterStatic2E[] GetGifts(bool korean)
    {
        if (korean)
            return Array.Empty<EncounterStatic2E>();
        if (!ParseSettings.AllowGBCartEra)
            return Encounters2.StaticEventsVC;
        return Encounters2.StaticEventsGB;
    }

    private static EncounterArea2[] GetAreas(GameVersion game, bool korean) => game switch
    {
        GameVersion.GD => Encounters2.SlotsGD,
        GameVersion.SI => Encounters2.SlotsSV,
        GameVersion.C => !korean ? Encounters2.SlotsC : Array.Empty<EncounterArea2>(),
        GameVersion.GS => Encounters2.SlotsGS,
        GameVersion.GSC => !korean ? Encounters2.SlotsGSC : Encounters2.SlotsGS,
        _ => throw new ArgumentOutOfRangeException(nameof(game), game, null),
    };

    private static EncounterStatic2[] GetStatic(GameVersion game, bool korean) => game switch
    {
        GameVersion.GD => Encounters2.StaticGS,
        GameVersion.SI => Encounters2.StaticGS,
        GameVersion.C => !korean ? Encounters2.StaticC : Array.Empty<EncounterStatic2>(),
        GameVersion.GS => Encounters2.StaticGS,
        GameVersion.GSC => !korean ? Encounters2.StaticGSC : Encounters2.StaticGS,
        _ => throw new ArgumentOutOfRangeException(nameof(game), game, null),
    };

    private static EncounterTrade2[] GetTrade(GameVersion game) => game switch
    {
        _ => Encounters2.TradeGift_GSC,
    };

    private const int Generation = 2;
    private const EntityContext Context = EntityContext.Gen2;
    private const byte EggLevel = 5;

    private static IEnumerable<EncounterEgg> GetEggs(PKM pk, EvoCriteria[] chain)
    {
        var devolved = chain[^1];
        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species)
            yield break; // no split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            yield break;
        // Sanity Check 3
        if (!PersonalTable.C.IsPresentInGame(species, form))
            yield break;

        // Gen2 was before split-breed species existed; try to ensure that the egg we try and match to can actually originate in the game.
        // Species must be < 251
        // Form must be 0 (Unown cannot breed).
        if (form != 0)
            yield break; // Forms don't exist in Gen2, besides Unown (which can't breed). Nothing can form-change.

        // Depending on the game it was hatched (GS vs C), met data will be present.
        // Since met data can't be used to infer which game it was created on, we yield both if possible.
        var egg = CreateEggEncounter(species, 0, GameVersion.GS);
        yield return egg;
        if (ParseSettings.AllowGen2Crystal(pk))
            yield return egg with { Version = GameVersion.C };
    }

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation))
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves2.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }

    private static bool GetCanBeEgg(PKM pk)
    {
        if (pk.Format == 1 && !ParseSettings.AllowGen1Tradeback)
            return false;

        if (pk.CurrentLevel < EggLevel)
            return false;

        var format = pk.Format;
        if (pk.IsEgg)
            return format == 2;

        var metLevel = pk.Met_Level;
        if (format > 2)
            return metLevel >= EggLevel;

        // 2->1->2 clears met info
        return metLevel switch
        {
            0 => pk.Met_Location == 0,
            1 => true, // Met location of 0 is valid -- second floor of every Pokémon Center
            _ => false,
        };
    }
}
