using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace PKHeX.WinForms.Controls
{
    public sealed class BitmapAnimator : Timer
    {
        public BitmapAnimator(Image extraLayer = null)
        {
            ExtraLayer = extraLayer;
            Elapsed += TimerElapsed;
        }

        private Image GlowBase;
        private byte[] GlowData;
        private readonly Image ExtraLayer;
        private Image[] GlowCache;
        public Image OriginalBackground;

        private PictureBox pb;
        private int GlowInterval;
        private int GlowCounter;

        public int GlowFps { get; set; } = 60;
        public Color GlowToColor { get; set; } = Color.LightSkyBlue;
        public Color GlowFromColor { get; set; } = Color.White;

        public new void Start() => throw new ArgumentException();

        public new void Stop()
        {
            if (pb == null || !Enabled)
                return;
            Enabled = false;

            // reset logic
            GlowCounter = 0;
            pb.BackgroundImage = OriginalBackground;
            for (int i = 0; i < GlowCache.Length; i++)
                GlowCache[i] = null;
        }

        public void Start(PictureBox pbox, Image baseImage, byte[] glowData, Image original)
        {
            GlowBase = baseImage;
            GlowData = glowData;
            pb = pbox;
            OriginalBackground = original;
            GlowCache = new Image[GlowFps];
            GlowInterval = 1000 / GlowFps;
            Interval = GlowInterval;
            Enabled = true;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!Enabled)
                return; // timer canceled, was waiting to proceed
            GlowCounter = (GlowCounter + 1) % (GlowInterval * 2); // loop backwards
            int frameIndex = GlowCounter >= GlowInterval ? (GlowInterval * 2) - GlowCounter : GlowCounter;
            pb.BackgroundImage = GetFrame(frameIndex);
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

            frame = ImageUtil.GetBitmap(frameData, GlowBase.Width, GlowBase.Height);
            if (ExtraLayer != null)
                frame = ImageUtil.LayerImage(frame, ExtraLayer, 0, 0, 1);
            frame = ImageUtil.LayerImage(OriginalBackground, frame, 0, 0, 1);
            return GlowCache[frameIndex] = frame;
        }

        private Color GetFrameColor(double elapsedFraction) => ImageUtil.Blend(GlowToColor, GlowFromColor, elapsedFraction);
    }
}