using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 C-Gear Background Image
/// </summary>
public sealed class CGearBackground
{
    public const string Extension = "cgb";
    public const string Filter = $"C-Gear Background|*.{Extension}";
    public const int Width = 256; // px
    public const int Height = 192; // px
    private const int ColorCount = 0x10;
    private const int TileSize = 8;
    private const int TileCount = (Width / TileSize) * (Height / TileSize); // 0x300

    internal const int CountTilePool = 0xFF;
    private const int LengthTilePool = CountTilePool * Tile.SIZE_TILE; // 0x1FE0
    private const int LengthColorData = ColorCount * sizeof(ushort); // 0x20
    private const int OffsetTileMap = LengthTilePool + LengthColorData; // 0x2000
    private const int LengthTileMap = TileCount * sizeof(ushort); // 0x600

    public const int SIZE_CGB = OffsetTileMap + LengthTileMap; // 0x2600

    /* CGearBackground Documentation
    * CGearBackgrounds (.cgb) are tiled images.
    * Tiles are 8x8, and serve as a tileset for building the image.
    * The first 0x2000 bytes are the tile building region.
    * A tile to have two pixels defined in one byte of space.
    * A tile takes up 64 pixels, 32 bytes, 0x20 chunks.
    * The last tile is actually the colors used in the image (16bit).
    * Only 16 colors can be used for the entire image.
    * 255 tiles may be chosen from, as (0x2000-(0x20))/0x20 = 0xFF
    * The last 0x600 bytes are the tiles used.
    * 256/8 = 32, 192/8 = 24
    * 32 * 24 = 0x300
    * The tiles are chosen based on the 16bit index of the tile.
    * 0x300 * 2 = 0x600!
    *
    * CGearBackgrounds tilemap (when stored on B/W) employs some obfuscation.
    * B/W obfuscates by adding 0xA0A0.
    * The obfuscated number is then tweaked by adding 15*(i/17)
    * To reverse, use a similar reverse calculation
    * PSK files are basically raw game rips (obfuscated)
    * CGB files are un-obfuscated / B2/W2.
    * Due to B/W and B2/W2 using different obfuscation adds, PSK files are incompatible between the versions.
    */

    public readonly int[] ColorPalette;
    public readonly Tile[] Tiles;
    public readonly TileMap Map;

    public CGearBackground(ReadOnlySpan<byte> data)
    {
        if (data.Length != SIZE_CGB)
            throw new ArgumentOutOfRangeException(nameof(data));

        var dataTiles = data[..LengthTilePool];
        var dataColors = data.Slice(LengthTilePool, LengthColorData);
        var dataArrange = data.Slice(OffsetTileMap, LengthTileMap);

        Tiles = ReadTiles(dataTiles);
        ColorPalette = ReadColorPalette(dataColors);
        Map = new TileMap(dataArrange);

        foreach (var tile in Tiles)
            tile.SetTile(ColorPalette);
    }

    private CGearBackground(int[] palette, Tile[] tilelist, TileMap tm)
    {
        Map = tm;
        ColorPalette = palette;
        Tiles = tilelist;
    }

    /// <summary>
    /// Writes the <see cref="CGearBackground"/> data to a binary form.
    /// </summary>
    /// <param name="data">Destination buffer to write the skin to</param>
    /// <param name="cgb">True if the destination game is <see cref="GameVersion.B2W2"/>, otherwise false for <see cref="GameVersion.BW"/>.</param>
    /// <returns>Serialized skin data for writing to the save file</returns>
    public void Write(Span<byte> data, bool cgb)
    {
        var dataTiles = data[..LengthTilePool];
        var dataColors = data.Slice(LengthTilePool, LengthColorData);
        var dataArrange = data.Slice(OffsetTileMap, LengthTileMap);

        WriteTiles(dataTiles, Tiles);
        WriteColorPalette(dataColors, ColorPalette);
        Map.Write(dataArrange, cgb);
    }

    private static Tile[] ReadTiles(ReadOnlySpan<byte> data)
    {
        var result = new Tile[data.Length / Tile.SIZE_TILE];
        for (int i = 0; i < result.Length; i++)
        {
            var span = data.Slice(i * Tile.SIZE_TILE, Tile.SIZE_TILE);
            result[i] = new Tile(span);
        }
        return result;
    }

    private static void WriteTiles(Span<byte> data, ReadOnlySpan<Tile> tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            var tile = tiles[i];
            var span = data.Slice(i * Tile.SIZE_TILE, Tile.SIZE_TILE);
            tile.Write(span);
        }
    }

    private static int[] ReadColorPalette(ReadOnlySpan<byte> data)
    {
        var colors = new int[data.Length / 2]; // u16->rgb32
        ReadColorPalette(data, colors);
        return colors;
    }

    private static void ReadColorPalette(ReadOnlySpan<byte> data, Span<int> colors)
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

    private static void WriteColorPalette(Span<byte> data, ReadOnlySpan<int> colors)
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

    /// <summary>
    /// Creates a new C-Gear Background object from an input image data byte array, with 32 bits per pixel.
    /// </summary>
    /// <param name="data">Image data</param>
    /// <returns>new C-Gear Background object</returns>
    public static CGearBackground GetBackground(ReadOnlySpan<byte> data)
    {
        const int bpp = 4;
        const int expectLength = Width * Height * bpp;
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, expectLength);

        var colors = GetColorData(data);
        var palette = colors.Distinct().ToArray();
        if (palette.Length > ColorCount)
            throw new ArgumentException($"Too many unique colors. Expected <= 16, got {palette.Length}");

        var tiles = GetTiles(colors, palette);
        GetTileList(tiles, out List<Tile> tilelist, out TileMap tm);
        if (tilelist.Count >= CountTilePool)
            throw new ArgumentException($"Too many unique tiles. Expected < 256, received {tilelist.Count}.");

        // Finished!
        return new CGearBackground(palette, [.. tilelist], tm);
    }

    private static int[] GetColorData(ReadOnlySpan<byte> data)
    {
        var pixels = MemoryMarshal.Cast<byte, int>(data);
        int[] colors = new int[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            var pixel = pixels[i];
            if (!BitConverter.IsLittleEndian)
                pixel = ReverseEndianness(pixel);
            colors[i] = Color15Bit.GetColorAsOpaque(pixel);
        }
        return colors;
    }

    private static Tile[] GetTiles(ReadOnlySpan<int> colors, ReadOnlySpan<int> palette)
    {
        var tiles = new Tile[TileCount];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = GetTile(colors, palette, i);
        return tiles;
    }

    private static Tile GetTile(ReadOnlySpan<int> colors, ReadOnlySpan<int> palette, int tileIndex)
    {
        int x = (tileIndex * 8) % Width;
        int y = 8 * ((tileIndex * 8) / Width);

        var t = new Tile();
        var choices = t.ColorChoices;
        for (uint ix = 0; ix < 8; ix++)
        {
            for (uint iy = 0; iy < 8; iy++)
            {
                int index = ((int) (y + iy) * Width) + (int) (x + ix);
                var c = colors[index];

                choices[(ix % 8) + (iy * 8)] = (byte)palette.IndexOf(c);
            }
        }

        t.SetTile(palette);
        return t;
    }

    private static void GetTileList(ReadOnlySpan<Tile> tiles, out List<Tile> tilelist, out TileMap tm)
    {
        tilelist = [tiles[0]];
        tm = new TileMap(LengthTileMap);

        // start at 1 as the 0th tile is always non-duplicate
        for (int i = 1; i < tm.TileChoices.Length; i++)
            FindPossibleRotatedTile(tiles[i], tilelist, tm, i);
    }

    private static void FindPossibleRotatedTile(Tile t, List<Tile> tilelist, TileMap tm, int tileIndex)
    {
        // Test all tiles currently in the list
        for (int i = 0; i < tilelist.Count; i++)
        {
            var rotVal = t.GetRotationValue(tilelist[i].ColorChoices);
            if (rotVal == Tile.ROTATION_BAD)
                continue;
            tm.TileChoices[tileIndex] = (byte)i;
            tm.Rotations[tileIndex] = rotVal;
            return;
        }

        // No tile found, add to list
        tilelist.Add(t);
        tm.TileChoices[tileIndex] = (byte)(tilelist.Count - 1);
        tm.Rotations[tileIndex] = 0;
    }

    public byte[] GetImageData()
    {
        byte[] data = new byte[4 * Width * Height];
        WriteImageData(data);
        return data;
    }

    private void WriteImageData(Span<byte> data)
    {
        var tiles = Map.TileChoices;
        var rotations = Map.Rotations;
        for (int i = 0; i < tiles.Length; i++)
        {
            var choice = tiles[i];
            var rotation = rotations[i];
            var tile = Tiles[choice];
            var tileData = tile.Rotate(rotation);

            int x = (i * TileSize) % Width;
            int y = TileSize * ((i * TileSize) / Width);

            for (int row = 0; row < TileSize; row++)
            {
                const int pixelLineSize = TileSize * sizeof(int);
                int ofsSrc = row * pixelLineSize;
                int ofsDest = (((y + row) * Width) + x) * sizeof(int);
                var line = tileData.Slice(ofsSrc, pixelLineSize);
                line.CopyTo(data[ofsDest..]);
            }
        }
    }
}

/// <summary>
/// Generation 5 image tile composed of 8x8 pixels.
/// </summary>
/// <remarks>Each pixel's color choice is a nibble (4 bits).</remarks>
public sealed class Tile
{
    internal const int SIZE_TILE = 0x20;
    private const int TileWidth = 8;
    private const int TileHeight = 8;
    internal readonly byte[] ColorChoices = new byte[TileWidth * TileHeight];

    // Keep track of known rotations for this tile.
    // If the tile's rotated value has not yet been calculated, the field is null.
    private byte[] PixelData = [];
    private byte[]? PixelDataX;
    private byte[]? PixelDataY;

    private const byte FlagFlipX = 0b0100; // 0x4
    private const byte FlagFlipY = 0b1000; // 0x8
    private const byte FlagFlipXY = FlagFlipX | FlagFlipY; // 0xC
    private const byte FlagFlipNone = 0b0000; // 0x0
    internal const byte ROTATION_BAD = byte.MaxValue;

    internal Tile() { }

    internal Tile(ReadOnlySpan<byte> data) : this()
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, SIZE_TILE);

        // Unpack the nibbles into the color choice array.
        for (int i = 0; i < data.Length; i++)
        {
            var value = data[i];
            var ofs = i * 2;
            ColorChoices[ofs + 0] = (byte)(value & 0xF);
            ColorChoices[ofs + 1] = (byte)(value >> 4);
        }
    }

    internal void SetTile(ReadOnlySpan<int> palette) => PixelData = GetTileData(palette, ColorChoices);

    private static byte[] GetTileData(ReadOnlySpan<int> Palette, ReadOnlySpan<byte> choices)
    {
        byte[] data = new byte[choices.Length * 4];
        SetTileData(data, Palette, choices);
        return data;
    }

    private static void SetTileData(Span<byte> result, ReadOnlySpan<int> palette, ReadOnlySpan<byte> colorChoices)
    {
        for (int i = 0; i < colorChoices.Length; i++)
        {
            var choice = colorChoices[i];
            var value = palette[choice];
            var span = result.Slice(4 * i, 4);
            WriteInt32LittleEndian(span, value);
        }
    }

    public void Write(Span<byte> data) => Write(data, ColorChoices);

    private static void Write(Span<byte> data, ReadOnlySpan<byte> colorChoices)
    {
        for (int i = 0; i < data.Length; i++)
        {
            var span = colorChoices.Slice(i * 2, 2);
            var second = span[1] & 0xF;
            var first = span[0] & 0xF;
            data[i] = (byte)(first | (second << 4));
        }
    }

    public ReadOnlySpan<byte> Rotate(int rotFlip)
    {
        if (rotFlip == 0)
            return PixelData;
        if ((rotFlip & FlagFlipXY) == FlagFlipXY)
            return FlipY(PixelDataX ??= FlipX(PixelData, TileWidth), TileHeight);
        if ((rotFlip & FlagFlipX) == FlagFlipX)
            return PixelDataX ??= FlipX(PixelData, TileWidth);
        if ((rotFlip & FlagFlipY) == FlagFlipY)
            return PixelDataY ??= FlipY(PixelData, TileHeight);
        return PixelData;
    }

    private static byte[] FlipX(ReadOnlySpan<byte> data, [ConstantExpected(Min = 0)] int width, [ConstantExpected(Min = 4, Max = 4)] int bpp = 4)
    {
        byte[] result = new byte[data.Length];
        FlipX(data, result, width, bpp);
        return result;
    }

    private static byte[] FlipY(ReadOnlySpan<byte> data, [ConstantExpected(Min = 0)] int height, [ConstantExpected(Min = 4, Max = 4)] int bpp = 4)
    {
        byte[] result = new byte[data.Length];
        FlipY(data, result, height, bpp);
        return result;
    }

    private static void FlipX(ReadOnlySpan<byte> data, Span<byte> result, [ConstantExpected(Min = 0)] int width, [ConstantExpected(Min = 4, Max = 4)] int bpp)
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

    private static void FlipY(ReadOnlySpan<byte> data, Span<byte> result, [ConstantExpected(Min = 0)] int height, [ConstantExpected(Min = 4, Max = 4)] int bpp)
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

    internal byte GetRotationValue(ReadOnlySpan<byte> tileColors)
    {
        // Check all rotation types
        if (tileColors.SequenceEqual(ColorChoices))
            return FlagFlipNone;

        if (IsMirrorX(tileColors))
            return FlagFlipX;
        if (IsMirrorY(tileColors))
            return FlagFlipY;
        if (IsMirrorXY(tileColors))
            return FlagFlipXY;

        return ROTATION_BAD;
    }

    private bool IsMirrorX(ReadOnlySpan<byte> tileColors)
    {
        const int pixels = TileWidth * TileHeight;
        for (int i = 0; i < pixels; i++)
        {
            var index = (7 - (i & 7)) + (8 * (i / 8));
            if (ColorChoices[index] != tileColors[i])
                return false;
        }

        return true;
    }

    private bool IsMirrorY(ReadOnlySpan<byte> tileColors)
    {
        const int pixels = TileWidth * TileHeight;
        for (int i = 0; i < pixels; i++)
        {
            var index = pixels - (8 * (1 + (i / 8))) + (i & 7);
            if (ColorChoices[index] != tileColors[i])
                return false;
        }

        return true;
    }

    private bool IsMirrorXY(ReadOnlySpan<byte> tileColors)
    {
        const int pixels = TileWidth * TileHeight;
        for (int i = 0; i < pixels; i++)
        {
            var index = pixels - 1 - i;
            if (ColorChoices[index] != tileColors[i])
                return false;
        }

        return true;
    }
}

public sealed class TileMap(int length)
{
    public readonly byte[] TileChoices = new byte[length / 2];
    public readonly byte[] Rotations = new byte[length / 2];

    internal TileMap(ReadOnlySpan<byte> data) : this(data.Length) => LoadData(data, TileChoices, Rotations);

    private static void LoadData(ReadOnlySpan<byte> data, Span<byte> tiles, Span<byte> rotations)
    {
        bool isCGB = IsCGB(data);
        if (!isCGB)
            LoadDataPSK(data, tiles, rotations);
        else
            LoadDataCGB(data, tiles, rotations);
    }

    private static void LoadDataCGB(ReadOnlySpan<byte> data, Span<byte> tiles, Span<byte> rotations)
    {
        for (int i = 0; i < data.Length; i += 2)
        {
            var span = data.Slice(i, 2);
            var tile = span[0];
            var rot = span[1];

            tiles[i / 2] = tile;
            rotations[i / 2] = rot;
        }
    }

    private static void LoadDataPSK(ReadOnlySpan<byte> data, Span<byte> tiles, Span<byte> rotations)
    {
        for (int i = 0; i < data.Length; i += 2)
        {
            var span = data.Slice(i, 2);
            var value = ReadUInt16LittleEndian(span);
            var (tile, rot) = DecomposeValuePSK(value);
            tiles[i / 2] = tile;
            rotations[i / 2] = rot;
        }
    }

    private static bool IsCGB(ReadOnlySpan<byte> data)
    {
        // check odd bytes for anything not rotation flag
        for (int i = 0; i < data.Length; i += 2)
        {
            if ((data[i + 1] & ~0b1100) != 0)
                return false;
        }
        return true;
    }

    public void Write(Span<byte> data, bool cgb) => Write(data, TileChoices, Rotations, cgb);

    private static void Write(Span<byte> data, ReadOnlySpan<byte> choices, ReadOnlySpan<byte> rotations, bool cgb)
    {
        if (choices.Length != rotations.Length)
            throw new ArgumentException($"length of {nameof(TileChoices)} and {nameof(Rotations)} must be equal");
        if (data.Length != choices.Length + rotations.Length)
            throw new ArgumentException($"data length must be twice the length of the {nameof(TileMap)}");

        if (!cgb)
            WriteDataPSK(data, choices, rotations);
        else
            WriteDataCGB(data, choices, rotations);
    }

    private static void WriteDataCGB(Span<byte> data, ReadOnlySpan<byte> choices, ReadOnlySpan<byte> rotations)
    {
        for (int i = 0; i < data.Length; i += 2)
        {
            var span = data.Slice(i, 2);
            span[0] = choices[i / 2];
            span[1] = rotations[i / 2];
        }
    }

    private static void WriteDataPSK(Span<byte> data, ReadOnlySpan<byte> choices, ReadOnlySpan<byte> rotations)
    {
        for (int i = 0; i < data.Length; i += 2)
        {
            var span = data.Slice(i, 2);
            var tile = choices[i / 2];
            var rot = rotations[i / 2];
            int val = GetPSKValue(tile, rot);
            WriteUInt16LittleEndian(span, (ushort)val);
        }
    }

    public static (byte Tile, byte Rotation) DecomposeValuePSK(ushort val)
    {
        ushort value = UnmapPSKValue(val);
        byte tile = (byte)value;
        byte rot = (byte)(value >> 8);
        if (tile == CGearBackground.CountTilePool) // out of range?
            tile = 0;
        return (tile, rot);
    }

    private static ushort UnmapPSKValue(ushort val)
    {
        var rot = val & 0xFC00;
        var trunc = (val & 0x3FF);
        if (trunc is < 0xA0 or > 0x280)
            return (ushort)(rot | CGearBackground.CountTilePool); // default empty
        return (ushort)(rot | ((val & 0x1F) + (17 * ((trunc - 0xA0) >> 5))));
    }

    public static ushort GetPSKValue(byte tile, byte rot)
    {
        if (tile == CGearBackground.CountTilePool) // out of range?
            tile = 0;

        var result = ((rot & 0x0C) << 8) | ((15 * (tile / 17)) + tile + 0xA0) | 0xA000;
        return (ushort)result;
    }
}
