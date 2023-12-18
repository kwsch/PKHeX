using System;

namespace PKHeX.Core;

public static class EggHatchLocation9
{
    private const byte MaskScarlet = 1 << 0; // 1
    private const byte MaskViolet = 1 << 1; // 2
    public static bool IsValidMet9SL(int location) => HasLocationFlag(LocationPermitted9, MaskScarlet, location);
    public static bool IsValidMet9VL(int location) => HasLocationFlag(LocationPermitted9, MaskViolet, location);

    private static bool HasLocationFlag(ReadOnlySpan<byte> arr, byte mask, int location)
    {
        if ((uint)location >= arr.Length)
            return false;
        var flags = arr[location];
        return (flags & mask) != 0;
    }

    // 130 Naranja Academy does not exist in Violet
    // 131 Uva Academy does not exist in Scarlet
    private static ReadOnlySpan<byte> LocationPermitted9 =>
    [
        0, 0, 0, 0, 0, 0, 3, 0, 0, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 0, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 0, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 0, 3, 0, 3,
        3, 0, 3, 0, 0, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 0, 3,
        0, 3, 0, 3, 0, 3, 0, 3, 0, 3,
        0, 3, 0, 3, 0, 3, 0, 3, 3, 0,
        0, 0, 0, 0, 3, 0, 0, 0, 0, 0,
        1, 2, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        0, 0, 3, 0, 0, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, 0, 3, 0, 3, 0, 3, 0, 3, 0,
        3, // Terarium (Entry Tunnel)
    ];
}
