using System;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class InventoryPouch7 : InventoryPouch
    {
        public InventoryPouch7(InventoryType type, ushort[] legal, int maxCount, int offset)
            : base(type, legal, maxCount, offset)
        {
            OriginalItems = Array.Empty<InventoryItem>();
        }

        public bool SetNew { get; set; } = false;
        private InventoryItem[] OriginalItems;

        public override void GetPouch(byte[] data)
        {
            var items = new InventoryItem[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                // 10bit itemID
                // 10bit count
                // 10bit freespace index
                // 1 bit new flag
                // 1 bit reserved
                uint val = BitConverter.ToUInt32(data, Offset + (i * 4));
                items[i] = new InventoryItem
                {
                    Index = (int)(val & 0x3FF),
                    Count = (int)(val >> 10 & 0x3FF),
                    New = (val & 0x40000000) != 0, // 30th bit is "NEW"
                    FreeSpace = (val >> 20 & 0x3FF) != 0, // "FREE SPACE" sortIndex
                };
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
                // Build Item Value
                uint val = 0;
                val |= (uint)(Items[i].Index & 0x3FF);
                val |= (uint)(Items[i].Count & 0x3FF) << 10;
                if (SetNew)
                    Items[i].New |= OriginalItems.All(z => z.Index != Items[i].Index);
                if (Items[i].New)
                    val |= 0x40000000;
                if (Items[i].FreeSpace)
                    val |= 0x100000;
                BitConverter.GetBytes(val).CopyTo(data, Offset + (i * 4));
            }
        }
    }
}