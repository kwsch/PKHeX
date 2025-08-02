using System;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.EncryptionConstant"/>.
/// </summary>
public sealed class PIDVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.PID;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Format >= 6)
            VerifyEC(data);

        var enc = data.EncounterMatch;
        if (enc.Species == (int)Species.Wurmple)
            VerifyECPIDWurmple(data);
        else if (enc.Species is (int)Species.Tandemaus or (int)Species.Dunsparce)
            VerifyEC100(data, enc.Species);

        if (pk.PID == 0)
            data.AddLine(Get(Severity.Fishy, PIDZero));
        if (!pk.Nature.IsFixed()) // out of range
            data.AddLine(GetInvalid(PIDNatureMismatch));
        if (data.Info.EncounterMatch is IEncounterEgg egg)
            VerifyEggPID(data, pk, egg);

        VerifyShiny(data);
    }

    private static void VerifyEggPID(LegalityAnalysis data, PKM pk, IEncounterEgg egg)
    {
        if (egg is EncounterEgg4 e4)
        {
            // Gen4 Eggs are "egg available" based on the stored PID value in the save file.
            // If this value is 0 or is generated as 0 (possible), the game will see "false" and no egg is available.
            // Only a non-zero value is possible to obtain.
            // However, With Masuda Method, the egg PID is re-rolled with the ARNG (until shiny, at most 4 times) upon receipt.
            // None of the un-rolled states share the same shiny-xor as PID=0, you can re-roll into an all-zero PID.
            // Flag it as fishy, because more often than not, it is hacked rather than a legitimately obtained egg.
            if (pk.EncryptionConstant == 0)
                data.AddLine(Get(CheckIdentifier.EC, Severity.Fishy, PIDEncryptZero));

            if (Breeding.IsGenderSpeciesDetermination(e4.Species))
                VerifyEggGender8000(data, pk);
        }
        else if (egg is EncounterEgg3 e3)
        {
            if (!Daycare3.IsValidProcPID(pk.EncryptionConstant, e3.Version))
                data.AddLine(Get(CheckIdentifier.EC, Severity.Invalid, PIDEncryptZero));

            if (Breeding.IsGenderSpeciesDetermination(e3.Species))
                VerifyEggGender8000(data, pk);
            // PID and IVs+Inheritance randomness is sufficiently random; any permutation of vBlank correlations is possible.
        }
    }

    private static void VerifyEggGender8000(LegalityAnalysis data, PKM pk)
    {
        var gender = pk.Gender;
        if (Breeding.IsValidSpeciesBit34(pk.EncryptionConstant, gender))
            return; // 50/50 chance!
        if (gender == 1 || IsEggBitRequiredMale34(data.Info.Moves))
            data.AddLine(GetInvalid(CheckIdentifier.EC, PIDGenderMismatch));
    }

    private void VerifyShiny(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterMatch;

        if (!enc.Shiny.IsValid(pk))
            data.AddLine(GetInvalid(CheckIdentifier.Shiny, EncStaticPIDShiny));

        switch (enc)
        {
            // Forced PID or generated without an encounter
            case IGeneration { Generation: 5 } and IFixedGender { Gender: 0 or 1 } fg:
                if (enc is not PGF && !MonochromeRNG.IsValidForcedRandomGender(pk.EncryptionConstant, fg.Gender))
                    data.AddLine(GetInvalid(CheckIdentifier.PID, EncConditionBadRNGFrame));

                if (enc is EncounterStatic5 { IsWildCorrelationPID: true })
                    VerifyG5PID_IDCorrelation(data);
                break;
            case EncounterStatic5 { IsWildCorrelationPID: true }:
                VerifyG5PID_IDCorrelation(data);
                break;

            case EncounterSlot5 {IsHiddenGrotto: true} when pk.IsShiny:
                data.AddLine(GetInvalid(CheckIdentifier.Shiny, G5PIDShinyGrotto));
                break;
            case EncounterSlot5 {IsHiddenGrotto: false}:
                VerifyG5PID_IDCorrelation(data);
                break;

            case PCD d: // fixed PID
                if (d.IsFixedPID() && pk.EncryptionConstant != d.Gift.PK.PID)
                    data.AddLine(GetInvalid(CheckIdentifier.Shiny, EncGiftPIDMismatch));
                break;

            case WC7 { IsAshGreninja: true } when pk.IsShiny:
                data.AddLine(GetInvalid(CheckIdentifier.Shiny, EncGiftShinyMismatch));
                break;
            // Underground Raids are originally anti-shiny on encounter.
            // When selecting a prize at the end, the game rolls and force-shiny is applied to be XOR=1.
            case EncounterStatic8U u when !u.IsShinyXorValid(pk.ShinyXor):
                data.AddLine(GetInvalid(CheckIdentifier.Shiny, EncStaticPIDShiny));
                break;
        }
    }

    private void VerifyG5PID_IDCorrelation(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var result = MonochromeRNG.GetBitXor(pk, pk.EncryptionConstant);
        if (result != 0)
            data.AddLine(GetInvalid(PIDTypeMismatch));
    }

    private static void VerifyECPIDWurmple(LegalityAnalysis data)
    {
        var pk = data.Entity;

        if (pk.Species == (int)Species.Wurmple)
        {
            // Indicate what it will evolve into
            var evoVal = WurmpleUtil.GetWurmpleEvoVal(pk.EncryptionConstant);
            var evolvesTo = evoVal == WurmpleEvolution.Silcoon ? (ushort)Species.Beautifly : (ushort)Species.Dustox;
            data.AddLine(GetValid(CheckIdentifier.EC, HintEvolvesToSpecies_0, evolvesTo));
        }
        else if (!WurmpleUtil.IsWurmpleEvoValid(pk))
        {
            data.AddLine(GetInvalid(CheckIdentifier.EC, PIDEncryptWurmple));
        }
    }

    private static void VerifyEC100(LegalityAnalysis data, ushort encSpecies)
    {
        var pk = data.Entity;
        if (pk.Species != encSpecies)
            return; // Evolved, don't need to calculate the final evolution for the verbose report.

        // Indicate the evolution for the user.
        var rare = EvolutionRestrictions.IsEvolvedSpeciesFormRare(pk.EncryptionConstant);
        var hint = rare ? (byte)1 : (byte)0;
        data.AddLine(GetValid(CheckIdentifier.EC, HintEvolvesToRareForm_0, hint));
    }

    private static void VerifyEC(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var Info = data.Info;

        if (pk.EncryptionConstant == 0)
        {
            if (Info.EncounterMatch is WC8 {IsHOMEGift: true})
                return; // HOME Gifts
            data.AddLine(Get(CheckIdentifier.EC, Severity.Fishy, PIDEncryptZero));
        }

        // Gen3-5 => Gen6 have PID==EC with an edge case exception.
        if (Info.Generation is 3 or 4 or 5)
        {
            VerifyTransferEC(data);
            return;
        }

        // Gen1-2, Gen6+ should have PID != EC
        if (pk.PID == pk.EncryptionConstant)
        {
            // Check for edge cases
            var enc = Info.EncounterMatch;
            if (enc is WA8 {IsEquivalentFixedECPID: true})
                return;
            if (enc is WB8 {IsEquivalentFixedECPID: true})
                return;

            data.AddLine(GetInvalid(CheckIdentifier.EC, PIDEqualsEC)); // better to flag than 1:2^32 odds since RNG is not feasible to yield match
            return;
        }

        // Check for Gen3-5 => Gen6 edge case being incorrectly applied here.
        if ((pk.PID ^ 0x80000000) == pk.EncryptionConstant)
        {
            var xor = pk.ShinyXor;
            if (xor >> 3 == 1) // 8 <= x <= 15
                data.AddLine(Get(CheckIdentifier.EC, Severity.Fishy, TransferEncryptGen6Xor));
        }
    }

    /// <summary>
    /// Returns the expected <see cref="PKM.EncryptionConstant"/> for a Gen3-5 transfer to Gen6.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <param name="ec">Encryption constant result</param>
    /// <returns>True if the <see cref="ec"/> is appropriate to use.</returns>
    public static bool GetTransferEC(PKM pk, out uint ec)
    {
        var version = pk.Version;
        if (version is 0 or >= GameVersion.X) // Gen6+ ignored
        {
            ec = 0;
            return false;
        }

        var pid = pk.PID;
        var tmp = pid ^ pk.ID32;
        var XOR = (ushort)(tmp ^ (tmp >> 16));

        // Ensure we don't have a shiny.
        if (XOR >> 3 == 1) // Illegal, fix. (not 16<XOR>=8)
            ec = pid ^ 0x80000000; // Keep as shiny, so we have to mod the EC
        else if ((XOR ^ 0x8000) >> 3 == 1 && pid != pk.EncryptionConstant)
            ec = pid ^ 0x80000000; // Already anti-shiny, ensure the anti-shiny relationship is present.
        else
            ec = pid; // Ensure the copy correlation is present.

        return true;
    }

    private static void VerifyTransferEC(LegalityAnalysis data)
    {
        var pk = data.Entity;

        // Check to see if the PID and EC are properly configured.
        var expect = PK5.GetTransferPID(pk.EncryptionConstant, pk.ID32, out var bitFlipProc);
        if (pk.PID == expect)
            return;

        var msg = bitFlipProc ? TransferEncryptGen6BitFlip : TransferEncryptGen6Equals;
        data.AddLine(GetInvalid(CheckIdentifier.EC, msg));
    }

    private static bool IsEggBitRequiredMale34(ReadOnlySpan<MoveResult> moves)
    {
        // If female, it must match the correlation.
        // If Ditto was used with a Male Nidoran / Volbeat, it'll always be that gender.
        // If Ditto was not used and a Male was obtained, it must match the correlation.
        // This not-Ditto scenario is detectable if the entity has any Inherited level up moves.
        foreach (var move in moves)
        {
            // Egg Moves (passed via Male) are allowed. Check only for inherited level up moves.
            if (move.Info.Method is LearnMethod.InheritLevelUp)
                return true;
        }
        return false;
    }
}
