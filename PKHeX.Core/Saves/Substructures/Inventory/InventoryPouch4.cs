using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Inventory Pouch with 4 bytes per item (u16 ID, u16 count)
    /// </summary>
    public sealed class InventoryPouch4 : InventoryPouch
    {
        public InventoryPouch4(InventoryType type, ushort[] legal, int maxCount, int offset)
            : base(type, legal, maxCount, offset)
        {
        }

        // size: 32bit
        // u16 id
        // u16 count

        public override void GetPouch(byte[] data)
        {
            var items = new InventoryItem[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new InventoryItem
                {
                    Index = BitConverter.ToUInt16(data, Offset + (i * 4)),
                    Count = BitConverter.ToUInt16(data, Offset + (i * 4) + 2)
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
                BitConverter.GetBytes((ushort)Items[i].Index).CopyTo(data, Offset + (i * 4));
                BitConverter.GetBytes((ushort)Items[i].Count).CopyTo(data, Offset + (i * 4) + 2);
            }
        }
    }
}