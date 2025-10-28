using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core.Bulk;

/// <summary>
/// Checks for duplicate event egg mystery gifts among Pokémon in a bulk legality analysis.
/// </summary>
public sealed class DuplicateGiftChecker : IBulkAnalyzer
{
    /// <summary>
    /// Analyzes the provided <see cref="BulkAnalysis"/> for duplicate event egg mystery gifts.
    /// </summary>
    /// <param name="input">The bulk analysis data to check.</param>
    public void Analyze(BulkAnalysis input)
    {
        if (input.Trainer.Generation <= 2)
            return;
        CheckDuplicateOwnedGifts(input);
    }

    private static void CheckDuplicateOwnedGifts(BulkAnalysis input)
    {
        var all = input.AllData;
        var initialCapacity = all.Count / 20; // less likely to have gift eggs
        var combined = new List<CombinedReference>(initialCapacity);
        for (int i = 0; i < all.Count; i++)
        {
            var c = all[i];
            var la = input.AllAnalysis[i];
            if (!IsEventEgg(c, la))
                continue;
            combined.Add(new(c, la, i));
        }

        if (combined.Count < 2)
            return; // not enough to compare

        var dupes = combined.GroupBy(EventEggGroupKey);

        foreach (var dupe in dupes)
        {
            var tidGroup = dupe.GroupBy(z => z.Slot.Entity.ID32)
                .Select(z => z.ToList())
                .Where(z => z.Count >= 2).ToList();
            if (tidGroup.Count == 0)
                continue;

            var grp = tidGroup[0];
            var first = grp[0].Slot;
            var second = grp[1].Slot;
            input.AddLine(first, second, Encounter, grp[0].Index, grp[1].Index, LegalityCheckResultCode.BulkDuplicateMysteryGiftEggReceived);
        }
    }

    private static string EventEggGroupKey(CombinedReference z)
    {
        var enc = z.Analysis.EncounterMatch;
        if (enc is not MysteryGift mg)
            throw new Exception("Expected a mystery gift.");
        return mg.CardTitle + $"{mg.Species}"; // differentiator for duplicate named event eggs -- species should be enough?
    }

    private static bool IsEventEgg(SlotCache c, LegalityAnalysis la)
    {
        if (la.Info.Generation < 3)
            return false; // don't care
        if (la.EncounterMatch is not MysteryGift { IsEgg: true })
            return false; // only interested in ^
        return !c.Entity.WasTradedEgg;
    }
}
