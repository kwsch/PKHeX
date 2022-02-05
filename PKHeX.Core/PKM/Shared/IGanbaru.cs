using System;

namespace PKHeX.Core;

public interface IGanbaru
{
    int GV_HP { get; set; }
    int GV_ATK { get; set; }
    int GV_DEF { get; set; }
    int GV_SPE { get; set; }
    int GV_SPA { get; set; }
    int GV_SPD { get; set; }
}

public static class GanbaruExtensions
{
    public const int TrueMax = 10;

    private static readonly ushort[] GanbaruMultiplier = { 0, 2, 3, 4, 7, 8, 9, 14, 15, 16, 25 };

    public static int GetMax(this IGanbaru _, PKM pk, int index) => GetMaxGanbaru(pk, index);

    public static int GetMaxGanbaru(this PKM pk, int index)
    {
        var iv = pk.GetIV(index);
        return GetMaxGanbaru(iv);
    }

    private static int GetMaxGanbaru(int iv)
    {
        var bias = GetBias(iv);
        return TrueMax - bias;
    }

    public static int GetBias(int iv) => iv switch
    {
        >= 31 => 3,
        >= 26 => 2,
        >= 20 => 1,
            _ => 0,
    };

    public static void SetSuggestedGanbaruValues(this IGanbaru g, PKM pk)
    {
        g.GV_HP  = GetMaxGanbaru(pk.IV_HP);
        g.GV_ATK = pk.IV_ATK == 0 ? 0 : GetMaxGanbaru(pk.IV_ATK);
        g.GV_DEF = GetMaxGanbaru(pk.IV_DEF);
        g.GV_SPE = pk.IV_SPE == 0 ? 0 : GetMaxGanbaru(pk.IV_SPE);
        g.GV_SPA = GetMaxGanbaru(pk.IV_SPA);
        g.GV_SPD = GetMaxGanbaru(pk.IV_SPD);
    }

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

    public static void ClearGanbaruValues(this IGanbaru g)
    {
        g.GV_HP  = 0;
        g.GV_ATK = 0;
        g.GV_DEF = 0;
        g.GV_SPE = 0;
        g.GV_SPA = 0;
        g.GV_SPD = 0;
    }

    public static int GetGanbaruMultiplier(int gv, int iv) => GanbaruMultiplier[Math.Min(gv + GetBias(iv), 10)];

    /// <summary>
    /// Sets one of the <see cref="IAwakened"/> values based on its index within the array.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="index">Index to set to</param>
    /// <param name="value">Value to set</param>
    public static int SetGV(this IGanbaru pk, int index, int value) => index switch
    {
        0 => pk.GV_HP = value,
        1 => pk.GV_ATK = value,
        2 => pk.GV_DEF = value,
        3 => pk.GV_SPE = value,
        4 => pk.GV_SPA = value,
        5 => pk.GV_SPD = value,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    /// <summary>
    /// Sets one of the <see cref="IAwakened"/> values based on its index within the array.
    /// </summary>
    /// <param name="pk">Pokémon to check.</param>
    /// <param name="index">Index to get</param>
    public static int GetGV(this IGanbaru pk, int index) => index switch
    {
        0 => pk.GV_HP,
        1 => pk.GV_ATK,
        2 => pk.GV_DEF,
        3 => pk.GV_SPE,
        4 => pk.GV_SPA,
        5 => pk.GV_SPD,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };
}
