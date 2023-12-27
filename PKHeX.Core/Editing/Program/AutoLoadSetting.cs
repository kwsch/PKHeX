namespace PKHeX.Core;

/// <summary>
/// Option to load a save file automatically to an editing environment.
/// </summary>
public enum AutoLoadSetting
{
    /// <summary>
    /// Doesn't autoload a save file, and instead uses a fake save file data.
    /// </summary>
    Disabled,

    /// <summary>
    /// Loads the most recently created Save File in the usual backup locations.
    /// </summary>
    RecentBackup,

    /// <summary>
    /// Loads the most recently opened Save File path.
    /// </summary>
    LastLoaded,
}
