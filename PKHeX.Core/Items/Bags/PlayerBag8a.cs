using System;
using System.Collections.Generic;
using static PKHeX.Core.SaveBlockAccessor8LA;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag8a : PlayerBag
{
    public override InventoryPouch8a[] Pouches { get; }
    public override ItemStorage8LA Info => ItemStorage8LA.Instance;

    public PlayerBag8a(SAV8LA sav) => Pouches = LoadPouches(sav.Accessor);

    private InventoryPouch8a[] LoadPouches(SCBlockAccessor access)
    {
        var satchel = (uint)access.GetBlock(KSatchelUpgrades).GetValue();
        var regularSize = (int)Math.Min(675, satchel + 20);
        var pouches = GetPouches(Info, regularSize);
        LoadFrom(pouches, access);
        return pouches;
    }

    private static InventoryPouch8a[] GetPouches(ItemStorage8LA info, int size) =>
    [
        new(size, 999, info, Items),
        new(0100, 001, info, KeyItems),
        new(0180, 999, info, PCItems),
        new(0070, 001, info, Treasure),
    ];

    public override void CopyTo(SaveFile sav) => CopyTo((SAV8LA)sav);
    public void CopyTo(SAV8LA sav) => CopyTo(sav.Accessor);
    public void CopyTo(SCBlockAccessor access) => CopyTo(Pouches, access);

    private static void LoadFrom(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        pouches[0].GetPouch(access.GetBlock(KItemRegular).Data);
        pouches[1].GetPouch(access.GetBlock(KItemKey).Data);
        pouches[2].GetPouch(access.GetBlock(KItemStored).Data);
        pouches[3].GetPouch(access.GetBlock(KItemRecipe).Data);
        LoadFavorites(pouches, access);
    }

    private static void CopyTo(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        SavePouches(pouches, access);
        SaveFavorites(pouches, access);
    }

    private static void SavePouches(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        pouches[0].SetPouch(access.GetBlock(KItemRegular).Data);
        pouches[1].SetPouch(access.GetBlock(KItemKey).Data);
        pouches[2].SetPouch(access.GetBlock(KItemStored).Data);
        pouches[3].SetPouch(access.GetBlock(KItemRecipe).Data);
    }

    private static void LoadFavorites(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        var favorites = access.GetBlock(KItemFavorite).Data;
        foreach (var arr in pouches)
            LoadFavorites(arr.Items, favorites);
    }

    private static void SaveFavorites(ReadOnlySpan<InventoryPouch8a> pouches, SCBlockAccessor access)
    {
        var favorites = access.GetBlock(KItemFavorite).Data;
        favorites.Clear();
        foreach (var arr in pouches)
            SaveFavorites(arr.Items, favorites);
    }

    private static void LoadFavorites(ReadOnlySpan<InventoryItem8a> items, ReadOnlySpan<byte> favorites)
    {
        foreach (var item in items)
        {
            var itemID = item.Index;
            var ofs = itemID >> 3;
            if ((uint)ofs >= favorites.Length)
                continue;

            var bit = itemID & 7;
            item.IsFavorite = FlagUtil.GetFlag(favorites, ofs, bit);
        }
    }

    private static void SaveFavorites(IEnumerable<InventoryItem8a> items, Span<byte> favorites)
    {
        foreach (var item in items)
        {
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
