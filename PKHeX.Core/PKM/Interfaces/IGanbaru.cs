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
    /// Gets the stat calculation modifier using the saved <see cref="gv"/> and base <see cref="iv"/>.
    /// </summary>
    public static byte GetGanbaruMultiplier(byte gv, int iv) => GanbaruMultiplier[Math.Min(gv + GetBias(iv), TrueMax)];
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

    extension(PKM pk)
    {
        /// <summary>
        /// Gets the max possible value that can be legally stored for the specific stat <see cref="index"/>.
        /// </summary>
        public byte GetMaxGanbaru(int index)
        {
            var iv = pk.GetIV(index);
            return GetMaxGanbaru(iv);
        }
    }

    extension(IGanbaru g)
    {
        /// <summary>
        /// Gets the max possible value that can be legally stored for the specific stat <see cref="index"/>.
        /// </summary>
        public byte GetMax(PKM pk, int index) => pk.GetMaxGanbaru(index);

        /// <summary>
        /// Sets all values to the maximum. Attack and Speed are set to 0 if the corresponding IV is zero.
        /// </summary>
        public void SetSuggestedGanbaruValues(PKM pk)
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
        public bool IsGanbaruValuesMax(PKM pk)
        {
            var result = true;
            result &= g.GV_HP == GetMaxGanbaru(pk.IV_HP);
            result &= IsMaxOr01(g.GV_ATK, pk.IV_ATK);
            result &= g.GV_DEF == GetMaxGanbaru(pk.IV_DEF);
            result &= g.GV_SPA == GetMaxGanbaru(pk.IV_SPA);
            result &= g.GV_SPD == GetMaxGanbaru(pk.IV_SPD);
            result &= IsMaxOr01(g.GV_SPE, pk.IV_SPE);
            return result;

            static bool IsMaxOr01(byte gv, int iv)
            {
                if (iv <= 1 && gv == 0)
                    return true;
                return gv == GetMaxGanbaru(iv);
            }
        }

        /// <summary>
        /// Sets all values to 0.
        /// </summary>
        public void ClearGanbaruValues()
        {
            g.GV_HP  = 0;
            g.GV_ATK = 0;
            g.GV_DEF = 0;
            g.GV_SPE = 0;
            g.GV_SPA = 0;
            g.GV_SPD = 0;
        }

        /// <summary>
        /// Sets one of the <see cref="IGanbaru"/> values based on its index within the array.
        /// </summary>
        /// <param name="index">Index to set to</param>
        /// <param name="value">Value to set</param>
        public byte SetGV(int index, byte value) => index switch
        {
            0 => g.GV_HP  = value,
            1 => g.GV_ATK = value,
            2 => g.GV_DEF = value,
            3 => g.GV_SPE = value,
            4 => g.GV_SPA = value,
            5 => g.GV_SPD = value,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be between 0 and 5."),
        };

        /// <summary>
        /// Sets one of the <see cref="IGanbaru"/> values based on its index within the array.
        /// </summary>
        /// <param name="index">Index to get</param>
        public byte GetGV(int index) => index switch
        {
            0 => g.GV_HP,
            1 => g.GV_ATK,
            2 => g.GV_DEF,
            3 => g.GV_SPE,
            4 => g.GV_SPA,
            5 => g.GV_SPD,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be between 0 and 5."),
        };

        /// <summary>
        /// Loads the <see cref="IGanbaru"/> values and stores them in the provided <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="value">Span to store values in.</param>
        public void GetGVs(Span<byte> value)
        {
            if (value.Length != 6)
                return;
            value[0] = g.GV_HP;
            value[1] = g.GV_ATK;
            value[2] = g.GV_DEF;
            value[3] = g.GV_SPE;
            value[4] = g.GV_SPA;
            value[5] = g.GV_SPD;
        }

        /// <summary>
        /// Checks if any of the <see cref="IGanbaru"/> values are below a reference's minimum value.
        /// </summary>
        /// <param name="obj">Reference to check</param>
        public bool IsGanbaruValuesBelow(IGanbaru obj)
        {
            if (g.GV_HP < obj.GV_HP)
                return true;
            if (g.GV_ATK < obj.GV_ATK)
                return true;
            if (g.GV_DEF < obj.GV_DEF)
                return true;
            if (g.GV_SPA < obj.GV_SPA)
                return true;
            if (g.GV_SPD < obj.GV_SPD)
                return true;
            if (g.GV_SPE < obj.GV_SPE)
                return true;
            return false;
        }
    }
}
