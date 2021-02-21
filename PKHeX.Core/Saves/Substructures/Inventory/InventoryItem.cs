using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class InventoryItem
    {
        /// <summary> Item ID </summary>
        public int Index;
        /// <summary> Quantity </summary>
        public int Count;

        /// <summary> Indicates if the item is "NEW"ly obtained and not yet viewed. </summary>
        public bool New;

        /// <summary> Indicates if the item should be shown in the Free Space pouch instead (Generation 5). </summary>
        public bool FreeSpace;

        /// <summary> Creates a copy of the object. </summary>
        public InventoryItem Clone() => (InventoryItem) MemberwiseClone();

        /// <summary>
        /// Checks if the item is compatible with a pouch.
        /// </summary>
        /// <param name="legal">Legal Item IDs for the pouch</param>
        /// <param name="maxItemID">Max item ID that exists in the game</param>
        /// <param name="HaX">Bend the rules for cheaters?</param>
        public bool IsValid(IList<ushort> legal, int maxItemID, bool HaX = false)
        {
            if (Index == 0)
                return true;
            if ((uint) Index > maxItemID)
                return false;
            return HaX || legal.Contains((ushort)Index);
        }

        /// <summary>
        /// Resets all data in the object back to zero.
        /// </summary>
        public void Clear()
        {
            Index = Count = 0;
            New = FreeSpace = false;
        }
    }
}
