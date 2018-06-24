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
            {
                if (pkm.Format == 4 && pkm.Species == 292) // Shedinja glitch
                {
                    // should match original gender
                    var gender = PKX.GetGenderFromPIDAndRatio(pkm.PID, 0x7F); // 50M-50F
                    if (gender == pkm.Gender)
                        return true;
                }
                else if (pkm.Format > 5 && (pkm.Species == 183 || pkm.Species == 184)) // Azurill/Marill Gender Ratio Change
                {
                    var gv = pkm.PID & 0xFF;
                    if (gv > 63 && pkm.Gender == 1) // evolved from azurill after transferring to keep gender
                        return true;
                }
                return false;
            }

            // check for mixed->fixed gender incompatibility by checking the gender of the original species
            var EncounterMatch = data.EncounterMatch;
            if (Legal.FixedGenderFromBiGender.Contains(EncounterMatch.Species) && pkm.Gender != 2) // shedinja
            {
                var gender = PKX.GetGenderFromPID(EncounterMatch.Species, pkm.EncryptionConstant);
                if (gender != pkm.Gender)  // gender must not be different from original
                    return false;
            }
            return true;
        }
    }
}
