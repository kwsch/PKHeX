using System;
using System.Buffers;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using PKHeX.Drawing;
using Timer = System.Timers.Timer;

namespace PKHeX.WinForms.Controls;

public sealed class BitmapAnimator : IDisposable
{
    public BitmapAnimator() => Timer.Elapsed += TimerElapsed;

    private readonly Timer Timer = new();

    private int imgWidth;
    private int imgHeight;
    private ReadOnlyMemory<byte> GlowData;
    private Image? ExtraLayer;
    private Image?[]? GlowCache;
    private Image? OriginalBackground;
    private readonly Lock Lock = new();

    private PictureBox? pb;
    private int GlowInterval;
    private int GlowCounter;

    public int GlowFps { get; set; } = 60;
    public Color GlowToColor { get; set; } = Color.LightSkyBlue;
    public Color GlowFromColor { get; set; } = Color.White;
    public bool Enabled { get => Timer.Enabled; set => Timer.Enabled = value; }

    public void Stop()
    {
        if (pb is null || !Enabled)
            return;

        lock (Lock)
        {
            Enabled = false;
            pb.BackgroundImage = OriginalBackground;
        }

        // reset logic
        ArgumentNullException.ThrowIfNull(GlowCache);
        GlowCounter = 0;
        for (int i = 0; i < GlowCache.Length; i++)
            GlowCache[i] = null;
    }

    public void Start(PictureBox pbox, Image baseImage, ReadOnlyMemory<byte> glowData, Image? original, Image extra)
    {
        Enabled = false;
        imgWidth = baseImage.Width;
        imgHeight = baseImage.Height;
        GlowData = glowData;
        GlowCounter = 0;
        GlowCache = new Image[GlowFps];
        GlowInterval = 1000 / GlowFps;
        Timer.Interval = GlowInterval;
        lock (Lock)
        {
            pb = pbox;
            ExtraLayer = extra;
            OriginalBackground = original;
        }
        Enabled = true;
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs? elapsedEventArgs)
    {
        if (!Enabled)
            return; // timer canceled, was waiting to proceed
        GlowCounter = (GlowCounter + 1) % (GlowInterval * 2); // loop backwards
        int frameIndex = GlowCounter >= GlowInterval ? (GlowInterval * 2) - GlowCounter : GlowCounter;

        lock (Lock)
        {
            if (!Enabled)
                return;

            if (pb is null)
                return;
            try { pb.BackgroundImage = GetFrame(frameIndex); } // drawing GDI can be silly sometimes #2072
            catch (AccessViolationException ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }
    }

    private Image GetFrame(int frameIndex)
    {
        var cache = GlowCache;
        ArgumentNullException.ThrowIfNull(cache);
        var frame = cache[frameIndex];
        if (frame is not null)
            return frame;

        var elapsedFraction = (double)frameIndex / GlowInterval;
        var frameColor = GetFrameColor(elapsedFraction);

        ArgumentOutOfRangeException.ThrowIfEqual(GlowData.Length, 0);

        var frameData = ArrayPool<byte>.Shared.Rent(GlowData.Length);
        var frameSpan = frameData.AsSpan(0, GlowData.Length);
        GlowData.Span.CopyTo(frameSpan);
        ImageUtil.ChangeAllColorTo(frameSpan, frameColor);
        frame = ImageUtil.GetBitmap(frameData, imgWidth, imgHeight, GlowData.Length);
        frameSpan.Clear();
        ArrayPool<byte>.Shared.Return(frameData);

        if (ExtraLayer is not null)
            frame = ImageUtil.LayerImage(frame, ExtraLayer, 0, 0);
        if (OriginalBackground is not null)
            frame = ImageUtil.LayerImage(OriginalBackground, frame, 0, 0);
        return cache[frameIndex] = frame;
    }

    private Color GetFrameColor(double elapsedFraction) => ColorUtil.Blend(GlowToColor, GlowFromColor, elapsedFraction);

    public void Dispose()
    {
        GlowCache = null;
        Timer.Enabled = false;
        Timer.Elapsed -= TimerElapsed;
        Timer.Dispose();
    }
}
