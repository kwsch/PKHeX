using System;

namespace PKHeX.Core;

public sealed class PokeDexSkin5(Memory<byte> Raw)
{
    public const string Extension = "pds";

    /// <summary> Pixel width of the background image. </summary>
    public const int Width = 256; // px
    /// <summary> Pixel height of the background image. </summary>
    public const int Height = 192; // px

    public const int SIZE = LengthTilePool + LengthColorData + LengthColorDataBackground + Unused + 4; // 0x6204

    private const int TilePoolCount = 768; // 0x300
    private const int CountColorForeground = 16;
    private const int CountColorBackground = 64;

    private const int LengthTilePool = TilePoolCount * PaletteTile.SIZE; // 0x6000
    private const int LengthColorData = CountColorForeground * sizeof(ushort); // 0x20
    private const int LengthColorDataBackground = CountColorBackground * sizeof(ushort); // 0x80
    private const int Unused = 0x160;

    public Span<byte> Data => Raw.Span;
    public Span<byte> ColorChoices => Data[..LengthTilePool];
    public Span<byte> ColorForeground => Data.Slice(LengthTilePool, LengthColorData);
    public Span<byte> ColorBackground => Data.Slice(LengthTilePool + LengthColorData, LengthColorDataBackground);
    public bool IsUninitialized => !Data.ContainsAnyExcept<byte>(0xFF, 0);
}
