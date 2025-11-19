using System;
using System.Drawing;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides utility methods for composing and extending QR code images with overlays and text.
/// </summary>
public static class QRImageUtil
{
    /// <summary>
    /// Creates a new QR image with a preview image layered in the center.
    /// </summary>
    /// <param name="qr">The base QR code image.</param>
    /// <param name="preview">The preview image to overlay.</param>
    /// <returns>A new bitmap with the preview image centered on the QR code.</returns>
    public static Bitmap GetQRImage(Image qr, Image preview)
    {
        // create a small area with the pk sprite, with a white background
        var foreground = new Bitmap(preview.Width + 4, preview.Height + 4);
        using (Graphics gfx = Graphics.FromImage(foreground))
        {
            gfx.FillRectangle(SystemBrushes.ControlLightLight, 0, 0, foreground.Width, foreground.Height);
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

    /// <summary>
    /// Creates an extended QR image with additional text and formatting.
    /// </summary>
    /// <param name="font">The font to use for text.</param>
    /// <param name="qr">The base QR code image.</param>
    /// <param name="pk">The preview image to overlay.</param>
    /// <param name="width">The width of the final image.</param>
    /// <param name="height">The height of the final image.</param>
    /// <param name="lines">The lines of text to display.</param>
    /// <param name="extraText">Additional text to display.</param>
    /// <returns>A new bitmap with the preview image and extended text.</returns>
    public static Bitmap GetQRImageExtended(Font font, Image qr, Image pk, int width, int height, ReadOnlySpan<string> lines, string extraText)
    {
        var pic = GetQRImage(qr, pk);
        return ExtendImage(font, qr, width, height, pic, lines, extraText);
    }

    /// <summary>
    /// Extends an image with additional lines of text and formatting.
    /// </summary>
    /// <param name="font">The font to use for text.</param>
    /// <param name="qr">The base QR code image.</param>
    /// <param name="width">The width of the final image.</param>
    /// <param name="height">The height of the final image.</param>
    /// <param name="pic">The image to extend.</param>
    /// <param name="lines">The lines of text to display.</param>
    /// <param name="extraText">Additional text to display.</param>
    /// <returns>A new bitmap with extended text.</returns>
    private static Bitmap ExtendImage(Font font, Image qr, int width, int height, Image pic, ReadOnlySpan<string> lines, string extraText)
    {
        var newpic = new Bitmap(width, height);
        using Graphics g = Graphics.FromImage(newpic);
        g.FillRectangle(SystemBrushes.ControlLightLight, 0, 0, newpic.Width, newpic.Height);
        g.DrawImage(pic, 0, 0);

        var black = SystemBrushes.ControlText;
        const int indent = 18;
        g.DrawString(GetLine(lines, 0), font, black, indent, qr.Height - 5);
        g.DrawString(GetLine(lines, 1), font, black, indent, qr.Height + 8);
        g.DrawString(GetLine2(lines)  , font, black, indent, qr.Height + 20);
        g.DrawString(GetLine(lines, 3) + extraText, font, black, indent, qr.Height + 32);
        return newpic;
    }

    /// <summary>
    /// Gets and formats the second line of text for display.
    /// </summary>
    /// <param name="lines">The lines of text.</param>
    /// <returns>The formatted second line.</returns>
    private static string GetLine2(ReadOnlySpan<string> lines) => GetLine(lines, 2)
        .Replace(Environment.NewLine, "/")
        .Replace("//", "   ")
        .Replace(":/", ": ");

    /// <summary>
    /// Gets a specific line of text or an empty string if the line does not exist.
    /// </summary>
    /// <param name="lines">The lines of text.</param>
    /// <param name="line">The line index to retrieve.</param>
    /// <returns>The requested line or an empty string.</returns>
    private static string GetLine(ReadOnlySpan<string> lines, int line) => lines.Length <= line ? string.Empty : lines[line];
}
