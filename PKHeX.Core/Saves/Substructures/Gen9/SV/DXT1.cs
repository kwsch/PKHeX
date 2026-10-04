using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.TextureUtil;

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
}
