using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

public static class EncounterVerifier
{
    /// <summary>
    /// Gets the method to verify the <see cref="IEncounterable"/> data.
    /// </summary>
    /// <param name="generation">Source generation to verify</param>
    /// <returns>Returns the verification method appropriate for the input PKM</returns>
    public static Func<PKM, IEncounterable, CheckResult> GetEncounterVerifierMethod(int generation) => generation switch
    {
        1 or 2 => VerifyEncounterG12,
        _ => VerifyEncounter,
    };

    private static CheckResult VerifyEncounter(PKM pk, IEncounterTemplate enc) => enc switch
    {
        EncounterEgg e => VerifyEncounterEgg(pk, e.Generation),
        EncounterTrade t => VerifyEncounterTrade(pk, t),
        EncounterSlot w => VerifyEncounterWild(w),
        EncounterStatic s => VerifyEncounterStatic(pk, s),
        MysteryGift g => VerifyEncounterEvent(pk, g),
        _ => new CheckResult(Severity.Invalid, LEncInvalid, CheckIdentifier.Encounter),
    };

    private static CheckResult VerifyEncounterG12(PKM pk, IEncounterTemplate enc)
    {
        if (enc.EggEncounter)
            return VerifyEncounterEgg(pk, enc.Generation);

        return enc switch
        {
            EncounterSlot1 => new CheckResult(Severity.Valid, LEncCondition, CheckIdentifier.Encounter),
            EncounterSlot2 s2 => VerifyWildEncounterGen2(pk, s2),
            EncounterStatic s => VerifyEncounterStatic(pk, s),
            EncounterTrade t => VerifyEncounterTrade(pk, t),
            _ => new CheckResult(Severity.Invalid, LEncInvalid, CheckIdentifier.Encounter),
        };
    }

    // Gen2 Wild Encounters
    private static CheckResult VerifyWildEncounterGen2(PKM pk, EncounterSlot2 encounter)
    {
        switch (encounter.SlotType)
        {
            case SlotType.Headbutt:
                return VerifyWildEncounterCrystalHeadbutt(pk, encounter);

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

    private static CheckResult VerifyWildEncounterCrystalHeadbutt(ITrainerID tr, EncounterSlot2 s2)
    {
        return s2.IsTreeAvailable(tr.TID)
            ? new CheckResult(Severity.Valid, LG2TreeID, CheckIdentifier.Encounter)
            : new CheckResult(Severity.Invalid, LG2InvalidTileTreeNotFound, CheckIdentifier.Encounter);
    }

    // Eggs
    private static CheckResult VerifyEncounterEgg(PKM pk, int gen) => gen switch
    {
        2 => new CheckResult(CheckIdentifier.Encounter), // valid -- no met location info
        3 => pk.IsEgg ? VerifyUnhatchedEgg3(pk) : VerifyEncounterEgg3(pk),
        4 => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade4) : VerifyEncounterEgg4(pk),
        5 => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade5) : VerifyEncounterEgg5(pk),
        6 => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade6) : VerifyEncounterEgg6(pk),
        7 => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade6) : VerifyEncounterEgg7(pk),
        8 when GameVersion.BDSP.Contains((GameVersion)pk.Version) => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade6NPC, Locations.Default8bNone) : VerifyEncounterEgg8BDSP(pk),
        8 => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade6) : VerifyEncounterEgg8(pk),
        _ => new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter),
    };

    private static CheckResult VerifyUnhatchedEgg3(PKM pk)
    {
        if (pk.Met_Level != 0)
            return new CheckResult(Severity.Invalid, string.Format(LEggFMetLevel_0, 0), CheckIdentifier.Encounter);

        // Only EncounterEgg should reach here.
        var loc = pk.FRLG ? Locations.HatchLocationFRLG : Locations.HatchLocationRSE;
        if (pk.Met_Location != loc)
            return new CheckResult(Severity.Invalid, LEggMetLocationFail, CheckIdentifier.Encounter);

        return new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter);
    }

    private static CheckResult VerifyEncounterEgg3(PKM pk)
    {
        if (pk.Format != 3)
            return VerifyEncounterEgg3Transfer(pk);

        if (pk.Met_Level != 0)
            return new CheckResult(Severity.Invalid, string.Format(LEggFMetLevel_0, 0), CheckIdentifier.Encounter);

        // Check the origin game list.
        var met = (byte)pk.Met_Location;
        var locs = pk.FRLG ? Legal.ValidMet_FRLG : pk.E ? Legal.ValidMet_E : Legal.ValidMet_RS;
        if (locs.Contains(met))
            return new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter);

        // Version isn't updated when hatching on a different game. Check any game.
        if (Legal.ValidMet_FRLG.Contains(met) || Legal.ValidMet_E.Contains(met) || Legal.ValidMet_RS.Contains(met))
            return new CheckResult(Severity.Valid, LEggLocationTrade, CheckIdentifier.Encounter);
        return new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter);
    }

    private static CheckResult VerifyEncounterEgg3Transfer(PKM pk)
    {
        if (pk.IsEgg)
            return new CheckResult(Severity.Invalid, LTransferEgg, CheckIdentifier.Encounter);
        if (pk.Met_Level < 5)
            return new CheckResult(Severity.Invalid, LTransferEggMetLevel, CheckIdentifier.Encounter);

        var expectEgg = pk is PB8 ? Locations.Default8bNone : 0;
        if (pk.Egg_Location != expectEgg)
            return new CheckResult(Severity.Invalid, LEggLocationNone, CheckIdentifier.Encounter);

        if (pk.Format != 4)
        {
            if (pk.Met_Location != Locations.Transfer4)
                return new CheckResult(Severity.Invalid, LTransferEggLocationTransporter, CheckIdentifier.Encounter);
        }
        else
        {
            if (pk.Met_Location != Locations.Transfer3)
                return new CheckResult(Severity.Invalid, LEggLocationPalPark, CheckIdentifier.Encounter);
        }

        return new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter);
    }

    private static CheckResult VerifyEncounterEgg4(PKM pk)
    {
        if (pk.Format == 4)
        {
            // Traded eggs don't update Version, like in future games.
            var locations = pk.WasTradedEgg ? Legal.ValidMet_4 :
                pk.HGSS ? Legal.ValidMet_HGSS :
                pk.Pt ? Legal.ValidMet_Pt :
                Legal.ValidMet_DP;
            return VerifyEncounterEggLevelLoc(pk, 0, locations);
        }
        if (pk.IsEgg)
            return new CheckResult(Severity.Invalid, LTransferEgg, CheckIdentifier.Encounter);

        // transferred
        if (pk.Met_Level < 1)
            return new CheckResult(Severity.Invalid, LTransferEggMetLevel, CheckIdentifier.Encounter);
        if (pk.Met_Location != Locations.Transfer4)
            return new CheckResult(Severity.Invalid, LTransferEggLocationTransporter, CheckIdentifier.Encounter);
        return new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter);
    }

    private static CheckResult VerifyEncounterEgg5(PKM pk)
    {
        return VerifyEncounterEggLevelLoc(pk, 1, pk.B2W2 ? Legal.ValidMet_B2W2 : Legal.ValidMet_BW);
    }

    private static CheckResult VerifyEncounterEgg6(PKM pk)
    {
        if (pk.AO)
            return VerifyEncounterEggLevelLoc(pk, 1, Legal.ValidMet_AO);

        if (pk.Egg_Location == Locations.HatchLocation6AO) // Battle Resort Daycare is only OR/AS.
            return new CheckResult(Severity.Invalid, LEggMetLocationFail, CheckIdentifier.Encounter);

        return VerifyEncounterEggLevelLoc(pk, 1, Legal.ValidMet_XY);
    }

    private static CheckResult VerifyEncounterEgg7(PKM pk)
    {
        if (pk.SM)
            return VerifyEncounterEggLevelLoc(pk, 1, Legal.ValidMet_SM);
        if (pk.USUM)
            return VerifyEncounterEggLevelLoc(pk, 1, Legal.ValidMet_USUM);

        // no other games
        return new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter);
    }

    private static CheckResult VerifyEncounterEgg8(PKM pk)
    {
        if (pk.SWSH)
        {
            if (pk.BDSP)
                return VerifyEncounterEggLevelLoc(pk, 1, (location, game) => location == (game == GameVersion.SW ? Locations.HOME_SWBD : Locations.HOME_SHSP));
            return VerifyEncounterEggLevelLoc(pk, 1, Legal.ValidMet_SWSH);
        }

        // no other games
        return new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter);
    }

    private static CheckResult VerifyEncounterEgg8BDSP(PKM pk)
    {
        if (pk.BDSP)
            return VerifyEncounterEggLevelLoc(pk, 1, Legal.IsValidEggHatchLocation8b);

        // no other games
        return new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter);
    }

    private static CheckResult VerifyEncounterEggLevelLoc(PKM pk, int eggLevel, ICollection<ushort> MetLocations)
    {
        return VerifyEncounterEggLevelLoc(pk, eggLevel, (location, _) => MetLocations.Contains(location));
    }

    // (hatch location, hatch version, bool result)
    private static CheckResult VerifyEncounterEggLevelLoc(PKM pk, int eggLevel, Func<ushort, GameVersion, bool> isValid)
    {
        if (pk.Met_Level != eggLevel)
            return new CheckResult(Severity.Invalid, string.Format(LEggFMetLevel_0, eggLevel), CheckIdentifier.Encounter);
        return isValid((ushort)pk.Met_Location, (GameVersion)pk.Version)
            ? new CheckResult(Severity.Valid, LEggLocation, CheckIdentifier.Encounter)
            : new CheckResult(Severity.Invalid, LEggLocationInvalid, CheckIdentifier.Encounter);
    }

    private static CheckResult VerifyUnhatchedEgg(PKM pk, int tradeLoc, int noneLoc = 0)
    {
        var eggLevel = pk.Format < 5 ? 0 : 1;
        if (pk.Met_Level != eggLevel)
            return new CheckResult(Severity.Invalid, string.Format(LEggFMetLevel_0, eggLevel), CheckIdentifier.Encounter);
        if (pk.Egg_Location == tradeLoc)
            return new CheckResult(Severity.Invalid, LEggLocationTradeFail, CheckIdentifier.Encounter);

        var met = pk.Met_Location;
        if (met == tradeLoc)
            return new CheckResult(Severity.Valid, LEggLocationTrade, CheckIdentifier.Encounter);
        return met == noneLoc
            ? new CheckResult(Severity.Valid, LEggUnhatched, CheckIdentifier.Encounter)
            : new CheckResult(Severity.Invalid, LEggLocationNone, CheckIdentifier.Encounter);
    }

    // Other
    private static CheckResult VerifyEncounterWild(EncounterSlot slot)
    {
        var summary = slot.GetConditionString(out bool valid);
        return new CheckResult(valid ? Severity.Valid : Severity.Invalid, summary, CheckIdentifier.Encounter);
    }

    private static CheckResult VerifyEncounterStatic(PKM pk, EncounterStatic s)
    {
        // Check for Unreleased Encounters / Collisions
        switch (s.Generation)
        {
            case 3:
                if (s is EncounterStaticShadow {EReader: true} && pk.Language != (int)LanguageID.Japanese) // Non-JP E-reader Pokemon
                    return new CheckResult(Severity.Invalid, LG3EReader, CheckIdentifier.Encounter);

                switch (s.Species)
                {
                    case (int)Species.Mew when s.Location == 201 && pk.Language != (int)LanguageID.Japanese: // Non-JP Mew (Old Sea Map)
                        return new CheckResult(Severity.Invalid, LEncUnreleasedEMewJP, CheckIdentifier.Encounter);
                    case (int)Species.Deoxys when s.Location == 200 && pk.Language == (int)LanguageID.Japanese: // JP Deoxys (Birth Island)
                        return new CheckResult(Severity.Invalid, LEncUnreleased, CheckIdentifier.Encounter);
                }

                break;
            case 4:
                if (s is EncounterStatic4 {Roaming: true} && pk.Met_Location == 193 && pk is IGroundTile {GroundTile:GroundTileType.Water}) // Roaming pokemon surfing in Johto Route 45
                    return new CheckResult(Severity.Invalid, LG4InvalidTileR45Surf, CheckIdentifier.Encounter);
                break;
            case 7:
                if (s.EggLocation == Locations.Daycare5 && pk.RelearnMove1 != 0) // Eevee gift egg
                    return new CheckResult(Severity.Invalid, LEncStaticRelearn, CheckIdentifier.RelearnMove); // not gift egg
                break;
        }
        if (s.EggEncounter && !pk.IsEgg) // hatched
        {
            var hatchCheck = VerifyEncounterEgg(pk, s.Generation);
            if (!hatchCheck.Valid)
                return hatchCheck;
        }

        return new CheckResult(Severity.Valid, LEncStaticMatch, CheckIdentifier.Encounter);
    }

    private static CheckResult VerifyEncounterTrade(ISpeciesForm pk, EncounterTrade trade)
    {
        var species = pk.Species;
        if (trade.EvolveOnTrade && trade.Species == species)
        {
            // Pokemon that evolve on trade can not be in the phase evolution after the trade
            // If the trade holds an everstone EvolveOnTrade will be false for the encounter
            var names = ParseSettings.SpeciesStrings;
            var evolved = names[species + 1];
            var unevolved = names[species];
            return new CheckResult(Severity.Invalid, string.Format(LEvoTradeReq, unevolved, evolved), CheckIdentifier.Encounter);
        }
        return new CheckResult(Severity.Valid, LEncTradeMatch, CheckIdentifier.Encounter);
    }

    private static CheckResult VerifyEncounterEvent(PKM pk, MysteryGift gift)
    {
        switch (gift)
        {
            case PCD pcd:
                if (!pcd.CanBeReceivedBy(pk.Version) && pcd.Gift.PK.Version == 0)
                    return new CheckResult(Severity.Invalid, string.Format(L_XMatches0_1, gift.CardHeader, $"-- {LEncGiftVersionNotDistributed}"), CheckIdentifier.Encounter);
                break;
        }
        if (!pk.IsEgg && gift.IsEgg) // hatched
        {
            var hatchCheck = VerifyEncounterEgg(pk, gift.Generation);
            if (!hatchCheck.Valid)
                return hatchCheck;
        }

        // Strict matching already performed by EncounterGenerator. May be worth moving some checks here to better flag invalid gifts.
        return new CheckResult(Severity.Valid, string.Format(L_XMatches0_1, gift.CardHeader, string.Empty), CheckIdentifier.Encounter);
    }
}
