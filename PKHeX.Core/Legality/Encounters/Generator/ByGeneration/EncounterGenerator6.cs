using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public sealed class EncounterGenerator6 : IEncounterGenerator
{
    public static readonly EncounterGenerator6 Instance = new();
    public bool CanGenerateEggs => true;

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible6(chain, groups, game);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 6);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var iterator = new EncounterEnumerator6(pk, chain, pk.Version);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    private const byte Generation = 6;
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
