using System;

namespace PKHeX.Core;

/// <summary>
/// Rotation flags for a palette tile.
/// </summary>
/// <see cref="PaletteTileSelection"/>
[Flags]
public enum PaletteTileRotation
{
    /// <summary> No rotation. </summary>
    None = 0,

    /// <summary> Flip the tile horizontally. </summary>
    FlipX = 1 << 0,

    /// <summary> Flip the tile vertically. </summary>
    FlipY = 1 << 1,

    /// <summary> Flip the tile horizontally and vertically. </summary>
    FlipXY = FlipX | FlipY,

    /// <summary> Invalid rotation. </summary>
    Invalid = 1 << 2,
}
