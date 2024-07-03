using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public sealed class EncounterGenerator2 : IEncounterGenerator
{
    public static readonly EncounterGenerator2 Instance = new();
    public bool CanGenerateEggs => true;

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible2(chain, groups, game, pk);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        throw new ArgumentException("Generator does not support direct calls to this method.");
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, GameVersion game)
    {
        var chain = EncounterOrigin.GetOriginChain12(pk, game);
        if (chain.Length == 0)
            return [];
        return GetEncounters(pk, chain);
    }

    private static IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain)
    {
        var iterator = new EncounterEnumerator2(pk, chain);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    private const byte Generation = 2;
    private const EntityContext Context = EntityContext.Gen2;
    private const byte EggLevel = 5;

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

    public static bool TryGetEgg(ReadOnlySpan<EvoCriteria> chain, GameVersion version, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            return false;

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species)
            return false; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            return false;
        if (form != 0)
            return false; // Forms don't exist in Gen2, besides Unown (which can't breed). Nothing can form-change.
        // Sanity Check 3
        if (!PersonalTable.C.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, form, version);
        return true;
    }

    // Depending on the game it was hatched (GS vs C), met data will be present.
    // Since met data can't be used to infer which game it was created on, we yield both if possible.
    public static bool TryGetEggCrystal(PKM pk, EncounterEgg egg, [NotNullWhen(true)] out EncounterEgg? crystal)
    {
        if (!ParseSettings.AllowGen2Crystal(pk))
        {
            crystal = null;
            return false;
        }
        crystal = egg with { Version = GameVersion.C };
        return true;
    }
}
