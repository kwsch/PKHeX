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

            if (pkm.IsEgg)
            {
                // Can't have any super training data as an egg.
                if (TrainCount > 0)
                    data.AddLine(GetInvalid(LSuperEgg));
                if (pkm.SecretSuperTrainingUnlocked)
                    data.AddLine(GetInvalid(LSuperNoUnlocked));
                if (pkm.SecretSuperTrainingComplete)
                    data.AddLine(GetInvalid(LSuperNoComplete));
                return;
            }

            if (Info.Generation >= 7 || Info.Generation <= 2)
            {
                // Can't have any super training data if it never visited Gen6.
                if (TrainCount > 0)
                    data.AddLine(GetInvalid(LSuperUnavailable));
                if (pkm.SecretSuperTrainingUnlocked)
                    data.AddLine(GetInvalid(LSuperNoUnlocked));
                if (pkm.SecretSuperTrainingComplete)
                    data.AddLine(GetInvalid(LSuperNoComplete));
                return;
            }

            if (pkm.Format >= 7)
            {
                // Gen6->Gen7 transfer wipes the two Secret flags.
                if (pkm.SecretSuperTrainingUnlocked)
                    data.AddLine(GetInvalid(LSuperNoUnlocked));
                if (pkm.SecretSuperTrainingComplete)
                    data.AddLine(GetInvalid(LSuperNoComplete));
                return;
            }

            // Only reach here if Format==6.
            if (TrainCount == 30 ^ pkm.SecretSuperTrainingComplete)
                data.AddLine(GetInvalid(LSuperComplete));
        }

        private void VerifyMedalsEvent(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            byte value = pkm.Data[0x3A];
            if (value != 0)
                data.AddLine(GetInvalid(LSuperDistro));
        }
    }
}
