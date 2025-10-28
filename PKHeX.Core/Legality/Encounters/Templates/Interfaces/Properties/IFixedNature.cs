namespace PKHeX.Core;

/// <summary>
/// Contains information about the initial nature of the encounter.
/// </summary>
public interface IFixedNature
{
    /// <summary>
    /// Magic nature index for the encounter.
    /// </summary>
    Nature Nature { get; }

    /// <summary>
    /// Indicates if the nature is a single value (not random).
    /// </summary>
    bool IsFixedNature => Nature != Nature.Random;
}
