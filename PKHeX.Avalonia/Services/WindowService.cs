using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using PKHeX.Application.Abstractions;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// Shows a ViewModel as a modal dialog. The matching View is resolved via <see cref="ViewLocator"/>,
/// wrapped in a host <c>Window</c>, and shown over the main window — preserving the previous
/// modal/centered behavior while keeping View resolution out of the Presentation layer.
/// Also hosts modeless tool windows (see <see cref="ShowTool"/>).
/// </summary>
public sealed class WindowService : IWindowService
{
    // Open modeless tool windows, keyed by their ViewModel instance, so re-invoking focuses
    // the existing window instead of opening a duplicate.
    private readonly Dictionary<object, Window> _tools = new();

    // Remembered size/position per tool ViewModel type, so a reopened tool (even after the VM
    // is rebuilt on save change) returns to where the user last left it for this session.
    private static readonly Dictionary<string, (PixelPoint Position, double Width, double Height)> ToolBounds = new();

    public async Task ShowDialogAsync(object viewModel, string title)
    {
        var owner = MainWindow;
        if (owner is null) return;

        var dialog = new Window
        {
            Title = title,
            Content = ViewLocator.Build(viewModel),
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = true,
            MaxWidth = 1400,
            MaxHeight = 820,
        };

        if (viewModel is ICloseableDialog closeable)
            closeable.CloseRequested = dialog.Close;

        await dialog.ShowDialog(owner);
    }

    public void ShowTool(object viewModel, string title)
    {
        // Already open for this ViewModel? Bring it forward instead of duplicating.
        if (_tools.TryGetValue(viewModel, out var existing))
        {
            existing.Activate();
            return;
        }

        var owner = MainWindow;
        if (owner is null) return;

        var key = viewModel.GetType().FullName ?? viewModel.GetType().Name;
        var window = new Window
        {
            Title = title,
            Content = ViewLocator.Build(viewModel),
            CanResize = true,
            MaxWidth = 1400,
            MaxHeight = 820,
        };

        if (ToolBounds.TryGetValue(key, out var bounds))
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Position = bounds.Position;
            window.Width = bounds.Width;
            window.Height = bounds.Height;
        }
        else
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.SizeToContent = SizeToContent.WidthAndHeight;
        }

        if (viewModel is ICloseableDialog closeable)
            closeable.CloseRequested = window.Close;

        window.Closed += (_, _) =>
        {
            // Remember where the user left it (skip if minimized/zeroed).
            if (window.Width > 0 && window.Height > 0)
                ToolBounds[key] = (window.Position, window.Width, window.Height);
            _tools.Remove(viewModel);
        };

        _tools[viewModel] = window;
        window.Show(owner);
    }

    public void CloseAllTools()
    {
        // Copy first: Close fires Closed handlers that mutate _tools.
        foreach (var window in _tools.Values.ToList())
            window.Close();
        _tools.Clear();
    }

    private static Window? MainWindow =>
        (global::Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
}
