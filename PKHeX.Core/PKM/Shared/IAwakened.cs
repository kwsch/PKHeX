using System;

namespace PKHeX.Core
{
    public interface IAwakened
    {
        int AV_HP { get; set; }
        int AV_ATK { get; set; }
        int AV_DEF { get; set; }
        int AV_SPE { get; set; }
        int AV_SPA { get; set; }
        int AV_SPD { get; set; }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Sums all values.
        /// </summary>
        /// <param name="pk">Data to sum with</param>
        public static int AwakeningSum(this IAwakened pk) => pk.AV_HP + pk.AV_ATK + pk.AV_DEF + pk.AV_SPE + pk.AV_SPA + pk.AV_SPD;

        /// <summary>
        /// Clears all values.
        /// </summary>
        /// <param name="pk">Data to clear from</param>
        public static void AwakeningClear(this IAwakened pk) => pk.AV_HP = pk.AV_ATK = pk.AV_DEF = pk.AV_SPE = pk.AV_SPA = pk.AV_SPD = 0;

        /// <summary>
        /// Sets all values to the maximum value.
        /// </summary>
        /// <param name="pk">Data to clear from</param>
        public static void AwakeningMax(this IAwakened pk) => pk.AwakeningSetAllTo(Legal.AwakeningMax);

        /// <summary>
        /// Sets all values to the specified value.
        /// </summary>
        /// <param name="pk">Data to clear from</param>
        /// <param name="value">Value to set all to</param>
        public static void AwakeningSetAllTo(this IAwakened pk, int value) => pk.AV_HP = pk.AV_ATK = pk.AV_DEF = pk.AV_SPE = pk.AV_SPA = pk.AV_SPD = value;

        /// <summary>
        /// Gets if all values are within legal limits.
        /// </summary>
        /// <param name="pk">Data to check</param>
        public static bool AwakeningAllValid(this IAwakened pk)
        {
            if (pk.AV_HP > Legal.AwakeningMax)
                return false;
            if (pk.AV_ATK > Legal.AwakeningMax)
                return false;
            if (pk.AV_DEF > Legal.AwakeningMax)
                return false;
            if (pk.AV_SPE > Legal.AwakeningMax)
                return false;
            if (pk.AV_SPA > Legal.AwakeningMax)
                return false;
            if (pk.AV_SPD > Legal.AwakeningMax)
                return false;
            return true;
        }

        /// <summary>
        /// Sets one of the <see cref="IAwakened"/> values based on its index within the array.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Index to set to</param>
        /// <param name="value">Value to set</param>
        public static void SetAV(this IAwakened pk, int index, int value)
        {
            switch (index)
            {
                case 0: pk.AV_HP = value; break;
                case 1: pk.AV_ATK = value; break;
                case 2: pk.AV_DEF = value; break;
                case 3: pk.AV_SPE = value; break;
                case 4: pk.AV_SPA = value; break;
                case 5: pk.AV_SPD = value; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// Sets one of the <see cref="IAwakened"/> values based on its index within the array.
        /// </summary>
        /// <param name="pk">Pokémon to check.</param>
        /// <param name="index">Index to get</param>
        public static int GetAV(this IAwakened pk, int index)
        {
            return index switch
            {
                0 => pk.AV_HP,
                1 => pk.AV_ATK,
                2 => pk.AV_DEF,
                3 => pk.AV_SPE,
                4 => pk.AV_SPA,
                5 => pk.AV_SPD,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }

        /// <summary>
        /// Sets the values based on the current <see cref="PKM.IVs"/>.
        /// </summary>
        /// <param name="a">Accessor for setting the values</param>
        /// <param name="pk">Retriever for IVs</param>
        public static void SetSuggestedAwakenedValues(this IAwakened a, PKM pk)
        {
            a.AV_HP  = Legal.AwakeningMax;
            a.AV_ATK = pk.IV_ATK == 0 ? 0 : Legal.AwakeningMax;
            a.AV_DEF = Legal.AwakeningMax;
            a.AV_SPE = pk.IV_SPE == 0 ? 0 : Legal.AwakeningMax;
            a.AV_SPA = Legal.AwakeningMax;
            a.AV_SPD = Legal.AwakeningMax;
        }

        public static bool IsAwakeningBelow(this IAwakened current, IAwakened initial) => !current.IsAwakeningAboveOrEqual(initial);

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
    }
}