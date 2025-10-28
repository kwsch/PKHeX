using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Object storing reversal paths for evolution nodes.
/// </summary>
public interface IEvolutionReverse
{
    /// <summary>
    /// Gets the reverse lookup information for the evolutionary lineage.
    /// </summary>
    /// <remarks>The lineage data is derived from the provided personal and entry information. This property
    /// is read-only and provides a precomputed result.</remarks>
    EvolutionReverseLookup Lineage { get; }

    /// <summary>
    /// Gets the reverse evolution node for the specified species and form.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Form ID</param>
    ref readonly EvolutionNode GetReverse(ushort species, byte form);

    /// <summary>
    /// Enumerates all pre-evolutions for the given species and form, yielding them in order of increasing evolution stage.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Form ID</param>
    IEnumerable<(ushort Species, byte Form)> GetPreEvolutions(ushort species, byte form);

    /// <summary>
    /// Attempts to determine if the specified Pokémon can devolve based on the provided criteria.
    /// </summary>
    /// <typeparam name="T">The type representing the species and form of the Pokémon. Must implement <see cref="ISpeciesForm"/>.</typeparam>
    /// <param name="head">The species and form of the Pokémon to evaluate for devolution.</param>
    /// <param name="pk">The Pokémon instance to check for devolution eligibility.</param>
    /// <param name="currentMaxLevel">The maximum level currently allowed for devolution.</param>
    /// <param name="levelMin">The minimum level required for devolution.</param>
    /// <param name="skipChecks">A value indicating whether to bypass additional validation checks during the devolution process.</param>
    /// <param name="tweak">The evolution rule tweak to apply when evaluating devolution criteria.</param>
    /// <param name="result">When this method returns, contains the devolution criteria if the operation succeeds; otherwise, the default
    /// value.</param>
    /// <returns><see langword="true"/> if the Pokémon can devolve based on the provided criteria; otherwise, <see
    /// langword="false"/>.</returns>
    bool TryDevolve<T>(T head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, EvolutionRuleTweak tweak, out EvoCriteria result) where T : ISpeciesForm;
}
