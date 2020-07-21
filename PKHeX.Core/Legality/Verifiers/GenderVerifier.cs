using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.Gender"/>.
    /// </summary>
    public sealed class GenderVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Gender;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var pi = pkm.PersonalInfo;
            if (pi.Genderless != (pkm.Gender == 2))
            {
                // DP/HGSS shedinja glitch -- only generation 4 spawns
                bool ignore = pkm.Format == 4 && pkm.Species == (int)Species.Shedinja && pkm.Met_Level != pkm.CurrentLevel;
                if (!ignore)
                    data.AddLine(GetInvalid(LGenderInvalidNone));
                return;
            }

            // Check for PID relationship to Gender & Nature if applicable
            int gen = data.Info.Generation;

            if (3 <= gen && gen <= 5)
            {
                // Gender-PID & Nature-PID relationship check
                var result = IsValidGenderPID(data) ? GetValid(LPIDGenderMatch) : GetInvalid(LPIDGenderMismatch);
                data.AddLine(result);

                if (gen != 5)
                    VerifyNaturePID(data);
                return;
            }

            // Check fixed gender cases
            if ((pi.OnlyFemale && pkm.Gender != 1) || (pi.OnlyMale && pkm.Gender != 0))
                data.AddLine(GetInvalid(LGenderInvalidNone));
        }

        private static void VerifyNaturePID(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var result = pkm.EncryptionConstant % 25 == pkm.Nature
                ? GetValid(LPIDNatureMatch, CheckIdentifier.Nature)
                : GetInvalid(LPIDNatureMismatch, CheckIdentifier.Nature);
            data.AddLine(result);
        }

        private static bool IsValidGenderPID(LegalityAnalysis data)
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
                case (int)Species.Shedinja when pkm.Format == 4: // Shedinja evolution gender glitch, should match original Gender
                    return pkm.Gender == PKX.GetGenderFromPIDAndRatio(pkm.EncryptionConstant, 0x7F); // 50M-50F

                case (int)Species.Marill    when pkm.Format >= 6:
                case (int)Species.Azumarill when pkm.Format >= 6: // evolved from azurill after transferring to keep gender
                    return pkm.Gender == 1 && (pkm.EncryptionConstant & 0xFF) > 0x3F;

                default: return false;
            }
        }
    }
}
