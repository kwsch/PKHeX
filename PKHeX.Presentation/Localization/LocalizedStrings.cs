using System.ComponentModel;
using System.Text.Json;

namespace PKHeX.Presentation.Localization;

/// <summary>
/// Runtime store for the Avalonia shell's own UI chrome strings (menus, buttons, labels, dialog
/// titles, status messages) — <b>not</b> PKHeX.Core game data (species/moves/etc.), which
/// <see cref="Core.GameInfo.Strings"/> already localizes independently.
/// </summary>
/// <remarks>
/// <para>
/// Design rationale (JSON dictionaries over .resx): the shell needs <i>live</i> language switching
/// bound from XAML. A JSON-backed <c>Dictionary&lt;string,string&gt;</c> exposed through a single
/// string indexer lets us invalidate every bound string at once by raising one
/// <see cref="INotifyPropertyChanged"/> notification for the indexer — .resx generates a
/// <c>static</c> accessor that does not raise change notifications and couples to
/// <see cref="System.Globalization.CultureInfo"/> in ways that are awkward to flip at runtime and to
/// bind from Avalonia. JSON files are also trivial for contributors/translators to edit, diff, and
/// fall back per-key.
/// </para>
/// <para>
/// This type is deliberately Avalonia-free (BCL only) so it can live in Presentation and be reused
/// directly by ViewModels (e.g. dialog/status strings). The XAML-facing markup extension lives in
/// the Avalonia host layer.
/// </para>
/// <para>
/// English (<c>en.json</c>) is the source of truth and is always loaded as the fallback table:
/// any key missing from the active language resolves to the English string; a key missing from
/// English too resolves to the key itself (so gaps are visible, never blank).
/// </para>
/// </remarks>
public sealed class LocalizedStrings : INotifyPropertyChanged
{
    /// <summary>Process-wide singleton the markup extension and ViewModels bind against.</summary>
    public static LocalizedStrings Instance { get; } = new();

    /// <summary>Language codes matching PKHeX.Core's supported data languages / the resource file names.</summary>
    public static readonly string[] SupportedLanguages =
        ["en", "ja", "fr", "it", "de", "es", "ko", "zh-Hans", "zh-Hant"];

    private readonly IReadOnlyDictionary<string, string> _english;
    private IReadOnlyDictionary<string, string> _active;
    private string _language = "en";

    public event PropertyChangedEventHandler? PropertyChanged;

    private LocalizedStrings()
    {
        _english = Load("en");
        _active = _english;
    }

    /// <summary>The currently active UI language code (one of <see cref="SupportedLanguages"/>).</summary>
    public string CurrentLanguage => _language;

    /// <summary>
    /// Resolves a UI string by key with English fallback. Bound from XAML as <c>[Key]</c> so that a
    /// single indexer invalidation refreshes every localized element on the screen.
    /// </summary>
    /// <summary>
    /// Suffix marking a touch-first wording of a key. Some strings name a desktop interaction the
    /// user cannot perform on a phone (e.g. "Set (Shift+Click)"); adding <c>&lt;key&gt;_Touch</c> to
    /// the resource files gives that key a touch-appropriate wording without forking the view.
    /// </summary>
    public const string TouchVariantSuffix = "_Touch";

    /// <summary>
    /// Whether <c>_Touch</c> wordings win over their desktop counterparts. Defaults to true on
    /// Android — the only touch-first host — and can be set by a host that knows better.
    /// </summary>
    public static bool PreferTouchVariants { get; set; } = OperatingSystem.IsAndroid();

    public string this[string key]
    {
        get
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (PreferTouchVariants)
            {
                var touchKey = key + TouchVariantSuffix;
                if (_active.TryGetValue(touchKey, out var touch))
                    return touch;
                if (_english.TryGetValue(touchKey, out var touchFallback))
                    return touchFallback;
            }

            if (_active.TryGetValue(key, out var value))
                return value;
            if (_english.TryGetValue(key, out var fallback))
                return fallback;
            return key; // visible marker for a missing key rather than a blank control
        }
    }

    /// <summary>
    /// Convenience for ViewModel code paths that need a formatted string (status/dialog messages).
    /// </summary>
    public string Format(string key, params object?[] args) => string.Format(this[key], args);

    /// <summary>
    /// Switches the active UI language and refreshes all bound strings live. Unknown codes fall back
    /// to English. Safe to call repeatedly (idempotent re-raise is harmless).
    /// </summary>
    public void SetLanguage(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode) || Array.IndexOf(SupportedLanguages, languageCode) < 0)
            languageCode = "en";

        _language = languageCode;
        _active = languageCode == "en" ? _english : Load(languageCode);

        // Empty name = "all properties changed": Avalonia's INPC binding plugin re-reads every
        // accessor on this source, including the string indexer used by every localized element.
        // "Item[]" is raised explicitly as well for binding stacks that key on the indexer name.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
    }

    private static IReadOnlyDictionary<string, string> Load(string languageCode)
    {
        var asm = typeof(LocalizedStrings).Assembly;
        var resourceName = $"PKHeX.Presentation.Localization.Strings.{languageCode}.json";
        using var stream = asm.GetManifestResourceStream(resourceName);
        if (stream is null)
            return new Dictionary<string, string>(StringComparer.Ordinal);

        using var doc = JsonDocument.Parse(stream);
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var property in doc.RootElement.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.String)
                map[property.Name] = property.Value.GetString() ?? string.Empty;
        }
        return map;
    }
}
