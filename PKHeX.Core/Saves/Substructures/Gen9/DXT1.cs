using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Provides methods for working with DXT1 compressed texture data, including decompression and size calculations.
/// </summary>
/// <remarks>
/// DXT1 is a lossy compression format commonly used for texture data in graphics applications.
/// This class includes methods to calculate the decompressed size of a texture and to decompress DXT1-compressed data into raw RGBA pixel data.
/// </remarks>
public static class DXT1
{
    private const int bpp = 4;

    /// <summary>
    /// Calculates the total decompressed size of an image based on its dimensions.
    /// </summary>
    /// <param name="width">The width of the image, in pixels. Must be a positive integer.</param>
    /// <param name="height">The height of the image, in pixels. Must be a positive integer.</param>
    /// <returns>The total decompressed size of the image, in bytes.</returns>
    public static int GetDecompressedSize(int width, int height) => bpp * width * height;

    /// <inheritdoc cref="Decompress(ReadOnlySpan{byte}, int, int, Span{byte})"/>
    public static byte[] Decompress(ReadOnlySpan<byte> data, int width, int height)
    {
        var result = new byte[GetDecompressedSize(width, height)];
        Decompress(data, width, height, result);
        return result;
    }

    /// <summary>
    /// Decompresses a block-compressed texture into raw pixel data.
    /// </summary>
    /// <remarks>The decompressed pixel data is written in RGBA format, with each pixel represented by 4 bytes
    /// (red, green, blue, and alpha channels). The input data is expected to be in a block-compressed format, where
    /// each 4x4 pixel block is encoded in 8 bytes.</remarks>
    /// <param name="data">The input span containing the compressed texture data. The data must be in a block-compressed format.</param>
    /// <param name="width">The width of the texture in pixels. Must be a multiple of 4.</param>
    /// <param name="height">The height of the texture in pixels. Must be a multiple of 4.</param>
    /// <param name="result">
    /// The output span where the decompressed pixel data will be written.
    /// The span must have sufficient capacity to hold <paramref name="width"/> × <paramref name="height"/> × 4 bytes.
    /// </param>
    public static void Decompress(ReadOnlySpan<byte> data, int width, int height, Span<byte> result)
    {
        int blockCountX = width / bpp;
        int blockCountY = height / bpp;
        Span<Color> colors = stackalloc Color[4];
        for (int y = 0; y < blockCountY; y++)
        {
            for (int x = 0; x < blockCountX; x++)
            {
                int blockOffset = ((y * blockCountX) + x) * 8;
                var span = data.Slice(blockOffset, 8);

                var color0 = ReadUInt16LittleEndian(span);
                var color1 = ReadUInt16LittleEndian(span[2..]);
                uint indices = ReadUInt32LittleEndian(span[4..]);
                GetColors(colors, color0, color1);

                for (int pixelY = 0; pixelY < 4; pixelY++)
                {
                    int baseIndex = (((4 * y) + pixelY) * width) + (x * 4);
                    int baseShift = (4 * pixelY);
                    for (int pixelX = 0; pixelX < 4; pixelX++)
                    {
                        int pixelIndex = baseIndex + pixelX;
                        var dest = result[(pixelIndex * 4)..];

                        int shift = (baseShift + pixelX) << 1;
                        int index = (int)(indices >> shift) & 0x3;
                        var color = colors[index];

                        dest[0] = color.B;
                        dest[1] = color.G;
                        dest[2] = color.R;
                        dest[3] = color.A;
                    }
                }
            }
        }
    }

    private static void GetColors(Span<Color> colors, ushort color0, ushort color1)
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

    private static Color RGB565ToColor(ushort rgb565)
    {
        byte r = (byte)((rgb565 >> 11) & 0x1F);
        byte g = (byte)((rgb565 >> 5) & 0x3F);
        byte b = (byte)(rgb565 & 0x1F);

        r = (byte)(r << 3 | r >> 2);
        g = (byte)(g << 2 | g >> 4);
        b = (byte)(b << 3 | b >> 2);
        return new(0xFF, r, g, b);
    }

    private static Color Lerp(Color c1, Color c2, float t)
    {
        byte r = (byte)(c1.R + ((c2.R - c1.R) * t));
        byte g = (byte)(c1.G + ((c2.G - c1.G) * t));
        byte b = (byte)(c1.B + ((c2.B - c1.B) * t));
        byte a = (byte)(c1.A + ((c2.A - c1.A) * t));

        return new(a, r, g, b);
    }

    private readonly record struct Color(byte A, byte R, byte G, byte B);
}
