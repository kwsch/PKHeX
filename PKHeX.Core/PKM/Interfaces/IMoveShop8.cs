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
    public static bool IsValidMasteredEncounterOnly(this IMoveShop8Mastery p, Span<int> moves)
    {
        // If any are purchased, all must be purchased if mastered.
        // If none are purchased, all or none must be mastered.
        bool purchased = IsPurchasedEncounterMove(p, moves);

        bool any = false;
        foreach (var move in moves)
        {
            if (move == 0)
                continue;
            var index = p.MoveShopPermitIndexes.IndexOf((ushort)move);
            if (index == -1)
                continue; // manually mastered for encounter, not a tutor

            bool b = p.GetPurchasedRecordFlag(index);
            bool m = p.GetMasteredRecordFlag(index);
            if (purchased && m && !b)
                return false;
            if (any && !m)
                return false;
            any |= m;
        }

        return true;
    }

    private static bool IsPurchasedEncounterMove(IMoveShop8 p, Span<int> moves)
    {
        foreach (var move in moves)
        {
            if (move == 0)
                continue;
            var index = p.MoveShopPermitIndexes.IndexOf((ushort)move);
            if (index == -1)
                continue; // manually mastered for encounter, not a tutor

            if (p.GetPurchasedRecordFlag(index))
                return true;
        }

        return false;
    }
}
