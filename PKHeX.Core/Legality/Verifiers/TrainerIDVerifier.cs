namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.OT_Name"/>.
    /// </summary>
    public sealed class TrainerIDVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Trainer;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (!TrainerNameVerifier.IsPlayerOriginalTrainer(data.EncounterMatch))
                return; // already verified

            if (pkm.BDSP)
            {
                if (pkm.TID == 0 && pkm.SID == 0) // Game loops to ensure a nonzero full-ID
                {
                    data.AddLine(GetInvalid(LegalityCheckStrings.LOT_IDInvalid));
                    return;
                }
                if (pkm.TID == 0xFFFF && pkm.SID == 0x7FFF) // int.MaxValue cannot be yielded by Unity's Random.Range[min, max)
                {
                    data.AddLine(GetInvalid(LegalityCheckStrings.LOT_IDInvalid));
                    return;
                }
            }
            else if (pkm.VC && pkm.SID != 0)
            {
                data.AddLine(GetInvalid(LegalityCheckStrings.LOT_SID0Invalid));
                return;
            }

            if (pkm.TID == 0 && pkm.SID == 0)
            {
                data.AddLine(Get(LegalityCheckStrings.LOT_IDs0, Severity.Fishy));
            }
            else if (pkm.TID == pkm.SID)
            {
                data.AddLine(Get(LegalityCheckStrings.LOT_IDEqual, Severity.Fishy));
            }
            else if (pkm.TID == 0)
            {
                data.AddLine(Get(LegalityCheckStrings.LOT_TID0, Severity.Fishy));
            }
            else if (pkm.SID == 0)
            {
                data.AddLine(Get(LegalityCheckStrings.LOT_SID0, Severity.Fishy));
            }
            else if (IsOTIDSuspicious(pkm.TID, pkm.SID))
            {
                data.AddLine(Get(LegalityCheckStrings.LOTSuspicious, Severity.Fishy));
            }
        }

        public static bool IsOTIDSuspicious(int tid16, int sid16)
        {
            if (tid16 == 12345 && sid16 == 54321)
                return true;

            // 1234_123456 (SID7_TID7)
            if (tid16 == 15040 && sid16 == 18831)
                return true;

            return false;
        }
    }
}
