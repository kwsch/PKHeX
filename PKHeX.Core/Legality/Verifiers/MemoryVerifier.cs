using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public class MemoryVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Memory;

        public override void Verify(LegalityAnalysis data)
        {
            var hist = VerifyHistory(data);
            data.AddLine(hist);
        }
        private CheckResult VerifyHistory(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var Info = data.Info;
            var EncounterMatch = data.EncounterMatch;

            if (Info.Generation < 6)
            {
                if (pkm.Format < 6)
                    return new CheckResult(Severity.Valid, V128, CheckIdentifier.History);

                if (pkm.OT_Affection != 0 && Info.Generation <= 2 || IsInvalidContestAffection(pkm))
                    return new CheckResult(Severity.Invalid, V129, CheckIdentifier.History);
                if (pkm.OT_Memory > 0 || pkm.OT_Feeling > 0 || pkm.OT_Intensity > 0 || pkm.OT_TextVar > 0)
                    return new CheckResult(Severity.Invalid, V130, CheckIdentifier.History);
            }

            if (pkm.Format >= 6 && Info.Generation != pkm.Format && pkm.CurrentHandler != 1)
                return new CheckResult(Severity.Invalid, V124, CheckIdentifier.History);

            if (pkm.HT_Gender > 1)
                return new CheckResult(Severity.Invalid, string.Format(V131, pkm.HT_Gender), CheckIdentifier.History);

            if (EncounterMatch is WC6 wc6 && wc6.OT_Name.Length > 0)
            {
                if (pkm.OT_Friendship != PersonalTable.AO[EncounterMatch.Species].BaseFriendship)
                    return new CheckResult(Severity.Invalid, V132, CheckIdentifier.History);
                if (pkm.OT_Affection != 0 && (pkm.AO || !pkm.IsUntraded) && IsInvalidContestAffection(pkm))
                    return new CheckResult(Severity.Invalid, V133, CheckIdentifier.History);
                if (pkm.CurrentHandler != 1)
                    return new CheckResult(Severity.Invalid, V134, CheckIdentifier.History);
            }
            else if (EncounterMatch is WC7 wc7 && wc7.OT_Name.Length > 0 && wc7.TID != 18075) // Ash Pikachu QR Gift doesn't set Current Handler
            {
                if (pkm.OT_Friendship != PersonalTable.USUM[EncounterMatch.Species].BaseFriendship)
                    return new CheckResult(Severity.Invalid, V132, CheckIdentifier.History);
                if (pkm.OT_Affection != 0)
                    return new CheckResult(Severity.Invalid, V133, CheckIdentifier.History);
                if (pkm.CurrentHandler != 1)
                    return new CheckResult(Severity.Invalid, V134, CheckIdentifier.History);
            }
            else if (EncounterMatch is MysteryGift mg && mg.Format < 6 && pkm.Format >= 6)
            {
                if (pkm.OT_Affection != 0 && IsInvalidContestAffection(pkm))
                    return new CheckResult(Severity.Invalid, V133, CheckIdentifier.History);
                if (pkm.CurrentHandler != 1)
                    return new CheckResult(Severity.Invalid, V134, CheckIdentifier.History);
            }

            // Geolocations
            var geo = new[]
            {
                pkm.Geo1_Country, pkm.Geo2_Country, pkm.Geo3_Country, pkm.Geo4_Country, pkm.Geo5_Country,
                pkm.Geo1_Region, pkm.Geo2_Region, pkm.Geo3_Region, pkm.Geo4_Region, pkm.Geo5_Region,
            };

            // Check sequential order (no zero gaps)
            bool geoEnd = false;
            for (int i = 0; i < 5; i++)
            {
                if (geoEnd && geo[i] != 0)
                    return new CheckResult(Severity.Invalid, V135, CheckIdentifier.History);

                if (geo[i] != 0)
                    continue;
                if (geo[i + 5] != 0)
                    return new CheckResult(Severity.Invalid, V136, CheckIdentifier.History);
                geoEnd = true;
            }
            if (pkm.Format >= 7)
                return VerifyHistory7(data, geo);

            // Determine if we should check for Handling Trainer Memories
            // A Pokémon is untraded if...
            bool untraded = pkm.HT_Name.Length == 0 || pkm.Geo1_Country == 0;
            if (EncounterMatch is MysteryGift gift)
            {
                untraded |= !pkm.WasEventEgg;
                untraded &= gift.IsEgg;
            }

            if (EncounterMatch is EncounterLink link && !link.OT)
                untraded = false;
            else if (Info.Generation < 6)
                untraded = false;

            if (untraded) // Is not Traded
            {
                if (pkm.HT_Name.Length != 0)
                    return new CheckResult(Severity.Invalid, V146, CheckIdentifier.History);
                if (pkm.Geo1_Country != 0)
                    return new CheckResult(Severity.Invalid, V147, CheckIdentifier.History);
                if (pkm.HT_Memory != 0)
                    return new CheckResult(Severity.Invalid, V148, CheckIdentifier.History);
                if (pkm.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
                    return new CheckResult(Severity.Invalid, V139, CheckIdentifier.History);
                if (pkm.HT_Friendship != 0)
                    return new CheckResult(Severity.Invalid, V140, CheckIdentifier.History);
                if (pkm.HT_Affection != 0)
                    return new CheckResult(Severity.Invalid, V141, CheckIdentifier.History);
                if (pkm.XY && pkm is IContestStats s && s.HasContestStats())
                    return new CheckResult(Severity.Invalid, V138, CheckIdentifier.History);

                if (VerifyHistoryUntradedHandler(pkm, out CheckResult chk1))
                    return chk1;
                if (EncounterMatch.Species != pkm.Species && VerifyHistoryUntradedEvolution(pkm, Info.EvoChainsAllGens, out CheckResult chk2))
                    return chk2;
            }
            else // Is Traded
            {
                if (pkm.Format == 6 && pkm.HT_Memory == 0 && !pkm.IsEgg)
                    return new CheckResult(Severity.Invalid, V150, CheckIdentifier.History);
            }

            // Memory ChecksResult
            if (pkm.IsEgg)
            {
                if (pkm.HT_Memory != 0)
                    return new CheckResult(Severity.Invalid, V149, CheckIdentifier.History);
                if (pkm.OT_Memory != 0)
                    return new CheckResult(Severity.Invalid, V151, CheckIdentifier.History);
            }
            else if (!(EncounterMatch is WC6))
            {
                if (pkm.OT_Memory == 0 ^ !pkm.Gen6)
                    return new CheckResult(Severity.Invalid, V152, CheckIdentifier.History);
                if (Info.Generation < 6 && pkm.OT_Affection != 0)
                    return new CheckResult(Severity.Invalid, V129, CheckIdentifier.History);
            }
            // Unimplemented: Ingame Trade Memories

            return new CheckResult(Severity.Valid, V145, CheckIdentifier.History);
        }
        private CheckResult VerifyHistory7(LegalityAnalysis data, int[] geo)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;
            var Info = data.Info;

            if (pkm.VC1)
            {
                var hasGeo = geo.Any(d => d != 0);

                if (!hasGeo)
                    return new CheckResult(Severity.Invalid, V137, CheckIdentifier.History);
            }

            if ((2 >= Info.Generation || Info.Generation >= 7) && pkm is IContestStats s && s.HasContestStats())
                return new CheckResult(Severity.Invalid, V138, CheckIdentifier.History);

            if (!pkm.WasEvent && pkm.HT_Name.Length == 0) // Is not Traded
            {
                if (VerifyHistoryUntradedHandler(pkm, out CheckResult chk1))
                    return chk1;
                if (EncounterMatch.Species != pkm.Species && VerifyHistoryUntradedEvolution(pkm, Info.EvoChainsAllGens, out CheckResult chk2))
                    return chk2;
            }

            return new CheckResult(Severity.Valid, V145, CheckIdentifier.History);
        }
        private static bool VerifyHistoryUntradedHandler(PKM pkm, out CheckResult result)
        {
            result = null;
            if (pkm.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
                result = new CheckResult(Severity.Invalid, V139, CheckIdentifier.History);
            else if (pkm.HT_Friendship != 0)
                result = new CheckResult(Severity.Invalid, V140, CheckIdentifier.History);
            else if (pkm.HT_Affection != 0)
                result = new CheckResult(Severity.Invalid, V141, CheckIdentifier.History);
            else
                return false;

            return true;
        }
        private static bool VerifyHistoryUntradedEvolution(PKM pkm, IReadOnlyList<EvoCriteria>[] chain, out CheckResult result)
        {
            result = null;
            // Handling Trainer string is empty implying it has not been traded.
            // If it must be trade evolved, flag it.

            if (pkm.Species == 350) // Milotic
            {
                if (Legal.IsTradeEvolved(chain, pkm.Format))
                    return false;
                if (pkm is IContestStats s && s.CNT_Beauty < 170) // Beauty Contest Stat Requirement
                    result = new CheckResult(Severity.Invalid, V143, CheckIdentifier.History);
                else if (pkm.CurrentLevel == 1)
                    result = new CheckResult(Severity.Invalid, V144, CheckIdentifier.History);
                else
                    return false;
                return true;
            }
            if (!Legal.IsTradeEvolved(chain, pkm.Format))
                return false;
            result = new CheckResult(Severity.Invalid, V142, CheckIdentifier.History);
            return true;
        }
        private CheckResult VerifyCommonMemory(PKM pkm, int handler)
        {
            Memories.GetMemoryVariables(pkm, out int m, out int t, out int i, out int f, out string tr, handler);
            int matchingMoveMemory = Array.IndexOf(Memories.MoveSpecificMemories[0], m);
            if (matchingMoveMemory != -1 && pkm.Species != 235 && !Legal.GetCanLearnMachineMove(pkm, Memories.MoveSpecificMemories[1][matchingMoveMemory], 6))
                return GetInvalid(string.Format(V153, tr));

            if (m == 6 && !Memories.LocationsWithPKCenter[0].Contains(t))
                return GetInvalid(string.Format(V154, tr));

            if (m == 21) // {0} saw {2} carrying {1} on its back. {4} that {3}.
                if (!Legal.GetCanLearnMachineMove(new PK6 { Species = t, EXP = PKX.GetEXP(100, t) }, 19, 6))
                    return GetInvalid(string.Format(V153, tr));

            if ((m == 16 || m == 48) && (t == 0 || !Legal.GetCanKnowMove(pkm, t, 6)))
                return GetInvalid(string.Format(V153, tr));

            if (m == 49 && (t == 0 || !Legal.GetCanRelearnMove(pkm, t, 6))) // {0} was able to remember {2} at {1}'s instruction. {4} that {3}.
                return GetInvalid(string.Format(V153, tr));

            if (!Memories.CanHaveIntensity(m, i))
                return GetInvalid(string.Format(V254, tr, Memories.GetMinimumIntensity(m)));

            if (m != 4 && !Memories.CanHaveFeeling(m, f))
                return GetInvalid(string.Format(V255, tr));

            return GetValid(string.Format(V155, tr));
        }

        private void VerifyOTMemoryIs(LegalityAnalysis data, int m, int i, int t, int f)
        {
            var pkm = data.pkm;
            if (pkm.OT_Memory != m)
                data.AddLine(Severity.Invalid, string.Format(V197, V205, m), CheckIdentifier.Memory);
            if (pkm.OT_Intensity != i)
                data.AddLine(Severity.Invalid, string.Format(V198, V205, i), CheckIdentifier.Memory);
            if (pkm.OT_TextVar != t)
                data.AddLine(Severity.Invalid, string.Format(V199, V205, t), CheckIdentifier.Memory);
            if (pkm.OT_Feeling != f)
                data.AddLine(Severity.Invalid, string.Format(V200, V205, f), CheckIdentifier.Memory);
        }
        public void VerifyOTMemory(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format < 6)
                return;

            var Info = data.Info;
            if (Info.Generation < 6 || pkm.IsEgg)
            {
                VerifyOTMemoryIs(data, 0, 0, 0, 0); // empty
                return;
            }

            switch (data.EncounterMatch)
            {
                case EncounterTrade _:
                    switch (Info.Generation)
                    {
                        case 6:
                            break; // Undocumented, uncommon, and insignificant -- don't bother.
                        case 7:
                            VerifyOTMemoryIs(data, 1, 3, 40, 5);
                            break;
                    }
                    return;
                case WC6 g when !g.IsEgg:
                    VerifyOTMemoryIs(data, g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling);
                    return;
                case WC7 g when !g.IsEgg:
                    VerifyOTMemoryIs(data, g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling);
                    return;
            }

            if (Info.Generation >= 7)
            {
                VerifyOTMemoryIs(data, 0, 0, 0, 0); // empty
                return;
            }

            switch (pkm.OT_Memory)
            {
                case 2: // {0} hatched from an Egg and saw {1} for the first time at... {2}. {4} that {3}.
                    if (pkm.Egg_Location == 0)
                        data.AddLine(Severity.Invalid, string.Format(V160, V205), CheckIdentifier.Memory);
                    break;

                case 4: // {0} became {1}’s friend when it arrived via Link Trade at... {2}. {4} that {3}.
                    data.AddLine(Severity.Invalid, string.Format(V161, V205), CheckIdentifier.Memory);
                    return;

                case 6: // {0} went to the Pokémon Center in {2} with {1} and had its tired body healed there. {4} that {3}.
                    int matchingOriginGame = Array.IndexOf(Memories.LocationsWithPKCenter[0], pkm.OT_TextVar);
                    if (matchingOriginGame != -1)
                    {
                        int gameID = Memories.LocationsWithPKCenter[1][matchingOriginGame];
                        if (pkm.XY && gameID != 0 || pkm.AO && gameID != 1)
                            data.AddLine(Severity.Invalid, string.Format(V162, V205), CheckIdentifier.Memory);
                    }
                    data.AddLine(VerifyCommonMemory(pkm, 0));
                    return;

                case 14:
                    if (!Legal.GetCanBeCaptured(pkm.OT_TextVar, Info.Generation, (GameVersion)pkm.Version))
                        data.AddLine(Severity.Invalid, string.Format(V165, V205), CheckIdentifier.Memory);
                    else
                        data.AddLine(Severity.Valid, string.Format(V164, V205), CheckIdentifier.Memory);
                    return;
            }
            if (pkm.XY && Memories.Memory_NotXY.Contains(pkm.OT_Memory))
                data.AddLine(Severity.Invalid, string.Format(V163, V205), CheckIdentifier.Memory);
            if (pkm.AO && Memories.Memory_NotAO.Contains(pkm.OT_Memory))
                data.AddLine(Severity.Invalid, string.Format(V163, V205), CheckIdentifier.Memory);

            data.AddLine(VerifyCommonMemory(pkm, 0));
        }
        public void VerifyHTMemory(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format < 6)
                return;

            var Info = data.Info;
            if (pkm.Format >= 7)
            {
                /*
                *  Bank Transfer adds in the Link Trade Memory.
                *  Trading 7<->7 between games (not Bank) clears this data.
                */
                if (pkm.HT_Memory == 0)
                {
                    if (pkm.HT_TextVar != 0 || pkm.HT_Intensity != 0 || pkm.HT_Feeling != 0)
                        data.AddLine(Severity.Invalid, V329, CheckIdentifier.Memory);
                    return;
                }

                // Transfer 6->7 & withdraw to same HT => keeps past gen memory
                // Don't require link trade memory for these past gen cases
                int gen = Info.Generation;
                if (3 <= gen && gen < 7 && pkm.CurrentHandler == 1)
                    return;

                if (pkm.HT_Memory != 4)
                    data.AddLine(Severity.Invalid, V156, CheckIdentifier.Memory);
                if (pkm.HT_TextVar != 0)
                    data.AddLine(Severity.Invalid, V157, CheckIdentifier.Memory);
                if (pkm.HT_Intensity != 1)
                    data.AddLine(Severity.Invalid, V158, CheckIdentifier.Memory);
                if (pkm.HT_Feeling > 10)
                    data.AddLine(Severity.Invalid, V159, CheckIdentifier.Memory);
                return;
            }

            switch (pkm.HT_Memory)
            {
                case 0:
                    if (string.IsNullOrEmpty(pkm.HT_Name))
                        return;
                    data.AddLine(Severity.Invalid, V150, CheckIdentifier.Memory); return;
                case 1: // {0} met {1} at... {2}. {1} threw a Poké Ball at it, and they started to travel together. {4} that {3}.
                    data.AddLine(Severity.Invalid, string.Format(V202, V206), CheckIdentifier.Memory); return;

                case 2: // {0} hatched from an Egg and saw {1} for the first time at... {2}. {4} that {3}.
                    data.AddLine(Severity.Invalid, string.Format(V160, V206), CheckIdentifier.Memory); return;

                case 14:
                    if (Legal.GetCanBeCaptured(pkm.HT_TextVar, 6))
                        data.AddLine(Severity.Valid, string.Format(V164, V206), CheckIdentifier.Memory);
                    else
                        data.AddLine(Severity.Invalid, string.Format(V165, V206), CheckIdentifier.Memory);
                    return;
            }
            data.AddLine(VerifyCommonMemory(pkm, 1));
        }

        // ORAS contests mistakenly apply 20 affection to the OT instead of the current handler's value
        private static bool IsInvalidContestAffection(PKM pkm) => pkm.OT_Affection != 255 && pkm.OT_Affection % 20 != 0;
    }
}
