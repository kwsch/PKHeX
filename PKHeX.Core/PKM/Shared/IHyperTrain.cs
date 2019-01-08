using System;

namespace PKHeX.Core
{
    public interface IHyperTrain
    {
        int HyperTrainFlags { get; set; }
        bool HT_HP { get; set; }
        bool HT_ATK { get; set; }
        bool HT_DEF { get; set; }
        bool HT_SPA { get; set; }
        bool HT_SPD { get; set; }
        bool HT_SPE { get; set; }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Toggles the Hyper Training flag for a given stat.
        /// </summary>
        /// <param name="t">Hyper Trainable object</param>
        /// <param name="index">Battle Stat (H/A/B/S/C/D)</param>
        /// <returns>Final Hyper Training Flag value</returns>
        public static bool HyperTrainInvert(this IHyperTrain t, int index)
        {
            switch (index)
            {
                case 0: return t.HT_HP ^= true;
                case 1: return t.HT_ATK ^= true;
                case 2: return t.HT_DEF ^= true;
                case 3: return t.HT_SPE ^= true;
                case 4: return t.HT_SPA ^= true;
                case 5: return t.HT_SPD ^= true;
            }
            return false;
        }

        public static bool IsHyperTrainedAll(this IHyperTrain t) => t.HyperTrainFlags == 0x3F;
        public static void HyperTrainClear(this IHyperTrain t) => t.HyperTrainFlags = 0;
        public static bool IsHyperTrained(this IHyperTrain t) => t.HyperTrainFlags != 0;

        public static bool IsHyperTrained(this IHyperTrain t, int index)
        {
            switch (index)
            {
                case 0: return t.HT_HP;
                case 1: return t.HT_ATK;
                case 2: return t.HT_DEF;
                case 3: return t.HT_SPE;
                case 4: return t.HT_SPA;
                case 5: return t.HT_SPD;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// Gets one of the <see cref="IHyperTrain"/> values based on its index within the array.
        /// </summary>
        /// <param name="pk">Pokémon to check.</param>
        /// <param name="index">Index to get</param>
        public static bool GetHT(this IHyperTrain pk, int index)
        {
            switch (index)
            {
                case 0: return pk.HT_HP;
                case 1: return pk.HT_ATK;
                case 2: return pk.HT_DEF;
                case 3: return pk.HT_SPE;
                case 4: return pk.HT_SPA;
                case 5: return pk.HT_SPD;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// Sets <see cref="IHyperTrain.HyperTrainFlags"/> to valid values which may best enhance the <see cref="PKM"/> stats.
        /// </summary>
        /// <param name="pkm"></param>
        /// <param name="IVs"><see cref="PKM.IVs"/> to use (if already known). Will fetch the current <see cref="PKM.IVs"/> if not provided.</param>
        public static void SetSuggestedHyperTrainingData(this PKM pkm, int[] IVs = null)
        {
            if (!(pkm is IHyperTrain t))
                return;
            if (pkm.CurrentLevel < 100)
            {
                t.HyperTrainFlags = 0;
                return;
            }
            if (IVs == null)
                IVs = pkm.IVs;

            t.HT_HP = IVs[0] != 31;
            t.HT_ATK = IVs[1] != 31 && IVs[1] > 2;
            t.HT_DEF = IVs[2] != 31;
            t.HT_SPE = IVs[3] != 31 && IVs[3] > 2;
            t.HT_SPA = IVs[4] != 31;
            t.HT_SPD = IVs[5] != 31;

            if (pkm is PB7 pb)
                pb.ResetCP();
        }
    }
}
