using System.Runtime.InteropServices;
using System;

namespace PKHeX.Core;

/// <summary>
/// Simple record to store the unique tile and color count of a <see cref="ITiledImage"/>.
/// </summary>
/// <param name="TileCount">Count of unique tiles in the data</param>
/// <param name="ColorCount">Count of unique colors in the data</param>
public readonly record struct TiledImageStat(int TileCount, int ColorCount);

/// <summary>
/// Logic to calculate a <see cref="TiledImageStat"/> from an <see cref="ITiledImage"/>.
/// </summary>
public static class TiledImageUtil
{
    /// <summary>
    /// Get the unique tile and color count of the <see cref="ITiledImage"/>.
    /// </summary>
    /// <remarks>Useful if you want to inspect a tiled image directly without rebuilding between image file.</remarks>
    public static TiledImageStat GetStats(this ITiledImage img)
    {
        var tileCount = GetUniqueTileCount(img.Tiles);
        var colorCount = GetUniqueColorCount(img.Colors);
        return new TiledImageStat(tileCount, colorCount);
    }

    private static int GetUniqueColorCount(ReadOnlySpan<byte> imgColors)
    {
        var u16 = MemoryMarshal.Cast<byte, ushort>(imgColors);
        int count = 1; // first color is always unique
        for (int i = 1; i < u16.Length; i++)
        {
            var color = u16[i];
            if (!u16[..i].Contains(color))
                count++;
        }

        return count;
    }

    private static int GetUniqueTileCount(ReadOnlySpan<byte> data)
    {
        // Get count of unique tiles, based on unique sequences of bytes
        int count = 1; // first tile is always unique
        const int length = PaletteTile.SIZE;
        for (int i = length; i < data.Length; i += length)
        {
            var tile = data.Slice(i, length);
            if (IsUniqueTile(data[..i], tile))
                count++;
        }

        return count;
    }

    private static bool IsUniqueTile(ReadOnlySpan<byte> otherTiles, ReadOnlySpan<byte> tile)
    {
        const int length = PaletteTile.SIZE;
        for (int i = 0; i < otherTiles.Length; i += length)
        {
            var other = otherTiles.Slice(i, length);
            if (tile.SequenceEqual(other))
                return false;
        }
        return true;
    }
}
