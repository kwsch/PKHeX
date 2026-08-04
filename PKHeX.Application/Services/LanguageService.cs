using System.ComponentModel;
using System.Globalization;
using PKHeX.Core;

namespace PKHeX.Application.Services;

/// <summary>
/// Application service that owns the ordered mutation of Core's global localization state
/// (<see cref="GameInfo.CurrentLanguage"/> / <see cref="GameInfo.Strings"/>). It is data-bound by the
/// language selector, so it implements <see cref="INotifyPropertyChanged"/> directly (BCL only — no
/// MVVM-framework dependency).
/// </summary>
public sealed class LanguageService : INotifyPropertyChanged
{
    private static readonly string[] SupportedLanguages = ["en", "ja", "fr", "it", "de", "es", "ko", "zh-Hans", "zh-Hant"];
    private static readonly string[] LanguageNames = ["English", "日本語", "Français", "Italiano", "Deutsch", "Español", "한국어", "简体中文", "繁體中文"];

    private string _currentLanguage = "en";

    public string CurrentLanguage
    {
        get => _currentLanguage;
        private set
        {
            if (_currentLanguage == value) return;
            _currentLanguage = value;
            OnPropertyChanged(nameof(CurrentLanguage));
        }
    }

    public IReadOnlyList<LanguageOption> AvailableLanguages { get; }

    public LanguageOption? CurrentLanguageOption
    {
        get => AvailableLanguages.FirstOrDefault(l => l.Code == CurrentLanguage);
        set
        {
            if (value is not null && value.Code != CurrentLanguage)
                SetLanguage(value.Code);
        }
    }

    public event Action? LanguageChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public LanguageService()
    {
        AvailableLanguages = SupportedLanguages
            .Select((code, i) => new LanguageOption(code, LanguageNames[i]))
            .ToList();
    }

    public void SetLanguage(string languageCode)
    {
        if (!SupportedLanguages.Contains(languageCode))
            languageCode = "en";

        CurrentLanguage = languageCode;
        OnPropertyChanged(nameof(CurrentLanguageOption));
        GameInfo.CurrentLanguage = languageCode;
        GameInfo.Strings = GameInfo.GetStrings(languageCode);
        ApplyCulture(languageCode);
        LanguageChanged?.Invoke();
    }

    /// <summary>
    /// Aligns the thread/process culture with the selected language so date and number formatting in
    /// the UI follows the same locale (issue #132 acceptance criterion). The nine supported codes are
    /// all valid .NET culture names (including <c>zh-Hans</c>/<c>zh-Hant</c>); unknown codes are
    /// ignored rather than throwing.
    /// </summary>
    private static void ApplyCulture(string languageCode)
    {
        try
        {
            var culture = CultureInfo.GetCultureInfo(languageCode);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
        catch (CultureNotFoundException)
        {
            // Leave the existing culture in place if the platform lacks this locale.
        }
    }
}

public record LanguageOption(string Code, string Name)
{
    public override string ToString() => Name;
}
