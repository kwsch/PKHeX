using System;
using static PKHeX.Core.GroundTileType;

namespace PKHeX.Core;

/// <summary>
/// Ground Tile Type the <see cref="PKM"/> was encountered from.
/// </summary>
/// <remarks>
/// Used in Generation 4 games, this value is set depending on what type of overworld tile the player is standing on when the <see cref="PKM"/> is obtained.
/// </remarks>
#pragma warning disable CA1069, RCS1234 // Enums values should not be duplicated
public enum GroundTileType : byte
{
    None     = 00, // No animation for the tile
    Sand     = 01, // Obtainable only via HG/SS
    Grass    = 02,
    Puddle   = 03, // No encounters from this tile type
    Rock     = 04,
    Cave     = 05,
    Snow     = 06, // No encounters from this tile type
    Water    = 07,
    Ice      = 08, // No encounters from this tile type
    Building = 09,
    Marsh    = 10,
    Bridge   = 11, // No encounters from this tile type
    Max_DP   = 12, // Unspecific, catch-all for D/P undefined tiles.

    // added tile types in Pt
    // no encounters from these tile types
    Elite4_1           = 12, // Elite Four Room #1
    Elite4_2           = 13, // Elite Four Room #2
    Elite4_3           = 14, // Elite Four Room #3
    Elite4_4           = 15, // Elite Four Room #4
    Elite4_M           = 16, // Elite Four Champion Room
    DistortionSideways = 17, // Distortion World (Not Giratina)
    BattleTower        = 18,
    BattleFactory      = 19,
    BattleArcade       = 20,
    BattleCastle       = 21,
    BattleHall         = 22,

    Distortion         = 23,
    Max_Pt             = 24, // Unspecific, catch-all for Pt undefined tiles.
}

public static class GroundTileTypeExtensions
{
    public static bool IsObtainable(this GroundTileType type) => ((0b_1_10000000_00010110_10110111 >> (int) type) & 1) == 1;

    public static ReadOnlySpan<byte> ValidTileTypes =>
    [
        (byte)None, (byte)Sand, (byte)Grass, (byte)Rock, (byte)Cave, (byte)Water, (byte)Building, (byte)Marsh, (byte)Max_DP, (byte)Distortion, (byte)Max_Pt,
    ];
}
