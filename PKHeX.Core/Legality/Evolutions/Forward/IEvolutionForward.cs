using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Object storing forward paths for evolution nodes.
/// </summary>
public interface IEvolutionForward
{
    /// <summary>
    /// Retrieves the forward evolution methods for a specified species and form.
    /// </summary>
    /// <param name="species">The species identifier of the Pokémon.</param>
    /// <param name="form">The form identifier of the Pokémon.</param>
    /// <returns>A read-only memory segment containing the evolution methods for the specified species and form. If no evolution
    /// methods are available, the returned segment will be empty.</returns>
    ReadOnlyMemory<EvolutionMethod> GetForward(ushort species, byte form);

    /// <summary>
    /// Retrieves the evolutions for a given species and form.
    /// </summary>
    /// <param name="species">The species identifier of the entity for which to retrieve evolutions.</param>
    /// <param name="form">The form identifier of the entity for which to retrieve evolutions.</param>
    /// <returns>An enumerable collection of tuples, where each tuple contains the species identifier and form identifier of a
    /// possible evolution. The collection will be empty if no evolutions are available.</returns>
    IEnumerable<(ushort Species, byte Form)> GetEvolutions(ushort species, byte form);

    /// <summary>
    /// Attempts to determine if the specified Pokémon can evolve into the target species and form.
    /// </summary>
    /// <remarks>This method evaluates all applicable evolution methods for the given Pokémon and determines
    /// if the evolution to the specified target species and form is valid based on the provided parameters. If the
    /// evolution is valid, the resulting criteria are returned in the <paramref name="result"/> parameter.</remarks>
    /// <typeparam name="T">The type representing the species and form of the Pokémon.</typeparam>
    /// <param name="head">The current species and form of the Pokémon.</param>
    /// <param name="next">The target species and form to evolve into.</param>
    /// <param name="pk">The Pokémon instance to evaluate for evolution.</param>
    /// <param name="currentMaxLevel">The maximum level currently allowed for evolution.</param>
    /// <param name="levelMin">The minimum level required for evolution.</param>
    /// <param name="skipChecks">A value indicating whether to bypass certain evolution checks.</param>
    /// <param name="tweak">Additional rules or tweaks to apply during the evolution check.</param>
    /// <param name="result">When this method returns, contains the evolution criteria if the evolution is valid; otherwise, the default
    /// value.</param>
    /// <returns><see langword="true"/> if the Pokémon can evolve into the specified species and form; otherwise, <see
    /// langword="false"/>.</returns>
    bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, EvolutionRuleTweak tweak, out EvoCriteria result) where T : ISpeciesForm;
}
