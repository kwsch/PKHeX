using Avalonia.Styling;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// Custom Avalonia <see cref="ThemeVariant"/> values beyond the built-in Light/Dark, used as
/// <c>ResourceDictionary.ThemeDictionaries</c> keys in Styles/Theme.axaml.
/// </summary>
public static class AppThemeVariants
{
    /// <summary>
    /// High-contrast palette. Inherits from Dark so any resource not explicitly overridden in the
    /// HighContrast dictionary still resolves to a sensible value.
    /// </summary>
    public static readonly ThemeVariant HighContrast = new("HighContrast", ThemeVariant.Dark);
}
