namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Presentation-layer notification that the display language changed. The Application
/// <c>LanguageService</c> raises a plain C# event; <c>MainWindowViewModel</c> relays it onto the
/// MVVM messenger so on-demand dialog ViewModels (e.g. the PKM database) can refresh.
/// </summary>
public record LanguageChangedMessage(string LanguageCode);
