using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 C-Gear Background Image, tile positions formatted for <see cref="GameVersion.BW"/>.
/// </summary>
public sealed class CGearBackgroundBW(Memory<byte> Raw) : CGearBackground(Raw)
{
    /// <summary>
    /// Originally supported by PokeStock, a PokeSkin file.
    /// </summary>
    public const string Extension = "psk";

    // Tiles in RAM are stored in a 15x17 grid starting at position 160 (0xA0).
    private const int TileBWStart = 16 * 10; // 0xA0
    private const int TileBWWidth = 17;
    private const int TileBWHeight = 15;
    // Could this have been an off-by-one when trying to iterate over 16x16? Who knows. /% 32 works to reverse it.

    /// <summary> Gets the 0-based index of the tile from an offset layout index. </summary>
    public static ushort GetVisualIndex(int tile)
    {
        if (tile < TileBWStart)
            return 0; // force special tiles to be represented by the first tile.
        var result = (ushort)((TileBWWidth * ((tile - TileBWStart) / 32)) + (tile % 32));
        if (result >= 300)
            return 0;
        return result;
    }

    /// <summary> Gets the offset layout index of the tile from a 0-based index. </summary>
    public static ushort GetLayoutIndex(int tile) => (ushort)((TileBWHeight * (tile / TileBWWidth)) + tile + TileBWStart);
}
