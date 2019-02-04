using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public abstract class SlotView : IList<PKM>
    {
        public static readonly SlotArray Empty = new SlotArray(null, Array.Empty<StorageSlotOffset>(), true);

        protected readonly SaveFile SAV;
        public StorageSlotType Type { get; internal set; }
        public bool IsParty { get; internal set; }

        public int Capacity { get; protected set; }
        public bool IsReadOnly { get; }

        public bool Full => Count == Capacity;
        public decimal PercentFull => Count / (decimal)Capacity;
        public IEnumerable<int> Indexes => Enumerable.Range(0, Capacity);
        public IEnumerable<int> Offsets => Indexes.Select(GetOffset);
        public IEnumerable<PKM> Stream => Indexes.Select(z => this[z]);
        public PKM[] Items => Stream.ToArray();

        internal List<SlotChange> Changes { private get; set; }

        protected abstract int GetOffset(int index);

        protected bool IsPKMPresent(int index)
        {
            int offset = GetOffset(index);
            return SAV.IsPKMPresent(offset);
        }

        public int FirstEmptyIndex(int start = 0)
        {
            for (int i = start; i < Capacity; i++)
            {
                int offset = GetOffset(i);
                if (!IsPKMPresent(offset))
                    return i;
            }
            return -1;
        }

        protected SlotView(SaveFile sav, int capacity, bool isReadOnly)
        {
            SAV = sav;
            IsReadOnly = isReadOnly;
            Capacity = capacity;
        }

        #region IList Implementation

        public virtual int Count => Offsets.Count(SAV.IsPKMPresent);
        public bool Contains(PKM item) => IndexOf(item) >= 0;
        public IEnumerator<PKM> GetEnumerator() => Stream.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Stream.GetEnumerator();

        public PKM this[int index]
        {
            get => GetIndex(index);
            set => SetIndex(value, index);
        }

        protected void SetIndex(PKM value, int index)
        {
            if (IsReadOnly)
                throw new ArgumentException(nameof(IsReadOnly));

            int offset = GetOffset(index);

            Changes.Add(GetUndo(index, offset));

            if (!IsParty)
            {
                SAV.SetStoredSlot(value, offset);
                return;
            }

            value.ForcePartyData();
            SAV.SetPartySlot(value, offset);
        }

        private SlotChange GetUndo(int index, int offset)
        {
            return new SlotChange
            {
                IsPartyFormat = IsParty,
                Offset = offset,
                Editable = IsReadOnly,
                PKM = GetFromOffset(offset),
                Box = index,
                Type = Type,
            };
        }

        private PKM GetIndex(int index) => GetFromOffset(GetOffset(index));
        private PKM GetFromOffset(int offset) => IsParty ? SAV.GetPartySlot(offset) : SAV.GetStoredSlot(offset);

        public void Insert(int index, PKM item)
        {
            int offset = GetOffset(index);
            if (SAV.IsPKMPresent(offset))
                ShiftDown(index);
            this[index] = item;
        }

        public void RemoveAt(int index)
        {
            int offset = GetOffset(index);
            if (SAV.IsPKMPresent(offset))
                this[index] = SAV.BlankPKM;
            ShiftUp(index);
        }

        /// <summary>
        /// Adds the item to the first empty index.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(PKM item)
        {
            var index = FirstEmptyIndex();
            if (index < 0)
                throw new ArgumentException(nameof(item));
            this[index] = item;
        }

        public void Clear()
        {
            var blank = SAV.BlankPKM;
            if (blank == null)
                throw new ArgumentException(nameof(blank));
            for (int i = 0; i < Capacity; i++)
                this[i] = blank;
        }

        public bool Remove(PKM item)
        {
            var index = IndexOf(item);
            if (index < 0)
                return false;
            RemoveAt(index);
            return true;
        }

        public virtual void CopyTo(PKM[] array, int arrayIndex)
        {
            for (int i = arrayIndex, ctr = 0; i < Capacity; i++, ctr++)
                array[i] = this[ctr];
        }

        public int IndexOf(PKM item)
        {
            int ctr = 0;
            foreach (var obj in Stream)
            {
                if (obj.Equals(item))
                    return ctr;
                ctr++;
            }
            return -1;
        }

        protected virtual void ShiftUp(int startIndex)
        {
            // something was deleted, move all slots up starting at index
            var end = FirstEmptyIndex(startIndex + 1);
            if (end < 0)
                end = Capacity - 1;

            for (int i = startIndex; i < end; i++)
                this[i] = this[i + 1];
            this[Capacity - 1] = SAV.BlankPKM;
        }

        protected virtual void ShiftDown(int startIndex)
        {
            // something needs to be inserted, move all slots down starting at index
            var end = FirstEmptyIndex(startIndex + 1);
            if (end < 0)
                end = Capacity - 1;

            for (int i = end; i >= startIndex; i--)
                this[i + 1] = this[i];
            this[Capacity - 1] = SAV.BlankPKM;
        }

        #endregion
    }
}
