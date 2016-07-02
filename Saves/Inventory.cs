using System;

namespace PKHeX
{
    public enum InventoryType
    {
        Items,
        KeyItems,
        TMHMs,
        Medicine,
        Berries,
        Balls,
        BattleItems,
        MailItems,
    }
    public class InventoryItem
    {
        public int Index, Count;
    }

    public class InventoryPouch
    {
        public readonly InventoryType Type;
        public readonly ushort[] LegalItems;
        public readonly int MaxCount;
        private readonly int Offset;
        private readonly int PouchDataSize;
        public uint SecurityKey { private get; set; } // = 0 // Gen3 Only
        public InventoryItem[] Items;
        
        public InventoryPouch(InventoryType type, ushort[] legal, int maxcount, int offset, int size = -1)
        {
            Type = type;
            LegalItems = legal;
            MaxCount = maxcount;
            Offset = offset;
            PouchDataSize = size > -1 ? size : legal.Length;
        }

        public void getPouch(ref byte[] Data)
        {
            InventoryItem[] items = new InventoryItem[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new InventoryItem
                {
                    Index = BitConverter.ToUInt16(Data, Offset + i*4),
                    Count = BitConverter.ToUInt16(Data, Offset + i*4 + 2) ^ (ushort)SecurityKey
                };
            }
            Items = items;
        }
        public void setPouch(ref byte[] Data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            for (int i = 0; i < Items.Length; i++)
            {
                BitConverter.GetBytes((ushort)Items[i].Index).CopyTo(Data, Offset + i*4);
                BitConverter.GetBytes((ushort)Items[i].Count ^ (ushort)SecurityKey).CopyTo(Data, Offset + i*4 + 2);
            }
        }
    }
}
