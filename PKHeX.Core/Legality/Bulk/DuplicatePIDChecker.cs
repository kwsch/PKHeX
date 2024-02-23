using System.Collections.Generic;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core.Bulk;

public sealed class DuplicatePIDChecker : IBulkAnalyzer
{
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
            Verify(input, dict, ca, cp);
        }
    }

    private static void Verify(BulkAnalysis input, Dictionary<uint, CombinedReference> dict, LegalityAnalysis ca, SlotCache cp)
    {
        bool g345 = ca.Info.Generation is 3 or 4 or 5;
        var id = g345 ? cp.Entity.EncryptionConstant : cp.Entity.PID;

        var cr = new CombinedReference(cp, ca);
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
            input.AddLine(ps, cs, "PID sharing across generations detected.", ident);
            return;
        }

        bool gbaNDS = gen is 3 or 4 or 5;
        if (!gbaNDS)
        {
            input.AddLine(ps, cs, "PID sharing for 3DS-onward origin detected.", ident);
            return;
        }

        // eggs/mystery gifts shouldn't share with wild encounters
        var cenc = ca.Info.EncounterMatch;
        bool eggMysteryCurrent = cenc is EncounterEgg or MysteryGift;
        var penc = pa.Info.EncounterMatch;
        bool eggMysteryPrevious = penc is EncounterEgg or MysteryGift;

        if (eggMysteryCurrent != eggMysteryPrevious)
        {
            input.AddLine(ps, cs, "PID sharing across RNG encounters detected.", ident);
        }
    }
}
