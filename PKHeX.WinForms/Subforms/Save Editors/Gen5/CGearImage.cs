using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms;

public static class CGearImage
{
    public static Bitmap GetBitmap(CGearBackground bg)
    {
        return ImageUtil.GetBitmap(bg.GetImageData(), CGearBackground.Width, CGearBackground.Height);
    }

    public static CGearBackground GetCGearBackground(Bitmap img)
    {
        const int Width = CGearBackground.Width;
        const int Height = CGearBackground.Height;
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
