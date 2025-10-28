using System;

namespace PKHeX.Core;

/// <summary>
/// Seed reversal logic for the <see cref="LCRNG"/> algorithm, with a gap in between the two observed rand() results.
/// </summary>
/// <remarks>
/// Abuses the pattern in Low16 bit results of rand() to reduce the search space to 2^3 instead of 2^16
/// https://github.com/StarfBerry/poke-scripts/blob/f641f3eab264e2caf1ee7c3c0e5553a9d8086921/RNG/LCG_Reversal.py#L19-L93
/// </remarks>
public static class LCRNGReversalSkip
{
    private const uint LCRNG_MOD_2 = 0x3A89; // u32(0x3a89 * LCRNG_MUL_2) < 2^16 (for seed and seed + 0x3a89, we have a good chance that the 16bit high of the next output will be the same for both)
    private const uint LCRNG_PAT_2 = 0x2E4C; // pattern in the distribution of the "low solutions" modulo 0x3a89
    private const uint LCRNG_INC_2 = 0x5831; // (((diff * 0x3a89 + 0x5831) >> 16) * 0x2e4c) % 0x3a89 line up with the first 16bit low solution modulo 0x3a89 if it exists (see diff definition in code)

    /// <summary>
    /// Finds all seeds that can generate the IVs, with a vblank skip between the two IV rand() calls.
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="hp" >Entity IV for HP</param>
    /// <param name="atk">Entity IV for Attack</param>
    /// <param name="def">Entity IV for Defense</param>
    /// <param name="spa">Entity IV for Special Attack</param>
    /// <param name="spd">Entity IV for Special Defense</param>
    /// <param name="spe">Entity IV for Speed</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeedsIVs(Span<uint> result, uint hp, uint atk, uint def, uint spa, uint spd, uint spe)
    {
        uint first = (hp | (atk << 5) | (def << 10)) << 16;
        uint second = (spe | (spa << 5) | (spd << 10)) << 16;
        return GetSeedsIVs(result, first, second);
    }

    /// <summary>
    /// Finds all the origin seeds for two 16 bit rand() calls (ignoring a rand() in between)
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="first">First rand() call, 16 bits, already shifted left 16 bits.</param>
    /// <param name="third">Third rand() call, 16 bits, already shifted left 16 bits.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeeds(Span<uint> result, uint first, uint third)
    {
        var diff = (third - LCRNG.Next2(first)) >> 16;
        var low = ((((diff * LCRNG_MOD_2) + LCRNG_INC_2) >> 16) * LCRNG_PAT_2) % LCRNG_MOD_2;

        int ctr = 0;
        // at most 5 iterations
        do
        {
            var seed = first | low;
            if ((LCRNG.Next2(seed) & 0xffff0000) == third)
                result[ctr++] = LCRNG.Prev(seed);
        } while ((low += LCRNG_MOD_2) < 0x1_0000);
        return ctr;
    }

    /// <summary>
    /// Finds all the origin seeds for two 15 bit rand() calls (ignoring a rand() in between)
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="first">First rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <param name="third">Third rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeedsIVs(Span<uint> result, uint first, uint third)
    {
        var diff = (third - LCRNG.Next2(first)) >> 16;
        var start1 = ((((diff * LCRNG_MOD_2) + LCRNG_INC_2) >> 16) * LCRNG_PAT_2) % LCRNG_MOD_2;
        var start2 = (((((diff ^ 0x8000) * LCRNG_MOD_2) + LCRNG_INC_2) >> 16) * LCRNG_PAT_2) % LCRNG_MOD_2;

        int ctr = 0;
        AddSeeds(result, start1, first, third, ref ctr);
        AddSeeds(result, start2, first, third, ref ctr);
        return ctr;
    }

    private static void AddSeeds(Span<uint> result, uint low, uint first, uint third, ref int ctr)
    {
        // at most 5 iterations
        do
        {
            var test = first | low;
            if ((LCRNG.Next2(test) & 0x7fff0000) != third)
                continue;
            var seed = LCRNG.Prev(test);
            result[ctr++] = seed;
            result[ctr++] = seed ^ 0x80000000;
        } while ((low += LCRNG_MOD_2) < 0x1_0000);
    }
}
