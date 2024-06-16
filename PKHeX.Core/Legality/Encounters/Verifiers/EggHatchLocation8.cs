using System;

namespace PKHeX.Core;

/// <summary>
/// Hatch Location validity for <see cref="GameVersion.SWSH"/>.
/// </summary>
public static class EggHatchLocation8
{
    /// <summary>
    /// Returns true if the hatch location is valid for Sword and Shield.
    /// </summary>
    public static bool IsValidMet8SWSH(ushort location)
    {
        if (location % 2 != 0)
            return false;

        var index = location >> 1;
        var arr = LocationPermitted8;
        if ((uint)index >= arr.Length)
            return false;
        return arr[index] != 0;
    }

    // Odd indexes ignored.
    private static ReadOnlySpan<byte> LocationPermitted8 =>
    [
        0, 0, 0, 1, 1, 0, 1, 1, 1, 1,
        1, 1, 1, 0, 1, 1, 1, 1, 1, 0,
        1, 0, 1, 1, 1, 0, 1, 1, 1, 1,
        1, 0, 1, 1, 1, 1, 1, 0, 1, 1,
        1, 0, 1, 1, 1, 1, 1, 1, 1, 1,
        0, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 0, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1,
    ];
}
