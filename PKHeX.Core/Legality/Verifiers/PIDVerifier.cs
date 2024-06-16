using static PKHeX.Core.LegalityCheckStrings;

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
            data.AddLine(Get(LPIDZero, Severity.Fishy));
        if (!pk.Nature.IsFixed()) // out of range
            data.AddLine(GetInvalid(LPIDNatureMismatch));

        VerifyShiny(data);
    }

    private void VerifyShiny(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterMatch;

        if (!enc.Shiny.IsValid(pk))
            data.AddLine(GetInvalid(LEncStaticPIDShiny, CheckIdentifier.Shiny));

        switch (enc)
        {
            // Forced PID or generated without an encounter
            // Crustle has 0x80 for its StartWildBattle flag; dunno what it does, but sometimes it doesn't align with the expected PID xor.
            case EncounterStatic5 { IsWildCorrelationPID: true }:
                VerifyG5PID_IDCorrelation(data);
                break;
            case EncounterSlot5 {IsHiddenGrotto: true} when pk.IsShiny:
                data.AddLine(GetInvalid(LG5PIDShinyGrotto, CheckIdentifier.Shiny));
                break;
            case EncounterSlot5 {IsHiddenGrotto: false}:
                VerifyG5PID_IDCorrelation(data);
                break;

            case PCD d: // fixed PID
                if (d.IsFixedPID() && pk.EncryptionConstant != d.Gift.PK.PID)
                    data.AddLine(GetInvalid(LEncGiftPIDMismatch, CheckIdentifier.Shiny));
                break;

            case WC7 { IsAshGreninja: true } when pk.IsShiny:
                data.AddLine(GetInvalid(LEncGiftShinyMismatch, CheckIdentifier.Shiny));
                break;
            // Underground Raids are originally anti-shiny on encounter.
            // When selecting a prize at the end, the game rolls and force-shiny is applied to be XOR=1.
            case EncounterStatic8U u when !u.IsShinyXorValid(pk.ShinyXor):
                data.AddLine(GetInvalid(LEncStaticPIDShiny, CheckIdentifier.Shiny));
                break;
        }
    }

    private void VerifyG5PID_IDCorrelation(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var pid = pk.EncryptionConstant;
        var result = (pid & 1) ^ (pid >> 31) ^ (pk.TID16 & 1) ^ (pk.SID16 & 1);
        if (result != 0)
            data.AddLine(GetInvalid(LPIDTypeMismatch));
    }

    private static void VerifyECPIDWurmple(LegalityAnalysis data)
    {
        var pk = data.Entity;

        if (pk.Species == (int)Species.Wurmple)
        {
            // Indicate what it will evolve into
            var evoVal = WurmpleUtil.GetWurmpleEvoVal(pk.EncryptionConstant);
            var evolvesTo = evoVal == WurmpleEvolution.Silcoon ? (int)Species.Beautifly : (int)Species.Dustox;
            var species = ParseSettings.SpeciesStrings[evolvesTo];
            var msg = string.Format(L_XWurmpleEvo_0, species);
            data.AddLine(GetValid(msg, CheckIdentifier.EC));
        }
        else if (!WurmpleUtil.IsWurmpleEvoValid(pk))
        {
            data.AddLine(GetInvalid(LPIDEncryptWurmple, CheckIdentifier.EC));
        }
    }

    private static void VerifyEC100(LegalityAnalysis data, ushort encSpecies)
    {
        var pk = data.Entity;
        if (pk.Species != encSpecies)
            return; // Evolved, don't need to calculate the final evolution for the verbose report.

        // Indicate the evolution for the user.
        uint evoVal = pk.EncryptionConstant % 100;
        bool rare = evoVal == 0;
        var (species, form) = EvolutionRestrictions.GetEvolvedSpeciesFormEC100(encSpecies, rare);
        var str = GameInfo.Strings;
        var forms = FormConverter.GetFormList(species, str.Types, str.forms, GameInfo.GenderSymbolASCII, EntityContext.Gen9);
        var msg = string.Format(L_XRareFormEvo_0_1, forms[form], rare);
        data.AddLine(GetValid(msg, CheckIdentifier.EC));
    }

    private static void VerifyEC(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var Info = data.Info;

        if (pk.EncryptionConstant == 0)
        {
            if (Info.EncounterMatch is WC8 {IsHOMEGift: true})
                return; // HOME Gifts
            data.AddLine(Get(LPIDEncryptZero, Severity.Fishy, CheckIdentifier.EC));
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

            data.AddLine(GetInvalid(LPIDEqualsEC, CheckIdentifier.EC)); // better to flag than 1:2^32 odds since RNG is not feasible to yield match
            return;
        }

        // Check for Gen3-5 => Gen6 edge case being incorrectly applied here.
        if ((pk.PID ^ 0x80000000) == pk.EncryptionConstant)
        {
            var xor = pk.ShinyXor;
            if (xor >> 3 == 1) // 8 <= x <= 15
                data.AddLine(Get(LTransferPIDECXor, Severity.Fishy, CheckIdentifier.EC));
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
        // When transferred to Generation 6, the Encryption Constant is copied from the PID.
        // The PID is then checked to see if it becomes shiny with the new Shiny rules (>>4 instead of >>3)
        // If the PID is nonshiny->shiny, the top bit is flipped.

        // Check to see if the PID and EC are properly configured.
        var bitFlipProc = GetExpectedTransferPID(pk, out var expect);
        bool valid = pk.PID == expect;
        if (valid)
            return;

        var msg = bitFlipProc ? LTransferPIDECBitFlip : LTransferPIDECEquals;
        data.AddLine(GetInvalid(msg, CheckIdentifier.EC));
    }

    private static bool GetExpectedTransferPID(PKM pk, out uint expect)
    {
        var ec = pk.EncryptionConstant; // should be original PID
        var tmp = ec ^ pk.ID32;
        var xor = tmp ^ (tmp >> 16);
        bool xorPID = (xor & 0xFFF8u) == 8;
        expect = (xorPID ? (ec ^ 0x80000000) : ec);
        return xorPID;
    }
}
