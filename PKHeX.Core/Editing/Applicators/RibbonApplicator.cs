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
    /// <returns>True if any ribbons were applied.</returns>
    public static void SetAllValidRibbons(PKM pk) => SetAllValidRibbons(new LegalityAnalysis(pk));

    /// <inheritdoc cref="SetAllValidRibbons(PKM)"/>
    public static void SetAllValidRibbons(LegalityAnalysis la)
    {
        var args = new RibbonVerifierArguments(la.Entity, la.EncounterMatch, la.Info.EvoChainsAllGens);
        SetAllRibbonState(args, true);
        FixInvalidRibbons(args);

        // Ribbon Deadlock
        if (la.Entity is IRibbonSetCommon6 c6)
            InvertDeadlockContest(c6, true, args);
    }

    /// <summary>
    /// Sets all valid ribbons to the <see cref="pk"/>.
    /// </summary>
    /// <param name="pk">Entity to set ribbons for.</param>
    /// <returns>True if any ribbons were removed.</returns>
    public static void RemoveAllValidRibbons(PKM pk) => RemoveAllValidRibbons(new LegalityAnalysis(pk));

    /// <inheritdoc cref="RemoveAllValidRibbons(PKM)"/>
    public static void RemoveAllValidRibbons(LegalityAnalysis la)
    {
        var args = new RibbonVerifierArguments(la.Entity, la.EncounterMatch, la.Info.EvoChainsAllGens);
        SetAllRibbonState(args, false);
        FixInvalidRibbons(args);
    }

    private static void FixInvalidRibbons(RibbonVerifierArguments args)
    {
        Span<RibbonResult> result = stackalloc RibbonResult[RibbonVerifier.MaxRibbonCount];
        var count = RibbonVerifier.GetRibbonResults(args, result);
        foreach (var ribbon in result[..count])
            ribbon.Fix(args);
    }

    private static void SetAllRibbonState(RibbonVerifierArguments args, bool state)
    {
        for (int i = 0; i < (int)RibbonIndex.MAX_COUNT; i++)
            new RibbonResult((RibbonIndex)i, state).Fix(args);
        for (int i = 0; i < (int)RibbonIndex3.MAX_COUNT; i++)
            new RibbonResult((RibbonIndex3)i, state).Fix(args);
        for (int i = 0; i < (int)RibbonIndex4.MAX_COUNT; i++)
            new RibbonResult((RibbonIndex4)i, state).Fix(args);
    }

    private static void InvertDeadlockContest(IRibbonSetCommon6 c6, bool desiredState, RibbonVerifierArguments args)
    {
        // RibbonContestStar depends on having all contest ribbons, and having RibbonContestStar requires all.
        // Since the above logic sets individual ribbons, we must try setting this deadlock pair manually.
        if (c6.RibbonMasterToughness == desiredState || c6.RibbonContestStar == desiredState)
            return;

        c6.RibbonMasterToughness = c6.RibbonContestStar = desiredState;
        FixInvalidRibbons(args);
    }
}
