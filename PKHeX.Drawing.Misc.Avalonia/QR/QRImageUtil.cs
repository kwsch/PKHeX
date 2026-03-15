using System;
using PKHeX.Drawing.Avalonia;
using SkiaSharp;

namespace PKHeX.Drawing.Misc.Avalonia;

public static class QRImageUtil
{
    public static SKBitmap GetQRImage(SKBitmap qr, SKBitmap preview)
    {
        // create a small area with the pk sprite, with a white background
        var foreground = new SKBitmap(preview.Width + 4, preview.Height + 4);
        using (var canvas = new SKCanvas(foreground))
        {
            canvas.Clear(SKColors.White);
            int x = (foreground.Width / 2) - (preview.Width / 2);
            int y = (foreground.Height / 2) - (preview.Height / 2);
            canvas.DrawBitmap(preview, x, y);
        }

        // Layer on Preview Image
        {
            int x = (qr.Width / 2) - (foreground.Width / 2);
            int y = (qr.Height / 2) - (foreground.Height / 2);
            return ImageUtil.LayerImage(qr, foreground, x, y);
        }
    }

    public static SKBitmap GetQRImageExtended(SKFont font, SKBitmap qr, SKBitmap pk, int width, int height, ReadOnlySpan<string> lines, string extraText)
    {
        var pic = GetQRImage(qr, pk);
        return ExtendImage(font, qr, width, height, pic, lines, extraText);
    }

    private static SKBitmap ExtendImage(SKFont font, SKBitmap qr, int width, int height, SKBitmap pic, ReadOnlySpan<string> lines, string extraText)
    {
        var newpic = new SKBitmap(width, height);
        using var g = new SKCanvas(newpic);
        g.Clear(SKColors.White);
        g.DrawBitmap(pic, 0, 0);

        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
        };

        const int indent = 18;
        g.DrawText(GetLine(lines, 0), indent, qr.Height - 5, SKTextAlign.Left, font, paint);
        g.DrawText(GetLine(lines, 1), indent, qr.Height + 8, SKTextAlign.Left, font, paint);
        g.DrawText(GetLine2(lines), indent, qr.Height + 20, SKTextAlign.Left, font, paint);
        g.DrawText(GetLine(lines, 3) + extraText, indent, qr.Height + 32, SKTextAlign.Left, font, paint);
        return newpic;
    }

    private static string GetLine2(ReadOnlySpan<string> lines) => GetLine(lines, 2)
        .Replace(Environment.NewLine, "/")
        .Replace("//", "   ")
        .Replace(":/", ": ");

    private static string GetLine(ReadOnlySpan<string> lines, int line) => lines.Length <= line ? string.Empty : lines[line];
}
