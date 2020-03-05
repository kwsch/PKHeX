using System;
using System.Linq;

namespace PKHeX.Core
{
    public static class MarkingApplicator
    {
        /// <summary>
        /// Default <see cref="MarkingMethod"/> when applying markings.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public static Func<PKM, Func<int, int, int>> MarkingMethod { get; set; } = FlagHighLow;

        /// <summary>
        /// Sets the <see cref="PKM.Markings"/> to indicate flawless (or near-flawless) <see cref="PKM.IVs"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="ivs"><see cref="PKM.IVs"/> to use (if already known). Will fetch the current <see cref="PKM.IVs"/> if not provided.</param>
        public static void SetMarkings(this PKM pk, int[] ivs)
        {
            if (pk.Format <= 3)
                return; // no markings (gen3 only has 4; can't mark stats intelligently

            var markings = ivs.Select(MarkingMethod(pk)).ToArray();
            pk.Markings = PKX.ReorderSpeedLast(markings);
        }

        /// <summary>
        /// Sets the <see cref="PKM.Markings"/> to indicate flawless (or near-flawless) <see cref="PKM.IVs"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetMarkings(this PKM pk)
        {
            if (pk.Format <= 3)
                return; // no markings (gen3 only has 4; can't mark stats intelligently

            pk.SetMarkings(pk.IVs);
        }

        /// <summary>
        /// Toggles the marking at a given index.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Marking index to toggle</param>
        /// <returns>Current marking values</returns>
        public static int[] ToggleMarking(this PKM pk, int index) => pk.ToggleMarking(index, pk.Markings);

        /// <summary>
        /// Toggles the marking at a given index.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Marking index to toggle</param>
        /// <param name="markings">Current marking values (optional)</param>
        /// <returns>Current marking values</returns>
        public static int[] ToggleMarking(this PKM pk, int index, int[] markings)
        {
            switch (pk.Format)
            {
                case 3:
                case 4:
                case 5:
                case 6: // on/off
                    markings[index] ^= 1; // toggle
                    pk.Markings = markings;
                    break;
                case 7: // 0 (none) | 1 (blue) | 2 (pink)
                case 8:
                    markings[index] = (markings[index] + 1) % 3; // cycle 0->1->2->0...
                    pk.Markings = markings;
                    break;
            }
            return markings;
        }

        private static Func<int, int, int> FlagHighLow(PKM pk)
        {
            if (pk.Format < 7)
                return GetSimpleMarking;
            return GetComplexMarking;

            static int GetSimpleMarking(int val, int _) => val == 31 ? 1 : 0;
            static int GetComplexMarking(int val, int _)
            {
                if (val == 31 || val == 1)
                    return 1;
                if (val == 30 || val == 0)
                    return 2;
                return 0;
            }
        }
    }
}