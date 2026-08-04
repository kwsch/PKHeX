namespace PKHeX.Application.Abstractions;

/// <summary>
/// Port describing where the application stores per-user data. Implemented in the Infrastructure
/// layer, which resolves platform-standard locations (macOS <c>~/Library/Application Support</c>,
/// Windows <c>%APPDATA%</c>/<c>%LOCALAPPDATA%</c>, Linux <c>$XDG_CONFIG_HOME</c>/<c>$XDG_DATA_HOME</c>
/// with fallbacks). Keeps OS/file-system knowledge out of the inner layers.
/// </summary>
public interface IAppPaths
{
    /// <summary>Directory for user configuration (settings). Created on demand by writers.</summary>
    string ConfigDirectory { get; }

    /// <summary>Directory for larger user data (backups, caches). Created on demand by writers.</summary>
    string DataDirectory { get; }

    /// <summary>Full path to the settings file inside <see cref="ConfigDirectory"/>.</summary>
    string ConfigFilePath { get; }

    /// <summary>
    /// Full path to the legacy settings file that older builds wrote next to the executable.
    /// Used once for migration, then ignored.
    /// </summary>
    string LegacyConfigFilePath { get; }
}
