using System;

namespace PKHeX.Core;

/// <summary>
/// Stores two reverse <see cref="EvolutionLink"/> values instead of having a backing array.
/// </summary>
public struct EvolutionNode
{
    /// <summary> First reverse evolution in the node. </summary>
    public EvolutionLink First;
    /// <summary> Second reverse evolution in the node. Often empty. </summary>
    public EvolutionLink Second;

    /// <summary>
    /// Registers an evolution link into the next empty slot in the node.
    /// </summary>
    /// <param name="link">Link to register</param>
    /// <exception cref="InvalidOperationException"> is thrown if the node is full.</exception>
    public void Add(EvolutionLink link)
    {
        if (First.IsEmpty)
            First = link;
        else if (Second.IsEmpty)
            Second = link;
        else
            throw new InvalidOperationException($"{nameof(EvolutionNode)} already has two links.");
    }

    /// <summary>
    /// Registers a function that disallows the reverse evolution link from being valid if the <see cref="func"/> is not satisfied.
    /// </summary>
    /// <param name="func">Function that checks if the link should be allowed as an evolution path.</param>
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
