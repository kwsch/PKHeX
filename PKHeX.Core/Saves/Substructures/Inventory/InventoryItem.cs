using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public class InventoryItem
    {
        /// <summary> Item ID </summary>
        public int Index { get; set; }
        /// <summary> Quantity </summary>
        public int Count { get; set; }

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
        public virtual void Clear() => Index = Count = 0;

        /// <summary>
        /// Item has been detected as a newly given item. Set the appropriate new details.
        /// </summary>
        /// <param name="count">Final count of items.</param>
        public virtual void SetNewDetails(int count) => Count = count;

        /// <summary>
        /// Item has been matched to a previously existing item. Copy over the misc details.
        /// </summary>
        public virtual void MergeOverwrite<T>(T other) where T : InventoryItem => Count = Math.Max(Count, other.Count);
    }

    public interface IItemNew
    {
        /// <summary> Indicates if the item is "NEW"ly obtained and not yet viewed. </summary>
        bool IsNew { get; set; }
    }

    public interface IItemFavorite
    {
        /// <summary> Indicates if the item should be indicated as a favorite item. </summary>
        bool IsFavorite { get; set; }
    }
    public interface IItemFreeSpaceIndex
    {
        /// <summary> Indicates if the item should be shown in the Free Space pouch instead (Generation 7). </summary>
        uint FreeSpaceIndex { get; set; }
    }

    public interface IItemFreeSpace
    {
        /// <summary> Indicates if the item should be shown in the Free Space pouch instead (Generation 5). </summary>
        bool IsFreeSpace { get; set; }
    }
}
