using System;

namespace PKHeX.Core;

public static class EggHatchLocation8
{
    public static bool IsValidMet8SWSH(int location)
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
