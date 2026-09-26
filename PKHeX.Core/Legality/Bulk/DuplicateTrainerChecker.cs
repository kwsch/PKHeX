using System;
using System.Collections.Generic;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core.Bulk;

/// <summary>
/// Checks for duplicate Trainer IDs among Pokémon in a bulk legality analysis.
/// </summary>
public sealed class DuplicateTrainerChecker : IBulkAnalyzer
{
    /// <summary>
    /// Analyzes the provided <see cref="BulkAnalysis"/> for duplicate Trainer IDs.
    /// </summary>
    /// <param name="input">The bulk analysis data to check.</param>
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
            var cr = new CombinedReference(cs, ca, i);
            Verify(input, dict, cr);
        }
    }

    private static void Verify(BulkAnalysis input, Dictionary<uint, CombinedReference> dict, CombinedReference cr)
    {
        var ca = cr.Analysis;
        var cs = cr.Slot;
        var id = cs.Entity.ID32;

        if (!dict.TryGetValue(id, out var pr))
        {
            dict.Add(id, cr);
            return;
        }

        var pa = pr.Analysis;
        // ignore GB era collisions
        // a 16bit TID16 can reasonably occur for multiple trainers, and versions 
        if (ca.Info.Generation <= 2 && pa.Info.Generation <= 2)
            return;

        if (VerifyIDReuse(input, pr, cr))
            return;

        // egg encounters can be traded before hatching
        // store the current loop pk if it's a better reference
        var ps = pr.Slot;
        if (ps.Entity.WasTradedEgg && !ca.Entity.WasTradedEgg)
            dict[id] = cr;
    }

    private static bool VerifyIDReuse(BulkAnalysis input, CombinedReference pr, CombinedReference cr)
    {
        var pa = pr.Analysis;
        var ps = pr.Slot;
        var ca = cr.Analysis;
        var cs = cr.Slot;
        if (IsNotPlayerDetails(pa.EncounterMatch) || IsNotPlayerDetails(ca.EncounterMatch))
            return false;

        const CheckIdentifier ident = Trainer;

        var pp = ps.Entity;
        var cp = cs.Entity;

        // 32bit ID-SID16 should only occur for one generation
        // Trainer-ID-SID16 should only occur for one version
        if (IsSharedVersion(pp, pa, cp, ca))
        {
            input.AddLine(ps, cs, ident, pr.Index, cr.Index, LegalityCheckResultCode.BulkSharingTrainerVersion);
            return true;
        }

        // ID-SID16 should only occur for one Trainer name
        if (IsSharedTrainerName(pp, cp))
        {
            var severity = ca.Info.Generation == 4 ? Severity.Fishy : Severity.Invalid;
            input.AddLine(ps, cs, ident, pr.Index, cr.Index, LegalityCheckResultCode.BulkSharingTrainerIDs, s: severity);
            return true;
        }

        return false;
    }

    private static bool IsSharedTrainerName(PKM pp, PKM cp)
    {
        if (!IsTrainerNameMatch(pp, cp))
            return false;

        // Gen3 Eggs can be JPN-Egg vs a not-JPN not-Egg. Need to guess at the eventual language ID. Don't bother checking all possible language IDs.
        if (IsEggMatchLanguage3(pp, cp))
            return false;
        if (IsEggMatchLanguage3(cp, pp))
            return false;

        return true;
    }

    private static bool IsTrainerNameMatch(PKM first, PKM second)
    {
        var trash1 = first.OriginalTrainerTrash;
        var trash2 = second.OriginalTrainerTrash;
        Span<char> text1 = stackalloc char[first.MaxStringLengthTrainer];
        Span<char> text2 = stackalloc char[second.MaxStringLengthTrainer];
        var len1 = first.LoadString(trash1, text1);
        var len2 = second.LoadString(trash2, text2);
        // inner method checks length equivalence immediately, no need to do it explicitly before.
        return text1[..len1].SequenceEqual(text2[..len2]);
    }

    private static bool IsEggMatchLanguage3(PKM egg, PKM notEgg)
    {
        // side games cannot receive eggs even via trade, so PK3 is the only valid type.
        if (egg is not PK3 { IsEgg: true })
            return false;
        if (notEgg.IsEgg)
            return false;

        // Reinterpret trash bytes as originating from the not-egg-entity's language.
        var language = notEgg.Language;
        var trash = egg.OriginalTrainerTrash;
        Span<char> text = stackalloc char[trash.Length];
        int len = StringConverter3.LoadString(trash, text, language);
        return text[..len].SequenceEqual(notEgg.OriginalTrainerName);
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
