using System.Linq;

namespace PKHeX.Core
{
    public abstract class InventoryPouch
    {
        public readonly InventoryType Type;
        public readonly ushort[] LegalItems;
        public readonly int MaxCount;
        public int Count => Items.Count(it => it.Count > 0);
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
            var x = Items.Where(item => item.Valid(LegalItems, HaX, MaxItemID)).ToArray();
            Items = x.Concat(new byte[PouchDataSize - x.Length].Select(_ => new InventoryItem())).ToArray();
        }
    }
}