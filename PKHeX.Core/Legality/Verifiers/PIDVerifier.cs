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

        if (pk.PID == 0)
            data.AddLine(Get(LPIDZero, Severity.Fishy));
        if (pk.Nature >= 25) // out of range
            data.AddLine(GetInvalid(LPIDNatureMismatch));

        VerifyShiny(data);
    }

    private void VerifyShiny(LegalityAnalysis data)
    {
        var pk = data.Entity;

        switch (data.EncounterMatch)
        {
            case EncounterStatic s:
                if (!s.Shiny.IsValid(pk))
                    data.AddLine(GetInvalid(LEncStaticPIDShiny, CheckIdentifier.Shiny));

                // Underground Raids are originally anti-shiny on encounter.
                // When selecting a prize at the end, the game rolls and force-shiny is applied to be XOR=1.
                if (s is EncounterStatic8U {Shiny: Shiny.Random})
                {
                    if (pk.ShinyXor is <= 15 and not 1)
                        data.AddLine(GetInvalid(LEncStaticPIDShiny, CheckIdentifier.Shiny));
                    break;
                }

                if (s.Generation != 5)
                    break;

                // Generation 5 has a correlation for wild captures.
                // Certain static encounter types are just generated straightforwardly.
                if (s.Location == 75) // Entree Forest
                    break;

                // Not wild / forced ability
                if (s.Gift || s.Ability == AbilityPermission.OnlyHidden)
                    break;

                // Forced PID or generated without an encounter
                // Crustle has 0x80 for its StartWildBattle flag; dunno what it does, but sometimes it doesn't align with the expected PID xor.
                if (s is EncounterStatic5 { IsWildCorrelationPID: true })
                    VerifyG5PID_IDCorrelation(data);
                break;

            case EncounterSlot5 {IsHiddenGrotto: true}:
                if (pk.IsShiny)
                    data.AddLine(GetInvalid(LG5PIDShinyGrotto, CheckIdentifier.Shiny));
                break;
            case EncounterSlot5:
                VerifyG5PID_IDCorrelation(data);
                break;

            case PCD d: // fixed PID
                if (d.IsFixedPID() && pk.EncryptionConstant != d.Gift.PK.PID)
                    data.AddLine(GetInvalid(LEncGiftPIDMismatch, CheckIdentifier.Shiny));
                break;

            case WC7 wc7 when wc7.IsAshGreninjaWC7(pk) && pk.IsShiny:
                data.AddLine(GetInvalid(LEncGiftShinyMismatch, CheckIdentifier.Shiny));
                break;
        }
    }

    private void VerifyG5PID_IDCorrelation(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var pid = pk.EncryptionConstant;
        var result = (pid & 1) ^ (pid >> 31) ^ (pk.TID & 1) ^ (pk.SID & 1);
        if (result != 0)
            data.AddLine(GetInvalid(LPIDTypeMismatch));
    }

    private static void VerifyECPIDWurmple(LegalityAnalysis data)
    {
        var pk = data.Entity;

        if (pk.Species == (int)Species.Wurmple)
        {
            // Indicate what it will evolve into
            uint evoVal = WurmpleUtil.GetWurmpleEvoVal(pk.EncryptionConstant);
            var evolvesTo = evoVal == 0 ? (int)Species.Beautifly : (int)Species.Dustox;
            var species = ParseSettings.SpeciesStrings[evolvesTo];
            var msg = string.Format(L_XWurmpleEvo_0, species);
            data.AddLine(GetValid(msg, CheckIdentifier.EC));
        }
        else if (!WurmpleUtil.IsWurmpleEvoValid(pk))
        {
            data.AddLine(GetInvalid(LPIDEncryptWurmple, CheckIdentifier.EC));
        }
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
            int xor = pk.TSV ^ pk.PSV;
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
        var ver = pk.Version;
        if (ver is 0 or >= (int)GameVersion.X) // Gen6+ ignored
        {
            ec = 0;
            return false;
        }

        uint pid = pk.PID;
        uint LID = pid & 0xFFFF;
        uint HID = pid >> 16;
        uint XOR = (uint)(pk.TID ^ LID ^ pk.SID ^ HID);

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
        bool xorPID = ((pk.TID ^ pk.SID ^ (int) (ec & 0xFFFF) ^ (int) (ec >> 16)) & ~0x7) == 8;
        expect = (xorPID ? (ec ^ 0x80000000) : ec);
        return xorPID;
    }
}
