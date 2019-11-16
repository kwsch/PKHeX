using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using PKHeX.Drawing;
using Timer = System.Timers.Timer;

namespace PKHeX.WinForms.Controls
{
    public sealed class BitmapAnimator : Timer
    {
        public BitmapAnimator()
        {
            Elapsed += TimerElapsed;
        }

        private int imgWidth;
        private int imgHeight;
        private byte[] GlowData;
        private Image ExtraLayer;
        private Image[] GlowCache;
        public Image OriginalBackground;
        private readonly object Lock = new object();

        private PictureBox pb;
        private int GlowInterval;
        private int GlowCounter;

        public int GlowFps { get; set; } = 60;
        public Color GlowToColor { get; set; } = Color.LightSkyBlue;
        public Color GlowFromColor { get; set; } = Color.White;

        public new static void Start() => throw new ArgumentException();

        public new void Stop()
        {
            if (pb == null || !Enabled)
                return;

            lock (Lock)
            {
                Enabled = false;
                pb.BackgroundImage = OriginalBackground;
            }

            // reset logic
            GlowCounter = 0;
            for (int i = 0; i < GlowCache.Length; i++)
                GlowCache[i] = null;
        }

        public void Start(PictureBox pbox, Image baseImage, byte[] glowData, Image original, Image extra)
        {
            Enabled = false;
            imgWidth = baseImage.Width;
            imgHeight = baseImage.Height;
            GlowData = glowData;
            GlowCounter = 0;
            GlowCache = new Image[GlowFps];
            GlowInterval = 1000 / GlowFps;
            Interval = GlowInterval;
            lock (Lock)
            {
                pb = pbox;
                ExtraLayer = extra;
                OriginalBackground = original;
            }
            Enabled = true;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!Enabled)
                return; // timer canceled, was waiting to proceed
            GlowCounter = (GlowCounter + 1) % (GlowInterval * 2); // loop backwards
            int frameIndex = GlowCounter >= GlowInterval ? (GlowInterval * 2) - GlowCounter : GlowCounter;

            lock (Lock)
            {
                if (!Enabled)
                    return;
                try { pb.BackgroundImage = GetFrame(frameIndex); } // drawing GDI can be silly sometimes #2072
                catch (AccessViolationException ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
            }
        }

        private Image GetFrame(int frameIndex)
        {
            var frame = GlowCache[frameIndex];
            if (frame != null)
                return frame;

            var elapsedFraction = (double)frameIndex / GlowInterval;
            var frameColor = GetFrameColor(elapsedFraction);
            var frameData = (byte[])GlowData.Clone();
            ImageUtil.ChangeAllColorTo(frameData, frameColor);

            frame = ImageUtil.GetBitmap(frameData, imgWidth, imgHeight);
            if (ExtraLayer != null)
                frame = ImageUtil.LayerImage(frame, ExtraLayer, 0, 0);
            frame = ImageUtil.LayerImage(OriginalBackground, frame, 0, 0);
            return GlowCache[frameIndex] = frame;
        }

        private Color GetFrameColor(double elapsedFraction) => ImageUtil.Blend(GlowToColor, GlowFromColor, elapsedFraction);
    }
}