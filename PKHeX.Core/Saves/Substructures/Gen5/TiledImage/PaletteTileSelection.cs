using System;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for reading and writing the palette tile selection format.
/// </summary>
/// <remarks>
/// The structure is a 16-bit packed value with the following format:
/// <code>
/// 6 bits tile index
/// 2 bits flip y|x
/// 4 bits palette type
/// </code>
/// </remarks>
public static class PaletteTileSelection
{
    /// <summary>
    /// Breaks down the selection value into its components.
    /// </summary>
    public static void DecomposeSelection(ushort value, out ushort choice, out byte flip, out byte palette)
    {
        choice = (ushort)(value & 0x3FF);
        flip = (byte)((value >> 10) & 0b11);
        palette = (byte)((value >> 12) & 0xF);
    }

    /// <summary>
    /// Combines the components into a selection value.
    /// </summary>
    public static ushort ComposeSelection(ushort choice, byte flip, byte palette)
    {
        return (ushort)((palette << 12) | ((flip & 3) << 10) | (choice & 0x3FF));
    }

    /// <summary>
    /// Checks if any of the selection values are in the shifted tile arrangement format.
    /// </summary>
    public static bool IsPaletteShiftFormat(ReadOnlySpan<byte> data)
    {
        var u16 = MemoryMarshal.Cast<byte, ushort>(data);
        foreach (ushort v in u16)
        {
            ushort value = v;
            if (!BitConverter.IsLittleEndian)
                value = ReverseEndianness(value);
            DecomposeSelection(value, out var choice, out _, out var palette);
            if (palette != 0)
                return true;
            if (choice > 0xFF)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Converts the selection values to the shifted tile arrangement format.
    /// </summary>
    /// <remarks>Used to convert from B2/W2 to B/W.</remarks>
    public static void ConvertToShiftFormat<T>(Span<byte> data) where T : ITiledImage
    {
        var u16 = MemoryMarshal.Cast<byte, ushort>(data);
        for (int i = 0; i < u16.Length; i++)
        {
            ushort value = u16[i];
            if (!BitConverter.IsLittleEndian)
                value = ReverseEndianness(value);
            DecomposeSelection(value, out var choice, out var flip, out var palette);
            if (palette != 0)
                throw new ArgumentException("Palette shift format already present.");
            if (choice >= T.TilePoolCount)
                throw new ArgumentException("Choice value too large for shift format.");

            choice = CGearBackgroundBW.GetLayoutIndex(choice);
            u16[i] = ComposeSelection(choice, flip, 10);
        }
    }

    /// <summary>
    /// Converts the selection values from the shifted tile arrangement format.
    /// </summary>
    /// <remarks>Used to convert from B/W to B2/W2.</remarks>
    public static void ConvertFromShiftFormat(Span<byte> data)
    {
        var u16 = MemoryMarshal.Cast<byte, ushort>(data);
        for (int i = 0; i < u16.Length; i++)
        {
            ushort value = u16[i];
            if (!BitConverter.IsLittleEndian)
                value = ReverseEndianness(value);
            DecomposeSelection(value, out var choice, out var flip, out _);
            choice = CGearBackgroundBW.GetVisualIndex(choice);
            u16[i] = ComposeSelection(choice, flip, 0);
        }
    }

    /// <summary>
    /// Reads all the layout details from the arrangement list.
    /// </summary>
    /// <param name="data">Buffer containing the arrangement list.</param>
    /// <param name="choice">Result storage for tile choice at index i</param>
    /// <param name="flip">Result storage for flip type at index i</param>
    /// <param name="palette">Result storage for palette type at index i</param>
    public static void ReadLayoutDetails(ReadOnlySpan<byte> data, Span<ushort> choice, Span<byte> flip, Span<byte> palette)
    {
        var u16 = MemoryMarshal.Cast<byte, ushort>(data);
        for (int i = 0; i < choice.Length; i++)
        {
            var val = u16[i];
            if (!BitConverter.IsLittleEndian)
                val = ReverseEndianness(val);
            DecomposeSelection(val, out choice[i], out flip[i], out palette[i]);
        }
    }

    /// <summary>
    /// Writes all the layout details to the arrangement list.
    /// </summary>
    /// <param name="data">Buffer containing the arrangement list.</param>
    /// <param name="choice">Input storage for tile choice at index i</param>
    /// <param name="flip">Input storage for flip type at index i</param>
    /// <param name="palette">Input storage for palette type at index i</param>
    public static void WriteLayoutDetails(Span<byte> data, ReadOnlySpan<ushort> choice, ReadOnlySpan<byte> flip, ReadOnlySpan<byte> palette)
    {
        var u16 = MemoryMarshal.Cast<byte, ushort>(data);
        for (int i = 0; i < choice.Length; i++)
        {
            var val = ComposeSelection(choice[i], flip[i], palette[i]);
            if (!BitConverter.IsLittleEndian)
                val = ReverseEndianness(val);
            u16[i] = val;
        }
    }

    /// <inheritdoc cref="WriteLayoutDetails(Span{byte}, ReadOnlySpan{ushort}, ReadOnlySpan{byte}, ReadOnlySpan{byte})"/>
    public static void WriteLayoutDetails(Span<byte> data, ReadOnlySpan<ushort> choice, ReadOnlySpan<byte> flip)
    {
        var u16 = MemoryMarshal.Cast<byte, ushort>(data);
        for (int i = 0; i < choice.Length; i++)
        {
            var val = ComposeSelection(choice[i], flip[i], 0);
            if (!BitConverter.IsLittleEndian)
                val = ReverseEndianness(val);
            u16[i] = val;
        }
    }
}
