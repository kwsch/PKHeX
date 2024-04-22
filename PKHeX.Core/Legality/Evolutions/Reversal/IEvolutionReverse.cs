using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Object storing reversal paths for evolution nodes.
/// </summary>
public interface IEvolutionReverse
{
    /// <summary>
    /// Gets the reverse evolution pathways for the given species and form.
    /// </summary>
    ref readonly EvolutionNode GetReverse(ushort species, byte form);

    /// <summary>
    /// Gets all species the <see cref="species"/>-<see cref="form"/> can evolve from, yielded in order of increasing evolution stage.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Form ID</param>
    /// <returns>Enumerable of species IDs (with the Form IDs included, left shifted by 11).</returns>
    IEnumerable<(ushort Species, byte Form)> GetPreEvolutions(ushort species, byte form);

    /// <summary>
    /// Tries to devolve the given <see cref="pk"/> to the next evolution stage.
    /// </summary>
    /// <param name="head">Current species and form to try devolving</param>
    /// <param name="pk">Entity to devolve</param>
    /// <param name="currentMaxLevel">Maximum allowed level for the result</param>
    /// <param name="levelMin">Minimum level for the result</param>
    /// <param name="skipChecks">Skip evolution exclusion checks</param>
    /// <param name="tweak">Rule tweaks to use when checking evolution criteria</param>
    /// <param name="result">Resulting evolution criteria</param>
    /// <returns>True if the de-evolution is possible and <see cref="result"/> is valid.</returns>
    bool TryDevolve<T>(T head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, EvolutionRuleTweak tweak, out EvoCriteria result) where T : ISpeciesForm;
}
