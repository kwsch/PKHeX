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
    public static bool IsValidMasteredEncounter(this IMoveShop8Mastery shop, Span<int> moves, Learnset learn, Learnset mastery, int level, ushort alpha)
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
                if (p || !m)
                    return false;
            }
        }

        return true;
    }
}
