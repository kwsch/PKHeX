namespace PKHeX.Core;

/// <summary>
/// Enumeration indicating if a date range check is satisfied.
/// </summary>
public enum EncounterServerDateCheck
{
    /// <summary> No need to consider date range. </summary>
    None,
    /// <summary> Date does fall within the range it was available. </summary>
    Valid,
    /// <summary> Date does NOT fall within the range it was available. </summary>
    Invalid,
}
