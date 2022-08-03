using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

/// <summary>
/// Object storing a reversal path for evolution nodes.
/// </summary>
public class EvolutionReverseLookup : IEvolutionLookup
{
    private readonly EvolutionNode[] Nodes;
    private readonly int MaxSpecies;
    private readonly Dictionary<int, int> KeyLookup;

    public EvolutionReverseLookup(IEnumerable<(int Key, EvolutionLink Value)> links, int maxSpecies)
    {
        MaxSpecies = maxSpecies;
        KeyLookup = new Dictionary<int, int>(maxSpecies);
        var nodes = new EvolutionNode[maxSpecies * 2];
        int ctr = maxSpecies + 1;
        foreach (var (key, value) in links)
        {
            var index = key <= MaxSpecies ? key : KeyLookup.TryGetValue(key, out var x) ? x : KeyLookup[key] = ctr++;
            ref var node = ref nodes[index];
            node.Add(value);
        }
        Nodes = nodes;
        Debug.Assert(KeyLookup.Count < maxSpecies);
    }

    private int GetIndex(int key)
    {
        if (key <= MaxSpecies)
            return key;
        return KeyLookup.TryGetValue(key, out var index) ? index : 0;
    }

    public ref EvolutionNode this[int key] => ref Nodes[GetIndex(key)];
}

public interface IEvolutionLookup
{
    ref EvolutionNode this[int key] { get; }
}
