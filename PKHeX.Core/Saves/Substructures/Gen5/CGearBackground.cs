using System;
using System.Collections.Generic;
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
    private const int CountColors = 0x10;
    private const int LengthColorData = CountColors * 2; // 0x20
    private const int OffsetTileMap = LengthTilePool + LengthColorData; // 0x2000
    private const int LengthTileMap = TileCount * 2; // 0x600

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
    * CGearBackgrounds tilemap (when stored on BW) employs some obfuscation.
    * BW obfuscates by adding 0xA0A0.
    * The obfuscated number is then tweaked by adding 15*(i/17)
    * To reverse, use a similar reverse calculation
    * PSK files are basically raw game rips (obfuscated)
    * CGB files are un-obfuscated / B2W2.
    * Due to BW and B2W2 using different obfuscation adds, PSK files are incompatible between the versions.
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
        var result = new int[data.Length / 2];
        for (int i = 0; i < result.Length; i++)
            result[i] = Color15Bit.GetRGB555_16(ReadUInt16LittleEndian(data[(i * 2)..]));
        return result;
    }

    private static void WriteColorPalette(Span<byte> data, ReadOnlySpan<int> colors)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            var value = Color15Bit.GetRGB555(colors[i]);
            WriteUInt16LittleEndian(data[(i * 2)..], value);
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
        if (Width * Height * bpp != data.Length)
            throw new ArgumentException("Invalid image data size.");

        var colors = GetColorData(data);
        var palette = colors.Distinct().ToArray();
        if (palette.Length > ColorCount)
            throw new ArgumentException($"Too many unique colors. Expected <= 16, got {palette.Length}");

        var tiles = GetTiles(colors, palette);
        GetTileList(tiles, out List<Tile> tilelist, out TileMap tm);
        if (tilelist.Count >= CountTilePool)
            throw new ArgumentException($"Too many unique tiles. Expected < 256, received {tilelist.Count}.");

        // Finished!
        return new CGearBackground(palette, tilelist.ToArray(), tm);
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
            colors[i] = Color15Bit.GetRGB555_32(pixel);
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
        tilelist = new List<Tile> { tiles[0] };
        tm = new TileMap(LengthTileMap);

        // start at 1 as the 0th tile is always non-duplicate
        for (int i = 1; i < tm.TileChoices.Length; i++)
            FindPossibleRotatedTile(tiles[i], tilelist, tm, i);
    }

    private static void FindPossibleRotatedTile(Tile t, IList<Tile> tilelist, TileMap tm, int tileIndex)
    {
        // Test all tiles currently in the list
        for (byte i = 0; i < tilelist.Count; i++)
        {
            var rotVal = t.GetRotationValue(tilelist[i].ColorChoices);
            if (rotVal == Tile.ROTATION_BAD)
                continue;
            tm.TileChoices[tileIndex] = i;
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
        for (int i = 0; i < Map.TileChoices.Length; i++)
        {
            int x = (i * 8) % Width;
            int y = 8 * ((i * 8) / Width);
            var choice = Map.TileChoices[i] % (Tiles.Length + 1);
            var tile = Tiles[choice];
            var tileData = tile.Rotate(Map.Rotations[i]);
            for (int iy = 0; iy < 8; iy++)
            {
                const int size = 4 * 8;
                int src = iy * size;
                int dest = (((y + iy) * Width) + x) * 4;
                tileData.Slice(src, size).CopyTo(data.Slice(dest, size));
            }
        }
    }
}

public sealed class Tile
{
    internal const int SIZE_TILE = 0x20;
    private const int TileWidth = 8;
    private const int TileHeight = 8;
    internal readonly byte[] ColorChoices = new byte[TileWidth * TileHeight];

    // Keep track of known rotations for this tile.
    private byte[] PixelData = Array.Empty<byte>();
    private byte[]? PixelDataX;
    private byte[]? PixelDataY;

    internal Tile() { }

    internal Tile(ReadOnlySpan<byte> data) : this()
    {
        if (data.Length != SIZE_TILE)
            throw new ArgumentException(null, nameof(data));

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
            data[i] = (byte)((span[0] & 0xF) | ((span[1] & 0xF) << 4));
        }
    }

    public ReadOnlySpan<byte> Rotate(int rotFlip)
    {
        if (rotFlip == 0)
            return PixelData;
        if ((rotFlip & 4) > 0)
            return PixelDataX ??= FlipX(PixelData, TileWidth);
        if ((rotFlip & 8) > 0)
            return PixelDataY ??= FlipY(PixelData, TileHeight);
        return PixelData;
    }

    private static byte[] FlipX(ReadOnlySpan<byte> data, int width, int bpp = 4)
    {
        byte[] result = new byte[data.Length];
        Result(data, result, width, bpp);
        return result;
    }

    private static byte[] FlipY(ReadOnlySpan<byte> data, int height, int bpp = 4)
    {
        byte[] result = new byte[data.Length];
        FlipY(data, result, height, bpp);
        return result;
    }

    private static void Result(ReadOnlySpan<byte> data, Span<byte> result, int width, int bpp)
    {
        int pixels = data.Length / bpp;
        for (int i = 0; i < pixels; i++)
        {
            int x = i % width;
            int y = i / width;

            x = width - x - 1; // flip x
            int dest = ((y * width) + x) * bpp;

            var o = i * bpp;
            result[dest + 0] = data[o + 0];
            result[dest + 1] = data[o + 1];
            result[dest + 2] = data[o + 2];
            result[dest + 3] = data[o + 3];
        }
    }

    private static void FlipY(ReadOnlySpan<byte> data, Span<byte> result, int height, int bpp)
    {
        int pixels = data.Length / bpp;
        int width = pixels / height;
        for (int i = 0; i < pixels; i++)
        {
            int x = i % width;
            int y = i / width;

            y = height - y - 1; // flip x
            int dest = ((y * width) + x) * bpp;

            var o = i * bpp;
            result[dest + 0] = data[o + 0];
            result[dest + 1] = data[o + 1];
            result[dest + 2] = data[o + 2];
            result[dest + 3] = data[o + 3];
        }
    }

    internal const byte ROTATION_BAD = byte.MaxValue;

    internal byte GetRotationValue(ReadOnlySpan<byte> tileColors)
    {
        // Check all rotation types
        if (tileColors.SequenceEqual(ColorChoices))
            return 0;

        if (IsMirrorX(tileColors))
            return 4;
        if (IsMirrorY(tileColors))
            return 8;
        if (IsMirrorXY(tileColors))
            return 12;

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

public sealed class TileMap
{
    public readonly byte[] TileChoices;
    public readonly byte[] Rotations;

    public TileMap(int length)
    {
        TileChoices = new byte[length / 2];
        Rotations = new byte[length / 2];
    }

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
        ushort value;
        var trunc = (val & 0x3FF);
        if (trunc is < 0xA0 or > 0x280)
            value = (ushort)((val & 0x5C00) | 0xFF);
        else
            value = (ushort)(((val % 0x20) + (17 * ((trunc - 0xA0) / 0x20))) | (val & 0x5C00));

        byte tile = (byte)value;
        byte rot = (byte)(value >> 8);
        if (tile == CGearBackground.CountTilePool) // out of range?
            tile = 0;
        return (tile, rot);
    }

    public static ushort GetPSKValue(byte tile, byte rot)
    {
        if (tile == CGearBackground.CountTilePool) // out of range?
            tile = 0;

        var result = tile + (15 * (tile / 17)) + 0xA0A0 + rot;
        return (ushort)result;
    }
}

public static class Color15Bit
{
    public static int GetRGB555_32(int val) => unchecked((int)0xFF_000000) | val; // Force opaque

    public static int GetRGB555_16(ushort val)
    {
        int R = (val >> 0) & 0x1F;
        int G = (val >> 5) & 0x1F;
        int B = (val >> 10) & 0x1F;

        R = Convert5To8[R];
        G = Convert5To8[G];
        B = Convert5To8[B];

        return (0xFF << 24) | (R << 16) | (G << 8) | B;
    }

    public static ushort GetRGB555(int v)
    {
        var R = (byte)(v >> 16);
        var G = (byte)(v >> 8);
        var B = (byte)(v >> 0);

        int val = 0;
        val |= Convert8to5(R) << 0;
        val |= Convert8to5(G) << 5;
        val |= Convert8to5(B) << 10;
        return (ushort)val;
    }

    private static byte Convert8to5(int colorval)
    {
        byte i = 0;
        while (colorval > Convert5To8[i])
            i++;
        return i;
    }

    private static ReadOnlySpan<byte> Convert5To8 => new byte[] // 0x20 entries
    {
        0x00,0x08,0x10,0x18,0x20,0x29,0x31,0x39,
        0x41,0x4A,0x52,0x5A,0x62,0x6A,0x73,0x7B,
        0x83,0x8B,0x94,0x9C,0xA4,0xAC,0xB4,0xBD,
        0xC5,0xCD,0xD5,0xDE,0xE6,0xEE,0xF6,0xFF,
    };
}
