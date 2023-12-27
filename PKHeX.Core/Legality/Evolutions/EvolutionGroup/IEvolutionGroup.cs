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

    /// <summary>
    /// Walks down the evolution chain to find previous evolutions.
    /// </summary>
    /// <param name="result">Array to store results in.</param>
    /// <param name="pk">PKM to check.</param>
    /// <param name="enc">Cached metadata about the PKM.</param>
    /// <returns>Number of results found.</returns>
    int Devolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc);

    /// <summary>
    /// Walks up the evolution chain to find the evolution path.
    /// </summary>
    /// <param name="result">Array to store best results in.</param>
    /// <param name="pk">PKM to check.</param>
    /// <param name="enc">Cached metadata about the PKM.</param>
    /// <param name="history">History of evolutions to cache arrays for individual contexts.</param>
    /// <returns>Number of results found.</returns>
    int Evolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, EvolutionHistory history);

    /// <summary>
    /// Discards all entries that do not exist in the group.
    /// </summary>
    void DiscardForOrigin(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc);
}

/// <summary>
/// Provides information about how to evolve to the next evolution stage.
/// </summary>
public interface IEvolutionEnvironment
{
    /// <summary>
    /// Attempts to devolve the provided <see cref="head"/> to the previous evolution.
    /// </summary>
    /// <returns>True if the de-evolution was successful and <see cref="result"/> should be used.</returns>
    bool TryDevolve<T>(T head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm;

    /// <summary>
    /// Attempts to evolve the provided <see cref="head"/> to the next evolution.
    /// </summary>
    /// <returns>True if the evolution was successful and <see cref="result"/> should be used.</returns>
    bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm;
}
