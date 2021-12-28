using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class InventoryPouch3GC : InventoryPouch
    {
        public InventoryPouch3GC(InventoryType type, ushort[] legal, int maxCount, int offset, int size)
            : base(type, legal, maxCount, offset, size)
        {
        }

        public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };

        public override void GetPouch(byte[] data)
        {
            var items = new InventoryItem[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                ReadOnlySpan<byte> item = data.AsSpan(Offset + (i * 4));
                items[i] = new InventoryItem
                {
                    Index = ReadUInt16BigEndian(item),
                    Count = ReadUInt16BigEndian(item[2..]),
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
                var item = data.AsSpan(Offset + (i * 4));
                WriteUInt32BigEndian(item,      (ushort)Items[i].Index);
                WriteUInt32BigEndian(item[2..], (ushort)Items[i].Count);
            }
        }
    }
}