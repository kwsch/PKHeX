using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.EncryptionConstant"/>.
    /// </summary>
    public sealed class PIDVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.PID;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format >= 6)
                VerifyEC(data);

            var enc = data.EncounterMatch;
            if (enc.Species == (int)Species.Wurmple)
                VerifyECPIDWurmple(data);

            if (pkm.PID == 0)
                data.AddLine(Get(LPIDZero, Severity.Fishy));
            if (pkm.Nature >= 25) // out of range
                data.AddLine(GetInvalid(LPIDNatureMismatch));

            VerifyShiny(data);
        }

        private void VerifyShiny(LegalityAnalysis data)
        {
            var pkm = data.pkm;

            switch (data.EncounterMatch)
            {
                case EncounterStatic s:
                    if (!s.Shiny.IsValid(pkm))
                        data.AddLine(GetInvalid(LEncStaticPIDShiny, CheckIdentifier.Shiny));

                    if (s is EncounterStatic8U {Shiny: Shiny.Random})
                    {
						// Underground Raids are originally anti-shiny on encounter.
						// When selecting a prize at the end, the game rolls and force-shiny is applied to be XOR=1.
                        var xor = pkm.ShinyXor;
                        if (xor is <= 15 and not 1)
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
                    if (s.Gift || s.Ability == 4)
                        break;

                    // Forced PID or generated without an encounter
                    // Crustle has 0x80 for its StartWildBattle flag; dunno what it does, but sometimes it doesn't align with the expected PID xor.
                    if (s is EncounterStatic5 s5 && (s5.Roaming || s5.Shiny != Shiny.Random || s5.Species == (int)Species.Crustle))
                        break;
                    VerifyG5PID_IDCorrelation(data);
                    break;

                case EncounterSlot5 w:
                    if (w.Area.Type == SlotType.HiddenGrotto && pkm.IsShiny)
                        data.AddLine(GetInvalid(LG5PIDShinyGrotto, CheckIdentifier.Shiny));
                    if (w.Area.Type != SlotType.HiddenGrotto)
                        VerifyG5PID_IDCorrelation(data);
                    break;

                case PCD d: // fixed PID
                    if (d.Gift.PK.PID != 1 && pkm.EncryptionConstant != d.Gift.PK.PID)
                        data.AddLine(GetInvalid(LEncGiftPIDMismatch, CheckIdentifier.Shiny));
                    break;

                case WC7 wc7 when wc7.IsAshGreninjaWC7(pkm) && pkm.IsShiny:
                        data.AddLine(GetInvalid(LEncGiftShinyMismatch, CheckIdentifier.Shiny));
                    break;
            }
        }

        private void VerifyG5PID_IDCorrelation(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var pid = pkm.EncryptionConstant;
            var result = (pid & 1) ^ (pid >> 31) ^ (pkm.TID & 1) ^ (pkm.SID & 1);
            if (result != 0)
                data.AddLine(GetInvalid(LPIDTypeMismatch));
        }

        private void VerifyECPIDWurmple(LegalityAnalysis data)
        {
            var pkm = data.pkm;

            if (pkm.Species == (int)Species.Wurmple)
            {
                // Indicate what it will evolve into
                uint evoVal = WurmpleUtil.GetWurmpleEvoVal(pkm.EncryptionConstant);
                var evolvesTo = evoVal == 0 ? (int)Species.Beautifly : (int)Species.Dustox;
                var species = ParseSettings.SpeciesStrings[evolvesTo];
                var msg = string.Format(L_XWurmpleEvo_0, species);
                data.AddLine(GetValid(msg, CheckIdentifier.EC));
            }
            else if (!WurmpleUtil.IsWurmpleEvoValid(pkm))
            {
                data.AddLine(GetInvalid(LPIDEncryptWurmple, CheckIdentifier.EC));
            }
        }

        private static void VerifyEC(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var Info = data.Info;

            if (pkm.EncryptionConstant == 0)
            {
                if (Info.EncounterMatch is WC8 {PID: 0, EncryptionConstant: 0})
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
            if (pkm.PID == pkm.EncryptionConstant)
            {
                data.AddLine(GetInvalid(LPIDEqualsEC, CheckIdentifier.EC)); // better to flag than 1:2^32 odds since RNG is not feasible to yield match
                return;
            }

            // Check for Gen3-5 => Gen6 edge case being incorrectly applied here.
            if ((pkm.PID ^ 0x80000000) == pkm.EncryptionConstant)
            {
                int xor = pkm.TSV ^ pkm.PSV;
                if (xor >> 3 == 1) // 8 <= x <= 15
                    data.AddLine(Get(LTransferPIDECXor, Severity.Fishy, CheckIdentifier.EC));
            }
        }

        private static void VerifyTransferEC(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            // When transferred to Generation 6, the Encryption Constant is copied from the PID.
            // The PID is then checked to see if it becomes shiny with the new Shiny rules (>>4 instead of >>3)
            // If the PID is nonshiny->shiny, the top bit is flipped.

            // Check to see if the PID and EC are properly configured.
            var ec = pkm.EncryptionConstant; // should be original PID
            bool xorPID = ((pkm.TID ^ pkm.SID ^ (int)(ec & 0xFFFF) ^ (int)(ec >> 16)) & ~0x7) == 8;
            bool valid = pkm.PID == (xorPID ? (ec ^ 0x80000000) : ec);
            if (valid)
                return;

            var msg = xorPID ? LTransferPIDECBitFlip : LTransferPIDECEquals;
            data.AddLine(GetInvalid(msg, CheckIdentifier.EC));
        }
    }
}
