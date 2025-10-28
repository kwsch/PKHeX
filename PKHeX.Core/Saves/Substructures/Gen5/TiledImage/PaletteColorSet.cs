using System;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for interacting with a palette color set.
/// </summary>
/// <see cref="ITiledImage"/>
public static class PaletteColorSet
{
    /// <summary>
    /// Converts a 32-bit ARGB pixel list to a 15-bit color.
    /// </summary>
    public static int GetUniqueColorsFromPixelData(ReadOnlySpan<byte> data, Span<int> result)
    {
        int count = 0;
        var pixels = MemoryMarshal.Cast<byte, int>(data);
        foreach (var p in pixels)
        {
            var pixel = p;
            if (!BitConverter.IsLittleEndian)
                pixel = ReverseEndianness(pixel);

            // Don't support transparency in pixels
            pixel = Color15Bit.GetColorAsOpaque(pixel);

            int index = result[..count].IndexOf(pixel);
            if (index != -1)
                continue; // already exists
            if (count == result.Length)
                continue; // too many unique colors
            result[count++] = pixel;
        }
        return count;
    }

    public static void Read(ReadOnlySpan<byte> data, Span<int> colors)
    {
        var buffer = MemoryMarshal.Cast<byte, ushort>(data)[..colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            var value = buffer[i];
            if (!BitConverter.IsLittleEndian)
                value = ReverseEndianness(value);
            colors[i] = Color15Bit.GetColorExpand(value);
        }
    }

    public static void Write(ReadOnlySpan<int> colors, Span<byte> data)
    {
        var buffer = MemoryMarshal.Cast<byte, ushort>(data)[..colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            var value = Color15Bit.GetColorCompress(colors[i]);
            if (!BitConverter.IsLittleEndian)
                value = ReverseEndianness(value);
            buffer[i] = value;
        }
    }
}
