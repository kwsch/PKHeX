using System;
using static PKHeX.Core.LegalityCheckResultCode;

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
        EncounterShadow3Colo { IsEReader: true } when pk.Language != (int)LanguageID.Japanese => GetInvalid(G3EReader),
        EncounterStatic3 { Species: (int)Species.Mew } when pk.Language != (int)LanguageID.Japanese => GetInvalid(EncUnreleasedEMewJP),
        EncounterStatic3 { Species: (int)Species.Deoxys, Location: 200 } when pk.Language == (int)LanguageID.Japanese => GetInvalid(EncUnreleased),
        EncounterStatic4 { Species: (int)Species.Shaymin } when pk.Language == (int)LanguageID.Korean => GetInvalid(EncUnreleased),
        EncounterStatic4 { IsRoaming: true } when pk is G4PKM { MetLocation: 193, GroundTile: GroundTileType.Water } => GetInvalid(G4InvalidTileR45Surf),
        MysteryGift g => VerifyEncounterEvent(pk, g),
        IEncounterEgg e when pk.IsEgg => VerifyEncounterEggUnhatched(pk, e),
        { IsEgg: true } when !pk.IsEgg => VerifyEncounterEggHatched(pk, enc.Context),
        EncounterInvalid => GetInvalid(EncInvalid),
        _ => GetValid(Valid),
    };

    private static CheckResult VerifyEncounterG12(PKM pk, IEncounterTemplate enc)
    {
        if (enc.IsEgg)
            return pk.IsEgg ? VerifyUnhatchedEgg2(pk) : VerifyEncounterEgg2(pk);

        return enc switch
        {
            EncounterSlot1 => GetValid(EncCondition),
            EncounterSlot2 s2 => VerifyWildEncounterGen2(pk, s2),
            EncounterTrade1 t => VerifyEncounterTrade(pk, t),
            EncounterTrade2 => GetValid(EncTradeMatch),
            _ => GetValid(Valid),
        };
    }

    // Gen2 Wild Encounters
    private static CheckResult VerifyWildEncounterGen2(ITrainerID16 pk, EncounterSlot2 enc) => enc.Type switch
    {
        SlotType2.Headbutt or SlotType2.HeadbuttSpecial => enc.IsTreeAvailable(pk.TID16)
            ? GetValid(G2TreeID)
            : GetInvalid(G2InvalidTileTreeNotFound),
        _ => GetValid(EncCondition),
    };

    // Eggs
    private static CheckResult VerifyEncounterEggUnhatched(PKM pk, IEncounterEgg egg) => egg switch
    {
        EncounterEgg2 => VerifyUnhatchedEgg2(pk),
        EncounterEgg3 => VerifyUnhatchedEgg3(pk),
        EncounterEgg4 => VerifyUnhatchedEgg(pk, Locations.LinkTrade4),
        EncounterEgg5 => VerifyUnhatchedEgg5(pk),
        EncounterEgg6 => VerifyUnhatchedEgg(pk, Locations.LinkTrade6),
        EncounterEgg7 => VerifyUnhatchedEgg(pk, Locations.LinkTrade6),
        EncounterEgg8b=> VerifyUnhatchedEgg(pk, Locations.LinkTrade6NPC, Locations.Default8bNone),
        EncounterEgg8 => VerifyUnhatchedEgg(pk, Locations.LinkTrade6),
        EncounterEgg9 => VerifyUnhatchedEgg(pk, Locations.LinkTrade6),
        _ => GetInvalid(EggLocationInvalid),
    };

    private static CheckResult VerifyEncounterEggHatched(PKM pk, EntityContext context) => context switch
    {
        EntityContext.Gen2 => VerifyEncounterEgg2(pk),
        EntityContext.Gen3 => VerifyEncounterEgg3(pk),
        EntityContext.Gen4 => VerifyEncounterEgg4(pk),
        EntityContext.Gen5 => VerifyEncounterEgg5(pk),
        EntityContext.Gen6 => VerifyEncounterEgg6(pk),
        EntityContext.Gen7 => VerifyEncounterEgg7(pk),
        EntityContext.Gen8b=> VerifyEncounterEgg8BDSP(pk),
        EntityContext.Gen8 => VerifyEncounterEgg8(pk),
        EntityContext.Gen9 => VerifyEncounterEgg9(pk),
        _ => GetInvalid(EggLocationInvalid),
    };

    private static CheckResult VerifyEncounterEgg2(PKM pk)
    {
        if (pk is not ICaughtData2 { CaughtData: not 0 } c2)
            return GetValid(EggLocation);

        if (c2.MetLevel != EggStateLegality.EggMetLevel)
            return GetInvalid(EggFMetLevel_0, EggStateLegality.EggMetLevel);

        if (pk.MetLocation > 95)
            return GetInvalid(EggMetLocationFail);
        // Any met location is fine.
        return GetValid(EggLocation);
    }

    private static CheckResult VerifyUnhatchedEgg2(PKM pk)
    {
        if (pk is not ICaughtData2 { CaughtData: not 0 } c2)
            return CheckResult.GetValid(CheckIdentifier.Encounter);

        if (c2.MetLevel != EggStateLegality.EggMetLevel)
            return GetInvalid(EggFMetLevel_0, EggStateLegality.EggMetLevel);
        if (c2.MetLocation != 0)
            return GetInvalid(EggLocationInvalid);
        return GetValid(EggLocation);
    }

    private static CheckResult VerifyUnhatchedEgg3(PKM pk)
    {
        if (pk.MetLevel != EggStateLegality.EggMetLevel34)
            return GetInvalid(EggFMetLevel_0, EggStateLegality.EggMetLevel34);

        // Only EncounterEgg should reach here.
        var loc = pk.FRLG ? Locations.HatchLocationFRLG : Locations.HatchLocationRSE;
        if (pk.MetLocation != loc)
            return GetInvalid(EggMetLocationFail);

        return GetValid(EggLocation);
    }

    private static CheckResult VerifyEncounterEgg3(PKM pk)
    {
        if (pk.Format != 3)
            return VerifyEncounterEgg3Transfer(pk);

        if (pk.MetLevel != EggStateLegality.EggMetLevel34)
            return GetInvalid(EggFMetLevel_0, EggStateLegality.EggMetLevel34);

        // Check the origin game list.
        var met = (byte)pk.MetLocation;

        // Emerald Location IDs: Birth Island is inaccessible for Japanese, and Faraway Island is inaccessible for non-Japanese.
        byte inaccessible = pk.Japanese ? (byte)200 : (byte)201;
        if (met == inaccessible)
            return GetInvalid(EggLocationInvalid, met);

        bool valid = EggHatchLocation3.IsValidMet3(met, pk.Version);
        if (valid)
            return GetValid(EggLocation);

        // Version isn't updated when hatching on a different game. Check any game.
        if (EggHatchLocation3.IsValidMet3Any(met))
            return GetValid(EggLocationTrade);
        return GetInvalid(EggLocationInvalid);
    }

    private static CheckResult GetInvalid(LegalityCheckResultCode message, CheckIdentifier ident = CheckIdentifier.Encounter) => CheckResult.Get(Severity.Invalid, ident, message);
    private static CheckResult GetInvalid(LegalityCheckResultCode message, byte value, CheckIdentifier ident = CheckIdentifier.Encounter) => CheckResult.Get(Severity.Invalid, ident, message, value);
    private static CheckResult GetValid(LegalityCheckResultCode message) => CheckResult.Get(Severity.Valid, CheckIdentifier.Encounter, message);

    private static CheckResult VerifyEncounterEgg3Transfer(PKM pk)
    {
        if (pk.IsEgg)
            return GetInvalid(TransferEgg);
        if (pk.MetLevel < EggStateLegality.EggLevel23)
            return GetInvalid(TransferEggMetLevel);

        var expectEgg = pk is PB8 ? Locations.Default8bNone : 0;
        if (pk.EggLocation != expectEgg)
            return GetInvalid(EggLocationNone);

        if (pk.Format != 4)
        {
            if (pk.MetLocation != Locations.Transfer4)
                return GetInvalid(TransferEggLocationTransporter);
        }
        else
        {
            if (pk.MetLocation != Locations.Transfer3)
                return GetInvalid(EggLocationPalPark);
        }

        return GetValid(EggLocation);
    }

    private static CheckResult VerifyEncounterEgg4(PKM pk)
    {
        if (pk.Format != 4) // transferred
        {
            if (pk.IsEgg)
                return GetInvalid(TransferEgg);
            if (pk.MetLevel < EggStateLegality.EggLevel)
                return GetInvalid(TransferEggMetLevel);
            if (pk.MetLocation != Locations.Transfer4)
                return GetInvalid(TransferEggLocationTransporter);
            return GetValid(EggLocation);
        }

        // Native
        const byte level = EggStateLegality.EggMetLevel34;
        if (pk.MetLevel != level)
            return GetInvalid(EggFMetLevel_0, level);

        var met = pk.MetLocation;

        // Shaymin event was never distributed in Korea. Cannot hatch in Seabreak Path or Flower Paradise.
        if (pk.Language == (int)LanguageID.Korean && met is 63 or 85)
            return GetInvalid(EggLocationInvalid);

        bool valid = EggHatchLocation4.IsValidMet4(met, pk.Version);
        if (valid)
            return GetValid(EggLocation);

        // Version isn't updated when hatching on a different game. Check any game.
        if (pk.EggLocation == Locations.LinkTrade4 && EggHatchLocation4.IsValidMet4Any(met))
            return GetValid(EggLocationTrade);
        return GetInvalid(EggLocationInvalid);
    }

    private static CheckResult VerifyEncounterEgg5(PKM pk)
    {
        const byte level = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != level)
            return GetInvalid(EggFMetLevel_0, level);

        var met = pk.MetLocation;
        bool valid = EggHatchLocation5.IsValidMet5(met, pk.Version);

        if (valid)
            return GetValid(EggLocation);
        return GetInvalid(EggLocationInvalid);
    }

    private static CheckResult VerifyEncounterEgg6(PKM pk)
    {
        const byte level = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != level)
            return GetInvalid(EggFMetLevel_0, level);

        var met = pk.MetLocation;
        bool valid = pk.XY
            ? EggHatchLocation6.IsValidMet6XY(met)
            : EggHatchLocation6.IsValidMet6AO(met);

        if (valid)
            return GetValid(EggLocation);
        return GetInvalid(EggLocationInvalid);
    }

    private static CheckResult VerifyEncounterEgg7(PKM pk)
    {
        const byte level = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != level)
            return GetInvalid(EggFMetLevel_0, level);

        var met = pk.MetLocation;
        bool valid = pk.SM
            ? EggHatchLocation7.IsValidMet7SM(met)
            : EggHatchLocation7.IsValidMet7USUM(met);

        if (valid)
            return GetValid(EggLocation);
        return GetInvalid(EggLocationInvalid);
    }

    private static CheckResult VerifyEncounterEgg8(PKM pk)
    {
        const byte level = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != level)
            return GetInvalid(EggFMetLevel_0, level);

        var valid = IsValidMetForeignEggSWSH(pk, pk.MetLocation);
        if (valid)
            return GetValid(EggLocation);
        return GetInvalid(EggLocationInvalid);
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
            return GetInvalid(EggFMetLevel_0, level);

        var met = pk.MetLocation;
        bool valid = pk.Version == GameVersion.BD
            ? EggHatchLocation8b.IsValidMet8BD(met)
            : EggHatchLocation8b.IsValidMet8SP(met);

        if (valid)
            return GetValid(EggLocation);
        return GetInvalid(EggLocationInvalid);
    }

    private static CheckResult VerifyEncounterEgg9(PKM pk)
    {
        if (pk is PK8)
            return VerifyEncounterEgg8(pk);

        const byte level = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != level)
            return GetInvalid(EggFMetLevel_0, level);

        var met = pk.MetLocation;
        bool valid = pk.Version == GameVersion.SL
            ? EggHatchLocation9.IsValidMet9SL(met)
            : EggHatchLocation9.IsValidMet9VL(met);

        if (valid)
            return GetValid(EggLocation);
        return GetInvalid(EggLocationInvalid);
    }

    private static CheckResult VerifyUnhatchedEgg(PKM pk, int tradeLoc, ushort noneLoc = 0)
    {
        var eggLevel = pk.Format is 3 or 4 ? EggStateLegality.EggMetLevel34 : EggStateLegality.EggMetLevel;
        if (pk.MetLevel != eggLevel)
            return GetInvalid(EggFMetLevel_0, eggLevel);
        if (pk.EggLocation == tradeLoc)
            return GetInvalid(EggLocationTradeFail);

        var met = pk.MetLocation;
        if (met == tradeLoc)
            return GetValid(EggLocationTrade);
        return met == noneLoc
            ? GetValid(EggUnhatched)
            : GetInvalid(EggLocationNone);
    }

    private static CheckResult VerifyUnhatchedEgg5(PKM pk)
    {
        const byte eggLevel = EggStateLegality.EggMetLevel;
        if (pk.MetLevel != eggLevel)
            return GetInvalid(EggFMetLevel_0, eggLevel);
        if (pk.EggLocation is (Locations.LinkTrade5 or Locations.LinkTrade5NPC))
            return GetInvalid(EggLocationTradeFail);

        var met = pk.MetLocation;
        if (met is (Locations.LinkTrade5 or Locations.LinkTrade5NPC))
            return GetValid(EggLocationTrade);
        return met == 0
            ? GetValid(EggUnhatched)
            : GetInvalid(EggLocationNone);
    }

    private static CheckResult VerifyEncounterTrade(ISpeciesForm pk, EncounterTrade1 trade)
    {
        var species = pk.Species;
        if (trade.EvolveOnTrade && trade.Species == species)
        {
            // Pokémon that evolve on trade can not be in the phase evolution after the trade
            // If the trade holds an Everstone, EvolveOnTrade will be false for the encounter
            // No need to range check the species, as it matched to a valid encounter species.
            return GetInvalid(EncTradeShouldHaveEvolvedToSpecies_0);
        }
        return GetValid(EncTradeMatch);
    }

    private static CheckResult VerifyEncounterEvent(PKM pk, MysteryGift gift)
    {
        if (!pk.IsEgg && gift.IsEgg) // hatched
        {
            var hatchCheck = VerifyEncounterEggHatched(pk, gift.Context);
            if (!hatchCheck.Valid)
                return hatchCheck;
        }

        // Strict matching already performed by EncounterGenerator. May be worth moving some checks here to better flag invalid gifts.
        return GetValid(Valid);
    }
}
