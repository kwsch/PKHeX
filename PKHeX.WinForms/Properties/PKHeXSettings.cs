using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    [Serializable]
    public sealed class PKHeXSettings
    {
        public StartupSettings Startup { get; set; } = new();
        public BackupSettings Backup { get; set; } = new();

        // General
        public LegalitySetting Legality { get; set; } = new();
        public SetImportSettings Import { get; set; } = new();
        public SlotWriteSettings SlotWrite { get; set; } = new();
        public PrivacySettings Privacy { get; set; } = new();

        // UI Tweaks
        public DisplaySettings Display { get; set; } = new();
        public SoundSettings Sounds { get; set; } = new();
        public HoverSettings Hover { get; set; } = new();

        // GUI Specific
        public DrawConfig Draw { get; set; } = new();

        public static PKHeXSettings GetSettings(string configPath)
        {
            if (!File.Exists(configPath))
                return new PKHeXSettings();

            try
            {
                var lines = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<PKHeXSettings>(lines) ?? new PKHeXSettings();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception x)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                DumpConfigError(x);
                return new PKHeXSettings();
            }
        }

        public static void SaveSettings(string configPath, PKHeXSettings cfg)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    DefaultValueHandling = DefaultValueHandling.Populate,
                    NullValueHandling = NullValueHandling.Ignore,
                };
                var text = JsonConvert.SerializeObject(cfg, settings);
                File.WriteAllText(configPath, text);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception x)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                DumpConfigError(x);
            }
        }

        private static void DumpConfigError(Exception x)
        {
            try
            {
                File.WriteAllLines("config error.txt", new[] { x.ToString() });
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Debug.WriteLine(x); // ???
            }
        }
    }

    [Serializable]
    public sealed class LegalitySetting
    {
        [Description("Flag as Illegal if HOME Tracker is Missing")]
        public bool FlagMissingTracker { get; set; }

        [Description("GB: Allow Generation 2 tradeback learnsets")]
        public bool AllowGen1Tradeback { get; set; } = true;
    }

    [Serializable]
    public sealed class BackupSettings
    {
        [Description("Automatic Backups of Save Files are copied to the backup folder when true.")]
        public bool BAKEnabled { get; set; } = true;

        [Description("Tracks if the \"Create Backup\" prompt has been issued to the user.")]
        public bool BAKPrompt { get; set; }

        [Description("List of extra locations to look for Save Files.")]
#pragma warning disable CA1819 // Properties should not return arrays
        public string[] OtherBackupPaths { get; set; } = Array.Empty<string>();
#pragma warning restore CA1819 // Properties should not return arrays
    }

    [Serializable]
    public sealed class StartupSettings
    {
        [Browsable(false)]
        [Description("Last version that the program was run with.")]
        public string Version { get; set; } = string.Empty;

        [Description("Force HaX mode on Program Launch")]
        public bool ForceHaXOnLaunch { get; set; }

        [Description("Automatically locates the most recently saved Save File when opening a new file.")]
        public bool TryDetectRecentSave { get; set; } = true;

        [Description("Automatically Detect Save File on Program Startup")]
        public AutoLoadSetting AutoLoadSaveOnStartup { get; set; } = AutoLoadSetting.RecentBackup;

        public List<string> RecentlyLoaded = new(MaxRecentCount);

        // Don't let invalid values slip into the startup version.
        private GameVersion _defaultSaveVersion = GameVersion.SW;
        private string _language = GameLanguage.DefaultLanguage;

        [Browsable(false)]
        public string Language
        {
            get => _language;
            set
            {
                if (GameLanguage.GetLanguageIndex(value) == -1)
                    return;
                _language = value;
            }
        }

        [Browsable(false)]
        public GameVersion DefaultSaveVersion
        {
            get => _defaultSaveVersion;
            set
            {
                if (!value.IsValidSavedVersion())
                    return;
                _defaultSaveVersion = value;
            }
        }

        private const int MaxRecentCount = 10;

        public void LoadSaveFile(string path)
        {
            if (RecentlyLoaded.Contains(path))
                return;
            if (RecentlyLoaded.Count >= MaxRecentCount)
                RecentlyLoaded.RemoveAt(RecentlyLoaded.Count - 1);
            RecentlyLoaded.Insert(0, path);
        }
    }

    public enum AutoLoadSetting
    {
        Disabled,
        RecentBackup,
        LastLoaded,
    }

    [Serializable]
    public sealed class HoverSettings
    {
        [Description("Show PKM Slot ToolTip on Hover")]
        public bool HoverSlotShowText { get; set; } = true;

        [Description("Play PKM Slot Cry on Hover")]
        public bool HoverSlotPlayCry { get; set; } = true;

        [Description("Show a Glow effect around the PKM on Hover")]
        public bool HoverSlotGlowEdges { get; set; } = true;
    }

    [Serializable]
    public sealed class SoundSettings
    {
        [Description("Play Sound when loading a new Save File")]
        public bool PlaySoundSAVLoad { get; set; } = true;
        [Description("Play Sound when popping up Legality Report")]
        public bool PlaySoundLegalityCheck { get; set; } = true;
    }

    [Serializable]
    public sealed class SetImportSettings
    {
        [Description("Apply StatNature to Nature on Import")]
        public bool ApplyNature { get; set; } = true;
        [Description("Apply Markings on Import")]
        public bool ApplyMarkings { get; set; } = true;
    }

    [Serializable]
    public sealed class SlotWriteSettings
    {
        [Description("Automatically modify the Save File's Pokédex when injecting a PKM.")]
        public bool SetUpdateDex { get; set; } = true;

        [Description("Automatically adapt the PKM Info to the Save File (Handler, Format)")]
        public bool SetUpdatePKM { get; set; } = true;

        [Description("When enabled and closing/loading a save file, the program will alert if the current save file has been modified without saving.")]
        public bool ModifyUnset { get; set; } = true;
    }

    [Serializable]
    public sealed class DisplaySettings
    {
        [Description("Show Unicode gender symbol characters, or ASCII when disabled.")]
        public bool Unicode { get; set; } = true;

        [Description("Show fanmade shiny sprites when the PKM is shiny.")]
        public bool ShinySprites { get; set; } = true;

        [Description("Show an Egg Sprite As Held Item rather than hiding the PKM")]
        public bool ShowEggSpriteAsHeldItem { get; set; } = true;

        [Description("Don't show the Legality popup if Legal!")]
        public bool IgnoreLegalPopup { get; set; } = true;

        [Description("Flag Illegal Slots in Save File")]
        public bool FlagIllegal { get; set; } = true;
    }

    [Serializable]
    public sealed class PrivacySettings
    {
        [Description("Hide Save File Details in Program Title")]
        public bool HideSAVDetails { get; set; }

        [Description("Hide Secret Details in Editors")]
        public bool HideSecretDetails { get; set; }
    }
}
