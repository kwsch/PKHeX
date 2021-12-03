using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class InventoryItem8 : InventoryItem, IItemFavorite, IItemNew
    {
        public bool IsFavorite { get; set; }
        public bool IsNew { get; set; }

        /// <summary> Creates a copy of the object. </summary>
        public new InventoryItem8 Clone() => (InventoryItem8)MemberwiseClone();

        public static InventoryItem8 GetValue(uint value) => new()
        {
            // 15bit itemID
            // 15bit count
            // 1 bit new flag
            // 1 bit favorite flag
            Index = (int)(value & 0x7FF),
            Count = (int)(value >> 15 & 0x3FF), // clamp to sane values
            IsNew = (value & 0x40000000) != 0, // 30th bit is "NEW"
            IsFavorite = (value & 0x80000000) != 0, // 31th bit is "FAVORITE"
        };

        public uint GetValue(bool setNew, IReadOnlyList<InventoryItem8> original)
        {
            // 15bit itemID
            // 15bit count
            // 1 bit new flag
            // 1 bit favorite flag
            uint val = 0;
            val |= (uint)(Index & 0x7FF);
            val |= (uint)(Count & 0x3FF) << 15; // clamped to sane limit

            bool isNew = IsNew || (setNew && original.All(z => z.Index != Index));
            if (isNew)
                val |= 0x40000000;
            if (IsFavorite)
                val |= 0x80000000;
            return val;
        }

        public override void SetNewDetails(int count)
        {
            base.SetNewDetails(count);
            IsNew = true;
            IsFavorite = false;
        }

        /// <summary>
        /// Item has been matched to a previously existing item. Copy over the misc details.
        /// </summary>
        public override void MergeOverwrite<T>(T other)
        {
            base.MergeOverwrite(other);
            if (other is not InventoryItem8 item)
                return;
            IsNew = item.IsNew;
            IsFavorite = item.IsFavorite;
        }
    }
}
