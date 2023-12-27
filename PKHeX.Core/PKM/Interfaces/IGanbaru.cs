using System;

namespace PKHeX.Core;

/// <summary>
/// Effort Level stat values gained via applied Grit items.
/// </summary>
public interface IGanbaru
{
    byte GV_HP { get; set; }
    byte GV_ATK { get; set; }
    byte GV_DEF { get; set; }
    byte GV_SPE { get; set; }
    byte GV_SPA { get; set; }
    byte GV_SPD { get; set; }
}

public static class GanbaruExtensions
{
    /// <summary>
    /// Overall maximum value permitted (adding the base factor and gained value).
    /// </summary>
    public const byte TrueMax = 10;

    private static ReadOnlySpan<byte> GanbaruMultiplier => [ 0, 2, 3, 4, 7, 8, 9, 14, 15, 16, 25 ];

    /// <summary>
    /// Gets the max possible value that can be legally stored for the specific stat <see cref="index"/>.
    /// </summary>
    public static byte GetMax(this IGanbaru _, PKM pk, int index) => GetMaxGanbaru(pk, index);

    /// <summary>
    /// Gets the max possible value that can be legally stored for the specific stat <see cref="index"/>.
    /// </summary>
    public static byte GetMaxGanbaru(this PKM pk, int index)
    {
        var iv = pk.GetIV(index);
        return GetMaxGanbaru(iv);
    }

    /// <summary>
    /// Gets the max possible value that can be legally stored for a stat with value <see cref="iv"/>.
    /// </summary>
    private static byte GetMaxGanbaru(int iv)
    {
        var bias = GetBias(iv);
        return (byte)(TrueMax - bias);
    }

    /// <summary>
    /// Gets the added boost for a stat with a base potential <see cref="iv"/>.
    /// </summary>
    public static byte GetBias(int iv) => iv switch
    {
        >= 31 => 3,
        >= 26 => 2,
        >= 20 => 1,
            _ => 0,
    };

    /// <summary>
    /// Sets all values to the maximum. Attack and Speed are set to 0 if the corresponding IV is zero.
    /// </summary>
    public static void SetSuggestedGanbaruValues(this IGanbaru g, PKM pk)
    {
        g.GV_HP  = GetMaxGanbaru(pk.IV_HP);
        g.GV_ATK = pk.IV_ATK == 0 ? (byte)0: GetMaxGanbaru(pk.IV_ATK);
        g.GV_DEF = GetMaxGanbaru(pk.IV_DEF);
        g.GV_SPE = pk.IV_SPE == 0 ? (byte)0 : GetMaxGanbaru(pk.IV_SPE);
        g.GV_SPA = GetMaxGanbaru(pk.IV_SPA);
        g.GV_SPD = GetMaxGanbaru(pk.IV_SPD);
    }

    /// <summary>
    /// Checks if the values are at the favorable maximum per <see cref="SetSuggestedGanbaruValues"/>.
    /// </summary>
    public static bool IsGanbaruValuesMax(this IGanbaru g, PKM pk)
    {
        var result = true;
        result &= g.GV_HP == GetMaxGanbaru(pk.IV_HP);
        result &= g.GV_ATK >= (pk.IV_ATK == 0 ? 0 : GetMaxGanbaru(pk.IV_ATK));
        result &= g.GV_DEF == GetMaxGanbaru(pk.IV_DEF);
        result &= g.GV_SPE >= (pk.IV_SPE == 0 ? 0 : GetMaxGanbaru(pk.IV_SPE));
        result &= g.GV_SPA == GetMaxGanbaru(pk.IV_SPA);
        result &= g.GV_SPD == GetMaxGanbaru(pk.IV_SPD);
        return result;
    }

    /// <summary>
    /// Sets all values to 0.
    /// </summary>
    public static void ClearGanbaruValues(this IGanbaru g)
    {
        g.GV_HP  = 0;
        g.GV_ATK = 0;
        g.GV_DEF = 0;
        g.GV_SPE = 0;
        g.GV_SPA = 0;
        g.GV_SPD = 0;
    }

    /// <summary>
    /// Gets the stat calculation modifier using the saved <see cref="gv"/> and base <see cref="iv"/>.
    /// </summary>
    public static byte GetGanbaruMultiplier(byte gv, int iv) => GanbaruMultiplier[Math.Min(gv + GetBias(iv), TrueMax)];

    /// <summary>
    /// Sets one of the <see cref="IGanbaru"/> values based on its index within the array.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="index">Index to set to</param>
    /// <param name="value">Value to set</param>
    public static byte SetGV(this IGanbaru pk, int index, byte value) => index switch
    {
        0 => pk.GV_HP  = value,
        1 => pk.GV_ATK = value,
        2 => pk.GV_DEF = value,
        3 => pk.GV_SPE = value,
        4 => pk.GV_SPA = value,
        5 => pk.GV_SPD = value,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be between 0 and 5."),
    };

    /// <summary>
    /// Sets one of the <see cref="IGanbaru"/> values based on its index within the array.
    /// </summary>
    /// <param name="pk">Pokémon to check.</param>
    /// <param name="index">Index to get</param>
    public static byte GetGV(this IGanbaru pk, int index) => index switch
    {
        0 => pk.GV_HP,
        1 => pk.GV_ATK,
        2 => pk.GV_DEF,
        3 => pk.GV_SPE,
        4 => pk.GV_SPA,
        5 => pk.GV_SPD,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be between 0 and 5."),
    };

    /// <summary>
    /// Loads the <see cref="IGanbaru"/> values and stores them in the provided <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="pk">Pokémon to check.</param>
    /// <param name="value">Span to store values in.</param>
    public static void GetGVs(this IGanbaru pk, Span<byte> value)
    {
        if (value.Length != 6)
            return;
        value[0] = pk.GV_HP;
        value[1] = pk.GV_ATK;
        value[2] = pk.GV_DEF;
        value[3] = pk.GV_SPE;
        value[4] = pk.GV_SPA;
        value[5] = pk.GV_SPD;
    }

    /// <summary>
    /// Checks if any of the <see cref="IGanbaru"/> values are below a reference's minimum value.
    /// </summary>
    /// <param name="pk">Pokémon to check.</param>
    /// <param name="obj">Reference to check</param>
    public static bool IsGanbaruValuesBelow(this IGanbaru pk, IGanbaru obj)
    {
        if (pk.GV_HP < obj.GV_HP)
            return true;
        if (pk.GV_ATK < obj.GV_ATK)
            return true;
        if (pk.GV_DEF < obj.GV_DEF)
            return true;
        if (pk.GV_SPA < obj.GV_SPA)
            return true;
        if (pk.GV_SPD < obj.GV_SPD)
            return true;
        if (pk.GV_SPE < obj.GV_SPE)
            return true;
        return false;
    }
}
