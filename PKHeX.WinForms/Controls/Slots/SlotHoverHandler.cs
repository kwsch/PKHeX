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
    public DrawConfig Draw { private get; set; } = new();
    public bool GlowHover { private get; set; } = true;

    private readonly SummaryPreviewer Preview = new();
    private static Bitmap Hover => SpriteUtil.Spriter.Hover;

    private readonly BitmapAnimator HoverWorker = new();

    private PictureBox? Slot;
    private SlotTrackerImage? LastSlot;

    public void Start(PictureBox pb, SlotTrackerImage lastSlot)
    {
        var view = WinFormsUtil.FindFirstControlOfType<ISlotViewer<PictureBox>>(pb);
        if (view == null)
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

        if (orig != null)
            bg = ImageUtil.LayerImage(orig, bg, 0, 0);
        pb.BackgroundImage = LastSlot.CurrentBackground = bg;

        Preview.Show(pb, pk);
    }

    public void Stop()
    {
        if (Slot != null)
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

    public void Dispose()
    {
        HoverWorker.Dispose();
        Slot = null;
        Draw.Dispose();
    }

    public void UpdateMousePosition(Point location)
    {
        Preview.UpdatePreviewPosition(location);
    }
}
