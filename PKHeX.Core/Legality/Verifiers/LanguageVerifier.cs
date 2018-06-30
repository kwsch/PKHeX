using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
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
                data.AddLine(GetInvalid(string.Format(V5, $"<={(LanguageID)maxLanguageID}", currentLanguage)));
                return;
            }

            // Korean Gen4 games can not trade with other Gen4 languages, but can use Pal Park with any Gen3 game/language.
            if (pkm.Format == 4 && pkm.Gen4 && !IsValidG4Korean(currentLanguage))
            {
                bool kor = currentLanguage == (int)LanguageID.Korean;
                var msgpkm = kor ? V611 : V612;
                var msgsav = kor ? V612 : V611;
                data.AddLine(GetInvalid(string.Format(V610, msgpkm, msgsav)));
                return;
            }

            // Korean Crystal does not exist, neither do VC1
            if (originalGeneration <= 2 && pkm.Korean && !GameVersion.GS.Contains((GameVersion)pkm.Version))
            {
                data.AddLine(GetInvalid(string.Format(V5, $"!={(LanguageID)currentLanguage}", currentLanguage)));
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
            bool savKOR = Legal.SavegameLanguage == (int) LanguageID.Korean;
            bool pkmKOR = currentLanguage == (int) LanguageID.Korean;
            if (savKOR == pkmKOR)
                return true;

            return Legal.SavegameLanguage < 0; // check not overriden by Legality settings
        }
    }
}
