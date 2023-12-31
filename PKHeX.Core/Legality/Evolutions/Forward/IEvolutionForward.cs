using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Object storing forward paths for evolution nodes.
/// </summary>
public interface IEvolutionForward
{
    /// <summary>
    /// Gets the forward evolution paths for the given species and form.
    /// </summary>
    ReadOnlyMemory<EvolutionMethod> GetForward(ushort species, byte form);

    /// <summary>
    /// Gets all species the <see cref="species"/>-<see cref="form"/> can evolve to, yielded in order of increasing evolution stage.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Form ID</param>
    /// <returns>Enumerable of species IDs (with the Form IDs included, left shifted by 11).</returns>
    IEnumerable<(ushort Species, byte Form)> GetEvolutions(ushort species, byte form);

    /// <summary>
    /// Tries to evolve the given <see cref="pk"/> to the next evolution stage.
    /// </summary>
    /// <param name="head">Current species and form to try evolving</param>
    /// <param name="next">Expected species and form after evolution</param>
    /// <param name="pk">Entity to evolve</param>
    /// <param name="currentMaxLevel">Maximum allowed level for the result</param>
    /// <param name="levelMin">Minimum level for the result</param>
    /// <param name="skipChecks">Skip evolution exclusion checks</param>
    /// <param name="tweak">Rule tweaks to use when checking evolution criteria</param>
    /// <param name="result">Resulting evolution criteria</param>
    /// <returns>True if the evolution is possible and <see cref="result"/> is valid.</returns>
    bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, EvolutionRuleTweak tweak, out EvoCriteria result) where T : ISpeciesForm;
}
