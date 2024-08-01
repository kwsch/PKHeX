namespace PKHeX.Core;

/// <summary>
/// Contains information about the level the object was met at.
/// </summary>
public interface IMetLevel
{
    /// <summary>
    /// Level met.
    /// </summary>
    /// <remarks>Can be below the original current level if the encounter says so.</remarks>
    byte MetLevel { get; }
}
