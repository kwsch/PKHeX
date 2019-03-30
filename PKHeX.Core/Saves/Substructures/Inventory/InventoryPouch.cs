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
            Type = type;
            LegalItems = legal;
            MaxCount = maxcount;
            Offset = offset;
            PouchDataSize = size > -1 ? size : legal.Length;
        }

        public abstract void GetPouch(byte[] Data);
        public abstract void SetPouch(byte[] Data);

        public void SortByCount(bool reverse = false)
        {
            var list = Items.Where(item => item.Index != 0).OrderBy(item => item.Count == 0);
            list = reverse
                ? list.ThenByDescending(item => item.Count)
                : list.ThenBy(item => item.Count);
            Items = list.Concat(Items.Where(item => item.Index == 0)).ToArray();
        }

        public void SortByIndex(bool reverse = false)
        {
            var list = Items.Where(item => item.Index != 0).OrderBy(item => item.Count == 0);
            list = reverse
                ? list.ThenByDescending(item => item.Index)
                : list.ThenBy(item => item.Index);
            Items = list.Concat(Items.Where(item => item.Index == 0)).ToArray();
        }

        public void SortByName(string[] names, bool reverse = false)
        {
            var list = Items.Where(item => item.Index != 0 && item.Index < names.Length).OrderBy(item => item.Count == 0);
            list = reverse
                ? list.ThenByDescending(item => names[item.Index])
                : list.ThenBy(item => names[item.Index]);
            Items = list.Concat(Items.Where(item => item.Index == 0 || item.Index >= names.Length)).ToArray();
        }

        public void Sanitize(bool HaX, int MaxItemID)
        {
            var x = GetValidItems(HaX, MaxItemID);
            var count = PouchDataSize - x.Count;
            Items = x.Concat(Enumerable.Range(0, count).Select(_ => new InventoryItem())).ToArray();
        }

        public IList<InventoryItem> GetValidItems(bool HaX, int MaxItemID)
        {
            return Items
                .Where(item => item.Valid(LegalItems, HaX, MaxItemID))
                .ToList();
        }

        public IList<InventoryItem> GetInvalidItems(bool HaX, int MaxItemID)
        {
            return Items
                .Where(item => !item.Valid(LegalItems, HaX, MaxItemID))
                .ToList();
        }

        public void MoveEmptySlots()
        {
            Items = Items.OrderBy(z => z.Count == 0).ToArray();
        }

        public void RemoveAll()
        {
            foreach (var item in Items)
                item.Clear();
        }

        public void RemoveAll(Func<InventoryItem, bool> deleteCriteria)
        {
            if (deleteCriteria == null)
                throw new ArgumentNullException(nameof(deleteCriteria));
            foreach (var item in Items.Where(deleteCriteria))
                item.Clear();
        }

        public void RemoveAll(Func<InventoryItem, int, bool> deleteCriteria)
        {
            if (deleteCriteria == null)
                throw new ArgumentNullException(nameof(deleteCriteria));
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
            if (modifyCriteria == null)
                throw new ArgumentNullException(nameof(modifyCriteria));
            foreach (var item in Items.Where(z => z.Count != 0).Where(modifyCriteria))
                item.Count = value;
        }

        public void ModifyAllCount(int value, Func<InventoryItem, int, bool> modifyCriteria)
        {
            if (modifyCriteria == null)
                throw new ArgumentNullException(nameof(modifyCriteria));
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
            if (modification == null)
                throw new ArgumentNullException(nameof(modification));
            foreach (var item in Items.Where(z => z.Count != 0))
                item.Count = modification(item);
        }

        public void GiveAllItems(IReadOnlyList<ushort> newItems, Func<InventoryItem, int> getSuggestedItemCount, int count = -1)
        {
            if (getSuggestedItemCount == null)
                throw new ArgumentNullException(nameof(getSuggestedItemCount));

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
            }
            else if (itemcnt > MaxCount)
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