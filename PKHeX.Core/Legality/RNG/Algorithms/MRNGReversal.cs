using System;

namespace PKHeX.Core;

/// <summary>
/// Seed reversal logic for the <see cref="MRNG"/> algorithm.
/// </summary>
public static class MRNGReversal
{
    // https://github.com/StarfBerry/PokeRNG/blob/main/Recovery/LCG_Recovery.py
    private const uint RMult2 = 0xDC6C95D9; // reverse multiplier constant
    private const uint RLag0 = 0x6C31; // 27697
    private const uint RLag1IVs = 0x2E90; // -43474 mod 27697
    private const uint RLower = 0x670A0357; // ((-0x5277CA86A92 + 0x7fff_ffff) >> 16) + (27697 << 16)
    private const uint RUpper = 0x670A2A11; // (-0x526D5EE6A92 >> 16) + (27697 << 16)

    /// <summary>
    /// Finds all Ranch RNG states that can generate the IVs.
    /// </summary>
    public static int GetSeedsIVs(Span<uint> result, uint hp, uint atk, uint def, uint spa, uint spd, uint spe)
    {
        uint first = (spe | (spa << 5) | (spd << 10)) << 16;
        uint third = (hp | (atk << 5) | (def << 10)) << 16;
        return GetSeedsIVs(result, first, third);
    }

    /// <summary>
    /// Finds all Ranch RNG states from the first and third packed IV outputs.
    /// </summary>
    public static int GetSeedsIVs(Span<uint> result, uint first, uint third)
    {
        ulong tmp = (ulong)(((first - (third * RMult2)) >> 16) & 0xFFFF) * RLag0;
        var lo = (uint)((tmp + RLower) >> 15);
        var up = (uint)((tmp + RUpper) >> 15);

        int ctr = 0;
        AddSeeds(result, (lo * RLag1IVs) % RLag0, first, third, ref ctr);
        if (lo != up) // true in around 30% of cases
            AddSeeds(result, (up * RLag1IVs) % RLag0, first, third, ref ctr);
        return ctr;
    }

    private static void AddSeeds(Span<uint> result, uint low, uint first, uint third, ref int ctr)
    {
        // each loop performs at most 3 iterations
        do
        {
            var seed = MRNG.Prev2(third | low);
            if ((seed & 0x7FFF0000) != first)
                continue;
            seed = MRNG.Prev(seed);
            result[ctr++] = seed;
            result[ctr++] = seed ^ 0x80000000;
        } while ((low += RLag0) < 0x1_0000);
    }

    /// <summary>
    /// Checks if the given combined 30-bit HABCDS IVs can generate from any seed.
    /// </summary>
    /// <returns>True if there are seeds that can generate the IVs, otherwise false.</returns>
    public static bool HasSeeds(uint hp, uint atk, uint def, uint spa, uint spd, uint spe)
    {
        uint third = (hp | (atk << 5) | (def << 10)) << 16;
        uint first = (spe | (spa << 5) | (spd << 10)) << 16;
        return HasSeeds(first, third);
    }

    /// <summary>
    /// Checks if the given combined 30-bit HABCDS IVs can generate from any seed.
    /// </summary>
    /// <param name="combined">The combined 30-bit HABCDS IVs.</param>
    /// <returns>True if there are seeds that can generate the IVs, otherwise false.</returns>
    public static bool HasSeeds(uint combined)
    {
        var third = (combined >> 00) & 0x7FFF; // HAB
        var first = (combined >> 15) & 0x7FFF; // CDS
        return HasSeeds(first, third);
    }

    /// <summary>
    /// Checks if the given first and third packed IV outputs can generate from any seed.
    /// </summary>
    /// <param name="first">The first packed IV output (CDS).</param>
    /// <param name="third">The third packed IV output (HAB).</param>
    /// <returns>True if there are seeds that can generate the IVs, otherwise false.</returns>
    public static bool HasSeeds(uint first, uint third)
    {
        Span<uint> seeds = stackalloc uint[LCRNG.MaxCountSeedsIV];
        var count = GetSeedsIVs(seeds, first, third);
        return count != 0;
    }
}
