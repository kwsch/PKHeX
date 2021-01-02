using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using PKHeX.Core.Searching;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core
{
    public sealed class BulkAnalysis
    {
        public readonly IReadOnlyList<PKM> AllData;
        public readonly IReadOnlyList<LegalityAnalysis> AllAnalysis;
        public readonly ITrainerInfo Trainer;
        public readonly List<CheckResult> Parse = new();
        public readonly Dictionary<ulong, PKM> Trackers = new();
        public readonly bool Valid;

        private readonly bool[] CloneFlags;

        public BulkAnalysis(SaveFile sav)
        {
            Trainer = sav;
            AllData = sav.GetAllPKM();
            AllAnalysis = GetIndividualAnalysis(AllData);
            CloneFlags = new bool[AllData.Count];

            Valid = ScanAll();
        }

        public BulkAnalysis(ITrainerInfo tr, IEnumerable<PKM> pkms)
        {
            Trainer = tr;
            AllData = pkms is IReadOnlyList<PKM> pk ? pk : pkms.ToList();
            AllAnalysis = GetIndividualAnalysis(AllData);
            CloneFlags = new bool[AllData.Count];

            Valid = ScanAll();
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

            CheckDuplicateOwnedGifts();
            return Parse.All(z => z.Valid);
        }

        private void AddLine(PKM first, PKM second, string msg, CheckIdentifier i, Severity s = Severity.Invalid)
        {
            static string GetSummary(PKM pk) => $"[{pk.Box:00}, {pk.Slot:00}] {pk.FileName}";

            var c = $"{msg}{Environment.NewLine}{GetSummary(first)}{Environment.NewLine}{GetSummary(second)}";
            var chk = new CheckResult(s, c, i);
            Parse.Add(chk);
        }

        private void AddLine(PKM first, string msg, CheckIdentifier i, Severity s = Severity.Invalid)
        {
            static string GetSummary(PKM pk) => $"[{pk.Box:00}, {pk.Slot:00}] {pk.FileName}";

            var c = $"{msg}{Environment.NewLine}{GetSummary(first)}";
            var chk = new CheckResult(s, c, i);
            Parse.Add(chk);
        }

        private void CheckClones()
        {
            var dict = new Dictionary<string, LegalityAnalysis>();
            for (int i = 0; i < AllData.Count; i++)
            {
                var cp = AllData[i];
                var ca = AllAnalysis[i];
                Debug.Assert(cp.Format == Trainer.Generation);

                // Check the upload tracker to see if there's any duplication.
                if (cp is IHomeTrack home)
                {
                    if (home.Tracker != 0)
                    {
                        var tracker = home.Tracker;
                        if (Trackers.TryGetValue(tracker, out var clone))
                            AddLine(clone, cp, "Clone detected (Duplicate Tracker).", Encounter);
                        else
                            Trackers.Add(tracker, cp);
                    }
                    else if (ca.Info.Generation < 8)
                    {
                        AddLine(cp, "Missing tracker.", Encounter);
                    }
                }

                // Hash Details like EC/IV to see if there's any duplication.
                var identity = SearchUtil.HashByDetails(cp);
                if (!dict.TryGetValue(identity, out var pa))
                {
                    dict.Add(identity, ca);
                    continue;
                }

                CloneFlags[i] = true;
                AddLine(pa.pkm, cp, "Clone detected (Details).", Encounter);
            }
        }

        private void CheckDuplicateOwnedGifts()
        {
            var dupes = AllAnalysis.Where(z =>
                    z.Info.Generation >= 3
                    && z.EncounterMatch is MysteryGift {EggEncounter: true} && !z.pkm.WasTradedEgg)
                .GroupBy(z => ((MysteryGift)z.EncounterMatch).CardTitle);

            foreach (var dupe in dupes)
            {
                var tidGroup = dupe.GroupBy(z => z.pkm.TID | (z.pkm.SID << 16))
                    .Select(z => z.ToList())
                    .Where(z => z.Count >= 2).ToList();
                if (tidGroup.Count == 0)
                    continue;

                AddLine(tidGroup[0][0].pkm, tidGroup[0][1].pkm, $"Receipt of the same egg mystery gifts detected: {dupe.Key}", Encounter);
            }
        }

        private void CheckECReuse()
        {
            var dict = new Dictionary<uint, LegalityAnalysis>();
            for (int i = 0; i < AllData.Count; i++)
            {
                if (CloneFlags[i])
                    continue; // already flagged
                var cp = AllData[i];
                var ca = AllAnalysis[i];
                Debug.Assert(cp.Format >= 6);
                var id = cp.EncryptionConstant;

                if (!dict.TryGetValue(id, out var pa))
                {
                    dict.Add(id, ca);
                    continue;
                }
                VerifyECShare(pa, ca);
            }
        }

        private void CheckPIDReuse()
        {
            var dict = new Dictionary<uint, LegalityAnalysis>();
            for (int i = 0; i < AllData.Count; i++)
            {
                if (CloneFlags[i])
                    continue; // already flagged
                var cp = AllData[i];
                var ca = AllAnalysis[i];
                bool g345 = ca.Info.Generation is 3 or 4 or 5;
                var id = g345 ? cp.EncryptionConstant : cp.PID;

                if (!dict.TryGetValue(id, out var pa))
                {
                    dict.Add(id, ca);
                    continue;
                }
                VerifyPIDShare(pa, ca);
            }
        }

        private void CheckIDReuse()
        {
            var dict = new Dictionary<int, LegalityAnalysis>();
            for (int i = 0; i < AllData.Count; i++)
            {
                if (CloneFlags[i])
                    continue; // already flagged
                var cp = AllData[i];
                var ca = AllAnalysis[i];
                var id = cp.TID + (cp.SID << 16);
                Debug.Assert(cp.TID <= ushort.MaxValue);

                if (!dict.TryGetValue(id, out var pa))
                {
                    dict.Add(id, ca);
                    continue;
                }

                // ignore GB era collisions
                // a 16bit TID can reasonably occur for multiple trainers, and versions 
                if (ca.Info.Generation <= 2 && pa.Info.Generation <= 2)
                    continue;

                var pp = pa.pkm;
                if (VerifyIDReuse(pp, pa, cp, ca))
                    continue;

                // egg encounters can be traded before hatching
                // store the current loop pkm if it's a better reference
                if (pp.WasTradedEgg && !cp.WasTradedEgg)
                    dict[id] = ca;
            }
        }

        private void VerifyECShare(LegalityAnalysis pa, LegalityAnalysis ca)
        {
            const CheckIdentifier ident = PID;
            int gen = pa.Info.Generation;
            bool gbaNDS = gen is 3 or 4 or 5;

            if (!gbaNDS)
            {
                if (ca.Info.Generation != gen)
                {
                    AddLine(pa.pkm, ca.pkm, "EC sharing across generations detected.", ident);
                    return;
                }
                AddLine(pa.pkm, ca.pkm, "EC sharing for 3DS-onward origin detected.", ident);
                return;
            }

            // eggs/mystery gifts shouldn't share with wild encounters
            var cenc = ca.Info.EncounterMatch;
            bool eggMysteryCurrent = cenc is EncounterEgg or MysteryGift;
            var penc = pa.Info.EncounterMatch;
            bool eggMysteryPrevious = penc is EncounterEgg or MysteryGift;

            if (eggMysteryCurrent != eggMysteryPrevious)
            {
                AddLine(pa.pkm, ca.pkm, "EC sharing across RNG encounters detected.", ident);
            }
        }

        private void VerifyPIDShare(LegalityAnalysis pa, LegalityAnalysis ca)
        {
            const CheckIdentifier ident = PID;
            int gen = pa.Info.Generation;
            if (ca.Info.Generation != gen)
            {
                AddLine(pa.pkm, ca.pkm, "PID sharing across generations detected.", ident);
                return;
            }

            bool gbaNDS = gen is 3 or 4 or 5;
            if (!gbaNDS)
            {
                AddLine(pa.pkm, ca.pkm, "PID sharing for 3DS-onward origin detected.", ident);
                return;
            }

            // eggs/mystery gifts shouldn't share with wild encounters
            var cenc = ca.Info.EncounterMatch;
            bool eggMysteryCurrent = cenc is EncounterEgg or MysteryGift;
            var penc = pa.Info.EncounterMatch;
            bool eggMysteryPrevious = penc is EncounterEgg or MysteryGift;

            if (eggMysteryCurrent != eggMysteryPrevious)
            {
                AddLine(pa.pkm, ca.pkm, "PID sharing across RNG encounters detected.", ident);
            }
        }

        private bool VerifyIDReuse(PKM pp, LegalityAnalysis pa, PKM cp, LegalityAnalysis ca)
        {
            if (pa.EncounterMatch is MysteryGift {EggEncounter: false})
                return false;
            if (ca.EncounterMatch is MysteryGift {EggEncounter: false})
                return false;

            const CheckIdentifier ident = CheckIdentifier.Trainer;

            // 32bit ID-SID should only occur for one generation
            // Trainer-ID-SID should only occur for one version
            if (IsSharedVersion(pp, pa, cp, ca))
            {
                AddLine(pa.pkm, ca.pkm, "TID sharing across versions detected.", ident);
                return true;
            }

            // ID-SID should only occur for one Trainer name
            if (pp.OT_Name != cp.OT_Name)
            {
                var severity = ca.Info.Generation == 4 ? Severity.Fishy : Severity.Invalid;
                AddLine(pa.pkm, ca.pkm, "TID sharing across different trainer names detected.", ident, severity);
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

        private static IReadOnlyList<LegalityAnalysis> GetIndividualAnalysis(IReadOnlyList<PKM> pkms)
        {
            var results = new LegalityAnalysis[pkms.Count];
            for (int i = 0; i < pkms.Count; i++)
                results[i] = new LegalityAnalysis(pkms[i]);
            return results;
        }
    }
}
