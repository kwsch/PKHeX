using System;

namespace PKHeX.Core;

public sealed class EvolutionGroup2 : IEvolutionGroup
{
    public static readonly EvolutionGroup2 Instance = new();
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves2;
    private const int MaxSpecies = Legal.MaxSpeciesID_2;
    private const int Generation = 2;
    private static PersonalTable2 Personal => PersonalTable.C;

    public IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc) => pk.Format > Generation ? EvolutionGroup7.Instance : null;
    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc) => pk.Format != 1 ? EvolutionGroup1.Instance : null;

    public bool Append(PKM pk, EvolutionHistory history, ref ReadOnlySpan<EvoCriteria> chain, EvolutionOrigin enc)
    {
        // Get the first evolution in the chain that can be present in this group
        var any = GetFirstEvolution(chain, out var evo);
        if (!any)
            return false;

        // Get the evolution tree from this group and get the new chain from it.
        byte min;
        if (pk.Format > Generation)
            min = enc.LevelMin;
        else if (pk is ICaughtData2 { CaughtData: not 0 } c)
            min = (byte)c.Met_Level;
        else
            min = enc.LevelMin;
        var criteria = enc with { LevelMax = evo.LevelMax, LevelMin = min };
        var local = GetInitialChain(pk, criteria, evo.Species, evo.Form);

        // Revise the tree
        var revised = Prune(local);

        // Set the tree to the history field
        history.Gen2 = revised;

        // Retain a reference to the current chain for future appending as we step backwards.
        chain = revised;

        return revised.Length != 0;
    }

    public EvoCriteria[] GetInitialChain(PKM pk, EvolutionOrigin enc, ushort species, byte form)
    {
        return Tree.GetExplicitLineage(species, form, pk, enc.LevelMin, enc.LevelMax, MaxSpecies, enc.SkipChecks, enc.Species);
    }

    private static EvoCriteria[] Prune(EvoCriteria[] chain) => chain;

    private static bool GetFirstEvolution(ReadOnlySpan<EvoCriteria> chain, out EvoCriteria result)
    {
        var pt = Personal;
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
