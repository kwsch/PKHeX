using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// Abstraction for dialog operations (file pickers, alerts, confirmations).
/// </summary>
public interface IDialogService
{
    Task<string?> OpenFileAsync(string title, IReadOnlyList<string>? filters = null);
    Task<string[]?> OpenFilesAsync(string title, IReadOnlyList<string>? filters = null);
    Task<string?> SaveFileAsync(string title, string defaultFileName, IReadOnlyList<string>? filters = null);
    Task<string?> OpenFolderAsync(string title);

    Task ShowAlertAsync(string title, string message);
    Task ShowErrorAsync(string title, string message);
    Task<bool> ShowConfirmAsync(string title, string message);
    Task<string?> ShowPromptAsync(string title, string message, string defaultValue = "");

    /// <summary>
    /// Shows a dialog with a list of choices and returns the selected index, or -1 if cancelled.
    /// </summary>
    Task<int> ShowSelectionAsync(string title, string message, IReadOnlyList<string> choices);
}
