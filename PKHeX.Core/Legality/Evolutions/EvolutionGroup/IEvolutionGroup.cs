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

    int Devolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc);

    int Evolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, EvolutionHistory history);

    /// <summary>
    /// Discards all entries that do not exist in the group.
    /// </summary>
    void DiscardForOrigin(Span<EvoCriteria> result, PKM pk);
}

public interface IEvolutionEnvironment
{
    bool TryDevolve(ISpeciesForm head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result);

    bool TryEvolve(ISpeciesForm head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result);
}
