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
        if (!Breeding.CanHatchAsEgg(chain[0].Species))
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

        var species = baseID.Species;
        if (species > Legal.MaxSpeciesID_2)
            yield break;

        // Depending on the game it was hatched (GS vs C), met data will be present.
        // Since met data can't be used to infer which game it was created on, we yield both if possible.
        if (ParseSettings.AllowGen2Crystal(pk))
            yield return new EncounterEgg(species, 0, 5, 2, GameVersion.C, EntityContext.Gen2);
        yield return new EncounterEgg(species, 0, 5, 2, GameVersion.GS, EntityContext.Gen2);
    }

    private static bool GetCanBeEgg(PKM pk)
    {
        if (pk.Format == 1 && !ParseSettings.AllowGen1Tradeback)
            return false;
        if (!GetCanBeEgg2(pk))
            return false;
        if (!IsEvolutionValid(pk))
            return false;

        return true;
    }

    private static bool GetCanBeEgg2(PKM pk)
    {
        if (pk.CurrentLevel < 5)
            return false;

        var format = pk.Format;
        if (pk.IsEgg)
            return format == 2;
        if (format > 2)
            return pk.Met_Level >= 5;

        // 2->1->2 clears met info
        return pk.Met_Level switch
        {
            0 => pk.Met_Location == 0,
            1 => true, // Met location of 0 is valid -- second floor of every Pokémon Center
            _ => false,
        };
    }

    private static bool IsEvolutionValid(PKM pk)
    {
        var curr = EvolutionChain.GetValidPreEvolutions(pk, minLevel: 5);
        var poss = EvolutionChain.GetValidPreEvolutions(pk, maxLevel: 100, minLevel: 5, skipChecks: true);
        return curr.Length >= poss.Length;
    }
}
