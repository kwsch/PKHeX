using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the transfer data for a <see cref="PKM"/> that has been irreversibly transferred forward.
/// </summary>
public sealed class TransferVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Encounter;

    public override void Verify(LegalityAnalysis data)
    {
        throw new Exception("Don't call via this.");
    }

    public void VerifyTransferLegalityG12(LegalityAnalysis data)
    {
        VerifyVCOTGender(data);
        VerifyVCNatureEXP(data);
        VerifyVCShinyXorIfShiny(data);
        VerifyVCGeolocation(data);
    }

    private void VerifyVCOTGender(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.OriginalTrainerGender == 1 && pk.Version != GameVersion.C)
            data.AddLine(GetInvalid(LG2OTGender));
    }

    private void VerifyVCNatureEXP(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var met = pk.MetLevel;

        if (met == 100) // check for precise match, can't receive EXP after transfer.
        {
            var nature = Experience.GetNatureVC(pk.EXP);
            if (nature != pk.Nature)
                data.AddLine(GetInvalid(LTransferNature));
            return;
        }
        if (met <= 2) // Not enough EXP to have every nature -- check for exclusions!
        {
            var pi = pk.PersonalInfo;
            var growth = pi.EXPGrowth;
            var nature = pk.Nature;
            bool valid = VerifyVCNature(growth, nature);
            if (!valid)
                data.AddLine(GetInvalid(LTransferNature));
        }
    }

    private static bool VerifyVCNature(byte growth, Nature nature) => growth switch
    {
        // exp % 25 with a limited amount of EXP does not allow for every nature
        0 => (0x01FFFF03u & (1u << (byte)nature)) != 0, // MediumFast -- Can't be Brave, Adamant, Naughty, Bold, Docile, or Relaxed
        4 => (0x001FFFC0u & (1u << (byte)nature)) != 0, // Fast -- Can't be Gentle, Sassy, Careful, Quirky, Hardy, Lonely, Brave, Adamant, Naughty, or Bold
        5 => (0x01FFFCFFu & (1u << (byte)nature)) != 0, // Slow -- Can't be Impish or Lax
        _ => true,
    };

    private static void VerifyVCShinyXorIfShiny(LegalityAnalysis data)
    {
        // Star, not square. Requires transferring a shiny and having the initially random PID to already be a Star shiny.
        // (15:65536, ~1:4096) odds on a given shiny transfer!
        var xor = data.Entity.ShinyXor;
        if (xor is <= 15 and not 0)
            data.AddLine(Get(LEncStaticPIDShiny, ParseSettings.Settings.Game.Gen7.Gen7TransferStarPID, CheckIdentifier.PID));
    }

    private static void VerifyVCGeolocation(LegalityAnalysis data)
    {
        if (data.Entity is not PK7 pk7)
            return;

        // VC Games were region locked to the Console, meaning not all language games are available.
        var within = Locale3DS.IsRegionLockedLanguageValidVC(pk7.ConsoleRegion, pk7.Language);
        if (!within)
            data.AddLine(GetInvalid(string.Format(LOTLanguage, $"!={(LanguageID)pk7.Language}", ((LanguageID)pk7.Language).ToString()), CheckIdentifier.Language));
    }

    public void VerifyTransferLegalityG3(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Format == 4) // Pal Park (3->4)
        {
            if (pk.MetLocation != Locations.Transfer3)
                data.AddLine(GetInvalid(LEggLocationPalPark));
        }
        else // Transporter (4->5)
        {
            if (pk.MetLocation != Locations.Transfer4)
                data.AddLine(GetInvalid(LTransferEggLocationTransporter));
        }
    }

    public void VerifyTransferLegalityG4(LegalityAnalysis data)
    {
        var pk = data.Entity;
        ushort loc = pk.MetLocation;
        if (loc == Locations.Transfer4)
            return;

        // Crown met location must be present if transferred via lock capsule
        switch (pk.Species)
        {
            case (int)Species.Celebi:
                if (loc is not (Locations.Transfer4_CelebiUnused or Locations.Transfer4_CelebiUsed))
                    data.AddLine(GetInvalid(LTransferMet));
                break;
            case (int)Species.Raikou or (int)Species.Entei or (int)Species.Suicune:
                if (loc is not (Locations.Transfer4_CrownUnused or Locations.Transfer4_CrownUsed))
                    data.AddLine(GetInvalid(LTransferMet));
                break;
            default:
                data.AddLine(GetInvalid(LTransferEggLocationTransporter));
                break;
        }
    }

    public void VerifyTransferLegalityG8(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterMatch;
        bool required = HomeTrackerUtil.IsRequired(enc, pk);
        if (required)
            VerifyHOMETracker(data, pk);

        if (enc.Generation < 8)
        {
            VerifyHOMETransfer(data, pk);
            // Check for impossible 7->8 transfers
            if (enc is EncounterStatic7 { IsTotem: true } s)
            {
                if (s.IsTotemNoTransfer)
                    data.AddLine(GetInvalid(LTransferBad));
                else if (pk.Form != s.GetTotemBaseForm())
                    data.AddLine(GetInvalid(LTransferBad));
            }
        }

        // Starting in Generation 8, games have a selective amount of species/forms from prior games.
        IPersonalTable pt = pk switch
        {
            PA8 => PersonalTable.LA,
            PB8 => PersonalTable.BDSP,
            PK9 => PersonalTable.SV,
            _ => PersonalTable.SWSH,
        };
        if (!pt.IsPresentInGame(pk.Species, pk.Form))
            data.AddLine(GetInvalid(LTransferBad));
    }

    private void VerifyHOMETransfer(LegalityAnalysis data, PKM pk)
    {
        if (pk is not IScaledSize s)
            return;

        if (pk.LGPE || pk.GO)
            return; // can have any size value

        // HOME in 3.0.0 will actively re-roll (0,0) to non-zero values.
        // Transfer between Gen8 can be done before HOME 3.0.0, so we can allow (0,0) or re-rolled.
        if (pk.Context is not (EntityContext.Gen8 or EntityContext.Gen8a or EntityContext.Gen8b))
        {
            if (s is { HeightScalar: 0, WeightScalar: 0 } && !data.Info.EvoChainsAllGens.HasVisitedPLA)
                data.AddLine(GetInvalid(LTransferBad));
        }
    }

    private void VerifyHOMETracker(LegalityAnalysis data, PKM pk)
    {
        // Tracker value is set via Transfer across HOME.
        // Can't validate the actual values (we aren't the server), so we can only check against zero.
        if (pk is IHomeTrack { HasTracker: false })
        {
            data.AddLine(Get(LTransferTrackerMissing, ParseSettings.Settings.HOMETransfer.HOMETransferTrackerNotPresent));
            // To the reader: It seems like the best course of action for setting a tracker is:
            // - Transfer a 0-Tracker pk to HOME to get assigned a valid Tracker
            // - Don't make one up.
        }
    }

    public void VerifyVCEncounter(PKM pk, IEncounterTemplate original, EncounterTransfer7 transfer, LegalityAnalysis data)
    {
        if (pk.MetLocation != transfer.Location)
            data.AddLine(GetInvalid(LTransferMetLocation));

        var expectEgg = pk is PB8 ? Locations.Default8bNone : transfer.EggLocation;
        if (pk.EggLocation != expectEgg)
            data.AddLine(GetInvalid(LEggLocationNone));

        // Flag Moves that cannot be transferred
        if (original is EncounterStatic2 { DizzyPunchEgg: true}) // Dizzy Punch Gifts
            FlagIncompatibleTransferMove(pk, data.Info.Moves, 146, 2); // can't have Dizzy Punch at all

        bool checkShiny = pk.VC2 || (pk.VC1 && GBRestrictions.IsTimeCapsuleTransferred(pk, data.Info.Moves, original).WasTimeCapsuleTransferred());
        if (!checkShiny)
            return;

        if (pk.Gender == 1) // female
        {
            var enc = data.EncounterOriginal;
            var pi = PersonalTable.USUM[enc.Species];
            if (pi.Gender == 31 && pk.IsShiny) // impossible gender-shiny
                data.AddLine(GetInvalid(LEncStaticPIDShiny, CheckIdentifier.PID));
        }
        else if (pk.Species == (int)Species.Unown)
        {
            if (pk.Form is not (8 or 21) && pk.IsShiny) // impossibly form-shiny (not I or V)
                data.AddLine(GetInvalid(LEncStaticPIDShiny, CheckIdentifier.PID));
        }
    }

    private static void FlagIncompatibleTransferMove(PKM pk, Span<MoveResult> parse, ushort move, byte generation)
    {
        int index = pk.GetMoveIndex(move);
        if (index < 0)
            return; // doesn't have move

        if (parse[index].Generation == generation) // not obtained from a future gen
            parse[index] = MoveResult.Unobtainable(0);
    }
}
