using System;
using System.Collections.Generic;
using System.ComponentModel;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed class StartupSettings : IStartupSettings
{
    [Browsable(false)]
    [LocalizedDescription("Last version that the program was run with.")]
    public string Version { get; set; } = string.Empty;

    [LocalizedDescription("Use the Dark color mode for the application on startup.")]
    public bool DarkMode { get; set; }

    [LocalizedDescription("Force HaX mode on Program Launch")]
    public bool ForceHaXOnLaunch { get; set; }

    [LocalizedDescription("Skips displaying the splash screen on Program Launch.")]
    public bool SkipSplashScreen { get; set; }

    [LocalizedDescription("Automatically locates the most recently saved Save File when opening a new file.")]
    public bool TryDetectRecentSave { get; set; } = true;

    [LocalizedDescription("Automatically Detect Save File on Program Startup")]
    public SaveFileLoadSetting AutoLoadSaveOnStartup { get; set; } = SaveFileLoadSetting.RecentBackup;

    [LocalizedDescription("Show the changelog when a new version of the program is run for the first time.")]
    public bool ShowChangelogOnUpdate { get; set; } = true;

    [LocalizedDescription("Loads plugins from the plugins folder, assuming the folder exists.")]
    public bool PluginLoadEnable { get; set; } = true;

    [LocalizedDescription("Loads any plugins that were merged into the main executable file.")]
    public bool PluginLoadMerged { get; set; }

    [Browsable(false)]
    public List<string> RecentlyLoaded { get; set; } = new(DefaultMaxRecent);

    private const int DefaultMaxRecent = 10;
    private uint MaxRecentCount = DefaultMaxRecent;

    [LocalizedDescription("Amount of recently loaded save files to remember.")]
    public uint RecentlyLoadedMaxCount
    {
        get => MaxRecentCount;
        // Sanity check to not let the user foot-gun themselves a slow recall time.
        set => MaxRecentCount = Math.Clamp(value, 1, 1000);
    }

    // Don't let invalid values slip into the startup version.

    [Browsable(false)]
    public string Language
    {
        get;
        set
        {
            if (!GameLanguage.IsLanguageValid(value))
            {
                // Migrate old language codes set in earlier versions.
                field = value switch
                {
                    "zh" => "zh-Hans",
                    "zh2" => "zh-Hant",
                    _ => field,
                };
                return;
            }

            field = value;
        }
    } = WinFormsUtil.GetCultureLanguage();

    [Browsable(false)]
    public GameVersion DefaultSaveVersion
    {
        get;
        set
        {
            if (!value.IsValidSavedVersion())
                return;
            field = value;
        }
    } = Latest.Version;

    public void LoadSaveFile(string path)
    {
        var recent = RecentlyLoaded;
        // Remove from list if already present.
        if (!recent.Remove(path) && recent.Count >= MaxRecentCount)
            recent.RemoveAt(recent.Count - 1);
        recent.Insert(0, path);
    }
}
