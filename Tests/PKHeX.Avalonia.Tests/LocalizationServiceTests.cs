using System.ComponentModel;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using PKHeX.Avalonia.Localization;
using PKHeX.Presentation.Localization;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioural tests for the UI localization service (issue #132): resource-file completeness,
/// English fallback, and the live-switch change notification that the XAML markup extension relies
/// on. File-parity is validated against the on-disk JSON so a translator dropping/adding a key is
/// caught in CI.
/// </summary>
public class LocalizationServiceTests
{
    private static readonly string[] Languages = ["en", "ja", "fr", "it", "de", "es", "ko", "zh-Hans", "zh-Hant"];

    private static string StringsDir()
    {
        var dir = AppContext.BaseDirectory;
        while (!Directory.Exists(Path.Combine(dir, "PKHeX.Presentation")))
        {
            var parent = Directory.GetParent(dir) ?? throw new DirectoryNotFoundException("repo root");
            dir = parent.FullName;
        }
        return Path.Combine(dir, "PKHeX.Presentation", "Localization", "Strings");
    }

    private static Dictionary<string, string> LoadFile(string code)
    {
        var path = Path.Combine(StringsDir(), $"{code}.json");
        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var p in doc.RootElement.EnumerateObject())
            map[p.Name] = p.Value.GetString() ?? "";
        return map;
    }

    [Fact]
    public void AllNineLanguageFiles_Exist_And_Are_KeyComplete_Against_English()
    {
        var english = LoadFile("en");
        Assert.NotEmpty(english);

        foreach (var code in Languages)
        {
            var map = LoadFile(code);
            var missing = english.Keys.Except(map.Keys).ToList();
            var extra = map.Keys.Except(english.Keys).ToList();
            Assert.True(missing.Count == 0, $"{code}.json is missing keys: {string.Join(", ", missing)}");
            Assert.True(extra.Count == 0, $"{code}.json has unknown keys: {string.Join(", ", extra)}");
        }
    }

    [Fact]
    public void Translations_Preserve_Format_Placeholders()
    {
        var english = LoadFile("en");
        var keysWithPlaceholder = english.Where(kv => kv.Value.Contains("{0}")).Select(kv => kv.Key).ToList();
        Assert.NotEmpty(keysWithPlaceholder); // sanity: en has placeholders (e.g. Status_GameFormat)

        foreach (var code in Languages.Where(c => c != "en"))
        {
            var map = LoadFile(code);
            foreach (var key in keysWithPlaceholder)
                Assert.True(map[key].Contains("{0}"), $"{code}.json key '{key}' dropped the {{0}} placeholder.");
        }
    }

    [Fact]
    public void Indexer_Switches_Value_Live_And_Falls_Back_To_English()
    {
        var loc = LocalizedStrings.Instance;
        try
        {
            loc.SetLanguage("en");
            var englishFile = LoadFile("en");
            var sampleKey = "Menu_File_Save";
            Assert.Equal(englishFile[sampleKey], loc[sampleKey]);

            // Switch to Japanese: the same key resolves to the Japanese value.
            loc.SetLanguage("ja");
            Assert.Equal(LoadFile("ja")[sampleKey], loc[sampleKey]);
            Assert.Equal("ja", loc.CurrentLanguage);

            // Unknown key resolves to the key itself (visible marker, never blank).
            Assert.Equal("____NoSuchKey____", loc["____NoSuchKey____"]);

            // Unknown language falls back to English.
            loc.SetLanguage("xx-Unknown");
            Assert.Equal("en", loc.CurrentLanguage);
            Assert.Equal(englishFile[sampleKey], loc[sampleKey]);
        }
        finally
        {
            loc.SetLanguage("en");
        }
    }

    [AvaloniaFact]
    public void LocExtension_Binding_Renders_And_Updates_On_Language_Switch()
    {
        var loc = LocalizedStrings.Instance;
        try
        {
            // Exercise the exact Binding the XAML {loc:Loc Key} markup extension produces.
            var binding = (Binding)new LocExtension("Menu_File_Save").ProvideValue(null!);
            var textBlock = new TextBlock();
            textBlock.Bind(TextBlock.TextProperty, binding);

            loc.SetLanguage("en");
            Assert.Equal(LoadFile("en")["Menu_File_Save"], textBlock.Text);

            // Live switch: the same control now shows the German string, no reload.
            loc.SetLanguage("de");
            Assert.Equal(LoadFile("de")["Menu_File_Save"], textBlock.Text);
            Assert.NotEqual(LoadFile("en")["Menu_File_Save"], textBlock.Text);
        }
        finally
        {
            loc.SetLanguage("en");
        }
    }

    [Fact]
    public void SetLanguage_Raises_ChangeNotification_For_Bindings()
    {
        var loc = LocalizedStrings.Instance;
        var raised = new List<string?>();
        PropertyChangedEventHandler handler = (_, e) => raised.Add(e.PropertyName);
        loc.PropertyChanged += handler;
        try
        {
            loc.SetLanguage("fr");
            // An empty/"Item[]" notification is what makes every {loc:Loc} binding re-evaluate.
            Assert.Contains(raised, n => string.IsNullOrEmpty(n) || n == "Item[]");
        }
        finally
        {
            loc.PropertyChanged -= handler;
            loc.SetLanguage("en");
        }
    }
}
