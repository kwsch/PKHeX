using System.Collections.Generic;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core.Bulk;

/// <summary>
/// Checks for duplicate PIDs among Pokémon in a bulk legality analysis.
/// </summary>
public sealed class DuplicatePIDChecker : IBulkAnalyzer
{
    /// <summary>
    /// Analyzes the provided <see cref="BulkAnalysis"/> for duplicate PIDs.
    /// </summary>
    /// <param name="input">The bulk analysis data to check.</param>
    public void Analyze(BulkAnalysis input)
    {
        if (input.Trainer.Generation < 3)
            return; // no PID yet
        CheckPIDReuse(input);
    }

    private static void CheckPIDReuse(BulkAnalysis input)
    {
        var dict = new Dictionary<uint, CombinedReference>();
        for (int i = 0; i < input.AllData.Count; i++)
        {
            if (input.GetIsClone(i))
                continue; // already flagged
            var cp = input.AllData[i];
            var ca = input.AllAnalysis[i];
            var cr = new CombinedReference(cp, ca, i);
            Verify(input, dict, cr);
        }
    }

    private static void Verify(BulkAnalysis input, Dictionary<uint, CombinedReference> dict, CombinedReference cr)
    {
        var ca = cr.Analysis;
        var cp = cr.Slot;
        bool g345 = ca.Info.Generation is 3 or 4 or 5;
        var id = g345 ? cp.Entity.EncryptionConstant : cp.Entity.PID;

        if (!dict.TryGetValue(id, out var pr))
        {
            dict.Add(id, cr);
            return;
        }

        VerifyPIDShare(input, pr, cr);
    }

    private static void VerifyPIDShare(BulkAnalysis input, CombinedReference pr, CombinedReference cr)
    {
        var ps = pr.Slot;
        var pa = pr.Analysis;
        var cs = cr.Slot;
        var ca = cr.Analysis;
        const CheckIdentifier ident = PID;
        var gen = pa.Info.Generation;

        if (ca.Info.Generation != gen)
        {
            input.AddLine(ps, cs, ident, pr.Index, cr.Index, LegalityCheckResultCode.BulkSharingPIDGenerationDifferent);
            return;
        }

        bool gbaNDS = gen is 3 or 4 or 5;
        if (!gbaNDS)
        {
            input.AddLine(ps, cs, ident, pr.Index, cr.Index, LegalityCheckResultCode.BulkSharingPIDGenerationSame);
            return;
        }

        // eggs/mystery gifts shouldn't share with wild encounters
        var cenc = ca.Info.EncounterMatch;
        bool eggMysteryCurrent = cenc is IEncounterEgg or MysteryGift;
        var penc = pa.Info.EncounterMatch;
        bool eggMysteryPrevious = penc is IEncounterEgg or MysteryGift;

        if (eggMysteryCurrent != eggMysteryPrevious)
            input.AddLine(ps, cs, ident, pr.Index, cr.Index, LegalityCheckResultCode.BulkSharingPIDEncounterType);
    }
}
