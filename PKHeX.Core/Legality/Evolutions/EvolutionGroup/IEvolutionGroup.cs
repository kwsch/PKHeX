using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in the games that it represents.
/// </summary>
public interface IEvolutionGroup
{
    /// <summary>
    /// Gets the previous (backward) generation group to traverse to continue processing.
    /// </summary>
    IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc);

    /// <summary>
    /// Gets the next (forward) generation group to traverse to continue processing.
    /// </summary>
    IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc);

    bool Append(PKM pk, EvolutionHistory history, ref ReadOnlySpan<EvoCriteria> chain, EvolutionOrigin enc);

    EvoCriteria[] GetInitialChain(PKM pk, EvolutionOrigin enc, ushort species, byte form);
}
