using System;

namespace PKHeX.Core;

public interface IMoveShop8
{
    ReadOnlySpan<bool> MoveShopPermitFlags { get; }
    ReadOnlySpan<ushort> MoveShopPermitIndexes { get; }
    bool GetPurchasedRecordFlag(int index);
    void SetPurchasedRecordFlag(int index, bool value);
    bool GetPurchasedRecordFlagAny();
    int GetPurchasedCount();
}

public interface IMoveShop8Mastery : IMoveShop8
{
    bool GetMasteredRecordFlag(int index);
    void SetMasteredRecordFlag(int index, bool value);
    bool GetMasteredRecordFlagAny();
}

public static class MoveShop8MasteryExtensions
{
    public static bool IsValidPurchasedEncounter(this IMoveShop8 shop, Learnset learn, int level, ushort alpha, bool allowPurchasedAlpha)
    {
        var permit = shop.MoveShopPermitIndexes;
        var current = shop.MoveShopPermitFlags;
        for (int i = 0; i < current.Length; i++)
        {
            if (!current[i])
                continue;

            if (!shop.GetPurchasedRecordFlag(i))
                continue;

            var move = permit[i];

            // Can only purchase a move if it is not already in the available learnset.
            var learnLevel = learn.GetMoveLevel(move);
            if (learnLevel <= level)
                return false;

            // Can only purchase an Alpha Move if it was pre-1.1 patch.
            if (move == alpha && allowPurchasedAlpha)
                continue;

            return false;
        }

        return true;
    }

    public static bool IsValidMasteredEncounter(this IMoveShop8Mastery shop, Span<int> moves, Learnset learn, Learnset mastery, int level, ushort alpha, bool allowPurchasedAlpha)
    {
        foreach (var move in moves)
        {
            if (move == 0)
                continue;
            var index = shop.MoveShopPermitIndexes.IndexOf((ushort)move);
            if (index == -1)
                continue; // manually mastered for encounter, not a tutor

            bool p = shop.GetPurchasedRecordFlag(index);
            bool m = shop.GetMasteredRecordFlag(index);

            var masteryLevel = mastery.GetMoveLevel(move);
            if (masteryLevel > level && move != alpha) // no master flag set
            {
                if (!p && m)
                {
                    // Check for seed of mastery usage
                    if (learn.GetMoveLevel(move) > level)
                        return false;
                }
            }
            else
            {
                // Pre 1.1 patch, players could purchase the Alpha Move from the move shop.
                // After the patch, the Alpha Move is considered purchased (even without the flag).
                // Players won't be able to waste money after the patch :)
                // For legality, allow the Alpha Move to be flagged as Purchased if it was a pre-patch capture.
                if (p && (move != alpha || !allowPurchasedAlpha))
                    return false;
                if (!m)
                    return false;
            }
        }

        return true;
    }
}
