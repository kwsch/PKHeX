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
        public static Func<PKM, LegalInfo, CheckResult> GetEncounterVerifierMethod(PKM pkm) => pkm.Generation switch
        {
            1 or 2 => VerifyEncounterG12,
            _ => VerifyEncounter,
        };

        private static CheckResult VerifyEncounter(PKM pkm, LegalInfo info) => info.EncounterMatch switch
        {
            EncounterEgg e => VerifyEncounterEgg(pkm, e.Generation),
            EncounterTrade t => VerifyEncounterTrade(pkm, t),
            EncounterSlot w => VerifyEncounterWild(w),
            EncounterStatic s => VerifyEncounterStatic(pkm, s),
            MysteryGift g => VerifyEncounterEvent(pkm, g),
            _ => new CheckResult(Severity.Invalid, LEncInvalid, CheckIdentifier.Encounter)
        };

        private static CheckResult VerifyEncounterG12(PKM pkm, LegalInfo info)
        {
            var enc = info.EncounterMatch;
            if (enc.EggEncounter)
                return VerifyEncounterEgg(pkm, enc.Generation);

            return enc switch
            {
                EncounterSlot1 => new CheckResult(Severity.Valid, LEncCondition, CheckIdentifier.Encounter),
                EncounterSlot2 s2 => VerifyWildEncounterGen2(pkm, s2),
                EncounterStatic s => VerifyEncounterStatic(pkm, s),
                EncounterTrade t => VerifyEncounterTrade(pkm, t),
                _ => new CheckResult(Severity.Invalid, LEncInvalid, CheckIdentifier.Encounter)
            };
        }

        // Gen2 Wild Encounters
        private static CheckResult VerifyWildEncounterGen2(PKM pkm, EncounterSlot2 encounter)
        {
            if (encounter.Version == GameVersion.C)
                return VerifyWildEncounterCrystal(pkm, encounter);

            return new CheckResult(Severity.Valid, LEncCondition, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyWildEncounterCrystal(PKM pkm, EncounterSlot encounter)
        {
            switch (encounter.Area.Type)
            {
                case SlotType.Headbutt:
                    return VerifyWildEncounterCrystalHeadbutt(pkm, encounter);

                case SlotType.Old_Rod or SlotType.Good_Rod or SlotType.Super_Rod:
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
            return Encounters2.IsTreeAvailable(encounter, tr.TID)
                ? new CheckResult(Severity.Valid, LG2TreeID, CheckIdentifier.Encounter)
                : new CheckResult(Severity.Invalid, LG2InvalidTileTreeNotFound, CheckIdentifier.Encounter);
        }

        // Eggs
        private static CheckResult VerifyEncounterEgg(PKM pkm, int gen) => gen switch
        {
            2 => new CheckResult(CheckIdentifier.Encounter), // valid -- no met location info
            3 => pkm.IsEgg ? VerifyUnhatchedEgg3(pkm) : VerifyEncounterEgg3(pkm),
            4 => pkm.IsEgg ? VerifyUnhatchedEgg(pkm, Locations.LinkTrade4) : VerifyEncounterEgg4(pkm),
            5 => pkm.IsEgg ? VerifyUnhatchedEgg(pkm, Locations.LinkTrade5) : VerifyEncounterEgg5(pkm),
            6 => pkm.IsEgg ? VerifyUnhatchedEgg(pkm, Locations.LinkTrade6) : VerifyEncounterEgg6(pkm),
            7 => pkm.IsEgg ? VerifyUnhatchedEgg(pkm, Locations.LinkTrade6) : VerifyEncounterEgg7(pkm),
            8 => pkm.IsEgg ? VerifyUnhatchedEgg(pkm, Locations.LinkTrade6) : VerifyEncounterEgg8(pkm),
            _ => new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter)
        };

        private static CheckResult VerifyUnhatchedEgg3(PKM pkm)
        {
            if (pkm.Met_Level != 0)
                return new CheckResult(Severity.Invalid, string.Format(LEggFMetLevel_0, 0), CheckIdentifier.Encounter);

            // Only EncounterEgg should reach here.
            var loc = pkm.FRLG ? Locations.HatchLocationFRLG : Locations.HatchLocationRSE;
            if (pkm.Met_Location != loc)
                return new CheckResult(Severity.Invalid, LEggMetLocationFail, CheckIdentifier.Encounter);

            return new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterEgg3(PKM pkm)
        {
            if (pkm.Format != 3)
                return VerifyEncounterEgg3Transfer(pkm);

            if (pkm.Met_Level != 0)
                return new CheckResult(Severity.Invalid, string.Format(LEggFMetLevel_0, 0), CheckIdentifier.Encounter);

            // Check the origin game list.
            var met = pkm.Met_Location;
            var locs = pkm.FRLG ? Legal.ValidMet_FRLG : pkm.E ? Legal.ValidMet_E : Legal.ValidMet_RS;
            if (locs.Contains(met))
                return new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter);

            // Version isn't updated when hatching on a different game. Check any game.
            if (Legal.ValidMet_FRLG.Contains(met) || Legal.ValidMet_E.Contains(met) || Legal.ValidMet_RS.Contains(met))
                return new CheckResult(Severity.Valid, LEggLocationTrade, CheckIdentifier.Encounter);
            return new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterEgg3Transfer(PKM pkm)
        {
            if (pkm.IsEgg)
                return new CheckResult(Severity.Invalid, LTransferEgg, CheckIdentifier.Encounter);
            if (pkm.Met_Level < 5)
                return new CheckResult(Severity.Invalid, LTransferEggMetLevel, CheckIdentifier.Encounter);
            if (pkm.Egg_Location != 0)
                return new CheckResult(Severity.Invalid, LEggLocationNone, CheckIdentifier.Encounter);

            if (pkm.Format != 4)
            {
                if (pkm.Met_Location != Locations.Transfer4)
                    return new CheckResult(Severity.Invalid, LTransferEggLocationTransporter, CheckIdentifier.Encounter);
            }
            else
            {
                if (pkm.Met_Location != Locations.Transfer3)
                    return new CheckResult(Severity.Invalid, LEggLocationPalPark, CheckIdentifier.Encounter);
            }

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

            var met = pkm.Met_Location;
            if (met == tradeLoc)
                return new CheckResult(Severity.Valid, LEggLocationTrade, CheckIdentifier.Encounter);
            return met == 0
                ? new CheckResult(Severity.Valid, LEggUnhatched, CheckIdentifier.Encounter)
                : new CheckResult(Severity.Invalid, LEggLocationNone, CheckIdentifier.Encounter);
        }

        // Other
        private static CheckResult VerifyEncounterWild(EncounterSlot slot)
        {
            // Check for Unreleased Encounters / Collisions
            switch (slot.Generation)
            {
                case 4:
                    if (slot.Location == 193 && slot.Area.Type == SlotType.Surf) // surfing in Johto Route 45
                        return new CheckResult(Severity.Invalid, LG4InvalidTileR45Surf, CheckIdentifier.Encounter);
                    break;
            }

            var summary = slot.GetConditionString(out bool valid);
            return new CheckResult(valid ? Severity.Valid : Severity.Invalid, summary, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterStatic(PKM pkm, EncounterStatic s)
        {
            // Check for Unreleased Encounters / Collisions
            switch (s.Generation)
            {
                case 3:
                    if (s is EncounterStaticShadow {EReader: true} && pkm.Language != (int)LanguageID.Japanese) // Non-JP E-reader Pokemon
                        return new CheckResult(Severity.Invalid, LG3EReader, CheckIdentifier.Encounter);

                    switch (pkm.Species)
                    {
                        case (int)Species.Mew when s.Location == 201 && pkm.Language != (int)LanguageID.Japanese: // Non-JP Mew (Old Sea Map)
                            return new CheckResult(Severity.Invalid, LEncUnreleasedEMewJP, CheckIdentifier.Encounter);
                        case (int)Species.Deoxys when s.Location == 200 && pkm.Language == (int)LanguageID.Japanese: // JP Deoxys (Birth Island)
                            return new CheckResult(Severity.Invalid, LEncUnreleased, CheckIdentifier.Encounter);
                    }

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
                    if (pkm.Met_Location == 193 && s is EncounterStatic4 {Roaming: true}) // Roaming pokemon surfing in Johto Route 45
                        return new CheckResult(Severity.Invalid, LG4InvalidTileR45Surf, CheckIdentifier.Encounter);
                    break;
                case 7:
                    if (s.EggLocation == Locations.Daycare5 && pkm.RelearnMoves.Any(m => m != 0)) // Eevee gift egg
                        return new CheckResult(Severity.Invalid, LEncStaticRelearn, CheckIdentifier.RelearnMove); // not gift egg
                    break;
            }
            if (s.EggEncounter && !pkm.IsEgg) // hatched
            {
                var hatchCheck = VerifyEncounterEgg(pkm, s.Generation);
                if (!hatchCheck.Valid)
                    return hatchCheck;
            }

            return new CheckResult(Severity.Valid, LEncStaticMatch, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterTrade(ISpeciesForm pkm, EncounterTrade trade)
        {
            if (trade.EvolveOnTrade && trade.Species == pkm.Species)
            {
                // Pokemon that evolve on trade can not be in the phase evolution after the trade
                // If the trade holds an everstone EvolveOnTrade will be false for the encounter
                var species = ParseSettings.SpeciesStrings;
                var unevolved = species[pkm.Species];
                var evolved = species[pkm.Species + 1];
                return new CheckResult(Severity.Invalid, string.Format(LEvoTradeReq, unevolved, evolved), CheckIdentifier.Encounter);
            }
            return new CheckResult(Severity.Valid, LEncTradeMatch, CheckIdentifier.Encounter);
        }

        private static CheckResult VerifyEncounterEvent(PKM pkm, MysteryGift gift)
        {
            switch (gift)
            {
                case PCD pcd:
                    if (!pcd.CanBeReceivedBy(pkm.Version) && pcd.Gift.PK.Version == 0)
                        return new CheckResult(Severity.Invalid, string.Format(L_XMatches0_1, gift.CardHeader, $"-- {LEncGiftVersionNotDistributed}"), CheckIdentifier.Encounter);
                    break;
            }
            if (!pkm.IsEgg && gift.IsEgg) // hatched
            {
                var hatchCheck = VerifyEncounterEgg(pkm, gift.Generation);
                if (!hatchCheck.Valid)
                    return hatchCheck;
            }

            // Strict matching already performed by EncounterGenerator. May be worth moving some checks here to better flag invalid gifts.
            return new CheckResult(Severity.Valid, string.Format(L_XMatches0_1, gift.CardHeader, string.Empty), CheckIdentifier.Encounter);
        }
    }
}
