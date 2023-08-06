using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterTypeGroup;
using System.Diagnostics.CodeAnalysis;

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
            if (TryGetEgg(chain, game, out var egg))
            {
                yield return egg;
                yield return MutateEggTrade(egg);
                if (TryGetSplit(egg, chain, out var split))
                {
                    yield return split;
                    yield return MutateEggTrade(split);
                }
            }
        }
        if (groups.HasFlag(Static))
        {
            if (game is GameVersion.X or GameVersion.Y)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters6XY.Encounter_XY))
                    yield return enc;
                var table = game == GameVersion.X ? Encounters6XY.StaticX : Encounters6XY.StaticY;
                foreach (var enc in GetPossibleAll(chain, table))
                    yield return enc;
            }
            else if (game is GameVersion.AS or GameVersion.OR)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters6AO.Encounter_AO))
                    yield return enc;
                var table = game == GameVersion.AS ? Encounters6AO.StaticA : Encounters6AO.StaticO;
                foreach (var enc in GetPossibleAll(chain, table))
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
            foreach (var enc in GetPossibleAll(chain, table))
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

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 6);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var iterator = new EncounterEnumerator6(pk, chain, (GameVersion)pk.Version);
        foreach (var enc in iterator)
            yield return enc.Encounter;
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
    private const byte EggLevel = EggStateLegality.EggMetLevel;

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

    public static bool TryGetEgg(ReadOnlySpan<EvoCriteria> chain, GameVersion version, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            return false;

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            return false; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            return false;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            return false;
        // Sanity Check 3
        if (!PersonalTable.AO.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, form, version);
        return true;
    }

    public static EncounterEgg MutateEggTrade(EncounterEgg egg) => egg with { Version = GetOtherGamePair(egg.Version) };

    public static bool TryGetSplit(EncounterEgg other, ReadOnlySpan<EvoCriteria> chain, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        // Check for split-breed
        var devolved = chain[^1];
        if (other.Species == devolved.Species)
        {
            if (chain.Length < 2)
                return false; // no split-breed
            devolved = chain[^2];
        }
        if (!Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            return false;

        result = other with { Species = devolved.Species, Form = devolved.Form };
        return true;
    }
}
