using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    public class SlotHoverHandler : IDisposable
    {
        public SaveFile SAV { private get; set; }
        public DrawConfig Draw { private get; set; }
        public bool GlowHover { private get; set; } = true;

        public static readonly CryPlayer CryPlayer = new CryPlayer();
        public static readonly SummaryPreviewer Preview = new SummaryPreviewer();

        private readonly BitmapAnimator HoverWorker = new BitmapAnimator(Resources.slotHover);

        private PictureBox Slot;

        public void Start(PictureBox pb, SlotTrackerImage LastSlot)
        {
            var view = WinFormsUtil.FindFirstControlOfType<ISlotViewer<PictureBox>>(pb);
            var data = view.GetSlotData(pb);
            var pk = SAV.GetStoredSlot(data.Offset);
            Slot = pb;

            var orig = LastSlot.OriginalBackground = pb.BackgroundImage;

            Bitmap bg;
            if (GlowHover)
            {
                HoverWorker.Stop();

                SpriteUtil.GetSpriteGlow(pk, Draw.GlowInitial.B, Draw.GlowInitial.G, Draw.GlowInitial.R, out var glowdata, out var GlowBase);
                bg = ImageUtil.LayerImage(GlowBase, Resources.slotHover, 0, 0);
                HoverWorker.GlowToColor = Draw.GlowFinal;
                HoverWorker.GlowFromColor = Draw.GlowInitial;
                HoverWorker.Start(pb, GlowBase, glowdata, orig);
            }
            else
            {
                bg = Resources.slotHover;
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
                HoverWorker.Stop();
                Slot = null;
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