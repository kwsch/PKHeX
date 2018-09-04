using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.SuperTrainingMedalCount"/> and associated values.
    /// </summary>
    public sealed class MedalVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Training;

        public override void Verify(LegalityAnalysis data)
        {
            VerifyMedalsRegular(data);
            VerifyMedalsEvent(data);
        }

        private void VerifyMedalsRegular(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var Info = data.Info;
            uint value = BitConverter.ToUInt32(pkm.Data, 0x2C);
            if ((value & 3) != 0) // 2 unused flags
                data.AddLine(GetInvalid(LSuperUnused));
            int TrainCount = pkm.SuperTrainingMedalCount();

            if (pkm.IsEgg && TrainCount > 0)
            {
                data.AddLine(GetInvalid(LSuperEgg));
                return;
            }
            if (TrainCount > 0 && Info.Generation > 6)
            {
                data.AddLine(GetInvalid(LSuperUnavailable));
                return;
            }
            if (pkm.Format >= 7)
            {
                if (pkm.SecretSuperTrainingUnlocked)
                    data.AddLine(GetInvalid(LSuperNoUnlocked));
                if (pkm.SecretSuperTrainingComplete)
                    data.AddLine(GetInvalid(LSuperNoComplete));
                return;
            }
            if (TrainCount == 30 ^ pkm.SecretSuperTrainingComplete)
                data.AddLine(GetInvalid(LSuperComplete));
        }

        private void VerifyMedalsEvent(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var Info = data.Info;
            byte value = pkm.Data[0x3A];
            if ((value & 0xC0) != 0) // 2 unused flags highest bits
                data.AddLine(GetInvalid(LSuperUnused));

            int TrainCount = 0;
            for (int i = 0; i < 6; i++)
            {
                if ((value & 1) != 0)
                    TrainCount++;
                value >>= 1;
            }
            if (pkm.IsEgg && TrainCount > 0)
                data.AddLine(GetInvalid(LSuperEgg));
            else if (TrainCount > 0 && Info.Generation > 6)
                data.AddLine(GetInvalid(LSuperUnavailable));
            else if (TrainCount > 0)
                data.AddLine(Get(LSuperDistro, Severity.Fishy));
        }
    }
}
