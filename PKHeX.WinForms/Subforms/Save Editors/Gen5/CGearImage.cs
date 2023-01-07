using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms;

/// <summary>
/// Utility logic to convert images to C-Gear Backgrounds and reverse.
/// </summary>
public static class CGearImage
{
    private const int Width = CGearBackground.Width;
    private const int Height = CGearBackground.Height;

    /// <summary>
    /// Gets the visual image of a <see cref="CGearBackground"/>.
    /// </summary>
    public static Bitmap GetBitmap(CGearBackground bg)
    {
        var data = bg.GetImageData();
        return ImageUtil.GetBitmap(data, Width, Height);
    }

    /// <summary>
    /// Converts a <see cref="Bitmap"/> to a <see cref="CGearBackground"/>.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public static CGearBackground GetCGearBackground(Bitmap img)
    {
        if (img.Width != Width)
            throw new ArgumentException($"Invalid image width. Expected {Width} pixels wide.");
        if (img.Height != Height)
            throw new ArgumentException($"Invalid image height. Expected {Height} pixels high.");
        if (img.PixelFormat is not PixelFormat.Format32bppArgb)
            throw new ArgumentException($"Invalid image format. Expected {PixelFormat.Format32bppArgb}");

        // get raw bytes of image
        byte[] data = ImageUtil.GetPixelData(img);
        const int bpp = 4;
        Debug.Assert(data.Length == Width * Height * bpp);

        return CGearBackground.GetBackground(data);
    }
}
