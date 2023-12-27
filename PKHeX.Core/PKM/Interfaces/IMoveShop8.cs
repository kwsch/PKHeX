using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes info about the Move Shop in Legends: Arceus
/// </summary>
public interface IMoveShop8
{
    IPermitRecord Permit { get; }

    /// <summary>
    /// Indicates if the move shop offering at the requested index has been purchased.
    /// </summary>
    /// <param name="index">Move shop offering</param>
    /// <returns>True if purchased</returns>
    bool GetPurchasedRecordFlag(int index);

    /// <summary>
    /// Sets the flag indicating that the move shop offering at the requested index has been purchased.
    /// </summary>
    /// <param name="index">Move shop offering</param>
    /// <param name="value">True if purchased</param>
    void SetPurchasedRecordFlag(int index, bool value);

    /// <summary>
    /// Indicates if any move has been purchased from the move shop.
    /// </summary>
    bool GetPurchasedRecordFlagAny();

    /// <summary>
    /// Gets a count of move shop flags that are present in the entity.
    /// </summary>
    int GetPurchasedCount();
}

/// <summary>
/// Exposes info about the Move Shop Mastery (via Seed of Mastery) in Legends: Arceus
/// </summary>
public interface IMoveShop8Mastery : IMoveShop8
{
    /// <summary>
    /// Indicates if the move shop offering at the requested index has been flagged as mastered.
    /// </summary>
    /// <param name="index">Move shop offering</param>
    /// <returns>True if mastered</returns>
    bool GetMasteredRecordFlag(int index);

    /// <summary>
    /// Sets the flag indicating that the move shop offering at the requested index has been flagged as mastered.
    /// </summary>
    /// <param name="index">Move shop offering</param>
    /// <param name="value">True if purchased</param>
    void SetMasteredRecordFlag(int index, bool value);

    /// <summary>
    /// Indicates if any move has been flagged as mastered.
    /// </summary>
    bool GetMasteredRecordFlagAny();
}

public static class MoveShop8MasteryExtensions
{
    public static bool IsValidPurchasedEncounter(this IMoveShop8 shop, Learnset learn, int level, ushort alpha, bool allowPurchasedAlpha)
    {
        var permit = shop.Permit.RecordPermitIndexes;
        var current = shop.Permit;
        for (int i = 0; i < current.RecordCountUsed; i++)
        {
            if (!current.IsRecordPermitted(i))
                continue;

            if (!shop.GetPurchasedRecordFlag(i))
                continue;

            var move = permit[i];

            // Can only purchase a move if it is not already in the available learnset.
            var learnLevel = learn.GetLevelLearnMove(move);
            if ((uint)learnLevel <= level)
                return false;

            // Can only purchase an Alpha Move if it was pre-1.1 patch.
            if (move == alpha && !allowPurchasedAlpha)
                return false;
        }

        return true;
    }

    public static bool IsValidMasteredEncounter(this IMoveShop8Mastery shop, Span<ushort> moves, Learnset learn, Learnset mastery, int metLevel, ushort alphaMove, bool allowPurchasedAlpha)
    {
        foreach (var move in moves)
        {
            if (move == 0)
                continue;
            var index = shop.Permit.RecordPermitIndexes.IndexOf(move);
            if (index == -1)
                continue; // manually mastered for encounter, not a tutor

            bool purchased = shop.GetPurchasedRecordFlag(index);
            bool mastered = shop.GetMasteredRecordFlag(index);

            var masteryLevel = mastery.GetLevelLearnMove(move);
            if (masteryLevel > metLevel && move != alphaMove) // no master flag set
            {
                if (!mastered)
                    continue;
                if (purchased)
                    continue;
                // Check for seed of mastery usage
                if (learn.GetLevelLearnMove(move) > metLevel)
                    return false;
            }
            else
            {
                // Pre 1.1 patch, players could purchase the Alpha Move from the move shop.
                // After the patch, the Alpha Move is considered purchased (even without the flag).
                // Players won't be able to waste money after the patch :)
                // For legality, allow the Alpha Move to be flagged as Purchased if it was a pre-patch capture.
                if (!mastered)
                    return false;
                if (purchased && (move != alphaMove || !allowPurchasedAlpha))
                    return false;
            }
        }

        return true;
    }
}
