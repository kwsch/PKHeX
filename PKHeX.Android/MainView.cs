using Avalonia;
using Avalonia.Controls;
using Android.Util;

namespace PKHeX.Android;

/// <summary>Adapts the full PKHeX Avalonia window content to Android's single-view lifetime.</summary>
public sealed class MainView : UserControl
{
    private readonly Grid _mainRoot;

    public Panel Overlay { get; } = new()
    {
        IsHitTestVisible = false,
    };

    public MainView(Control content)
    {
        Log.Info("PKHEX_ANDROID", "MainView: start");
        _mainRoot = new Grid();
        _mainRoot.Children.Add(content);
        Content = _mainRoot;
        Log.Info("PKHEX_ANDROID", "MainView: content assigned");
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
