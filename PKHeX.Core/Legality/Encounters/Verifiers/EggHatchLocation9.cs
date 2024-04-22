using System;

namespace PKHeX.Core;

/// <summary>
/// Hatch Location validity for <see cref="GameVersion.SV"/>.
/// </summary>
public static class EggHatchLocation9
{
    private const byte MaskScarlet = 1 << 0; // 1
    private const byte MaskViolet = 1 << 1; // 2

    /// <summary>
    /// Returns true if the hatch location is valid for Scarlet.
    /// </summary>
    public static bool IsValidMet9SL(ushort location) => HasLocationFlag(LocationPermitted9, MaskScarlet, location);

    /// <summary>
    /// Returns true if the hatch location is valid for Violet.
    /// </summary>
    public static bool IsValidMet9VL(ushort location) => HasLocationFlag(LocationPermitted9, MaskViolet, location);

    private static bool HasLocationFlag(ReadOnlySpan<byte> arr, byte mask, ushort location)
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
