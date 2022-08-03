using System;

namespace PKHeX.Core;

/// <summary>
/// Stores two reverse <see cref="EvolutionLink"/> values instead of having a backing array.
/// </summary>
public struct EvolutionNode
{
    internal EvolutionLink First;
    public EvolutionLink Second;

    public void Add(EvolutionLink link)
    {
        if (First.Species == 0)
            First = link;
        else if (Second.Species == 0)
            Second = link;
        else
            throw new InvalidOperationException($"{nameof(EvolutionNode)} already has two links.");
    }

    public void Ban(Func<PKM, bool> func)
    {
        ref var first = ref First;
        if (first.IsEmpty)
            return;
        first.Ban(func);
        ref var second = ref Second;
        if (second.IsEmpty)
            return;
        second.Ban(func);
    }
}
