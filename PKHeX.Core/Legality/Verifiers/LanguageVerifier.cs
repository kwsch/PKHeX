using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.LanguageID;

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
        var originalGeneration = data.Info.Generation;
        var currentLanguage = (LanguageID)pk.Language;
        var maxLanguageID = (LanguageID)Legal.GetMaxLanguageID(originalGeneration);
        var enc = data.EncounterMatch;
        if (!IsValidLanguageID(currentLanguage, maxLanguageID, pk, enc))
        {
            data.AddLine(GetInvalid(Identifier, OTLanguageShouldBeLeq_0, (byte)maxLanguageID, (byte)currentLanguage));
            return;
        }

        // Korean Gen4 games can not trade with other Gen4 languages, but can use Pal Park with any Gen3 game/language.
        if (pk.Format == 4 && enc.Generation == 4 && !IsValidGen4Korean(currentLanguage)
            && enc is not EncounterTrade4PID { IsLanguageSwap: true } // ger magikarp / eng pikachu
           )
        {
            bool kor = currentLanguage == Korean;
            data.AddLine(GetInvalid(TransferKoreanGen4));
            return;
        }

        if (originalGeneration <= 2)
        {
            // Korean Crystal does not exist, neither do Korean VC1
            if (pk is { Korean: true, Version: not (GD or SI) })
                data.AddLine(GetInvalid(OTLanguageCannotPlayOnVersion_0, (byte)pk.Version));

            // Japanese VC is language locked; cannot obtain Japanese-Blue version as other languages.
            if (pk is { Japanese: false, Version: BU })
                data.AddLine(GetInvalid(OTLanguageCannotPlayOnVersion_0, (byte)pk.Version));
        }
    }

    public static bool IsValidLanguageID(LanguageID currentLanguage, LanguageID maxLanguageID, PKM pk, IEncounterTemplate enc)
    {
        if (currentLanguage == UNUSED_6)
            return false; // Language ID 6 is unused.

        if (currentLanguage > maxLanguageID)
            return false; //  Language not available (yet)

        if (currentLanguage == 0 && !(enc is EncounterTrade5BW && EncounterTrade5BW.IsValidMissingLanguage(pk)))
            return false; // Missing Language value is not obtainable

        return true; // Language is possible
    }

    /// <summary>
    /// Check if the <see cref="pkmLanguage"/> can exist in the Generation 4 save file.
    /// </summary>
    /// <remarks>
    /// Korean Gen4 games can not trade with other Gen4 languages, but can use Pal Park with any Gen3 game/language.
    /// Anything with Gen4 origin cannot exist in the other language save file.
    /// </remarks>
    public static bool IsValidGen4Korean(LanguageID pkmLanguage)
    {
        if (ParseSettings.ActiveTrainer is not SAV4 tr)
            return true; // ignore
        return IsValidGen4Korean(pkmLanguage, tr);
    }

    /// <inheritdoc cref="IsValidGen4Korean(LanguageID)"/>
    public static bool IsValidGen4Korean(LanguageID pkmLanguage, SAV4 tr)
    {
        bool savKOR = (LanguageID)tr.Language == Korean;
        bool pkmKOR = pkmLanguage == Korean;
        return savKOR == pkmKOR;
    }
}
