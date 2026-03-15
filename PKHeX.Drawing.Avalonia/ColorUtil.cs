using System;
using SkiaSharp;

namespace PKHeX.Drawing.Avalonia;

public static class ColorUtil
{
    private const byte MaxStat = 180;
    private const byte MinStat = 0;
    private const byte ShiftDownBST = 175;
    private const float ShiftDivBST = 3;

    public static SKColor ColorBaseStat(int stat)
    {
        var x = (uint)stat >= MaxStat ? 1f : ((float)stat) / MaxStat;
        return GetPastelRYG(x);
    }

    public static SKColor ColorBaseStatTotal(int bst)
    {
        var sumToSingle = Math.Max(MinStat, bst - ShiftDownBST) / ShiftDivBST;
        return ColorBaseStat((int)sumToSingle);
    }

    public static SKColor GetPastelRYG(float x)
    {
        var r = x > .5f ? 510 * (1 - x) : 255;
        var g = x > .5f ? 255 : 510 * x;
        const float white = 0.4f;
        const byte b = (byte)(0xFF * (1 - white));
        var br = (byte)((r * white) + b);
        var bg = (byte)((g * white) + b);
        return new SKColor(br, bg, b);
    }

    public static SKColor Blend(SKColor color, SKColor backColor, double amount)
    {
        byte r = MixComponent(color.Red, backColor.Red, amount);
        byte g = MixComponent(color.Green, backColor.Green, amount);
        byte b = MixComponent(color.Blue, backColor.Blue, amount);
        return new SKColor(r, g, b);
    }

    public static byte MixComponent(byte a, byte b, double x) => (byte)((a * x) + (b * (1 - x)));
}
