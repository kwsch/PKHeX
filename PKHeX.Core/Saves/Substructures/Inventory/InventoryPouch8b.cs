using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class InventoryPouch8b : InventoryPouch
    {
        private const int SIZE_ITEM = 0x10;

        private InventoryItem[] OriginalItems = Array.Empty<InventoryItem>();
        public bool SetNew { get; set; } = false;

        public InventoryPouch8b(InventoryType type, ushort[] legal, int maxCount, int offset) : base(type, legal, maxCount, offset) { }

        public override void GetPouch(byte[] data)
        {
            Items = new InventoryItem[LegalItems.Length];
            int ctr = 0;
            foreach (var index in LegalItems)
            {
                var ofs = GetItemOffset(index, Offset);
                var item = ReadItem(index, data, ofs);
                if (item.Count == 0)
                    continue;
                Items[ctr++] = item;
            }

            while (ctr != LegalItems.Length)
                Items[ctr++] = new InventoryItem();
            OriginalItems = Items.Select(i => i.Clone()).ToArray();
        }

        public override void SetPouch(byte[] data)
        {
            HashSet<ushort> processed = new();

            // Write all the item slots still present in the pouch. Keep track of the item IDs processed.
            foreach (var item in Items)
            {
                var index = (ushort)item.Index;
                var isInLegal = Array.IndexOf(LegalItems, index);
                if (isInLegal == -1)
                {
                    Debug.WriteLine($"Invalid Item ID returned within this pouch: {index}");
                    continue;
                }

                if (SetNew && item.Index != 0)
                    item.New |= OriginalItems.All(z => z.Index != item.Index);

                var ofs = GetItemOffset(index, Offset);
                WriteItem(item, data, ofs);

                if (!processed.Contains(index)) // we will allow duplicate item definitions, but they'll overwrite instead of sum/separate.
                    processed.Add(index);
            }

            // For all the items that were not present in the pouch, clear the data for them.
            foreach (var index in LegalItems)
            {
                if (processed.Contains(index))
                    continue;
                var ofs = GetItemOffset(index, Offset);
                WriteItem(new InventoryItem(), data, ofs);
            }
        }

        public static int GetItemOffset(ushort index, int baseOffset) => baseOffset + (SIZE_ITEM * index);

        public static InventoryItem ReadItem(ushort index, byte[] data, int ofs)
        {
            var count = BitConverter.ToInt32(data, ofs);
            bool isNew = BitConverter.ToInt32(data, ofs + 4) == 0;
            bool isFavorite = BitConverter.ToInt32(data, ofs + 0x8) == 1;
            // ushort sortOrder = BitConverter.ToUInt16(data, ofs + 0xE);
            return new InventoryItem { Index = index, Count = count, New = isNew, FreeSpace = isFavorite };
        }

        public static void WriteItem(InventoryItem item, byte[] data, int ofs)
        {
            BitConverter.GetBytes((uint)item.Count).CopyTo(data, ofs);
            BitConverter.GetBytes(item.New ? 0u : 1u).CopyTo(data, ofs + 4);
            BitConverter.GetBytes(item.FreeSpace ? 1u : 0u).CopyTo(data, ofs + 8);
            if (item.Count == 0)
                BitConverter.GetBytes((ushort)0xFFFF).CopyTo(data, ofs + 0xE);
        }
    }
}
