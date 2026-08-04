using PKHeX.Application.Services;

namespace PKHeX.Application.Abstractions;

/// <summary>
/// Port for loading and persisting <see cref="AppSettings"/>. The Infrastructure implementation is
/// responsible for platform path resolution, one-time migration from the legacy location,
/// corrupt-file recovery, and forward-compatible JSON handling. Keeps file IO out of the inner
/// layers (see issue #138).
/// </summary>
public interface ISettingsStore
{
    /// <summary>
    /// Loads settings from the platform config directory. Migrates a legacy file on first run,
    /// recovers to defaults (preserving a <c>.bak</c>) if the file is corrupt, and never throws.
    /// </summary>
    AppSettings Load();

    /// <summary>Persists settings to the platform config directory. Never throws.</summary>
    void Save(AppSettings settings);
}
