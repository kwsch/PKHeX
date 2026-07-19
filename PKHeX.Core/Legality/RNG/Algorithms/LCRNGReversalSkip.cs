using System;

namespace PKHeX.Core;

/// <summary>
/// Seed reversal logic for the <see cref="LCRNG"/> algorithm, with a gap in between the two observed rand() results.
/// </summary>
/// <remarks>
/// Uses lattice-reduction bounds from https://github.com/StarfBerry/PokeRNG/blob/main/Recovery/LCG_Recovery.py.
/// </remarks>
public static class LCRNGReversalSkip
{
    // https://github.com/StarfBerry/PokeRNG/blob/main/Recovery/LCG_Recovery.py
    private const uint RMult2 = 0xDC6C95D9; // reverse multiplier constant
    private const uint RLag0 = 0x6C31; // 27697
    private const uint RLag1PID = 0x5D20; // -59251 mod 27697
    private const uint RLag1IVs = 0x2E90; // -43474 mod 27697
    private const uint RLowerPID = 0x4B8D621D; // ((-0x20A49DE2F046 + 0xffff_ffff) >> 16) + (27697 << 16)
    private const uint RLowerIVs = 0x4B8CE21D; // ((-0x20A49DE2F046 + 0x7fff_ffff) >> 16) + (27697 << 16)
    private const uint RUpper = 0x4B8D08D7; //  (-0x20A3F728F046 >> 16) + (27697 << 16)

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
        uint third = (spe | (spa << 5) | (spd << 10)) << 16;
        return GetSeedsIVs(result, first, third);
    }

    /// <summary>
    /// Finds all the origin seeds for two 16 bit rand() calls, ignoring a rand() in between.
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="first">First rand() call, 16 bits, already shifted left 16 bits.</param>
    /// <param name="third">Third rand() call, 16 bits, already shifted left 16 bits.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeeds(Span<uint> result, uint first, uint third)
    {
        ulong tmp = (ulong)(((first - (third * RMult2)) >> 16) & 0xFFFF) * RLag0;
        var lo = (uint)((tmp + RLowerPID) >> 16);
        var up = (uint)((tmp + RUpper) >> 16);
        if (lo != up) // true in around 35% of cases
            return 0;

        int ctr = 0;
        // at most 3 iterations (around 2.37 in average)
        uint low = (lo * RLag1PID) % RLag0;
        do
        {
            var seed = LCRNG.Prev2(third | low);
            if ((seed & 0xffff0000) == first)
                result[ctr++] = LCRNG.Prev(seed);
        } while ((low += RLag0) < 0x1_0000);
        return ctr;
    }

    /// <summary>
    /// Finds all the origin seeds for two 15 bit rand() calls, ignoring a rand() in between.
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="first">First rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <param name="third">Third rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeedsIVs(Span<uint> result, uint first, uint third)
    {
        ulong tmp = (ulong)(((first - (third * RMult2)) >> 16) & 0xFFFF) * RLag0;
        var lo = (uint)((tmp + RLowerIVs) >> 15);
        var up = (uint)((tmp + RUpper) >> 15);

        int ctr = 0;
        AddSeeds(result, (lo * RLag1IVs) % RLag0, first, third, ref ctr);
        if (lo != up) // true in around 30% of cases
            AddSeeds(result, (up * RLag1IVs) % RLag0, first, third, ref ctr);
        return ctr;
    }

    private static void AddSeeds(Span<uint> result, uint low, uint first, uint third, ref int ctr)
    {
        // at most 3 iterations
        do
        {
            var seed = LCRNG.Prev2(third | low);
            if ((seed & 0x7fff0000) != first)
                continue;
            seed = LCRNG.Prev(seed);
            result[ctr++] = seed;
            result[ctr++] = seed ^ 0x80000000;
        } while ((low += RLag0) < 0x1_0000);
    }
}
