using Avalonia.Controls;

namespace PKHeX.Avalonia.Views;

/// <summary>
/// Layout choices that differ between the desktop and the touch host, exposed for
/// <c>{x:Static}</c> use from shared views.
/// </summary>
/// <remarks>
/// A style setter cannot be used for these: the shared views set the property locally in XAML, and
/// a local value outranks a style in Avalonia's value precedence. Binding the property to a static
/// keeps one definition for both hosts instead of forking the view.
/// </remarks>
public static class PlatformLayout
{
    /// <summary>True on the touch-first host (Android).</summary>
    public static bool IsTouchPrimary { get; } = OperatingSystem.IsAndroid();

    /// <summary>
    /// Where the entity editor's section tabs (Main/Stats/Met/...) sit. A vertical strip is fine on
    /// a wide desktop window, but on a phone it eats about a third of the width and pushes the
    /// field grids off screen, so the tabs go across the top there.
    /// </summary>
    public static Dock EditorTabStripPlacement { get; } = IsTouchPrimary ? Dock.Top : Dock.Left;
}
