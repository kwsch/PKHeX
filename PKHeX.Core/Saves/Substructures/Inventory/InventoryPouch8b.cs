using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core
{
    public sealed class InventoryPouch8b : InventoryPouch
    {
        private const int SIZE_ITEM = 0x10;

        public bool SetNew { get; set; }

        public InventoryPouch8b(InventoryType type, ushort[] legal, int maxCount, int offset)
            : base(type, legal, maxCount, offset) { }

        public override InventoryItem GetEmpty() => new InventoryItem8b { IsNew = true };

        public override void GetPouch(byte[] data)
        {
            var items = new InventoryItem8b[LegalItems.Length];

            int ctr = 0;
            foreach (var index in LegalItems)
            {
                var item = GetItem(data, index);
                if (!item.IsValidSaveSortNumberCount)
                    continue;
                items[ctr++] = item;
            }
            while (ctr != LegalItems.Length)
                items[ctr++] = new InventoryItem8b();

            Items = items;
            SortBy<InventoryItem8b, ushort>(z => !z.IsValidSaveSortNumberCount ? (ushort)0xFFFF : z.SortOrder);
        }

        public InventoryItem8b GetItem(byte[] data, ushort itemID)
        {
            var ofs = GetItemOffset(itemID, Offset);
            return InventoryItem8b.Read(itemID, data, ofs);
        }

        public override void SetPouch(byte[] data)
        {
            HashSet<ushort> processed = new();

            // Write all the item slots still present in the pouch. Keep track of the item IDs processed.
            var items = (InventoryItem8b[])Items;
            for (int i = 0; i < items.Length; i++)
                items[i].SortOrder = (ushort)(i + 1);

            foreach (var item in items)
            {
                var index = (ushort)item.Index;
                if (index == 0)
                    continue;
                var isInLegal = Array.IndexOf(LegalItems, index);
                if (isInLegal == -1)
                {
                    Debug.WriteLine($"Invalid Item ID returned within this pouch: {index}");
                    continue;
                }

                if (SetNew && item.Index != 0)
                {
                    var original = GetItem(data, (ushort)item.Index);
                    item.IsNew |= !original.IsValidSaveSortNumberCount;
                }

                var ofs = GetItemOffset(index, Offset);
                item.Write(data, ofs);

                if (!processed.Contains(index)) // we will allow duplicate item definitions, but they'll overwrite instead of sum/separate.
                    processed.Add(index);
            }

            // For all the items that were not present in the pouch, clear the data for them.
            foreach (var index in LegalItems)
            {
                if (processed.Contains(index))
                    continue;
                ClearItem(data, index);
            }
        }

        public static int GetItemOffset(ushort index, int baseOffset) => baseOffset + (SIZE_ITEM * index);

        public static void ClearItem(byte[] data, int ofs) => Array.Clear(data, ofs, 0x10);
    }
}
