using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for modifying the Move Shop Record flags of a <see cref="PA8"/>.
/// </summary>
public static class MoveShopRecordApplicator
{
    /// <summary>
    /// Clears all the "purchased" and "mastered" move shop flags.
    /// </summary>
    public static void ClearMoveShopFlags(this IMoveShop8 shop)
    {
        var bits = shop.Permit;
        for (int i = 0; i < bits.RecordCountUsed; i++)
            shop.SetPurchasedRecordFlag(i, false);

        if (shop is IMoveShop8Mastery m)
            m.ClearMoveShopFlagsMastered();
    }

    /// <summary>
    /// Clears all the "mastered" move shop flags.
    /// </summary>
    public static void ClearMoveShopFlagsMastered(this IMoveShop8Mastery shop)
    {
        var bits = shop.Permit;
        for (int i = 0; i < bits.RecordCountUsed; i++)
            shop.SetMasteredRecordFlag(i, false);
    }

    /// <summary>
    /// Sets the required move shop flags for the requested entity.
    /// </summary>
    public static void SetMoveShopFlags(this IMoveShop8Mastery shop, PKM pk)
    {
        Span<ushort> moves = stackalloc ushort[4];
        pk.GetMoves(moves);
        shop.SetMoveShopFlags(moves, pk);
    }

    /// <summary>
    /// Sets the required move shop flags for the requested entity.
    /// </summary>
    public static void SetMoveShopFlags(this IMoveShop8Mastery shop, ReadOnlySpan<ushort> moves, PKM pk)
    {
        var (learn, mastery) = LearnSource8LA.GetLearnsetAndMastery(pk.Species, pk.Form);
        shop.SetMoveShopFlags(moves, learn, mastery, pk.CurrentLevel);
    }

    /// <summary>
    /// Sets all possible move shop flags for the requested entity.
    /// </summary>
    public static void SetMoveShopFlagsAll(this IMoveShop8Mastery shop, PKM pk)
    {
        var (learn, mastery) = LearnSource8LA.GetLearnsetAndMastery(pk.Species, pk.Form);
        shop.SetMoveShopFlagsAll(learn, mastery, pk.CurrentLevel);
    }

    /// <summary>
    /// Sets all possible move shop flags for the requested entity.
    /// </summary>
    public static void SetMoveShopFlagsAll(this IMoveShop8Mastery shop, Learnset learn, Learnset mastery, int level)
    {
        var permit = shop.Permit;
        var possible = permit.RecordPermitIndexes;
        for (int index = 0; index < permit.RecordCountUsed; index++)
        {
            var allowed = permit.IsRecordPermitted(index);
            if (!allowed)
                continue;

            var move = possible[index];
            SetMasteredFlag(shop, learn, mastery, level, index, move);
        }
    }

    /// <summary>
    /// Sets all move shop flags for the currently known moves.
    /// </summary>
    public static void SetMoveShopFlags(this IMoveShop8Mastery shop, ReadOnlySpan<ushort> moves, Learnset learn, Learnset mastery, int level)
    {
        var permit = shop.Permit;
        var possible = permit.RecordPermitIndexes;
        foreach (var move in moves)
        {
            var index = possible.IndexOf(move);
            if (index == -1)
                continue;
            if (!permit.IsRecordPermitted(index))
                continue;
            SetMasteredFlag(shop, learn, mastery, level, index, move);
        }
    }

    /// <summary>
    /// Sets the "mastered" move shop flag for the requested move.
    /// </summary>
    public static void SetMasteredFlag(this IMoveShop8Mastery shop, Learnset learn, Learnset mastery, int level, int index, ushort move)
    {
        if (shop.GetMasteredRecordFlag(index))
            return;

        if (level < (uint)learn.GetLevelLearnMove(move)) // Can't learn it yet; must purchase.
        {
            shop.SetPurchasedRecordFlag(index, true);
            shop.SetMasteredRecordFlag(index, true);
            return;
        }

        if (level < (uint)mastery.GetLevelLearnMove(move)) // Can't master it yet; must Seed of Mastery
            shop.SetMasteredRecordFlag(index, true);
    }

    /// <summary>
    /// Sets the "mastered" move shop flag for the encounter.
    /// </summary>
    public static void SetEncounterMasteryFlags(this IMoveShop8Mastery shop, ReadOnlySpan<ushort> moves, Learnset mastery, int level)
    {
        var permit = shop.Permit;
        var possible = permit.RecordPermitIndexes;
        foreach (var move in moves)
        {
            var index = possible.IndexOf(move);
            if (index == -1)
                continue;
            if (!permit.IsRecordPermitted(index))
                continue;

            // If the Pokémon is caught with any move shop move in its learnset,
            // and it is high enough level to master it, the game will automatically
            // give it the "Mastered" flag but not the "Purchased" flag
            // For moves that are not in the learnset, it returns -1 which is true, thus set as mastered.
            if (level >= mastery.GetLevelLearnMove(move))
                shop.SetMasteredRecordFlag(index, true);
        }
    }
}
