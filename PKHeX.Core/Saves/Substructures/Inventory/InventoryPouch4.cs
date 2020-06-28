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

        public override void GetPouch(byte[] Data)
        {
            var items = new InventoryItem[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new InventoryItem
                {
                    Index = BitConverter.ToUInt16(Data, Offset + (i * 4)),
                    Count = BitConverter.ToUInt16(Data, Offset + (i * 4) + 2)
                };
            }
            Items = items;
        }

        public override void SetPouch(byte[] Data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            for (int i = 0; i < Items.Length; i++)
            {
                BitConverter.GetBytes((ushort)Items[i].Index).CopyTo(Data, Offset + (i * 4));
                BitConverter.GetBytes((ushort)Items[i].Count).CopyTo(Data, Offset + (i * 4) + 2);
            }
        }
    }
}