using System;
using System.IO;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// In-memory-ish <see cref="IAppPaths"/> for tests: points at a throwaway temp directory so
/// nothing touches the real user config/data location. Each instance gets its own unique root.
/// </summary>
internal sealed class FakeAppPaths : IAppPaths
{
    public string ConfigDirectory { get; }
    public string DataDirectory { get; }
    public string ConfigFilePath { get; }
    public string LegacyConfigFilePath { get; }

    public FakeAppPaths()
    {
        var root = Path.Combine(Path.GetTempPath(), "pkhex-apppaths-tests-" + Guid.NewGuid().ToString("N"));
        ConfigDirectory = Path.Combine(root, "config");
        DataDirectory = Path.Combine(root, "data");
        ConfigFilePath = Path.Combine(ConfigDirectory, "config.json");
        LegacyConfigFilePath = Path.Combine(root, "legacy.json");
    }
}
