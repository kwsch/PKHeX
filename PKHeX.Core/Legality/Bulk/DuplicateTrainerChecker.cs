using System.Collections.Generic;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core.Bulk;

public sealed class DuplicateTrainerChecker : IBulkAnalyzer
{
    public void Analyze(BulkAnalysis input) => CheckIDReuse(input);

    private static void CheckIDReuse(BulkAnalysis input)
    {
        var dict = new Dictionary<uint, CombinedReference>();
        for (int i = 0; i < input.AllData.Count; i++)
        {
            if (input.GetIsClone(i))
                continue; // already flagged
            var cs = input.AllData[i];
            var ca = input.AllAnalysis[i];
            Verify(input, dict, cs, ca);
        }
    }

    private static void Verify(BulkAnalysis input, Dictionary<uint, CombinedReference> dict, SlotCache cs, LegalityAnalysis ca)
    {
        var id = cs.Entity.ID32;

        if (!dict.TryGetValue(id, out var pr))
        {
            var r = new CombinedReference(cs, ca);
            dict.Add(id, r);
            return;
        }

        var pa = pr.Analysis;
        // ignore GB era collisions
        // a 16bit TID16 can reasonably occur for multiple trainers, and versions 
        if (ca.Info.Generation <= 2 && pa.Info.Generation <= 2)
            return;

        var ps = pr.Slot;
        if (VerifyIDReuse(input, ps, pa, cs, ca))
            return;

        // egg encounters can be traded before hatching
        // store the current loop pk if it's a better reference
        if (ps.Entity.WasTradedEgg && !cs.Entity.WasTradedEgg)
            dict[id] = new CombinedReference(cs, ca);
    }

    private static bool VerifyIDReuse(BulkAnalysis input, SlotCache ps, LegalityAnalysis pa, SlotCache cs, LegalityAnalysis ca)
    {
        if (IsNotPlayerDetails(pa.EncounterMatch) || IsNotPlayerDetails(ca.EncounterMatch))
            return false;

        const CheckIdentifier ident = Trainer;

        var pp = ps.Entity;
        var cp = cs.Entity;

        // 32bit ID-SID16 should only occur for one generation
        // Trainer-ID-SID16 should only occur for one version
        if (IsSharedVersion(pp, pa, cp, ca))
        {
            input.AddLine(ps, cs, "TID sharing across versions detected.", ident);
            return true;
        }

        // ID-SID16 should only occur for one Trainer name
        if (pp.OriginalTrainerName != cp.OriginalTrainerName)
        {
            var severity = ca.Info.Generation == 4 ? Severity.Fishy : Severity.Invalid;
            input.AddLine(ps, cs, "TID sharing across different trainer names detected.", ident, severity);
        }

        return false;
    }

    private static bool IsNotPlayerDetails(IEncounterTemplate enc) => enc switch
    {
        IFixedTrainer { IsFixedTrainer: true } => true,
        MysteryGift { IsEgg: false } => true,
        _ => false,
    };

    private static bool IsSharedVersion(PKM pp, LegalityAnalysis pa, PKM cp, LegalityAnalysis ca)
    {
        if (pp.Version == cp.Version || pp.Version == 0 || cp.Version == 0)
            return false;

        // Traded eggs retain the original version ID, only on the same generation
        if (pa.Info.Generation != ca.Info.Generation)
            return false;

        // Gen3/4 traded eggs do not have an Egg Location, and do not update the Version upon hatch.
        // These eggs can obtain another trainer's TID16/SID16/OT and be valid with a different version ID.
        if (pa.EncounterMatch.IsEgg && IsTradedEggVersionNoUpdate(pp, pa))
            return false; // version doesn't update on trade
        if (ca.EncounterMatch.IsEgg && IsTradedEggVersionNoUpdate(cp, ca))
            return false; // version doesn't update on trade

        static bool IsTradedEggVersionNoUpdate(PKM pk, LegalityAnalysis la) => la.Info.Generation switch
        {
            2 => true, // No version stored, just ignore.
            3 => true, // No egg location, assume can be traded. Doesn't update version upon hatch.
            4 => pk.WasTradedEgg, // Gen4 traded eggs do not update version upon hatch.
            _ => false, // Gen5+ eggs have an egg location, and update the version upon hatch.
        };

        return true;
    }
}
