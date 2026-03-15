using System;
using System.Collections.Generic;
using PKHeX.Core;

namespace PKHeX.Avalonia.Settings;

public sealed class StartupSettings : IStartupSettings
{
    public string Version { get; set; } = string.Empty;
    public bool DarkMode { get; set; }
    public bool ForceHaXOnLaunch { get; set; }
    public bool SkipSplashScreen { get; set; }
    public bool TryDetectRecentSave { get; set; } = true;
    public SaveFileLoadSetting AutoLoadSaveOnStartup { get; set; } = SaveFileLoadSetting.RecentBackup;
    public bool ShowChangelogOnUpdate { get; set; } = true;
    public bool PluginLoadEnable { get; set; } = true;
    public bool PluginLoadMerged { get; set; }
    public List<string> RecentlyLoaded { get; set; } = new(DefaultMaxRecent);

    private const int DefaultMaxRecent = 10;
    private uint MaxRecentCount = DefaultMaxRecent;

    public uint RecentlyLoadedMaxCount
    {
        get => MaxRecentCount;
        set => MaxRecentCount = Math.Clamp(value, 1, 1000);
    }

    public string Language { get; set; } = GameLanguage.DefaultLanguage;

    public GameVersion DefaultSaveVersion { get; set; } = Latest.Version;

    public void LoadSaveFile(string path)
    {
        var recent = RecentlyLoaded;
        if (!recent.Remove(path) && recent.Count >= MaxRecentCount)
            recent.RemoveAt(recent.Count - 1);
        recent.Insert(0, path);
    }
}
