using System;
using System.ComponentModel.DataAnnotations;

namespace PKHeX.Core;

/// <summary>
/// Logic for Channel Jirachi RNG seeds prior to PID/IV generation.
/// </summary>
/// <remarks>An arbitrary PID/IV seed is valid about 18% of the time.</remarks>
public static class ChannelJirachi
{
    /// <summary>
    /// Checks if the seed that immediately generates the PID/IV is valid.
    /// </summary>
    /// <param name="seed">Seed that generates the PID/IV</param>
    /// <returns>True if valid.</returns>
    public static bool IsPossible(uint seed) => GetPossible(seed).Pattern != ChannelJirachiRandomResult.None;

    /// <summary>
    /// Checks if any of the 3 Jirachi float patterns are possible.
    /// </summary>
    /// <param name="seed">Seed that generates the PID/IV</param>
    /// <returns>First possible float pattern and origin seed (pre-menu).</returns>
    public static (ChannelJirachiRandomResult Pattern, uint Seed) GetPossible(uint seed)
    {
        // Jirachi floating into the screen has 3 different patterns to follow
        // Check each of them
        for (uint i = 0; i < 3; i++)
        {
            var origin = seed;
            if (!IsValid(ref origin, i))
                continue;
            return ((ChannelJirachiRandomResult)(i + 1), origin);
        }
        return default;
    }

    /// <summary>
    /// Check if the seed is valid for the Jirachi float pattern type AND menu pattern.
    /// </summary>
    /// <param name="origin">Seed that generated the PID/IV, to be reversed into the true origin seed.</param>
    /// <param name="possibleBranch"><see cref="ChannelJirachiRandomResult"/></param>
    /// <returns>True if valid.</returns>
    public static bool IsValid(ref uint origin, uint possibleBranch)
    {
        if (!IsValidAccept(ref origin, possibleBranch))
            return false;
        if (!IsValidMenu(ref origin))
            return false;
        origin = XDRNG.Prev(origin); // reverse to the seed before the menu pattern
        return true;
    }

    /// <summary>
    /// Check if the seed is valid for the Accept random chances.
    /// </summary>
    /// <param name="seed">Seed that generated the PID/IV, to be reversed to the post-menu seed.</param>
    /// <param name="acceptPivot">Float pattern (0-2, not 1-3).</param>
    public static bool IsValidAccept(ref uint seed, uint acceptPivot)
    {
        // The game checks for two random results (25% and 33%).
        // If either passes, we advance once, otherwise we advance twice.
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(acceptPivot, 3u, nameof(acceptPivot));

        // 71.9% chance of passing at least one of the following branches.
        // 28.1% chance of failing all branches.

        if (acceptPivot == 0) // 8 advances total
        {
            // 2: 25% fails, 33% fails. 2 advances. ~50.25% chance of success.
            seed = XDRNG.Prev(seed); // 2
            if (XDRNG.Prev16(ref seed) <= 0x547A) // 33% should fail
                return false;
            if (XDRNG.Prev16(ref seed) <= 0x4000) // 25% should fail
                return false;
        }
        else if (acceptPivot == 1) // 6 advances total
        {
            // 0: 25% passes, 33% unchecked. 1 advance. 25% chance of success.
            if (XDRNG.Prev16(ref seed) > 0x4000) // 25% should pass
                return false;
        }
        else // 7 advances total
        {
            // 25% fails, 33% passes. 1 advance. 24.75% chance of success.
            if (XDRNG.Prev16(ref seed) > 0x547A) // 33% should pass
                return false;
            if (XDRNG.Prev16(ref seed) <= 0x4000) // 25% should fail
                return false;
        }
        seed = XDRNG.Prev5(seed);
        return true;
    }

    /// <summary>
    /// Check if the seed is valid for the Jirachi menu pattern.
    /// </summary>
    /// <param name="seed">Already reversed to the seed immediately after the menu pattern, to be reversed to the origin seed.</param>
    /// <returns>True if valid.</returns>
    public static bool IsValidMenu(ref uint seed)
    {
        // 25% chance of being an impossible seed (0 as the final result).
        // 2/3 chance of being an impossible seed (end value reoccurs before the other two values are seen).
        // 25% chance of a seed being valid.

        // From the starting seed, the game determines the order of [1,2,3] by looping until all 3 are generated via (seed >> 30).
        // Track which values we've seen. If we've not yet seen the result, add it to the bitmask.
        var end = seed >> 30;
        // Menu can't end on a 0; we expect 1-3.
        if (end == 0)
            return false;

        // Start with the final result.
        var seen = 1u << (int)end;

        // Iterate backwards until all 3 results are observed.
        // If we encounter the ending result again, then the seed isn't legal -- a duplicate end value would be impossible to stop on.
        while (true)
        {
            seed = XDRNG.Prev(seed);
            var pattern = seed >> 30; // 0-3
            if (pattern == end)
                return false; // Duplicate. Input seed cannot be landed on.

            // Ignore skipping 0; add it to the bitmask anyway (reduce branching).
            seen |= 1u << (int)pattern;
            if (seen >= BitPattern123) // only true if 1-3 bits set. 0th bit irrelevant.
                return true; // Seen all! seed is legal.
        }
    }

    private const byte BitPattern123 = 0b1110; // ignore bit 0; we need to see bits 1-3

    /// <summary>
    /// Runs the Menu pattern calculation forward to see which pattern order is generated.
    /// </summary>
    /// <param name="seed">Initial seed (not yet advanced once).</param>
    /// <returns>Order and final seed.</returns>
    public static (byte First, byte Second, byte Third, uint EndSeed) GetMenuPattern(uint seed)
    {
        Span<byte> pattern = stackalloc byte[3];
        var result = GetMenuPattern(seed, pattern);
        return (pattern[0], pattern[1], pattern[2], result);
    }

    /// <summary>
    /// Runs the Menu pattern calculation forward to see which pattern order is generated.
    /// </summary>
    /// <param name="seed">Initial seed (not yet advanced once).</param>
    /// <param name="pattern">Result to store the order.</param>
    /// <returns>Final seed.</returns>
    public static uint GetMenuPattern(uint seed, [Length(3,3)] Span<byte> pattern)
    {
        int i = 0;
        while (i != pattern.Length)
        {
            seed = XDRNG.Next(seed);
            var p = (byte)(seed >> 30);
            if (pattern.Contains(p))
                continue; // 0 will always return true, yay zero initialized arrays!
            pattern[i++] = p;
        }
        return seed;
    }

    /// <summary>
    /// Skips from origin to the PID/IV generation seed.
    /// </summary>
    public static uint SkipToPIDIV(uint seed)
    {
        seed = SkipMenuPattern(seed);
        seed = SkipAccept(seed);
        return seed;
    }

    /// <summary>
    /// Skips the menu pattern calculation to the seed that is used by the Accept step.
    /// </summary>
    public static uint SkipMenuPattern(uint seed)
    {
        uint seen = 0;
        while (true)
        {
            seed = XDRNG.Next(seed);
            var p = (byte)(seed >> 30);
            seen |= 1u << p;
            if (seen >= BitPattern123) // only true if 1-3 bits set. 0th bit irrelevant.
                return seed;
        }
    }

    /// <summary>
    /// Skips the 25% and 33% calculation done when accepting the Jirachi to the end seed that then can be used to generate the PID/IV.
    /// </summary>
    public static uint SkipAccept(uint seed)
    {
        seed = XDRNG.Next4(seed);

        // If we pass either a 25% or 33% check, we advance once.
        // Each check is independent of the other, using a different random value.
        var rand = XDRNG.Next16(ref seed);
        if (rand <= 0x4000) // 25%
            return XDRNG.Next(seed);

        rand = XDRNG.Next16(ref seed);
        if (rand <= 0x547A) // 33%
            return XDRNG.Next(seed);

        // Both failed, advance twice.
        return XDRNG.Next2(seed);
    }
}

public enum ChannelJirachiRandomResult : byte
{
    None = 0,

    // Ordered by most likely to occur.
    BothFail = 1, // 50.25%
    FirstPass = 2, // 25%
    SecondPass = 3, // 24.75%
}
