using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Hatch Location validity for <see cref="GameVersion.Gen5"/>.
/// </summary>
public static class EggHatchLocation5
{
    private const byte MaskWhite = 1 << 0; // 1
    private const byte MaskBlack = 1 << 1; // 2
    private const byte MaskWhite2 = 1 << 2; // 4
    private const byte MaskBlack2 = 1 << 3; // 8

    /// <summary>
    /// Returns true if the location is valid for the specified Generation 5 game.
    /// </summary>
    public static bool IsValidMet5(ushort location, GameVersion game)
    {
        var shift = (uint)(game - W);
        if (shift >= 4)
            return false;

        var mask = (byte)(1 << (int)shift);
        return HasLocationFlag(LocationPermitted5, mask, location);
    }

    /// <summary>
    /// Returns true if the hatch location is valid for White.
    /// </summary>
    public static bool IsValidMet5W(ushort location) => HasLocationFlag(LocationPermitted5, MaskWhite, location);

    /// <summary>
    /// Returns true if the hatch location is valid for Black.
    /// </summary>
    public static bool IsValidMet5B(ushort location) => HasLocationFlag(LocationPermitted5, MaskBlack, location);

    /// <summary>
    /// Returns true if the hatch location is valid for White 2.
    /// </summary>
    public static bool IsValidMet5W2(ushort location) => HasLocationFlag(LocationPermitted5, MaskWhite2, location);

    /// <summary>
    /// Returns true if the hatch location is valid for Black 2.
    /// </summary>
    public static bool IsValidMet5B2(ushort location) => HasLocationFlag(LocationPermitted5, MaskBlack2, location);

    private static bool HasLocationFlag(ReadOnlySpan<byte> arr, byte mask, ushort location)
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
