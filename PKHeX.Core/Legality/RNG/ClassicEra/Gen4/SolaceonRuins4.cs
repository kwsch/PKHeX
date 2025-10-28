using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for <see cref="EntityContext.Gen4"/> Solaceon Ruins Unown forms.
/// </summary>
public static class SolaceonRuins4
{
    /// <summary>
    /// Met location for the Solaceon Ruins.
    /// </summary>
    public const ushort Location = 53;

    /// <summary>
    /// Checks if the requested <see cref="form"/> is valid for the given seed.
    /// </summary>
    /// <param name="seed">Seed that originated the PID/IV.</param>
    /// <param name="form">Form to validate</param>
    /// <returns>True if the form is valid</returns>
    public static bool IsFormValid(uint seed, byte form)
    {
        if (IsSingleFormRoomUnown(form))
            return true; // FRIEND: Specific rooms with only one form.

        // ABCD|E(Item)|F(Form) determination
        var f = LCRNG.Next6(seed) >> 16;
        var expect = GetUnownForm(f, form);
        return expect == form;
    }

    /// <inheritdoc cref="IsFormValid(uint,byte)"/>
    public static bool IsFormValid(PKM pk, byte form)
    {
        if (IsSingleFormRoomUnown(form))
            return true; // FRIEND: Specific rooms with only one form.

        // Only forms beyond here are the !? and Dead End rooms.

        if (!MethodFinder.GetLCRNGMethod1Match(pk, out var seed))
            return true; // invalid anyway, don't care.

        // ABCD|E(Item)|F(Form) determination
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

    /// <summary>
    /// Determines whether the specified Unown form is a single-form room Unown.
    /// </summary>
    /// <param name="actual">The form ID of the Unown.</param>
    /// <returns><see langword="true"/> if the specified form is a single-form room Unown; otherwise, <see langword="false"/>.</returns>
    public static bool IsSingleFormRoomUnown(byte actual) => (FormRandomUnown & (1 << actual)) == 0;
}
