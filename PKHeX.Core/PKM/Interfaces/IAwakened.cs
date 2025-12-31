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

    extension(IAwakened pk)
    {
        /// <summary>
        /// Sums all values.
        /// </summary>
        public int AwakeningSum() => pk.AV_HP + pk.AV_ATK + pk.AV_DEF + pk.AV_SPE + pk.AV_SPA + pk.AV_SPD;

        /// <summary>
        /// Clears all values.
        /// </summary>
        public void AwakeningClear() => pk.AwakeningSetAllTo(0);

        /// <summary>
        /// Sets all values to the maximum value.
        /// </summary>
        public void AwakeningMaximize() => pk.AwakeningSetAllTo(AwakeningMax);

        /// <summary>
        /// Sets all values to the specified value.
        /// </summary>
        /// <param name="value">Value to set all to</param>
        public void AwakeningSetAllTo(byte value) => pk.AV_HP = pk.AV_ATK = pk.AV_DEF = pk.AV_SPE = pk.AV_SPA = pk.AV_SPD = value;

        /// <summary>
        /// Sets all values to the bare minimum legal value.
        /// </summary>
        public void AwakeningMinimum()
        {
            if (pk is not PB7 pb7)
                return;

            Span<byte> result = stackalloc byte[6];
            SetExpectedMinimumAVs(pb7, result);
            pb7.AwakeningSetVisual(result);
        }

        /// <summary>
        /// Sets all values to the specified value.
        /// </summary>
        /// <param name="min">Minimum value to set</param>
        /// <param name="max">Maximum value to set</param>
        public void AwakeningSetRandom(byte min = 0, int max = AwakeningMax)
        {
            if (pk is not PB7 pb7)
                return;

            Span<byte> result = stackalloc byte[6];
            SetExpectedMinimumAVs(pb7, result);

            var rnd = Util.Rand;
            foreach (ref var av in result)
            {
                var realMin = Math.Max(min, av);
                var realMax = Math.Max(av, max);
                av = (byte)rnd.Next(realMin, realMax + 1);
            }
            pb7.AwakeningSetVisual(result);
        }

        /// <summary>
        /// Gets the awakening values according to their displayed order.
        /// </summary>
        /// <param name="value">Value storage result</param>
        public void AwakeningGetVisual(Span<byte> value)
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
        /// <param name="value">Value storage to set from</param>
        public void AwakeningSetVisual(ReadOnlySpan<byte> value)
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
        public bool AwakeningAllValid()
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
        /// <param name="index">Index to set to</param>
        /// <param name="value">Value to set</param>
        public byte SetAV(int index, byte value) => index switch
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
        /// <param name="index">Index to get</param>
        public byte GetAV(int index) => index switch
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
        public void GetAVs(Span<byte> value)
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
        /// <param name="pk1">Retriever for IVs</param>
        public void SetSuggestedAwakenedValues(PKM pk1)
        {
            Span<byte> result = stackalloc byte[6];
            SetExpectedMinimumAVs((PB7)pk, result);
            pk.AV_HP  = AwakeningMax;
            pk.AV_ATK = pk1.IV_ATK == 0 ? result[1] : AwakeningMax;
            pk.AV_DEF = AwakeningMax;
            pk.AV_SPA = AwakeningMax;
            pk.AV_SPD = AwakeningMax;
            pk.AV_SPE = pk1.IV_SPE == 0 ? result[5] : AwakeningMax;
        }

        public bool IsAwakeningBelow(IAwakened initial) => !pk.IsAwakeningAboveOrEqual(initial);

        /// <summary>
        /// Checks if the <see cref="pk"/> has values greater or equal to the <see cref="initial"/>.
        /// </summary>
        public bool IsAwakeningAboveOrEqual(IAwakened initial)
        {
            if (pk.AV_HP < initial.AV_HP)
                return false;
            if (pk.AV_ATK < initial.AV_ATK)
                return false;
            if (pk.AV_DEF < initial.AV_DEF)
                return false;
            if (pk.AV_SPA < initial.AV_SPA)
                return false;
            if (pk.AV_SPD < initial.AV_SPD)
                return false;
            if (pk.AV_SPE < initial.AV_SPE)
                return false;
            return true;
        }
    }

    /// <summary>
    /// Updates the <see cref="result"/> span with the expected minimum values for each <see cref="IAwakened"/> index.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <param name="result">Stat results</param>
    public static void SetExpectedMinimumAVs(PB7 pk, Span<byte> result)
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
