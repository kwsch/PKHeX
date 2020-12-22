using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Inventory Pouch used by <see cref="GameVersion.GG"/>
    /// </summary>
    public sealed class InventoryPouch7b : InventoryPouch
    {
        public InventoryPouch7b(InventoryType type, ushort[] legal, int maxCount, int offset, int size)
            : base(type, legal, maxCount, offset, size)
        {
        }

        public bool SetNew { get; set; }
        private InventoryItem[] OriginalItems = Array.Empty<InventoryItem>();

        public override void GetPouch(byte[] Data)
        {
            var items = new InventoryItem[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                uint val = BitConverter.ToUInt32(Data, Offset + (i * 4));
                items[i] = GetItem(val);
            }
            Items = items;
            OriginalItems = Items.Select(i => i.Clone()).ToArray();
        }

        public override void SetPouch(byte[] Data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            for (int i = 0; i < Items.Length; i++)
            {
                uint val = SetItem(Items[i]);
                BitConverter.GetBytes(val).CopyTo(Data, Offset + (i * 4));
            }
        }

        private static InventoryItem GetItem(uint val)
        {
            // 15bit itemID
            // 15bit count
            // 1 bit new flag
            // 1 bit reserved
            return new()
            {
                Index = (int)(val & 0x7FF),
                Count = (int)(val >> 15 & 0x3FF), // clamp to sane values
                New = (val & 0x40000000) != 0, // 30th bit is "NEW"
            };
        }

        private uint SetItem(InventoryItem item)
        {
            // 15bit itemID
            // 15bit count
            // 1 bit new flag
            // 1 bit reserved
            uint val = 0;
            val |= (uint)(item.Index & 0x7FF);
            val |= (uint)(item.Count & 0x3FF) << 15; // clamped to sane limit
            if (SetNew)
                item.New |= OriginalItems.All(z => z.Index != item.Index);
            if (item.New)
                val |= 0x40000000;
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

        public static int GetSuggestedCount(InventoryType t, int item, int requestVal)
        {
            switch (t)
            {
                // mixed regular battle items & mega stones
                case InventoryType.BattleItems when item > 100:
                // mixed regular items & key items
                case InventoryType.Items when Legal.Pouch_Regular_GG_Key.Contains((ushort)item):
                    return Math.Min(1, requestVal);

                default:
                    return requestVal;
            }
        }
    }
}