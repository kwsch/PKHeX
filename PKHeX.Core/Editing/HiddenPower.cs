using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Logic for calculating a Hidden Power Type based on IVs and generation-format.
/// </summary>
public static class HiddenPower
{
    /// <summary>
    /// Gets the current Hidden Power Type of the input <see cref="IVs"/> for the requested format generation.
    /// </summary>
    /// <param name="IVs">Current IVs</param>
    /// <returns>Hidden Power Type of the <see cref="IVs"/></returns>
    /// <param name="context">Generation format</param>
    public static int GetType(ReadOnlySpan<int> IVs, EntityContext context)
    {
        if (context.Generation() <= 2)
            return GetTypeGB(IVs);
        return GetType(IVs);
    }

    /// <summary>
    /// Gets the current Hidden Power Type of the input <see cref="IVs"/> for Generations 3+
    /// </summary>
    /// <param name="IVs">Current IVs</param>
    /// <returns>Hidden Power Type of the <see cref="IVs"/></returns>
    public static int GetType(ReadOnlySpan<int> IVs)
    {
        int hp = 0;
        for (int i = 0; i < 6; i++)
            hp |= (IVs[i] & 1) << i;
        hp *= 0xF;
        hp /= 0x3F;
        return hp;
    }

    /// <summary>
    /// Gets the current Hidden Power Type of the input <see cref="IVs"/> for Generations 1 &amp; 2
    /// </summary>
    /// <param name="IVs">Current IVs</param>
    /// <returns>Hidden Power Type of the <see cref="IVs"/></returns>
    public static int GetTypeGB(ReadOnlySpan<int> IVs)
    {
        var atk = IVs[1];
        var def = IVs[2];
        return ((atk & 3) << 2) | (def & 3);
    }

    /// <summary>
    /// Modifies the provided <see cref="IVs"/> to have the requested <see cref="hiddenPowerType"/> for Generations 1 &amp; 2
    /// </summary>
    /// <param name="hiddenPowerType">Hidden Power Type</param>
    /// <param name="IVs">Current IVs</param>
    /// <returns>True if the Hidden Power of the <see cref="IVs"/> is obtained, with or without modifications</returns>
    public static bool SetTypeGB(int hiddenPowerType, Span<int> IVs)
    {
        IVs[1] = (IVs[1] & ~3) | (hiddenPowerType >> 2);
        IVs[2] = (IVs[2] & ~3) | (hiddenPowerType & 3);
        return true;
    }

    /// <summary>
    /// Modifies the provided <see cref="IVs"/> to have the requested <see cref="hiddenPowerType"/>.
    /// </summary>
    /// <param name="hiddenPowerType">Hidden Power Type</param>
    /// <param name="IVs">Current IVs (6 total)</param>
    /// <param name="context">Generation format</param>
    /// <returns>True if the Hidden Power of the <see cref="IVs"/> is obtained, with or without modifications</returns>
    public static bool SetIVsForType(int hiddenPowerType, Span<int> IVs, EntityContext context)
    {
        if (context.Generation() <= 2)
            return SetTypeGB(hiddenPowerType, IVs);
        return SetIVsForType(hiddenPowerType, IVs);
    }

    /// <summary>
    /// Sets the <see cref="IVs"/> to the requested <see cref="hpVal"/> for Generation 3+ game formats.
    /// </summary>
    /// <param name="hpVal">Hidden Power Type</param>
    /// <param name="IVs">Current IVs (6 total)</param>
    /// <returns>True if the Hidden Power of the <see cref="IVs"/> is obtained, with or without modifications</returns>
    public static bool SetIVsForType(int hpVal, Span<int> IVs)
    {
        int flawlessCount = IVs.Count(31);
        if (flawlessCount == 0)
            return false;

        if (flawlessCount == IVs.Length)
        {
            SetIVs(hpVal, IVs); // Get IVs
            return true;
        }

        int current = GetType(IVs);
        if (current == hpVal)
            return true; // no mods necessary

        // Required HP type doesn't match IVs. Make currently-flawless IVs flawed.
        Span<int> scratch = stackalloc int[IVs.Length];
        Span<int> result = stackalloc int[IVs.Length];
        var success = GetSuggestedHiddenPowerIVs(hpVal, IVs, scratch, result);
        if (!success)
            return false; // can't force hidden power?

        // set IVs back to array
        result.CopyTo(IVs);
        return true;
    }

    // Non-recursive https://en.wikipedia.org/wiki/Heap%27s_algorithm
    private static bool GetSuggestedHiddenPowerIVs(int hpVal, ReadOnlySpan<int> original, Span<int> ivs, Span<int> best)
    {
        const int max = 31;

        // Get a list of indexes that can be mutated
        Span<int> indexes = stackalloc int[original.Length];
        int flaw = 0;
        for (int i = 0; i < original.Length; i++)
        {
            if (original[i] == max)
                indexes[flaw++] = i;
        }
        indexes = indexes[..flaw];
        Span<int> c = stackalloc int[indexes.Length];

        int mutated = c.Length + 1; // result tracking
        for (int i = 1; i < c.Length;)
        {
            ref int ci = ref c[i];
            if (i <= ci) // Reset the state and simulate popping the stack by incrementing the pointer.
            {
                ci = 0;
                ++i;
                continue;
            }

            var x = (i & 1) * ci; // if lowest bit set, ci : 0 (branch-less)
            Swap(ref indexes[i], ref indexes[x]);

            // Inlined continuance check
            original.CopyTo(ivs);
            var q = Math.Min(indexes.Length, mutated);
            for (var j = 0; j < q; j++)
            {
                ivs[indexes[j]] ^= 1;
                if (hpVal != GetType(ivs))
                    continue;

                var ct = j + 1;
                if (ct >= mutated)
                    break; // any further flaws are always worse

                mutated = ct;
                ivs.CopyTo(best);
                if (j == 0) // nothing will be better than only 1 flaw
                    return true;
                break; // any further flaws are always worse
            }

            ci++;
            i = 1;
        }

        return mutated <= c.Length; // did we actually find a suitable result?
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Swap<T>(ref T a, ref T b) => (a, b) = (b, a);

    /// <summary>Calculate the Hidden Power Type of the entered IVs.</summary>
    /// <param name="type">Hidden Power Type</param>
    /// <param name="ivs">Individual Values (H/A/B/S/C/D)</param>
    /// <param name="context">Generation specific format</param>
    public static void SetIVs(int type, Span<int> ivs, EntityContext context = PKX.Context)
    {
        if (context.Generation() <= 2)
        {
            ivs[1] = (ivs[1] & ~3) | (type >> 2);
            ivs[2] = (ivs[2] & ~3) | (type & 3);
        }
        else
        {
            var bits = DefaultLowBits[type];
            for (int i = 0; i < 6; i++)
                ivs[i] = (ivs[i] & 0x1E) + ((bits >> i) & 1);
        }
    }

    /// <summary>
    /// Hidden Power IV values (even or odd) to achieve a specified Hidden Power Type
    /// </summary>
    /// <remarks>
    /// There are other IV combinations to achieve the same Hidden Power Type.
    /// These are just precomputed for fast modification.
    /// Individual Values (H/A/B/S/C/D)
    /// </remarks>
    public static readonly byte[] DefaultLowBits =
    {
        0b000011, // Fighting
        0b001000, // Flying
        0b001011, // Poison
        0b001111, // Ground
        0b010011, // Rock
        0b011001, // Bug
        0b011101, // Ghost
        0b011111, // Steel
        0b100101, // Fire
        0b101001, // Water
        0b101101, // Grass
        0b101111, // Electric
        0b110101, // Psychic
        0b111001, // Ice
        0b111101, // Dragon
        0b111111, // Dark
    };
}
