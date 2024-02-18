using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about the amount of Awakened stat boosts gained.
/// </summary>
/// <remarks>Used only in LGP/E.</remarks>
public interface IAwakened
{
    byte AV_HP { get; set; }
    byte AV_ATK { get; set; }
    byte AV_DEF { get; set; }
    byte AV_SPE { get; set; }
    byte AV_SPA { get; set; }
    byte AV_SPD { get; set; }
}

/// <summary>
/// Logic for interacting with LGP/E Awakening values.
/// </summary>
public static class AwakeningUtil
{
    public const byte AwakeningMax = 200;

    /// <summary>
    /// Sums all values.
    /// </summary>
    /// <param name="pk">Data to sum with</param>
    public static int AwakeningSum(this IAwakened pk) => pk.AV_HP + pk.AV_ATK + pk.AV_DEF + pk.AV_SPE + pk.AV_SPA + pk.AV_SPD;

    /// <summary>
    /// Clears all values.
    /// </summary>
    /// <param name="pk">Data to clear from</param>
    public static void AwakeningClear(this IAwakened pk) => pk.AwakeningSetAllTo(0);

    /// <summary>
    /// Sets all values to the maximum value.
    /// </summary>
    /// <param name="pk">Data to set values for</param>
    public static void AwakeningMaximize(this IAwakened pk) => pk.AwakeningSetAllTo(AwakeningMax);

    /// <summary>
    /// Sets all values to the specified value.
    /// </summary>
    /// <param name="pk">Data to set values for</param>
    /// <param name="value">Value to set all to</param>
    public static void AwakeningSetAllTo(this IAwakened pk, byte value) => pk.AV_HP = pk.AV_ATK = pk.AV_DEF = pk.AV_SPE = pk.AV_SPA = pk.AV_SPD = value;

    /// <summary>
    /// Sets all values to the bare minimum legal value.
    /// </summary>
    /// <param name="pk">Data to set values for</param>
    public static void AwakeningMinimum(this IAwakened pk)
    {
        if (pk is not PB7 pb7)
            return;

        Span<byte> result = stackalloc byte[6];
        SetExpectedMinimumAVs(result, pb7);
        AwakeningSetVisual(pb7, result);
    }

    /// <summary>
    /// Sets all values to the specified value.
    /// </summary>
    /// <param name="pk">Data to set values for</param>
    /// <param name="min">Minimum value to set</param>
    /// <param name="max">Maximum value to set</param>
    public static void AwakeningSetRandom(this IAwakened pk, byte min = 0, int max = AwakeningMax)
    {
        if (pk is not PB7 pb7)
            return;

        Span<byte> result = stackalloc byte[6];
        SetExpectedMinimumAVs(result, pb7);

        var rnd = Util.Rand;
        foreach (ref var av in result)
        {
            var realMin = Math.Max(min, av);
            var realMax = Math.Max(av, max);
            av = (byte)rnd.Next(realMin, realMax + 1);
        }
        AwakeningSetVisual(pb7, result);
    }

    /// <summary>
    /// Gets the awakening values according to their displayed order.
    /// </summary>
    /// <param name="pk">Data to set values for</param>
    /// <param name="value">Value storage result</param>
    public static void AwakeningGetVisual(IAwakened pk, Span<byte> value)
    {
        value[0] = pk.AV_HP;
        value[1] = pk.AV_ATK;
        value[2] = pk.AV_DEF;
        value[3] = pk.AV_SPA;
        value[4] = pk.AV_SPD;
        value[5] = pk.AV_SPE;
    }

    /// <summary>
    /// Sets the awakening values according to their displayed order.
    /// </summary>
    /// <param name="pk">Data to set values for</param>
    /// <param name="value">Value storage to set from</param>
    public static void AwakeningSetVisual(IAwakened pk, ReadOnlySpan<byte> value)
    {
        pk.AV_HP = value[0];
        pk.AV_ATK = value[1];
        pk.AV_DEF = value[2];
        pk.AV_SPA = value[3];
        pk.AV_SPD = value[4];
        pk.AV_SPE = value[5];
    }

    /// <summary>
    /// Gets if all values are within legal limits.
    /// </summary>
    /// <param name="pk">Data to check</param>
    public static bool AwakeningAllValid(this IAwakened pk)
    {
        if (pk.AV_HP > AwakeningMax)
            return false;
        if (pk.AV_ATK > AwakeningMax)
            return false;
        if (pk.AV_DEF > AwakeningMax)
            return false;
        if (pk.AV_SPE > AwakeningMax)
            return false;
        if (pk.AV_SPA > AwakeningMax)
            return false;
        if (pk.AV_SPD > AwakeningMax)
            return false;
        return true;
    }

    /// <summary>
    /// Sets one of the <see cref="IAwakened"/> values based on its index within the array.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="index">Index to set to</param>
    /// <param name="value">Value to set</param>
    public static byte SetAV(this IAwakened pk, int index, byte value) => index switch
    {
        0 => pk.AV_HP = value,
        1 => pk.AV_ATK = value,
        2 => pk.AV_DEF = value,
        3 => pk.AV_SPE = value,
        4 => pk.AV_SPA = value,
        5 => pk.AV_SPD = value,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    /// <summary>
    /// Sets one of the <see cref="IAwakened"/> values based on its index within the array.
    /// </summary>
    /// <param name="pk">Pokémon to check.</param>
    /// <param name="index">Index to get</param>
    public static byte GetAV(this IAwakened pk, int index) => index switch
    {
        0 => pk.AV_HP,
        1 => pk.AV_ATK,
        2 => pk.AV_DEF,
        3 => pk.AV_SPE,
        4 => pk.AV_SPA,
        5 => pk.AV_SPD,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    /// <summary>
    /// Loads the <see cref="IAwakened"/> values to the input span.
    /// </summary>
    public static void GetAVs(this IAwakened pk, Span<byte> value)
    {
        if (value.Length != 6)
            return;
        value[0] = pk.AV_HP;
        value[1] = pk.AV_ATK;
        value[2] = pk.AV_DEF;
        value[3] = pk.AV_SPE;
        value[4] = pk.AV_SPA;
        value[5] = pk.AV_SPD;
    }

    /// <summary>
    /// Sets the values based on the current <see cref="PKM.IVs"/>.
    /// </summary>
    /// <param name="a">Accessor for setting the values</param>
    /// <param name="pk">Retriever for IVs</param>
    public static void SetSuggestedAwakenedValues(this IAwakened a, PKM pk)
    {
        Span<byte> result = stackalloc byte[6];
        SetExpectedMinimumAVs(result, (PB7)a);
        a.AV_HP  = AwakeningMax;
        a.AV_ATK = pk.IV_ATK == 0 ? result[1] : AwakeningMax;
        a.AV_DEF = AwakeningMax;
        a.AV_SPA = AwakeningMax;
        a.AV_SPD = AwakeningMax;
        a.AV_SPE = pk.IV_SPE == 0 ? result[5] : AwakeningMax;
    }

    public static bool IsAwakeningBelow(this IAwakened current, IAwakened initial) => !current.IsAwakeningAboveOrEqual(initial);

    /// <summary>
    /// Checks if the <see cref="current"/> has values greater or equal to the <see cref="initial"/>.
    /// </summary>
    public static bool IsAwakeningAboveOrEqual(this IAwakened current, IAwakened initial)
    {
        if (current.AV_HP < initial.AV_HP)
            return false;
        if (current.AV_ATK < initial.AV_ATK)
            return false;
        if (current.AV_DEF < initial.AV_DEF)
            return false;
        if (current.AV_SPA < initial.AV_SPA)
            return false;
        if (current.AV_SPD < initial.AV_SPD)
            return false;
        if (current.AV_SPE < initial.AV_SPE)
            return false;
        return true;
    }

    /// <summary>
    /// Updates the <see cref="result"/> span with the expected minimum values for each <see cref="IAwakened"/> index.
    /// </summary>
    /// <param name="result">Stat results</param>
    /// <param name="pk">Entity to check</param>
    public static void SetExpectedMinimumAVs(Span<byte> result, PB7 pk)
    {
        // GO Park transfers start with 2 AVs for all stats.
        // Every other encounter is either all 0, or can legally start at 0 (trades).
        if (pk.Version == GameVersion.GO)
            result.Fill(GP1.InitialAV);

        // Leveling up in-game adds 1 AV to a "random" index.
        var start = pk.MetLevel;
        var end = pk.CurrentLevel;
        if (start == end)
            return;

        // Level up from met level to current level.
        var nature = pk.Nature;
        var character = pk.Characteristic;
        var ec = pk.EncryptionConstant;

        for (int i = start + 1; i <= end; i++)
        {
            var lm10 = i % 10;
            var bits = (ec >> (3 * lm10)) & 7;
            var index = PB7.GetRandomIndex((int)bits, character, nature);
            ++result[index];
        }
    }
}
