using System;
using System.IO;
using System.Text.Json;
using PKHeX.Infrastructure.Configuration;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Covers the persistence behaviour required by issue #138: round-trip, one-time legacy migration,
/// corrupt-file recovery, and forward-compatible unknown-field handling. Uses a throwaway temp
/// directory so nothing touches the real user config location.
/// </summary>
public sealed class SettingsStoreTests : IDisposable
{
    private readonly string _root;
    private readonly TempAppPaths _paths;

    public SettingsStoreTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "pkhex-settings-tests-" + Guid.NewGuid().ToString("N"));
        var configDir = Path.Combine(_root, "config");
        var dataDir = Path.Combine(_root, "data");
        var exeDir = Path.Combine(_root, "exe");
        Directory.CreateDirectory(exeDir);
        _paths = new TempAppPaths
        {
            ConfigDirectory = configDir,
            DataDirectory = dataDir,
            ConfigFilePath = Path.Combine(configDir, "config.json"),
            LegacyConfigFilePath = Path.Combine(exeDir, "config.json"),
        };
    }

    public void Dispose()
    {
        try { Directory.Delete(_root, recursive: true); }
        catch { /* best effort */ }
    }

    private SettingsStore NewStore() => new(_paths);

    [Fact]
    public void Load_WhenNoFile_ReturnsDefaults()
    {
        var settings = NewStore().Load();

        Assert.NotNull(settings);
        Assert.Equal("en", settings.DisplayLanguage);
        Assert.False(File.Exists(_paths.ConfigFilePath)); // load must not create a file
    }

    [Fact]
    public void Save_CreatesDirectoryAndFile()
    {
        var store = NewStore();
        store.Save(new AppSettings { DisplayLanguage = "fr" });

        Assert.True(File.Exists(_paths.ConfigFilePath));
    }

    [Fact]
    public void SaveThenLoad_RoundTripsChangedValues()
    {
        var store = NewStore();
        var toSave = new AppSettings { DisplayLanguage = "de" };
        toSave.Startup.ForceHaXOnLaunch = true;
        toSave.Startup.RecentlyLoaded.Add("/tmp/recent.sav");
        store.Save(toSave);

        var loaded = NewStore().Load();

        Assert.Equal("de", loaded.DisplayLanguage);
        Assert.True(loaded.Startup.ForceHaXOnLaunch);
        Assert.Contains("/tmp/recent.sav", loaded.Startup.RecentlyLoaded);
    }

    [Fact]
    public void Migration_ImportsLegacyConfig_WhenTargetMissing()
    {
        File.WriteAllText(_paths.LegacyConfigFilePath, "{ \"DisplayLanguage\": \"it\" }");

        var loaded = NewStore().Load();

        Assert.Equal("it", loaded.DisplayLanguage);
        Assert.True(File.Exists(_paths.ConfigFilePath), "legacy config should have been copied to the new location");
        Assert.True(File.Exists(_paths.LegacyConfigFilePath), "legacy config must be left in place, not moved");
    }

    [Fact]
    public void Migration_HappensOnce_LegacyIgnoredAfterImport()
    {
        // First run migrates the legacy file.
        File.WriteAllText(_paths.LegacyConfigFilePath, "{ \"DisplayLanguage\": \"it\" }");
        Assert.Equal("it", NewStore().Load().DisplayLanguage);

        // Simulate the user changing settings (new file) and a stale legacy file changing too.
        File.WriteAllText(_paths.ConfigFilePath, "{ \"DisplayLanguage\": \"ja\" }");
        File.WriteAllText(_paths.LegacyConfigFilePath, "{ \"DisplayLanguage\": \"ko\" }");

        // Second run must read the new file and ignore the legacy one entirely.
        Assert.Equal("ja", NewStore().Load().DisplayLanguage);
    }

    [Fact]
    public void Load_CorruptFile_ReturnsDefaults_AndPreservesBak()
    {
        Directory.CreateDirectory(_paths.ConfigDirectory);
        const string garbage = "{ this is not valid json ]";
        File.WriteAllText(_paths.ConfigFilePath, garbage);

        var loaded = NewStore().Load(); // must not throw

        Assert.Equal("en", loaded.DisplayLanguage); // defaults
        var bak = _paths.ConfigFilePath + ".bak";
        Assert.True(File.Exists(bak), "corrupt file should be preserved as .bak");
        Assert.Equal(garbage, File.ReadAllText(bak));
        Assert.False(File.Exists(_paths.ConfigFilePath), "corrupt file should have been moved aside");
    }

    [Fact]
    public void Load_UnknownJsonFields_AreTolerated()
    {
        Directory.CreateDirectory(_paths.ConfigDirectory);
        File.WriteAllText(_paths.ConfigFilePath,
            "{ \"DisplayLanguage\": \"es\", \"FutureFeature\": { \"enabled\": true }, \"AnotherNewKey\": 42 }");

        var loaded = NewStore().Load(); // must not throw on unknown keys

        Assert.Equal("es", loaded.DisplayLanguage);
    }

    [Fact]
    public void Save_PreservesUnknownTopLevelFields_ForForwardCompat()
    {
        Directory.CreateDirectory(_paths.ConfigDirectory);
        File.WriteAllText(_paths.ConfigFilePath,
            "{ \"DisplayLanguage\": \"es\", \"FutureFeature\": { \"enabled\": true } }");

        var store = NewStore();
        var loaded = store.Load();
        store.Save(loaded); // an older version re-saving must not drop the newer version's key

        using var doc = JsonDocument.Parse(File.ReadAllText(_paths.ConfigFilePath));
        Assert.True(doc.RootElement.TryGetProperty("FutureFeature", out var future));
        Assert.True(future.GetProperty("enabled").GetBoolean());
    }

    private sealed class TempAppPaths : IAppPaths
    {
        public required string ConfigDirectory { get; init; }
        public required string DataDirectory { get; init; }
        public required string ConfigFilePath { get; init; }
        public required string LegacyConfigFilePath { get; init; }
    }
}
