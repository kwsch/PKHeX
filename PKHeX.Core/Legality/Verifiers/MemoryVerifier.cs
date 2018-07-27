using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.OT_Memory"/>, <see cref="PKM.HT_Memory"/>, and associated values.
    /// </summary>
    public sealed class MemoryVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Memory;

        public override void Verify(LegalityAnalysis data)
        {
            var hist = VerifyHistory(data);
            VerifyOTMemory(data);
            VerifyHTMemory(data);
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
                    return GetValid(V128);

                if ((pkm.OT_Affection != 0 && Info.Generation <= 2) || IsInvalidContestAffection(pkm))
                    return GetInvalid(V129);
                if (pkm.OT_Memory > 0 || pkm.OT_Feeling > 0 || pkm.OT_Intensity > 0 || pkm.OT_TextVar > 0)
                    return GetInvalid(V130);
            }

            if (pkm.Format >= 6 && Info.Generation != pkm.Format && pkm.CurrentHandler != 1)
                return GetInvalid(V124);

            if (pkm.HT_Gender > 1)
                return GetInvalid(string.Format(V131, pkm.HT_Gender));

            if (EncounterMatch is WC6 wc6 && wc6.OT_Name.Length > 0)
            {
                if (pkm.OT_Friendship != PersonalTable.AO[EncounterMatch.Species].BaseFriendship)
                    return GetInvalid(V132);
                if (pkm.OT_Affection != 0 && (pkm.AO || !pkm.IsUntraded) && IsInvalidContestAffection(pkm))
                    return GetInvalid(V133);
                if (pkm.CurrentHandler != 1)
                    return GetInvalid(V134);
            }
            else if (EncounterMatch is WC7 wc7 && wc7.OT_Name.Length > 0 && wc7.TID != 18075) // Ash Pikachu QR Gift doesn't set Current Handler
            {
                if (pkm.OT_Friendship != PersonalTable.USUM[EncounterMatch.Species].BaseFriendship)
                    return GetInvalid(V132);
                if (pkm.OT_Affection != 0)
                    return GetInvalid(V133);
                if (pkm.CurrentHandler != 1)
                    return GetInvalid(V134);
            }
            else if (EncounterMatch is MysteryGift mg && mg.Format < 6 && pkm.Format >= 6)
            {
                if (pkm.OT_Affection != 0 && IsInvalidContestAffection(pkm))
                    return GetInvalid(V133);
                if (pkm.CurrentHandler != 1)
                    return GetInvalid(V134);
            }

            // Check sequential order (no zero gaps)
            if (pkm is IGeoTrack t)
            {
                var valid = t.GetValidity();
                if (valid == GeoValid.CountryAfterPreviousEmpty)
                    return GetInvalid(V135);
                if (valid == GeoValid.RegionWithoutCountry)
                    return GetInvalid(V136);
            }
            if (pkm.Format >= 7)
                return VerifyHistory7(data);

            // Determine if we should check for Handling Trainer Memories
            // A Pokémon is untraded if...
            bool untraded = GetIsUntradedByEncounterMemories(pkm, EncounterMatch, Info.Generation);
            if (untraded) // Is not Traded
            {
                if (pkm.HT_Name.Length != 0)
                    return GetInvalid(V146);
                if (pkm is IGeoTrack g && g.Geo1_Country != 0)
                    return GetInvalid(V147);
                if (pkm.HT_Memory != 0)
                    return GetInvalid(V148);
                if (pkm.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
                    return GetInvalid(V139);
                if (pkm.HT_Friendship != 0)
                    return GetInvalid(V140);
                if (pkm.HT_Affection != 0)
                    return GetInvalid(V141);
                if (pkm.XY && pkm is IContestStats s && s.HasContestStats())
                    return GetInvalid(V138);

                if (VerifyHistoryUntradedHandler(pkm, out CheckResult chk1))
                    return chk1;
                if (EncounterMatch.Species != pkm.Species && VerifyHistoryUntradedEvolution(pkm, Info.EvoChainsAllGens, out CheckResult chk2))
                    return chk2;
            }
            else // Is Traded
            {
                if (pkm.Format == 6 && pkm.HT_Memory == 0 && !pkm.IsEgg)
                    return GetInvalid(V150);
            }

            // Memory ChecksResult
            if (pkm.IsEgg)
            {
                if (pkm.HT_Memory != 0)
                    return GetInvalid(V149);
                if (pkm.OT_Memory != 0)
                    return GetInvalid(V151);
            }
            else if (!(EncounterMatch is WC6))
            {
                if (pkm.OT_Memory == 0 ^ !pkm.Gen6)
                    return GetInvalid(V152);
                if (Info.Generation < 6 && pkm.OT_Affection != 0 && IsInvalidContestAffection(pkm))
                    return GetInvalid(V129);
            }
            // Unimplemented: Ingame Trade Memories

            return GetValid(V145);
        }

        private CheckResult VerifyHistory7(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;
            var Info = data.Info;

            if (pkm.VC1 && pkm is IGeoTrack g)
            {
                var hasGeo = g.Geo1_Country != 0;
                if (!hasGeo)
                    return GetInvalid(V137);
            }

            if ((2 >= Info.Generation || Info.Generation >= 7) && pkm is IContestStats s && s.HasContestStats())
                return GetInvalid(V138);

            if (!pkm.WasEvent && pkm.HT_Name.Length == 0) // Is not Traded
            {
                if (VerifyHistoryUntradedHandler(pkm, out CheckResult chk1))
                    return chk1;
                if (EncounterMatch.Species != pkm.Species && VerifyHistoryUntradedEvolution(pkm, Info.EvoChainsAllGens, out CheckResult chk2))
                    return chk2;
            }

            return GetValid(V145);
        }

        private bool VerifyHistoryUntradedHandler(PKM pkm, out CheckResult result)
        {
            result = null;
            if (pkm.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
                result = GetInvalid(V139);
            else if (pkm.HT_Friendship != 0)
                result = GetInvalid(V140);
            else if (pkm.HT_Affection != 0)
                result = GetInvalid(V141);
            else
                return false;

            return true;
        }

        private bool VerifyHistoryUntradedEvolution(PKM pkm, IReadOnlyList<EvoCriteria>[] chain, out CheckResult result)
        {
            result = null;
            // Handling Trainer string is empty implying it has not been traded.
            // If it must be trade evolved, flag it.

            if (pkm.Species == 350) // Milotic
            {
                if (Legal.IsTradeEvolved(chain, pkm.Format))
                    return false;
                if (pkm is IContestStats s && s.CNT_Beauty < 170) // Beauty Contest Stat Requirement
                    result = GetInvalid(V143);
                else if (pkm.CurrentLevel == 1)
                    result = GetInvalid(V144);
                else
                    return false;
                return true;
            }
            if (!Legal.IsTradeEvolved(chain, pkm.Format))
                return false;
            result = GetInvalid(V142);
            return true;
        }

        private CheckResult VerifyCommonMemory(PKM pkm, int handler)
        {
            Memories.GetMemoryVariables(pkm, out int m, out int t, out int i, out int f, out string tr, handler);
            int matchingMoveMemory = Array.IndexOf(Memories.MoveSpecificMemories[0], m);
            if (matchingMoveMemory != -1 && pkm.Species != 235 && !Legal.GetCanLearnMachineMove(pkm, Memories.MoveSpecificMemories[1][matchingMoveMemory], 6))
                return GetInvalid(string.Format(V153, tr));

            switch (m)
            {
                case 6 when !Memories.LocationsWithPKCenter[0].Contains(t):
                    return GetInvalid(string.Format(V154, tr));

                // {0} saw {2} carrying {1} on its back. {4} that {3}.
                case 21 when !Legal.GetCanLearnMachineMove(new PK6 {Species = t, EXP = PKX.GetEXP(100, t)}, 19, 6):
                    return GetInvalid(string.Format(V153, tr));

                case 16 when t == 0 || !Legal.GetCanKnowMove(pkm, t, 6):
                case 48 when t == 0 || !Legal.GetCanKnowMove(pkm, t, 6):
                    return GetInvalid(string.Format(V153, tr));

                // {0} was able to remember {2} at {1}'s instruction. {4} that {3}.
                case 49 when t == 0 || !Legal.GetCanRelearnMove(pkm, t, 6):
                    return GetInvalid(string.Format(V153, tr));
            }

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
                data.AddLine(GetInvalid(string.Format(V197, V205, m)));
            if (pkm.OT_Intensity != i)
                data.AddLine(GetInvalid(string.Format(V198, V205, i)));
            if (pkm.OT_TextVar != t)
                data.AddLine(GetInvalid(string.Format(V199, V205, t)));
            if (pkm.OT_Feeling != f)
                data.AddLine(GetInvalid(string.Format(V200, V205, f)));
        }

        private void VerifyOTMemory(LegalityAnalysis data)
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
                        if ((pkm.XY && gameID != 0) || (pkm.AO && gameID != 1))
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

        private void VerifyHTMemory(LegalityAnalysis data)
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

        private static bool GetIsUntradedByEncounterMemories(PKM pkm, IEncounterable EncounterMatch, int generation)
        {
            if (generation < 6)
                return false;
            if (EncounterMatch is EncounterLink link && !link.OT)
                return false;

            bool untraded = pkm.HT_Name.Length == 0 || (pkm is IGeoTrack g && g.Geo1_Country == 0);
            if (!(EncounterMatch is MysteryGift gift))
                return untraded;

            untraded |= !pkm.WasEventEgg;
            untraded &= gift.IsEgg;
            return untraded;
        }
    }
}
