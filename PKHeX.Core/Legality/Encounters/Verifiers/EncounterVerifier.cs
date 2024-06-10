using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies encounter data to check if the encounter really matches the <see cref="PKM"/>.
/// </summary>
public static class EncounterVerifier
{
    /// <summary>
    /// Gets the method to verify the <see cref="IEncounterable"/> data.
    /// </summary>
    /// <param name="generation">Source generation to verify</param>
    /// <returns>Returns the verification method appropriate for the input PKM</returns>
    public static Func<PKM, IEncounterable, CheckResult> GetEncounterVerifierMethod(byte generation) => generation switch
    {
        1 or 2 => VerifyEncounterG12,
        _ => VerifyEncounter,
    };

    private static CheckResult VerifyEncounter(PKM pk, IEncounterTemplate enc) => enc switch
    {
        EncounterEgg e => VerifyEncounterEgg(pk, e.Generation),
        EncounterShadow3Colo { IsEReader: true } when pk.Language != (int)LanguageID.Japanese => GetInvalid(LG3EReader),
        EncounterStatic3 { Species: (int)Species.Mew } when pk.Language != (int)LanguageID.Japanese => GetInvalid(LEncUnreleasedEMewJP),
        EncounterStatic3 { Species: (int)Species.Deoxys, Location: 187 } when pk.Language == (int)LanguageID.Japanese => GetInvalid(LEncUnreleased),
        EncounterStatic4 { Roaming: true } when pk is G4PKM { MetLocation: 193, GroundTile: GroundTileType.Water } => GetInvalid(LG4InvalidTileR45Surf),
        MysteryGift g => VerifyEncounterEvent(pk, g),
        { IsEgg: true } when !pk.IsEgg => VerifyEncounterEgg(pk, enc.Generation),
        EncounterInvalid => GetInvalid(LEncInvalid),
        _ => GetValid(string.Empty), // todo: refactor
    };

    private static CheckResult VerifyEncounterG12(PKM pk, IEncounterTemplate enc)
    {
        if (enc.IsEgg)
            return VerifyEncounterEgg(pk, 2);

        return enc switch
        {
            EncounterSlot1 => GetValid(LEncCondition),
            EncounterSlot2 s2 => VerifyWildEncounterGen2(pk, s2),
            EncounterTrade1 t => VerifyEncounterTrade(pk, t),
            EncounterTrade2 => GetValid(LEncTradeMatch),
            _ => GetValid(string.Empty), // todo: refactor
        };
    }

    // Gen2 Wild Encounters
    private static CheckResult VerifyWildEncounterGen2(ITrainerID16 pk, EncounterSlot2 enc) => enc.Type switch
    {
        SlotType2.Headbutt or SlotType2.HeadbuttSpecial => enc.IsTreeAvailable(pk.TID16)
            ? GetValid(LG2TreeID)
            : GetInvalid(LG2InvalidTileTreeNotFound),
        _ => GetValid(LEncCondition),
    };

    // Eggs
    private static CheckResult VerifyEncounterEgg(PKM pk, byte generation) => generation switch
    {
        2 => pk.IsEgg ? VerifyUnhatchedEgg2(pk) : VerifyEncounterEgg2(pk),
        3 => pk.IsEgg ? VerifyUnhatchedEgg3(pk) : VerifyEncounterEgg3(pk),
        4 => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade4) : VerifyEncounterEgg4(pk),
        5 => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade5) : VerifyEncounterEgg5(pk),
        6 => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade6) : VerifyEncounterEgg6(pk),
        7 => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade6) : VerifyEncounterEgg7(pk),
        8 when GameVersion.BDSP.Contains(pk.Version) => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade6NPC, Locations.Default8bNone) : VerifyEncounterEgg8BDSP(pk),
        8 => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade6) : VerifyEncounterEgg8(pk),
        9 => pk.IsEgg ? VerifyUnhatchedEgg(pk, Locations.LinkTrade6) : VerifyEncounterEgg9(pk),
        _ => GetInvalid(LEggLocationInvalid),
    };

    private static CheckResult VerifyEncounterEgg2(PKM pk)
    {
        if (pk is not ICaughtData2 { CaughtData: not 0 } c2)
            return GetValid(LEggLocation);

        if (c2.MetLevel != 1)
            return GetInvalid(string.Format(LEggFMetLevel_0, 1));

        if (pk.MetLocation > 95)
            return GetInvalid(LEggMetLocationFail);
        // Any met location is fine.
        return GetValid(LEggLocation);
    }

    private static CheckResult VerifyUnhatchedEgg2(PKM pk)
    {
        if (pk is not ICaughtData2 { CaughtData: not 0 } c2)
            return new CheckResult(CheckIdentifier.Encounter);

        if (c2.MetLevel != 1)
            return GetInvalid(string.Format(LEggFMetLevel_0, 1));
        if (c2.MetLocation != 0)
            return GetInvalid(LEggLocationInvalid);
        return GetValid(LEggLocation);
    }

    private static CheckResult VerifyUnhatchedEgg3(PKM pk)
    {
        if (pk.MetLevel != 0)
            return GetInvalid(string.Format(LEggFMetLevel_0, 0));

        // Only EncounterEgg should reach here.
        var loc = pk.FRLG ? Locations.HatchLocationFRLG : Locations.HatchLocationRSE;
        if (pk.MetLocation != loc)
            return GetInvalid(LEggMetLocationFail);

        return GetValid(LEggLocation);
    }

    private static CheckResult VerifyEncounterEgg3(PKM pk)
    {
        if (pk.Format != 3)
            return VerifyEncounterEgg3Transfer(pk);

        if (pk.MetLevel != 0)
            return GetInvalid(string.Format(LEggFMetLevel_0, 0));

        // Check the origin game list.
        var met = (byte)pk.MetLocation;
        bool valid = EggHatchLocation3.IsValidMet3(met, pk.Version);
        if (valid)
            return GetValid(LEggLocation);

        // Version isn't updated when hatching on a different game. Check any game.
        if (EggHatchLocation3.IsValidMet3Any(met))
            return GetValid(LEggLocationTrade);
        return GetInvalid(LEggLocationInvalid);
    }

    private static CheckResult GetInvalid(string message, CheckIdentifier ident = CheckIdentifier.Encounter) => new(Severity.Invalid, ident, message);
    private static CheckResult GetValid(string message) => new(Severity.Valid, CheckIdentifier.Encounter, message);

    private static CheckResult VerifyEncounterEgg3Transfer(PKM pk)
    {
        if (pk.IsEgg)
            return GetInvalid(LTransferEgg);
        if (pk.MetLevel < 5)
            return GetInvalid(LTransferEggMetLevel);

        var expectEgg = pk is PB8 ? Locations.Default8bNone : 0;
        if (pk.EggLocation != expectEgg)
            return GetInvalid(LEggLocationNone);

        if (pk.Format != 4)
        {
            if (pk.MetLocation != Locations.Transfer4)
                return GetInvalid(LTransferEggLocationTransporter);
        }
        else
        {
            if (pk.MetLocation != Locations.Transfer3)
                return GetInvalid(LEggLocationPalPark);
        }

        return GetValid(LEggLocation);
    }

    private static CheckResult VerifyEncounterEgg4(PKM pk)
    {
        if (pk.Format != 4) // transferred
        {
            if (pk.IsEgg)
                return GetInvalid(LTransferEgg);
            if (pk.MetLevel < 1)
                return GetInvalid(LTransferEggMetLevel);
            if (pk.MetLocation != Locations.Transfer4)
                return GetInvalid(LTransferEggLocationTransporter);
            return GetValid(LEggLocation);
        }

        // Native
        const byte level = EggStateLegality.EggMetLevel34;
        if (pk.MetLevel != level)
            return GetInvalid(string.Format(LEggFMetLevel_0, level));

        var met = pk.MetLocation;
        bool valid = EggHatchLocation4.IsValidMet4(met, pk.Version);
        if (valid)
            return GetValid(LEggLocation);

        // Version isn't updated when hatching on a different game. Check any game.
        if (EggHatchLocation4.IsValidMet4Any(met))
            return GetValid(LEggLocationTrade);
        return GetInvalid(LEggLocationInvalid);
    }

    private static CheckResult VerifyEncounterEgg5(PKM pk)
    {
        const byte level = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != level)
            return GetInvalid(string.Format(LEggFMetLevel_0, level));

        var met = pk.MetLocation;
        bool valid = EggHatchLocation5.IsValidMet5(met, pk.Version);

        if (valid)
            return GetValid(LEggLocation);
        return GetInvalid(LEggLocationInvalid);
    }

    private static CheckResult VerifyEncounterEgg6(PKM pk)
    {
        const byte level = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != level)
            return GetInvalid(string.Format(LEggFMetLevel_0, level));

        var met = pk.MetLocation;
        bool valid = pk.XY
            ? EggHatchLocation6.IsValidMet6XY(met)
            : EggHatchLocation6.IsValidMet6AO(met);

        if (valid)
            return GetValid(LEggLocation);
        return GetInvalid(LEggLocationInvalid);
    }

    private static CheckResult VerifyEncounterEgg7(PKM pk)
    {
        const byte level = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != level)
            return GetInvalid(string.Format(LEggFMetLevel_0, level));

        var met = pk.MetLocation;
        bool valid = pk.SM
            ? EggHatchLocation7.IsValidMet7SM(met)
            : EggHatchLocation7.IsValidMet7USUM(met);

        if (valid)
            return GetValid(LEggLocation);
        return GetInvalid(LEggLocationInvalid);
    }

    private static CheckResult VerifyEncounterEgg8(PKM pk)
    {
        const byte level = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != level)
            return GetInvalid(string.Format(LEggFMetLevel_0, level));

        var valid = IsValidMetForeignEggSWSH(pk, pk.MetLocation);
        if (valid)
            return GetValid(LEggLocation);
        return GetInvalid(LEggLocationInvalid);
    }

    private static bool IsValidMetForeignEggSWSH(PKM pk, ushort met)
    {
        if (pk.BDSP)
            return LocationsHOME.IsValidMetBDSP(met, pk.Version);
        if (pk.SV)
            return LocationsHOME.IsValidMetSV(met, pk.Version);
        return EggHatchLocation8.IsValidMet8SWSH(met);
    }

    private static CheckResult VerifyEncounterEgg8BDSP(PKM pk)
    {
        if (pk is PK8)
            return VerifyEncounterEgg8(pk);

        const byte level = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != level)
            return GetInvalid(string.Format(LEggFMetLevel_0, level));

        var met = pk.MetLocation;
        bool valid = pk.Version == GameVersion.BD
            ? EggHatchLocation8b.IsValidMet8BD(met)
            : EggHatchLocation8b.IsValidMet8SP(met);

        if (valid)
            return GetValid(LEggLocation);
        return GetInvalid(LEggLocationInvalid);
    }

    private static CheckResult VerifyEncounterEgg9(PKM pk)
    {
        if (pk is PK8)
            return VerifyEncounterEgg8(pk);

        const byte level = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != level)
            return GetInvalid(string.Format(LEggFMetLevel_0, level));

        var met = pk.MetLocation;
        bool valid = pk.Version == GameVersion.SL
            ? EggHatchLocation9.IsValidMet9SL(met)
            : EggHatchLocation9.IsValidMet9VL(met);

        if (valid)
            return GetValid(LEggLocation);
        return GetInvalid(LEggLocationInvalid);
    }

    private static CheckResult VerifyUnhatchedEgg(PKM pk, int tradeLoc, int noneLoc = 0)
    {
        var eggLevel = pk.Format is 3 or 4 ? EggStateLegality.EggMetLevel34 : EggStateLegality.EggMetLevel;
        if (pk.MetLevel != eggLevel)
            return GetInvalid(string.Format(LEggFMetLevel_0, eggLevel));
        if (pk.EggLocation == tradeLoc)
            return GetInvalid(LEggLocationTradeFail);

        var met = pk.MetLocation;
        if (met == tradeLoc)
            return GetValid(LEggLocationTrade);
        return met == noneLoc
            ? GetValid(LEggUnhatched)
            : GetInvalid(LEggLocationNone);
    }

    private static CheckResult VerifyEncounterTrade(ISpeciesForm pk, EncounterTrade1 trade)
    {
        var species = pk.Species;
        if (trade.EvolveOnTrade && trade.Species == species)
        {
            // Pokémon that evolve on trade can not be in the phase evolution after the trade
            // If the trade holds an Everstone, EvolveOnTrade will be false for the encounter
            // No need to range check the species, as it matched to a valid encounter species.
            var names = ParseSettings.SpeciesStrings;
            var evolved = names[species + 1];
            var unevolved = names[species];
            return GetInvalid(string.Format(LEvoTradeReq, unevolved, evolved));
        }
        return GetValid(LEncTradeMatch);
    }

    private static CheckResult VerifyEncounterEvent(PKM pk, MysteryGift gift)
    {
        switch (gift)
        {
            case PCD pcd:
                if (!pcd.CanBeReceivedByVersion(pk.Version) && pcd.Gift.PK.Version == 0)
                    return GetInvalid(string.Format(L_XMatches0_1, gift.CardHeader, $"-- {LEncGiftVersionNotDistributed}"));
                break;
        }
        if (!pk.IsEgg && gift.IsEgg) // hatched
        {
            var hatchCheck = VerifyEncounterEgg(pk, gift.Generation);
            if (!hatchCheck.Valid)
                return hatchCheck;
        }

        // Strict matching already performed by EncounterGenerator. May be worth moving some checks here to better flag invalid gifts.
        return GetValid(string.Format(L_XMatches0_1, gift.CardHeader, string.Empty));
    }
}
