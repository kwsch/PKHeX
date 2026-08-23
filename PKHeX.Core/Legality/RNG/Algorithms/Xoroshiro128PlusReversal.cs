using System.Collections;
using System.Collections.Generic;

using static PKHeX.Core.Xoroshiro128Plus;

namespace PKHeX.Core;

/// <summary>
/// Reversal logic for consecutive <see cref="Xoroshiro128Plus.Next()"/> calls.
/// </summary>
public static class Xoroshiro128PlusReversal
{
    /*
     * https://github.com/StarfBerry/PokeRNG/blob/1e9b9ddf2494837c7d6704c7b8a3831f644bdea9/Recovery/Xoroshiro_Recovery.py
     * Instead of assuming bits 32~37 of the seeds, we assume bits 58~63.
     * These new bits make it possible to check if the system of equations is solvable.
     * On average, this avoids about half of the calculations.
     *
     * In the case of two outputs with a skip in between:
     * - the old algorithm assumes 1 carry bit and bits 32~36 and 48~53 of the seeds,
     * - the new algorithm assumes 2 carry bits and bits 43~50 of the seeds.
     *
     * The number of brute-forced bits has been reduced from 12 to 10.
     * This approach is between 7 and 36 times faster than the old algorithm when benchmarked.
     *
     * C# yield syntax allocates an object on the heap rather than a struct, so we must manually implement the iterator.
     * This manual iterator repeats the intro work before the eager check, but it is still faster than the old algorithm.
     * If a fatter struct is used, we can avoid the repeated work by storing the intermediate results in the struct.
     * Not really worth the extra effort at this time.
     */

    /// <summary>
    /// Reverses two consecutive <see cref="Xoroshiro128Plus.Next()"/> calls (low 32-bits) to get the original 64-bit seed.
    /// </summary>
    /// <param name="seed">Resulting seed</param>
    /// <param name="out1">First output, low 32-bits</param>
    /// <param name="out2">Second output, low 32-bits</param>
    /// <param name="assume">Brute-forced guess of bits 58-63</param>
    /// <param name="carry">Carry bit</param>
    /// <returns>True if a satisfactory seed was found, false otherwise.</returns>
    public static bool Explore(out ulong seed, uint out1, uint out2, byte assume, byte carry)
    {
        ulong baseSeed = (uint)unchecked(out1 - XOROSHIRO_CONST);
        ulong x0 = baseSeed ^ XOROSHIRO_CONST;

        baseSeed |= ((out2 - (x0 >> 27) ^ x0) & 0x1F) << 40;
        ulong x1 = (baseSeed << 6) ^ (x0 >> 18) ^ (x0 >> 2);
        ulong sub = (out2 >> 18) - (x1 ^ assume);
        ulong r = (sub - carry) & 0x3FFF;

        seed = ((ulong)assume << 58) | baseSeed;
        if ((assume & 1u) != (r >> 13))
            return false; // the assumed and recovered bits 58 don't match

        // the xor operation to recover the bits can be applied after the check because ((XOROSHIRO_CONST >> 45) >> 13) & 1 == 0
        // 
        // 45-57
        seed |= ((r ^ 0x1515) << 45); // 0x1515 = (XOROSHIRO_CONST >> 45) & 0x3fff
        // 32-39
        seed |= (((((out2 - ((seed >> 40) ^ x0)) >> 5) ^ 0x75) & 0xFF) << 32); // 0x75 = (XOROSHIRO_CONST >> 32) & 0xff

        var check = new Xoroshiro128Plus(seed);
        return (uint)check.Next() == out1 && (uint)check.Next() == out2;
    }

    /// <summary>
    /// Reverses two consecutive <see cref="Xoroshiro128Plus.Next()"/> calls (low 32-bits) with an unknown result in between to get the original 64-bit seed.
    /// </summary>
    /// <param name="seed">Resulting seed</param>
    /// <param name="out1">First output, low 32-bits</param>
    /// <param name="out2">Second output, low 32-bits</param>
    /// <param name="assume2">Brute-forced guess of bits 43-50</param>
    /// <param name="carry1">First carry bit</param>
    /// <param name="carry2">Second carry bit</param>
    /// <returns>True if a satisfactory seed was found, false otherwise.</returns>
    public static bool ExploreDouble(out ulong seed, uint out1, uint out2, byte assume2, byte carry1, byte carry2)
    {
        ulong baseSeed = (uint)unchecked(out1 - XOROSHIRO_CONST);
        ulong bitsCheck = baseSeed & 7;
        ulong x0 = baseSeed ^ XOROSHIRO_CONST;

        ulong x1_ = (baseSeed >> 19) ^ (x0 >> 6) ^ 0x56;
        ulong x2_ = (x0 >> 16) ^ 0x65;
        ulong x3_ = x1_ ^ (x0 >> 27);
        ulong x4_ = x2_ ^ (x0 >> 27);
        ulong x5 = (baseSeed >> 16) ^ x0 ^ 0x2B1;
        ulong x6 = (baseSeed >> 3) ^ (x0 >> 11) ^ 0xE0A;
        ulong x7 = XOROSHIRO_CONST ^ (x0 >> 24);
        ulong t0 = out2 >> 16;
        ulong t1 = out2 >> 27;

        ulong assume = baseSeed | ((ulong)assume2 << 43);
        x1_ ^= assume2;
        x2_ ^= assume2;
        x3_ ^= assume2;
        x4_ ^= assume2;

        ulong sub0 = t0 - carry1;

        // 32-36
        ulong tmp = ((((sub0 - x3_) ^ x4_) & 0x1F) << 32) | assume;
        x0 = tmp ^ XOROSHIRO_CONST;

        // 37-39
        tmp |= (((sub0 - (x1_ ^ (x0 >> 27))) ^ x2_ ^ (x0 >> 27)) & 0xFF) << 32;
        x0 = tmp ^ XOROSHIRO_CONST;

        ulong r = (((out2 - (x5 ^ (x0 >> 27) ^ (x0 >> 24))) ^ x6 ^ (x0 >> 27)) & 0x1FFF);
        if ((r >> 10) != bitsCheck) // recovered bits 37-39 cannot yield a solution
        {
            seed = 0;
            return false;
        }

        // 54-63
        tmp |= (r & 0x3FF) << 54;
        x0 = tmp ^ XOROSHIRO_CONST;
        ulong x8 = (tmp >> 30) ^ (x0 >> 17) ^ (x0 >> 54);
        ulong x9 = (tmp >> 43) ^ (tmp >> 51) ^ (x0 >> 27) ^ (x0 >> 54) ^ 3;
        ulong x10 = x8 ^ (x0 >> 38);
        ulong x11 = x9 ^ (x0 >> 38);
        ulong x12 = x7 ^ (x0 >> 35) ^ (tmp >> 48);
        ulong x13 = (tmp >> 19) ^ (x0 >> 6);
        ulong x14 = x13 ^ (x0 >> 27);

        ulong sub1 = t1 - carry2;
        seed = ((((sub1 - x10) ^ x11) & 3) << 51) | tmp;
        x0 = seed ^ XOROSHIRO_CONST;
        seed |= ((((sub0 - (x14 ^ (x0 >> 43))) >> 8) ^ x12 ^ (x0 >> 51)) & 3) << 40;
        x0 = seed ^ XOROSHIRO_CONST;
        seed |= (((sub1 - (x8 ^ (x0 >> 38))) ^ x9 ^ (x0 >> 38)) & 7) << 51;
        x0 = seed ^ XOROSHIRO_CONST;

        r = ((((sub0 - (x13 ^ (x0 >> 27) ^ (x0 >> 43))) >> 8) ^ x7 ^ (x0 >> 35) ^ (seed >> 48) ^ (x0 >> 51)) & 0x7F);
        if ((assume2 & 0xFu) != (r >> 3))
            return false;

        seed |= r << 40;
        var check = new Xoroshiro128Plus(seed);
        if ((uint)check.Next() != out1)
            return false;
        _ = check.Next();
        return (uint)check.Next() == out2;
    }
}

/// <summary>
/// Provides enumeration of all possible seeds that can generate the given output of two consecutive 32-bit results.
/// </summary>
/// <param name="First">First 32-bit result</param>
/// <param name="Second">Second 32-bit result</param>
/// <remarks>
/// State machine implementation as we normally don't need to know all results.
/// Only one result seed will be useful in verifying all remaining rand() calls.
/// </remarks>
public record struct XoroMachineConsecutive(uint First, uint Second) : IEnumerator<ulong>
{
    public readonly ulong Current => seed;

    private ulong seed;
    private byte assume1; // assume 58-63
    private byte carry;

    public bool MoveNext()
    {
        do
        {
            while (carry < 2)
            {
                if (Xoroshiro128PlusReversal.Explore(out seed, First, Second, assume1, carry++))
                    return true;
            }
            carry = 0;
        } while (++assume1 < 0x40);
        return false;
    }

    // IEnumerator Implementation -- used for foreach syntax sugar inlining
    public void Reset() => assume1 = carry = 0;
    readonly object IEnumerator.Current => Current;
    public readonly void Dispose() { }
    public readonly IEnumerator<ulong> GetEnumerator() => this;
}

/// <summary>
/// Provides enumeration of all possible seeds that can generate the given output of two consecutive 32-bit results with an unknown result in-between the first and third result.
/// </summary>
/// <param name="First">First 32-bit result</param>
/// <param name="Third">Third 32-bit result</param>
/// <remarks>
/// State machine implementation as we normally don't need to know all results.
/// Only one result seed will be useful in verifying all remaining rand() calls.
/// </remarks>
public record struct XoroMachineSkip(uint First, uint Third) : IEnumerator<ulong>
{
    public readonly ulong Current => seed;

    private ulong seed;
    private byte assume1; // assume 43-50
    private byte carry1;
    private byte carry2;

    public bool MoveNext()
    {
        do
        {
            do
            {
                while (carry1 < 2)
                {
                    if (Xoroshiro128PlusReversal.ExploreDouble(out seed, First, Third, assume1, carry1++, carry2))
                        return true;
                }
                carry1 = 0;
            } while ((carry2 ^= 1) != 0);
        } while (++assume1 != 0);
        return false;
    }

    // IEnumerator Implementation -- used for foreach syntax sugar inlining
    public void Reset() => assume1 = carry1 = carry2 = 0;
    readonly object IEnumerator.Current => Current;
    public readonly void Dispose() { }
    public readonly IEnumerator<ulong> GetEnumerator() => this;
}
