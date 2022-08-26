using System.Collections.Generic;

using static PKHeX.Core.Legal;

namespace PKHeX.Core;

public static class EncounterEggGenerator
{
    public static IEnumerable<EncounterEgg> GenerateEggs(PKM pk, int generation, bool all = false)
    {
        var table = EvolutionTree.GetEvolutionTree(pk.Context);
        int maxSpeciesOrigin = GetMaxSpeciesOrigin(generation);
        var evos = table.GetValidPreEvolutions(pk, levelMax: 100, maxSpeciesOrigin: maxSpeciesOrigin, skipChecks: true);
        return GenerateEggs(pk, evos, generation, all);
    }

    public static IEnumerable<EncounterEgg> GenerateEggs(PKM pk, EvoCriteria[] chain, int generation, bool all = false)
    {
        System.Diagnostics.Debug.Assert(generation >= 3); // if generating Gen2 eggs, use the other generator.
        var currentSpecies = pk.Species;
        if (!Breeding.CanHatchAsEgg(currentSpecies))
            yield break;

        var currentForm = pk.Form;
        if (!Breeding.CanHatchAsEgg(currentSpecies, currentForm, generation))
            yield break; // can't originate from eggs

        // version is a true indicator for all generation 3-5 origins
        var ver = (GameVersion)pk.Version;
        if (!Breeding.CanGameGenerateEggs(ver))
            yield break;

        var context = ver.GetContext();
        var lvl = EggStateLegality.GetEggLevel(generation);
        int max = GetMaxSpeciesOrigin(generation);

        var (species, form) = GetBaseSpecies(chain, 0);
        if (species != 0 && species <= max)
        {
            // NOTE: THE SPLIT-BREED SECTION OF CODE SHOULD BE EXACTLY THE SAME AS THE BELOW SECTION
            if (FormInfo.IsBattleOnlyForm(species, form, generation))
                form = FormInfo.GetOutOfBattleForm(species, form, generation);
            if (Breeding.CanHatchAsEgg(species, form, ver))
            {
                yield return new EncounterEgg(species, form, lvl, generation, ver, context);
                if (generation > 5 && (pk.WasTradedEgg || all) && HasOtherGamePair(ver))
                    yield return new EncounterEgg(species, form, lvl, generation, GetOtherTradePair(ver), context);
            }
        }

        if (!Breeding.GetSplitBreedGeneration(generation).Contains(currentSpecies))
            yield break; // no other possible species

        var otherSplit = species;
        (species, form) = GetBaseSpecies(chain, 1);
        if ((uint)species == otherSplit)
            yield break;

        if (species <= max)
        {
            // NOTE: THIS SECTION OF CODE SHOULD BE EXACTLY THE SAME AS THE ABOVE SECTION
            if (FormInfo.IsBattleOnlyForm(species, form, generation))
                form = FormInfo.GetOutOfBattleForm(species, form, generation);
            if (Breeding.CanHatchAsEgg(species, form, ver))
            {
                yield return new EncounterEgg(species, form, lvl, generation, ver, context);
                if (generation > 5 && (pk.WasTradedEgg || all) && HasOtherGamePair(ver))
                    yield return new EncounterEgg(species, form, lvl, generation, GetOtherTradePair(ver), context);
            }
        }
    }

    // Gen6+ update the origin game when hatched. Quick manip for X.Y<->A.O | S.M<->US.UM, ie X->A
    private static GameVersion GetOtherTradePair(GameVersion ver) => ver switch
    {
        <= GameVersion.OR => (GameVersion) ((int) ver ^ 2), // gen6
        <= GameVersion.MN => ver + 2, // gen7
        _ => ver - 2,
    };

    private static bool HasOtherGamePair(GameVersion ver)
    {
        return ver < GameVersion.GP; // lgpe and sw/sh don't have a sister pair
    }

    private static (ushort Species, byte Form) GetBaseSpecies(EvoCriteria[] evolutions, int skipOption)
    {
        ushort species = evolutions[0].Species;
        if (species == (int)Species.Shedinja) // Shedinja
            return ((int)Species.Nincada, 0); // Nincada

        // skip n from end, return empty if invalid index
        int index = evolutions.Length - 1 - skipOption;
        if ((uint)index >= evolutions.Length)
            return default;
        var evo = evolutions[index];
        return (evo.Species, evo.Form);
    }
}
