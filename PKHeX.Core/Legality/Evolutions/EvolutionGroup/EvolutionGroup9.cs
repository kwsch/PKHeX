using System;

namespace PKHeX.Core;

public sealed class EvolutionGroup9 : IEvolutionGroup
{
    public static readonly EvolutionGroup9 Instance = new();
    private static readonly EvolutionTree Tree9  = EvolutionTree.Evolves9;
    private const int MaxSpecies = Legal.MaxSpeciesID_9;
    private const int Generation = 9;

    public IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc) => null;
    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc)
    {
        if (enc.Generation >= Generation)
            return null;
        return null;
    }

    public bool Append(PKM pk, EvolutionHistory history, ref ReadOnlySpan<EvoCriteria> chain, EvolutionOrigin enc)
    {
        if (chain.Length == 0)
            return false;

        var sv = Append(pk, chain, enc, PersonalTable.SV, Tree9, ref history.Gen9);

        if (!sv)
            return false;

        chain = history.Gen9;
        return chain.Length != 0;
    }

    private static bool Append<T>(PKM pk, ReadOnlySpan<EvoCriteria> chain, EvolutionOrigin enc, T pt, EvolutionTree tree, ref EvoCriteria[] dest) where T : IPersonalTable
    {
        // Get the first evolution in the chain that can be present in this group
        var any = GetFirstEvolution(pt, chain, out var evo);
        if (!any)
            return false;

        // Get the evolution tree from this group and get the new chain from it.
        var criteria = enc with { LevelMax = evo.LevelMax, LevelMin = (byte)pk.Met_Level };
        var local = GetInitialChain(pk, criteria, evo.Species, evo.Form, tree);

        // Revise the tree
        var revised = Prune(local);

        // Set the tree to the history field
        dest = revised;

        return revised.Length != 0;
    }

    public EvoCriteria[] GetInitialChain(PKM pk, EvolutionOrigin enc, ushort species, byte form)
    {
        if (!GetPreferredGroup(species, form, out var group))
            return Array.Empty<EvoCriteria>();
        var tree = GetTree(group);
        return GetInitialChain(pk, enc, species, form, tree);
    }

    private static EvoCriteria[] GetInitialChain(PKM pk, EvolutionOrigin enc, ushort species, byte form, EvolutionTree tree)
    {
        var result = new EvoCriteria[EvolutionTree.MaxEvolutions];
        var count = tree.GetExplicitLineage(result, species, form, pk, enc.LevelMin, enc.LevelMax, MaxSpecies, enc.SkipChecks, enc.Species);
        return result[..count];
    }

    private static EvolutionTree GetTree(PreferredGroup group) => group switch
    {
        _ => Tree9,
    };

    private static bool GetPreferredGroup(ushort species, byte form, out PreferredGroup result)
    {
        if (PersonalTable.SV.IsPresentInGame(species, form))
            result = PreferredGroup.SV;
        else
            result = PreferredGroup.None;
        return result != 0;
    }

    private static EvoCriteria[] Prune(EvoCriteria[] chain) => chain;

    private enum PreferredGroup
    {
        None,
        SV,
    }

    private static bool GetFirstEvolution<T>(T pt, ReadOnlySpan<EvoCriteria> chain, out EvoCriteria result) where T : IPersonalTable
    {
        foreach (var evo in chain)
        {
            // If the evo can't exist in the game, it must be a future evolution.
            if (!pt.IsPresentInGame(evo.Species, evo.Form))
                continue;

            result = evo;
            return true;
        }

        result = default;
        return false;
    }
}
