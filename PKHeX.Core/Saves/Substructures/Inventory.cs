using System;
using System.Linq;

namespace PKHeX.Core
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
        PCItems,
        FreeSpace,
        ZCrystals,
    }
    public class InventoryItem
    {
        public bool New;
        public bool FreeSpace;
        public int Index, Count;
        public InventoryItem Clone()
        {
            return new InventoryItem {Count = Count, Index = Index, New = New};
        }

        // Check Pouch Compatibility
        public bool Valid(ushort[] LegalItems, bool HaX, int MaxItemID)
        {
            if (Index == 0)
                return true;
            if (Index <= MaxItemID)
                return HaX || LegalItems.Contains((ushort)Index);
            return false;
        }
    }

    public class InventoryPouch
    {
        public readonly InventoryType Type;
        public readonly ushort[] LegalItems;
        public readonly int MaxCount;
        public int Count => Items.Count(it => it.Count > 0);
        public uint SecurityKey { private get; set; } // = 0 // Gen3 Only
        public InventoryItem[] Items;

        private readonly int Offset;
        private readonly int PouchDataSize;
        private InventoryItem[] OriginalItems;
        
        public InventoryPouch(InventoryType type, ushort[] legal, int maxcount, int offset, int size = -1)
        {
            Type = type;
            LegalItems = legal;
            MaxCount = maxcount;
            Offset = offset;
            PouchDataSize = size > -1 ? size : legal.Length;
        }

        public void GetPouch(ref byte[] Data)
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
        public void SetPouch(ref byte[] Data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            for (int i = 0; i < Items.Length; i++)
            {
                BitConverter.GetBytes((ushort)Items[i].Index).CopyTo(Data, Offset + i*4);
                BitConverter.GetBytes((ushort)((ushort)Items[i].Count ^ (ushort)SecurityKey)).CopyTo(Data, Offset + i*4 + 2);
            }
        }
        public void GetPouch7(ref byte[] Data)
        {
            InventoryItem[] items = new InventoryItem[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                // 10bit itemID
                // 10bit count
                // 12bit flags/reserved
                uint val = BitConverter.ToUInt32(Data, Offset + i*4);
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
        public void SetPouch7(ref byte[] Data, bool setNEW = false)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            for (int i = 0; i < Items.Length; i++)
            {
                // Build Item Value
                uint val = 0;
                val |= (uint)(Items[i].Index & 0x3FF);
                val |= (uint)(Items[i].Count & 0x3FF) << 10;
                if (setNEW)
                    Items[i].New |= OriginalItems.All(z => z.Index != Items[i].Index);
                if (Items[i].New)
                    val |= 0x40000000;
                if (Items[i].FreeSpace)
                    val |= 0x100000;
                BitConverter.GetBytes(val).CopyTo(Data, Offset + i * 4);
            }
        }

        public void GetPouchBigEndian(ref byte[] Data)
        {
            InventoryItem[] items = new InventoryItem[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new InventoryItem
                {
                    Index = BigEndian.ToUInt16(Data, Offset + i * 4),
                    Count = BigEndian.ToUInt16(Data, Offset + i * 4 + 2) ^ (ushort)SecurityKey
                };
            }
            Items = items;
        }
        public void SetPouchBigEndian(ref byte[] Data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            for (int i = 0; i < Items.Length; i++)
            {
                BigEndian.GetBytes((ushort)Items[i].Index).CopyTo(Data, Offset + i * 4);
                BigEndian.GetBytes((ushort)((ushort)Items[i].Count ^ (ushort)SecurityKey)).CopyTo(Data, Offset + i * 4 + 2);
            }
        }

        public void GetPouchG1(ref byte[] Data)
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
                if (numStored > PouchDataSize) // uninitialized yellow (0xFF), sanity check for out-of-bounds values
                    numStored = 0;
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
        public void SetPouchG1(ref byte[] Data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");
            switch (Type)
            {
                case InventoryType.TMHMs:
                    foreach (InventoryItem t in Items)
                    {
                        if (!LegalItems.Any(it => it == t.Index))
                            continue;
                        int index = Offset + Array.FindIndex(LegalItems, it => t.Index == it);
                        Data[index] = (byte)t.Count;
                    }
                    break;
                case InventoryType.KeyItems:
                    for (int i = 0; i < Items.Length; i++)
                    {
                        Data[Offset + i + 1] = (byte)Items[i].Index;
                    }
                    Data[Offset] = (byte)Count;
                    Data[Offset + 1 + Count] = 0xFF;
                    break;
                default:
                    for (int i = 0; i < Items.Length; i++)
                    {
                        Data[Offset + i * 2 + 1] = (byte)Items[i].Index;
                        Data[Offset + i * 2 + 2] = (byte)Items[i].Count;
                    }
                    Data[Offset] = (byte)Count;
                    Data[Offset + 1 + 2 * Count] = 0xFF;
                    break;
            }
        }

        public void SortByCount(bool reverse = false)
        {
            if (reverse)
                Items = Items.Where(item => item.Index != 0).OrderBy(item => item.Count)
                        .Concat(Items.Where(item => item.Index == 0)).ToArray();
            else
                Items = Items.Where(item => item.Index != 0).OrderByDescending(item => item.Count)
                        .Concat(Items.Where(item => item.Index == 0)).ToArray();
        }
        public void SortByIndex(bool reverse = false)
        {
            if (reverse)
                Items = Items.Where(item => item.Index != 0).OrderByDescending(item => item.Index)
                    .Concat(Items.Where(item => item.Index == 0)).ToArray();
            else
                Items = Items.Where(item => item.Index != 0).OrderBy(item => item.Index)
                    .Concat(Items.Where(item => item.Index == 0)).ToArray();
        }
        public void SortByName(string[] names, bool reverse = false)
        {
            if (reverse)
                Items = Items.Where(item => item.Index != 0 && item.Index < names.Length).OrderByDescending(item => names[item.Index])
                        .Concat(Items.Where(item => item.Index == 0 || item.Index >= names.Length)).ToArray();
            else
                Items = Items.Where(item => item.Index != 0).OrderBy(item => names[item.Index])
                        .Concat(Items.Where(item => item.Index == 0 || item.Index >= names.Length)).ToArray();
        }

        public void Sanitize(bool HaX, int MaxItemID)
        {
            var x = Items.Where(item => item.Valid(LegalItems, HaX, MaxItemID)).ToArray();
            Items = x.Concat(new byte[PouchDataSize - x.Length].Select(i => new InventoryItem())).ToArray();
        }
    }
}
