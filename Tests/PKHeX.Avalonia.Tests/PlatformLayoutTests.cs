using Avalonia.Controls;
using PKHeX.Avalonia.Views;

namespace PKHeX.Avalonia.Tests;

public sealed class PlatformLayoutTests
{
    [Fact]
    public void Desktop_KeepsTheVerticalEditorTabStrip()
    {
        // The test host is not Android, so this asserts the desktop side of the switch: the entity
        // editor keeps its side tab strip. Flipping it would silently reshape every desktop editor.
        Assert.False(PlatformLayout.IsTouchPrimary);
        Assert.Equal(Dock.Left, PlatformLayout.EditorTabStripPlacement);
    }

    [Fact]
    public void TouchPrimary_AndLocalizationTouchVariants_AgreeOnTheHost()
    {
        // Both switches read the same platform fact; if they ever disagree, one host would get
        // touch wordings with a desktop layout or the reverse.
        Assert.Equal(PlatformLayout.IsTouchPrimary, Presentation.Localization.LocalizedStrings.PreferTouchVariants);
    }
}
