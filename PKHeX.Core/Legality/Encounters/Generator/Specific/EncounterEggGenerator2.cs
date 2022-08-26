using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Specialized Egg Generator for Gen2
/// </summary>
internal static class EncounterEggGenerator2
{
    public static IEnumerable<EncounterEgg> GenerateEggs(PKM pk, bool all = false)
    {
        var table = EvolutionTree.Evolves2;
        const int maxSpeciesOrigin = Legal.MaxSpeciesID_2;
        var evos = table.GetValidPreEvolutions(pk, levelMax: 100, maxSpeciesOrigin: maxSpeciesOrigin, skipChecks: true);
        return GenerateEggs(pk, evos, all);
    }

    public static IEnumerable<EncounterEgg> GenerateEggs(PKM pk, EvoCriteria[] chain, bool all = false)
    {
        var species = pk.Species;
        if (!Breeding.CanHatchAsEgg(species))
            yield break;

        var canBeEgg = all || GetCanBeEgg(pk);
        if (!canBeEgg)
            yield break;

        // Gen2 was before split-breed species existed; try to ensure that the egg we try and match to can actually originate in the game.
        // Species must be < 251
        // Form must be 0 (Unown cannot breed).
        var baseID = chain[^1];
        if ((baseID.Species >= Legal.MaxSpeciesID_2 || baseID.Form != 0) && chain.Length != 1)
            baseID = chain[^2];
        if (baseID.Form != 0)
            yield break; // Forms don't exist in Gen2, besides Unown (which can't breed). Nothing can form-change.

        species = baseID.Species;
        if (species > Legal.MaxSpeciesID_2)
            yield break;
        if (GetCanBeCrystalEgg(pk, species, all))
            yield return new EncounterEgg(species, 0, 5, 2, GameVersion.C, EntityContext.Gen2); // gen2 egg
        yield return new EncounterEgg(species, 0, 5, 2, GameVersion.GS, EntityContext.Gen2); // gen2 egg
    }

    private static bool GetCanBeCrystalEgg(PKM pk, ushort species, bool all)
    {
        if (!ParseSettings.AllowGen2Crystal(pk))
            return false;

        if (all)
            return true;

        // Check if the met data is present or could have been erased.
        if (pk.Format > 2)
            return true; // doesn't have original met location
        if (pk.IsEgg)
            return true; // doesn't have location yet
        if (pk.Met_Level == 1) // Met location of 0 is valid -- second floor of every Pokémon Center 
            return true; // has original met location
        if (species < Legal.MaxSpeciesID_1)
            return true; // can trade RBY to wipe location
        if (pk.Species < Legal.MaxSpeciesID_1)
            return true; // can trade RBY to wipe location

        return false;
    }

    private static bool GetCanBeEgg(PKM pk)
    {
        bool canBeEgg = !(pk.Format == 1 && !ParseSettings.AllowGen1Tradeback) && GetCanBeEgg2(pk);
        if (!canBeEgg)
            return false;

        if (!IsEvolutionValid(pk))
            return false;

        return true;
    }

    private static bool GetCanBeEgg2(PKM pk)
    {
        if (pk.IsEgg)
            return pk.Format == 2;

        if (pk.Format > 2)
        {
            if (pk.Met_Level < 5)
                return false;
        }
        else
        {
            if (pk.Met_Location != 0 && pk.Met_Level != 1) // 2->1->2 clears met info
                return false;
        }

        return pk.CurrentLevel >= 5;
    }

    private static bool IsEvolutionValid(PKM pk)
    {
        var curr = EvolutionChain.GetValidPreEvolutions(pk, minLevel: 5);
        var poss = EvolutionChain.GetValidPreEvolutions(pk, maxLevel: 100, minLevel: 5, skipChecks: true);
        return curr.Length >= poss.Length;
    }
}
