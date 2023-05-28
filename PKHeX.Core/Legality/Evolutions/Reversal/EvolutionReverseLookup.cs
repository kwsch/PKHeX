using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

/// <summary>
/// Object storing a reversal path for evolution nodes.
/// </summary>
public sealed class EvolutionReverseLookup : IEvolutionLookup
{
    private readonly EvolutionNode[] Nodes;
    private readonly Dictionary<int, int> KeyLookup;

    public EvolutionReverseLookup(IEnumerable<((ushort Species, byte Form), EvolutionLink Value)> links, ushort maxSpecies)
    {
        KeyLookup = new Dictionary<int, int>(maxSpecies);
        var nodes = new EvolutionNode[maxSpecies * 2];
        int ctr = maxSpecies + 1;
        foreach (((ushort Species, byte Form), EvolutionLink Value) value in links)
        {
            int key = value.Item1.Species;
            if (value.Item1.Form != 0)
                key |= value.Item1.Form << 11;
            var index = key <= maxSpecies ? key : KeyLookup.TryGetValue(key, out var x) ? x : KeyLookup[key] = ctr++;
            ref var node = ref nodes[index];
            node.Add(value.Value);
        }
        Nodes = nodes;
        Debug.Assert(KeyLookup.Count < maxSpecies);
    }

    private int GetIndex(ushort species, byte form)
    {
        if (form == 0)
            return species;
        var key = species | (form << 11);
        return KeyLookup.TryGetValue(key, out var index) ? index : 0;
    }

    public ref EvolutionNode this[ushort species, byte form] => ref Nodes[GetIndex(species, form)];
}

public interface IEvolutionLookup
{
    ref EvolutionNode this[ushort species, byte form] { get; }
}
