namespace PKHeX.Core;

/// <summary>
/// Logic for calculating and detecting Generation 3 Event PIDs.
/// </summary>
/// <remarks>
/// Commonly referred to as B-A-C-D, the actual call order of the PID may be different.
/// We still call them B-A-C-D just to keep them lumped together.
/// </remarks>
public static class CommonEvent3
{
    /// <summary>
    /// Standard calculation for the BA-CD PID generation.
    /// </summary>
    /// <param name="a16">First rand16 call, placed in the upper bits of the PID.</param>
    /// <param name="b16">Second rand16 call, placed in the lower bits of the PID.</param>
    /// <returns>32bit PID</returns>
    public static uint GetRegular(uint a16, uint b16) => (a16 << 16) | b16;

    /// <summary>
    /// Alternate PID generation method for events, notably used in the PC-NY Gotta Catch 'Em All events.
    /// </summary>
    /// <param name="a16">First rand16 call, placed in the upper bits of the PID.</param>
    /// <param name="b16">Second rand16 call, placed in the lower bits of the PID.</param>
    /// <param name="idXor">Trainer ID xor value.</param>
    /// <remarks><see cref="a16"/> must be greater than 7.</remarks>
    /// <returns>32bit PID</returns>
    public static uint GetAntishiny(uint a16, uint b16, uint idXor) => ((a16 ^ (idXor ^ b16)) << 16) | b16;

    /// <summary>
    /// Alternate PID generation method for events, notably used by the Berry Glitch Fix (Zigzagoon).
    /// </summary>
    /// <param name="x16">First rand16 call, placed in the upper bits of the PID.</param>
    /// <param name="b16">Second rand16 call, placed in the lower bits of the PID.</param>
    /// <param name="idXor">Trainer ID xor value.</param>
    /// <remarks><see cref="x16"/> instead of a16, this is 1 frame earlier. The a16 call is unused.</remarks>
    /// <returns>32bit PID</returns>
    public static uint GetForceShiny(uint x16, uint b16, uint idXor) => (x16 << 16) | (((idXor ^ x16) & 0xFFF8) | (b16 & 0b111));

    /// <summary>
    /// Regular PID generation method for events, that changes the PID to be non-shiny if it were shiny.
    /// </summary>
    /// <param name="pid">Calculated 32-bit PID from the regular generation method.</param>
    /// <returns>Revised 32-bit PID</returns>
    private static uint GetRegularAntishiny(uint pid) => (pid + 8) & 0xFFFFFFF8;

    /// <inheritdoc cref="GetRegularAntishiny(uint)"/>
    /// <param name="a16">First rand16 call, placed in the upper bits of the PID.</param>
    /// <param name="b16">Second rand16 call, placed in the lower bits of the PID.</param>
    /// <param name="idXor">Trainer ID xor value.</param>
    public static uint GetRegularAntishiny(uint a16, uint b16, uint idXor)
    {
        var pid = GetRegular(a16, b16);
        if (ShinyUtil.GetIsShiny3(idXor, pid))
            pid = GetRegularAntishiny(pid);
        return pid;
    }

    /// <inheritdoc cref="GetRegular(uint, uint)"/>
    /// <param name="seed">Seed to roll forward to generate the PID.</param>
    public static uint GetRegular(ref uint seed)
    {
        var a16 = LCRNG.Next16(ref seed);
        var b16 = LCRNG.Next16(ref seed);
        return GetRegular(a16, b16);
    }

    /// <inheritdoc cref="GetAntishiny(uint, uint, uint)"/>
    /// <param name="seed">Seed to roll forward to generate the PID.</param>
    /// <param name="idXor">Trainer ID xor value.</param>
    public static uint GetAntishiny(ref uint seed, uint idXor)
    {
        uint a16;
        do a16 = LCRNG.Next16(ref seed); // If it were, it would result in a shiny. We need any bits that won't yield a shiny.
        while ((a16 & ~0b111) == 0);

        var b16 = LCRNG.Next16(ref seed);
        return GetAntishiny(a16, b16, idXor);
    }

    /// <inheritdoc cref="GetForceShiny(uint, uint, uint)"/>
    /// <param name="seed">Seed to roll forward to generate the PID.</param>
    /// <param name="idXor">Trainer ID xor value.</param>
    public static uint GetForceShiny(ref uint seed, uint idXor)
    {
        var x16 = LCRNG.Next16(ref seed);
        _ = LCRNG.Next16(ref seed); // A (ends up unused)
        var b16 = LCRNG.Next16(ref seed);
        return GetForceShiny(x16, b16, idXor);
    }

    /// <inheritdoc cref="GetRegularAntishiny(uint, uint, uint)"/>
    /// <param name="seed">Seed to roll forward to generate the PID.</param>
    /// <param name="idXor">Trainer ID xor value.</param>
    public static uint GetRegularAntishiny(ref uint seed, uint idXor)
    {
        var a16 = LCRNG.Next16(ref seed);
        var b16 = LCRNG.Next16(ref seed);
        return GetRegularAntishiny(a16, b16, idXor);
    }

    /// <summary>
    /// Checks if the PID is a regular PID generated from the given a16 and b16 values.
    /// </summary>
    /// <inheritdoc cref="GetRegular(uint,uint)"/>
    public static bool IsRegular(uint pid, uint a16, uint b16)
    {
        uint result = GetRegular(a16, b16);
        return result == pid;
    }

    /// <summary>
    /// Checks if the PID is an anti-shiny PID generated from the given a16 and b16 values.
    /// </summary>
    /// <inheritdoc cref="IsForceAntishiny(uint,uint,uint,uint)"/>
    public static bool IsForceAntishiny(uint pid, uint a16, uint b16, uint idXor)
    {
        // First RNG call should be sought to be above 7 to ensure a shiny is not generated.
        if ((a16 & ~0b111) == 0)
            return false; // If it were, it would result in a shiny. We need any bits that won't yield a shiny.

        // 0-Origin
        // A-PIDH component (>= 8)
        // B-PIDL
        uint result = GetAntishiny(a16, b16, idXor);
        return result == pid;
    }

    /// <summary>
    /// Checks if the PID is an anti-shiny PID generated from the given a16 and b16 values.
    /// </summary>
    /// <inheritdoc cref="GetForceShiny(uint,uint,uint)"/>
    public static bool IsForceShiny(uint pid, uint x16, uint b16, uint idXor)
    {
        // The mutation algorithm doesn't end up using the 'B' portion of the PID.
        if (x16 != pid >> 16)
            return false; // eager check to avoid the more expensive calculation.

        // 0-Origin
        // X-PIDH
        // A-PIDL (ends up unused)
        // B-FORCEBITS
        uint result = GetForceShiny(x16, b16, idXor);
        return result == pid;
    }

    /// <summary>
    /// Checks if the PID is a regular anti-shiny PID generated from the given a16 and b16 values.
    /// </summary>
    /// <inheritdoc cref="GetRegularAntishiny(uint,uint,uint)"/>
    public static bool IsRegularAntishiny(uint pid, uint a16, uint b16, uint idXor)
    {
        uint result = GetRegularAntishiny(a16, b16, idXor);
        return result == pid;
    }

    /// <inheritdoc cref="IsRegularAntishiny(uint,uint,uint,uint)"/>
    /// <param name="expectPID">Regular BA PID that is expected from the correlation, no corrections.</param>
    /// <param name="actualPID">The actual PID of the Pokémon.</param>
    public static bool IsRegularAntishiny(uint actualPID, uint expectPID)
    {
        var result = GetRegularAntishiny(expectPID);
        return result == actualPID;
    }

    /// <summary>Checks for eggs hatched with the regular Anti-shiny correlation, but originating from an unknown ID xor. </summary>
    /// <inheritdoc cref="IsRegularAntishiny(uint,uint,uint,uint)"/>
    /// <remarks>Only call this method when the Pokémon is SHINY.</remarks>
    /// <param name="expectPID">Regular BA PID that is expected from the correlation, no corrections.</param>
    /// <param name="actualPID">The actual PID of the Pokémon.</param>
    public static bool IsRegularAntishinyDifferentOT(uint actualPID, uint expectPID)
    {
        // The mutation algorithm basically adds until it is non-shiny.
        if ((actualPID & 0b111) != 0)
            return false; // eager check to avoid the more expensive calculation.
        var delta = actualPID - expectPID;
        return delta is (not 0) and < 8;
    }

    /// <summary>Checks for eggs hatched with the Force Anti-shiny correlation, but originating from an unknown ID xor. </summary>
    /// <inheritdoc cref="IsForceAntishiny"/>
    /// <remarks>Only call this method when the Pokémon is SHINY.</remarks>
    public static bool IsForceAntishinyDifferentOT(uint pid, uint a16, uint b16, uint idXor)
    {
        // First RNG call should be sought to be above 7 to ensure a shiny is not generated.
        if ((a16 & ~0b111) == 0)
            return false; // If it were, it would result in a shiny. We need any bits that won't yield a shiny.
        if ((pid & 0xFFFF) != b16)
            return false;

        // The ID xor is used in the top half. Since we need a different OT, these must be different.
        // Be sure to ignore the 3 bits of TSV (bit-shift 19 bits instead of 16).
        uint result = GetAntishiny(a16, b16, idXor);
        return result >> 19 != pid >> 19;
    }

    /// <summary>Checks for eggs hatched with the Force Shiny correlation, but originating from an unknown ID xor. </summary>
    /// <inheritdoc cref="IsForceShiny"/>
    /// <remarks>Only call this method when the Pokémon is NOT SHINY.</remarks>
    public static bool IsForceShinyDifferentOT(uint pid, uint x16, uint b16, uint idXor)
    {
        if (x16 != pid >> 16)
            return false; // eager check to avoid the more expensive calculation.

        // Due to the nature of the Force Shiny algorithm, bits 0-2 are from b16, and x16 is the high bits.
        if ((pid & 0b111) != (b16 & 0b111))
            return false;
        // Bits 3-15 are from the ID xor, which is x16 ^ idXor. But we don't know the original.
        // However, it must not be the same as the hatching OT xor. Because this method is only called when not shiny.
        var low = (idXor ^ x16) & 0xFFF8;
        var actual = (pid & 0xFFF8);
        return low != actual;
    }

    public static uint GetGenderBit0(uint rand16) => (rand16 / 3) & 1;
    public static uint GetGenderBit3(uint rand16) => (rand16 >> 3) & 1;
    public static uint GetGenderBit7(uint rand16) => ((rand16 >> 7) & 1) ^ 1; // Returns Female (1) when calc is 0
    public static uint GetGenderBit15(uint rand16) => (rand16 >> 15) & 1;
    public static uint GetGenderBit0(ref uint seed) => GetGenderBit0(LCRNG.Next16(ref seed));
    public static uint GetGenderBit3(ref uint seed) => GetGenderBit3(LCRNG.Next16(ref seed));
    public static uint GetGenderBit7(ref uint seed) => GetGenderBit7(LCRNG.Next16(ref seed));
    public static uint GetGenderBit15(ref uint seed) => GetGenderBit15(LCRNG.Next2(seed) >> 15);

    /// <summary>
    /// Gets a random restricted (16-bit) seed that respects the gender correlation.
    /// </summary>
    /// <param name="seed">Seed that generates the PID/IV then gender.</param>
    /// <param name="gender5">Requested gender</param>
    public static uint GetRandomRestrictedGenderBit0(uint seed, byte gender5)
    {
        while (true)
        {
            var u16 = seed & 0xFFFF;
            var rand5 = LCRNG.Next5(u16) >> 16;
            if (GetGenderBit0(rand5) == gender5)
                return u16;
            seed = LCRNG.Next(seed);
        }
    }
}
