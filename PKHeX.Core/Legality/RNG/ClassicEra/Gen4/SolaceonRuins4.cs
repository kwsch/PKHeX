using System;

namespace PKHeX.Core;

public static class SolaceonRuins4
{
    /// <summary>
    /// Checks if the requested <see cref="form"/> is valid for the given seed.
    /// </summary>
    public static bool IsUnownFormValid(PKM pk, byte form)
    {
        if (IsSingleFormRoomUnown(form))
            return true; // FRIEND: Specific rooms with only one form.

        // Only forms beyond here are the !? and Dead End rooms.

        if (!MethodFinder.GetLCRNGMethod1Match(pk, out var seed))
            return true; // invalid anyway, don't care.

        var f = LCRNG.Next6(seed) >> 16;
        var expect = GetUnownForm(f, form);
        return expect == form;
    }

    /// <summary>
    /// Unown forms available in every Dead End room.
    /// </summary>
    private static ReadOnlySpan<byte> Unown0 =>
    [
        0, 1, 2,
        6, 7,
        9, 10, 11, 12,
        14, 15, 16,
        18, 19, 20, 21, 22, 23, 24, 25,
    ];

    private static byte GetUnownForm(uint rand, byte prefer)
    {
        if (prefer >= 26) // Area from Maniac Tunnel
            return (byte)(26 + (rand & 1));
        return Unown0[(int)(rand % Unown0.Length)];
    }

    // Forms that appear with others in the same room, and are not 100% guaranteed.
    // 100% Guaranteed Forms are F,R,I,E,N,D
    private const uint FormRandomUnown = 0b1111111111011101111011000111u;
    public static bool IsSingleFormRoomUnown(byte actual) => (FormRandomUnown & (1 << actual)) == 0;
}
