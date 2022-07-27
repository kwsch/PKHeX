using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.LanguageID;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.Nickname"/>.
/// </summary>
public sealed class NicknameVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Nickname;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;

        // If the Pokémon is not nicknamed, it should match one of the language strings.
        var nickname = pk.Nickname;
        if (nickname.Length == 0)
        {
            data.AddLine(GetInvalid(LNickLengthShort));
            return;
        }
        if (pk.Species > SpeciesName.SpeciesLang[0].Count)
        {
            data.AddLine(Get(LNickLengthShort, Severity.Invalid));
            return;
        }

        var enc = data.EncounterOriginal;
        if (enc is ILangNicknamedTemplate n)
        {
            VerifyFixedNicknameEncounter(data, n, enc, pk, nickname);
            if (pk.IsEgg)
                VerifyNicknameEgg(data);
            return;
        }

        if (pk.Format <= 7 && pk.IsNicknamed) // can nickname afterwards
        {
            if (pk.VC)
                VerifyG1NicknameWithinBounds(data, nickname.AsSpan());
            else if (enc is MysteryGift {IsEgg: false})
                data.AddLine(Get(LEncGiftNicknamed, ParseSettings.NicknamedMysteryGift));
        }

        if (enc is EncounterTrade t)
        {
            VerifyNicknameTrade(data, t);
            if (t.HasNickname)
                return;
        }

        if (pk.IsEgg)
        {
            VerifyNicknameEgg(data);
            return;
        }

        if (VerifyUnNicknamedEncounter(data, pk, nickname))
            return;

        // Non-nicknamed strings have already been checked.
        if (ParseSettings.CheckWordFilter && pk.IsNicknamed)
        {
            if (WordFilter.IsFiltered(nickname, out string bad))
                data.AddLine(GetInvalid($"Word Filter: {bad}"));
            if (TrainerNameVerifier.ContainsTooManyNumbers(nickname, data.Info.Generation))
                data.AddLine(GetInvalid("Word Filter: Too many numbers."));
        }
    }

    private void VerifyFixedNicknameEncounter(LegalityAnalysis data, ILangNicknamedTemplate n, IEncounterTemplate enc, PKM pk, string nickname)
    {
        var nick = n.GetNickname(pk.Language);

        if (string.IsNullOrWhiteSpace(nick))
        {
            if (n is WC8 {IsHOMEGift: true})
            {
                VerifyHomeGiftNickname(data, enc, pk, nickname);
                return;
            }

            if (n.CanHandleOT(pk.Language))
                return;

            if (n is WC3 && pk.Format != 3)
            {
                // Gen3 gifts transferred to Generation 4 from another language can set the nickname flag.
                var evos = data.Info.EvoChainsAllGens.Gen3;
                bool matchAny = evos.Any(evo => !SpeciesName.IsNicknamedAnyLanguage(evo.Species, nickname, 3));
                if (matchAny)
                    return;
            }

            if (pk.IsNicknamed)
                data.AddLine(Get(LEncGiftNicknamed, Severity.Invalid));
            return;
        }

        if (!pk.IsNicknamed)
        {
            // Check if it had a nickname at all
            var orig = SpeciesName.GetSpeciesNameGeneration(enc.Species, pk.Language, enc.Generation);
            if (orig == nick)
            {
                // Didn't have a nickname. Ensure that the language matches the current nickname string.
                if (!SpeciesName.IsNicknamed(pk.Species, nickname, pk.Language, pk.Format))
                    return;
            }

            // Should have a nickname present.
            data.AddLine(GetInvalid(LNickMatchLanguageFail));
            return;
        }

        // Encounter has a nickname, and PKM should have it.
        var severity = nick != nickname || !pk.IsNicknamed ? Severity.Invalid : Severity.Valid;
        data.AddLine(Get(LEncGiftNicknamed, severity));
    }

    private void VerifyHomeGiftNickname(LegalityAnalysis data, IEncounterTemplate enc, ILangNick pk, string nickname)
    {
        // can nickname on redemption
        if (!pk.IsNicknamed)
            return;

        // Can't nickname everything.
        if (enc.Species == (int) Species.Melmetal)
        {
            data.AddLine(GetInvalid(LEncGiftNicknamed));
            return;
        }

        // Ensure the nickname does not match species name
        var orig = SpeciesName.GetSpeciesNameGeneration(enc.Species, pk.Language, enc.Generation);
        if (nickname == orig)
            data.AddLine(GetInvalid(LNickMatchLanguageFlag));
    }

    private bool VerifyUnNicknamedEncounter(LegalityAnalysis data, PKM pk, string nickname)
    {
        if (pk.IsNicknamed)
        {
            if (data.Info.Generation >= 8)
            {
                // Can only nickname if it matches your language.
                // Setting the nickname to the same as the species name does not set the Nickname flag (equals unmodified, no flag)
                if (!SpeciesName.IsNicknamed(pk.Species, nickname, pk.Language, pk.Format))
                {
                    data.AddLine(Get(LNickMatchLanguageFlag, Severity.Invalid));
                    return true;
                }
            }
            for (int i = 0; i < SpeciesName.SpeciesDict.Count; i++)
            {
                if (!SpeciesName.SpeciesDict[i].TryGetValue(nickname, out int species))
                    continue;
                var msg = species == pk.Species && i != pk.Language ? LNickMatchNoOthersFail : LNickMatchLanguageFlag;
                data.AddLine(Get(msg, ParseSettings.NicknamedAnotherSpecies));
                return true;
            }
            if (pk.Format <= 7 && StringConverter.HasEastAsianScriptCharacters(nickname.AsSpan()) && pk is not PB7) // East Asian Scripts
            {
                data.AddLine(GetInvalid(LNickInvalidChar));
                return true;
            }
            if (nickname.Length > Legal.GetMaxLengthNickname(data.Info.Generation, (LanguageID)pk.Language))
            {
                int length = GetForeignNicknameLength(pk, data.Info.EncounterOriginal, data.Info.Generation);
                var severe = (length != 0 && nickname.Length <= length) ? Severity.Fishy : Severity.Invalid;
                data.AddLine(Get(LNickLengthLong, severe));
                return true;
            }
            data.AddLine(GetValid(LNickMatchNoOthers));
        }
        else if (pk.Format < 3)
        {
            // pk1/pk2 IsNicknamed getter checks for match, logic should only reach here if matches.
            data.AddLine(GetValid(LNickMatchLanguage));
        }
        else
        {
            var enc = data.EncounterOriginal;
            bool valid = IsNicknameValid(pk, enc, nickname);
            var result = valid ? GetValid(LNickMatchLanguage) : GetInvalid(LNickMatchLanguageFail);
            data.AddLine(result);
        }
        return false;
    }

    private static int GetForeignNicknameLength(PKM pk, IEncounterTemplate match, int origin)
    {
        // HOME gifts already verified prior to reaching here.
        System.Diagnostics.Debug.Assert(match is not WC8 {IsHOMEGift:true});

        int length = 0;
        if (origin is (4 or 5 or 6 or 7) && match.EggEncounter && pk.WasTradedEgg)
            length = Legal.GetMaxLengthNickname(origin, English);

        if (pk.FatefulEncounter)
            return length;

        if (pk.Format < 8 || pk.BDSP)
            return length;

        // Can only nickname if the language matches.
        var future = Legal.GetMaxLengthNickname(pk.Format, (LanguageID)pk.Language);
        return Math.Max(length, future);
    }

    private static bool IsNicknameValid(PKM pk, IEncounterTemplate enc, string nickname)
    {
        int species = pk.Species;
        int format = pk.Format;
        int language = pk.Language;
        if (SpeciesName.GetSpeciesNameGeneration(species, language, format) == nickname)
            return true;

        // Can't have another language name if it hasn't evolved or wasn't a language-traded egg.
        // Starting in Generation 8, hatched language-traded eggs will take the Language from the trainer that hatched it.
        // Also in Generation 8, evolving in a foreign language game will retain the original language as the source for the newly evolved species name.
        // Transferring from Gen7->Gen8 realigns the Nickname string to the Language, if not nicknamed.
        bool canHaveAnyLanguage = format <= 7 && (enc.Species != species || pk.WasTradedEgg) && !pk.GG;
        if (canHaveAnyLanguage && !SpeciesName.IsNicknamedAnyLanguage(species, nickname, format))
            return true;

        switch (enc)
        {
            case WC7 wc7 when wc7.IsAshGreninjaWC7(pk):
                return true;
            case ILangNick loc:
                if (loc.Language != 0 && !loc.IsNicknamed && !SpeciesName.IsNicknamedAnyLanguage(species, nickname, format))
                    return true; // fixed language without nickname, nice job event maker!
                break;
        }

        if (format == 5 && !pk.IsNative) // transfer
        {
            if (canHaveAnyLanguage)
                return !SpeciesName.IsNicknamedAnyLanguage(species, nickname, 4);
            return SpeciesName.GetSpeciesNameGeneration(species, language, 4) == nickname;
        }

        return false;
    }

    private static void VerifyNicknameEgg(LegalityAnalysis data)
    {
        var Info = data.Info;
        var pk = data.Entity;

        bool flagState = EggStateLegality.IsNicknameFlagSet(Info.EncounterMatch, pk);
        if (pk.IsNicknamed != flagState)
            data.AddLine(GetInvalid(flagState ? LNickFlagEggYes : LNickFlagEggNo, CheckIdentifier.Egg));

        var nick = pk.Nickname;
        if (pk.Format == 2 && !SpeciesName.IsNicknamedAnyLanguage(0, nick, 2))
            data.AddLine(GetValid(LNickMatchLanguageEgg, CheckIdentifier.Egg));
        else if (nick != SpeciesName.GetEggName(pk.Language, Info.Generation))
            data.AddLine(GetInvalid(LNickMatchLanguageEggFail, CheckIdentifier.Egg));
        else
            data.AddLine(GetValid(LNickMatchLanguageEgg, CheckIdentifier.Egg));
    }

    private static void VerifyNicknameTrade(LegalityAnalysis data, EncounterTrade t)
    {
        switch (data.Info.Generation)
        {
            case 8 when t is EncounterTrade8b b: VerifyTrade8b(data, b); return;

            case 1: VerifyTrade12(data, t); return;
            case 2: return; // already checked all relevant properties when fetching with getValidEncounterTradeVC2
            case 3: VerifyTrade3(data, t); return;
            case 4: VerifyTrade4(data, t); return;
            case 5: VerifyTrade5(data, t); return;
            case 6:
            case 7:
            case 8:
                VerifyTrade(data, t, data.Entity.Language); return;
        }
    }

    private void VerifyG1NicknameWithinBounds(LegalityAnalysis data, ReadOnlySpan<char> str)
    {
        var pk = data.Entity;
        if (StringConverter12.GetIsG1English(str))
        {
            if (str.Length > 10)
                data.AddLine(GetInvalid(LNickLengthLong));
        }
        else if (StringConverter12.GetIsG1Japanese(str))
        {
            if (str.Length > 5)
                data.AddLine(GetInvalid(LNickLengthLong));
        }
        else if (pk.Korean && StringConverter2KOR.GetIsG2Korean(str))
        {
            if (str.Length > 5)
                data.AddLine(GetInvalid(LNickLengthLong));
        }
        else
        {
            data.AddLine(GetInvalid(LG1CharNick));
        }
    }

    private static void VerifyTrade12(LegalityAnalysis data, EncounterTrade t)
    {
        var t1 = (EncounterTrade1)t;
        if (!t1.IsNicknameValid(data.Entity))
            data.AddLine(GetInvalid(LEncTradeChangedNickname, CheckIdentifier.Nickname));
        if (!t1.IsTrainerNameValid(data.Entity))
            data.AddLine(GetInvalid(LEncTradeChangedOT, CheckIdentifier.Trainer));
    }

    private static void VerifyTrade3(LegalityAnalysis data, EncounterTrade t)
    {
        var pk = data.Entity;
        int lang = pk.Language;
        if (t.Species == (int)Species.Jynx) // FRLG Jynx
            lang = DetectTradeLanguageG3DANTAEJynx(pk, lang);
        VerifyTrade(data, t, lang);
    }

    private static void VerifyTrade4(LegalityAnalysis data, EncounterTrade t)
    {
        var pk = data.Entity;
        if (pk.TID == 1000)
        {
            VerifyTradeOTOnly(data, t);
            return;
        }
        int lang = pk.Language;
        switch (t.Species)
        {
            case (int)Species.Pikachu: // HGSS Pikachu
                lang = DetectTradeLanguageG4SurgePikachu(pk, t, lang);
                // flag korean on gen4 saves since the pk.Language is German
                FlagKoreanIncompatibleSameGenTrade(data, pk, lang);
                break;
            case (int)Species.Magikarp: // DPPt Magikarp
                lang = DetectTradeLanguageG4MeisterMagikarp(pk, t, lang);
                // flag korean on gen4 saves since the pk.Language is German
                FlagKoreanIncompatibleSameGenTrade(data, pk, lang);
                break;

            default:
                if (t is EncounterTrade4PID && pk.Version is ((int)GameVersion.D or (int)GameVersion.P)) // mainline DP
                {
                    // DP English origin are Japanese lang. Can't have LanguageID 2
                    if (lang == 2)
                    {
                        data.AddLine(GetInvalid(string.Format(LOTLanguage, Japanese, English), CheckIdentifier.Language));
                        break;
                    }

                    // Since two locales (JPN/ENG) can have the same LanguageID, check which we should be validating with.
                    if (lang == 1 && pk.OT_Name != t.GetOT(1)) // not Japanese
                        lang = 2; // verify strings with English locale instead.
                }
                break;
        }
        VerifyTrade(data, t, lang);
    }

    private static void VerifyTrade8b(LegalityAnalysis data, EncounterTrade8b t)
    {
        var pk = data.Entity;
        int lang = pk.Language;
        if (t.Species == (int)Species.Magikarp)
        {
            // Japanese 
            if (pk.Language == (int)Japanese && pk.OT_Name is "Diamond." or "Pearl.")
            {
                // Traded between players, the original OT is replaced with the above OT (version dependent) as the original OT is >6 chars in length.
                VerifyTradeNickname(data, t, t.Nicknames[(int)German], pk);
                return;
            }

            lang = DetectTradeLanguageG8MeisterMagikarp(pk, t, lang);
            if (lang == 0) // err
                data.AddLine(GetInvalid(string.Format(LOTLanguage, $"{Japanese}/{German}", $"{(LanguageID)pk.Language}"), CheckIdentifier.Language));
        }
        VerifyTrade(data, t, lang);
    }

    private static int DetectTradeLanguageG8MeisterMagikarp(PKM pk, EncounterTrade8b t, int currentLanguageID)
    {
        // Receiving the trade on a German game -> Japanese LanguageID.
        // Receiving the trade on any other language -> German LanguageID.
        if (currentLanguageID is not ((int)Japanese or (int)German))
            return 0;

        var nick = pk.Nickname;
        var ot = pk.OT_Name;
        for (int i = 1; i < (int)ChineseT; i++)
        {
            if (t.Nicknames[i] != nick)
                continue;
            if (t.TrainerNames[i] != ot)
                continue;

            // Language gets flipped to another language ID; can't be equal.
            var shouldNotBe = currentLanguageID == (int)German ? German : Japanese;
            return i != (int)shouldNotBe ? i : 0;
        }
        return 0;
    }

    private static void FlagKoreanIncompatibleSameGenTrade(LegalityAnalysis data, PKM pk, int lang)
    {
        if (pk.Format != 4 || lang != (int)Korean)
            return; // transferred or not appropriate
        if (ParseSettings.ActiveTrainer.Language != (int)Korean && ParseSettings.ActiveTrainer.Language >= 0)
            data.AddLine(GetInvalid(string.Format(LTransferOriginFInvalid0_1, L_XKorean, L_XKoreanNon), CheckIdentifier.Language));
    }

    private static int DetectTradeLanguage(string OT, EncounterTrade t, int currentLanguageID)
    {
        var names = t.TrainerNames;
        for (int lang = 1; lang < names.Count; lang++)
        {
            if (names[lang] != OT)
                continue;
            return lang;
        }
        return currentLanguageID;
    }

    private static int DetectTradeLanguageG3DANTAEJynx(PKM pk, int currentLanguageID)
    {
        if (currentLanguageID != (int)Italian)
            return currentLanguageID;

        if (pk.Version == (int)GameVersion.LG)
            currentLanguageID = (int)English; // translation error; OT was not localized => same as English
        return currentLanguageID;
    }

    private static int DetectTradeLanguageG4MeisterMagikarp(PKM pk, EncounterTrade t, int currentLanguageID)
    {
        if (currentLanguageID == (int)English)
            return (int)German;

        // All have German, regardless of origin version.
        var lang = DetectTradeLanguage(pk.OT_Name, t, currentLanguageID);
        if (lang == (int)English) // possible collision with FR/ES/DE. Check nickname
            return pk.Nickname == t.Nicknames[(int)French] ? (int)French : (int)Spanish; // Spanish is same as English

        return lang;
    }

    private static int DetectTradeLanguageG4SurgePikachu(PKM pk, EncounterTrade t, int currentLanguageID)
    {
        if (currentLanguageID == (int)French)
            return (int)English;

        // All have English, regardless of origin version.
        var lang = DetectTradeLanguage(pk.OT_Name, t, currentLanguageID);
        if (lang == 2) // possible collision with ES/IT. Check nickname
            return pk.Nickname == t.Nicknames[(int)Italian] ? (int)Italian : (int)Spanish;

        return lang;
    }

    private static void VerifyTrade5(LegalityAnalysis data, EncounterTrade t)
    {
        var pk = data.Entity;
        int lang = pk.Language;
        // Trades for JPN games have language ID of 0, not 1.
        if (pk.BW)
        {
            if (pk.Format == 5 && lang == (int)Japanese)
                data.AddLine(GetInvalid(string.Format(LOTLanguage, 0, Japanese), CheckIdentifier.Language));

            lang = Math.Max(lang, 1);
            VerifyTrade(data, t, lang);
        }
        else // B2W2
        {
            if (t.TID is Encounters5.YancyTID or Encounters5.CurtisTID)
                VerifyTradeOTOnly(data, t);
            else
                VerifyTrade(data, t, lang);
        }
    }

    private static void VerifyTradeOTOnly(LegalityAnalysis data, EncounterTrade t)
    {
        var result = CheckTradeOTOnly(data, t.TrainerNames);
        data.AddLine(result);
    }

    private static CheckResult CheckTradeOTOnly(LegalityAnalysis data, IReadOnlyList<string> validOT)
    {
        var pk = data.Entity;
        if (pk.IsNicknamed && (pk.Format < 8 || pk.FatefulEncounter))
            return GetInvalid(LEncTradeChangedNickname, CheckIdentifier.Nickname);
        int lang = pk.Language;
        if (validOT.Count <= lang)
            return GetInvalid(LEncTradeIndexBad, CheckIdentifier.Trainer);
        if (validOT[lang] != pk.OT_Name)
            return GetInvalid(LEncTradeChangedOT, CheckIdentifier.Trainer);
        return GetValid(LEncTradeUnchanged, CheckIdentifier.Nickname);
    }

    private static void VerifyTrade(LegalityAnalysis data, EncounterTrade t, int language)
    {
        var ot = t.GetOT(language);
        var nick = t.GetNickname(language);
        if (string.IsNullOrEmpty(nick))
            VerifyTradeOTOnly(data, t);
        else
            VerifyTradeOTNick(data, t, nick, ot);
    }

    private static void VerifyTradeOTNick(LegalityAnalysis data, EncounterTrade t, string nick, string OT)
    {
        var pk = data.Entity;
        // trades that are not nicknamed (but are present in a table with others being named)
        VerifyTradeNickname(data, t, nick, pk);

        if (OT != pk.OT_Name)
            data.AddLine(GetInvalid(LEncTradeChangedOT, CheckIdentifier.Trainer));
    }

    private static void VerifyTradeNickname(LegalityAnalysis data, EncounterTrade t, string expectedNickname, PKM pk)
    {
        var result = IsNicknameMatch(expectedNickname, pk, t)
            ? GetValid(LEncTradeUnchanged, CheckIdentifier.Nickname)
            : Get(LEncTradeChangedNickname, ParseSettings.NicknamedTrade, CheckIdentifier.Nickname);
        data.AddLine(result);
    }

    private static bool IsNicknameMatch(string nick, ILangNick pk, EncounterTrade enc)
    {
        if (nick == "Quacklin’" && pk.Nickname == "Quacklin'")
            return true;
        if (enc.IsNicknamed != pk.IsNicknamed)
            return false;
        if (nick != pk.Nickname) // if not match, must not be a nicknamed trade && not currently named
            return !enc.IsNicknamed && !pk.IsNicknamed;
        return true;
    }
}
