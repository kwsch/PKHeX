using System.Collections;
using System.Collections.Generic;
using static System.Numerics.BitOperations;

using static PKHeX.Core.Xoroshiro128Plus;

namespace PKHeX.Core;

/// <summary>
/// Reversal logic for consecutive <see cref="Xoroshiro128Plus.Next()"/> calls.
/// </summary>
public static class Xoroshiro128PlusReversal
{
    /// <summary>
    /// Reverses two consecutive <see cref="Xoroshiro128Plus.Next()"/> calls (low 32-bits) to get the original 64-bit seed.
    /// </summary>
    /// <param name="seed">Resulting seed</param>
    /// <param name="out1">First output, low 32-bits</param>
    /// <param name="out2">Second output, low 32-bits</param>
    /// <param name="assume1">Carry bit</param>
    /// <param name="assume2">Brute-forced guess of bits 32-37</param>
    /// <returns>True if a satisfactory seed was found, false otherwise.</returns>
    public static bool Explore(out ulong seed, uint out1, uint out2, ulong assume1, ulong assume2)
    {
        seed = 0;

        // out1 = A + B
        seed |= (uint)unchecked(out1 - XOROSHIRO_CONST);

        // Assume 32-37
        seed |= ((assume2 & 0x3F) << 32);

        ulong s0 = seed, s1 = XOROSHIRO_CONST;

        s1 ^= s0;
        ulong x0 = s1;

        //s0 = BitOperations.RotateLeft(s0, 24) ^ s1 ^ (s1 << 16);
        s1 = RotateLeft(s1, 37);

        // Extract 40-50
        seed |= (((out2 - s1) ^ x0) & 0x7FF) << 40;

        // Assuming carry, extract 51-58
        seed |= ((((out2 >> 24) - (seed ^ (x0 >> 8) ^ (x0 >> 24)) - assume1) ^ (XOROSHIRO_CONST >> 51)) & 0xFF) << 51;

        // Extract 38-39
        seed |= ((((out2 - ((seed >> 40) ^ x0)) >> 11) ^ (XOROSHIRO_CONST >> 38)) & 3) << 38;

        // Extract the rest
        seed |= (((out2 - RotateLeft(seed ^ XOROSHIRO_CONST, 37)) >> 19) ^ (x0 >> 3) ^ (x0 >> 19)) << 59;

        var check = new Xoroshiro128Plus(seed);
        var test1 = check.Next();
        var test2 = check.Next();

        // Double-check our result.
        if ((uint)test1 != out1)
            return false;
        if ((uint)test2 != out2)
            return false;
        return true;
    }

    /// <summary>
    /// Reverses two consecutive <see cref="Xoroshiro128Plus.Next()"/> calls (low 32-bits) with an unknown result in between to get the original 64-bit seed.
    /// </summary>
    /// <param name="seed">Resulting seed</param>
    /// <param name="out1">First output, low 32-bits</param>
    /// <param name="out2">Second output, low 32-bits</param>
    /// <param name="assume1">Carry bit</param>
    /// <param name="assume2">Brute-forced guess of bits 32-36</param>
    /// <param name="assume3">Brute-forced guess of bits 48-53</param>
    /// <returns>True if a satisfactory seed was found, false otherwise.</returns>
    public static bool ExploreDouble(out ulong seed, uint out1, uint out2, ulong assume1, ulong assume2, ulong assume3)
    {
        seed = 0;

        // out1 = A + B
        seed |= (uint)unchecked(out1 - XOROSHIRO_CONST);
        seed |= ((assume2 & 0x1F) << 32); // Assume 32-36
        seed |= ((assume3 & 0x3F) << 48); // Assume 48-53

        ulong s0 = seed, s1 = XOROSHIRO_CONST;

        s1 ^= s0;
        ulong x0 = s1;

        s0 = RotateLeft(s0, 24) ^ s1 ^ (s1 << 16);
        s1 = RotateLeft(s1, 37);

        s1 ^= s0;

        s0 = RotateLeft(s0, 24) ^ s1 ^ (s1 << 16);
        //s1 = BitOperations.RotateLeft(s1, 37);

        // Extract 54-63
        seed |= ((out2 - s0) ^ ((seed >> 3) ^ (x0 >> 27) ^ (x0 >> 11) ^ (XOROSHIRO_CONST >> 54))) << 54;

        // Mix in the new bits
        x0 = seed ^ XOROSHIRO_CONST;
        seed |= ((((out2 >> 21) - ((seed >> 24) ^ (x0 >> 48) ^ (x0 >> 32) ^ (x0 >> 11)) - assume1) ^ ((XOROSHIRO_CONST >> 61) ^ (XOROSHIRO_CONST >> 45) ^ (x0 >> 21) ^ (x0 >> 48) ^ (x0 >> 32))) & 0x7) << 37;

        // Mix in the new bits
        x0 = seed ^ XOROSHIRO_CONST;
        seed |= (((((out2 >> 21) - ((seed >> 24) ^ (x0 >> 48) ^ (x0 >> 32) ^ (x0 >> 11)) - assume1) >> 3) ^ (XOROSHIRO_CONST ^ (seed >> 48) ^ (x0 >> 24) ^ (x0 >> 51) ^ (x0 >> 35))) & 0x1F) << 40;

        // Mix in the new bits
        x0 = seed ^ XOROSHIRO_CONST;
        seed |= (((((out2 >> 21) - ((seed >> 24) ^ (x0 >> 48) ^ (x0 >> 32) ^ (x0 >> 11)) - assume1) >> 3) ^ (XOROSHIRO_CONST ^ (seed >> 48) ^ (x0 >> 24) ^ (x0 >> 51) ^ (x0 >> 35))) & 0xFF) << 40;

        // Double-check our result.
        var check = new Xoroshiro128Plus(seed);
        var test1 = check.Next();
        _ = check.Next(); // gap
        var test2 = check.Next();

        if ((uint)test1 != out1)
            return false;
        if ((uint)test2 != out2)
            return false;
        return true;
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
    private byte assume1;
    private byte carry;

    public bool MoveNext()
    {
        do
        {
            while (carry < 2)
            {
                if (Xoroshiro128PlusReversal.Explore(out seed, First, Second, carry++, assume1))
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
    private byte assume1;
    private byte assume2;
    private byte carry;

    public bool MoveNext()
    {
        do
        {
            do
            {
                while (carry < 2)
                {
                    if (Xoroshiro128PlusReversal.ExploreDouble(out seed, First, Third, carry++, assume1, assume2))
                        return true;
                }
                carry = 0;
            } while (++assume2 < 0x40);
            assume2 = 0;
        } while (++assume1 < 0x20);
        return false;
    }

    // IEnumerator Implementation -- used for foreach syntax sugar inlining
    public void Reset() => assume1 = assume2 = carry = 0;
    readonly object IEnumerator.Current => Current;
    public readonly void Dispose() { }
    public readonly IEnumerator<ulong> GetEnumerator() => this;
}
