using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.Encounters6;

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
            if (data.pkm.Format < 6)
                return;
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
                if ((pkm.OT_Affection != 0 && Info.Generation <= 2) || IsInvalidContestAffection(pkm))
                    return GetInvalid(LMemoryStatAffectionOT0);
                if (pkm.OT_Memory > 0 || pkm.OT_Feeling > 0 || pkm.OT_Intensity > 0 || pkm.OT_TextVar > 0)
                    return GetInvalid(LMemoryIndexIDOT0);
            }

            if (pkm.Format >= 6 && Info.Generation != pkm.Format && pkm.CurrentHandler != 1)
                return GetInvalid(LTransferHTFlagRequired);

            if (pkm.HT_Gender > 1)
                return GetInvalid(string.Format(LMemoryHTGender, pkm.HT_Gender));

            if (EncounterMatch is WC6 wc6 && wc6.OT_Name.Length > 0)
            {
                if (pkm.OT_Friendship != PersonalTable.AO[EncounterMatch.Species].BaseFriendship)
                    return GetInvalid(LMemoryStatFriendshipOTBaseEvent);
                if (pkm.OT_Affection != 0 && (pkm.AO || !pkm.IsUntraded) && IsInvalidContestAffection(pkm))
                    return GetInvalid(LMemoryStatAffectionOT0Event);
                if (pkm.CurrentHandler != 1)
                    return GetInvalid(LMemoryHTEvent);
            }
            else if (EncounterMatch is WC7 wc7 && wc7.OT_Name.Length > 0 && wc7.TID != 18075) // Ash Pikachu QR Gift doesn't set Current Handler
            {
                if (pkm.OT_Friendship != PersonalTable.USUM[EncounterMatch.Species].BaseFriendship)
                    return GetInvalid(LMemoryStatFriendshipOTBaseEvent);
                if (pkm.OT_Affection != 0)
                    return GetInvalid(LMemoryStatAffectionOT0Event);
                if (pkm.CurrentHandler != 1)
                    return GetInvalid(LMemoryHTEvent);
            }
            else if (EncounterMatch is MysteryGift mg && mg.Format < 6 && pkm.Format >= 6)
            {
                if (pkm.OT_Affection != 0 && IsInvalidContestAffection(pkm))
                    return GetInvalid(LMemoryStatAffectionOT0Event);
                if (pkm.CurrentHandler != 1)
                    return GetInvalid(LMemoryHTEvent);
            }

            // Check sequential order (no zero gaps)
            if (pkm is IGeoTrack t)
            {
                var valid = t.GetValidity();
                if (valid == GeoValid.CountryAfterPreviousEmpty)
                    return GetInvalid(LGeoBadOrder);
                if (valid == GeoValid.RegionWithoutCountry)
                    return GetInvalid(LGeoNoRegion);
            }
            if (pkm.Format >= 7)
                return VerifyHistory7(data);

            // Determine if we should check for Handling Trainer Memories
            // A Pokémon is untraded if...
            bool untraded = GetIsUntradedByEncounterMemories(pkm, EncounterMatch, Info.Generation);
            if (untraded) // Is not Traded
            {
                if (pkm.HT_Name.Length != 0)
                    return GetInvalid(LGeoNoCountryHT);
                if (pkm is IGeoTrack g && g.Geo1_Country != 0)
                    return GetInvalid(LGeoNoHT);
                if (pkm.HT_Memory != 0)
                    return GetInvalid(LMemoryMissingHTName);
                if (pkm.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
                    return GetInvalid(LMemoryHTFlagInvalid);
                if (pkm.HT_Friendship != 0)
                    return GetInvalid(LMemoryStatFriendshipHT0);
                if (pkm.HT_Affection != 0)
                    return GetInvalid(LMemoryStatAffectionHT0);
                if (pkm.XY && pkm is IContestStats s && s.HasContestStats())
                    return GetInvalid(LContestZero);

                if (VerifyHistoryUntradedHandler(pkm, out CheckResult chk1))
                    return chk1;
                if (EncounterMatch.Species != pkm.Species && VerifyHistoryUntradedEvolution(pkm, Info.EvoChainsAllGens, out CheckResult chk2))
                    return chk2;
            }
            else // Is Traded
            {
                if (pkm.Format == 6 && pkm.HT_Memory == 0 && !pkm.IsEgg)
                    return GetInvalid(LMemoryMissingHT);
            }

            // Memory ChecksResult
            if (pkm.IsEgg)
            {
                if (pkm.HT_Memory != 0)
                    return GetInvalid(LMemoryArgBadHT);
                if (pkm.OT_Memory != 0)
                    return GetInvalid(LMemoryArgBadEggOT);
            }
            else if (!(EncounterMatch is WC6))
            {
                if (pkm.OT_Memory == 0 ^ !pkm.Gen6)
                    return GetInvalid(LMemoryMissingOT);
                if (Info.Generation < 6 && pkm.OT_Affection != 0 && IsInvalidContestAffection(pkm))
                    return GetInvalid(LMemoryStatAffectionOT0);
            }
            // Unimplemented: Ingame Trade Memories

            return GetValid(LMemoryValid);
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
                    return GetInvalid(LGeoMemoryMissing);
            }

            if ((2 >= Info.Generation || Info.Generation >= 7) && pkm is IContestStats s && s.HasContestStats())
                return GetInvalid(LContestZero);

            if (!pkm.WasEvent && pkm.HT_Name.Length == 0) // Is not Traded
            {
                if (VerifyHistoryUntradedHandler(pkm, out CheckResult chk1))
                    return chk1;
                if (EncounterMatch.Species != pkm.Species && VerifyHistoryUntradedEvolution(pkm, Info.EvoChainsAllGens, out CheckResult chk2))
                    return chk2;
            }

            return GetValid(LMemoryValid);
        }

        private bool VerifyHistoryUntradedHandler(PKM pkm, out CheckResult result)
        {
            result = null;
            if (pkm.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
                result = GetInvalid(LMemoryHTFlagInvalid);
            else if (pkm.HT_Friendship != 0)
                result = GetInvalid(LMemoryStatFriendshipHT0);
            else if (pkm.HT_Affection != 0)
                result = GetInvalid(LMemoryStatAffectionHT0);
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
                    result = GetInvalid(LEvoBeautyTradeLow);
                else if (pkm.CurrentLevel == 1)
                    result = GetInvalid(LEvoBeautyUntrained);
                else
                    return false;
                return true;
            }
            if (!Legal.IsTradeEvolved(chain, pkm.Format))
                return false;
            result = GetInvalid(LEvoTradeRequiredMemory);
            return true;
        }

        private CheckResult VerifyCommonMemory(PKM pkm, int handler)
        {
            var memory = MemoryVariableSet.Read(pkm, handler);
            int matchingMoveMemory = Array.IndexOf(Memories.MoveSpecificMemories[0], memory.MemoryID);
            if (matchingMoveMemory != -1 && pkm.Species != 235 && !Legal.GetCanLearnMachineMove(pkm, Memories.MoveSpecificMemories[1][matchingMoveMemory], 6))
                return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));

            switch (memory.MemoryID)
            {
                case 6 when !Memories.LocationsWithPKCenter.Contains(memory.Variable):
                    return GetInvalid(string.Format(LMemoryArgBadPokecenter, memory.Handler));

                // {0} saw {2} carrying {1} on its back. {4} that {3}.
                case 21 when !Legal.GetCanLearnMachineMove(new PK6 {Species = memory.Variable, EXP = Experience.GetEXP(100, memory.Variable, 0)}, 19, 6):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));

                case 16 when memory.Variable == 0 || !Legal.GetCanKnowMove(pkm, memory.Variable, 6):
                case 48 when memory.Variable == 0 || !Legal.GetCanKnowMove(pkm, memory.Variable, 6):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));

                // {0} was able to remember {2} at {1}'s instruction. {4} that {3}.
                case 49 when memory.Variable == 0 || !Legal.GetCanRelearnMove(pkm, memory.Variable, 6):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));
            }

            if (!Memories.CanHaveIntensity(memory.MemoryID, memory.Intensity))
                return GetInvalid(string.Format(LMemoryIndexIntensityMin, memory.Handler, Memories.GetMinimumIntensity(memory.MemoryID)));

            if (memory.MemoryID != 4 && !Memories.CanHaveFeeling(memory.MemoryID, memory.Feeling))
                return GetInvalid(string.Format(LMemoryFeelInvalid, memory.Handler));

            return GetValid(string.Format(LMemoryF_0_Valid, memory.Handler));
        }

        private void VerifyOTMemoryIs(LegalityAnalysis data, int m, int i, int t, int f)
        {
            var pkm = data.pkm;
            if (pkm.OT_Memory != m)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexID, L_XOT, m)));
            if (pkm.OT_Intensity != i)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexIntensity, L_XOT, i)));
            if (pkm.OT_TextVar != t)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexVar, L_XOT, t)));
            if (pkm.OT_Feeling != f)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexFeel, L_XOT, f)));
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
                case WC6 g when !g.IsEgg && g.OTGender != 3:
                    VerifyOTMemoryIs(data, g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling);
                    return;
                case WC7 g when !g.IsEgg && g.OTGender != 3:
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
                        data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadHatch, L_XOT), CheckIdentifier.Memory);
                    break;

                case 4: // {0} became {1}’s friend when it arrived via Link Trade at... {2}. {4} that {3}.
                    data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadOTEgg, L_XOT), CheckIdentifier.Memory);
                    return;

                case 6: // {0} went to the Pokémon Center in {2} with {1} and had its tired body healed there. {4} that {3}.
                    int matchingOriginGame = Array.IndexOf(Memories.LocationsWithPKCenter, pkm.OT_TextVar);
                    if (matchingOriginGame != -1)
                    {
                        var gameID = Memories.GetGameVersionForPokeCenterIndex(matchingOriginGame);
                        if (!gameID.Contains((GameVersion)pkm.Version))
                            data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadLocation, L_XOT), CheckIdentifier.Memory);
                    }
                    data.AddLine(VerifyCommonMemory(pkm, 0));
                    return;

                case 14:
                    if (!GetCanBeCaptured(pkm.OT_TextVar, Info.Generation, (GameVersion)pkm.Version))
                        data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadSpecies, L_XOT), CheckIdentifier.Memory);
                    else
                        data.AddLine(Severity.Valid, string.Format(LMemoryArgSpecies, L_XOT), CheckIdentifier.Memory);
                    return;
            }
            if (pkm.XY && Memories.Memory_NotXY.Contains(pkm.OT_Memory))
                data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadID, L_XOT), CheckIdentifier.Memory);
            if (pkm.AO && Memories.Memory_NotAO.Contains(pkm.OT_Memory))
                data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadID, L_XOT), CheckIdentifier.Memory);

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
                        data.AddLine(Severity.Invalid, LMemoryCleared, CheckIdentifier.Memory);
                    return;
                }

                // Transfer 6->7 & withdraw to same HT => keeps past gen memory
                // Don't require link trade memory for these past gen cases
                int gen = Info.Generation;
                if (3 <= gen && gen < 7 && pkm.CurrentHandler == 1)
                    return;

                if (pkm.HT_Memory != 4)
                    data.AddLine(Severity.Invalid, LMemoryIndexLinkHT, CheckIdentifier.Memory);
                if (pkm.HT_TextVar != 0)
                    data.AddLine(Severity.Invalid, LMemoryIndexArgHT, CheckIdentifier.Memory);
                if (pkm.HT_Intensity != 1)
                    data.AddLine(Severity.Invalid, LMemoryIndexIntensityHT1, CheckIdentifier.Memory);
                if (pkm.HT_Feeling > 10)
                    data.AddLine(Severity.Invalid, LMemoryIndexFeelHT09, CheckIdentifier.Memory);
                return;
            }

            switch (pkm.HT_Memory)
            {
                case 0:
                    if (string.IsNullOrEmpty(pkm.HT_Name))
                        return;
                    data.AddLine(Severity.Invalid, LMemoryMissingHT, CheckIdentifier.Memory); return;
                case 1: // {0} met {1} at... {2}. {1} threw a Poké Ball at it, and they started to travel together. {4} that {3}.
                    data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadCatch, L_XHT), CheckIdentifier.Memory); return;

                case 2: // {0} hatched from an Egg and saw {1} for the first time at... {2}. {4} that {3}.
                    data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadHatch, L_XHT), CheckIdentifier.Memory); return;

                case 14:
                    if (GetCanBeCaptured(pkm.HT_TextVar, 6))
                        data.AddLine(Severity.Valid, string.Format(LMemoryArgSpecies, L_XHT), CheckIdentifier.Memory);
                    else
                        data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadSpecies, L_XHT), CheckIdentifier.Memory);
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

            bool untraded = pkm.HT_Name.Length == 0 || (pkm is IGeoTrack g && g.Geo1_Country == 0);
            if (EncounterMatch is WC6 gift)
                return gift.OTGender == 3 && untraded;
            return untraded;
        }

        private static bool GetCanBeCaptured(int species, int gen, GameVersion version = GameVersion.Any)
        {
            switch (gen)
            {
                // Capture Memory only obtainable via Gen 6.
                case 6:
                    switch (version)
                    {
                        case GameVersion.Any:
                            return Legal.FriendSafari.Contains(species)
                                   || GetCanBeCaptured(species, SlotsX, StaticX)
                                   || GetCanBeCaptured(species, SlotsY, StaticY)
                                   || GetCanBeCaptured(species, SlotsA, StaticA)
                                   || GetCanBeCaptured(species, SlotsO, StaticO);
                        case GameVersion.X:
                            return Legal.FriendSafari.Contains(species)
                                   || GetCanBeCaptured(species, SlotsX, StaticX);
                        case GameVersion.Y:
                            return Legal.FriendSafari.Contains(species)
                                   || GetCanBeCaptured(species, SlotsY, StaticY);

                        case GameVersion.AS:
                            return GetCanBeCaptured(species, SlotsA, StaticA);
                        case GameVersion.OR:
                            return GetCanBeCaptured(species, SlotsO, StaticO);
                    }
                    break;
            }
            return false;
        }

        private static bool GetCanBeCaptured(int species, IEnumerable<EncounterArea> area, IEnumerable<EncounterStatic> statics)
        {
            if (area.Any(loc => loc.Slots.Any(slot => slot.Species == species)))
                return true;
            if (statics.Any(enc => enc.Species == species && !enc.Gift))
                return true;
            return false;
        }
    }
}
