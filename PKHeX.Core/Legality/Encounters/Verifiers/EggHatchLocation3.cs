using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Hatch Location validity for <see cref="GameVersion.Gen3"/>.
/// </summary>
public static class EggHatchLocation3
{
    private const byte MaskRS   = 1 << 0; // 1
    private const byte MaskE    = 1 << 1; // 2
    private const byte MaskFRLG = 1 << 2; // 4
    private const byte MaskAll = MaskRS | MaskE | MaskFRLG; // 7

    /// <summary>
    /// Returns true if the hatch location is valid for the specified Generation 3 game.
    /// </summary>
    public static bool IsValidMet3(ushort location, GameVersion game) => game switch
    {
        R  or  S => IsValidMet3RS(location),
        E        => IsValidMet3E(location),
        FR or LG => IsValidMet3FRLG(location),
        _ => false,
    };

    /// <summary>
    /// Returns true if the hatch location is valid for Ruby and Sapphire.
    /// </summary>
    public static bool IsValidMet3RS(ushort location) => HasLocationFlag(LocationPermitted3, MaskRS, location);

    /// <summary>
    /// Returns true if the hatch location is valid for Emerald.
    /// </summary>
    public static bool IsValidMet3E(ushort location) => HasLocationFlag(LocationPermitted3, MaskE, location);

    /// <summary>
    /// Returns true if the hatch location is valid for FireRed and LeafGreen.
    /// </summary>
    public static bool IsValidMet3FRLG(ushort location) => HasLocationFlag(LocationPermitted3, MaskFRLG, location);

    /// <summary>
    /// Returns true if the hatch location is valid for any Generation 3 game.
    /// </summary>
    public static bool IsValidMet3Any(ushort location) => HasLocationFlag(LocationPermitted3, MaskAll, location);

    private static bool HasLocationFlag(ReadOnlySpan<byte> arr, byte mask, ushort location)
    {
        if ((uint)location >= arr.Length)
            return false;
        var flags = arr[location];
        return (flags & mask) != 0;
    }

    // 064 is an unused location for Meteor Falls
    // 084 is Inside a truck, no possible Pokémon can be obtained at the start of the game
    // 071 is Mirage island, cannot be obtained as the player is technically still on Route 130's map.
    // 075 is an unused location for Fiery Path
    // 077 is an unused location for Jagged Pass

    // 155 - 158 Sevii Isle 6-9 Unused
    // 171 - 173 Sevii Isle 22-24 Unused
    private static ReadOnlySpan<byte> LocationPermitted3 =>
    [
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 0, 3, 3, 3, 3, 3,
        3, 0, 3, 3, 3, 0, 3, 0, 3, 3,
        3, 3, 3, 3, 0, 3, 3, 7, 4, 4, // 87 = all
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 0, 0, 0, 0, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 0, 0, 0, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 6, 2, 2, 2, // 196 = fr/lg & e
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2,
    ];
}
