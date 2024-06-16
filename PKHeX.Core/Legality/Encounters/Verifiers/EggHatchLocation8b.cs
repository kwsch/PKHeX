using System;

namespace PKHeX.Core;

/// <summary>
/// Hatch Location validity for <see cref="GameVersion.BDSP"/>.
/// </summary>
public static class EggHatchLocation8b
{
    private const byte MaskBD = 1 << 0; // 1
    private const byte MaskSP = 1 << 1; // 2

    /// <summary>
    /// Returns true if the hatch location is valid for Brilliant Diamond.
    /// </summary>
    public static bool IsValidMet8BD(ushort location) => HasLocationFlag(LocationPermitted8b, MaskBD, location);

    /// <summary>
    /// Returns true if the hatch location is valid for Shining Pearl.
    /// </summary>
    public static bool IsValidMet8SP(ushort location) => HasLocationFlag(LocationPermitted8b, MaskSP, location);

    private static bool HasLocationFlag(ReadOnlySpan<byte> arr, byte mask, ushort location)
    {
        if ((uint)location >= arr.Length)
            return false;
        var flags = arr[location];
        return (flags & mask) != 0;
    }

    // BD Exclusive
    // 216, // Spear Pillar
    // 218, // Hall of Origin
    // 498, // Ramanas Park (Johto Room)
    // 503, // Ramanas Park (Rainbow Room)
    // 650, // Ramanas Park (Johto Room)
    // 655, // Ramanas Park (Rainbow Room)

    // SP Exclusive
    // 217, // Spear Pillar
    // 497, // Ramanas Park (Kanto Room)
    // 504, // Ramanas Park (Squall Room)
    // 618, // Hall of Origin
    // 649, // Ramanas Park (Kanto Room)
    // 656, // Ramanas Park (Squall Room)

    // Unobtainable
    // 094, 103, 107,                // Hearthome City
    // 154, 155, 158,                // Sunyshore City
    // 181, 182, 183,                // Pokémon League
    // 329,                          // Lake Acuity
    // 337, 338,                     // Battle Park
    // 339, 340, 341, 342, 343, 344, // Battle Tower
    // 345, 353, 421,                // Mystery Zone
    // 474,                          // Resort Area
    // 483, 484,                     // Mystery Zone
    // 491, 492, 493,                // Mystery Zone
    // 495,                          // Ramanas Park
    // 620, 621, 622, 623,           // Grand Underground (Secret Base)
    // 625,                          // Sea (sailing animation)
    // 627, 628, 629, 630, 631, 632, // Grand Underground (Secret Base)
    // 633, 634, 635, 636, 637, 638, // Grand Underground (Secret Base)
    // 639, 640, 641, 642, 643, 644, // Grand Underground (Secret Base)
    // 645, 646, 647,                // Grand Underground (Secret Base)

    private static ReadOnlySpan<byte> LocationPermitted8b =>
    [
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 3, 3, 0, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 2, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3,
        3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 3, 0, 0, 0, 3, 0, 3, 2, 1, 3, 3, 3, 3, 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 3, 0, 0, 0, 0, 3, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 3, 2, 1, 3, 3, 3, 3, 1, 2, 3,
    ];
}
