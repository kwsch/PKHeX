using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    /// <summary>
    /// Handles Hovering operations for an editor, where only one (1) slot can be animated at a given time when hovering over it.
    /// </summary>
    public sealed class SlotHoverHandler : IDisposable
    {
        public DrawConfig Draw { private get; set; }
        public bool GlowHover { private get; set; } = true;

        public static readonly CryPlayer CryPlayer = new CryPlayer();
        public static readonly SummaryPreviewer Preview = new SummaryPreviewer();
        private static Bitmap Hover => SpriteUtil.Spriter.Hover;

        private readonly BitmapAnimator HoverWorker = new BitmapAnimator();

        private PictureBox Slot;
        private SlotTrackerImage LastSlot;

        public void Start(PictureBox pb, SlotTrackerImage lastSlot)
        {
            var view = WinFormsUtil.FindFirstControlOfType<ISlotViewer<PictureBox>>(pb);
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
                SpriteUtil.GetSpriteGlow(pk, Draw.GlowInitial.B, Draw.GlowInitial.G, Draw.GlowInitial.R, out var glowdata, out var GlowBase);
                bg = ImageUtil.LayerImage(GlowBase, hover, 0, 0);
                HoverWorker.GlowToColor = Draw.GlowFinal;
                HoverWorker.GlowFromColor = Draw.GlowInitial;
                HoverWorker.Start(pb, GlowBase, glowdata, orig, hover);
            }
            else
            {
                bg = Hover;
            }

            if (orig != null)
                bg = ImageUtil.LayerImage(orig, bg, 0, 0);
            pb.BackgroundImage = LastSlot.CurrentBackground = bg;

            if (Settings.Default.HoverSlotShowText)
                Preview.Show(pb, pk);
            if (Settings.Default.HoverSlotPlayCry)
                CryPlayer.PlayCry(pk);
        }

        public void Stop()
        {
            if (Slot != null)
            {
                if (HoverWorker.Enabled)
                    HoverWorker.Stop();
                else
                    Slot.BackgroundImage = LastSlot.OriginalBackground;
                Slot = null;
                LastSlot = null;
            }
            Preview.Clear();
            CryPlayer.Stop();
        }

        public void Dispose()
        {
            HoverWorker?.Dispose();
            Slot?.Dispose();
            Draw?.Dispose();
        }
    }
}