using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Hatch Location validity for <see cref="GameVersion.Gen4"/>.
/// </summary>
public static class EggHatchLocation4
{
    private const byte MaskDP = 1 << 0; // 1
    private const byte MaskPt = 1 << 1; // 2
    private const byte MaskHGSS = 1 << 2; // 4
    private const byte MaskAll4 = MaskDP | MaskPt | MaskHGSS; // 7

    /// <summary>
    /// Returns true if the hatch location is valid for the specified Generation 4 game.
    /// </summary>
    public static bool IsValidMet4(ushort location, GameVersion game) => game switch
    {
        D or P => IsValidMet4DP(location),
        Pt => IsValidMet4Pt(location),
        HG or SS => IsValidMet4HGSS(location),
        _ => false,
    };

    /// <summary>
    /// Returns true if the hatch location is valid for Diamond and Pearl.
    /// </summary>
    public static bool IsValidMet4DP(ushort location) => HasLocationFlag(LocationPermitted4, MaskDP, location);

    /// <summary>
    /// Returns true if the hatch location is valid for Platinum.
    /// </summary>
    public static bool IsValidMet4Pt(ushort location) => HasLocationFlag(LocationPermitted4, MaskPt, location);

    /// <summary>
    /// Returns true if the hatch location is valid for HeartGold and SoulSilver.
    /// </summary>
    public static bool IsValidMet4HGSS(ushort location) => HasLocationFlag(LocationPermitted4, MaskHGSS, location);

    /// <summary>
    /// Returns true if the hatch location is valid for any Generation 4 game.
    /// </summary>
    public static bool IsValidMet4Any(ushort location) => HasLocationFlag(LocationPermitted4, MaskAll4, location);

    private static bool HasLocationFlag(ReadOnlySpan<byte> arr, byte mask, ushort location)
    {
        if ((uint)location >= arr.Length)
            return false;
        var flags = arr[location];
        return (flags & mask) != 0;
    }

    private static ReadOnlySpan<byte> LocationPermitted4 =>
    [
        0, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 2, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 2,
        7, 3, 3, 3, 3, 2, 0, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 6, 6, 6, 6, 6, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 0, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 0, 4,
    ];
}
