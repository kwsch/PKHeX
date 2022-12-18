using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Player item pouches storage
/// </summary>
/// <remarks>size=0xBB80 (<see cref="ItemSaveSize"/> items)</remarks>
public sealed class MyItem9 : MyItem
{
    public const int ItemSaveSize = 3000;

    public MyItem9(SaveFile SAV, SCBlock block) : base(SAV, block.Data) { }

    public int GetItemQuantity(ushort itemIndex)
    {
        var ofs = InventoryPouch9.GetItemOffset(itemIndex);
        var span = Data.AsSpan(ofs, InventoryItem9.SIZE);
        var item = InventoryItem9.Read(itemIndex, span);
        return item.Count;
    }

    public void SetItemQuantity(ushort itemIndex, int quantity)
    {
        var ofs = InventoryPouch9.GetItemOffset(itemIndex);
        var span = Data.AsSpan(ofs, InventoryItem9.SIZE);
        var item = InventoryItem9.Read(itemIndex, span);
        item.Count = quantity;
        item.Pouch = GetPouchIndex(GetType(itemIndex));
        item.Write(span);
    }

    public static InventoryType GetType(ushort itemIndex)
    {
        var types = new[]
        {
            InventoryType.Items, InventoryType.KeyItems, InventoryType.TMHMs, InventoryType.Medicine,
            InventoryType.Berries, InventoryType.Balls, InventoryType.BattleItems, InventoryType.Treasure,
            InventoryType.Ingredients, InventoryType.Candy,
        };
        return Array.Find(types, z => GetLegal(z).Contains(itemIndex));
    }

    public override IReadOnlyList<InventoryPouch> Inventory { get => ConvertToPouches(); set => LoadFromPouches(value); }

    private IReadOnlyList<InventoryPouch> ConvertToPouches()
    {
        var pouches = new[]
        {
            MakePouch(InventoryType.Medicine, IsHeldItemLegal),
            MakePouch(InventoryType.Balls, IsHeldItemLegal),
            MakePouch(InventoryType.BattleItems, IsHeldItemLegal),
            MakePouch(InventoryType.Berries, IsHeldItemLegal),
            MakePouch(InventoryType.Items, IsHeldItemLegal),
            MakePouch(InventoryType.TMHMs, IsHeldItemLegal),
            MakePouch(InventoryType.Treasure, IsHeldItemLegal),
            MakePouch(InventoryType.Ingredients, IsHeldItemLegal),
            MakePouch(InventoryType.KeyItems),
            MakePouch(InventoryType.Candy, IsHeldItemLegal),
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
            GetLegal(InventoryType.Ingredients),
            GetLegal(InventoryType.Candy),
        }.SelectMany(z => z).Distinct();

        var hashSet = new HashSet<ushort>(all);
        for (ushort i = 0; i < (ushort)SAV.MaxItemID; i++) // even though there are 3000, just overwrite the ones that people will mess up.
        {
            if (!hashSet.Contains(i))
                InventoryItem9.Clear(Data, InventoryPouch9.GetItemOffset(i));
        }
    }

    private static InventoryPouch9 MakePouch(InventoryType type, Func<ushort, bool>? isLegal = null)
    {
        ushort[] legal = GetLegal(type);
        var max = GetMax(type);
        return new InventoryPouch9(type, legal, max, GetPouchIndex(type), isLegal);
    }

    public static bool IsHeldItemLegal(ushort item) => !Legal.HeldItems_SV.Contains(item) || Legal.ReleasedHeldItems_9[item];

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
        InventoryType.Ingredients => 999, // 999
        InventoryType.Candy => 999, // 999
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    private static ushort[] GetLegal(InventoryType type) => type switch
    {
        InventoryType.Items => Legal.Pouch_Other_SV,
        InventoryType.KeyItems => Legal.Pouch_Event_SV,
        InventoryType.TMHMs => Legal.Pouch_TM_SV,
        InventoryType.Medicine => Legal.Pouch_Medicine_SV,
        InventoryType.Berries => Legal.Pouch_Berries_SV,
        InventoryType.Balls => Legal.Pouch_Ball_SV,
        InventoryType.BattleItems => Legal.Pouch_Battle_SV,
        InventoryType.Treasure => Legal.Pouch_Treasure_SV,
        InventoryType.Ingredients => Legal.Pouch_Picnic_SV,
        InventoryType.Candy => Legal.Pouch_Material_SV,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    private static uint GetPouchIndex(InventoryType type) => type switch
    {
        InventoryType.Items => InventoryItem9.PouchOther,
        InventoryType.KeyItems => InventoryItem9.PouchEvent,
        InventoryType.TMHMs => InventoryItem9.PouchTMHM,
        InventoryType.Medicine => InventoryItem9.PouchMedicine,
        InventoryType.Berries => InventoryItem9.PouchBerries,
        InventoryType.Balls => InventoryItem9.PouchBall,
        InventoryType.BattleItems => InventoryItem9.PouchBattle,
        InventoryType.Treasure => InventoryItem9.PouchTreasure,
        InventoryType.Ingredients => InventoryItem9.PouchPicnic,
        InventoryType.Candy => InventoryItem9.PouchMaterial,
        _ => InventoryItem9.PouchNone,
    };
}
