using System;
using System.Linq;

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
        public int Count => Items.Count(it => it.Count > 0);
        public readonly int Offset;
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
                BitConverter.GetBytes((ushort)((ushort)Items[i].Count ^ (ushort)SecurityKey)).CopyTo(Data, Offset + i*4 + 2);
            }
        }

        public void getPouchG1(ref byte[] Data)
        {
            InventoryItem[] items = new InventoryItem[PouchDataSize];
            if (Type == InventoryType.TMHMs)
            {
                int slot = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (Data[Offset + i] != 0)
                        items[slot++] = new InventoryItem
                        {
                            Index = LegalItems[i],
                            Count = Data[Offset+i]
                        };
                }
                while (slot < items.Length)
                    items[slot++] = new InventoryItem
                    {
                        Index = 0,
                        Count = 0
                    };
            }
            else
            {
                int numStored = Data[Offset];
                for (int i = 0; i < numStored; i++)
                {
                    switch (Type)
                    {
                        case InventoryType.KeyItems:
                            items[i] = new InventoryItem
                            {
                                Index = Data[Offset + i + 1],
                                Count = 1
                            };
                            break;
                        default:
                            items[i] = new InventoryItem
                            {
                                Index = Data[Offset + i * 2 + 1],
                                Count = Data[Offset + i * 2 + 2]
                            };
                            break;
                    }
                }
                for (int i = numStored; i < items.Length; i++)
                {
                    items[i] = new InventoryItem
                    {
                        Index = 0,
                        Count = 0
                    };
                }
            }
            Items = items;

        }

        public void setPouchG1(ref byte[] Data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");
            if (Type == InventoryType.TMHMs)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    if (LegalItems.Any(it => it == Items[i].Index))
                        Data[Offset + Array.FindIndex(LegalItems, it => Items[i].Index == it)] = (byte) Items[i].Count;
                }
            }
            else if (Type == InventoryType.KeyItems)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    Data[Offset + i + 1] = (byte)Items[i].Index;
                }
                Data[Offset] = (byte)Count;
                Data[Offset + 1 + Count] = 0xFF;
            }
            else
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    Data[Offset + i * 2 + 1] = (byte)Items[i].Index;
                    Data[Offset + i * 2 + 2] = (byte)Items[i].Count;
                }
                Data[Offset] = (byte)Count;
                Data[Offset + 1 + 2 * Count] = 0xFF;
            }
        }
    }
}
