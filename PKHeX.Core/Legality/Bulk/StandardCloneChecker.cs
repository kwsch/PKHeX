using System.Collections.Generic;
using System.Diagnostics;
using PKHeX.Core.Searching;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core.Bulk;

/// <summary>
/// Checks for cloned Pokémon in a bulk legality analysis.
/// </summary>
public sealed class StandardCloneChecker : IBulkAnalyzer
{
    /// <summary>
    /// Analyzes the provided <see cref="BulkAnalysis"/> for cloned Pokémon.
    /// </summary>
    /// <param name="input">The bulk analysis data to check.</param>
    public void Analyze(BulkAnalysis input) => CheckClones(input);

    private static void CheckClones(BulkAnalysis input)
    {
        var dict = new Dictionary<string, (SlotCache Slot, int Index)>();
        for (int i = 0; i < input.AllData.Count; i++)
        {
            var cs = input.AllData[i];
            Debug.Assert(cs.Entity.Format == input.Trainer.Generation);

            // Check the upload tracker to see if there's any duplication.
            if (cs.Entity is IHomeTrack home)
                CheckClonedTrackerHOME(input, home, cs, i);

            // Hash Details like EC/IV to see if there's any duplication.
            var identity = SearchUtil.HashByDetails(cs.Entity);
            if (!dict.TryGetValue(identity, out var ps))
            {
                dict.Add(identity, (cs, i));
                continue;
            }

            input.SetIsClone(i, true);
            input.AddLine(ps.Slot, cs, Encounter, i, ps.Index, LegalityCheckResultCode.BulkCloneDetectedDetails);
        }
    }

    private const ulong DefaultUnsetTrackerHOME = 0ul;

    private static void CheckClonedTrackerHOME(BulkAnalysis input, IHomeTrack home, SlotCache cs, int index)
    {
        var tracker = home.Tracker;
        if (tracker != DefaultUnsetTrackerHOME)
            CheckTrackerPresent(input, cs, tracker, index);
    }

    private static void CheckTrackerPresent(BulkAnalysis input, SlotCache cs, ulong tracker, int index)
    {
        if (input.Trackers.TryGetValue(tracker, out var clone))
            input.AddLine(cs, clone.Slot, Encounter, index, clone.Index, LegalityCheckResultCode.BulkCloneDetectedTracker);
        else
            input.Trackers.Add(tracker, (cs, index));
    }
}
