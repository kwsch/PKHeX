namespace PKHeX.Core;

/// <summary>
/// Provides indication that the encounter may or may not have a specific date range to consider.
/// </summary>
public interface IEncounterServerDate
{
    /// <summary>
    /// If true, the date range it may be acquired at is restricted to a specific date range.
    /// </summary>
    bool IsDateRestricted { get; }
}
