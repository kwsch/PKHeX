using System;
using System.Drawing;

namespace PKHeX.Drawing.Misc;

public static class QRImageUtil
{
    public static Bitmap GetQRImage(Image qr, Image preview)
    {
        // create a small area with the pk sprite, with a white background
        var foreground = new Bitmap(preview.Width + 4, preview.Height + 4);
        using (Graphics gfx = Graphics.FromImage(foreground))
        {
            gfx.FillRectangle(Brushes.White, 0, 0, foreground.Width, foreground.Height);
            int x = (foreground.Width / 2) - (preview.Width / 2);
            int y = (foreground.Height / 2) - (preview.Height / 2);
            gfx.DrawImage(preview, x, y);
        }

        // Layer on Preview Image
        {
            int x = (qr.Width / 2) - (foreground.Width / 2);
            int y = (qr.Height / 2) - (foreground.Height / 2);
            return ImageUtil.LayerImage(qr, foreground, x, y);
        }
    }

    public static Bitmap GetQRImageExtended(Font font, Image qr, Image pk, int width, int height, ReadOnlySpan<string> lines, string extraText)
    {
        var pic = GetQRImage(qr, pk);
        return ExtendImage(font, qr, width, height, pic, lines, extraText);
    }

    private static Bitmap ExtendImage(Font font, Image qr, int width, int height, Image pic, ReadOnlySpan<string> lines, string extraText)
    {
        var newpic = new Bitmap(width, height);
        using Graphics g = Graphics.FromImage(newpic);
        g.FillRectangle(Brushes.White, 0, 0, newpic.Width, newpic.Height);
        g.DrawImage(pic, 0, 0);

        var black = Brushes.Black;
        const int indent = 18;
        g.DrawString(GetLine(lines, 0), font, black, indent, qr.Height - 5);
        g.DrawString(GetLine(lines, 1), font, black, indent, qr.Height + 8);
        g.DrawString(GetLine2(lines)  , font, black, indent, qr.Height + 20);
        g.DrawString(GetLine(lines, 3) + extraText, font, black, indent, qr.Height + 32);
        return newpic;
    }

    private static string GetLine2(ReadOnlySpan<string> lines) => GetLine(lines, 2)
        .Replace(Environment.NewLine, "/")
        .Replace("//", "   ")
        .Replace(":/", ": ");

    private static string GetLine(ReadOnlySpan<string> lines, int line) => lines.Length <= line ? string.Empty : lines[line];
}
