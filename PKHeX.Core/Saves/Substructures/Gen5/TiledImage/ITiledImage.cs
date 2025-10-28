using System;

namespace PKHeX.Core;

/// <summary>
/// Tiled Image interface for Generation 5 C-Gear Backgrounds and PokéDex Skins.
/// </summary>
public interface ITiledImage
{
    /// <summary> Section of the data buffer storing all the tiles. </summary>
    Span<byte> Tiles { get; }
    /// <summary> Section of the data buffer storing the color palette. </summary>
    Span<byte> Colors { get; }
    /// <summary> Section of the data buffer storing the tile arrangement map. </summary>
    Span<byte> Arrange { get; }

    /// <summary> Get the tile color choice data for a specific tile index. </summary>
    Span<byte> GetTileData(int tile);

    /// <summary> Count of pixels in the image. </summary>
    static abstract int PixelCount { get; }
    /// <summary> Count of unique colors in the image. </summary>
    static abstract int ColorCount { get; }
    /// <summary> Count of tiles in the tile pool. </summary>
    static abstract int TilePoolCount { get; }
    /// <summary> Count of pixels for a side (width/height) of a tile. </summary>
    static abstract int TileSize { get; }
    /// <summary> Pixel width of the image. </summary>
    static abstract int Width { get; }
    /// <summary> Pixel height of the image. </summary>
    static abstract int Height { get; }
}
