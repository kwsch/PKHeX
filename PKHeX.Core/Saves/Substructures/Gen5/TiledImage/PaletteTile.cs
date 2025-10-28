using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 image tile composed of 8x8 pixels.
/// </summary>
/// <remarks>Each pixel's color choice is a nibble (4 bits) from a color palette.</remarks>
/// <see cref="ITiledImage"/>
public static class PaletteTile
{
    internal const int SIZE = (TileWidth * TileHeight) / 2; // 0x20
    private const int TileSize = 8;
    private const int TileWidth = TileSize;
    private const int TileHeight = TileSize;

    public static PaletteTileRotation GetRotationValue(ReadOnlySpan<byte> tileColors, ReadOnlySpan<byte> self)
    {
        // Check all rotation types
        if (tileColors.SequenceEqual(self))
            return PaletteTileRotation.None;

        if (IsMirrorX(tileColors, self))
            return PaletteTileRotation.FlipX;
        if (IsMirrorY(tileColors, self))
            return PaletteTileRotation.FlipY;
        if (IsMirrorXY(tileColors, self))
            return PaletteTileRotation.FlipXY;

        return PaletteTileRotation.Invalid;
    }

    private static bool IsMirrorX(ReadOnlySpan<byte> tileColors, ReadOnlySpan<byte> self)
    {
        const int pixels = TileWidth * TileHeight;
        for (int i = 0; i < pixels; i++)
        {
            var index = (7 - (i & 7)) + (8 * (i / 8));
            if (self[index] != tileColors[i])
                return false;
        }
        return true;
    }

    private static bool IsMirrorY(ReadOnlySpan<byte> tileColors, ReadOnlySpan<byte> self)
    {
        const int pixels = TileWidth * TileHeight;
        for (int i = 0; i < pixels; i++)
        {
            var index = pixels - (8 * (1 + (i / 8))) + (i & 7);
            if (self[index] != tileColors[i])
                return false;
        }
        return true;
    }

    private static bool IsMirrorXY(ReadOnlySpan<byte> tileColors, ReadOnlySpan<byte> self)
    {
        const int pixels = TileWidth * TileHeight;
        for (int i = 0; i < pixels; i++)
        {
            var index = pixels - 1 - i;
            if (self[index] != tileColors[i])
                return false;
        }
        return true;
    }
}
