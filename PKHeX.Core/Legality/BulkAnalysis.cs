using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using PKHeX.Core.Searching;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

/// <summary>
/// Analyzes content within a <see cref="SaveFile"/> for overall <see cref="PKM"/> legality analysis.
/// </summary>
public sealed class BulkAnalysis
{
    public readonly IReadOnlyList<SlotCache> AllData;
    public readonly IReadOnlyList<LegalityAnalysis> AllAnalysis;
    public readonly ITrainerInfo Trainer;
    public readonly List<CheckResult> Parse = new();
    public readonly Dictionary<ulong, SlotCache> Trackers = new();
    public readonly bool Valid;

    private readonly IBulkAnalysisSettings Settings;
    private readonly bool[] CloneFlags;

    public BulkAnalysis(SaveFile sav, IBulkAnalysisSettings settings)
    {
        Trainer = sav;
        Settings = settings;
        var list = new List<SlotCache>(sav.BoxSlotCount + (sav.HasParty ? 6 : 0) + 5);
        SlotInfoLoader.AddFromSaveFile(sav, list);
        list.RemoveAll(IsEmptyData);
        AllData = list;
        AllAnalysis = GetIndividualAnalysis(list);
        CloneFlags = new bool[AllData.Count];

        ScanAll();
        Valid = Parse.Count == 0 || Parse.All(z => z.Valid);
    }

    // Remove things that aren't actual stored data, or already flagged by legality checks.
    private static bool IsEmptyData(SlotCache obj)
    {
        var pk = obj.Entity;
        if ((uint)(pk.Species - 1) >= pk.MaxSpeciesID)
            return true;
        if (!pk.ChecksumValid)
            return true;
        return false;
    }

    private void ScanAll()
    {
        CheckClones();
        if (Trainer.Generation <= 2)
            return;

        CheckIDReuse();
        CheckPIDReuse();
        if (Trainer.Generation >= 6)
        {
            CheckECReuse();
            if (Settings.CheckActiveHandler)
                CheckHandlerFlag();
        }

        CheckDuplicateOwnedGifts();
    }

    private static string GetSummary(SlotCache entry) => $"[{entry.Identify()}] {entry.Entity.FileName}";

    private void AddLine(SlotCache first, SlotCache second, string msg, CheckIdentifier i, Severity s = Severity.Invalid)
    {
        var c = $"{msg}{Environment.NewLine}{GetSummary(first)}{Environment.NewLine}{GetSummary(second)}";
        var chk = new CheckResult(s, c, i);
        Parse.Add(chk);
    }

    private void AddLine(SlotCache first, string msg, CheckIdentifier i, Severity s = Severity.Invalid)
    {
        var c = $"{msg}{Environment.NewLine}{GetSummary(first)}";
        var chk = new CheckResult(s, c, i);
        Parse.Add(chk);
    }

    private void CheckClones()
    {
        var dict = new Dictionary<string, SlotCache>();
        for (int i = 0; i < AllData.Count; i++)
        {
            var cs = AllData[i];
            var ca = AllAnalysis[i];
            Debug.Assert(cs.Entity.Format == Trainer.Generation);

            // Check the upload tracker to see if there's any duplication.
            if (cs.Entity is IHomeTrack home)
            {
                if (home.Tracker != 0)
                {
                    var tracker = home.Tracker;
                    if (Trackers.TryGetValue(tracker, out var clone))
                        AddLine(cs, clone, "Clone detected (Duplicate Tracker).", Encounter);
                    else
                        Trackers.Add(tracker, cs);
                }
                else if (ca.Info.Generation is (< 8 and not -1))
                {
                    AddLine(cs, "Missing tracker.", Encounter);
                }
            }

            // Hash Details like EC/IV to see if there's any duplication.
            var identity = SearchUtil.HashByDetails(cs.Entity);
            if (!dict.TryGetValue(identity, out var ps))
            {
                dict.Add(identity, cs);
                continue;
            }

            CloneFlags[i] = true;
            AddLine(ps, cs, "Clone detected (Details).", Encounter);
        }
    }

    private void CheckDuplicateOwnedGifts()
    {
        var combined = new CombinedReference[AllData.Count];
        for (int i = 0; i < combined.Length; i++)
            combined[i] = new CombinedReference(AllData[i], AllAnalysis[i]);

        var dupes = combined.Where(z =>
                z.Analysis.Info.Generation >= 3
                && z.Analysis.EncounterMatch is MysteryGift {EggEncounter: true} && !z.Slot.Entity.WasTradedEgg)
            .GroupBy(z => ((MysteryGift)z.Analysis.EncounterMatch).CardTitle);

        foreach (var dupe in dupes)
        {
            var tidGroup = dupe.GroupBy(z => z.Slot.Entity.TID16 | (z.Slot.Entity.SID16 << 16))
                .Select(z => z.ToList())
                .Where(z => z.Count >= 2).ToList();
            if (tidGroup.Count == 0)
                continue;

            var first = tidGroup[0][0].Slot;
            var second = tidGroup[0][1].Slot;
            AddLine(first, second, $"Receipt of the same egg mystery gifts detected: {dupe.Key}", Encounter);
        }
    }

    private void CheckECReuse()
    {
        var dict = new Dictionary<uint, CombinedReference>();
        for (int i = 0; i < AllData.Count; i++)
        {
            if (CloneFlags[i])
                continue; // already flagged
            var cp = AllData[i];
            var ca = AllAnalysis[i];
            Debug.Assert(cp.Entity.Format >= 6);
            var id = cp.Entity.EncryptionConstant;

            var cr = new CombinedReference(cp, ca);
            if (!dict.TryGetValue(id, out var pa))
            {
                dict.Add(id, cr);
                continue;
            }
            VerifyECShare(pa, cr);
        }
    }

    private void CheckPIDReuse()
    {
        var dict = new Dictionary<uint, CombinedReference>();
        for (int i = 0; i < AllData.Count; i++)
        {
            if (CloneFlags[i])
                continue; // already flagged
            var cp = AllData[i];
            var ca = AllAnalysis[i];
            bool g345 = ca.Info.Generation is 3 or 4 or 5;
            var id = g345 ? cp.Entity.EncryptionConstant : cp.Entity.PID;

            var cr = new CombinedReference(cp, ca);
            if (!dict.TryGetValue(id, out var pr))
            {
                dict.Add(id, cr);
                continue;
            }
            VerifyPIDShare(pr, cr);
        }
    }

    private void CheckHandlerFlag()
    {
        for (var i = 0; i < AllData.Count; i++)
        {
            if (!AllAnalysis[i].Valid)
                continue;
            var cs = AllData[i];
            var pk = cs.Entity;
            var tr = cs.SAV;
            var withOT = tr.IsFromTrainer(pk);
            var flag = pk.CurrentHandler;
            var expect = withOT ? 0 : 1;
            if (flag != expect)
                AddLine(cs, LegalityCheckStrings.LTransferCurrentHandlerInvalid, CheckIdentifier.Trainer);

            if (flag == 1)
            {
                if (pk.HT_Name != tr.OT)
                    AddLine(cs, LegalityCheckStrings.LTransferHTMismatchName, CheckIdentifier.Trainer);
                if (pk is IHandlerLanguage h && h.HT_Language != tr.Language)
                    AddLine(cs, LegalityCheckStrings.LTransferHTMismatchLanguage, CheckIdentifier.Trainer);
            }
        }
    }

    private sealed record CombinedReference(SlotCache Slot, LegalityAnalysis Analysis);

    private void CheckIDReuse()
    {
        var dict = new Dictionary<uint, CombinedReference>();
        for (int i = 0; i < AllData.Count; i++)
        {
            if (CloneFlags[i])
                continue; // already flagged
            var cs = AllData[i];
            var ca = AllAnalysis[i];
            var id = cs.Entity.ID32;

            if (!dict.TryGetValue(id, out var pr))
            {
                var r = new CombinedReference(cs, ca);
                dict.Add(id, r);
                continue;
            }

            var pa = pr.Analysis;
            // ignore GB era collisions
            // a 16bit TID16 can reasonably occur for multiple trainers, and versions 
            if (ca.Info.Generation <= 2 && pa.Info.Generation <= 2)
                continue;

            var ps = pr.Slot;
            if (VerifyIDReuse(ps, pa, cs, ca))
                continue;

            // egg encounters can be traded before hatching
            // store the current loop pk if it's a better reference
            if (ps.Entity.WasTradedEgg && !cs.Entity.WasTradedEgg)
                dict[id] = new CombinedReference(cs, ca);
        }
    }

    private void VerifyECShare(CombinedReference pr, CombinedReference cr)
    {
        var (ps, pa) = pr;
        var (cs, ca) = cr;

        const CheckIdentifier ident = PID;
        int gen = pa.Info.Generation;
        bool gbaNDS = gen is 3 or 4 or 5;

        if (!gbaNDS)
        {
            if (ca.Info.Generation != gen)
            {
                AddLine(ps, cs, "EC sharing across generations detected.", ident);
                return;
            }
            AddLine(ps, cs, "EC sharing for 3DS-onward origin detected.", ident);
            return;
        }

        // eggs/mystery gifts shouldn't share with wild encounters
        var cenc = ca.Info.EncounterMatch;
        bool eggMysteryCurrent = cenc is EncounterEgg or MysteryGift;
        var penc = pa.Info.EncounterMatch;
        bool eggMysteryPrevious = penc is EncounterEgg or MysteryGift;

        if (eggMysteryCurrent != eggMysteryPrevious)
        {
            AddLine(ps, cs, "EC sharing across RNG encounters detected.", ident);
        }
    }

    private void VerifyPIDShare(CombinedReference pr, CombinedReference cr)
    {
        var ps = pr.Slot;
        var pa = pr.Analysis;
        var cs = cr.Slot;
        var ca = cr.Analysis;
        const CheckIdentifier ident = PID;
        int gen = pa.Info.Generation;

        if (ca.Info.Generation != gen)
        {
            AddLine(ps, cs, "PID sharing across generations detected.", ident);
            return;
        }

        bool gbaNDS = gen is 3 or 4 or 5;
        if (!gbaNDS)
        {
            AddLine(ps, cs, "PID sharing for 3DS-onward origin detected.", ident);
            return;
        }

        // eggs/mystery gifts shouldn't share with wild encounters
        var cenc = ca.Info.EncounterMatch;
        bool eggMysteryCurrent = cenc is EncounterEgg or MysteryGift;
        var penc = pa.Info.EncounterMatch;
        bool eggMysteryPrevious = penc is EncounterEgg or MysteryGift;

        if (eggMysteryCurrent != eggMysteryPrevious)
        {
            AddLine(ps, cs, "PID sharing across RNG encounters detected.", ident);
        }
    }

    private bool VerifyIDReuse(SlotCache ps, LegalityAnalysis pa, SlotCache cs, LegalityAnalysis ca)
    {
        if (pa.EncounterMatch is MysteryGift {EggEncounter: false})
            return false;
        if (ca.EncounterMatch is MysteryGift {EggEncounter: false})
            return false;

        const CheckIdentifier ident = CheckIdentifier.Trainer;

        var pp = ps.Entity;
        var cp = cs.Entity;

        // 32bit ID-SID16 should only occur for one generation
        // Trainer-ID-SID16 should only occur for one version
        if (IsSharedVersion(pp, pa, cp, ca))
        {
            AddLine(ps, cs, "TID16 sharing across versions detected.", ident);
            return true;
        }

        // ID-SID16 should only occur for one Trainer name
        if (pp.OT_Name != cp.OT_Name)
        {
            var severity = ca.Info.Generation == 4 ? Severity.Fishy : Severity.Invalid;
            AddLine(ps, cs, "TID16 sharing across different trainer names detected.", ident, severity);
        }

        return false;
    }

    private static bool IsSharedVersion(PKM pp, LegalityAnalysis pa, PKM cp, LegalityAnalysis ca)
    {
        if (pp.Version == cp.Version || pp.Version == 0 || cp.Version == 0)
            return false;

        // Traded eggs retain the original version ID, only on the same generation
        if (pa.Info.Generation != ca.Info.Generation)
            return false;

        // Gen3/4 traded eggs do not have an Egg Location, and do not update the Version upon hatch.
        // These eggs can obtain another trainer's TID16/SID16/OT and be valid with a different version ID.
        if (pa.EncounterMatch.EggEncounter && IsTradedEggVersionNoUpdate(pp, pa))
            return false; // version doesn't update on trade
        if (ca.EncounterMatch.EggEncounter && IsTradedEggVersionNoUpdate(cp, ca))
            return false; // version doesn't update on trade

        static bool IsTradedEggVersionNoUpdate(PKM pk, LegalityAnalysis la) => la.Info.Generation switch
        {
            3 => true, // No egg location, assume can be traded. Doesn't update version upon hatch.
            4 => pk.WasTradedEgg, // Gen4 traded eggs do not update version upon hatch.
            _ => false, // Gen5+ eggs have an egg location, and update the version upon hatch.
        };

        return true;
    }

    private static IReadOnlyList<LegalityAnalysis> GetIndividualAnalysis(IReadOnlyList<SlotCache> pkms)
    {
        var results = new LegalityAnalysis[pkms.Count];
        for (int i = 0; i < pkms.Count; i++)
            results[i] = Get(pkms[i]);
        return results;
    }

    private static LegalityAnalysis Get(SlotCache cache) => new(cache.Entity, cache.SAV.Personal, cache.Source.Origin);
}
