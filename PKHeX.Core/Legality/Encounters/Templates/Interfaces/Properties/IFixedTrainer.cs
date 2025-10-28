using System;

namespace PKHeX.Core;

/// <summary>
/// Contains information about initial Trainer Details
/// </summary>
public interface IFixedTrainer
{
    /// <summary>
    /// Indicates if the Trainer Name / details are specified by the encounter template.
    /// </summary>
    bool IsFixedTrainer { get; }

    /// <summary>
    /// Checks if the Trainer Name / details match the encounter template.
    /// </summary>
    /// <param name="pk">Entity to check.</param>
    /// <param name="trainer">Trainer name to check.</param>
    /// <param name="language">Language ID to check with.</param>
    /// <returns>True if matches.</returns>
    bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language);
}
