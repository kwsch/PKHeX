using System;

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
        return SixBitType[hp];
    }

    /// <summary>
    /// Gets the current Hidden Power Type of the input IVs for Generations 3+
    /// </summary>
    /// <param name="u32">32-bit value of the IVs</param>
    /// <returns>Hidden Power Type of the IVs</returns>
    public static int GetType(uint u32)
    {
        uint hp = 0;
        for (int i = 0; i < 6; i++)
        {
            hp |= (u32 & 1) << i;
            u32 >>= 5;
        }
        return SixBitType[(int)hp];
    }

    /// <summary>
    /// Gets the current Hidden Power Type of the input IVs for Generations 3+
    /// </summary>
    /// <param name="u32">32-bit value of the IVs</param>
    /// <remarks>IVs are stored in reverse order in the 32-bit value</remarks>
    /// <returns>Hidden Power Type of the IVs</returns>
    public static int GetTypeBigEndian(uint u32)
    {
        uint hp = 0;
        for (int i = 0; i < 6; i++)
        {
            hp |= (u32 & 1) << (5 - i);
            u32 >>= 5;
        }
        return SixBitType[(int)hp];
    }

    /// <summary>
    /// Count of unique Hidden Power Types
    /// </summary>
    public const int TypeCount = 16;

    /// <summary>
    /// Gets the Type Name index of the input Hidden Power Type
    /// </summary>
    /// <param name="type">Fetched Hidden Power Type</param>
    /// <param name="index">Type Name index</param>
    /// <returns>True if the input Hidden Power Type is valid</returns>
    public static bool TryGetTypeIndex(int type, out byte index)
    {
        if ((uint)type >= TypeCount)
        {
            index = default;
            return false;
        }
        index = (byte)(type + 1); // Normal type is not a valid Hidden Power type
        return true;
    }

    private static ReadOnlySpan<byte> SixBitType =>
    [
        // (low-bit mash) * 15 / 63
        00, 00, 00, 00, 00, 01, 01, 01,
        01, 02, 02, 02, 02, 03, 03, 03,
        03, 04, 04, 04, 04, 05, 05, 05,
        05, 05, 06, 06, 06, 06, 07, 07,
        07, 07, 08, 08, 08, 08, 09, 09,
        09, 09, 10, 10, 10, 10, 10, 11,
        11, 11, 11, 12, 12, 12, 12, 13,
        13, 13, 13, 14, 14, 14, 14, 15,
    ];

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

    /// <inheritdoc cref="GetTypeGB(ReadOnlySpan{int})"/>
    public static int GetTypeGB(ushort u16) => ((u16 >> 10) & 0b1100) | ((u16 >> 8) & 0b11);

    /// <summary>
    /// Modifies the provided <see cref="IVs"/> to have the requested <see cref="hiddenPowerType"/> for Generations 1 &amp; 2
    /// </summary>
    /// <param name="hiddenPowerType">Hidden Power Type</param>
    /// <param name="IVs">Current IVs</param>
    /// <returns>True if the Hidden Power of the <see cref="IVs"/> is obtained, with or without modifications</returns>
    public static bool SetTypeGB(int hiddenPowerType, Span<int> IVs)
    {
        IVs[1] = (IVs[1] & 0b1100) | (hiddenPowerType >> 2);
        IVs[2] = (IVs[2] & 0b1100) | (hiddenPowerType & 3);
        return true;
    }

    /// <inheritdoc cref="SetTypeGB(int, Span{int})"/>
    public static ushort SetTypeGB(int hiddenPowerType, ushort current)
    {
        // Extract bits from ATK and DEF.
        var u16 = ((hiddenPowerType & 0b1100) << 10) | ((hiddenPowerType & 0b11) << 8);
        return (ushort)((current & 0b1100_1100_1111_1111) | u16);
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
        int current = GetType(IVs);
        if (current == hpVal)
            return true; // no mods necessary

        int flawlessCount = IVs.Count(31);
        if (flawlessCount == 0)
            return false;

        if (flawlessCount == IVs.Length)
        {
            SetIVs(hpVal, IVs); // Get IVs
            return true;
        }

        // Required HP type doesn't match IVs. Make currently-flawless IVs flawed.
        var bits = GetSuggestedHiddenPowerIVs(hpVal, IVs);
        if (bits == NoResult)
            return false; // can't force hidden power?

        // set IVs back to array
        ForceLowBits(IVs, bits);
        return true;
    }

    private const byte NoResult = byte.MaxValue;

    private static byte GetSuggestedHiddenPowerIVs(int hpVal, ReadOnlySpan<int> IVs)
    {
        // Iterate through all bit combinations that yield our Hidden Power Type.
        // There's at most 5 we need to check, so brute force is fine.
        // Prefer the least amount of IVs changed (31 -> 30).

        // Get the starting index from our 64 possible bit states.
        int index = SixBitType.IndexOf((byte)hpVal);
        if (index == -1)
            return NoResult;

        var bestIndex = NoResult;
        var bestIndexFlaws = 6;
        do
        {
            var flaws = GetFlawedBitCount(IVs, index);
            if (flaws >= bestIndexFlaws)
                continue;
            bestIndex = (byte)index;
            bestIndexFlaws = flaws;
        } while (++index < SixBitType.Length && SixBitType[index] == hpVal);
        return bestIndex;
    }

    private static int GetFlawedBitCount(ReadOnlySpan<int> ivs, int bitValue)
    {
        const int max = 31;
        int flaws = 0;
        for (int i = 0; i < ivs.Length; i++)
        {
            var iv = ivs[i];
            if ((iv & 1) == (bitValue & (1 << i)))
                continue; // ok
            if (iv != max)
                return NoResult;
            flaws++;
        }
        return flaws;
    }

    /// <summary>Calculate the Hidden Power Type of the entered IVs.</summary>
    /// <param name="type">Hidden Power Type</param>
    /// <param name="ivs">Individual Values (H/A/B/S/C/D)</param>
    /// <param name="context">Generation specific format</param>
    public static void SetIVs(int type, Span<int> ivs, EntityContext context = PKX.Context)
    {
        if (context.Generation() <= 2)
        {
            ivs[1] = (ivs[1] & 0b1100) | (type >> 2);
            ivs[2] = (ivs[2] & 0b1100) | (type & 3);
        }
        else
        {
            ForceLowBits(ivs, DefaultLowBits[type]);
        }
    }

    private static void ForceLowBits(Span<int> ivs, byte bits)
    {
        for (int i = 0; i < ivs.Length; i++)
            ivs[i] = (ivs[i] & 0b11110) | ((bits >> i) & 1);
    }

    /// <inheritdoc cref="SetIVs(int,Span{int},EntityContext)"/>
    public static uint SetIVs(int type, uint ivs)
    {
        var bits = DefaultLowBits[type];
        for (int i = 0; i < 6; i++)
        {
            var bit = (bits >> i) & 1;
            var bitIndex = i * 5;
            var mask = (1u << bitIndex);
            if (bit == 0)
                ivs &= ~mask;
            else
                ivs |= mask;
        }
        return ivs;
    }

    /// <inheritdoc cref="SetIVs(int,uint)"/>
    /// <remarks>IVs are stored in reverse order in the 32-bit value</remarks>
    public static uint SetIVsBigEndian(int type, uint ivs)
    {
        var bits = DefaultLowBits[type];
        for (int i = 0; i < 6; i++)
        {
            var bit = (bits >> i) & 1;
            var bitIndex = (5 - i) * 5;
            var mask = (1u << bitIndex);
            if (bit == 0)
                ivs &= ~mask;
            else
                ivs |= mask;
        }
        return ivs;
    }

    /// <summary>
    /// Hidden Power IV values (even or odd) to achieve a specified Hidden Power Type
    /// </summary>
    /// <remarks>
    /// There are other IV combinations to achieve the same Hidden Power Type.
    /// These are just precomputed for fast modification.
    /// Individual Values (H/A/B/S/C/D)
    /// </remarks>
    public static ReadOnlySpan<byte> DefaultLowBits =>
    [
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
    ];

    /// <summary>
    /// Gets the suggested low-bits for the input Hidden Power Type
    /// </summary>
    public static byte GetLowBits(int type)
    {
        var arr = DefaultLowBits;
        return (uint)type < arr.Length ? arr[type] : (byte)0;
    }
}
