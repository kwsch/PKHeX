using Avalonia.Styling;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// Avalonia-side implementation of <see cref="IThemeService"/>. Drives the app-wide
/// <see cref="global::Avalonia.Application.RequestedThemeVariant"/>, which every open window/dialog
/// inherits automatically and re-styles live — so switching needs no restart. "Follow system" is
/// implemented by handing control back to Avalonia (<see cref="ThemeVariant.Default"/>), which
/// tracks <see cref="global::Avalonia.Application.PlatformSettings"/> live on macOS, Windows, and
/// Linux desktop portals that expose a color-scheme preference.
/// </summary>
public sealed class ThemeService : IThemeService
{
    private readonly AppSettings _settings;
    private readonly ISettingsStore _settingsStore;

    public ThemeService(AppSettings settings, ISettingsStore settingsStore)
    {
        _settings = settings;
        _settingsStore = settingsStore;
    }

    public AppTheme CurrentTheme => _settings.Theme.Selected;

    /// <summary>Applies the persisted theme preference. Call once at startup, before the main window is created.</summary>
    public void Initialize() => ApplyThemeVariant(_settings.Theme.Selected);

    public void ApplyTheme(AppTheme theme)
    {
        _settings.Theme.Selected = theme;
        _settingsStore.Save(_settings);
        ApplyThemeVariant(theme);
    }

    private static void ApplyThemeVariant(AppTheme theme)
    {
        var app = global::Avalonia.Application.Current;
        if (app is null)
            return;

        app.RequestedThemeVariant = theme switch
        {
            AppTheme.Light => ThemeVariant.Light,
            AppTheme.Dark => ThemeVariant.Dark,
            AppTheme.HighContrast => AppThemeVariants.HighContrast,
            AppTheme.System => ThemeVariant.Default, // Avalonia tracks PlatformSettings live.
            _ => ThemeVariant.Dark,
        };
    }
}
