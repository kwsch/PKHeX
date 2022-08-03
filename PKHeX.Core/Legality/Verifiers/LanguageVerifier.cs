using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.Language"/>.
/// </summary>
public sealed class LanguageVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Language;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        int originalGeneration = data.Info.Generation;
        int currentLanguage = pk.Language;
        int maxLanguageID = Legal.GetMaxLanguageID(originalGeneration);
        var enc = data.EncounterMatch;
        if (!IsValidLanguageID(currentLanguage, maxLanguageID, pk, enc))
        {
            data.AddLine(GetInvalid(string.Format(LOTLanguage, $"<={(LanguageID)maxLanguageID}", (LanguageID)currentLanguage)));
            return;
        }

        // Korean Gen4 games can not trade with other Gen4 languages, but can use Pal Park with any Gen3 game/language.
        if (pk.Format == 4 && enc.Generation == 4 && !IsValidG4Korean(currentLanguage)
            && enc is not EncounterTrade4PID {Species: (int)Species.Pikachu or (int)Species.Magikarp} // ger magikarp / eng pikachu
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
            if (pk.Korean && !GameVersion.GS.Contains((GameVersion)pk.Version))
                data.AddLine(GetInvalid(string.Format(LOTLanguage, $"!={(LanguageID)currentLanguage}", (LanguageID)currentLanguage)));

            // Japanese VC is language locked; cannot obtain Japanese-Blue version as other languages.
            if (pk.Version == (int)GameVersion.BU && !pk.Japanese)
                data.AddLine(GetInvalid(string.Format(LOTLanguage, nameof(LanguageID.Japanese), (LanguageID)currentLanguage)));
        }
    }

    public static bool IsValidLanguageID(int currentLanguage, int maxLanguageID, PKM pk, IEncounterTemplate enc)
    {
        if (currentLanguage == (int)LanguageID.UNUSED_6)
            return false; // Language ID 6 is unused.

        if (currentLanguage > maxLanguageID)
            return false; //  Language not available (yet)

        if (currentLanguage <= (int)LanguageID.Hacked && !(enc is EncounterTrade5PID && EncounterTrade5PID.IsValidMissingLanguage(pk)))
            return false; // Missing Language value is not obtainable

        return true; // Language is possible
    }

    /// <summary>
    /// Check if the <see cref="currentLanguage"/> can exist in the Generation 4 savefile.
    /// </summary>
    /// <param name="currentLanguage"></param>
    public static bool IsValidG4Korean(int currentLanguage)
    {
        var activeTr = ParseSettings.ActiveTrainer;
        var activeLang = activeTr.Language;
        bool savKOR = activeLang == (int) LanguageID.Korean;
        bool pkmKOR = currentLanguage == (int) LanguageID.Korean;
        if (savKOR == pkmKOR)
            return true;

        return activeLang < 0; // check not overriden by Legality settings
    }
}
