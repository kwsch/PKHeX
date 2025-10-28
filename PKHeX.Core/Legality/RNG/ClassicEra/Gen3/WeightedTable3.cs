using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Event Table Weighting Algorithm.
/// </summary>
public static class WeightedTable3
{
    /// <summary>
    /// Gets the random weight value, with a periodic frequency of repeating (*about* every 0x8000).
    /// </summary>
    /// <param name="rand">32-bit random number</param>
    /// <param name="max">Exclusive maximum value that can be returned</param>
    /// <returns>[0, max)</returns>
    public static uint GetPeriodicWeight(uint rand, [ConstantExpected] uint max)
    {
        var high = rand >> 16;
        var first = ((high << 2) & 0xFFFF) + high;
        var second = ((rand & 0xFFFF) << 1) + (first >> 16);
        second += high + (second >> 16);
        return (max * (second & 0xFFFF)) >> 16;
    }

    /// <inheritdoc cref="GetRandom32(ref uint)"/>
    public static uint GetRandom32(uint seed) => GetRandom32(ref seed);

    /// <summary>
    /// Gets a 32-bit random number from the given seed to use as the table random selector.
    /// </summary>
    public static uint GetRandom32(ref uint seed)
    {
        var rand1 = LCRNG.Next16(ref seed);
        var rand2 = LCRNG.Next16(ref seed);
        return (rand1 << 16) | rand2;
    }
}
