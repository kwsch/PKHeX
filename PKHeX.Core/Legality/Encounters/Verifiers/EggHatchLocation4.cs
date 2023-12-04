using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public static class EggHatchLocation4
{
    private const byte MaskDP = 1 << 0; // 1
    private const byte MaskPt = 1 << 1; // 2
    private const byte MaskHGSS = 1 << 2; // 4
    private const byte MaskAll4 = MaskDP | MaskPt | MaskHGSS; // 7

    public static bool IsValidMet4(int location, GameVersion game) => game switch
    {
        D or P => IsValidMet4DP(location),
        Pt => IsValidMet4Pt(location),
        HG or SS => IsValidMet4HGSS(location),
        _ => false,
    };

    public static bool IsValidMet4DP(int location) => HasLocationFlag(LocationPermitted4, MaskDP, location);
    public static bool IsValidMet4Pt(int location) => HasLocationFlag(LocationPermitted4, MaskPt, location);
    public static bool IsValidMet4HGSS(int location) => HasLocationFlag(LocationPermitted4, MaskHGSS, location);
    public static bool IsValidMet4Any(int location) => HasLocationFlag(LocationPermitted4, MaskAll4, location);

    private static bool HasLocationFlag(ReadOnlySpan<byte> arr, byte mask, int location)
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
