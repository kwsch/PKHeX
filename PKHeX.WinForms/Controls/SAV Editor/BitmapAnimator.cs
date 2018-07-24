using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace PKHeX.WinForms.Controls
{
    public sealed class BitmapAnimator : Timer
    {
        public BitmapAnimator(Bitmap baseImage, Bitmap extraLayer = null)
        {
            GlowBase = baseImage;
            ExtraLayer = extraLayer;
            Elapsed += TimerElapsed;
        }

        private Bitmap GlowBase;
        private Bitmap ExtraLayer;
        private Bitmap[] GlowCache;
        public Image OriginalBackground;

        private PictureBox pb;
        private int GlowInterval;
        private int GlowCounter;

        public int GlowFps { get; set; } = 60;
        public Color GlowToColor { get; set; } = Color.LightSkyBlue;
        public Color GlowFromColor { get; set; } = Color.White;
        private readonly object Lock = new object();

        public new void Start() => throw new ArgumentException();

        public new void Stop()
        {
            lock (Lock)
                StopTimer();
        }

        private void StopTimer()
        {
            Enabled = false;
            pb.BackgroundImage = OriginalBackground;
            GlowBase = ExtraLayer = null;
            OriginalBackground = null;
        }

        public void Start(PictureBox pbox, Image original)
        {
            pb = pbox;
            OriginalBackground = original;
            GlowCache = new Bitmap[GlowFps];
            GlowInterval = 1000 / GlowFps;
            Interval = GlowInterval;
            Enabled = true;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (Lock)
            {
                if (!Enabled)
                    return; // timer canceled, was waiting to proceed
                GlowCounter = (GlowCounter + 1) % (GlowInterval * 2); // loop backwards
                int frameIndex = GlowCounter >= GlowInterval ? (GlowInterval * 2) - GlowCounter : GlowCounter;
                try
                {
                    var frame = GetFrame(frameIndex);
                    pb.BackgroundImage = ImageUtil.LayerImage(OriginalBackground, frame, 0, 0, 1);
                }
                catch
                {
                    StopTimer();
                }
            }
        }

        private Bitmap GetFrame(int frameIndex)
        {
            var frame = GlowCache[frameIndex];
            if (frame != null)
                return frame;

            var elapsedFraction = (double)frameIndex / GlowInterval;
            var frameColor = GetFrameColor(elapsedFraction);
            frame = ImageUtil.ChangeAllColorTo(GlowBase, frameColor);
            if (ExtraLayer != null)
                frame = ImageUtil.LayerImage(frame, ExtraLayer, 0, 0, 1);
            return GlowCache[frameIndex] = frame;
        }

        private Color GetFrameColor(double elapsedFraction) => ImageUtil.Blend(GlowToColor, GlowFromColor, elapsedFraction);
    }
}