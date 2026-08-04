using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.ApplicationLifetimes;
using global::Avalonia.Input.Platform;
using global::Avalonia.Layout;
using global::Avalonia.Media;
using global::Avalonia.Platform.Storage;

namespace PKHeX.Avalonia.Services;

public sealed class DialogService : IDialogService
{
    private Window? GetMainWindow()
    {
        if (global::Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }

    /// <summary>
    /// Returns the window that should own a newly-shown dialog: the topmost active window if one
    /// exists, falling back to the main window. Using <see cref="GetMainWindow"/> unconditionally
    /// would parent dialogs raised while another modal is already open (e.g. an error surfaced from
    /// inside a tool window) to the main window instead of that modal, letting them appear behind it.
    /// </summary>
    private Window? GetActiveWindow()
    {
        if (global::Avalonia.Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return null;

        foreach (var window in desktop.Windows)
        {
            if (window.IsActive)
                return window;
        }

        return desktop.MainWindow;
    }

    public async Task<string?> OpenFileAsync(string title, string[]? filters = null)
    {
        var window = GetMainWindow();
        if (window is null) return null;

        var fileTypes = FileDialogFilterFactory.BuildOpenFileTypes(filters);

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = fileTypes
        };

        var result = await window.StorageProvider.OpenFilePickerAsync(options);
        var file = result.FirstOrDefault();

        if (file is null)
            return null;

        // On macOS, TryGetLocalPath may be needed
        if (file.TryGetLocalPath() is { } localPath)
            return localPath;

        // Fallback to Uri path
        return file.Path.LocalPath;
    }
    public async Task<string?> OpenFolderAsync(string title)
    {
        var window = GetMainWindow();
        if (window is null) return null;

        var options = new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        };

        var result = await window.StorageProvider.OpenFolderPickerAsync(options);
        var folder = result.FirstOrDefault();

        if (folder is null)
            return null;

        if (folder.TryGetLocalPath() is { } localPath)
            return localPath;

        return folder.Path.LocalPath;
    }

    public async Task<string?> SaveFileAsync(string title, string? defaultFileName = null, string[]? filters = null)
    {
        var window = GetMainWindow();
        if (window is null) return null;

        var options = new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = defaultFileName
        };

        var result = await window.StorageProvider.SaveFilePickerAsync(options);
        return result?.Path.LocalPath;
    }

    public async Task ShowErrorAsync(string title, string message) => await ShowMessageBoxAsync(title, message);
    public async Task ShowInformationAsync(string title, string message) => await ShowMessageBoxAsync(title, message);

    public async Task<bool> ShowConfirmationAsync(string title, string message, string confirmText = "Yes", string cancelText = "Cancel")
    {
        var window = GetActiveWindow();
        if (window is null) return false;

        var confirmButton = new Button { Content = confirmText, MinWidth = 90 };
        var cancelButton = new Button { Content = cancelText, MinWidth = 90 };

        var dialog = new Window
        {
            Title = title,
            Width = 440,
            MinHeight = 130,
            MaxWidth = 620,
            MaxHeight = 520,
            SizeToContent = SizeToContent.Height,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 16,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Spacing = 12,
                        Children = { cancelButton, confirmButton },
                    },
                },
            },
        };

        var result = false;
        confirmButton.Click += (_, _) => { result = true; dialog.Close(); };
        cancelButton.Click += (_, _) => { result = false; dialog.Close(); };

        // Esc cancels, Enter confirms the default action; focus starts on the
        // confirm button so keyboard/screen-reader users land somewhere meaningful.
        AttachDialogConventions(dialog, confirmButton, cancelButton);

        await dialog.ShowDialog(window);
        return result;
    }

    public void RevealInFileManager(string path)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{path}\"");
            }
            else if (OperatingSystem.IsMacOS())
            {
                System.Diagnostics.Process.Start("open", ["-R", path]);
            }
            else
            {
                // Linux/other: no universal "reveal and select" — open the containing folder instead.
                var folder = File.Exists(path) ? Path.GetDirectoryName(path) ?? path : path;
                System.Diagnostics.Process.Start("xdg-open", [folder]);
            }
        }
        catch
        {
            // Best effort — if the OS doesn't have the expected file manager binary, do nothing.
        }
    }

    private async Task ShowMessageBoxAsync(string title, string message)
    {
        var window = GetActiveWindow();
        if (window is null) return;

        var dialog = new Window
        {
            Title = title,
            // No fixed height — let the window size to content so long messages
            // (e.g. FR save error, file path info) are never cut off at the bottom.
            Width = 420,
            MinHeight = 120,
            MaxWidth = 600,
            MaxHeight = 500,
            SizeToContent = SizeToContent.Height,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap
                    },
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        MinWidth = 80
                    }
                }
            }
        };

        var button = ((StackPanel)dialog.Content).Children[1] as Button;
        button!.Click += (_, _) => dialog.Close();

        // Esc/Enter both dismiss this OK-only dialog; focus starts on the OK button.
        AttachDialogConventions(dialog, button!, button!);

        await dialog.ShowDialog(window);
    }

    /// <summary>
    /// Applies the app-wide modal dialog keyboard conventions: Esc triggers the
    /// cancel action (<see cref="Button.IsCancel"/>), Enter triggers the default/
    /// confirm action (<see cref="Button.IsDefault"/>), and initial focus lands on
    /// the default action button so keyboard/screen-reader users don't have to hunt
    /// for it. Avalonia restores focus to the invoking control when a modal
    /// <see cref="Window.ShowDialog(Window)"/> closes, so no explicit "focus returns
    /// to invoker" handling is needed here.
    /// </summary>
    private static void AttachDialogConventions(Window dialog, Button defaultButton, Button cancelButton)
    {
        defaultButton.IsDefault = true;
        cancelButton.IsCancel = true;
        dialog.Opened += (_, _) => defaultButton.Focus();
    }

    public async Task<string?> GetClipboardTextAsync()
    {
        var window = GetMainWindow();
        if (window?.Clipboard is { } clipboard)
            return await clipboard.TryGetTextAsync();
        return null;
    }

    public async Task SetClipboardTextAsync(string text)
    {
        var window = GetMainWindow();
        if (window?.Clipboard is { } clipboard)
            await clipboard.SetTextAsync(text);
    }
}
