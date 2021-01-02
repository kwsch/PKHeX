using System;
using System.Collections.Generic;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    /// <summary>
    /// Information about Held Item Restrictions
    /// </summary>
    public static class ItemRestrictions
    {
        /// <summary>
        /// Checks if a <see cref="PKM.HeldItem"/> is available to be held in the current <see cref="PKM.Format"/>.
        /// </summary>
        /// <param name="pk">Entity data</param>
        /// <returns>True if able to be held, false if not</returns>
        public static bool IsHeldItemAllowed(PKM pk)
        {
            if (pk is PB7)
                return pk.HeldItem == 0;
            return IsHeldItemAllowed(pk.HeldItem, pk.Format);
        }

        /// <summary>
        /// Checks if an <see cref="item"/> is available to be held in <see cref="generation"/>.
        /// </summary>
        /// <param name="item">Held Item ID</param>
        /// <param name="generation">Generation Number</param>
        /// <returns>True if able to be held, false if not</returns>
        public static bool IsHeldItemAllowed(int item, int generation)
        {
            if (item == 0)
                return true;
            var items = GetReleasedHeldItems(generation);
            return (uint)item < items.Count && items[item];
        }

        private static IReadOnlyList<bool> GetReleasedHeldItems(int generation) => generation switch
        {
            2 => ReleasedHeldItems_2,
            3 => ReleasedHeldItems_3,
            4 => ReleasedHeldItems_4,
            5 => ReleasedHeldItems_5,
            6 => ReleasedHeldItems_6,
            7 => ReleasedHeldItems_7,
            8 => ReleasedHeldItems_8,
            _ => Array.Empty<bool>()
        };
    }
}
