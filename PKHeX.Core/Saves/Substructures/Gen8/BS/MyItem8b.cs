using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Player item pouches storage
    /// </summary>
    /// <remarks>size=0xBB80 (<see cref="ItemSaveSize"/> items)</remarks>
    public sealed class MyItem8b : MyItem
    {
        public const int ItemSaveSize = 3000;

        public MyItem8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public int GetItemQuantity(ushort itemIndex)
        {
            var ofs = InventoryPouch8b.GetItemOffset(itemIndex, Offset);
            var item = InventoryItem8b.Read(itemIndex, Data, ofs);
            return item.Count;
        }

        public void SetItemQuantity(ushort itemIndex, int quantity)
        {
            var ofs = InventoryPouch8b.GetItemOffset(itemIndex, Offset);
            var item = InventoryItem8b.Read(itemIndex, Data, ofs);
            item.Count = quantity;
            if (!item.IsValidSaveSortNumberCount) // not yet obtained
            {
                var type = GetType(itemIndex);
                item.SortOrder = GetNextSortIndex(type);
            }
            item.Write(Data, ofs);
        }

        public static InventoryType GetType(ushort itemIndex)
        {
            var types = new[]
            {
                InventoryType.Items, InventoryType.KeyItems, InventoryType.TMHMs, InventoryType.Medicine,
                InventoryType.Berries, InventoryType.Balls, InventoryType.BattleItems, InventoryType.Treasure,
            };
            return Array.Find(types, z => GetLegal(z).Contains(itemIndex));
        }

        public ushort GetNextSortIndex(InventoryType type)
        {
            ushort max = 0;
            foreach (var itemID in GetLegal(type))
            {
                var ofs = InventoryPouch8b.GetItemOffset(itemID, Offset);
                var item = InventoryItem8b.Read(itemID, Data, ofs);
                if (item.SortOrder > max)
                    max = item.SortOrder;
            }
            return ++max;
        }

        public override IReadOnlyList<InventoryPouch> Inventory { get => ConvertToPouches(); set => LoadFromPouches(value); }

        private IReadOnlyList<InventoryPouch> ConvertToPouches()
        {
            var pouches = new[]
            {
                MakePouch(InventoryType.Items, IsHeldItemLegal),
                MakePouch(InventoryType.KeyItems),
                MakePouch(InventoryType.TMHMs, IsHeldItemLegal),
                MakePouch(InventoryType.Medicine, IsHeldItemLegal),
                MakePouch(InventoryType.Berries, IsHeldItemLegal),
                MakePouch(InventoryType.Balls, IsHeldItemLegal),
                MakePouch(InventoryType.BattleItems, IsHeldItemLegal),
                MakePouch(InventoryType.Treasure, IsHeldItemLegal),
            };
            return pouches.LoadAll(Data);
        }

        private void LoadFromPouches(IReadOnlyList<InventoryPouch> value)
        {
            value.SaveAll(Data);
            CleanIllegalSlots();
        }

        private void CleanIllegalSlots()
        {
            var all = new[]
            {
                GetLegal(InventoryType.Items),
                GetLegal(InventoryType.KeyItems),
                GetLegal(InventoryType.TMHMs),
                GetLegal(InventoryType.Medicine),
                GetLegal(InventoryType.Berries),
                GetLegal(InventoryType.Balls),
                GetLegal(InventoryType.BattleItems),
                GetLegal(InventoryType.Treasure),
            }.SelectMany(z => z).Distinct();

            var hashSet = new HashSet<ushort>(all);
            for (ushort i = 0; i < (ushort)SAV.MaxItemID; i++) // even though there are 3000, just overwrite the ones that people will mess up.
            {
                if (!hashSet.Contains(i))
                    InventoryItem8b.Clear(Data, InventoryPouch8b.GetItemOffset(i, Offset));
            }
        }

        private InventoryPouch8b MakePouch(InventoryType type, Func<ushort, bool>? isLegal = null)
        {
            ushort[] legal = GetLegal(type);
            var max = GetMax(type);
            return new InventoryPouch8b(type, legal, max, Offset, isLegal);
        }

        public static bool IsHeldItemLegal(ushort item) => !Legal.HeldItems_BS.Contains(item) || Legal.ReleasedHeldItems_8b[item];

        private static int GetMax(InventoryType type) => type switch
        {
            InventoryType.Items => 999,
            InventoryType.KeyItems => 1,
            InventoryType.TMHMs => 999,
            InventoryType.Medicine => 999,
            InventoryType.Berries => 999,
            InventoryType.Balls => 999,
            InventoryType.BattleItems => 999,
            InventoryType.Treasure => 999,
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

        private static ushort[] GetLegal(InventoryType type) => type switch
        {
            InventoryType.Items => Legal.Pouch_Regular_BS,
            InventoryType.KeyItems => Legal.Pouch_Key_BS,
            InventoryType.TMHMs => Legal.Pouch_TMHM_BS,
            InventoryType.Medicine => Legal.Pouch_Medicine_BS,
            InventoryType.Berries => Legal.Pouch_Berries_BS,
            InventoryType.Balls => Legal.Pouch_Ball_BS,
            InventoryType.BattleItems => Legal.Pouch_Battle_BS,
            InventoryType.Treasure => Legal.Pouch_Treasure_BS,
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }
}
