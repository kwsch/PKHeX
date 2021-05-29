using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Inventory Pouch used by <see cref="GameVersion.SWSH"/>
    /// </summary>
    public sealed class InventoryPouch8 : InventoryPouch
    {
        public InventoryPouch8(InventoryType type, ushort[] legal, int maxCount, int offset, int size)
            : base(type, legal, maxCount, offset, size)
        {
        }

        public bool SetNew { get; set; }
        private InventoryItem[] OriginalItems = Array.Empty<InventoryItem>();

        public override void GetPouch(byte[] data)
        {
            var items = new InventoryItem[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                uint val = BitConverter.ToUInt32(data, Offset + (i * 4));
                items[i] = GetItem(val);
            }
            Items = items;
            OriginalItems = Items.Select(i => i.Clone()).ToArray();
        }

        public override void SetPouch(byte[] data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            for (int i = 0; i < Items.Length; i++)
            {
                uint val = SetItem(Items[i]);
                BitConverter.GetBytes(val).CopyTo(data, Offset + (i * 4));
            }
        }

        private static InventoryItem GetItem(uint val)
        {
            // 15bit itemID
            // 15bit count
            // 1 bit new flag
            // 1 bit favorite flag
            return new()
            {
                Index = (int)(val & 0x7FF),
                Count = (int)(val >> 15 & 0x3FF), // clamp to sane values
                New = (val & 0x40000000) != 0, // 30th bit is "NEW"
                FreeSpace = (val & 0x80000000) != 0, // 31th bit is "FAVORITE"
            };
        }

        private uint SetItem(InventoryItem item)
        {
            // 15bit itemID
            // 15bit count
            // 1 bit new flag
            // 1 bit favorite flag
            uint val = 0;
            val |= (uint)(item.Index & 0x7FF);
            val |= (uint)(item.Count & 0x3FF) << 15; // clamped to sane limit
            if (SetNew)
                item.New |= OriginalItems.All(z => z.Index != item.Index);
            if (item.New)
                val |= 0x40000000;
            if (item.FreeSpace)
                val |= 0x80000000;
            return val;
        }

        /// <summary>
        /// Checks pouch contents for bad count values.
        /// </summary>
        /// <remarks>
        /// Certain pouches contain a mix of count-limited items and uncapped regular items.
        /// </remarks>
        internal void SanitizeCounts()
        {
            foreach (var item in Items)
                item.Count = GetSuggestedCount(Type, item.Index, item.Count);
        }

        public static int GetSuggestedCount(InventoryType t, int item, int requestVal) => t switch
        {
            // TMs are clamped to 1, let TRs be whatever
            InventoryType.TMHMs => item is >= 1130 and <= 1229 ? requestVal : 1,
            _ => requestVal
        };
    }
}
