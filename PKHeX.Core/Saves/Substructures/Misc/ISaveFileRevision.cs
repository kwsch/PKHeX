namespace PKHeX.Core;

/// <summary>
/// Exposes information to differentiate the save file from other patches of the same game.
/// </summary>
public interface ISaveFileRevision
{
    /// <summary>
    /// Incremented magic number to differentiate feature sets available.
    /// </summary>
    int SaveRevision { get; }

    /// <summary>
    /// Magic string to differentiate feature sets available.
    /// </summary>
    string SaveRevisionString { get; }
}
