namespace PKHeX.Core;

/// <summary>
/// Contains information about the number of perfect IVs the object has.
/// </summary>
public interface IFlawlessIVCount
{
    /// <summary>
    /// Number of perfect IVs the object has.
    /// </summary>
    byte FlawlessIVCount { get; }
}
