using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

[JsonSerializable(typeof(PKHeXSettings))]
public sealed partial class PKHeXSettingsContext : JsonSerializerContext;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class PKHeXSettings
{
    public StartupSettings Startup { get; set; } = new();
    public BackupSettings Backup { get; set; } = new();

    // General
    public LegalitySettings Legality { get; set; } = new();
    public EntityConverterSettings Converter { get; set; } = new();
    public SetImportSettings Import { get; set; } = new();
    public SlotWriteSettings SlotWrite { get; set; } = new();
    public PrivacySettings Privacy { get; set; } = new();
    public SaveLanguageSettings SaveLanguage { get; set; } = new();

    // UI Tweaks
    public DisplaySettings Display { get; set; } = new();
    public SpriteSettings Sprite { get; set; } = new();
    public SoundSettings Sounds { get; set; } = new();
    public HoverSettings Hover { get; set; } = new();

    // GUI Specific
    public DrawConfig Draw { get; set; } = new();
    public AdvancedSettings Advanced { get; set; } = new();
    public EntityEditorSettings EntityEditor { get; set; } = new();
    public EntityDatabaseSettings EntityDb { get; set; } = new();
    public EncounterDatabaseSettings EncounterDb { get; set; } = new();
    public MysteryGiftDatabaseSettings MysteryDb { get; set; } = new();
    public ReportGridSettings Report { get; set; } = new();

    [Browsable(false)]
    public SlotExportSettings SlotExport { get; set; } = new();

    private static PKHeXSettingsContext GetContext() => new(new()
    {
        WriteIndented = true,
        Converters = { new ColorJsonConverter() },
    });

    public sealed class ColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => ColorTranslator.FromHtml(reader.GetString() ?? string.Empty);

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) => writer.WriteStringValue($"#{value.R:x2}{value.G:x2}{value.B:x2}");
    }

    public static PKHeXSettings GetSettings(string configPath)
    {
        if (!File.Exists(configPath))
            return new PKHeXSettings();

        try
        {
            var lines = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize(lines, GetContext().PKHeXSettings) ?? new PKHeXSettings();
        }
        catch (Exception x)
        {
            DumpConfigError(x);
            return new PKHeXSettings();
        }
    }

    public static async Task SaveSettings(string configPath, PKHeXSettings cfg)
    {
        try
        {
            // Serialize the object asynchronously and write it to the path.
            await using var fs = File.Create(configPath);
            await JsonSerializer.SerializeAsync(fs, cfg, GetContext().PKHeXSettings).ConfigureAwait(false);
        }
        catch (Exception x)
        {
            DumpConfigError(x);
        }
    }

    private static async void DumpConfigError(Exception x)
    {
        try
        {
            await File.WriteAllTextAsync("config error.txt", x.ToString());
        }
        catch (Exception)
        {
            Debug.WriteLine(x); // ???
        }
    }
}

public sealed class BackupSettings
{
    [LocalizedDescription("Automatic Backups of Save Files are copied to the backup folder when true.")]
    public bool BAKEnabled { get; set; } = true;

    [LocalizedDescription("Tracks if the \"Create Backup\" prompt has been issued to the user.")]
    public bool BAKPrompt { get; set; }

    [LocalizedDescription("List of extra locations to look for Save Files.")]
    public List<string> OtherBackupPaths { get; set; } = [];

    [LocalizedDescription("Save File file-extensions (no period) that the program should also recognize.")]
    public List<string> OtherSaveFileExtensions { get; set; } = [];
}

public sealed class StartupSettings : IStartupSettings
{
    [Browsable(false)]
    [LocalizedDescription("Last version that the program was run with.")]
    public string Version { get; set; } = string.Empty;

    [LocalizedDescription("Force HaX mode on Program Launch")]
    public bool ForceHaXOnLaunch { get; set; }

    [LocalizedDescription("Automatically locates the most recently saved Save File when opening a new file.")]
    public bool TryDetectRecentSave { get; set; } = true;

    [LocalizedDescription("Automatically Detect Save File on Program Startup")]
    public AutoLoadSetting AutoLoadSaveOnStartup { get; set; } = AutoLoadSetting.RecentBackup;

    [LocalizedDescription("Show the changelog when a new version of the program is run for the first time.")]
    public bool ShowChangelogOnUpdate { get; set; } = true;

    [LocalizedDescription("Loads plugins from the plugins folder, assuming the folder exists. Try LoadFile to mitigate intermittent load failures.")]
    public PluginLoadSetting PluginLoadMethod { get; set; } = PluginLoadSetting.LoadFrom;

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
    private GameVersion _defaultSaveVersion = PKX.Version;
    private string _language = WinFormsUtil.GetCultureLanguage();

    [Browsable(false)]
    public string Language
    {
        get => _language;
        set
        {
            if (!GameLanguage.IsLanguageValid(value))
            {
                // Migrate old language codes set in earlier versions.
                _language = value switch
                {
                    "zh" => "zh-Hans",
                    "zh2" => "zh-Hant",
                    _ => _language,
                };
                return;
            }
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

    public void LoadSaveFile(string path)
    {
        var recent = RecentlyLoaded;
        // Remove from list if already present.
        if (!recent.Remove(path) && recent.Count >= MaxRecentCount)
            recent.RemoveAt(recent.Count - 1);
        recent.Insert(0, path);
    }
}

public enum PluginLoadSetting
{
    DontLoad,
    LoadFrom,
    LoadFile,
    UnsafeLoadFrom,
    LoadFromMerged,
    LoadFileMerged,
    UnsafeMerged,
}

public sealed class EntityConverterSettings
{
    [LocalizedDescription("Allow PKM file conversion paths that are not possible via official methods. Individual properties will be copied sequentially.")]
    public EntityCompatibilitySetting AllowIncompatibleConversion { get; set; } = EntityCompatibilitySetting.DisallowIncompatible;

    [LocalizedDescription("Allow PKM file conversion paths to guess the legal original encounter data that is not stored in the format that it was converted from.")]
    public EntityRejuvenationSetting AllowGuessRejuvenateHOME { get; set; } = EntityRejuvenationSetting.MissingDataHOME;

    [LocalizedDescription("Default version to set when transferring from Generation 1 3DS Virtual Console to Generation 7.")]
    public GameVersion VirtualConsoleSourceGen1 { get; set; } = GameVersion.RD;

    [LocalizedDescription("Default version to set when transferring from Generation 2 3DS Virtual Console to Generation 7.")]
    public GameVersion VirtualConsoleSourceGen2 { get; set; } = GameVersion.SI;

    [LocalizedDescription("Retain the Met Date when transferring from Generation 4 to Generation 5.")]
    public bool RetainMetDateTransfer45 { get; set; }
}

public sealed class AdvancedSettings
{
    [LocalizedDescription("Folder path that contains dump(s) of block hash-names. If a specific dump file does not exist, only names defined within the program's code will be loaded.")]
    public string PathBlockKeyList { get; set; } = string.Empty;

    [LocalizedDescription("Hide event variables below this event type value. Removes event values from the GUI that the user doesn't care to view.")]
    public NamedEventType HideEventTypeBelow { get; set; }

    [LocalizedDescription("Hide event variable names for that contain any of the comma-separated substrings below. Removes event values from the GUI that the user doesn't care to view.")]
    public string HideEvent8Contains { get; set; } = string.Empty;

    [Browsable(false)]
    public string[] GetExclusionList8() => Array.ConvertAll(HideEvent8Contains.Split(',', StringSplitOptions.RemoveEmptyEntries), z => z.Trim());
}

public sealed class EntityDatabaseSettings
{
    [LocalizedDescription("When loading content for the PKM Database, search within backup save files.")]
    public bool SearchBackups { get; set; } = true;

    [LocalizedDescription("When loading content for the PKM Database, search within OtherBackupPaths.")]
    public bool SearchExtraSaves { get; set; } = true;

    [LocalizedDescription("When loading content for the PKM Database, search subfolders within OtherBackupPaths.")]
    public bool SearchExtraSavesDeep { get; set; } = true;

    [LocalizedDescription("When loading content for the PKM database, the list will be ordered by this option.")]
    public DatabaseSortMode InitialSortMode { get; set; }

    [LocalizedDescription("Hides unavailable Species if the currently loaded save file cannot import them.")]
    public bool FilterUnavailableSpecies { get; set; } = true;
}

public enum DatabaseSortMode
{
    None,
    SpeciesForm,
    SlotIdentity,
}

public sealed class EntityEditorSettings
{
    [LocalizedDescription("When changing the Hidden Power type, automatically maximize the IVs to ensure the highest Base Power result. Otherwise, keep the IVs as close as possible to the original.")]
    public bool HiddenPowerOnChangeMaxPower { get; set; } = true;

    [LocalizedDescription("When showing the list of balls to select, show the legal balls before the illegal balls rather than sorting by Ball ID.")]
    public bool ShowLegalBallsFirst { get; set; } = true;

    [LocalizedDescription("When showing a Generation 1 format entity, show the gender it would have if transferred to other generations.")]
    public bool ShowGenderGen1 { get; set; }

    [LocalizedDescription("When showing an entity, show any stored Status Condition (Sleep/Burn/etc) it may have.")]
    public bool ShowStatusCondition { get; set; } = true;
}

public sealed class EncounterDatabaseSettings
{
    [LocalizedDescription("Skips searching if the user forgot to enter Species / Move(s) into the search criteria.")]
    public bool ReturnNoneIfEmptySearch { get; set; } = true;

    [LocalizedDescription("Hides unavailable Species if the currently loaded save file cannot import them.")]
    public bool FilterUnavailableSpecies { get; set; } = true;

    [LocalizedDescription("Use properties from the PKM Editor tabs to specify criteria like Gender and Nature when generating an encounter.")]
    public bool UseTabsAsCriteria { get; set; } = true;

    [LocalizedDescription("Use properties from the PKM Editor tabs even if the new encounter isn't the same evolution chain.")]
    public bool UseTabsAsCriteriaAnySpecies { get; set; } = true;
}

public sealed class MysteryGiftDatabaseSettings
{
    [LocalizedDescription("Hides gifts if the currently loaded save file cannot (indirectly) receive them.")]
    public bool FilterUnavailableSpecies { get; set; } = true;
}

public sealed class ReportGridSettings
{
    [LocalizedDescription("Extra entity properties to try and show in addition to the default properties displayed.")]
    public List<string> ExtraProperties { get; set; } = [];

    [LocalizedDescription("Properties to hide from the report grid.")]
    public List<string> HiddenProperties { get; set; } = [];
}

public sealed class HoverSettings
{
    [LocalizedDescription("Show PKM Slot Preview on Hover")]
    public bool HoverSlotShowPreview { get; set; } = true;

    [LocalizedDescription("Show Encounter Info on Hover")]
    public bool HoverSlotShowEncounter { get; set; } = true;

    [LocalizedDescription("Show all Encounter Info properties on Hover")]
    public bool HoverSlotShowEncounterVerbose { get; set; }

    [LocalizedDescription("Show PKM Slot ToolTip on Hover")]
    public bool HoverSlotShowText { get; set; } = true;

    [LocalizedDescription("Play PKM Slot Cry on Hover")]
    public bool HoverSlotPlayCry { get; set; } = true;

    [LocalizedDescription("Show a Glow effect around the PKM on Hover")]
    public bool HoverSlotGlowEdges { get; set; } = true;

    [LocalizedDescription("Show Showdown Paste in special Preview on Hover")]
    public bool PreviewShowPaste { get; set; } = true;

    [LocalizedDescription("Show a Glow effect around the PKM on Hover")]
    public Point PreviewCursorShift { get; set; } = new(16, 8);
}

public sealed class SoundSettings
{
    [LocalizedDescription("Play Sound when loading a new Save File")]
    public bool PlaySoundSAVLoad { get; set; } = true;
    [LocalizedDescription("Play Sound when popping up Legality Report")]
    public bool PlaySoundLegalityCheck { get; set; } = true;
}

public sealed class SetImportSettings
{
    [LocalizedDescription("Apply StatNature to Nature on Import")]
    public bool ApplyNature { get; set; } = true;
    [LocalizedDescription("Apply Markings on Import")]
    public bool ApplyMarkings { get; set; } = true;
}

public sealed class SlotWriteSettings
{
    [LocalizedDescription("Automatically modify the Save File's Pokédex when injecting a PKM.")]
    public bool SetUpdateDex { get; set; } = true;

    [LocalizedDescription("Automatically adapt the PKM Info to the Save File (Handler, Format)")]
    public bool SetUpdatePKM { get; set; } = true;

    [LocalizedDescription("Automatically increment the Save File's counters for obtained Pokémon (eggs/captures) when injecting a PKM.")]
    public bool SetUpdateRecords { get; set; } = true;

    [LocalizedDescription("When enabled and closing/loading a save file, the program will alert if the current save file has been modified without saving.")]
    public bool ModifyUnset { get; set; } = true;
}

public sealed class DisplaySettings
{
    [LocalizedDescription("Show Unicode gender symbol characters, or ASCII when disabled.")]
    public bool Unicode { get; set; } = true;

    [LocalizedDescription("Don't show the Legality popup if Legal!")]
    public bool IgnoreLegalPopup { get; set; }

    [LocalizedDescription("Display all properties of the encounter (auto-generated) when exporting a verbose report.")]
    public bool ExportLegalityVerboseProperties { get; set; }

    [LocalizedDescription("Flag Illegal Slots in Save File")]
    public bool FlagIllegal { get; set; } = true;

    [LocalizedDescription("Focus border indentation for custom drawn image controls.")]
    public int FocusBorderDeflate { get; set; } = 1;

    [LocalizedDescription("Disables the GUI scaling based on Dpi on program startup, falling back to font scaling.")]
    public bool DisableScalingDpi { get; set; }
}

public sealed class SpriteSettings : ISpriteSettings
{
    [LocalizedDescription("Choice for which sprite building mode to use.")]
    public SpriteBuilderPreference SpritePreference { get; set; } = SpriteBuilderPreference.UseSuggested;

    [LocalizedDescription("Show fan-made shiny sprites when the PKM is shiny.")]
    public bool ShinySprites { get; set; } = true;

    [LocalizedDescription("Show an Egg Sprite As Held Item rather than hiding the PKM")]
    public bool ShowEggSpriteAsHeldItem { get; set; } = true;

    [LocalizedDescription("Show the required ball for an Encounter Template")]
    public bool ShowEncounterBall { get; set; } = true;

    [LocalizedDescription("Show a background to differentiate an Encounter Template's type")]
    public SpriteBackgroundType ShowEncounterColor { get; set; } = SpriteBackgroundType.FullBackground;

    [LocalizedDescription("Show a background to differentiate the recognized Encounter Template type for PKM slots")]
    public SpriteBackgroundType ShowEncounterColorPKM { get; set; }

    [LocalizedDescription("Opacity for the Encounter Type background layer.")]
    public byte ShowEncounterOpacityBackground { get; set; } = 0x3F; // kinda low

    [LocalizedDescription("Opacity for the Encounter Type stripe layer.")]
    public byte ShowEncounterOpacityStripe { get; set; } = 0x5F; // 0xFF opaque

    [LocalizedDescription("Amount of pixels thick to show when displaying the encounter type color stripe.")]
    public int ShowEncounterThicknessStripe { get; set; } = 4; // pixels

    [LocalizedDescription("Show a thin stripe to indicate the percent of level-up progress")]
    public bool ShowExperiencePercent { get; set; }

    [LocalizedDescription("Show a background to differentiate the Tera Type for PKM slots")]
    public SpriteBackgroundType ShowTeraType { get; set; } = SpriteBackgroundType.BottomStripe;

    [LocalizedDescription("Amount of pixels thick to show when displaying the Tera Type color stripe.")]
    public int ShowTeraThicknessStripe { get; set; } = 4; // pixels

    [LocalizedDescription("Opacity for the Tera Type background layer.")]
    public byte ShowTeraOpacityBackground { get; set; } = 0xFF; // 0xFF opaque

    [LocalizedDescription("Opacity for the Tera Type stripe layer.")]
    public byte ShowTeraOpacityStripe { get; set; } = 0xAF; // 0xFF opaque
}

public sealed class PrivacySettings
{
    [LocalizedDescription("Hide Save File Details in Program Title")]
    public bool HideSAVDetails { get; set; }

    [LocalizedDescription("Hide Secret Details in Editors")]
    public bool HideSecretDetails { get; set; }
}

public sealed class SaveLanguageSettings
{
    [LocalizedDescription("Gen1: If unable to detect a language or version for a save file, use these instead.")]
    public LangVersion OverrideGen1 { get; set; } = new();

    [LocalizedDescription("Gen2: If unable to detect a language or version for a save file, use these instead.")]
    public LangVersion OverrideGen2 { get; set; } = new();

    [LocalizedDescription("Gen3 R/S: If unable to detect a language or version for a save file, use these instead.")]
    public LangVersion OverrideGen3RS { get; set; } = new();

    [LocalizedDescription("Gen3 FR/LG: If unable to detect a language or version for a save file, use these instead.")]
    public LangVersion OverrideGen3FRLG { get; set; } = new();

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed record LangVersion
    {
        public LanguageID Language { get; set; } = LanguageID.English;
        public GameVersion Version { get; set; }
    }

    public void Apply()
    {
        SaveLanguage.OverrideLanguageGen1 = OverrideGen1.Language;
        if (GameVersion.RBY.Contains(OverrideGen1.Version))
            SaveLanguage.OverrideVersionGen1 = OverrideGen1.Version;

        SaveLanguage.OverrideLanguageGen2 = OverrideGen2.Language;
        if (GameVersion.GS.Contains(OverrideGen2.Version))
            SaveLanguage.OverrideVersionGen2 = OverrideGen2.Version;

        SaveLanguage.OverrideLanguageGen3RS = OverrideGen3RS.Language;
        if (GameVersion.RS.Contains(OverrideGen3RS.Version))
            SaveLanguage.OverrideVersionGen3RS = OverrideGen3RS.Version;

        SaveLanguage.OverrideLanguageGen3FRLG = OverrideGen3FRLG.Language;
        if (GameVersion.FRLG.Contains(OverrideGen3FRLG.Version))
            SaveLanguage.OverrideVersionGen3FRLG = OverrideGen3FRLG.Version;
    }
}

public sealed class SlotExportSettings
{
    [LocalizedDescription("Settings to use for box exports.")]
    public BoxExportSettings BoxExport { get; set; } = new();

    [LocalizedDescription("Selected File namer to use for box exports for the GUI, if multiple are available.")]
    public string DefaultBoxExportNamer { get; set; } = "";

    [LocalizedDescription("Allow drag and drop of boxdata binary files from the GUI via the Box tab.")]
    public bool AllowBoxDataDrop { get; set; } // default to false, clunky to use
}
