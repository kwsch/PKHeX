using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public abstract class InventoryPouch
    {
        public readonly InventoryType Type;
        public readonly ushort[] LegalItems;
        public readonly int MaxCount;
        public int Count => Items.Count(it => it.Count > 0);
        public bool IsCramped => LegalItems.Length > Items.Length;

        public InventoryItem[] Items;

        protected readonly int Offset;
        protected readonly int PouchDataSize;

        protected InventoryPouch(InventoryType type, ushort[] legal, int maxcount, int offset, int size = -1)
        {
            Items = Array.Empty<InventoryItem>();
            Type = type;
            LegalItems = legal;
            MaxCount = maxcount;
            Offset = offset;
            PouchDataSize = size > -1 ? size : legal.Length;
        }

        public abstract void GetPouch(byte[] Data);
        public abstract void SetPouch(byte[] Data);

        public void SortByCount(bool reverse = false) => Array.Sort(Items, (x, y) => Compare(x.Count, y.Count, reverse));
        public void SortByIndex(bool reverse = false) => Array.Sort(Items, (x, y) => Compare(x.Index, y.Index, reverse));
        public void SortByName(string[] names, bool reverse = false) => Array.Sort(Items, (x, y) => Compare(x.Index, y.Index, names, reverse));
        public void SortByEmpty() => Array.Sort(Items, (x, y) => (x.Count == 0).CompareTo(y.Count == 0));

        private static int Compare<T>(int i1, int i2, IReadOnlyList<T> n, bool rev) where T : IComparable
        {
            if (i1 == 0 || i1 >= n.Count)
                return 1;
            if (i2 == 0 || i2 >= n.Count)
                return -1;
            return rev
                ? n[i2].CompareTo(n[i1])
                : n[i1].CompareTo(n[i2]);
        }

        private static int Compare(int i1, int i2, bool rev)
        {
            if (i1 == 0)
                return 1;
            if (i2 == 0)
                return -1;
            return rev
                ? i2.CompareTo(i1)
                : i1.CompareTo(i2);
        }

        public void Sanitize(int MaxItemID, bool HaX = false)
        {
            int ctr = 0;
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].Valid(LegalItems, MaxItemID, HaX))
                    Items[ctr++] = Items[i];
            }
            while (ctr < Items.Length)
                Items[ctr++] = new InventoryItem();
        }

        public void ClearCount0()
        {
            int ctr = 0;
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].Count != 0)
                    Items[ctr++] = Items[i];
            }
            while (ctr < Items.Length)
                Items[ctr++] = new InventoryItem();
        }

        public void RemoveAll()
        {
            foreach (var item in Items)
                item.Clear();
        }

        public void RemoveAll(Func<InventoryItem, bool> deleteCriteria)
        {
            foreach (var item in Items.Where(deleteCriteria))
                item.Clear();
        }

        public void RemoveAll(Func<InventoryItem, int, bool> deleteCriteria)
        {
            foreach (var item in Items.Where(deleteCriteria))
                item.Clear();
        }

        public void ModifyAllCount(int value)
        {
            foreach (var item in Items.Where(z => z.Count != 0))
                item.Count = value;
        }

        public void ModifyAllCount(int value, Func<InventoryItem, bool> modifyCriteria)
        {
            foreach (var item in Items.Where(z => z.Count != 0).Where(modifyCriteria))
                item.Count = value;
        }

        public void ModifyAllCount(int value, Func<InventoryItem, int, bool> modifyCriteria)
        {
            foreach (var item in Items.Where(z => z.Count != 0).Where(modifyCriteria))
                item.Count = value;
        }

        public void ModifyAllCount(SaveFile sav, int count = -1)
        {
            if (count <= 0)
                count = 1;
            foreach (var item in Items.Where(z => z.Count != 0))
                item.Count = GetSuggestedItemCount(sav, item.Index, count);
        }

        public void ModifyAllCount(Func<InventoryItem, int> modification)
        {
            foreach (var item in Items.Where(z => z.Count != 0))
                item.Count = modification(item);
        }

        public void GiveAllItems(IReadOnlyList<ushort> newItems, Func<InventoryItem, int> getSuggestedItemCount, int count = -1)
        {
            GiveAllItems(newItems, count);
            ModifyAllCount(getSuggestedItemCount);
        }

        public void GiveAllItems(SaveFile sav, IReadOnlyList<ushort> items, int count = -1)
        {
            GiveAllItems(items, count);
            ModifyAllCount(item => GetSuggestedItemCount(sav, item.Index, count));
        }

        public void GiveAllItems(SaveFile sav, int count = -1) => GiveAllItems(sav, LegalItems, count);

        private void GiveAllItems(IReadOnlyList<ushort> newItems, int count = -1)
        {
            if (count < 0)
                count = MaxCount;

            var current = (InventoryItem[]) Items.Clone();
            var itemEnd = Math.Min(Items.Length, newItems.Count);
            for (int i = 0; i < itemEnd; i++)
            {
                var item = Items[i] = new InventoryItem
                {
                    Index = newItems[i],
                    Count = count,
                    New = true,
                };

                var match = Array.Find(current, z => z.Index == newItems[i]);
                if (match == null)
                    continue;

                // load old values
                item.Count = Math.Max(item.Count, match.Count);
                item.FreeSpace = match.FreeSpace;
                item.New = match.New;
            }
        }

        public bool IsValidItemAndCount(ITrainerInfo SAV, int itemindex, bool HasNew, bool HaX, ref int itemcnt)
        {
            if (HaX && SAV.Generation != 7) // Gen7 has true cap at 1023, keep 999 cap.
            {
                // Cap at absolute maximum
                if (SAV.Generation <= 2 && itemcnt > byte.MaxValue)
                    itemcnt = byte.MaxValue;
                else if (SAV.Generation >= 3 && itemcnt > ushort.MaxValue)
                    itemcnt = ushort.MaxValue;
                return true;
            }

            if (itemcnt > MaxCount)
            {
                if (itemindex == 797 && itemcnt >= 2) // Edge case when for some reason the item count for Z-Ring was 2 in an unedited save and set 1 after using PKHeX
                    itemcnt = 2;
                else
                    itemcnt = MaxCount; // Cap at pouch maximum
            }
            else if (itemcnt <= 0 && !HasNew)
            {
                return false;
            }

            itemcnt = GetSuggestedItemCount(SAV, itemindex, itemcnt);
            return true;
        }

        private int GetSuggestedItemCount(ITrainerInfo sav, int item, int requestVal = 1)
        {
            if (sav is SAV7b) // mixed pouch count caps
                return InventoryPouch7b.GetSuggestedCount(Type, item, requestVal);
            if (ItemConverter.IsItemHM((ushort)item, sav.Generation))
                return 1;
            return Math.Min(MaxCount, requestVal);
        }
    }

    public static class InventoryPouchExtensions
    {
        public static InventoryPouch[] LoadAll(this InventoryPouch[] value, byte[] Data)
        {
            foreach (var p in value)
                p.GetPouch(Data);
            return value;
        }

        public static void SaveAll(this InventoryPouch[] value, byte[] Data)
        {
            foreach (var p in value)
                p.SetPouch(Data);
        }
    }
}