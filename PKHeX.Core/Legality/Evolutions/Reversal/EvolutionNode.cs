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
    public void Add(in EvolutionLink link)
    {
        if (First.IsEmpty)
            First = link;
        else if (Second.IsEmpty)
            Second = link;
        else
            throw new InvalidOperationException($"{nameof(EvolutionNode)} already has two links.");
    }
}
