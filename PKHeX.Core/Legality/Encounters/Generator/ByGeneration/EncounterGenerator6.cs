using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator6 : IEncounterGenerator
{
    public static readonly EncounterGenerator6 Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            var table = EncounterEvent.MGDB_G6;
            foreach (var enc in GetPossibleGifts(chain, table, game))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            var eggs = GetEggs(pk, chain, game);
            foreach (var enc in eggs)
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            if (game is GameVersion.X or GameVersion.Y)
            {
                foreach (var enc in GetPossible(chain, Encounters6XY.Encounter_XY))
                    yield return enc;
                var table = game == GameVersion.X ? Encounters6XY.StaticX : Encounters6XY.StaticY;
                foreach (var enc in GetPossible(chain, table))
                    yield return enc;
            }
            else if (game is GameVersion.AS or GameVersion.OR)
            {
                foreach (var enc in GetPossible(chain, Encounters6AO.Encounter_AO))
                    yield return enc;
                var table = game == GameVersion.AS ? Encounters6AO.StaticA : Encounters6AO.StaticO;
                foreach (var enc in GetPossible(chain, table))
                    yield return enc;
            }
        }
        if (groups.HasFlag(Slot))
        {
            if (game is GameVersion.X or GameVersion.Y)
            {
                var areas = game == GameVersion.X ? Encounters6XY.SlotsX : Encounters6XY.SlotsY;
                foreach (var enc in GetPossibleSlots<EncounterArea6XY, EncounterSlot6XY>(chain, areas))
                    yield return enc;
            }
            else if (game is GameVersion.AS or GameVersion.OR)
            {
                var areas = game == GameVersion.AS ? Encounters6AO.SlotsA : Encounters6AO.SlotsO;
                foreach (var enc in GetPossibleSlots<EncounterArea6AO, EncounterSlot6AO>(chain, areas))
                    yield return enc;
            }
        }
        if (groups.HasFlag(Trade))
        {
            var table = GetTrades(game);
            foreach (var enc in GetPossible(chain, table))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, IReadOnlyList<WC6> table, GameVersion game)
    {
        foreach (var enc in table)
        {
            if (!enc.CanBeReceivedByVersion((int)game))
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

    private static IEnumerable<TSlot> GetPossibleSlots<TArea, TSlot>(EvoCriteria[] chain, TArea[] areas) where TArea : IEncounterArea<TSlot> where TSlot : IEncounterable
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

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea6AO[] areas)
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

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 6);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        if (chain.Length == 0)
            yield break;
        var game = (GameVersion)pk.Version;

        IEncounterable? deferred = null;
        IEncounterable? partial = null;

        bool yielded = false;
        if (pk.FatefulEncounter || pk.Met_Location == Locations.LinkGift6)
        {
            foreach (var z in EncounterEvent.MGDB_G6)
            {
                foreach (var evo in chain)
                {
                    if (z.Species != evo.Species)
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
            if (!yielded)
            {
                if (deferred != null)
                {
                    yield return deferred;
                    yielded = true;
                }
                if (partial != null)
                {
                    yield return partial;
                    yielded = true;
                }
            }
            if (yielded)
                yield break;
        }

        if (Locations.IsEggLocationBred6(pk.Egg_Location))
        {
            var eggs = GetEggs(pk, chain, game);
            foreach (var egg in eggs)
            {
                yield return egg;
                yielded = true;
            }
            if (yielded)
                yield break;
        }

        if (game is GameVersion.X or GameVersion.Y)
        {
            foreach (var enc in Encounters6XY.Encounter_XY)
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
            var single = game == GameVersion.X ? Encounters6XY.StaticX : Encounters6XY.StaticY;
            foreach (var enc in single)
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
        }
        else
        {
            foreach (var enc in Encounters6AO.Encounter_AO)
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
            var single = game == GameVersion.AS ? Encounters6AO.StaticA : Encounters6AO.StaticO;
            foreach (var enc in single)
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
        }
        if (yielded)
            yield break;

        if (CanBeWildEncounter(pk))
        {
            var location = pk.Met_Location;
            if (game is GameVersion.X or GameVersion.Y)
            {
                var areas = game == GameVersion.X ? Encounters6XY.SlotsX : Encounters6XY.SlotsY;
                foreach (var area in areas)
                {
                    if (!area.IsMatchLocation(location))
                        continue;

                    var slots = area.GetMatchingSlots(pk, chain);
                    foreach (var slot in slots)
                    {
                        var match = slot.GetMatchRating(pk);
                        switch (match)
                        {
                            case Match: yield return slot; yielded = true; break;
                            case Deferred: deferred ??= slot; break;
                            case PartialMatch: partial ??= slot; break;
                        }
                    }
                }
            }
            else if (game is GameVersion.AS or GameVersion.OR)
            {
                var areas = game == GameVersion.AS ? Encounters6AO.SlotsA : Encounters6AO.SlotsO;
                foreach (var area in areas)
                {
                    if (!area.IsMatchLocation(location))
                        continue;

                    var slots = area.GetMatchingSlots(pk, chain);
                    foreach (var slot in slots)
                    {
                        var match = slot.GetMatchRating(pk);
                        switch (match)
                        {
                            case Match: yield return slot; yielded = true; break;
                            case Deferred: deferred ??= slot; break;
                            case PartialMatch: partial ??= slot; break;
                        }
                    }
                }
            }
            if (yielded)
                yield break;
        }

        var trades = GetTrades(game);
        foreach (var enc in trades)
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

        if (deferred != null)
            yield return deferred;
        if (partial != null)
            yield return partial;
    }

    private static EncounterTrade6[] GetTrades(GameVersion gameSource) => gameSource switch
    {
        GameVersion.X => Encounters6XY.TradeGift_XY,
        GameVersion.Y => Encounters6XY.TradeGift_XY,
        GameVersion.AS => Encounters6AO.TradeGift_AO,
        GameVersion.OR => Encounters6AO.TradeGift_AO,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private const int Generation = 6;
    private const EntityContext Context = EntityContext.Gen6;
    private const byte EggLevel = 1;

    private static IEnumerable<EncounterEgg> GetEggs(PKM pk, EvoCriteria[] chain, GameVersion version)
    {
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            yield break;

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            yield break; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            yield break;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            yield break;
        // Sanity Check 3
        if (!PersonalTable.AO.IsPresentInGame(species, form))
            yield break;

        var egg = CreateEggEncounter(species, form, version);
        yield return egg;
        if (pk.IsEgg)
            yield break;
        bool otherVersion = pk is { Egg_Location: Locations.LinkTrade6 };
        if (otherVersion)
            yield return egg with { Version = GetOtherGamePair(version) };

        // Check for split-breed
        if (species == devolved.Species)
        {
            if (chain.Length < 2)
                yield break; // no split-breed
            devolved = chain[^2];
        }
        if (!Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            yield break;

        species = devolved.Species;
        form = devolved.Form;
        egg = CreateEggEncounter(species, form, version);
        yield return egg;
        if (otherVersion)
            yield return egg with { Version = GetOtherGamePair(version) };
    }

    private static GameVersion GetOtherGamePair(GameVersion version)
    {
        // 24 -> 26 ( X -> AS)
        // 25 -> 27 ( Y -> OR)
        // 26 -> 24 (AS ->  X)
        // 27 -> 25 (OR ->  Y)
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        return version ^ (GameVersion)2;
    }

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Rotom or (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves6.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }
}
