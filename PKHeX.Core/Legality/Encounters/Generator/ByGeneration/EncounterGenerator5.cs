using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator5 : IEncounterGenerator
{
    public static readonly EncounterGenerator5 Instance = new();

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (groups.HasFlag(Mystery))
        {
            var table = EncounterEvent.MGDB_G5;
            foreach (var enc in GetPossibleGifts(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            var eggs = GetEggs(chain, game);
            foreach (var enc in eggs)
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
            var areas = GetAreas(game);
            foreach (var enc in GetPossibleSlots(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            var table = GetTrades(game);
            foreach (var enc in GetPossibleTrades(chain, table, game))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, IReadOnlyList<PGF> table)
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

    private static IEnumerable<IEncounterable> GetPossibleStatic(EvoCriteria[] chain, EncounterStatic5[] table)
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

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea5[] areas)
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

    private static IEnumerable<IEncounterable> GetPossibleTrades(EvoCriteria[] chain, EncounterTrade[] table, GameVersion game)
    {
        foreach (var enc in table)
        {
            if (enc.Version < GameVersion.BW && enc.Version != game)
                continue;
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var game = (GameVersion)pk.Version;

        bool yielded = false;
        if (pk.FatefulEncounter)
        {
            foreach (var mg in EncounterEvent.MGDB_G5)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != mg.Species)
                        continue;

                    if (mg.IsMatchExact(pk, evo))
                    {
                        yield return mg;
                        yielded = true;
                    }
                    break;
                }
            }
            if (yielded)
                yield break;
        }

        if (Locations.IsEggLocationBred5(pk.Egg_Location))
        {
            var eggs = GetEggs(chain, game);
            foreach (var egg in eggs)
            {
                yield return egg;
                yielded = true;
            }

            if (yielded)
                yield break;
        }

        IEncounterable? deferred = null;
        IEncounterable? partial = null;

        var encStatic = GetStatic(game);
        foreach (var enc in encStatic)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;

                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                switch (match)
                {
                    case Match: yield return enc; yielded = true; break;
                    case Deferred: deferred ??= enc; break;
                    case PartialMatch: partial ??= enc; break;
                }
                break;
            }
        }
        if (yielded)
            yield break;

        if (CanBeWildEncounter(pk))
        {
            var location = pk.Met_Location;
            var areas = GetAreas(game);
            foreach (var area in areas)
            {
                if (!area.IsMatchLocation(location))
                    continue;

                var slots = area.GetMatchingSlots(pk, chain);
                foreach (var enc in slots)
                {
                    var match = enc.GetMatchRating(pk);
                    switch (match)
                    {
                        case Match: yield return enc; yielded = true; break;
                        case Deferred: deferred ??= enc; break;
                        case PartialMatch: partial ??= enc; break;
                    }
                    break;
                }
            }
            if (yielded)
                yield break;
        }

        var trades = GetTrades(game);
        foreach (var trade in trades)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != trade.Species)
                    continue;
                if (!trade.IsMatchExact(pk, evo))
                    break;

                var match = trade.GetMatchRating(pk);
                switch (match)
                {
                    case Match: yield return trade; break;
                    case Deferred: deferred ??= trade; break;
                    case PartialMatch: partial ??= trade; break;
                }
                break;
            }
        }
        if (deferred != null)
            yield return deferred;
        if (partial != null)
            yield return partial;
    }

    private static EncounterStatic5[] GetStatic(GameVersion gameSource) => gameSource switch
    {
        GameVersion.B => Encounters5BW.StaticB,
        GameVersion.W => Encounters5BW.StaticW,
        GameVersion.B2 => Encounters5B2W2.StaticB2,
        GameVersion.W2 => Encounters5B2W2.StaticW2,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private static EncounterArea5[] GetAreas(GameVersion gameSource) => gameSource switch
    {
        GameVersion.B => Encounters5BW.SlotsB,
        GameVersion.W => Encounters5BW.SlotsW,
        GameVersion.B2 => Encounters5B2W2.SlotsB2,
        GameVersion.W2 => Encounters5B2W2.SlotsW2,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private static EncounterTrade[] GetTrades(GameVersion gameSource) => gameSource switch
    {
        GameVersion.B => Encounters5BW.TradeGift_BW,
        GameVersion.W => Encounters5BW.TradeGift_BW,
        GameVersion.B2 => Encounters5B2W2.TradeGift_B2W2,
        GameVersion.W2 => Encounters5B2W2.TradeGift_B2W2,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private const int Generation = 5;
    private const EntityContext Context = EntityContext.Gen5;
    private const byte EggLevel = 1;

    private static IEnumerable<EncounterEgg> GetEggs(EvoCriteria[] chain, GameVersion version)
    {
        var devolved = chain[^1];

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !IsValidBabySpecies(devolved.Species))
            yield break; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            yield break;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            yield break;
        // Sanity Check 3
        if (!PersonalTable.B2W2.IsPresentInGame(species, form))
            yield break;

        yield return CreateEggEncounter(species, form, version);
        // Both B/W and B2/W2 have the same egg move sets, so there is no point generating other-game pair encounters for traded eggs.
        // When hatched, the entity's Version is updated to the OT's.

        // Check for split-breed
        if (species == devolved.Species)
        {
            if (chain.Length < 2)
                yield break; // no split-breed
            devolved = chain[^2];
        }
        var splitSet = Breeding.GetSplitBreedGeneration(Generation);
        if (splitSet is null)
            yield break; // Shouldn't happen.
        if (!splitSet.Contains(devolved.Species))
            yield break;

        species = devolved.Species;
        form = devolved.Form;
        yield return CreateEggEncounter(species, form, version);
    }

    private static bool IsValidBabySpecies(ushort species)
    {
        var split = Breeding.GetSplitBreedGeneration(Generation);
        return split is not null && split.Contains(species);
    }

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Rotom or (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves5.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }
}
