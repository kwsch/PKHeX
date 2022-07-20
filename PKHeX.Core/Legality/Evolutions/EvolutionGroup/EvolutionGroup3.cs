using System;

namespace PKHeX.Core;

public sealed class EvolutionGroup3 : IEvolutionGroup
{
    public static readonly EvolutionGroup3 Instance = new();
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves3;
    private const int MaxSpecies = Legal.MaxSpeciesID_3;
    private const int Generation = 3;
    private static PersonalTable3 Personal => PersonalTable.E;

    public IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc) => pk.Format > Generation ? EvolutionGroup4.Instance : null;
    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc) => null;

    public bool Append(PKM pk, EvolutionHistory history, ref ReadOnlySpan<EvoCriteria> chain, EvolutionOrigin enc)
    {
        // Get the first evolution in the chain that can be present in this group
        var any = GetFirstEvolution(chain, out var evo);
        if (!any)
            return false;

        // Get the evolution tree from this group and get the new chain from it.
        var min = pk.Format > Generation ? enc.LevelMin : (byte)pk.Met_Level;
        var criteria = enc with { LevelMax = evo.LevelMax, LevelMin = min };
        var local = GetInitialChain(pk, criteria, evo.Species, evo.Form);

        // Revise the tree
        var revised = Prune(local);

        // Set the tree to the history field
        history.Gen3 = revised;

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
