using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public sealed class EvolutionGroup8 : IEvolutionGroup
{
    public static readonly EvolutionGroup8 Instance = new();
    private static readonly EvolutionTree Tree8  = EvolutionTree.Evolves8;
    private static readonly EvolutionTree Tree8a = EvolutionTree.Evolves8a;
    private static readonly EvolutionTree Tree8b = EvolutionTree.Evolves8b;
    private const int MaxSpecies = Legal.MaxSpeciesID_8a;
    private const int Generation = 8;

    public IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc) => null;
    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc)
    {
        if (enc.Generation >= Generation)
            return null;
        if ((GameVersion)enc.Version is GP or GE or GG or GO)
            return EvolutionGroup7b.Instance;
        return EvolutionGroup7.Instance;
    }

    public bool Append(PKM pk, EvolutionHistory history, ref ReadOnlySpan<EvoCriteria> chain, EvolutionOrigin enc)
    {
        if (chain.Length == 0)
            return false;

        var swsh = Append(pk, chain, enc, PersonalTable.SWSH, Tree8 , ref history.Gen8 );
        var pla  = Append(pk, chain, enc, PersonalTable.LA  , Tree8a, ref history.Gen8a);
        var bdsp = Append(pk, chain, enc, PersonalTable.BDSP, Tree8b, ref history.Gen8b);

        if (!(swsh || pla || bdsp))
            return false;

        // Block BD/SP transfers that are impossible
        BlockBDSP(history, enc);

        if (!pk.IsUntraded && !(ParseSettings.IgnoreTransferIfNoTracker && pk is IHomeTrack { Tracker: 0 }))
        {
            CrossPropagate(history);
        }
        else
        {
            DeleteAdjacent(pk, history);

            if (!HasVisited(history))
                return false;
        }

        chain = GetMaxChain(history);

        return chain.Length != 0;
    }

    private static bool HasVisited(EvolutionHistory history)
    {
        return history.Gen8.Length != 0 || history.Gen8a.Length != 0 || history.Gen8b.Length != 0;
    }

    private static void DeleteAdjacent(PKM pk, EvolutionHistory history)
    {
        if (pk is not PK8)
            history.Gen8 = Array.Empty<EvoCriteria>();
        if (pk is not PA8)
            history.Gen8a = Array.Empty<EvoCriteria>();
        if (pk is not PB8)
            history.Gen8b = Array.Empty<EvoCriteria>();
    }

    private static void BlockBDSP(EvolutionHistory history, EvolutionOrigin enc)
    {
        var bdsp = history.Gen8b;
        if (bdsp.Length == 0)
            return;

        // Spinda and Nincada cannot transfer in or out as the current species.
        // Remove them from their non-origin game evolution chains.
        var last = bdsp[^1];
        if (last.Species == (int)Species.Nincada)
            RemoveIfSpecies(history, enc);
        else if (last.Species == (int)Species.Spinda)
            RemoveIfSpecies(history, enc);

        static void RemoveIfSpecies(EvolutionHistory history, EvolutionOrigin enc)
        {
            var wasBDSP = BDSP.Contains(enc.Version);
            ref var evos = ref wasBDSP ? ref history.Gen8 : ref history.Gen8b;
            evos = evos.Length < 2 ? Array.Empty<EvoCriteria>() : evos.AsSpan(0, evos.Length - 1).ToArray();
        }
    }

    private static ReadOnlySpan<EvoCriteria> GetMaxChain(EvolutionHistory history)
    {
        var arr0 = history.Gen8;
        var arr1 = history.Gen8a;
        var arr2 = history.Gen8b;
        if (arr0.Length >= arr1.Length && arr0.Length >= arr2.Length)
            return arr0;
        if (arr1.Length >= arr2.Length)
            return arr1;
        return arr2;
    }

    private static void CrossPropagate(EvolutionHistory history)
    {
        var arr0 = history.Gen8;
        var arr1 = history.Gen8a;
        var arr2 = history.Gen8b;

        ReplaceIfBetter(arr0, arr1, arr2);
        ReplaceIfBetter(arr1, arr0, arr2);
        ReplaceIfBetter(arr2, arr0, arr1);
    }

    private static void ReplaceIfBetter(Span<EvoCriteria> local, ReadOnlySpan<EvoCriteria> other1, ReadOnlySpan<EvoCriteria> other2)
    {
        for (int i = 0; i < local.Length; i++)
        {
            ReplaceIfBetter(local, other1, i);
            ReplaceIfBetter(local, other2, i);
        }
    }

    private static void ReplaceIfBetter(Span<EvoCriteria> local, ReadOnlySpan<EvoCriteria> other, int parentIndex)
    {
        // Replace the evolution entry if another connected game has a better evolution method (different min/max).
        var native = local[parentIndex];

        // Check if the evo is in the other game; if not, we're done here.
        var index = IndexOfSpecies(other, native.Species);
        if (index == -1)
            return;

        var alt = other[index];
        if (alt.LevelMin < native.LevelMin || alt.LevelMax > native.LevelMax)
            local[parentIndex] = alt;
    }

    private static int IndexOfSpecies(ReadOnlySpan<EvoCriteria> evos, ushort species)
    {
        // Returns the index of the first evo that matches the species
        for (int i = 0; i < evos.Length; i++)
        {
            if (evos[i].Species == species)
                return i;
        }
        return -1;
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
        if (species is (int)Species.Dialga or (int)Species.Palkia or (int)Species.Arceus)
            form = 0; // PLA forms; play nice for yielding SW/SH context
        return tree.GetExplicitLineage(species, form, pk, enc.LevelMin, enc.LevelMax, MaxSpecies, enc.SkipChecks, enc.Species);
    }

    private static EvolutionTree GetTree(PreferredGroup group) => group switch
    {
        PreferredGroup.BDSP => Tree8b,
        PreferredGroup.LA => Tree8a,
        _ => Tree8,
    };

    private static bool GetPreferredGroup(ushort species, byte form, out PreferredGroup result)
    {
        if (PersonalTable.LA.IsPresentInGame(species, form))
            result = PreferredGroup.LA;
        else if (PersonalTable.SWSH.IsPresentInGame(species, form))
            result = PreferredGroup.SWSH;
        else if (PersonalTable.BDSP.IsPresentInGame(species, form))
            result = PreferredGroup.BDSP;
        else
            result = PreferredGroup.None;
        return result != 0;
    }

    private static EvoCriteria[] Prune(EvoCriteria[] chain) => chain;

    private enum PreferredGroup
    {
        None,
        LA,
        SWSH,
        BDSP,
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
