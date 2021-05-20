using System;

namespace PKHeX.Core
{
    public sealed class InventoryPouch3GC : InventoryPouch
    {
        public InventoryPouch3GC(InventoryType type, ushort[] legal, int maxCount, int offset, int size)
            : base(type, legal, maxCount, offset, size)
        {
        }

        public override void GetPouch(byte[] data)
        {
            var items = new InventoryItem[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new InventoryItem
                {
                    Index = BigEndian.ToUInt16(data, Offset + (i * 4)),
                    Count = BigEndian.ToUInt16(data, Offset + (i * 4) + 2)
                };
            }
            Items = items;
        }

        public override void SetPouch(byte[] data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            for (int i = 0; i < Items.Length; i++)
            {
                BigEndian.GetBytes((ushort)Items[i].Index).CopyTo(data, Offset + (i * 4));
                BigEndian.GetBytes((ushort)Items[i].Count).CopyTo(data, Offset + (i * 4) + 2);
            }
        }
    }
}