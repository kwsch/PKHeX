using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Header information for the stored <see cref="PKM"/> data.
    /// </summary>
    /// <remarks>
    /// This block simply contains the following:
    /// u16 Party Pointers * 6: Indicates which index occupies this slot. <see cref="SLOT_EMPTY"/> if nothing in slot.
    /// u16 Starter Pointer: Indicates which index is the starter. <see cref="SLOT_EMPTY"/> if no starter yet.
    /// u16 List Count: Points to the next empty slot, and indicates how many slots are stored in the list.
    /// </remarks>
    public sealed class PokeListHeader : SaveBlock
    {
        /// <summary>
        /// Raw representation of data, casted to ushort[].
        /// </summary>
        internal readonly int[] PokeListInfo;

        private const int STARTER = 6;
        private const int COUNT = 7;
        private const int MAX_SLOTS = 1000;
        private const int SLOT_EMPTY = 1001;

        public PokeListHeader(SAV7b sav, int offset) : base(sav)
        {
            Offset = offset;
            PokeListInfo = LoadPointerData();
            if (!sav.Exportable)
            {
                for (int i = 0; i < COUNT; i++)
                    PokeListInfo[i] = SLOT_EMPTY;
            }
            PartyCount = PokeListInfo.Take(6).Count(z => z < MAX_SLOTS);
        }

        private int _partyCount;

        public int PartyCount
        {
            get => _partyCount;
            set
            {
                if (_partyCount > value)
                {
                    for (int i = _partyCount; i < value; i++)
                        ClearPartySlot(i);
                }
                _partyCount = value;
            }
        }

        public bool ClearPartySlot(int slot)
        {
            if (slot >= 6 || PartyCount <= 1)
                return false;

            if (slot > PartyCount)
            {
                slot = PartyCount;
            }
            else if (slot != PartyCount - 1)
            {
                int countShiftDown = PartyCount - 1 - slot;
                Array.Copy(PokeListInfo, slot + 1, PokeListInfo, slot, countShiftDown);
                slot = PartyCount - 1;
            }
            PokeListInfo[slot] = SLOT_EMPTY;
            PartyCount--;
            return true;
        }

        public void RemoveStarter() => StarterIndex = SLOT_EMPTY;

        public int StarterIndex
        {
            get => PokeListInfo[STARTER];
            set
            {
                if ((ushort)value > 1000 && value != SLOT_EMPTY)
                    throw new ArgumentException(nameof(value));
                PokeListInfo[STARTER] = (ushort)value;
            }
        }

        public int Count
        {
            get => BitConverter.ToUInt16(Data, Offset + (COUNT * 2));
            set => BitConverter.GetBytes((ushort) value).CopyTo(Data, Offset + (COUNT * 2));
        }

        private int[] LoadPointerData()
        {
            var list = new int[7];
            for (int i = 0; i < list.Length; i++)
                list[i] = BitConverter.ToUInt16(Data, Offset + (i * 2));
            return list;
        }

        private void SetPointerData(IList<int> vals)
        {
            for (int i = 0; i < vals.Count; i++)
                BitConverter.GetBytes((ushort)vals[i]).CopyTo(Data, Offset + (i * 2));
            vals.CopyTo(PokeListInfo);
        }

        public int GetPartyOffset(int slot)
        {
            if ((uint)slot >= 6)
                throw new ArgumentException(nameof(slot) + " expected to be < 6.");
            int position = PokeListInfo[slot];
            return SAV.GetBoxSlotOffset(position);
        }

        public int GetPartyIndex(int box, int slot)
        {
            int slotIndex = slot + (SAV.BoxSlotCount * box);
            if ((uint)slotIndex >= MAX_SLOTS)
                return MAX_SLOTS;
            var index = Array.IndexOf(PokeListInfo, slotIndex);
            return index >= 0 ? index : MAX_SLOTS;
        }

        public bool CompressStorage()
        {
            // Box Data is stored as a list, instead of an array. Empty interstitials are not legal.
            // Fix stored slots!
            var arr = PokeListInfo.Take(7).ToArray();
            var result = SAV.CompressStorage(out int count, arr);
            Debug.Assert(count <= MAX_SLOTS);
            Count = count;
            if (StarterIndex > count && StarterIndex != SLOT_EMPTY)
            {
                // uh oh, we lost the starter! might have been moved out of its proper slot incorrectly.
                var spec = SAV.Version == GameVersion.GP ? 25 : 133;
                bool Starter(int species, int form) => species == spec && form != 0;
                int index = Array.FindIndex(SAV.BoxData.ToArray(), z => Starter(z.Species, z.AltForm));
                if (index >= 0)
                    arr[6] = index;
            }
            arr.CopyTo(PokeListInfo);

            SetPointerData(PokeListInfo);
            return result;
        }
    }
}