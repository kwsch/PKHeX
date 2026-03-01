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
    extension(Bitmap bmp)
    {
        /// <summary>
        /// Locks the bitmap and returns a span containing its raw pixel data in the specified pixel format.
        /// </summary>
        /// <remarks>
        /// The returned span provides direct access to the bitmap's memory. Modifying the span will update the bitmap.
        /// The caller is responsible for unlocking the bitmap using <see cref="Bitmap.UnlockBits"/> after processing. This method is not thread-safe.
        /// </remarks>
        /// <param name="bmpData">
        /// When this method returns, contains a BitmapData object representing the locked bitmap area.
        /// The caller must unlock the bitmap after processing the data.
        /// </param>
        /// <param name="format">
        /// The pixel format to use when locking the bitmap.
        /// Defaults to <see cref="PixelFormat.Format32bppArgb"/> to ensure the usages within this utility class process pixels in the expected way.
        /// </param>
        /// <returns>A span of bytes representing the bitmap's pixel data. The span covers the entire bitmap in the specified pixel format.</returns>
        public Span<byte> GetBitmapData(out BitmapData bmpData, PixelFormat format = PixelFormat.Format32bppArgb)
        {
            bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, format);
            var bpp = Image.GetPixelFormatSize(format) / 8;
            return GetSpan(bmpData.Scan0, bmp.Width * bmp.Height * bpp);
        }

        public void GetBitmapData(Span<byte> data, PixelFormat format = PixelFormat.Format32bppArgb)
        {
            var span = bmp.GetBitmapData(out var bmpData, format);
            span.CopyTo(data);
            bmp.UnlockBits(bmpData);
        }

        public void GetBitmapData(Span<int> data)
        {
            var span = bmp.GetBitmapData(out var bmpData);
            var src = MemoryMarshal.Cast<byte, int>(span);
            src.CopyTo(data);
            bmp.UnlockBits(bmpData);
        }

        public void SetBitmapData(ReadOnlySpan<byte> data, PixelFormat format = PixelFormat.Format32bppArgb)
        {
            var span = bmp.GetBitmapData(out var bmpData, format);
            data.CopyTo(span);
            bmp.UnlockBits(bmpData);
        }

        public void SetBitmapData(Span<int> data)
        {
            var span = bmp.GetBitmapData(out var bmpData);
            var dest = MemoryMarshal.Cast<byte, int>(span);
            data.CopyTo(dest);
            bmp.UnlockBits(bmpData);
        }

        public byte[] GetBitmapData()
        {
            var format = bmp.PixelFormat;
            var bpp = Image.GetPixelFormatSize(format) / 8;
            var result = new byte[bmp.Width * bmp.Height * bpp];
            bmp.GetBitmapData(result, format);
            return result;
        }

        public void ToGrayscale(float intensity)
        {
            if (intensity is <= 0.01f or > 1f)
                return; // don't care

            var data = bmp.GetBitmapData(out var bmpData);
            SetAllColorToGrayScale(data, intensity);
            bmp.UnlockBits(bmpData);
        }

        public void ChangeOpacity(double trans)
        {
            if (trans is <= 0.01f or > 1f)
                return; // don't care

            var data = bmp.GetBitmapData(out var bmpData);
            SetAllTransparencyTo(data, trans);
            bmp.UnlockBits(bmpData);
        }

        public void BlendTransparentTo(Color c, byte trans, int start = 0, int end = -1)
        {
            var data = bmp.GetBitmapData(out var bmpData);
            if (end == -1)
                end = data.Length;
            BlendAllTransparencyTo(data[start..end], c, trans);
            bmp.UnlockBits(bmpData);
        }

        public void ChangeAllColorTo(Color c)
        {
            var data = bmp.GetBitmapData(out var bmpData);
            ChangeAllColorTo(data, c);
            bmp.UnlockBits(bmpData);
        }

        public void ChangeTransparentTo(Color c, byte trans, int start = 0, int end = -1)
        {
            var data = bmp.GetBitmapData(out var bmpData);
            if (end == -1)
                end = data.Length;
            SetAllTransparencyTo(data[start..end], c, trans);
            bmp.UnlockBits(bmpData);
        }

        public void WritePixels(Color c, int start, int end)
        {
            var data = bmp.GetBitmapData(out var bmpData);
            ChangeAllTo(data, c, start, end);
            bmp.UnlockBits(bmpData);
        }

        public int GetAverageColor()
        {
            var data = bmp.GetBitmapData(out var bmpData);
            var avg = GetAverageColor(data);
            bmp.UnlockBits(bmpData);
            return avg;
        }
    }

    private static Span<byte> GetSpan(IntPtr ptr, int length)
        => MemoryMarshal.CreateSpan(ref Unsafe.AddByteOffset(ref Unsafe.NullRef<byte>(), ptr), length);

    public static Bitmap LayerImage(Bitmap baseLayer, Bitmap overLayer, int x, int y, double transparency)
    {
        overLayer = CopyChangeOpacity(overLayer, transparency);
        return LayerImage(baseLayer, overLayer, x, y);
    }

    public static Bitmap LayerImage(Bitmap baseLayer, Image overLayer, int x, int y)
    {
        var bmp = new Bitmap(baseLayer);
        using var gr = Graphics.FromImage(bmp);
        gr.DrawImage(overLayer, x, y, overLayer.Width, overLayer.Height);
        return bmp;
    }

    public static Bitmap CopyChangeOpacity(Bitmap img, double trans)
    {
        var bmp = (Bitmap)img.Clone();
        bmp.ChangeOpacity(trans);
        return bmp;
    }

    public static Bitmap CopyChangeAllColorTo(Bitmap img, Color c)
    {
        var bmp = (Bitmap)img.Clone();
        bmp.ChangeAllColorTo(c);
        return bmp;
    }

    public static Bitmap CopyChangeTransparentTo(Bitmap img, Color c, byte trans, int start = 0, int end = -1)
    {
        var bmp = (Bitmap)img.Clone();
        bmp.ChangeTransparentTo(c, trans, start, end);
        return bmp;
    }

    public static Bitmap CopyWritePixels(Bitmap img, Color c, int start, int end)
    {
        var bmp = (Bitmap)img.Clone();
        bmp.WritePixels(c, start, end);
        return bmp;
    }

    public static Bitmap GetBitmap(ReadOnlySpan<byte> data, int width, int height, int length, PixelFormat format = PixelFormat.Format32bppArgb)
    {
        var bmp = new Bitmap(width, height, format);
        var span = bmp.GetBitmapData(out var bmpData);
        data[..length].CopyTo(span);
        bmp.UnlockBits(bmpData);
        return bmp;
    }

    public static Bitmap GetBitmap(ReadOnlySpan<byte> data, int width, int height, PixelFormat format = PixelFormat.Format32bppArgb)
    {
        return GetBitmap(data, width, height, data.Length, format);
    }

    public static void SetAllUsedPixelsOpaque(Span<byte> data)
    {
        for (int i = data.Length - 4; i >= 0; i -= 4)
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
        for (int i = data.Length - 4; i >= 0; i -= 4)
            data[i + 3] = (byte)(data[i + 3] * trans);
    }

    private static void SetAllTransparencyTo(Span<byte> data, Color c, byte trans)
    {
        var arr = MemoryMarshal.Cast<byte, int>(data);
        var value = Color.FromArgb(trans, c).ToArgb();
        for (int i = data.Length - 4; i >= 0; i -= 4)
        {
            if (data[i + 3] == 0)
                arr[i >> 2] = value;
        }
    }

    private static void BlendAllTransparencyTo(Span<byte> data, Color c, byte trans)
    {
        var arr = MemoryMarshal.Cast<byte, int>(data);
        var value = Color.FromArgb(trans, c).ToArgb();
        for (int i = data.Length - 4; i >= 0; i -= 4)
        {
            var alpha = data[i + 3];
            if (alpha == 0)
                arr[i >> 2] = value;
            else if (alpha != 0xFF)
                arr[i >> 2] = BlendColor(arr[i >> 2], value);
        }
    }

    private static int GetAverageColor(Span<byte> data)
    {
        long r = 0, g = 0, b = 0;
        int count = 0;
        for (int i = data.Length - 4; i >= 0; i -= 4)
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

    private static void ChangeAllTo(Span<byte> data, Color c, int start, int end)
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
        for (int i = data.Length - 4; i >= 0; i -= 4)
        {
            if (data[i + 3] == 0)
                continue;
            data[i + 0] = B;
            data[i + 1] = G;
            data[i + 2] = R;
        }
    }

    private static void SetAllColorToGrayScale(Span<byte> data, float intensity)
    {
        if (intensity <= 0f)
            return;

        if (intensity >= 0.999f)
        {
            SetAllColorToGrayScale(data);
            return;
        }

        float inverse = 1f - intensity;
        for (int i = data.Length - 4; i >= 0; i -= 4)
        {
            if (data[i + 3] == 0)
                continue;
            byte greyS = (byte)((0.3 * data[i + 2]) + (0.59 * data[i + 1]) + (0.11 * data[i + 0]));
            data[i + 0] = (byte)((data[i + 0] * inverse) + (greyS * intensity));
            data[i + 1] = (byte)((data[i + 1] * inverse) + (greyS * intensity));
            data[i + 2] = (byte)((data[i + 2] * inverse) + (greyS * intensity));
        }
    }

    private static void SetAllColorToGrayScale(Span<byte> data)
    {
        for (int i = data.Length - 4; i >= 0; i -= 4)
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
        for (int i = data.Length - 4; i >= 0; i -= 4)
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
        for (int i = data.Length - 4; i >= 0; i -= 4)
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
