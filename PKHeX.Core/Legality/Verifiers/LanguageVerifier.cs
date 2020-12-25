using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.Language"/>.
    /// </summary>
    public sealed class LanguageVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Language;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            int originalGeneration = data.Info.Generation;
            int currentLanguage = pkm.Language;
            int maxLanguageID = Legal.GetMaxLanguageID(originalGeneration);

            if (!IsValidLanguageID(currentLanguage, maxLanguageID, pkm))
            {
                data.AddLine(GetInvalid(string.Format(LOTLanguage, $"<={(LanguageID)maxLanguageID}", (LanguageID)currentLanguage)));
                return;
            }

            // Korean Gen4 games can not trade with other Gen4 languages, but can use Pal Park with any Gen3 game/language.
            if (pkm.Format == 4 && pkm.Gen4 && !IsValidG4Korean(currentLanguage)
                && !(data.EncounterMatch is EncounterTrade4 {Species: (int)Species.Pikachu or (int)Species.Magikarp}) // ger magikarp / eng pikachu
            )
            {
                bool kor = currentLanguage == (int)LanguageID.Korean;
                var msgpkm = kor ? L_XKorean : L_XKoreanNon;
                var msgsav = kor ? L_XKoreanNon : L_XKorean;
                data.AddLine(GetInvalid(string.Format(LTransferOriginFInvalid0_1, msgpkm, msgsav)));
                return;
            }

            if (originalGeneration <= 2)
            {
                // Korean Crystal does not exist, neither do Korean VC1
                if (pkm.Korean && !GameVersion.GS.Contains((GameVersion)pkm.Version))
                    data.AddLine(GetInvalid(string.Format(LOTLanguage, $"!={(LanguageID)currentLanguage}", (LanguageID)currentLanguage)));

                // Japanese VC is language locked; cannot obtain Japanese-Blue version as other languages.
                if (pkm.Version == (int)GameVersion.BU && !pkm.Japanese)
                    data.AddLine(GetInvalid(string.Format(LOTLanguage, nameof(LanguageID.Japanese), (LanguageID)currentLanguage)));
            }
        }

        public static bool IsValidLanguageID(int currentLanguage, int maxLanguageID, PKM pkm)
        {
            if (currentLanguage == (int)LanguageID.UNUSED_6)
                return false; // Language ID 6 is unused.

            if (currentLanguage > maxLanguageID)
                return false; //  Language not available (yet)

            if (currentLanguage <= (int) LanguageID.Hacked && !Legal.IsValidMissingLanguage(pkm))
                return false; // Missing Language value is not obtainable

            return true; // Language is possible
        }

        /// <summary>
        /// Check if the <see cref="currentLanguage"/> can exist in the Generation 4 savefile.
        /// </summary>
        /// <param name="currentLanguage"></param>
        /// <returns></returns>
        public static bool IsValidG4Korean(int currentLanguage)
        {
            bool savKOR = ParseSettings.ActiveTrainer.Language == (int) LanguageID.Korean;
            bool pkmKOR = currentLanguage == (int) LanguageID.Korean;
            if (savKOR == pkmKOR)
                return true;

            return ParseSettings.ActiveTrainer.Language < 0; // check not overriden by Legality settings
        }
    }
}
