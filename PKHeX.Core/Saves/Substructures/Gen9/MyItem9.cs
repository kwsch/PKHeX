using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Player item pouches storage
/// </summary>
/// <remarks>size=0xBB80 (<see cref="ItemSaveSize"/> items)</remarks>
public sealed class MyItem9(SAV9SV sav, SCBlock block) : MyItem(sav, block.Data)
{
    public const int ItemSaveSize = 3000;

    public int GetItemQuantity(ushort itemIndex)
    {
        var ofs = InventoryPouch9.GetItemOffset(itemIndex);
        var span = Data.Slice(ofs, InventoryItem9.SIZE);
        var item = InventoryItem9.Read(itemIndex, span);
        return item.Count;
    }

    public void SetItemQuantity(ushort itemIndex, int quantity)
    {
        var ofs = InventoryPouch9.GetItemOffset(itemIndex);
        var span = Data.Slice(ofs, InventoryItem9.SIZE);
        var item = InventoryItem9.Read(itemIndex, span);
        item.Count = quantity;
        item.Pouch = GetPouchIndex(GetType(itemIndex));
        item.Write(span);
    }

    public static InventoryType GetType(ushort itemIndex) => ItemStorage9SV.GetInventoryPouch(itemIndex);

    public override IReadOnlyList<InventoryPouch> Inventory { get => ConvertToPouches(); set => LoadFromPouches(value); }

    private IReadOnlyList<InventoryPouch> ConvertToPouches()
    {
        InventoryPouch9[] pouches =
        [
            MakePouch(InventoryType.Medicine),
            MakePouch(InventoryType.Balls),
            MakePouch(InventoryType.BattleItems),
            MakePouch(InventoryType.Berries),
            MakePouch(InventoryType.Items),
            MakePouch(InventoryType.TMHMs),
            MakePouch(InventoryType.Treasure),
            MakePouch(InventoryType.Ingredients),
            MakePouch(InventoryType.KeyItems),
            MakePouch(InventoryType.Candy),
        ];
        return pouches.LoadAll(Data);
    }

    private void LoadFromPouches(IReadOnlyList<InventoryPouch> value)
    {
        value.SaveAll(Data);
        CleanIllegalSlots();
    }

    private void CleanIllegalSlots()
    {
        var types = ItemStorage9SV.ValidTypes;
        var hashSet = new HashSet<ushort>(Legal.MaxItemID_9);
        foreach (var type in types)
        {
            var items = ItemStorage9SV.GetLegal(type);
            foreach (var item in items)
                hashSet.Add(item);
        }
        for (ushort i = 0; i < (ushort)SAV.MaxItemID; i++) // even though there are 3000, just overwrite the ones that people will mess up.
        {
            if (!hashSet.Contains(i))
                InventoryItem9.Clear(Data, InventoryPouch9.GetItemOffset(i));
        }
    }

    private static InventoryPouch9 MakePouch(InventoryType type)
    {
        var info = ItemStorage9SV.Instance;
        var max = info.GetMax(type);
        return new InventoryPouch9(type, info, max, GetPouchIndex(type));
    }

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
