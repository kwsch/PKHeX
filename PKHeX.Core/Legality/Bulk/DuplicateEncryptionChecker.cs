using System.Collections.Generic;
using System.Diagnostics;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core.Bulk;

/// <summary>
/// Checks for duplicate Encryption Constants (EC) among Pokémon in a bulk legality analysis.
/// </summary>
public sealed class DuplicateEncryptionChecker : IBulkAnalyzer
{
    /// <summary>
    /// Analyzes the provided <see cref="BulkAnalysis"/> for duplicate Encryption Constants.
    /// </summary>
    /// <param name="input">The bulk analysis data to check.</param>
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
            var cr = new CombinedReference(cp, ca, i);
            Verify(input, dict, cr);
        }
    }

    private static void Verify(BulkAnalysis input, Dictionary<uint, CombinedReference> dict, CombinedReference cr)
    {
        var cp = cr.Analysis;

        Debug.Assert(cp.Entity.Format >= 6);
        var id = cp.Entity.EncryptionConstant;

        if (!dict.TryGetValue(id, out var pa))
        {
            dict.Add(id, cr);
            return;
        }

        VerifyECShare(input, pa, cr);
    }

    private static void VerifyECShare(BulkAnalysis input, CombinedReference pr, CombinedReference cr)
    {
        var (ps, pa, index1) = pr;
        var (cs, ca, index2) = cr;

        const CheckIdentifier ident = PID;
        var gen = pa.Info.Generation;
        bool gbaNDS = gen is 3 or 4 or 5;

        if (!gbaNDS)
        {
            if (ca.Info.Generation != gen)
            {
                input.AddLine(ps, cs, ident, index1, index2, LegalityCheckResultCode.BulkSharingEncryptionConstantGenerationDifferent);
                return;
            }
            input.AddLine(ps, cs, ident, index1, index2, LegalityCheckResultCode.BulkSharingEncryptionConstantGenerationSame);
            return;
        }

        // eggs/mystery gifts shouldn't share with wild encounters
        var cenc = ca.Info.EncounterMatch;
        bool eggMysteryCurrent = cenc is IEncounterEgg or MysteryGift;
        var penc = pa.Info.EncounterMatch;
        bool eggMysteryPrevious = penc is IEncounterEgg or MysteryGift;

        if (eggMysteryCurrent != eggMysteryPrevious)
        {
            input.AddLine(ps, cs, ident, index1, index2, LegalityCheckResultCode.BulkSharingEncryptionConstantEncounterType);
        }
    }
}
