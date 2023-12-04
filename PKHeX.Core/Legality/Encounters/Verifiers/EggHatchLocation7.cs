using System;

namespace PKHeX.Core;

public static class EggHatchLocation7
{
    private const byte MaskSM = 1 << 0; // 1
    private const byte MaskUSUM = 1 << 1; // 2

    public static bool IsValidMet7SM(int location)
    {
        if (HasLocationFlag(LocationPermitted7, MaskSM, location))
            return true;
        return location == Locations.Pelago7; // 30016
    }

    public static bool IsValidMet7USUM(int location)
    {
        if (HasLocationFlag(LocationPermitted7, MaskUSUM, location))
            return true;
        return location == Locations.Pelago7; // 30016
    }

    private static bool HasLocationFlag(ReadOnlySpan<byte> arr, byte mask, int location)
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
