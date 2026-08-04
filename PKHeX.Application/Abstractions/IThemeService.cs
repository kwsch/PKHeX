namespace PKHeX.Application.Abstractions;

/// <summary>
/// UI theme/appearance preference. Persisted via <c>AppSettings</c>.
/// </summary>
public enum AppTheme
{
    /// <summary>Current default appearance: dark, GitHub-dark-inspired palette.</summary>
    Dark,
    /// <summary>Light surfaces with dark text, tuned for WCAG AA contrast.</summary>
    Light,
    /// <summary>Maximum-contrast palette for accessibility.</summary>
    HighContrast,
    /// <summary>Tracks the OS light/dark preference live.</summary>
    System,
}

/// <summary>
/// Applies the active UI theme (colors/brushes exposed as resources) at runtime, so switching
/// requires no restart. Framework-free: the Avalonia-specific implementation lives in the host
/// project (see <c>PKHeX.Avalonia.Services.ThemeService</c>) and drives Avalonia's
/// <c>ThemeVariant</c>/<c>ThemeDictionaries</c> APIs, including tracking the platform's
/// light/dark preference for <see cref="AppTheme.System"/>.
/// </summary>
public interface IThemeService
{
    /// <summary>The currently-applied theme preference.</summary>
    AppTheme CurrentTheme { get; }

    /// <summary>Applies and persists the given theme preference immediately.</summary>
    void ApplyTheme(AppTheme theme);
}
