using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    public static class RNGUtil
    {
        /// <summary>
        /// Generates an IV for each RNG call using the top 5 bits of frame seeds.
        /// </summary>
        /// <param name="rng">RNG to use</param>
        /// <param name="seed">RNG seed</param>
        /// <returns>Array of 6 IVs as <see cref="uint"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint[] GetSequentialIVsUInt32(this LCRNG rng, uint seed)
        {
            uint[] ivs = new uint[6];
            for (int i = 0; i < 6; i++)
            {
                seed = rng.Next(seed);
                ivs[i] = seed >> 27;
            }
            return ivs;
        }

        /// <summary>
        /// Generates an IV for each RNG call using the top 5 bits of frame seeds.
        /// </summary>
        /// <param name="rng">RNG to use</param>
        /// <param name="seed">RNG seed</param>
        /// <returns>Array of 6 IVs as <see cref="int"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int[] GetSequentialIVsInt32(this LCRNG rng, uint seed)
        {
            int[] ivs = new int[6];
            for (int i = 0; i < 6; i++)
            {
                seed = rng.Next(seed);
                ivs[i] = (int)(seed >> 27);
            }
            return ivs;
        }
    }
}
