using Android.Util;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using PKHeX.Application.Abstractions;

namespace PKHeX.Android;

/// <summary>
/// Hosts the full PKHeX ViewLocator graph in an in-app overlay because Android's Avalonia backend
/// exposes a single view and intentionally does not implement desktop <see cref="Window"/>.
/// </summary>
public sealed class AndroidWindowService : IWindowService
{
    private readonly Dictionary<object, Border> _tools = new();

    /// <summary>
    /// Close actions for every open overlay, newest last, so the Back gesture can dismiss the
    /// top-most one instead of leaving the activity.
    /// </summary>
    private readonly List<Action> _openOverlays = [];

    public AndroidWindowService() => AndroidHostContext.SetWindowService(this);

    /// <summary>
    /// Dismisses the top-most overlay. Returns false when none is open, so the caller can fall
    /// back to Android's default Back behaviour.
    /// </summary>
    public bool TryCloseTopOverlay()
    {
        if (_openOverlays.Count == 0)
            return false;

        var close = _openOverlays[^1];
        close();
        return true;
    }

    public Task ShowDialogAsync(object viewModel, string title)
    {
        var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        RunOnUiThread(() =>
        {
            var root = AndroidHostContext.MainView;
            if (root is null)
            {
                completion.TrySetResult(true);
                return;
            }

            try
            {
                Border? overlay = null;
                Action close = () => CloseOverlay(root, overlay, viewModel, completion);
                overlay = CreateOverlay(title, global::PKHeX.Avalonia.ViewLocator.Build(viewModel), close);
                if (viewModel is ICloseableDialog closeable)
                    closeable.CloseRequested = close;
                _openOverlays.Add(close);
                root.Overlay.Children.Add(overlay);
                root.ShowOverlay();
            }
            catch (Exception ex)
            {
                Log.Error("PKHEX_ANDROID", $"Dialog view failed for {viewModel.GetType().Name}: {ex}");
                completion.TrySetException(ex);
            }
        });

        return completion.Task;
    }

    public void ShowTool(object viewModel, string title)
    {
        RunOnUiThread(() =>
        {
            var root = AndroidHostContext.MainView;
            if (root is null || _tools.ContainsKey(viewModel))
                return;

            try
            {
                Border? overlay = null;
                Action close = () => CloseTool(root, viewModel, overlay);
                overlay = CreateOverlay(title, global::PKHeX.Avalonia.ViewLocator.Build(viewModel), close);
                if (viewModel is ICloseableDialog closeable)
                    closeable.CloseRequested = close;
                _tools.Add(viewModel, overlay);
                _openOverlays.Add(close);
                root.Overlay.Children.Add(overlay);
                root.ShowOverlay();
            }
            catch (Exception ex)
            {
                Log.Error("PKHEX_ANDROID", $"Tool view failed for {viewModel.GetType().Name}: {ex}");
            }
        });
    }

    public void CloseAllTools()
    {
        RunOnUiThread(() =>
        {
            var root = AndroidHostContext.MainView;
            if (root is null)
                return;

            foreach (var pair in _tools.ToArray())
            {
                root.Overlay.Children.Remove(pair.Value);
                if (pair.Key is ICloseableDialog closeable)
                    closeable.CloseRequested = null;
            }

            _tools.Clear();
            _openOverlays.Clear();
            UpdateOverlayHitTesting(root);
        });
    }

    private static Border CreateOverlay(string title, Control content, Action close)
    {
        var closeButton = new global::Avalonia.Controls.Button
        {
            Content = "×",
            Width = 44,
            Height = 40,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
        };
        closeButton.Click += (_, _) => close();

        var header = new Grid { Height = 48 };
        header.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new global::Avalonia.Thickness(4, 0, 52, 0),
            TextWrapping = TextWrapping.Wrap,
        });
        header.Children.Add(closeButton);

        var body = new ScrollViewer
        {
            Content = content,
            IsHitTestVisible = true,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
        };
        content.IsHitTestVisible = true;
        var card = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#242424")),
            BorderBrush = new SolidColorBrush(Color.Parse("#606060")),
            BorderThickness = new global::Avalonia.Thickness(1),
            CornerRadius = new global::Avalonia.CornerRadius(8),
            Padding = new global::Avalonia.Thickness(12),
            MaxWidth = 980,
            MaxHeight = 1600,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsHitTestVisible = true,
            Child = new DockPanel
            {
                LastChildFill = true,
                Children =
                {
                    header,
                    body,
                },
            },
        };
        DockPanel.SetDock(header, Dock.Top);

        var overlay = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(190, 0, 0, 0)),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsHitTestVisible = true,
            Padding = new global::Avalonia.Thickness(12),
            Child = card,
        };

        // Android exposes one native Avalonia input connection for the single-view host. When a
        // popup-like overlay is added above an already focused editor, the pointer hit can land on
        // the overlay while the native text connection still points at the old TextBox. Reassert
        // focus on the actual TextBox under the pointer during the tunnel phase so Android IME and
        // adb/physical keyboard input follow the visible control.
        overlay.AddHandler(InputElement.PointerPressedEvent, (_, args) =>
        {
            for (var visual = args.Source as Visual; visual is not null; visual = visual.GetVisualParent())
            {
                if (visual is TextBox textBox)
                {
                    textBox.Focus();
                    break;
                }

                if (ReferenceEquals(visual, overlay))
                    break;
            }
        }, RoutingStrategies.Tunnel);

        return overlay;
    }

    private void CloseOverlay(MainView root, Border? overlay, object viewModel, TaskCompletionSource<bool> completion)
    {
        RunOnUiThread(() =>
        {
            if (overlay is not null)
                root.Overlay.Children.Remove(overlay);
            if (viewModel is ICloseableDialog closeable)
                closeable.CloseRequested = null;
            PopOverlay(overlay);
            UpdateOverlayHitTesting(root);
            completion.TrySetResult(true);
        });
    }

    private void CloseTool(MainView root, object viewModel, Border? overlay)
    {
        RunOnUiThread(() =>
        {
            if (overlay is not null)
                root.Overlay.Children.Remove(overlay);
            _tools.Remove(viewModel);
            if (viewModel is ICloseableDialog closeable)
                closeable.CloseRequested = null;
            PopOverlay(overlay);
            UpdateOverlayHitTesting(root);
        });
    }

    /// <summary>Drops the close action of an overlay that has just been removed.</summary>
    private void PopOverlay(Border? overlay)
    {
        if (overlay is not null && _openOverlays.Count > 0)
            _openOverlays.RemoveAt(_openOverlays.Count - 1);
    }

    private static void UpdateOverlayHitTesting(MainView root)
    {
        if (root.Overlay.Children.Count > 0)
            root.ShowOverlay();
        else
            root.ShowMainContent();
    }

    private static void RunOnUiThread(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
            action();
        else
            Dispatcher.UIThread.Post(action);
    }
}
