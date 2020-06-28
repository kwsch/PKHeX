using System;

namespace PKHeX.Core
{
    public sealed class InventoryPouch3 : InventoryPouch
    {
        public uint SecurityKey { private get; set; } // = 0 // Gen3 Only

        public InventoryPouch3(InventoryType type, ushort[] legal, int maxCount, int offset, int size)
            : base(type, legal, maxCount, offset, size)
        {
        }

        public override void GetPouch(byte[] Data)
        {
            var items = new InventoryItem[PouchDataSize];
            for (int i = 0; i<items.Length; i++)
            {
                items[i] = new InventoryItem
                {
                    Index = BitConverter.ToUInt16(Data, Offset + (i* 4)),
                    Count = BitConverter.ToUInt16(Data, Offset + (i* 4) + 2) ^ (ushort) SecurityKey
                };
            }
            Items = items;
        }

        public override void SetPouch(byte[] Data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            for (int i = 0; i<Items.Length; i++)
            {
                BitConverter.GetBytes((ushort) Items[i].Index).CopyTo(Data, Offset + (i* 4));
                BitConverter.GetBytes((ushort)((ushort) Items[i].Count ^ (ushort) SecurityKey)).CopyTo(Data, Offset + (i* 4) + 2);
            }
        }
    }
}