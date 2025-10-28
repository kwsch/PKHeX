namespace PKHeX.Core;

/// <summary>
/// Common Encounter Properties base interface.
/// <inheritdoc cref="IEncounterInfo"/>
/// </summary>
public interface IEncounterable : IEncounterInfo
{
    /// <summary>
    /// Short name to describe the encounter data, usually just indicating which of the main component encounter types the data is.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Long name to describe the encounter data, containing more detailed (type-specific) information.
    /// </summary>
    string LongName { get; }
}
