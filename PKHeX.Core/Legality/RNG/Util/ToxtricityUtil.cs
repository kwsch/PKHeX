using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for <see cref="Species.Toxtricity"></see> forms.
/// </summary>
public static class ToxtricityUtil
{
    /// <summary> Amped Nature Table </summary>
    private static ReadOnlySpan<byte> Nature0 => [ 03, 04, 02, 08, 09, 19, 22, 11, 13, 14, 00, 06, 24 ];
    /// <summary> Low Key Nature Table </summary>
    private static ReadOnlySpan<byte> Nature1 => [ 01, 05, 07, 10, 12, 15, 16, 17, 18, 20, 21, 23 ];

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

    /// <summary>
    /// Gets a random nature for Toxel -> Toxtricity, requiring a neutral nature.
    /// </summary>
    /// <param name="form">Target form</param>
    /// <param name="rand">Random number value (not from the generator, just a salt)</param>
    /// <returns>0/6 for 0, 12/18 for 1</returns>
    public static Nature GetRandomNatureNeutral(byte form, uint rand) => (Nature)(6 * ((rand & 1) | (uint)(form << 1)));

    /// <inheritdoc cref="GetRandomNatureNeutral(byte, uint)"/>
    public static Nature GetRandomNatureNeutral(byte form) => form == 0 ? Nature.Hardy : Nature.Serious;
}
