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

        public int GetItemQuantity(ushort item)
        {
            var ofs = InventoryPouch8b.GetItemOffset(item, Offset);
            var data = InventoryPouch8b.ReadItem(item, Data, ofs);
            return data.Count;
        }

        public void SetItemQuantity(ushort item, int quantity)
        {
            var ofs = InventoryPouch8b.GetItemOffset(item, Offset);
            var data = InventoryPouch8b.ReadItem(item, Data, ofs);
            data.Count = quantity;
            InventoryPouch8b.WriteItem(data, Data, ofs);
        }

        public override IReadOnlyList<InventoryPouch> Inventory { get => ConvertToPouches(); set => LoadFromPouches(value); }

        private IReadOnlyList<InventoryPouch> ConvertToPouches()
        {
            var pouches = new[]
            {
                MakePouch(InventoryType.Items),
                MakePouch(InventoryType.KeyItems),
                MakePouch(InventoryType.TMHMs),
                MakePouch(InventoryType.Medicine),
                MakePouch(InventoryType.Berries),
                MakePouch(InventoryType.Balls),
                MakePouch(InventoryType.BattleItems),
                MakePouch(InventoryType.Treasure),
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
            var empty = new InventoryItem();
            for (ushort i = 0; i < (ushort)SAV.MaxItemID; i++) // even though there are 3000, just overwrite the ones that people will mess up.
            {
                if (!hashSet.Contains(i))
                    InventoryPouch8b.WriteItem(empty, Data, InventoryPouch8b.GetItemOffset(i, Offset));
            }
        }

        private InventoryPouch8b MakePouch(InventoryType type)
        {
            ushort[] legal = GetLegal(type);
            var max = GetMax(type);
            return new InventoryPouch8b(type, legal, max, Offset);
        }

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
            _ => throw new ArgumentOutOfRangeException(nameof(type))
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
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}
