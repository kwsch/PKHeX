namespace PKHeX.Core;

/// <summary>
/// Interface for storage cleanup before exporting.
/// </summary>
public interface IStorageCleanup
{
    /// <summary>
    /// Fixes the storage before writing to the save.
    /// </summary>
    /// <returns>True if the storage was modified, false otherwise.</returns>
    bool FixStoragePreWrite();
}
