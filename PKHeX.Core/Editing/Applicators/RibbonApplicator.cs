using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for applying ribbons.
/// </summary>
public static class RibbonApplicator
{
    /// <summary>
    /// Sets all valid ribbons to the <see cref="pk"/>.
    /// </summary>
    /// <param name="pk">Entity to set ribbons for.</param>
    public static void SetAllValidRibbons(PKM pk) => SetAllValidRibbons(new LegalityAnalysis(pk));

    /// <inheritdoc cref="SetAllValidRibbons(PKM)"/>
    public static void SetAllValidRibbons(LegalityAnalysis la)
    {
        var pk = la.Entity;
        var args = new RibbonVerifierArguments(pk, la.EncounterMatch, la.Info.EvoChainsAllGens);
        SetAllRibbonState(args, true);
        FixInvalidRibbons(args);

        if (la.Entity.IsEgg)
            return;

        if (pk is IRibbonSetCommon6 c6)
        {
            // Medal Deadlock
            if (pk is ISuperTrain s && la.Info.EvoChainsAllGens.HasVisitedGen6)
            {
                s.SuperTrainBitFlags = RibbonRules.SetSuperTrainSupremelyTrained(s.SuperTrainBitFlags);
                if (pk.Format == 6) // cleared on 6->7 transfer; only set in Gen6.
                {
                    s.SecretSuperTrainingUnlocked = true;
                    s.SuperTrainSupremelyTrained = true;
                }
                c6.RibbonTraining = true;
            }
            // Ribbon Deadlock
            InvertDeadlockContest(c6, true);
        }
    }

    /// <summary>
    /// Sets all valid ribbons to the <see cref="pk"/>.
    /// </summary>
    /// <param name="pk">Entity to set ribbons for.</param>
    public static void RemoveAllValidRibbons(PKM pk) => RemoveAllValidRibbons(new LegalityAnalysis(pk));

    /// <inheritdoc cref="RemoveAllValidRibbons(PKM)"/>
    public static void RemoveAllValidRibbons(LegalityAnalysis la)
    {
        var args = new RibbonVerifierArguments(la.Entity, la.EncounterMatch, la.Info.EvoChainsAllGens);
        SetAllRibbonState(args, false);
        FixInvalidRibbons(args);
    }

    /// <summary>
    /// Parses the Entity for all ribbons, then fixes any ribbon that was invalid.
    /// </summary>
    public static void FixInvalidRibbons(in RibbonVerifierArguments args)
    {
        Span<RibbonResult> result = stackalloc RibbonResult[RibbonVerifier.MaxRibbonCount];
        var count = RibbonVerifier.GetRibbonResults(args, result);
        foreach (var ribbon in result[..count])
            ribbon.Fix(args);
    }

    private static void SetAllRibbonState(in RibbonVerifierArguments args, bool desiredState)
    {
        for (RibbonIndex3 r = 0; r < RibbonIndex3.MAX_COUNT; r++)
            r.Fix(args, desiredState);
        for (RibbonIndex4 r = 0; r < RibbonIndex4.MAX_COUNT; r++)
            r.Fix(args, desiredState);

        if (desiredState)
        {
            // Skip personality marks (Encounter specific, never required); don't set them.
            for (RibbonIndex r = 0; r <= RibbonIndex.MasterRank; r++)
                r.Fix(args, desiredState);
            for (RibbonIndex r = RibbonIndex.Hisui; r < RibbonIndex.MAX_COUNT; r++)
                r.Fix(args, desiredState);
        }
        else
        {
            // Remove Marks too.
            for (RibbonIndex r = 0; r < RibbonIndex.MAX_COUNT; r++)
                r.Fix(args, desiredState);
        }
    }

    private static void InvertDeadlockContest(IRibbonSetCommon6 c6, bool desiredState)
    {
        // Contest Star is a deadlock ribbon with the Master ribbons, as it needs all five Master ribbons to be true.
        if (desiredState)
            c6.RibbonContestStar = c6.HasAllContestRibbons();
    }
}
