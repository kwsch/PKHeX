using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace PKHeX.Drawing.Avalonia;

/// <summary>
/// Image Layering/Blending Utility
/// </summary>
public static class ImageUtil
{
    private static Span<byte> GetSpan(nint ptr, int length)
        => MemoryMarshal.CreateSpan(ref Unsafe.AddByteOffset(ref Unsafe.NullRef<byte>(), ptr), length);

    public static Span<byte> GetPixelSpan(this SKBitmap bmp)
    {
        return GetSpan(bmp.GetPixels(), bmp.ByteCount);
    }

    public static void GetBitmapData(this SKBitmap bmp, Span<byte> data)
    {
        bmp.GetPixelSpan().CopyTo(data);
    }

    public static void GetBitmapData(this SKBitmap bmp, Span<int> data)
    {
        var span = bmp.GetPixelSpan();
        var src = MemoryMarshal.Cast<byte, int>(span);
        src.CopyTo(data);
    }

    public static void SetBitmapData(this SKBitmap bmp, ReadOnlySpan<byte> data)
    {
        var span = bmp.GetPixelSpan();
        data.CopyTo(span);
    }

    public static void SetBitmapData(this SKBitmap bmp, Span<int> data)
    {
        var span = bmp.GetPixelSpan();
        var dest = MemoryMarshal.Cast<byte, int>(span);
        data.CopyTo(dest);
    }

    public static byte[] GetBitmapData(this SKBitmap bmp)
    {
        var result = new byte[bmp.ByteCount];
        bmp.GetBitmapData(result);
        return result;
    }

    public static void ToGrayscale(this SKBitmap bmp, float intensity)
    {
        if (intensity is <= 0.01f or > 1f)
            return;

        var data = bmp.GetPixelSpan();
        SetAllColorToGrayScale(data, intensity);
    }

    public static void ChangeOpacity(this SKBitmap bmp, double trans)
    {
        if (trans is <= 0.01f or > 1f)
            return;

        var data = bmp.GetPixelSpan();
        SetAllTransparencyTo(data, trans);
    }

    public static void BlendTransparentTo(this SKBitmap bmp, SKColor c, byte trans, int start = 0, int end = -1)
    {
        var data = bmp.GetPixelSpan();
        if (end == -1)
            end = data.Length;
        BlendAllTransparencyTo(data[start..end], c, trans);
    }

    public static void ChangeAllColorTo(this SKBitmap bmp, SKColor c)
    {
        var data = bmp.GetPixelSpan();
        ChangeAllColorTo(data, c);
    }

    public static void ChangeTransparentTo(this SKBitmap bmp, SKColor c, byte trans, int start = 0, int end = -1)
    {
        var data = bmp.GetPixelSpan();
        if (end == -1)
            end = data.Length;
        SetAllTransparencyTo(data[start..end], c, trans);
    }

    public static void WritePixels(this SKBitmap bmp, SKColor c, int start, int end)
    {
        var data = bmp.GetPixelSpan();
        ChangeAllTo(data, c, start, end);
    }

    public static int GetAverageColor(this SKBitmap bmp)
    {
        var data = bmp.GetPixelSpan();
        return GetAverageColor(data);
    }

    public static SKBitmap LayerImage(SKBitmap baseLayer, SKBitmap overLayer, int x, int y, double transparency)
    {
        overLayer = CopyChangeOpacity(overLayer, transparency);
        return LayerImage(baseLayer, overLayer, x, y);
    }

    public static SKBitmap LayerImage(SKBitmap baseLayer, SKBitmap overLayer, int x, int y)
    {
        var bmp = baseLayer.Copy();
        using var canvas = new SKCanvas(bmp);
        canvas.DrawBitmap(overLayer, x, y);
        return bmp;
    }

    public static SKBitmap CopyChangeOpacity(SKBitmap img, double trans)
    {
        var bmp = img.Copy();
        bmp.ChangeOpacity(trans);
        return bmp;
    }

    public static SKBitmap CopyChangeAllColorTo(SKBitmap img, SKColor c)
    {
        var bmp = img.Copy();
        bmp.ChangeAllColorTo(c);
        return bmp;
    }

    public static SKBitmap CopyChangeTransparentTo(SKBitmap img, SKColor c, byte trans, int start = 0, int end = -1)
    {
        var bmp = img.Copy();
        bmp.ChangeTransparentTo(c, trans, start, end);
        return bmp;
    }

    public static SKBitmap CopyWritePixels(SKBitmap img, SKColor c, int start, int end)
    {
        var bmp = img.Copy();
        bmp.WritePixels(c, start, end);
        return bmp;
    }

    public static SKBitmap GetBitmap(ReadOnlySpan<byte> data, int width, int height, int length)
    {
        var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        var bmp = new SKBitmap(info);
        var span = bmp.GetPixelSpan();
        data[..length].CopyTo(span);
        return bmp;
    }

    public static SKBitmap GetBitmap(ReadOnlySpan<byte> data, int width, int height)
    {
        return GetBitmap(data, width, height, data.Length);
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

    private static void SetAllTransparencyTo(Span<byte> data, SKColor c, byte trans)
    {
        var arr = MemoryMarshal.Cast<byte, int>(data);
        var value = ToArgbInt(c, trans);
        for (int i = data.Length - 4; i >= 0; i -= 4)
        {
            if (data[i + 3] == 0)
                arr[i >> 2] = value;
        }
    }

    private static void BlendAllTransparencyTo(Span<byte> data, SKColor c, byte trans)
    {
        var arr = MemoryMarshal.Cast<byte, int>(data);
        var value = ToArgbInt(c, trans);
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

    private static void ChangeAllTo(Span<byte> data, SKColor c, int start, int end)
    {
        var arr = MemoryMarshal.Cast<byte, int>(data[start..end]);
        var value = ToArgbInt(c);
        arr.Fill(value);
    }

    public static void ChangeAllColorTo(Span<byte> data, SKColor c)
    {
        byte R = c.Red;
        byte G = c.Green;
        byte B = c.Blue;
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
            if (data[i + 3] == 0)
                continue;

            int x = (i % stride) / 4;
            int y = i / stride;
            {
                int left = Math.Max(0, x - reach);
                int right = Math.Min(width - 1, x + reach);
                int top = Math.Max(0, y - reach);
                int bottom = Math.Min(height - 1, y + reach);
                for (int ix = left; ix <= right; ix++)
                {
                    for (int iy = top; iy <= bottom; iy++)
                    {
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
            if (data[i + 3] != 0)
                continue;

            var transparency = data[i + PollutePixelColorIndex];
            if (transparency == 0)
                continue;

            data[i + 0] = blue;
            data[i + 1] = green;
            data[i + 2] = red;
            data[i + 3] = transparency;
        }
    }

    /// <summary>
    /// Converts an SKColor to an ARGB int value (same format as System.Drawing.Color.ToArgb()).
    /// In BGRA8888 pixel format, this int value represents the pixel correctly on little-endian systems.
    /// </summary>
    private static int ToArgbInt(SKColor c, byte? alphaOverride = null)
    {
        byte a = alphaOverride ?? c.Alpha;
        return (a << 24) | (c.Red << 16) | (c.Green << 8) | c.Blue;
    }
}
