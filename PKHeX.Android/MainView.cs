using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Android.Util;

namespace PKHeX.Android;

/// <summary>Adapts the full PKHeX Avalonia window content to Android's single-view lifetime.</summary>
public sealed class MainView : UserControl
{
    private readonly Grid _mainRoot;
    private IInsetsManager? _insets;

    public Panel Overlay { get; } = new()
    {
        IsHitTestVisible = false,
    };

    /// <summary>Whether a tool/dialog overlay currently owns the view (drives Back handling).</summary>
    public bool IsOverlayVisible => ReferenceEquals(Content, Overlay);

    public MainView(Control content)
    {
        Log.Info("PKHEX_ANDROID", "MainView: start");
        _mainRoot = new Grid();
        _mainRoot.Children.Add(content);
        Content = _mainRoot;
        Log.Info("PKHEX_ANDROID", "MainView: content assigned");
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Draw behind the status/navigation bars like a modern Android app, then keep the content
        // inside the safe area so the menu row is not under the status bar and the bottom row is
        // not under the gesture handle.
        _insets = TopLevel.GetTopLevel(this)?.InsetsManager;
        if (_insets is null)
            return;

        _insets.DisplayEdgeToEdgePreference = true;
        _insets.SafeAreaChanged += OnSafeAreaChanged;
        ApplySafeArea(_insets.SafeAreaPadding);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_insets is not null)
        {
            _insets.SafeAreaChanged -= OnSafeAreaChanged;
            _insets = null;
        }
        base.OnDetachedFromVisualTree(e);
    }

    private void OnSafeAreaChanged(object? sender, SafeAreaChangedArgs e) => ApplySafeArea(e.SafeAreaPadding);

    private void ApplySafeArea(Thickness padding)
    {
        Padding = padding;
        Log.Info("PKHEX_ANDROID", $"MainView: safe area {padding}");
    }

    /// <summary>
    /// Gives Android's single native input connection exclusively to the active tool/dialog.
    /// </summary>
    public void ShowOverlay()
    {
        Overlay.IsHitTestVisible = true;
        Content = Overlay;
    }

    /// <summary>Restores the full editor once all Android-hosted tools/dialogs have closed.</summary>
    public void ShowMainContent()
    {
        Overlay.IsHitTestVisible = false;
        Content = _mainRoot;
    }
}
