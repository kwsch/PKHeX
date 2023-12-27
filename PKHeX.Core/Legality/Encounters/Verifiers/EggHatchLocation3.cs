using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public static class EggHatchLocation3
{
    private const byte MaskRS   = 1 << 0; // 1
    private const byte MaskE    = 1 << 1; // 2
    private const byte MaskFRLG = 1 << 2; // 4
    private const byte MaskAll = MaskRS | MaskE | MaskFRLG; // 7

    public static bool IsValidMet3(int location, GameVersion game) => game switch
    {
        R  or  S => IsValidMet3RS(location),
        E        => IsValidMet3E(location),
        FR or LG => IsValidMet3FRLG(location),
        _ => false,
    };

    public static bool IsValidMet3RS(int location) => HasLocationFlag(LocationPermitted3, MaskRS, location);
    public static bool IsValidMet3E(int location) => HasLocationFlag(LocationPermitted3, MaskE, location);
    public static bool IsValidMet3FRLG(int location) => HasLocationFlag(LocationPermitted3, MaskFRLG, location);
    public static bool IsValidMet3Any(int location) => HasLocationFlag(LocationPermitted3, MaskAll, location);

    private static bool HasLocationFlag(ReadOnlySpan<byte> arr, byte mask, int location)
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
