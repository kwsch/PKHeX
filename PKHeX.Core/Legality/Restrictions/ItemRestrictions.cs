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
            if (pk is PB7 pb7) // no held items in game
                return pb7.HeldItem == 0;
            return IsHeldItemAllowed(pk.HeldItem, pk.Format, pk);
        }

        /// <summary>
        /// Checks if an <see cref="item"/> is available to be held in <see cref="generation"/>.
        /// </summary>
        /// <param name="item">Held Item ID</param>
        /// <param name="generation">Generation Number</param>
        /// <returns>True if able to be held, false if not</returns>
        public static bool IsHeldItemAllowed(int item, int generation, PKM pk)
        {
            if (item == 0)
                return true;
            var items = GetReleasedHeldItems(generation, pk);
            return (uint)item < items.Count && items[item];
        }

        private static IReadOnlyList<bool> GetReleasedHeldItems(int generation, PKM pk) => generation switch
        {
            2 => ReleasedHeldItems_2,
            3 => ReleasedHeldItems_3,
            4 => ReleasedHeldItems_4,
            5 => ReleasedHeldItems_5,
            6 => ReleasedHeldItems_6,
            7 => ReleasedHeldItems_7,
            8 when pk is PB8 => ReleasedHeldItems_8b,
            8 => ReleasedHeldItems_8,
            _ => Array.Empty<bool>(),
        };
    }
}
