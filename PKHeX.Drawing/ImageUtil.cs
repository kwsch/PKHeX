using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PKHeX.Drawing;

/// <summary>
/// Image Layering/Blending Utility
/// </summary>
public static class ImageUtil
{
    public static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y, double transparency)
    {
        overLayer = ChangeOpacity(overLayer, transparency);
        return LayerImage(baseLayer, overLayer, x, y);
    }

    public static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y)
    {
        Bitmap img = new(baseLayer);
        using Graphics gr = Graphics.FromImage(img);
        gr.DrawImage(overLayer, x, y, overLayer.Width, overLayer.Height);
        return img;
    }

    public static Bitmap ChangeOpacity(Image img, double trans)
    {
        var bmp = (Bitmap)img.Clone();
        GetBitmapData(bmp, out var bmpData, out var data);
        SetAllTransparencyTo(data, trans);
        bmp.UnlockBits(bmpData);

        return bmp;
    }

    public static Bitmap ChangeAllColorTo(Image img, Color c)
    {
        var bmp = (Bitmap)img.Clone();
        GetBitmapData(bmp, out var bmpData, out var data);
        ChangeAllColorTo(data, c);
        bmp.UnlockBits(bmpData);

        return bmp;
    }

    public static Bitmap ChangeTransparentTo(Image img, Color c, byte trans, int start = 0, int end = -1)
    {
        var bmp = (Bitmap)img.Clone();
        GetBitmapData(bmp, out var bmpData, out var data);
        if (end == -1)
            end = data.Length - 4;
        SetAllTransparencyTo(data, c, trans, start, end);
        bmp.UnlockBits(bmpData);
        return bmp;
    }

    public static Bitmap BlendTransparentTo(Image img, Color c, byte trans, int start = 0, int end = -1)
    {
        var bmp = (Bitmap)img.Clone();
        GetBitmapData(bmp, out var bmpData, out var data);
        if (end == -1)
            end = data.Length - 4;
        BlendAllTransparencyTo(data, c, trans, start, end);
        bmp.UnlockBits(bmpData);
        return bmp;
    }

    public static Bitmap WritePixels(Image img, Color c, int start, int end)
    {
        var bmp = (Bitmap)img.Clone();
        GetBitmapData(bmp, out var bmpData, out var data);
        ChangeAllTo(data, c, start, end);
        bmp.UnlockBits(bmpData);
        return bmp;
    }

    public static Bitmap ToGrayscale(Image img)
    {
        var bmp = (Bitmap)img.Clone();
        GetBitmapData(bmp, out var bmpData, out var data);
        SetAllColorToGrayScale(data);
        bmp.UnlockBits(bmpData);
        return bmp;
    }

    private static void GetBitmapData(Bitmap bmp, out BitmapData bmpData, out Span<byte> data)
    {
        bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        var length = bmp.Width * bmp.Height * 4;
        data = MemoryMarshal.CreateSpan(ref Unsafe.AddByteOffset(ref Unsafe.NullRef<byte>(), bmpData.Scan0), length);
    }

    public static Bitmap GetBitmap(ReadOnlySpan<byte> data, int width, int height, int length, PixelFormat format = PixelFormat.Format32bppArgb)
    {
        var bmp = new Bitmap(width, height, format);
        var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, format);
        var span = MemoryMarshal.CreateSpan(ref Unsafe.AddByteOffset(ref Unsafe.NullRef<byte>(), bmpData.Scan0), length);
        data[..length].CopyTo(span);
        bmp.UnlockBits(bmpData);
        return bmp;
    }

    public static Bitmap GetBitmap(ReadOnlySpan<byte> data, int width, int height, PixelFormat format = PixelFormat.Format32bppArgb)
    {
        return GetBitmap(data, width, height, data.Length, format);
    }

    public static byte[] GetPixelData(Bitmap bitmap)
    {
        var argbData = new byte[bitmap.Width * bitmap.Height * 4];
        var bd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
        Marshal.Copy(bd.Scan0, argbData, 0, bitmap.Width * bitmap.Height * 4);
        bitmap.UnlockBits(bd);
        return argbData;
    }

    public static void SetAllUsedPixelsOpaque(Span<byte> data)
    {
        for (int i = 0; i < data.Length; i += 4)
        {
            if (data[i + 3] != 0)
                data[i + 3] = 0xFF;
        }
    }

    public static void RemovePixels(Span<byte> pixels, ReadOnlySpan<byte> original)
    {
        var arr = MemoryMarshal.Cast<byte, int>(pixels);
        for (int i = original.Length - 4; i >= 0; i -= 4)
        {
            if (original[i + 3] != 0)
                arr[i >> 2] = 0;
        }
    }

    private static void SetAllTransparencyTo(Span<byte> data, double trans)
    {
        for (int i = 0; i < data.Length; i += 4)
            data[i + 3] = (byte)(data[i + 3] * trans);
    }

    public static void SetAllTransparencyTo(Span<byte> data, Color c, byte trans, int start, int end)
    {
        var arr = MemoryMarshal.Cast<byte, int>(data);
        var value = Color.FromArgb(trans, c).ToArgb();
        for (int i = end; i >= start; i -= 4)
        {
            if (data[i + 3] == 0)
                arr[i >> 2] = value;
        }
    }

    public static void BlendAllTransparencyTo(Span<byte> data, Color c, byte trans, int start, int end)
    {
        var arr = MemoryMarshal.Cast<byte, int>(data);
        var value = Color.FromArgb(trans, c).ToArgb();
        for (int i = end; i >= start; i -= 4)
        {
            var alpha = data[i + 3];
            if (alpha == 0)
                arr[i >> 2] = value;
            else if (alpha != 0xFF)
                arr[i >> 2] = BlendColor(arr[i >> 2], value);
        }
    }

    public static int GetAverageColor(Span<byte> data)
    {
        long r = 0, g = 0, b = 0;
        int count = 0;
        for (int i = 0; i < data.Length; i += 4)
        {
            var alpha = data[i + 3];
            if (alpha == 0)
                continue;
            r += data[i + 2];
            g += data[i + 1];
            b += data[i + 0];
            count++;
        }
        if (count == 0)
            return 0;
        byte R = (byte)(r / count);
        byte G = (byte)(g / count);
        byte B = (byte)(b / count);
        return (0xFF << 24) | (R << 16) | (G << 8) | B;
    }

    // heavily favor second (new) color
    private static int BlendColor(int color1, int color2, double amount = 0.2)
    {
        var a1 = (color1 >> 24) & 0xFF;
        var r1 = (color1 >> 16) & 0xFF;
        var g1 = (color1 >> 8) & 0xFF;
        var b1 = color1 & 0xFF;

        var a2 = (color2 >> 24) & 0xFF;
        var r2 = (color2 >> 16) & 0xFF;
        var g2 = (color2 >> 8) & 0xFF;
        var b2 = color2 & 0xFF;

        byte a = (byte)((a1 * amount) + (a2 * (1 - amount)));
        byte r = (byte)((r1 * amount) + (r2 * (1 - amount)));
        byte g = (byte)((g1 * amount) + (g2 * (1 - amount)));
        byte b = (byte)((b1 * amount) + (b2 * (1 - amount)));

        return (a << 24) | (r << 16) | (g << 8) | b;
    }

    public static void ChangeAllTo(Span<byte> data, Color c, int start, int end)
    {
        var arr = MemoryMarshal.Cast<byte, int>(data[start..end]);
        var value = c.ToArgb();
        arr.Fill(value);
    }

    public static void ChangeAllColorTo(Span<byte> data, Color c)
    {
        byte R = c.R;
        byte G = c.G;
        byte B = c.B;
        for (int i = 0; i < data.Length; i += 4)
        {
            if (data[i + 3] == 0)
                continue;
            data[i + 0] = B;
            data[i + 1] = G;
            data[i + 2] = R;
        }
    }

    private static void SetAllColorToGrayScale(Span<byte> data)
    {
        for (int i = 0; i < data.Length; i += 4)
        {
            if (data[i + 3] == 0)
                continue;
            byte greyS = (byte)((0.3 * data[i + 2]) + (0.59 * data[i + 1]) + (0.11 * data[i + 0]));
            data[i + 0] = greyS;
            data[i + 1] = greyS;
            data[i + 2] = greyS;
        }
    }

    public static void GlowEdges(Span<byte> data, byte blue, byte green, byte red, int width, int reach = 3, double amount = 0.0777)
    {
        PollutePixels(data, width, reach, amount);
        CleanPollutedPixels(data, blue, green, red);
    }

    private const int PollutePixelColorIndex = 0;

    private static void PollutePixels(Span<byte> data, int width, int reach, double amount)
    {
        int stride = width * 4;
        int height = data.Length / stride;
        for (int i = 0; i < data.Length; i += 4)
        {
            // only pollute outwards if the current pixel is fully opaque
            if (data[i + 3] == 0)
                continue;

            int x = (i % stride) / 4;
            int y = (i / stride);
            {
                int left = Math.Max(0, x - reach);
                int right = Math.Min(width - 1, x + reach);
                int top = Math.Max(0, y - reach);
                int bottom = Math.Min(height - 1, y + reach);
                for (int ix = left; ix <= right; ix++)
                {
                    for (int iy = top; iy <= bottom; iy++)
                    {
                        // update one of the color bits
                        // it is expected that a transparent pixel RGBA value is 0.
                        var c = 4 * (ix + (iy * width));
                        ref var b = ref data[c + PollutePixelColorIndex];
                        b += (byte)(amount * (0xFF - b));
                    }
                }
            }
        }
    }

    private static void CleanPollutedPixels(Span<byte> data, byte blue, byte green, byte red)
    {
        for (int i = 0; i < data.Length; i += 4)
        {
            // only clean if the current pixel isn't transparent
            if (data[i + 3] != 0)
                continue;

            // grab the transparency from the donor byte
            var transparency = data[i + PollutePixelColorIndex];
            if (transparency == 0)
                continue;

            data[i + 0] = blue;
            data[i + 1] = green;
            data[i + 2] = red;
            data[i + 3] = transparency;
        }
    }
}
