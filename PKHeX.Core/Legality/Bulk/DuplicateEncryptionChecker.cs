using System.Collections.Generic;
using System.Diagnostics;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core.Bulk;

public sealed class DuplicateEncryptionChecker : IBulkAnalyzer
{
    public void Analyze(BulkAnalysis input)
    {
        if (input.Trainer.Generation < 6)
            return; // no EC yet
        CheckECReuse(input);
    }

    private static void CheckECReuse(BulkAnalysis input)
    {
        var dict = new Dictionary<uint, CombinedReference>();
        for (int i = 0; i < input.AllData.Count; i++)
        {
            if (input.GetIsClone(i))
                continue; // already flagged
            var cp = input.AllData[i];
            var ca = input.AllAnalysis[i];
            Verify(input, dict, cp, ca);
        }
    }

    private static void Verify(BulkAnalysis input, Dictionary<uint, CombinedReference> dict, SlotCache cp, LegalityAnalysis ca)
    {
        Debug.Assert(cp.Entity.Format >= 6);
        var id = cp.Entity.EncryptionConstant;

        var cr = new CombinedReference(cp, ca);
        if (!dict.TryGetValue(id, out var pa))
        {
            dict.Add(id, cr);
            return;
        }

        VerifyECShare(input, pa, cr);
    }

    private static void VerifyECShare(BulkAnalysis input, CombinedReference pr, CombinedReference cr)
    {
        var (ps, pa) = pr;
        var (cs, ca) = cr;

        const CheckIdentifier ident = PID;
        var gen = pa.Info.Generation;
        bool gbaNDS = gen is 3 or 4 or 5;

        if (!gbaNDS)
        {
            if (ca.Info.Generation != gen)
            {
                input.AddLine(ps, cs, "EC sharing across generations detected.", ident);
                return;
            }
            input.AddLine(ps, cs, "EC sharing for 3DS-onward origin detected.", ident);
            return;
        }

        // eggs/mystery gifts shouldn't share with wild encounters
        var cenc = ca.Info.EncounterMatch;
        bool eggMysteryCurrent = cenc is EncounterEgg or MysteryGift;
        var penc = pa.Info.EncounterMatch;
        bool eggMysteryPrevious = penc is EncounterEgg or MysteryGift;

        if (eggMysteryCurrent != eggMysteryPrevious)
        {
            input.AddLine(ps, cs, "EC sharing across RNG encounters detected.", ident);
        }
    }
}
