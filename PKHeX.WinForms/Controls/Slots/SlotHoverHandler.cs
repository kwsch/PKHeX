using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms.Controls;

/// <summary>
/// Handles Hovering operations for an editor, where only one (1) slot can be animated at a given time when hovering over it.
/// </summary>
public sealed class SlotHoverHandler : IDisposable
{
    /// <summary>
    /// Gets or sets the drawing configuration for the slot hover effect.
    /// </summary>
    public DrawConfig Draw { private get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the hover effect should display a glow.
    /// </summary>
    public bool GlowHover { private get; set; } = true;

    private readonly SummaryPreviewer Preview = new();
    private static Bitmap Hover => SpriteUtil.Spriter.Hover;

    private readonly BitmapAnimator HoverWorker = new();

    private PictureBox? Slot;
    private SlotTrackerImage? LastSlot;

    /// <summary>
    /// Starts the hover animation and preview for the specified slot.
    /// </summary>
    /// <param name="pb">The PictureBox representing the slot to animate.</param>
    /// <param name="lastSlot">The last slot tracker image to update.</param>
    public void Start(PictureBox pb, SlotTrackerImage lastSlot)
    {
        var view = WinFormsUtil.FindFirstControlOfType<ISlotViewer<PictureBox>>(pb);
        if (view is null)
            throw new InvalidCastException(nameof(view));
        var data = view.GetSlotData(pb);
        var pk = data.Read(view.SAV);
        Slot = pb;
        LastSlot = lastSlot;

        var orig = LastSlot.OriginalBackground = pb.BackgroundImage;

        Bitmap bg;
        if (GlowHover)
        {
            HoverWorker.Stop();
            var hover = Hover;
            var glow = Draw.GlowInitial;
            SpriteUtil.GetSpriteGlow(pk, glow.B, glow.G, glow.R, out var glowdata, out var imgGlowBase);
            bg = ImageUtil.LayerImage(imgGlowBase, hover, 0, 0);
            HoverWorker.GlowToColor = Draw.GlowFinal;
            HoverWorker.GlowFromColor = Draw.GlowInitial;
            HoverWorker.Start(pb, imgGlowBase, glowdata, orig, hover);
        }
        else
        {
            bg = Hover;
        }

        if (orig is not null)
            bg = ImageUtil.LayerImage(orig, bg, 0, 0);
        pb.BackgroundImage = LastSlot.CurrentBackground = bg;

        Preview.Show(pb, pk);
    }

    /// <summary>
    /// Stops the hover animation and restores the original slot background.
    /// </summary>
    public void Stop()
    {
        if (Slot is not null)
        {
            if (HoverWorker.Enabled)
                HoverWorker.Stop();
            else
                Slot.BackgroundImage = LastSlot?.OriginalBackground;
            Slot = null;
            LastSlot = null;
        }
        Preview.Clear();
    }

    /// <summary>
    /// Releases all resources used by the <see cref="SlotHoverHandler"/>.
    /// </summary>
    public void Dispose()
    {
        HoverWorker.Dispose();
        Slot = null;
        Draw.Dispose();
    }

    /// <summary>
    /// Updates the mouse position for the preview display.
    /// </summary>
    /// <param name="location">The current mouse location.</param>
    public void UpdateMousePosition(Point location)
    {
        Preview.UpdatePreviewPosition(location);
    }
}
