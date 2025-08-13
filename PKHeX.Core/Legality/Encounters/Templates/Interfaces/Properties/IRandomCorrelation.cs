namespace PKHeX.Core;

/// <summary>
/// Interface for a random <see cref="PIDType"/> correlation.
/// </summary>
public interface IRandomCorrelation
{
    /// <summary>
    /// Checks if the given <see cref="PIDType"/> is compatible with the encounter restrictions.
    /// </summary>
    /// <param name="type">Observed <see cref="PIDType"/> of the <see cref="pk"/></param>
    /// <param name="pk">Entity to compare against for other details</param>
    /// <returns>True if all details are compatible</returns>
    RandomCorrelationRating IsCompatible(PIDType type, PKM pk);

    /// <summary>
    /// Gets the suggested <see cref="PIDType"/> for the encounter.
    /// </summary>
    PIDType GetSuggestedCorrelation();
}

/// <summary>
/// Specialized interface for mutating a <see cref="PIDIV"/> value to a derived type.
/// </summary>
public interface IRandomCorrelationEvent3 : IRandomCorrelation
{
    /// <summary>
    /// Mutates the <see cref="PIDIV"/> value to a derived type, if possible.
    /// </summary>
    /// <param name="value">Value to check and mutate</param>
    /// <param name="pk">Entity to compare against</param>
    /// <returns>True if Compatible. Revision of the ref value is not returned.</returns>
    RandomCorrelationRating IsCompatibleReviseReset(ref PIDIV value, PKM pk);
}

public enum RandomCorrelationRating
{
    // Clear match, no issues.
    Match,
    // Weak match, could be better matched to another encounter.
    NotIdeal,

    // Invalid
    Mismatch,
}
