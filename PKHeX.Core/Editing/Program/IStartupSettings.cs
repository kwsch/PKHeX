using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Settings used for starting up an editing environment.
/// </summary>
public interface IStartupSettings
{
    /// <summary>
    /// Save File version to start the environment with if a preexisting save file has not been chosen.
    /// </summary>
    GameVersion DefaultSaveVersion { get; }

    /// <summary>
    /// Method to load the environment's initial save file.
    /// </summary>
    AutoLoadSetting AutoLoadSaveOnStartup { get; }

    /// <summary>
    /// List of recently loaded save file paths.
    /// </summary>
    List<string> RecentlyLoaded { get; }
}
