using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Stores the inventory of items that the player has acquired.
/// </summary>
/// <remarks>
/// Reads four separate pouch blobs: Items, Key Items, Storage, and Recipes.
/// </remarks>
public sealed class MyItem8a(SAV8LA sav, SCBlock block) : MyItem(sav, block.Data)
{
    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var access = sav.Accessor;
            var satchel = (uint)access.GetBlock(SaveBlockAccessor8LA.KSatchelUpgrades).GetValue();
            var regularSize = (int)Math.Min(675, satchel + 20);

            var info = ItemStorage8LA.Instance;
            var regular = new InventoryPouch8a(InventoryType.Items,    info, 999, regularSize);
            var key     = new InventoryPouch8a(InventoryType.KeyItems, info,   1, 100);
            var stored  = new InventoryPouch8a(InventoryType.PCItems,  info, 999, 180);
            var recipe = new InventoryPouch8a(InventoryType.Treasure,  info,   1,  70);
            regular.GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRegular).Data);
            key   .GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemKey).Data);
            stored.GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemStored).Data);
            recipe.GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRecipe).Data);

            InventoryPouch8a[] result = [ regular, key, stored, recipe ];
            LoadFavorites(result, access);
            return result;
        }
        set
        {
            var access = sav.Accessor;
            foreach (var p in value)
                ((InventoryPouch8a)p).SanitizeCounts();

            value[0].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRegular).Data);
            value[1].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemKey).Data);
            value[2].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemStored).Data);
            value[3].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRecipe).Data);
            SaveFavorites((InventoryPouch8a[])value, access);
        }
    }

    private static void LoadFavorites(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        var favorites = access.GetBlock(SaveBlockAccessor8LA.KItemFavorite).Data.AsSpan();
        foreach (var arr in pouches)
            LoadFavorites(arr.Items, favorites);
    }

    private static void SaveFavorites(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        var favorites = access.GetBlock(SaveBlockAccessor8LA.KItemFavorite).Data.AsSpan();
        favorites.Clear();
        foreach (var arr in pouches)
            SaveFavorites(arr.Items, favorites);
    }

    private static void LoadFavorites(ReadOnlySpan<InventoryItem> items, ReadOnlySpan<byte> favorites)
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
