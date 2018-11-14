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
    /// u16 Follower Pointer: Indicates which index is following the player. <see cref="SLOT_EMPTY"/> if nothing following.
    /// u16 List Count: Points to the next empty slot, and indicates how many slots are stored in the list.
    /// </remarks>
    public sealed class PokeListHeader : SaveBlock
    {
        private readonly SaveFile SAV;

        /// <summary>
        /// Raw representation of data, casted to ushort[].
        /// </summary>
        internal readonly int[] PokeListInfo;

        private const int FOLLOW = 6;
        private const int COUNT = 7;
        private const int MAX_SLOTS = 1000;
        private const int SLOT_EMPTY = 1001;

        public PokeListHeader(SaveFile sav) : base(sav)
        {
            SAV = sav;
            Offset = ((SAV7b)sav).GetBlockOffset(BelugaBlockIndex.PokeListHeader);
            PokeListInfo = LoadPointerData();
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

        public void RemoveFollower() => FollowerIndex = SLOT_EMPTY;

        public int FollowerIndex
        {
            get => PokeListInfo[FOLLOW];
            set
            {
                if ((ushort)value > 1000)
                    throw new ArgumentException(nameof(value));
                PokeListInfo[FOLLOW] = (ushort)value;
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

        public bool IsSlotInBattleTeam(int box, int slot)
        {
            if ((uint)slot >= SAV.SlotCount || (uint)box >= SAV.BoxCount)
                return false;

            int slotIndex = slot + (SAV.BoxSlotCount * box);
            return PokeListInfo.Take(6).Any(s => s == slotIndex) || FollowerIndex == slotIndex;
        }

        public bool IsSlotLocked(int box, int slot)
        {
            if ((uint)slot >= SAV.SlotCount || (uint)box >= SAV.BoxCount)
                return false;
            return false;
        }

        public bool CompressStorage()
        {
            // Box Data is stored as a list, instead of an array. Empty interstitials are not legal.
            // Fix stored slots!
            var arr = PokeListInfo.Take(7).ToArray();
            var result = SAV.CompressStorage(out int count, arr);
            Debug.Assert(count <= MAX_SLOTS);
            arr.CopyTo(PokeListInfo);
            Count = count;
            if (FollowerIndex > count && FollowerIndex != SLOT_EMPTY)
                RemoveFollower();

            if (result)
                SetPointerData(PokeListInfo);
            return result;
        }
    }
}