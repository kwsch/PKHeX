using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterTypeGroup;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public sealed class EncounterGenerator5 : IEncounterGenerator
{
    public static readonly EncounterGenerator5 Instance = new();

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 5);
        if (chain.Length == 0)
            return Array.Empty<IEncounterable>();
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            var table = EncounterEvent.MGDB_G5;
            foreach (var enc in GetPossibleGifts(chain, table, game))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            if (TryGetEgg(chain, game, out var egg))
            {
                yield return egg;
                if (TryGetSplit(egg, chain, out var split))
                    yield return split;
            }
        }
        if (groups.HasFlag(Static))
        {
            if (game is GameVersion.B or GameVersion.W)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters5DR.DreamWorld_Common))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters5BW.DreamWorld_BW))
                    yield return enc;

                foreach (var enc in GetPossibleAll(chain, Encounters5BW.Encounter_BW))
                    yield return enc;
                var specific = game == GameVersion.B ? Encounters5BW.StaticB : Encounters5BW.StaticW;
                foreach (var enc in GetPossibleAll(chain, specific))
                    yield return enc;
            }
            else
            {
                foreach (var enc in GetPossibleAll(chain, Encounters5DR.DreamWorld_Common))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters5B2W2.DreamWorld_B2W2))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters5DR.Encounter_DreamRadar))
                    yield return enc;

                foreach (var enc in GetPossibleAll(chain, Encounters5B2W2.Encounter_B2W2_Regular))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters5B2W2.Encounter_B2W2_N))
                    yield return enc;
                var specific = game == GameVersion.B2 ? Encounters5B2W2.StaticB2 : Encounters5B2W2.StaticW2;
                foreach (var enc in GetPossibleAll(chain, specific))
                    yield return enc;
            }
        }
        if (groups.HasFlag(Slot))
        {
            var areas = GetAreas(game);
            foreach (var enc in GetPossibleSlots<EncounterArea5, EncounterSlot5>(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            if (game is GameVersion.B or GameVersion.W)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters5BW.TradeGift_BW))
                    yield return enc;
                var specific = game == GameVersion.B ? Encounters5BW.TradeGift_B : Encounters5BW.TradeGift_W;
                foreach (var enc in GetPossibleAll(chain, specific))
                    yield return enc;
            }
            else
            {
                foreach (var enc in GetPossibleAll(chain, Encounters5B2W2.TradeGift_B2W2))
                    yield return enc;
                var specific = game == GameVersion.B2 ? Encounters5B2W2.TradeGift_B2 : Encounters5B2W2.TradeGift_W2;
                foreach (var enc in GetPossibleAll(chain, specific))
                    yield return enc;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, IReadOnlyList<PGF> table, GameVersion game)
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

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var iterator = new EncounterEnumerator5(pk, chain, (GameVersion)pk.Version);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    private static EncounterArea5[] GetAreas(GameVersion gameSource) => gameSource switch
    {
        GameVersion.B => Encounters5BW.SlotsB,
        GameVersion.W => Encounters5BW.SlotsW,
        GameVersion.B2 => Encounters5B2W2.SlotsB2,
        GameVersion.W2 => Encounters5B2W2.SlotsW2,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private const int Generation = 5;
    private const EntityContext Context = EntityContext.Gen5;
    private const byte EggLevel = EggStateLegality.EggMetLevel;

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
        if (!PersonalTable.B2W2.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, form, version);
        return true;
    }

    // Both B/W and B2/W2 have the same egg move sets, so there is no point generating other-game pair encounters for traded eggs.
    // When hatched, the entity's Version is updated to the OT's.

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
