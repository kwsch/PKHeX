using System;

namespace PKHeX.Core;

/// <summary>
/// Converts 15-bit colors (no transparency) to opaque 32-bit RBG Color values
/// </summary>
public static class Color15Bit
{
    /// <summary>
    /// Converts a 32-bit color value to an opaque 32-bit RGB color value
    /// </summary>
    public static int GetColorAsOpaque(int val) => unchecked((int)0xFF_000000) | val; // Force opaque

    /// <summary>
    /// Converts a 15-bit color value to an opaque 32-bit RGB color value
    /// </summary>
    public static int GetColorExpand(ushort val)
    {
        int R = (val >> 0) & 0x1F;
        int G = (val >> 5) & 0x1F;
        int B = (val >> 10) & 0x1F;

        R = Convert5To8[R];
        G = Convert5To8[G];
        B = Convert5To8[B];

        return (0xFF << 24) | (R << 16) | (G << 8) | B;
    }

    /// <summary>
    /// Converts a 32-bit color value to a 15-bit color value
    /// </summary>
    public static ushort GetColorCompress(int rgb)
    {
        var R = (byte)(rgb >> 16);
        var G = (byte)(rgb >> 8);
        var B = (byte)(rgb >> 0);

        int val = 0;
        val |= Convert8to5(R) << 0;
        val |= Convert8to5(G) << 5;
        val |= Convert8to5(B) << 10;
        return (ushort)val;
    }

    /// <summary>
    /// Converts a color value from 8-bit to 5-bit
    /// </summary>
    /// <param name="value">8-bit color value</param>
    /// <returns>5-bit color value</returns>
    private static byte Convert8to5(byte value)
    {
        byte i = 0;
        while (value > Convert5To8[i])
            i++;
        return i;
    }

    /// <summary>
    /// Interpolated color gradient lookup table for 5-bit to 8-bit expansion.
    /// </summary>
    private static ReadOnlySpan<byte> Convert5To8 => // 0x20 entries
    [
        0x00,0x08,0x10,0x18,0x20,0x29,0x31,0x39,
        0x41,0x4A,0x52,0x5A,0x62,0x6A,0x73,0x7B,
        0x83,0x8B,0x94,0x9C,0xA4,0xAC,0xB4,0xBD,
        0xC5,0xCD,0xD5,0xDE,0xE6,0xEE,0xF6,0xFF,
    ];
}
