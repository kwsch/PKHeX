using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// 32-bit ARGB pixel converter for interacting with square pixel tiles.
/// </summary>
/// <see cref="ITiledImage"/>
public static class PaletteTileOperations
{
    /// <summary>
    /// Reads the color choices from the <see cref="data"/> into the <see cref="colorChoices"/>.
    /// </summary>
    public static void SplitNibbles(ReadOnlySpan<byte> data, Span<byte> colorChoices)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length * 2, colorChoices.Length);
        for (int i = 0; i < data.Length; i++)
        {
            var value = data[i];
            var span = colorChoices.Slice(i * 2, 2);
            span[1] = (byte)(value >> 4);
            span[0] = (byte)(value & 0xF);
        }
    }

    /// <summary>
    /// Writes the <see cref="colorChoices"/> into the <see cref="data"/>.
    /// </summary>
    public static void MergeNibbles(ReadOnlySpan<byte> colorChoices, Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length * 2, colorChoices.Length);
        for (int i = 0; i < data.Length; i++)
        {
            var span = colorChoices.Slice(i * 2, 2);
            var second = span[1] & 0xF;
            var first = span[0] & 0xF;
            data[i] = (byte)(first | (second << 4));
        }
    }

    /// <summary>
    /// Inflates the <see cref="colorChoices"/> into the <see cref="result"/> using the <see cref="palette"/>.
    /// </summary>
    /// <param name="colorChoices">Color choices for each pixel</param>
    /// <param name="palette">32-bit ARGB palette</param>
    /// <param name="result">Resulting pixel data</param>
    public static void Inflate(ReadOnlySpan<byte> colorChoices, ReadOnlySpan<int> palette, Span<byte> result)
    {
        for (int i = 0; i < colorChoices.Length; i++)
        {
            var choice = colorChoices[i];
            var value = palette[choice];
            var span = result.Slice(4 * i, 4);
            WriteInt32LittleEndian(span, value);
        }
    }

    /// <summary>
    /// Deflates the <see cref="pixels"/> into the <see cref="tileColors"/> using the <see cref="palette"/>.
    /// </summary>
    /// <param name="pixels">Resulting pixel data</param>
    /// <param name="palette">32-bit ARGB palette</param>
    /// <param name="tileColors">Color choices for each pixel</param>
    public static void Deflate(ReadOnlySpan<int> pixels, ReadOnlySpan<int> palette, Span<byte> tileColors)
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            var pixel = pixels[i];
            if (!BitConverter.IsLittleEndian)
                pixel = ReverseEndianness(pixel);
            var color = Color15Bit.GetColorAsOpaque(pixel);
            tileColors[i] = (byte)palette.IndexOf(color);
        }
    }

    /// <summary>
    /// Flips the pixel data across the X axis.
    /// </summary>
    public static void FlipX(ReadOnlySpan<byte> data, Span<byte> result, int width, [ConstantExpected(Min = 4, Max = 4)] int bpp)
    {
        int pixels = data.Length / bpp;
        var resultInt = MemoryMarshal.Cast<byte, int>(result);
        var dataInt = MemoryMarshal.Cast<byte, int>(data);
        for (int i = 0; i < pixels; i++)
        {
            int x = i % width;
            int y = i / width;

            x = width - x - 1; // flip x

            int dest = ((y * width) + x);
            resultInt[dest] = dataInt[i];
        }
    }

    /// <summary>
    /// Flips the pixel data across the Y axis.
    /// </summary>
    public static void FlipY(ReadOnlySpan<byte> data, Span<byte> result, int height, [ConstantExpected(Min = 4, Max = 4)] int bpp)
    {
        int pixels = data.Length / bpp;
        int width = pixels / height;
        var resultInt = MemoryMarshal.Cast<byte, int>(result);
        var dataInt = MemoryMarshal.Cast<byte, int>(data);
        for (int i = 0; i < pixels; i++)
        {
            int x = i % width;
            int y = i / width;

            y = height - y - 1; // flip y

            int dest = ((y * width) + x);
            resultInt[dest] = dataInt[i];
        }
    }

    /// <summary>
    /// Flips the pixel data across both the X and Y axis.
    /// </summary>
    public static void FlipXY(ReadOnlySpan<byte> data, Span<byte> result, int height, [ConstantExpected(Min = 4, Max = 4)] int bpp)
    {
        int pixels = data.Length / bpp;
        int width = pixels / height;
        var resultInt = MemoryMarshal.Cast<byte, int>(result);
        var dataInt = MemoryMarshal.Cast<byte, int>(data);
        for (int i = 0; i < pixels; i++)
        {
            int x = i % width;
            int y = i / width;

            x = width - x - 1; // flip x
            y = height - y - 1; // flip y

            int dest = ((y * width) + x);
            resultInt[dest] = dataInt[i];
        }
    }

    /// <summary>
    /// Flips the 32-bit pixel data across the requested <see cref="rot"/>.
    /// </summary>
    /// <remarks>If no rotation is requested, the data is only copied.</remarks>
    /// <param name="tile">Unpacked (32-bit pixel) tile data to process</param>
    /// <param name="result">Resulting tile data</param>
    /// <param name="tileSize">Size of the tile (width/height)</param>
    /// <param name="rot">Rotation type to apply</param>
    /// <param name="bpp">Bits per pixel, requires 4.</param>
    public static void Flip(ReadOnlySpan<byte> tile, Span<byte> result, int tileSize, PaletteTileRotation rot,
        [ConstantExpected(Min = 4, Max = 4)] int bpp)
    {
        if (rot == PaletteTileRotation.FlipXY)
            FlipXY(tile, result, tileSize, bpp);
        else if (rot.HasFlag(PaletteTileRotation.FlipX))
            FlipX(tile, result, tileSize, bpp);
        else if (rot.HasFlag(PaletteTileRotation.FlipY))
            FlipY(tile, result, tileSize, bpp);
        else
            tile.CopyTo(result);
    }
}
