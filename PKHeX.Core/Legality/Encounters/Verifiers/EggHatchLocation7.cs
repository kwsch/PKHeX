using System;

namespace PKHeX.Core;

/// <summary>
/// Hatch Location validity for <see cref="GameVersion.Gen7"/>.
/// </summary>
public static class EggHatchLocation7
{
    private const byte MaskSM = 1 << 0; // 1
    private const byte MaskUSUM = 1 << 1; // 2

    /// <summary>
    /// Returns true if the hatch location is valid for Sun and Moon.
    /// </summary>
    public static bool IsValidMet7SM(ushort location)
    {
        if (HasLocationFlag(LocationPermitted7, MaskSM, location))
            return true;
        return location == Locations.Pelago7; // 30016
    }

    /// <summary>
    /// Returns true if the hatch location is valid for Ultra Sun and Ultra Moon.
    /// </summary>
    public static bool IsValidMet7USUM(ushort location)
    {
        if (HasLocationFlag(LocationPermitted7, MaskUSUM, location))
            return true;
        return location == Locations.Pelago7; // 30016
    }

    private static bool HasLocationFlag(ReadOnlySpan<byte> arr, byte mask, ushort location)
    {
        if ((uint)location >= arr.Length)
            return false;
        var flags = arr[location];
        return (flags & mask) != 0;
    }

    private static ReadOnlySpan<byte> LocationPermitted7 =>
    [
        0, 0, 0, 0, 0, 0, 3, 0, 3, 0, // 000
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 0, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        0, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 0, 0, 0, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0, // 100
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 0, 0, 0, 0, 2, 0,
        2, 0, 2, 0, 2, 0, 2, 0, 2, 0, // 200
        2, 0, 2, 0, 2, 0, 2, 0, 2, 0,
        2, 0, 2, 0, 2, 0, 2, 0, 2, 0,
        2, 0, 2,
    ];
}
