using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public static class EncounterVerifier
    {
        /// <summary>
        /// Gets the method to verify the <see cref="IEncounterable"/> data.
        /// </summary>
        /// <param name="pkm">Source data to verify</param>
        /// <returns>Returns the verification method appropriate for the input PKM</returns>
        public static Func<PKM, LegalInfo, CheckResult> GetEncounterVerifierMethod(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 1:
                case 2:
                    return VerifyEncounterG12;
                default:
                    return VerifyEncounter;
            }
        }

        private static CheckResult VerifyEncounter(PKM pkm, LegalInfo info)
        {
            return info.EncounterMatch switch
            {
                EncounterEgg _ => VerifyEncounterEgg(pkm),
                EncounterTrade t => VerifyEncounterTrade(pkm, t),
                EncounterSlot w => VerifyEncounterWild(pkm, w),
                EncounterStatic s => VerifyEncounterStatic(pkm, s),
                MysteryGift g => VerifyEncounterEvent(pkm, g),
                _ => new CheckResult(Severity.Invalid, LEncInvalid, CheckIdentifier.Encounter)
            };
        }

        private static CheckResult VerifyEncounterG12(PKM pkm, LegalInfo info)
        {
            var EncounterMatch = info.EncounterMatch;
            if (EncounterMatch.EggEncounter)
            {
                return VerifyEncounterEgg(pkm);
            }
            if (EncounterMatch is EncounterSlot1 l)
            {
                if (info.Generation == 2)
                    return VerifyWildEncounterGen2(pkm, l);
                return new CheckResult(Severity.Valid, LEncCondition, CheckIdentifier.Encounter);
            }
            if (EncounterMatch is EncounterStatic s)
                return VerifyEncounterStatic(pkm, s);
            if (EncounterMatch is EncounterTrade t)
                return VerifyEncounterTrade(pkm, t);

            return new CheckResult(Severity.Invalid, LEncInvalid, CheckIdentifier.Encounter);
        }

        // Gen2 Wild Encounters
        private static CheckResult VerifyWildEncounterGen2(PKM pkm, EncounterSlot1 encounter)
        {
            switch (encounter.Type)
            {
                // Fishing in the beta gen 2 Safari Zone
                case SlotType.Old_Rod_Safari:
                case SlotType.Good_Rod_Safari:
                case SlotType.Super_Rod_Safari:
                    return new CheckResult(Severity.Invalid, LG2InvalidTileSafari, CheckIdentifier.Encounter);
            }

            if (encounter.Version == GameVersion.C)
                return VerifyWildEncounterCrystal(pkm, encounter);

            return new CheckResult(Severity.Valid, LEncCondition, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyWildEncounterCrystal(PKM pkm, EncounterSlot encounter)
        {
            switch (encounter.Type)
            {
                case SlotType.Headbutt:
                case SlotType.Headbutt_Special:
                    return VerifyWildEncounterCrystalHeadbutt(pkm, encounter);

                case SlotType.Old_Rod:
                case SlotType.Good_Rod:
                case SlotType.Super_Rod:
                    switch (encounter.Location)
                    {
                        case 19: // National Park
                            return new CheckResult(Severity.Invalid, LG2InvalidTilePark, CheckIdentifier.Encounter);
                        case 76: // Route 14
                            return new CheckResult(Severity.Invalid, LG2InvalidTileR14, CheckIdentifier.Encounter);
                    }
                    break;
            }

            return new CheckResult(Severity.Valid, LEncCondition, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyWildEncounterCrystalHeadbutt(ITrainerID tr, EncounterSlot encounter)
        {
            var tree = Encounters2.GetGSCHeadbuttAvailability(encounter, tr.TID);
            return tree switch
            {
                TreeEncounterAvailable.ValidTree => new CheckResult(Severity.Valid, LG2TreeID, CheckIdentifier.Encounter),
                TreeEncounterAvailable.InvalidTree => new CheckResult(Severity.Invalid, LG2InvalidTileTreeID, CheckIdentifier.Encounter),
                _ => new CheckResult(Severity.Invalid, LG2InvalidTileTreeNotFound, CheckIdentifier.Encounter)
            };
        }

        // Eggs
        private static CheckResult VerifyEncounterEgg(PKM pkm, bool checkSpecies = true)
        {
            // Check Species
            if (checkSpecies && Legal.NoHatchFromEgg.Contains(pkm.Species))
                return new CheckResult(Severity.Invalid, LEggSpecies, CheckIdentifier.Encounter);

            switch (pkm.GenNumber)
            {
                case 1:
                case 2: return new CheckResult(CheckIdentifier.Encounter); // valid -- no met location info
                case 3: return pkm.Format != 3 ? VerifyEncounterEgg3Transfer(pkm) : VerifyEncounterEgg3(pkm);
                case 4: return pkm.IsEgg ? VerifyUnhatchedEgg(pkm, Locations.LinkTrade4) : VerifyEncounterEgg4(pkm);
                case 5: return pkm.IsEgg ? VerifyUnhatchedEgg(pkm, Locations.LinkTrade5) : VerifyEncounterEgg5(pkm);
                case 6: return pkm.IsEgg ? VerifyUnhatchedEgg(pkm, Locations.LinkTrade6) : VerifyEncounterEgg6(pkm);
                case 7: return pkm.IsEgg ? VerifyUnhatchedEgg(pkm, Locations.LinkTrade6) : VerifyEncounterEgg7(pkm);
                case 8: return pkm.IsEgg ? VerifyUnhatchedEgg(pkm, Locations.LinkTrade6) : VerifyEncounterEgg8(pkm);

                default: // none of the above
                    return new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter);
            }
        }

        private static CheckResult VerifyEncounterEgg3(PKM pkm)
        {
            return pkm.Format == 3 ? VerifyEncounterEgg3Native(pkm) : VerifyEncounterEgg3Transfer(pkm);
        }

        private static CheckResult VerifyEncounterEgg3Native(PKM pkm)
        {
            if (pkm.Met_Level != 0)
                return new CheckResult(Severity.Invalid, string.Format(LEggFMetLevel_0, 0), CheckIdentifier.Encounter);
            if (pkm.IsEgg)
            {
                var loc = pkm.FRLG ? Legal.ValidEggMet_FRLG : Legal.ValidEggMet_RSE;
                if (!loc.Contains(pkm.Met_Location))
                    return new CheckResult(Severity.Invalid, LEggMetLocationFail, CheckIdentifier.Encounter);
            }
            else
            {
                var locs = pkm.FRLG ? Legal.ValidMet_FRLG : pkm.E ? Legal.ValidMet_E : Legal.ValidMet_RS;
                if (locs.Contains(pkm.Met_Location))
                    return new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter);
                if (Legal.ValidMet_FRLG.Contains(pkm.Met_Location) || Legal.ValidMet_E.Contains(pkm.Met_Location) || Legal.ValidMet_RS.Contains(pkm.Met_Location))
                    return new CheckResult(Severity.Valid, LEggLocationTrade, CheckIdentifier.Encounter);
                return new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter);
            }
            return new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterEgg3Transfer(PKM pkm)
        {
            if (pkm.IsEgg)
                return new CheckResult(Severity.Invalid, LTransferEgg, CheckIdentifier.Encounter);
            if (pkm.Met_Level < 5)
                return new CheckResult(Severity.Invalid, LTransferEggMetLevel, CheckIdentifier.Encounter);
            if (pkm.Egg_Location != 0)
                return new CheckResult(Severity.Invalid, LEggLocationNone, CheckIdentifier.Encounter);
            if (pkm.Format == 4 && pkm.Met_Location != Locations.Transfer3)
                return new CheckResult(Severity.Invalid, LEggLocationPalPark, CheckIdentifier.Encounter);
            if (pkm.Format != 4 && pkm.Met_Location != Locations.Transfer4)
                return new CheckResult(Severity.Invalid, LTransferEggLocationTransporter, CheckIdentifier.Encounter);

            return new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterEgg4(PKM pkm)
        {
            if (pkm.Format == 4)
            {
                // Traded eggs don't update Version, like in future games.
                var locations = pkm.WasTradedEgg ? Legal.ValidMet_4 :
                    pkm.HGSS ? Legal.ValidMet_HGSS :
                    pkm.Pt ? Legal.ValidMet_Pt :
                    Legal.ValidMet_DP;
                return VerifyEncounterEggLevelLoc(pkm, 0, locations);
            }
            if (pkm.IsEgg)
                return new CheckResult(Severity.Invalid, LTransferEgg, CheckIdentifier.Encounter);

            // transferred
            if (pkm.Met_Level < 1)
                return new CheckResult(Severity.Invalid, LTransferEggMetLevel, CheckIdentifier.Encounter);
            if (pkm.Met_Location != Locations.Transfer4)
                return new CheckResult(Severity.Invalid, LTransferEggLocationTransporter, CheckIdentifier.Encounter);
            return new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterEgg5(PKM pkm)
        {
            return VerifyEncounterEggLevelLoc(pkm, 1, pkm.B2W2 ? Legal.ValidMet_B2W2 : Legal.ValidMet_BW);
        }

        private static CheckResult VerifyEncounterEgg6(PKM pkm)
        {
            if (pkm.AO)
                return VerifyEncounterEggLevelLoc(pkm, 1, Legal.ValidMet_AO);

            if (pkm.Egg_Location == 318)
                return new CheckResult(Severity.Invalid, LEggMetLocationFail, CheckIdentifier.Encounter);

            return VerifyEncounterEggLevelLoc(pkm, 1, Legal.ValidMet_XY);
        }

        private static CheckResult VerifyEncounterEgg7(PKM pkm)
        {
            if (pkm.SM)
                return VerifyEncounterEggLevelLoc(pkm, 1, Legal.ValidMet_SM);
            if (pkm.USUM)
                return VerifyEncounterEggLevelLoc(pkm, 1, Legal.ValidMet_USUM);

            // no other games
            return new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterEgg8(PKM pkm)
        {
            if (pkm.SWSH)
                return VerifyEncounterEggLevelLoc(pkm, 1, Legal.ValidMet_SWSH);

            // no other games
            return new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterEggLevelLoc(PKM pkm, int eggLevel, ICollection<int> MetLocations)
        {
            if (pkm.Met_Level != eggLevel)
                return new CheckResult(Severity.Invalid, string.Format(LEggFMetLevel_0, eggLevel), CheckIdentifier.Encounter);
            return MetLocations.Contains(pkm.Met_Location)
                ? new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter)
                : new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyUnhatchedEgg(PKM pkm, int tradeLoc)
        {
            var eggLevel = pkm.Format < 5 ? 0 : 1;
            if (pkm.Met_Level != eggLevel)
                return new CheckResult(Severity.Invalid, string.Format(LEggFMetLevel_0, eggLevel), CheckIdentifier.Encounter);
            if (pkm.Egg_Location == tradeLoc)
                return new CheckResult(Severity.Invalid, LEggLocationTradeFail, CheckIdentifier.Encounter);

            if (pkm.Met_Location == tradeLoc)
                return new CheckResult(Severity.Valid, LEggLocationTrade, CheckIdentifier.Encounter);
            return pkm.Met_Location == 0
                ? new CheckResult(Severity.Valid, LEggUnhatched, CheckIdentifier.Encounter)
                : new CheckResult(Severity.Invalid, LEggLocationNone, CheckIdentifier.Encounter);
        }

        // Other
        private static CheckResult VerifyEncounterWild(PKM pkm, EncounterSlot slot)
        {
            // Check for Unreleased Encounters / Collisions
            switch (pkm.GenNumber)
            {
                case 4:
                    if (slot.Location == 193 && slot.Type == SlotType.Surf) // surfing in Johto Route 45
                        return new CheckResult(Severity.Invalid, LG4InvalidTileR45Surf, CheckIdentifier.Encounter);
                    break;
            }

            if (slot.Permissions.IsNormalLead)
            {
                return slot.Permissions.Pressure
                    ? new CheckResult(Severity.Valid, LEncConditionLead, CheckIdentifier.Encounter)
                    : new CheckResult(Severity.Valid, LEncCondition, CheckIdentifier.Encounter);
            }

            // Decreased Level Encounters
            if (slot.Permissions.WhiteFlute)
            {
                return slot.Permissions.Pressure
                    ? new CheckResult(Severity.Valid, LEncConditionWhiteLead, CheckIdentifier.Encounter)
                    : new CheckResult(Severity.Valid, LEncConditionWhite, CheckIdentifier.Encounter);
            }

            // Increased Level Encounters
            if (slot.Permissions.BlackFlute)
            {
                return slot.Permissions.Pressure
                    ? new CheckResult(Severity.Valid, LEncConditionBlackLead, CheckIdentifier.Encounter)
                    : new CheckResult(Severity.Valid, LEncConditionBlack, CheckIdentifier.Encounter);
            }

            if (slot.Permissions.Pressure)
                return new CheckResult(Severity.Valid, LEncConditionLead, CheckIdentifier.Encounter);

            return new CheckResult(Severity.Valid, LEncConditionDexNav, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterStatic(PKM pkm, EncounterStatic s)
        {
            // Check for Unreleased Encounters / Collisions
            switch (pkm.GenNumber)
            {
                case 3:
                    if (s is EncounterStaticShadow w && w.EReader && pkm.Language != (int)LanguageID.Japanese) // Non-JP E-reader Pokemon
                        return new CheckResult(Severity.Invalid, LG3EReader, CheckIdentifier.Encounter);
                    if (pkm.Species == (int)Species.Mew && s.Location == 201 && pkm.Language != (int)LanguageID.Japanese) // Non-JP Mew (Old Sea Map)
                        return new CheckResult(Severity.Invalid, LEncUnreleasedEMewJP, CheckIdentifier.Encounter);
                    break;
                case 4:
                    switch (pkm.Species)
                    {
                        case (int)Species.Darkrai when s.Location == 079 && !pkm.Pt: // DP Darkrai
                            return new CheckResult(Severity.Invalid, LEncUnreleasedPtDarkrai, CheckIdentifier.Encounter);
                        case (int)Species.Shaymin when s.Location == 063 && !pkm.Pt:// DP Shaymin
                            return new CheckResult(Severity.Invalid, LEncUnreleasedPtShaymin, CheckIdentifier.Encounter);
                        case (int)Species.Arceus when s.Location == 086: // Azure Flute Arceus
                            return new CheckResult(Severity.Invalid, LEncUnreleasedHoOArceus, CheckIdentifier.Encounter);
                    }
                    if (s.Location == 193 && s is EncounterStaticTyped t && t.TypeEncounter == EncounterType.Surfing_Fishing) // Roaming pokemon surfing in Johto Route 45
                        return new CheckResult(Severity.Invalid, LG4InvalidTileR45Surf, CheckIdentifier.Encounter);
                    break;
                case 7:
                    if (s.EggLocation == Locations.Daycare5 && pkm.RelearnMoves.Any(m => m != 0)) // eevee gift egg
                        return new CheckResult(Severity.Invalid, LEncStaticRelearn, CheckIdentifier.RelearnMove); // not gift egg
                    break;
            }
            if (s.EggEncounter && !pkm.IsEgg) // hatched
            {
                var hatchCheck = VerifyEncounterEgg(pkm);
                if (!hatchCheck.Valid)
                    return hatchCheck;
            }

            return new CheckResult(Severity.Valid, LEncStaticMatch, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterTrade(PKM pkm, EncounterTrade trade)
        {
            if (trade.EvolveOnTrade && trade.Species == pkm.Species)
            {
                // Pokemon that evolve on trade can not be in the phase evolution after the trade
                // If the trade holds an everstone EvolveOnTrade will be false for the encounter
                var species = LegalityAnalysis.SpeciesStrings;
                var unevolved = species[pkm.Species];
                var evolved = species[pkm.Species + 1];
                return new CheckResult(Severity.Invalid, string.Format(LEvoTradeReq, unevolved, evolved), CheckIdentifier.Encounter);
            }
            return new CheckResult(Severity.Valid, LEncTradeMatch, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterEvent(PKM pkm, MysteryGift MatchedGift)
        {
            switch (MatchedGift)
            {
                case PCD pcd:
                    if (!pcd.CanBeReceivedBy(pkm.Version) && pcd.Gift.PK.Version == 0)
                        return new CheckResult(Severity.Invalid, string.Format(L_XMatches0_1, MatchedGift.CardHeader, $"-- {LEncGiftVersionNotDistributed}"), CheckIdentifier.Encounter);
                    break;
            }
            if (!pkm.IsEgg && MatchedGift.IsEgg) // hatched
            {
                var hatchCheck = VerifyEncounterEgg(pkm, false);
                if (!hatchCheck.Valid)
                    return hatchCheck;
            }

            // Strict matching already performed by EncounterGenerator. May be worth moving some checks here to better flag invalid gifts.
            return new CheckResult(Severity.Valid, string.Format(L_XMatches0_1, MatchedGift.CardHeader, string.Empty), CheckIdentifier.Encounter);
        }
    }
}
