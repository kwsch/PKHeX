using System;
using System.Globalization;

namespace PKHeX.Core;

/// <summary>
/// Utility for decompressing textures.
/// </summary>
public static class TextureUtil
{
    public static void GetColors(Span<Color> colors, ushort color0, ushort color1)
    {
        var c0 = colors[0] = RGB565ToColor(color0);
        var c1 = colors[1] = RGB565ToColor(color1);

        if (color0 > color1)
        {
            colors[2] = Lerp(c0, c1, 1f / 3f);
            colors[3] = Lerp(c0, c1, 2f / 3f);
        }
        else
        {
            colors[2] = Lerp(c0, c1, 0.5f);
            colors[3] = default; // 0
        }
    }

    public static Color RGB565ToColor(ushort rgb565)
    {
        byte r = (byte)((rgb565 >> 11) & 0x1F);
        byte g = (byte)((rgb565 >> 5) & 0x3F);
        byte b = (byte)(rgb565 & 0x1F);

        r = (byte)(r << 3 | r >> 2);
        g = (byte)(g << 2 | g >> 4);
        b = (byte)(b << 3 | b >> 2);
        return new(0xFF, r, g, b);
    }

    public static Color Lerp(Color c1, Color c2, float t)
    {
        byte r = (byte)(c1.R + ((c2.R - c1.R) * t));
        byte g = (byte)(c1.G + ((c2.G - c1.G) * t));
        byte b = (byte)(c1.B + ((c2.B - c1.B) * t));
        byte a = (byte)(c1.A + ((c2.A - c1.A) * t));

        return new(a, r, g, b);
    }

    public readonly record struct Color(byte A, byte R, byte G, byte B);
}
