using System;
using System.Runtime.InteropServices;

namespace PKHeX.Core;

/// <summary>
/// Conversion logic for <see cref="ITiledImage"/> objects.
/// </summary>
/// <remarks>Assumes 32-bit ARGB pixel data.</remarks>
public static class TiledImageConverter
{
    private const int bpp = 4; // 32-bit ARGB

    extension<T>(T bg) where T : ITiledImage
    {
        /// <summary>
        /// Allocates a new byte array and fills it with the image data.
        /// </summary>
        public byte[] GetImageData()
        {
            var result = new byte[T.PixelCount * bpp];
            bg.GetImageData(result);
            return result;
        }

        /// <summary>
        /// Fills the <see cref="result"/> buffer with the image data.
        /// </summary>
        /// <param name="result">Pixel data buffer to fill</param>
        /// <exception cref="ArgumentException"></exception>
        public void GetImageData(Span<byte> result)
        {
            if (result.Length < T.PixelCount * bpp) // debug assertion
                throw new ArgumentException($"Result buffer must be {T.PixelCount * bpp} bytes long.");

            // Get color palette
            Span<int> palette = stackalloc int[T.ColorCount];
            PaletteColorSet.Read(bg.Colors, palette);

            // Get tile choices
            var arrange = bg.Arrange;
            Span<ushort> choiceTil = stackalloc ushort[arrange.Length / 2];
            Span<byte>   choiceRot = stackalloc byte[arrange.Length / 2];
            Span<byte>   choicePal = stackalloc byte[arrange.Length / 2];
            PaletteTileSelection.ReadLayoutDetails(arrange, choiceTil, choiceRot, choicePal);
            if (bg is CGearBackgroundBW)
            {
                if (!choiceTil.ContainsAnyExcept<ushort>(0, 0x3FF))
                    return; // no tile data to render.
                foreach (ref var value in choiceTil)
                    value = CGearBackgroundBW.GetVisualIndex(value);
            }

            // Allocate a temp buffer to store the tile w/ palette
            PlaceTiles(bg, result, choiceTil, choiceRot, palette);
        }
    }

    private static void PlaceTiles<T>(T bg, Span<byte> result, Span<ushort> choiceTil, Span<byte> choiceRot, Span<int> palette)
        where T : ITiledImage
    {
        // Allocate a working buffer to store the tile data
        Span<byte> colors = stackalloc byte[PaletteTile.SIZE * 2]; // inflated, not nibbles. one pixel per index
        Span<byte> tileData = stackalloc byte[colors.Length * bpp];
        Span<byte> pixelData = stackalloc byte[colors.Length * bpp];

        for (int i = 0; i < choiceTil.Length; i++)
        {
            var tile = bg.GetTileData(choiceTil[i]);
            var rot = (PaletteTileRotation)choiceRot[i];
            PaletteTileOperations.SplitNibbles(tile, colors);
            PaletteTileOperations.Inflate(colors, palette, pixelData);
            PaletteTileOperations.Flip(pixelData, tileData, T.TileSize, rot, bpp);

            // Drop the tile into the result buffer at the needed coordinate.
            WriteTile<T>(result, i, tileData);
        }
    }

    private static void WriteTile<T>(Span<byte> result, int index, Span<byte> tileData)
        where T : ITiledImage
    {
        int pixelLineSize = T.TileSize * bpp;
        int x = (index * T.TileSize) % T.Width;
        int y = T.TileSize * ((index * T.TileSize) / T.Width);
        for (int row = 0; row < T.TileSize; row++)
        {
            int ofsSrc = row * pixelLineSize;
            int ofsDest = (((y + row) * T.Width) + x) * bpp;
            var line = tileData.Slice(ofsSrc, pixelLineSize);
            line.CopyTo(result[ofsDest..]);
        }
    }

    /// <summary>
    /// Sets the image data from the <see cref="data"/> buffer, converting into the background tile object.
    /// </summary>
    /// <param name="bg">Result to update with the image data</param>
    /// <param name="data">Pixel data buffer to read</param>
    /// <exception cref="ArgumentException"></exception>
    public static TiledImageStat SetImageData<T>(this T bg, ReadOnlySpan<byte> data)
        where T : ITiledImage
    {
        if (data.Length < T.PixelCount * bpp) // debug assertion
            throw new ArgumentException($"Data buffer must be {T.PixelCount * bpp} bytes long.");

        // Get color palette
        Span<int> palette = stackalloc int[T.ColorCount];
        int colorCount = PaletteColorSet.GetUniqueColorsFromPixelData(data, palette);
        if (colorCount < palette.Length) // If too many colors, don't trim. Only the result will indicate.
            palette = palette[..colorCount];

        // Get the tiles
        var arrange = bg.Arrange;
        Span<ushort> choiceTil = stackalloc ushort[arrange.Length / 2];
        Span<byte>  choiceRot = stackalloc byte[arrange.Length / 2];

        // Read tile data from the image; for each new tile, add it to the list.
        // Store temporary tile list as color choices, and expand the window as we add new tiles.
        int usedTiles = 0;
        int totalUniqueTiles = 0;
        Span<byte> tileColors = stackalloc byte[T.TileSize * T.TileSize];
        var maxTiles = bg.Tiles.Length / (tileColors.Length / 2);
        Span<byte> rawTiles = stackalloc byte[tileColors.Length * maxTiles];
        for (int tileIndex = 0; tileIndex < choiceTil.Length; tileIndex++)
        {
            ReadTile<T>(data, tileIndex, tileColors, palette);

            // Search the existing tiles for a match
            var usedTileLength = usedTiles * tileColors.Length;
            var allTiles = rawTiles[..usedTileLength];
            bool match = TryFindRotation(tileColors, allTiles, out var index, out var rot);
            if (!match)
            {
                // Add the new tile to the list
                if (usedTiles < maxTiles)
                {
                    tileColors.CopyTo(rawTiles[usedTileLength..]);
                    index = totalUniqueTiles = usedTiles++;
                }
                else
                {
                    // Impossible to store, so just keep default values and increment the total count of the image.
                    totalUniqueTiles++;
                }
            }

            // Set the tile index and rotation
            choiceTil[tileIndex] = (ushort)index;
            choiceRot[tileIndex] = (byte)rot;
        }

        // Set the data
        bg.Colors.Clear();
        PaletteColorSet.Write(palette, bg.Colors);
        PaletteTileOperations.MergeNibbles(rawTiles, bg.Tiles);
        PaletteTileSelection.WriteLayoutDetails(arrange, choiceTil, choiceRot);

        if (bg is CGearBackgroundBW)
            PaletteTileSelection.ConvertToShiftFormat<CGearBackgroundBW>(bg.Arrange);

        return new TiledImageStat { TileCount = totalUniqueTiles, ColorCount = colorCount };
    }

    private static bool TryFindRotation(ReadOnlySpan<byte> tileColors, ReadOnlySpan<byte> allTiles, out int index, out PaletteTileRotation rot)
    {
        for (int i = 0; i < allTiles.Length; i += tileColors.Length)
        {
            var tile = allTiles.Slice(i, tileColors.Length);
            var check = PaletteTile.GetRotationValue(tileColors, tile);
            if (check == PaletteTileRotation.Invalid)
                continue;
            index = i / tileColors.Length;
            rot = check;
            return true;
        }
        index = 0;
        rot = 0;
        return false;
    }

    private static void ReadTile<T>(ReadOnlySpan<byte> data, int i, Span<byte> tileColors, Span<int> colorChoices)
        where T : ITiledImage
    {
        var tSize = T.TileSize;
        int baseX = (i * tSize) % T.Width;
        int baseY = tSize * ((i * tSize) / T.Width);
        // Inflate to pixel offset
        int ofsSrc = ((baseY * T.Width) + baseX) * bpp;

        var length = tSize * bpp;
        for (int y = 0; y < tSize; y++)
        {
            int ofsDest = ofsSrc + (y * T.Width * bpp);
            var tileRow = data.Slice(ofsDest, length);
            var i32 = MemoryMarshal.Cast<byte, int>(tileRow);

            var slice = tileColors.Slice(y * tSize, T.TileSize);
            PaletteTileOperations.Deflate(i32, colorChoices, slice);
        }
    }
}
