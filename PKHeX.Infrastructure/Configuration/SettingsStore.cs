using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;

namespace PKHeX.Infrastructure.Configuration;

/// <summary>
/// JSON-backed <see cref="ISettingsStore"/>. Responsibilities (issue #138):
/// <list type="bullet">
/// <item>Reads/writes the settings file under the platform config directory (<see cref="IAppPaths"/>).</item>
/// <item>One-time migration: copies a legacy file found next to the executable into the new
/// location on first run, then leaves it untouched and never reads it again.</item>
/// <item>Corrupt-file recovery: renames the bad file to <c>.bak</c>, logs a warning, and returns
/// defaults instead of crashing.</item>
/// <item>Forward compatibility: unknown JSON keys are preserved via
/// <see cref="AppSettings.ExtensionData"/> (or at minimum tolerated) so newer/older versions
/// round-trip without data loss.</item>
/// </list>
/// Never throws to callers.
/// </summary>
public sealed class SettingsStore : ISettingsStore
{
    private readonly IAppPaths _paths;

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        // Tolerate anything a newer version might legitimately emit rather than treating it as corrupt.
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
    };

    public SettingsStore(IAppPaths paths) => _paths = paths;

    public AppSettings Load()
    {
        var configPath = _paths.ConfigFilePath;

        MigrateLegacyIfNeeded(configPath);

        if (!File.Exists(configPath))
            return new AppSettings();

        try
        {
            var json = File.ReadAllText(configPath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, ReadOptions);
            return settings ?? new AppSettings();
        }
        catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
        {
            RecoverCorruptFile(configPath, ex);
            return new AppSettings();
        }
    }

    public void Save(AppSettings settings)
    {
        try
        {
            var dir = _paths.ConfigDirectory;
            Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(settings, WriteOptions);

            // Write to a temp file in the same directory, then swap into place so a crash mid-write
            // cannot leave a half-written (corrupt) settings file behind.
            var tempPath = _paths.ConfigFilePath + ".tmp";
            File.WriteAllText(tempPath, json);
            File.Move(tempPath, _paths.ConfigFilePath, overwrite: true);
        }
        catch (Exception ex)
        {
            Trace.TraceWarning($"Failed to save settings: {ex.Message}");
        }
    }

    /// <summary>
    /// On first run (no file in the new location), imports a legacy <c>config.json</c> that older
    /// builds wrote next to the executable. The legacy file is copied (left in place) so a rollback
    /// still finds it; once the new file exists this is a no-op forever.
    /// </summary>
    private void MigrateLegacyIfNeeded(string configPath)
    {
        try
        {
            if (File.Exists(configPath))
                return;

            var legacy = _paths.LegacyConfigFilePath;
            if (!File.Exists(legacy))
                return;

            // Guard against the (degenerate) case where the legacy path resolves to the new path.
            if (string.Equals(Path.GetFullPath(legacy), Path.GetFullPath(configPath), StringComparison.Ordinal))
                return;

            Directory.CreateDirectory(_paths.ConfigDirectory);
            File.Copy(legacy, configPath, overwrite: false);
            Trace.TraceInformation($"Migrated legacy settings from '{legacy}' to '{configPath}'.");
        }
        catch (Exception ex)
        {
            // Migration is best-effort: a failure here just means we start from defaults, which the
            // caller handles. Do not surface it.
            Trace.TraceWarning($"Failed to migrate legacy settings: {ex.Message}");
        }
    }

    /// <summary>
    /// Preserves a corrupt settings file as <c>config.json.bak</c> (overwriting any previous backup)
    /// and logs a warning, so the reset is traceable and the bad data is recoverable.
    /// </summary>
    private static void RecoverCorruptFile(string configPath, Exception cause)
    {
        var backupPath = configPath + ".bak";
        try
        {
            File.Move(configPath, backupPath, overwrite: true);
            Trace.TraceWarning(
                $"Settings file '{configPath}' was unreadable ({cause.Message}); preserved as '{backupPath}' and reset to defaults.");
        }
        catch (Exception ex)
        {
            Trace.TraceWarning(
                $"Settings file '{configPath}' was unreadable ({cause.Message}) and could not be backed up: {ex.Message}. Using defaults.");
        }
    }
}
