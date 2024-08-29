using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Lower screen tiled image data for the C-Gear.
/// </summary>
/// <param name="Raw">Data buffer storing the skin.</param>
/// <remarks>Image tiles, color palette, and tile arrangement map.</remarks>
public abstract class CGearBackground(Memory<byte> Raw) : ITiledImage
{
    /// <summary> Pixel width of the background image. </summary>
    public const int Width = 256; // px
    /// <summary> Pixel height of the background image. </summary>
    public const int Height = 192; // px

    /// <summary> Byte size of the C-Gear background image. </summary>
    public const int SIZE = OffsetTileMap + LengthTileMap; // 0x2600

    private const int ColorCount = 0x10;
    private const int TilePoolCount = 0xFF;

    private const int TileSize = 8;
    private const int TileCount = (Width / TileSize) * (Height / TileSize); // 0x300

    private const int LengthTilePool = TilePoolCount * PaletteTile.SIZE; // 0x1FE0
    private const int LengthColorData = ColorCount * sizeof(ushort); // 0x20
    private const int OffsetTileMap = LengthTilePool + LengthColorData; // 0x2000
    private const int LengthTileMap = TileCount * sizeof(ushort); // 0x600

    static int ITiledImage.Width => Width;
    static int ITiledImage.Height => Height;
    static int ITiledImage.PixelCount => Height * Width;
    static int ITiledImage.ColorCount => ColorCount;
    static int ITiledImage.TilePoolCount => TilePoolCount;
    static int ITiledImage.TileSize => TileSize;

    public Span<byte> Data => Raw.Span;

    /* 0x0000 - 0x1FDF*/ public Span<byte> Tiles => Data[..LengthTilePool];
    /* 0x1FE0 - 0x1FFF*/ public Span<byte> Colors => Data.Slice(LengthTilePool, LengthColorData);
    /* 0x2000 - 0x25FF*/ public Span<byte> Arrange => Data.Slice(OffsetTileMap, LengthTileMap);

    public Span<byte> GetTileData(int tile) => Tiles.Slice(tile * PaletteTile.SIZE, PaletteTile.SIZE);
    public Span<byte> GetColorData(int color) => Colors.Slice(color * sizeof(ushort), sizeof(ushort));

    public ushort GetArrange(int tilePositionIndex) => ReadUInt16LittleEndian(Arrange[(tilePositionIndex * 2)..]);
    public void SetArrange(int tilePositionIndex, ushort value) => WriteUInt16LittleEndian(Arrange[(tilePositionIndex * 2)..], value);
}
