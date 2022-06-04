using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for modifying the <see cref="PKM.MarkValue"/>.
    /// </summary>
    public static class MarkingApplicator
    {
        /// <summary>
        /// Default <see cref="MarkingMethod"/> when applying markings.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public static Func<PKM, Func<int, int, int>> MarkingMethod { get; set; } = FlagHighLow;

        /// <summary>
        /// Sets the <see cref="PKM.MarkValue"/> to indicate flawless (or near-flawless) <see cref="PKM.IVs"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetMarkings(this PKM pk)
        {
            if (pk.MarkingCount < 6)
                return; // insufficient marking indexes

            var method = MarkingMethod(pk);
            pk.SetMarking(0, method(pk.IV_HP , 0));
            pk.SetMarking(1, method(pk.IV_ATK, 1));
            pk.SetMarking(2, method(pk.IV_DEF, 2));
            pk.SetMarking(3, method(pk.IV_SPA, 3));
            pk.SetMarking(4, method(pk.IV_SPD, 4));
            pk.SetMarking(5, method(pk.IV_SPE, 5));
        }

        /// <summary>
        /// Toggles the marking at a given index.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Marking index to toggle</param>
        /// <returns>Current marking value</returns>
        public static int ToggleMarking(this PKM pk, int index)
        {
            var marking = pk.GetMarking(index);
            var revised = NextMarking(pk.Format, marking);
            pk.SetMarking(index, revised);
            return revised;
        }

        private static int NextMarking(int format, int marking) => format switch
        {
            <= 6 => marking ^ 1, // toggle : 0 (off) | 1 (on)
            _ => (marking + 1) % 3, // cycle 0->1->2->0... : 0 (none) | 1 (blue) | 2 (pink)
        };

        private static Func<int, int, int> FlagHighLow(PKM pk)
        {
            if (pk.Format < 7)
                return GetSimpleMarking;
            return GetComplexMarking;

            static int GetSimpleMarking(int val, int _) => val == 31 ? 1 : 0;
            static int GetComplexMarking(int val, int _) => val switch
            {
                31 or 1 => 1,
                30 or 0 => 2,
                _ => 0,
            };
        }
    }
}
