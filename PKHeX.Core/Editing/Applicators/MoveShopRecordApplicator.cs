using System;

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

    public static void SetMoveShopFlags(this IMoveShop8Mastery shop, PKM pk)
    {
        Span<ushort> moves = stackalloc ushort[4];
        pk.GetMoves(moves);
        shop.SetMoveShopFlags(moves, pk);
    }

    public static void SetMoveShopFlags(this IMoveShop8Mastery shop, ReadOnlySpan<ushort> moves, PKM pk)
    {
        var index = PersonalTable.LA.GetFormIndex(pk.Species, pk.Form);
        var learn = Legal.LevelUpLA[index];
        var mastery = Legal.MasteryLA[index];
        var level = pk.CurrentLevel;

        shop.SetMoveShopFlags(moves, learn, mastery, level);
    }

    public static void SetMoveShopFlagsAll(this IMoveShop8Mastery shop, PKM pk)
    {
        var index = PersonalTable.LA.GetFormIndex(pk.Species, pk.Form);
        var learn = Legal.LevelUpLA[index];
        var mastery = Legal.MasteryLA[index];
        var level = pk.CurrentLevel;

        shop.SetMoveShopFlagsAll(learn, mastery, level);
    }

    public static void SetMoveShopFlagsAll(this IMoveShop8Mastery shop, Learnset learn, Learnset mastery, int level)
    {
        var possible = shop.MoveShopPermitIndexes;
        var permit = shop.MoveShopPermitFlags;
        for (int index = 0; index < possible.Length; index++)
        {
            var move = possible[index];
            var allowed = permit[index];
            if (!allowed)
                continue;

            SetMasteredFlag(shop, learn, mastery, level, index, move);
        }
    }

    public static void SetMoveShopFlags(this IMoveShop8Mastery shop, ReadOnlySpan<ushort> moves, Learnset learn, Learnset mastery, int level)
    {
        var possible = shop.MoveShopPermitIndexes;
        var permit = shop.MoveShopPermitFlags;
        foreach (var move in moves)
        {
            var index = possible.IndexOf(move);
            if (index == -1)
                continue;
            if (!permit[index])
                continue;
            SetMasteredFlag(shop, learn, mastery, level, index, move);
        }
    }

    public static void SetMasteredFlag(this IMoveShop8Mastery shop, Learnset learn, Learnset mastery, int level, int index, ushort move)
    {
        if (shop.GetMasteredRecordFlag(index))
            return;

        if (level < (uint)learn.GetMoveLevel(move)) // Can't learn it yet; must purchase.
        {
            shop.SetPurchasedRecordFlag(index, true);
            shop.SetMasteredRecordFlag(index, true);
            return;
        }

        if (level < (uint)mastery.GetMoveLevel(move)) // Can't master it yet; must Seed of Mastery
            shop.SetMasteredRecordFlag(index, true);
    }

    public static void SetEncounterMasteryFlags(this IMoveShop8Mastery shop, ReadOnlySpan<ushort> moves, Learnset mastery, int level)
    {
        var possible = shop.MoveShopPermitIndexes;
        var permit = shop.MoveShopPermitFlags;
        foreach (var move in moves)
        {
            var index = possible.IndexOf(move);
            if (index == -1)
                continue;
            if (!permit[index])
                continue;

            // If the Pokémon is caught with any move shop move in its learnset
            // and it is high enough level to master it, the game will automatically
            // give it the "Mastered" flag but not the "Purchased" flag
            // For moves that are not in the learnset, it returns -1 which is true, thus set as mastered.
            if (level >= mastery.GetMoveLevel(move))
                shop.SetMasteredRecordFlag(index, true);
        }
    }
}
