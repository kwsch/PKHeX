using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for <see cref="Species.Toxtricity"></see> forms.
/// </summary>
public static class ToxtricityUtil
{
    /// <summary> Amped Nature Table </summary>
    private static ReadOnlySpan<byte> Nature0 => [ 3, 4, 2, 8, 9, 19, 22, 11, 13, 14, 0, 6, 24 ];
    /// <summary> Low Key Nature Table </summary>
    private static ReadOnlySpan<byte> Nature1 => [ 1, 5, 7, 10, 12, 15, 16, 17, 18, 20, 21, 23 ];

    /// <summary>
    /// Gets a random nature for Toxel -> Toxtricity.
    /// </summary>
    /// <param name="rand">Random number generator</param>
    /// <param name="form">Desired Toxtricity form</param>
    /// <returns>0 for Amped, 1 for Low Key</returns>
    public static Nature GetRandomNature(ref Xoroshiro128Plus rand, byte form)
    {
        var table = form == 0 ? Nature0 : Nature1;
        return (Nature)table[(int)rand.NextInt((uint)table.Length)];
    }

    /// <summary>
    /// Calculates the evolution form for Toxel -> Toxtricity.
    /// </summary>
    /// <param name="nature">Entity nature</param>
    /// <returns>0 for Amped, 1 for Low Key</returns>
    public static int GetAmpLowKeyResult(Nature nature)
    {
        var index = nature - 1;
        if ((uint)index > 22)
            return 0;
        return (0b_0101_1011_1100_1010_0101_0001 >> (int)index) & 1;
    }
}
