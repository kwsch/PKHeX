using System.Linq;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core.Bulk;

public sealed class DuplicateGiftChecker : IBulkAnalyzer
{
    public void Analyze(BulkAnalysis input)
    {
        if (input.Trainer.Generation <= 2)
            return;
        CheckDuplicateOwnedGifts(input);
    }

    private static void CheckDuplicateOwnedGifts(BulkAnalysis input)
    {
        var all = input.AllData;
        var combined = new CombinedReference[all.Count];
        for (int i = 0; i < combined.Length; i++)
            combined[i] = new CombinedReference(all[i], input.AllAnalysis[i]);

        var dupes = combined.Where(z =>
                z.Analysis.Info.Generation >= 3
                && z.Analysis.EncounterMatch is MysteryGift { IsEgg: true } && !z.Slot.Entity.WasTradedEgg)
            .GroupBy(z => ((MysteryGift)z.Analysis.EncounterMatch).CardTitle);

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
            input.AddLine(first, second, $"Receipt of the same egg mystery gifts detected: {dupe.Key}", Encounter);
        }
    }
}
