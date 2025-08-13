using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 &amp; 4 RNG logic.
/// </summary>
public static class ClassicEraRNG
{
    /// <summary>
    /// Generate a chain shiny PID for the provided trainer ID.
    /// </summary>
    /// <param name="seed">Seed to use for the RNG.</param>
    /// <param name="id32">Trainer ID to use for the PID generation.</param>
    /// <returns>Shiny PID.</returns>
    /// <remarks>Consumes 15 RNG calls</remarks>
    public static uint GetChainShinyPID(ref uint seed, in uint id32)
    {
        // 1 3-bit for lower
        // 1 3-bit for upper
        // 13 rand bits
        uint lower = LCRNG.Next16(ref seed) & 7;
        uint upper = LCRNG.Next16(ref seed) & 7;
        for (int i = 3; i < 16; i++)
            lower |= (LCRNG.Next16(ref seed) & 1) << i;

        var tid16 = (ushort)(id32 & 0xFFFFu);
        var sid16 = (ushort)(id32 >> 16);
        upper = ((lower ^ tid16 ^ sid16) & 0xFFF8) | (upper & 0x7);
        return (upper << 16) | lower;
    }

    /// <summary>
    /// Rolls the RNG forward twice to get the usual Method 1 call-ordered PID.
    /// </summary>
    /// <param name="seed">Seed right before the first PID call.</param>
    /// <returns>32-bit value containing the PID (high | low).</returns>
    public static uint GetSequentialPID(ref uint seed)
    {
        var rand1 = LCRNG.Next16(ref seed);
        var rand2 = LCRNG.Next16(ref seed);
        return (rand2 << 16) | rand1;
    }

    /// <summary>
    /// Rolls the RNG forward twice to get the usual Method 1 call-ordered PID.
    /// </summary>
    /// <param name="seed">Seed right before the first PID call.</param>
    /// <returns>32-bit value containing the PID (high | low).</returns>
    public static uint GetSequentialPID(uint seed)
    {
        var rand1 = LCRNG.Next16(ref seed);
        var rand2 = LCRNG.Next16(ref seed);
        return (rand2 << 16) | rand1;
    }

    /// <summary>
    /// Rolls the RNG forward twice to get the reverse Method 1 call-ordered PID.
    /// </summary>
    /// <param name="seed">Seed right before the first PID call.</param>
    /// <remarks>Generation 3 Unown</remarks>
    /// <returns>32-bit value containing the PID (high | low).</returns>
    public static uint GetReversePID(ref uint seed)
    {
        var rand1 = LCRNG.Next16(ref seed);
        var rand2 = LCRNG.Next16(ref seed);
        return (rand2 << 16) | rand1;
    }

    /// <summary>
    /// Rolls the RNG forward twice to get the reverse Method 1 call-ordered PID.
    /// </summary>
    /// <param name="seed">Seed right before the first PID call.</param>
    /// <remarks>Generation 3 Unown</remarks>
    /// <returns>32-bit value containing the PID (high | low).</returns>
    public static uint GetReversePID(uint seed)
    {
        var rand1 = LCRNG.Next16(ref seed);
        var rand2 = LCRNG.Next16(ref seed);
        return (rand1 << 16) | rand2;
    }

    /// <summary>
    /// Generates IVs for a given seed.
    /// </summary>
    /// <param name="seed">Seed to use for the RNG.</param>
    /// <returns>32-bit value containing the IVs (HABSCD, low->high).</returns>
    public static uint GetSequentialIVs(ref uint seed)
    {
        var rand1 = LCRNG.Next15(ref seed);
        var rand2 = LCRNG.Next15(ref seed);
        return (rand2 << 15) | rand1;
    }

    /// <summary>
    /// Creates an initial seed from the given components.
    /// </summary>
    /// <param name="year">Year component (2000-2099).</param>
    /// <param name="month">Month component (1-12).</param>
    /// <param name="day">Day component (1-31).</param>
    /// <param name="hour">Hour component (0-23).</param>
    /// <param name="minute">Minute component (0-59).</param>
    /// <param name="second">Seconds component (0-59).</param>
    /// <param name="delay">Delay timer component.</param>
    /// <remarks>
    /// No sanity checking if the Month/Day/Year are valid.
    /// </remarks>
    public static uint GetInitialSeed(uint year, uint month, uint day, uint hour, uint minute, uint second, uint delay)
    {
        byte ab = (byte)((month * day) + minute + second);
        byte cd = (byte)hour;

        return (uint)((ab << 24) | (cd << 16)) + delay + year - 2000u;
    }

    /// <summary>
    /// Finds the initial seed for a given date and time.
    /// </summary>
    /// <param name="year">Year component (2000-2099).</param>
    /// <param name="month">Month component (1-12).</param>
    /// <param name="day">Day component (1-31).</param>
    /// <param name="seed">Origin seed to look backwards for the initial seed.</param>
    /// <remarks>
    /// No sanity checking if the Month/Day/Year are valid.
    /// </remarks>
    public static uint SeekInitialSeed(uint year, uint month, uint day, uint seed)
    {
        while (!IsInitialSeed(year, month, day, seed))
            seed = LCRNG.Prev(seed);
        var decompose = DecomposeSeed(seed, year, month, day);
        // Check one step previous, just in case that delay is better.
        var prevSeed = LCRNG.Prev(seed);
        while (!IsInitialSeed(year, month, day, prevSeed))
            prevSeed = LCRNG.Prev(prevSeed);

        var distance = LCRNG.GetDistance(prevSeed, seed);
        if (distance > 5000) // arbitrary limit, most won't need this many advances to RNG.
            return seed; // don't go too far back

        var prevDecompose = DecomposeSeed(prevSeed, year, month, day);
        // Check if the previous seed has a better delay
        if (prevDecompose.Delay < decompose.Delay)
            return prevSeed;
        return seed;
    }

    /// <summary>
    /// Checks if a seed is an initial seed for the given date and time.
    /// </summary>
    /// <param name="year">Year component (2000-2099).</param>
    /// <param name="month">Month component (1-12).</param>
    /// <param name="day">Day component (1-31).</param>
    /// <param name="seed">Initial seed to check.</param>
    /// <returns><see langword="true"/> if the seed is an initial seed for the given date and time; otherwise, <see langword="false"/>.</returns>
    public static bool IsInitialSeed(uint year, uint month, uint day, uint seed)
    {
        // Check component: hour
        var hour = (byte)(seed >> 16 & 0xFF);
        if (hour > 23)
            return false;

        // Check component: everything else but delay/year using modular arithmetic to handle overflow
        const uint maxBonusMinSec = 59 + 59; // min + sec
        var top = (byte)(seed >> 24);
        var topMin = (byte)(month * day);
        // Calculate the difference modulo 256. If it exceeds maxBonusMinSec, it's out of range.
        if ((byte)(top - topMin) > maxBonusMinSec)
            return false;

        // Check component: delay/year
        // Should be a plausible delay; even though delay can overflow, it would take at least half an hour of waiting to launch the game to do so.
        const uint baseDelay = 400; // hg/ss
        var minDelay = baseDelay + (year - 2000u);
        const uint maxDelay = 6000;

        var delayComponent = (ushort)(seed - minDelay);
        // Check if the delay is within the plausible range
        if (delayComponent > maxDelay)
            return false;

        return true;
    }

    /// <summary>
    /// Decomposes a seed into its datetime initial seed components.
    /// </summary>
    /// <param name="seed">Initial seed to decompose.</param>
    /// <param name="year">Year of the initial seed.</param>
    /// <param name="month">Month of the initial seed.</param>
    /// <param name="day">Day of the initial seed.</param>
    /// <exception cref="ArgumentOutOfRangeException"> if any component is out of range and is not an Initial Seed.</exception>
    public static InitialSeedComponents4 DecomposeSeed(uint seed, uint year, uint month, uint day)
    {
        // Check component: hour
        var hour = (byte)(seed >> 16 & 0xFF);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(hour, 23, nameof(hour));

        // Check component: everything else but delay/year using modular arithmetic to handle overflow
        const uint maxBonusMinSec = 59 + 59; // min + sec
        var top = (byte)(seed >> 24);
        var topMin = (byte)(month * day);
        // Calculate the difference modulo 256. If it exceeds maxBonusMinSec, it's out of range.
        var delta = (byte)(top - topMin);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(delta, maxBonusMinSec, nameof(top));

        var yearComponent = (byte)(year - 2000u);
        var delay = (ushort)((ushort)seed - yearComponent);
        var (min, sec) = GetMinuteSecond(delta);

        return new InitialSeedComponents4
        {
            Year = yearComponent,
            Month = (byte)month,
            Day = (byte)day,
            Hour = hour,
            Delay = delay,

            Minute = min,
            Second = sec,
        };
    }

    private static (byte Minute, byte Second) GetMinuteSecond(byte delta)
    {
        // Minute and seconds: prefer a seconds value at least 7, at most 15 if possible.
        const byte minSec = 7;
        const byte maxSec = 15;
        byte min; byte sec;
        if (delta < 59 + maxSec)
        {
            sec = delta switch
            {
                <= minSec => delta,
                <= minSec + 59 => (byte)Math.Max(minSec, delta - 59),
                _ => maxSec,
            };
            min = (byte)(delta - sec);
        }
        else
        {
            // need a higher seconds
            min = 59;
            sec = (byte)(delta - 59);
        }

        return (min, sec);
    }

    /// <summary>
    /// Finds the initial seed for a given set of IVs in Generation 4.
    /// </summary>
    /// <param name="ivs">IVs to use for the search.</param>
    /// <param name="year">Year of the initial seed.</param>
    /// <param name="month">Month of the initial seed.</param>
    /// <param name="day">Day of the initial seed.</param>
    /// <param name="origin">Seed that originated the IVs.</param>
    /// <returns>Initial datetime seed.</returns>
    public static uint SeekInitialSeedForIVs(ReadOnlySpan<int> ivs, uint year, uint month, uint day, out uint origin)
    {
        origin = 0;
        uint bestDistance = uint.MaxValue;

        Span<uint> seeds = stackalloc uint[LCRNG.MaxCountSeedsIV];
        var count = LCRNGReversal.GetSeedsIVs(seeds, (uint)ivs[0], (uint)ivs[1], (uint)ivs[2], (uint)ivs[4], (uint)ivs[5], (uint)ivs[3]);
        if (count == 0)
            return 0; // shouldn't happen; IVs should always find seeds.

        seeds = seeds[..count];

        uint best = 0;
        foreach (var seed in seeds)
        {
            var init = SeekInitialSeed(year, month, day, seed);
            var distance = LCRNG.GetDistance(init, seed);
            if (distance > bestDistance)
                continue;
            bestDistance = distance;
            best = init;
            origin = seed;
        }
        return best;
    }
}
