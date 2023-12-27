using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public static class EggHatchLocation5
{
    private const byte MaskWhite = 1 << 0; // 1
    private const byte MaskBlack = 1 << 1; // 2
    private const byte MaskWhite2 = 1 << 2; // 4
    private const byte MaskBlack2 = 1 << 3; // 8

    public static bool IsValidMet5(int location, GameVersion game)
    {
        var shift = (uint)(game - W);
        if (shift >= 4)
            return false;

        var mask = (byte)(1 << (int)shift);
        return HasLocationFlag(LocationPermitted5, mask, location);
    }

    public static bool IsValidMet5W(int location) => HasLocationFlag(LocationPermitted5, MaskWhite, location);
    public static bool IsValidMet5B(int location) => HasLocationFlag(LocationPermitted5, MaskBlack, location);
    public static bool IsValidMet5W2(int location) => HasLocationFlag(LocationPermitted5, MaskWhite2, location);
    public static bool IsValidMet5B2(int location) => HasLocationFlag(LocationPermitted5, MaskBlack2, location);

    private static bool HasLocationFlag(ReadOnlySpan<byte> arr, byte mask, int location)
    {
        if ((uint)location >= arr.Length)
            return false;
        var flags = arr[location];
        return (flags & mask) != 0;
    }

    // Two game-specific locations we need to double-check for.
    // White / White2 cannot access Black Gate (112)
    // Black / Black2 cannot access White Gate (113)
    private static ReadOnlySpan<byte> LocationPermitted5 =>
    [
        00, 00, 00, 00, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 03, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
        03, 15, 15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15, 15, 03,
        15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 03, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 10, 05, 15, 15, 15, 12, 12, 12,
        12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
        12, 12, 12, 12, 12, 12, 12, 12, 00, 12,
        12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
        12, 12, 12, 12,
    ];
}
