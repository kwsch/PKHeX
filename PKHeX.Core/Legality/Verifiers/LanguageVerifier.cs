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
        var maxLanguageID = (LanguageID)Legal.GetMaxLanguageID(originalGeneration, data.EncounterOriginal.Context);
        var enc = data.EncounterMatch;
        if (!IsValidLanguageID(currentLanguage, maxLanguageID, pk, enc))
        {
            data.AddLine(GetInvalid(Identifier, OTLanguageShouldBeLeq_0, (byte)maxLanguageID, (byte)currentLanguage));
            return;
        }

        // Check for GTS trade sanitization.
        if (pk.Format >= 4)
            CheckGTS(data, pk, currentLanguage, originalGeneration);

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

    private void CheckGTS(LegalityAnalysis data, PKM pk, LanguageID currentLanguage, byte originalGeneration)
    {
        bool possiblyRomanizedG4 = false;
        if (originalGeneration is 4 && currentLanguage is Korean && !pk.IsEgg)
        {
            // All OT names are half-width already, so they could have been manually entered.
            possiblyRomanizedG4 = Gen4GlobalTradeRules.IsRomanizedKoreanTrainerName(pk);
            // If not nicknamed, the sanitization also applies to the nickname text.
            // Check that separately, there is some nuance with trade-backs.
            if (possiblyRomanizedG4) // apply a tag to indicate to the checker, and also downstream checks.
                data.AddLine(GetValid(GTSTrainerSanitized)); // acts as an info tag.
        }

        if (pk.Format == 4)
        {
            // Any Gen4 trainer can send/receive Korean language, but can't otherwise directly trade across the language barrier.
            // Check for lockout of Korean GTS trades.
            var tr = ParseSettings.ActiveTrainer;

            // Check if it must have been traded across the GTS to its current residence.
            if (tr is null || !Gen4GlobalTradeRules.IsRequiredGTS(tr, currentLanguage))
                return;

            // Check if it actually can be traded across the GTS.
            var enc = data.EncounterMatch;
            if (enc is EncounterTrade4PID { IsLanguageSwap: true })
                return; // Can originate in Korean games and have international Language ID without traversing the GTS.

            // Eggs and Classic Ribbon cannot be traded on GTS.
            // If it must have been traded via GTS, it must have been sanitized if Korean.
            if (pk.IsEgg)
                data.AddLine(GetInvalid(GTSDisallowedTradedEgg));
            else if (enc is IRibbonSetEvent4 { RibbonClassic: true })
                data.AddLine(GetInvalid(GTSDisallowedClassicRibbon));
            else if (currentLanguage == Korean && !possiblyRomanizedG4)
                data.AddLine(GetInvalid(GTSTrainerSanitizedExpected));
            else // OK
                data.AddLine(GetValid(GTSTradedKoreanInternational));
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
}
