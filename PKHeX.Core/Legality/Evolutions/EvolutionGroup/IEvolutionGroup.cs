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

/// <summary>
/// Details about the original encounter.
/// </summary>
/// <param name="Species">Species the encounter originated as</param>
/// <param name="Version">Version the encounter originated on</param>
/// <param name="Generation">Generation the encounter originated in</param>
/// <param name="LevelMin">Minimum level the encounter originated at</param>
/// <param name="LevelMax">Maximum level in final state</param>
/// <param name="SkipChecks">Skip enforcement of legality for evolution criteria</param>
public readonly record struct EvolutionOrigin(ushort Species, byte Version, byte Generation, byte LevelMin, byte LevelMax, bool SkipChecks = false);
