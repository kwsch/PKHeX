using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Avalonia;

namespace PKHeX.Avalonia.Settings;

[JsonSerializable(typeof(PKHeXSettings))]
public sealed partial class PKHeXSettingsContext : JsonSerializerContext;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class PKHeXSettings : IProgramSettings
{
    public StartupSettings Startup { get; set; } = new();
    IStartupSettings IProgramSettings.Startup => Startup;
    public BackupSettings Backup { get; set; } = new();
    public LocalResourceSettings LocalResources { get; set; } = new();

    public LegalitySettings Legality { get; set; } = new();
    public EntityConverterSettings Converter { get; set; } = new();
    public SetImportSettings Import { get; set; } = new();
    public SlotWriteSettings SlotWrite { get; set; } = new();
    public PrivacySettings Privacy { get; set; } = new();
    public SaveLanguageSettings SaveLanguage { get; set; } = new();

    public DisplaySettings Display { get; set; } = new();
    public SpriteSettings Sprite { get; set; } = new();
    public SoundSettings Sounds { get; set; } = new();
    public HoverSettings Hover { get; set; } = new();
    public BattleTemplateSettings BattleTemplate { get; set; } = new();

    public DrawConfig Draw { get; set; } = new();
    public AdvancedSettings Advanced { get; set; } = new();
    public EntityEditorSettings EntityEditor { get; set; } = new();
    public EntityDatabaseSettings EntityDb { get; set; } = new();
    public EncounterDatabaseSettings EncounterDb { get; set; } = new();
    public MysteryGiftDatabaseSettings MysteryDb { get; set; } = new();
    public ReportGridSettings Report { get; set; } = new();

    [JsonIgnore]
    public SlotExportSettings SlotExport { get; set; } = new();

    public static JsonSerializerOptions SerializerOptions { get; } = new()
    {
        WriteIndented = true,
        TypeInfoResolver = PKHeXSettingsContext.Default,
    };

    private static PKHeXSettingsContext GetContext() => new(new()
    {
        WriteIndented = true,
    });

    public static PKHeXSettings GetSettings(string configPath)
    {
        if (!File.Exists(configPath))
            return new PKHeXSettings();
        try
        {
            var lines = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize(lines, GetContext().PKHeXSettings) ?? new PKHeXSettings();
        }
        catch (Exception)
        {
            return new PKHeXSettings();
        }
    }

    public static async Task SaveSettings(string configPath, PKHeXSettings cfg)
    {
        try
        {
            await using var fs = File.Create(configPath);
            await JsonSerializer.SerializeAsync(fs, cfg, GetContext().PKHeXSettings).ConfigureAwait(false);
        }
        catch
        {
            // Silently fail on settings save
        }
    }
}
