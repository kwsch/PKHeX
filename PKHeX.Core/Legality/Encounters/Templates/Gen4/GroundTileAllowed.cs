using System;

namespace PKHeX.Core;

/// <summary>
/// Permitted <see cref="GroundTileType"/> values an encounter can be acquired with.
/// </summary>
/// <remarks>
/// Some locations have multiple tile types that can proc an encounter, requiring multiple values possible.
/// </remarks>
[Flags]
public enum GroundTileAllowed
{
    Undefined = 0,
    None     = 1 << 00, // No animation for the tile
    Sand     = 1 << 01, // Obtainable only via HG/SS
    Grass    = 1 << 02,
    Puddle   = 1 << 03, // No encounters from this tile type
    Rock     = 1 << 04,
    Cave     = 1 << 05,
    Snow     = 1 << 06, // No encounters from this tile type
    Water    = 1 << 07,
    Ice      = 1 << 08, // No encounters from this tile type
    Building = 1 << 09,
    Marsh    = 1 << 10,
    Bridge   = 1 << 11, // No encounters from this tile type
    Max_DP   = 1 << 12, // Unspecific, catch-all for D/P undefined tiles.

    // added tile types in Pt
    // no encounters from these tile types
    Elite4_1           = 1 << 12, // Elite Four Room #1
    Elite4_2           = 1 << 13, // Elite Four Room #2
    Elite4_3           = 1 << 14, // Elite Four Room #3
    Elite4_4           = 1 << 15, // Elite Four Room #4
    Elite4_M           = 1 << 16, // Elite Four Champion Room
    DistortionSideways = 1 << 17, // Distortion World (Not Giratina)
    BattleTower        = 1 << 18,
    BattleFactory      = 1 << 19,
    BattleArcade       = 1 << 20,
    BattleCastle       = 1 << 21,
    BattleHall         = 1 << 22,

    Distortion         = 1 << 23,
    Max_Pt             = 1 << 24, // Unspecific, catch-all for Pt undefined tiles.
}

/// <summary>
/// Extension methods for <see cref="GroundTileAllowed"/>.
/// </summary>
public static class GroundTileAllowedExtensions
{
    /// <summary>
    /// Checks if the <see cref="GroundTileAllowed"/> value is a valid tile type.
    /// </summary>
    /// <param name="permit">Tile bit-permission value</param>
    /// <param name="value">Tile type to check.</param>
    public static bool Contains(this GroundTileAllowed permit, GroundTileType value) => (permit & (GroundTileAllowed)(1 << (int)value)) != 0;

    /// <summary>
    /// Grabs the lowest set bit from the tile value.
    /// </summary>
    /// <param name="permit">Tile bit-permission value</param>
    /// <returns>Bit index</returns>
    public static GroundTileType GetIndex(this GroundTileAllowed permit) => (GroundTileType)System.Numerics.BitOperations.Log2((uint)(permit & ~(permit - 1)));
}
