using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using PKHeX.Core.Searching;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core
{
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

        private readonly bool[] CloneFlags;

        public BulkAnalysis(SaveFile sav)
        {
            Trainer = sav;
            var list = new List<SlotCache>(sav.BoxSlotCount + (sav.HasParty ? 6 : 0) + 5);
            SlotInfoLoader.AddFromSaveFile(sav, list);
            list.RemoveAll(IsEmptyData);
            AllData = list;
            AllAnalysis = GetIndividualAnalysis(AllData);
            CloneFlags = new bool[AllData.Count];

            Valid = ScanAll();
        }

        // Remove things that aren't actual stored data, or already flagged by legality checks.
        private static bool IsEmptyData(SlotCache obj)
        {
            var pkm = obj.Entity;
            if ((uint)(pkm.Species - 1) >= pkm.MaxSpeciesID)
                return true;
            if (!pkm.ChecksumValid)
                return true;
            return false;
        }

        private bool ScanAll()
        {
            CheckClones();
            if (Trainer.Generation <= 2)
                return Parse.All(z => z.Valid);

            CheckIDReuse();
            CheckPIDReuse();
            if (Trainer.Generation >= 6)
                CheckECReuse();

            if (Trainer.Generation >= 8)
                CheckHOMETrackerReuse();

            CheckDuplicateOwnedGifts();
            return Parse.All(z => z.Valid);
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
                            AddLine(cs, clone!, "Clone detected (Duplicate Tracker).", Encounter);
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
                AddLine(ps!, cs, "Clone detected (Details).", Encounter);
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
                var tidGroup = dupe.GroupBy(z => z.Slot.Entity.TID | (z.Slot.Entity.SID << 16))
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
                if (!dict.TryGetValue(id, out var pa) || pa is null)
                {
                    dict.Add(id, cr);
                    continue;
                }
                VerifyECShare(pa, cr);
            }
        }

        private void CheckHOMETrackerReuse()
        {
            var dict = new Dictionary<ulong, SlotCache>();
            for (int i = 0; i < AllData.Count; i++)
            {
                if (CloneFlags[i])
                    continue; // already flagged
                var cp = AllData[i];
                Debug.Assert(cp.Entity.Format >= 8);
                Debug.Assert(cp.Entity is IHomeTrack);
                var id = ((IHomeTrack)cp.Entity).Tracker;

                if (id == 0)
                    continue;

                if (!dict.TryGetValue(id, out var ps) || ps is null)
                {
                    dict.Add(id, cp);
                    continue;
                }
                AddLine(ps, cp, "HOME Tracker sharing detected.", Misc);
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
                if (!dict.TryGetValue(id, out var pr) || pr is null)
                {
                    dict.Add(id, cr);
                    continue;
                }
                VerifyPIDShare(pr, cr);
            }
        }

        private sealed record CombinedReference(SlotCache Slot, LegalityAnalysis Analysis);

        private void CheckIDReuse()
        {
            var dict = new Dictionary<int, CombinedReference>();
            for (int i = 0; i < AllData.Count; i++)
            {
                if (CloneFlags[i])
                    continue; // already flagged
                var cs = AllData[i];
                var ca = AllAnalysis[i];
                var id = cs.Entity.TID + (cs.Entity.SID << 16);
                Debug.Assert(cs.Entity.TID <= ushort.MaxValue);

                if (!dict.TryGetValue(id, out var pr) || pr is null)
                {
                    var r = new CombinedReference(cs, ca);
                    dict.Add(id, r);
                    continue;
                }

                var pa = pr.Analysis;
                // ignore GB era collisions
                // a 16bit TID can reasonably occur for multiple trainers, and versions 
                if (ca.Info.Generation <= 2 && pa.Info.Generation <= 2)
                    continue;

                var ps = pr.Slot;
                if (VerifyIDReuse(ps, pa, cs, ca))
                    continue;

                // egg encounters can be traded before hatching
                // store the current loop pkm if it's a better reference
                if (ps.Entity.WasTradedEgg && !cs.Entity.WasTradedEgg)
                    dict[id] = new CombinedReference(cs, ca);
            }
        }

        private void VerifyECShare(CombinedReference pr, CombinedReference cr)
        {
            var ps = pr.Slot;
            var pa = pr.Analysis;
            var cs = cr.Slot;
            var ca = cr.Analysis;

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

            // 32bit ID-SID should only occur for one generation
            // Trainer-ID-SID should only occur for one version
            if (IsSharedVersion(pp, pa, cp, ca))
            {
                AddLine(ps, cs, "TID sharing across versions detected.", ident);
                return true;
            }

            // ID-SID should only occur for one Trainer name
            if (pp.OT_Name != cp.OT_Name)
            {
                var severity = ca.Info.Generation == 4 ? Severity.Fishy : Severity.Invalid;
                AddLine(ps, cs, "TID sharing across different trainer names detected.", ident, severity);
            }

            return false;
        }

        private static bool IsSharedVersion(PKM pp, LegalityAnalysis pa, PKM cp, LegalityAnalysis ca)
        {
            if (pp.Version == cp.Version)
                return false;

            // Traded eggs retain the original version ID, only on the same generation
            if (pa.Info.Generation != ca.Info.Generation)
                return false;

            if (pa.EncounterMatch.EggEncounter && pp.WasTradedEgg)
                return false; // version doesn't update on trade
            if (ca.EncounterMatch.EggEncounter && cp.WasTradedEgg)
                return false; // version doesn't update on trade

            return true;
        }

        private static IReadOnlyList<LegalityAnalysis> GetIndividualAnalysis(IReadOnlyList<SlotCache> pkms)
        {
            var results = new LegalityAnalysis[pkms.Count];
            for (int i = 0; i < pkms.Count; i++)
            {
                var entry = pkms[i];
                var pk = entry.Entity;
                results[i] = new LegalityAnalysis(pk);
            }
            return results;
        }
    }
}
