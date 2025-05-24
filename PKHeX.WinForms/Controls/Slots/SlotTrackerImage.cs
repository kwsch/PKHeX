using System.Drawing;

namespace PKHeX.WinForms.Controls;

/// <summary>
/// Tracks the background images for a slot, including the original and current backgrounds.
/// </summary>
public sealed class SlotTrackerImage
{
    /// <summary>
    /// Gets or sets the original background image of the slot.
    /// </summary>
    public Image? OriginalBackground { get; set; }
    /// <summary>
    /// Gets or sets the current background image of the slot.
    /// </summary>
    public Image? CurrentBackground { get; set; }

    /// <summary>
    /// Resets the background images to null.
    /// </summary>
    public void Reset()
    {
        OriginalBackground = CurrentBackground = null;
    }
}
