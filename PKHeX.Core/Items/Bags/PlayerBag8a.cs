using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag8a : PlayerBag
{
    public override InventoryPouch8a[] Pouches { get; }
    public override ItemStorage8LA Info => ItemStorage8LA.Instance;

    public PlayerBag8a(SAV8LA sav) => Pouches = LoadPouches(sav.Accessor);

    private InventoryPouch8a[] LoadPouches(SCBlockAccessor access)
    {
        var satchel = (uint)access.GetBlock(SaveBlockAccessor8LA.KSatchelUpgrades).GetValue();
        var regularSize = (int)Math.Min(675, satchel + 20);
        var pouches = GetPouches(Info, regularSize);
        LoadFrom(pouches, access);
        return pouches;
    }

    private static InventoryPouch8a[] GetPouches(ItemStorage8LA info, int size) =>
    [
        new(size, 999, info, InventoryType.Items),
        new(0100, 001, info, InventoryType.KeyItems),
        new(0180, 999, info, InventoryType.PCItems),
        new(0070, 001, info, InventoryType.Treasure),
    ];

    public override void CopyTo(SaveFile sav) => CopyTo((SAV8LA)sav);
    public void CopyTo(SAV8LA sav) => CopyTo(sav.Accessor);
    public void CopyTo(SCBlockAccessor access) => CopyTo(Pouches, access);

    private static void LoadFrom(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        pouches[0].GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRegular).Data);
        pouches[1].GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemKey).Data);
        pouches[2].GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemStored).Data);
        pouches[3].GetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRecipe).Data);
        LoadFavorites(pouches, access);
    }

    private static void CopyTo(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        SavePouches(pouches, access);
        SaveFavorites(pouches, access);
    }

    private static void SavePouches(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        pouches[0].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRegular).Data);
        pouches[1].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemKey).Data);
        pouches[2].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemStored).Data);
        pouches[3].SetPouch(access.GetBlock(SaveBlockAccessor8LA.KItemRecipe).Data);
    }

    private static void LoadFavorites(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        var favorites = access.GetBlock(SaveBlockAccessor8LA.KItemFavorite).Data;
        foreach (var arr in pouches)
            LoadFavorites(arr.Items, favorites);
    }

    private static void SaveFavorites(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        var favorites = access.GetBlock(SaveBlockAccessor8LA.KItemFavorite).Data;
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
