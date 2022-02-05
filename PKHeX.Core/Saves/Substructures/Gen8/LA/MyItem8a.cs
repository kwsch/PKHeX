using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Stores the inventory of items that the player has acquired.
/// </summary>
/// <remarks>
/// Reads four separate pouch blobs: Items, Key Items, Storage, and Recipes.
/// </remarks>
public sealed class MyItem8a : MyItem
{
    public MyItem8a(SAV8LA SAV, SCBlock block) : base(SAV, block.Data) { }

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var access = ((SAV8LA)SAV).Accessor;
            var satchel = (uint)access.GetBlock(SaveBlockAccessor8LA.KSatchelUpgrades).GetValue();
            var regularSize = (int)Math.Min(675, satchel);

            var regular = new InventoryPouch8a(InventoryType.Items,    Legal.Pouch_Items_LA , 999, regularSize);
            var key     = new InventoryPouch8a(InventoryType.KeyItems, Legal.Pouch_Key_LA   ,   1, 100);
            var stored  = new InventoryPouch8a(InventoryType.PCItems,  Legal.Pouch_Items_LA , 999, 180);
            var recipe = new InventoryPouch8a(InventoryType.Treasure,  Legal.Pouch_Recipe_LA,   1,  70);
            regular.GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRegular).Data);
            key   .GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemKey).Data);
            stored.GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemStored).Data);
            recipe.GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRecipe).Data);

            var result = new[] { regular, key, stored, recipe };
            LoadFavorites(result, access);
            return result;
        }
        set
        {
            var access = ((SAV8LA)SAV).Accessor;
            foreach (var p in value)
                ((InventoryPouch8a)p).SanitizeCounts();

            value[0].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRegular).Data);
            value[1].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemKey).Data);
            value[2].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemStored).Data);
            value[3].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRecipe).Data);
            SaveFavorites(value, access);
        }
    }

    private static void LoadFavorites(IEnumerable<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        var favorites = access.GetBlock(SaveBlockAccessor8LA.KItemFavorite).Data.AsSpan();
        foreach (var arr in pouches)
            LoadFavorites(arr.Items, favorites);
    }

    private static void SaveFavorites(IEnumerable<InventoryPouch> pouches, SCBlockAccessor access)
    {
        var favorites = access.GetBlock(SaveBlockAccessor8LA.KItemFavorite).Data.AsSpan();
        favorites.Clear();
        foreach (var arr in pouches)
            SaveFavorites(arr.Items, favorites);
    }

    private static void LoadFavorites(IEnumerable<InventoryItem> items, Span<byte> favorites)
    {
        foreach (var z in items)
        {
            var item = (InventoryItem8a)z;
            var itemID = item.Index;
            var ofs = itemID >> 3;
            if ((uint)ofs >= favorites.Length)
                continue;

            var bit = itemID & 7;
            item.IsFavorite = FlagUtil.GetFlag(favorites, ofs, bit);
        }
    }

    private static void SaveFavorites(IEnumerable<InventoryItem> items, Span<byte> favorites)
    {
        foreach (var z in items)
        {
            var item = (InventoryItem8a)z;
            var itemID = item.Index;
            var ofs = itemID >> 3;
            if ((uint)ofs >= favorites.Length)
                continue;

            var bit = itemID & 7;
            var value = FlagUtil.GetFlag(favorites, ofs, bit);
            value |= item.IsFavorite;
            FlagUtil.SetFlag(favorites, ofs, bit, value);
        }
    }
}
