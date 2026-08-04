namespace PKHeX.Application.Abstractions;

/// <summary>
/// Framework-free dialog/file services: native pickers (returning paths) and message boxes.
/// Showing a ViewModel as a window is handled separately by <see cref="IWindowService"/>.
/// </summary>
public interface IDialogService
{
    Task<string?> OpenFileAsync(string title, string[]? filters = null);
    Task<string?> OpenFolderAsync(string title);
    Task<string?> SaveFileAsync(string title, string? defaultFileName = null, string[]? filters = null);
    Task ShowErrorAsync(string title, string message);
    Task ShowInformationAsync(string title, string message);

    /// <summary>
    /// Shows a modal confirm/cancel prompt. Returns <see langword="true"/> only when the user
    /// explicitly confirms. Used to gate destructive actions such as writing to a live console.
    /// </summary>
    Task<bool> ShowConfirmationAsync(string title, string message, string confirmText = "Yes", string cancelText = "Cancel");

    /// <summary>Opens the OS file manager (Explorer/Finder/xdg-open) and highlights the given file, if possible.</summary>
    void RevealInFileManager(string path);

    Task<string?> GetClipboardTextAsync();
    Task SetClipboardTextAsync(string text);
}
