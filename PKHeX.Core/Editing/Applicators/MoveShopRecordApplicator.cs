using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Logic for modifying the Move Shop Record flags of a <see cref="PA8"/>.
/// </summary>
public static class MoveShopRecordApplicator
{
    public static void ClearMoveShopFlags(this IMoveShop8 shop)
    {
        var bits = shop.MoveShopPermitFlags;
        for (int i = 0; i < bits.Length; i++)
            shop.SetPurchasedRecordFlag(i, false);

        if (shop is IMoveShop8Mastery m)
            m.ClearMoveShopFlagsMastered();
    }

    public static void ClearMoveShopFlagsMastered(this IMoveShop8Mastery shop)
    {
        var bits = shop.MoveShopPermitFlags;
        for (int i = 0; i < bits.Length; i++)
            shop.SetMasteredRecordFlag(i, false);
    }

    public static void SetMoveShopFlags(this IMoveShop8 shop, bool value, int max = 100)
    {
        var bits = shop.MoveShopPermitFlags;
        max = Math.Min(bits.Length, max);
        for (int i = 0; i < max; i++)
            shop.SetPurchasedRecordFlag(i, value);
    }

    public static void SetMoveShopFlagsMastered(this IMoveShop8Mastery shop)
    {
        var bits = shop.MoveShopPermitFlags;
        for (int i = 0; i < bits.Length; i++)
            shop.SetMasteredRecordFlag(i, shop.GetPurchasedRecordFlag(i));
    }

    public static void SetMoveShopFlags(this IMoveShop8 shop)
    {
        var permit = shop.MoveShopPermitFlags;
        for (int index = 0; index < permit.Length; index++)
        {
            if (permit[index])
                shop.SetPurchasedRecordFlag(index, true);
        }
    }

    /// <summary>
    /// Sets the Shop Record flags for the <see cref="shop"/> based on the current moves.
    /// </summary>
    /// <param name="shop">Pokémon to modify.</param>
    /// <param name="moves">Moves to set flags for. If a move is not a Technical Record, it is skipped.</param>
    public static void SetMoveShopFlags(this IMoveShop8 shop, IEnumerable<int> moves)
    {
        var permit = shop.MoveShopPermitFlags;
        var moveIDs = shop.MoveShopPermitIndexes;
        foreach (var m in moves)
        {
            var index = moveIDs.IndexOf((ushort)m);
            if (index == -1)
                continue;
            if (permit[index])
                shop.SetPurchasedRecordFlag(index, true);
        }
    }
}