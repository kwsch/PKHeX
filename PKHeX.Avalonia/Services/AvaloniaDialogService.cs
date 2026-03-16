using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IDialogService"/> using platform storage APIs.
/// </summary>
public sealed class AvaloniaDialogService : IDialogService
{
    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }

    private static IStorageProvider? GetStorageProvider()
    {
        return GetMainWindow()?.StorageProvider;
    }

    public async Task<string?> OpenFileAsync(string title, IReadOnlyList<string>? filters = null)
    {
        var storage = GetStorageProvider();
        if (storage is null)
            return null;

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
        };

        if (filters is { Count: > 0 })
        {
            options.FileTypeFilter = filters
                .Select(f => new FilePickerFileType(f) { Patterns = [$"*.{f}"] })
                .ToList();
        }

        var result = await storage.OpenFilePickerAsync(options);
        return result.Count > 0 ? result[0].Path.LocalPath : null;
    }

    public async Task<string[]?> OpenFilesAsync(string title, IReadOnlyList<string>? filters = null)
    {
        var storage = GetStorageProvider();
        if (storage is null)
            return null;

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = true,
        };

        if (filters is { Count: > 0 })
        {
            options.FileTypeFilter = filters
                .Select(f => new FilePickerFileType(f) { Patterns = [$"*.{f}"] })
                .ToList();
        }

        var result = await storage.OpenFilePickerAsync(options);
        return result.Count > 0 ? result.Select(f => f.Path.LocalPath).ToArray() : null;
    }

    public async Task<string?> SaveFileAsync(string title, string defaultFileName, IReadOnlyList<string>? filters = null)
    {
        var storage = GetStorageProvider();
        if (storage is null)
            return null;

        var options = new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = defaultFileName,
        };

        if (filters is { Count: > 0 })
        {
            options.FileTypeChoices = filters
                .Select(f => new FilePickerFileType(f) { Patterns = [$"*.{f}"] })
                .ToList();
        }

        var result = await storage.SaveFilePickerAsync(options);
        return result?.Path.LocalPath;
    }

    public async Task<string?> OpenFolderAsync(string title)
    {
        var storage = GetStorageProvider();
        if (storage is null)
            return null;

        var result = await storage.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
        });

        return result.Count > 0 ? result[0].Path.LocalPath : null;
    }

    public async Task ShowAlertAsync(string title, string message)
    {
        var window = GetMainWindow();
        if (window is null)
            return;

        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new global::Avalonia.Thickness(20),
                Spacing = 10,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                    new Button { Content = "OK", HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center },
                },
            },
        };

        var button = ((StackPanel)dialog.Content).Children.OfType<Button>().First();
        button.Click += (_, _) => dialog.Close();
        await dialog.ShowDialog(window);
    }

    public Task ShowErrorAsync(string title, string message)
        => ShowAlertAsync(title, message);

    public async Task<bool> ShowConfirmAsync(string title, string message)
    {
        var window = GetMainWindow();
        if (window is null)
            return false;

        var result = false;
        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new global::Avalonia.Thickness(20),
                Spacing = 10,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                    new StackPanel
                    {
                        Orientation = global::Avalonia.Layout.Orientation.Horizontal,
                        HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center,
                        Spacing = 10,
                        Children =
                        {
                            new Button { Content = "Yes" },
                            new Button { Content = "No" },
                        },
                    },
                },
            },
        };

        var panel = ((StackPanel)dialog.Content).Children.OfType<StackPanel>().First();
        var yesBtn = panel.Children.OfType<Button>().First();
        var noBtn = panel.Children.OfType<Button>().Last();
        yesBtn.Click += (_, _) => { result = true; dialog.Close(); };
        noBtn.Click += (_, _) => { result = false; dialog.Close(); };
        await dialog.ShowDialog(window);
        return result;
    }

    public Task<string?> ShowPromptAsync(string title, string message, string defaultValue = "")
    {
        // Simplified prompt - full implementation would have a TextBox
        return Task.FromResult<string?>(defaultValue);
    }
}
