using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public class GenderVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Gender;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var pi = pkm.PersonalInfo;
            if (pi.Genderless != (pkm.Gender == 2))
            {
                // DP/HGSS shedinja glitch -- only generation 4 spawns
                bool ignore = pkm.Format == 4 && pkm.Species == 292 && pkm.Met_Level != pkm.CurrentLevel;
                if (!ignore)
                    data.AddLine(GetInvalid(V203));
                return;
            }

            // Check for PID relationship to Gender & Nature if applicable
            int gen = data.Info.Generation;

            if (3 <= gen && gen <= 5)
            {
                // Gender-PID & Nature-PID relationship check
                if (IsValidGenderPID(data))
                    data.AddLine(GetValid(V250));
                else
                    data.AddLine(GetInvalid(V251));

                if (gen != 5)
                    VerifyNaturePID(data);
                return;
            }

            // Check fixed gender cases
            if ((pi.OnlyFemale && pkm.Gender != 1) || (pi.OnlyMale && pkm.Gender != 0))
                data.AddLine(GetInvalid(V203));
        }

        private void VerifyNaturePID(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var result = pkm.EncryptionConstant % 25 == pkm.Nature
                ? GetValid(V252, CheckIdentifier.Nature)
                : GetInvalid(V253, CheckIdentifier.Nature);
            data.AddLine(result);
        }

        private bool IsValidGenderPID(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            bool genderValid = pkm.IsGenderValid();
            if (!genderValid)
                return IsValidGenderMismatch(pkm);

            // check for mixed->fixed gender incompatibility by checking the gender of the original species
            int original = data.EncounterMatch.Species;
            if (Legal.FixedGenderFromBiGender.Contains(original))
                return IsValidFixedGenderFromBiGender(pkm, original);

            return true;
        }

        private static bool IsValidFixedGenderFromBiGender(PKM pkm, int original)
        {
            var current = pkm.Gender;
            if (current == 2) // shedinja, genderless
                return true;
            var gender = PKX.GetGenderFromPID(original, pkm.EncryptionConstant);
            return gender == current;
        }

        private static bool IsValidGenderMismatch(PKM pkm)
        {
            switch (pkm.Species)
            {
                case 292 when pkm.Format == 4: // Shedinja evolution gender glitch, should match original Gender
                    return pkm.Gender == PKX.GetGenderFromPIDAndRatio(pkm.EncryptionConstant, 0x7F); // 50M-50F

                case 183 when pkm.Format >= 6:
                case 184 when pkm.Format >= 6: // evolved from azurill after transferring to keep gender
                    return pkm.Gender == 1 && (pkm.EncryptionConstant & 0xFF) > 0x3F;

                default: return false;
            }
        }
    }
}
