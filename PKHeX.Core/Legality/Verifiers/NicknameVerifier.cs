using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.LanguageID;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.Nickname"/>.
    /// </summary>
    public sealed class NicknameVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Nickname;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterOriginal;

            // If the Pokémon is not nicknamed, it should match one of the language strings.
            if (pkm.Nickname.Length == 0)
            {
                data.AddLine(GetInvalid(LNickLengthShort));
                return;
            }
            if (pkm.Species > SpeciesName.SpeciesLang[0].Count)
            {
                data.AddLine(Get(LNickLengthShort, Severity.Indeterminate));
                return;
            }

            if (pkm.Format <= 7) // can nickname afterwards
            {
                if (pkm.VC && pkm.IsNicknamed)
                {
                    VerifyG1NicknameWithinBounds(data, pkm.Nickname);
                }
                else if (EncounterMatch is MysteryGift m)
                {
                    if (pkm.IsNicknamed && !m.IsEgg)
                        data.AddLine(Get(LEncGiftNicknamed, ParseSettings.NicknamedMysteryGift));
                }
            }

            if (EncounterMatch is EncounterTrade t)
            {
                VerifyNicknameTrade(data, t);
                return;
            }

            if (pkm.IsEgg)
            {
                VerifyNicknameEgg(data);
                return;
            }

            string nickname = pkm.Nickname.Replace('\'', '’');
            if (VerifyUnNicknamedEncounter(data, pkm, nickname))
                return;

            // Non-nicknamed strings have already been checked.
            if (ParseSettings.CheckWordFilter && pkm.IsNicknamed)
            {
                if (WordFilter.IsFiltered(nickname, out string bad))
                    data.AddLine(GetInvalid($"Word Filter: {bad}"));
                if (TrainerNameVerifier.ContainsTooManyNumbers(nickname, data.Info.Generation))
                    data.AddLine(GetInvalid("Word Filter: Too many numbers."));
            }
        }

        private bool VerifyUnNicknamedEncounter(LegalityAnalysis data, PKM pkm, string nickname)
        {
            if (pkm.IsNicknamed)
            {
                for (int i = 0; i < SpeciesName.SpeciesLang.Count; i++)
                {
                    if (!SpeciesName.SpeciesDict[i].TryGetValue(nickname, out int index))
                        continue;
                    var msg = index == pkm.Species && i != pkm.Language ? LNickMatchNoOthersFail : LNickMatchLanguageFlag;
                    data.AddLine(Get(msg, Severity.Fishy));
                    return true;
                }
                if (pkm.Format <= 7 && StringConverter.HasEastAsianScriptCharacters(nickname) && !(pkm is PB7)) // East Asian Scripts
                {
                    data.AddLine(GetInvalid(LNickInvalidChar));
                    return true;
                }
                if (nickname.Length > Legal.GetMaxLengthNickname(data.Info.Generation, (LanguageID)pkm.Language))
                {
                    var severe = pkm.Format >= 8 || (data.EncounterOriginal.EggEncounter && pkm.WasTradedEgg && nickname.Length <= Legal.GetMaxLengthNickname(data.Info.Generation, English))
                            ? Severity.Fishy
                            : Severity.Invalid;
                    data.AddLine(Get(LNickLengthLong, severe));
                    return true;
                }
                data.AddLine(GetValid(LNickMatchNoOthers));
            }
            else if (pkm.Format < 3)
            {
                // pk1/pk2 IsNicknamed getter checks for match, logic should only reach here if matches.
                data.AddLine(GetValid(LNickMatchLanguage));
            }
            else
            {
                var EncounterMatch = data.EncounterOriginal;
                bool valid = IsNicknameValid(pkm, EncounterMatch, nickname);
                var result = valid ? GetValid(LNickMatchLanguage) : GetInvalid(LNickMatchLanguageFail);
                data.AddLine(result);
            }
            return false;
        }

        private static bool IsNicknameValid(PKM pkm, IEncounterable EncounterMatch, string nickname)
        {
            if (SpeciesName.GetSpeciesNameGeneration(pkm.Species, pkm.Language, pkm.Format) == nickname)
                return true;

            // Can't have another language name if it hasn't evolved or wasn't a language-traded egg.
            bool evolved = EncounterMatch.Species != pkm.Species;
            bool canHaveAnyLanguage = evolved || pkm.WasTradedEgg;
            if (canHaveAnyLanguage && !SpeciesName.IsNicknamedAnyLanguage(pkm.Species, nickname, pkm.Format))
                return true;

            switch (EncounterMatch)
            {
                case WC7 wc7 when wc7.IsAshGreninjaWC7(pkm):
                    return true;
                case ILangNick loc:
                    if (loc.Language != 0 && !loc.IsNicknamed && !SpeciesName.IsNicknamedAnyLanguage(pkm.Species, nickname, pkm.Format))
                        return true; // fixed language without nickname, nice job event maker!
                    break;
            }

            if (pkm.Format == 5 && !pkm.IsNative) // transfer
            {
                if (canHaveAnyLanguage)
                   return !SpeciesName.IsNicknamedAnyLanguage(pkm.Species, nickname, 4);
                return SpeciesName.GetSpeciesNameGeneration(pkm.Species, pkm.Language, 4) == nickname;
            }

            return false;
        }

        private void VerifyNicknameEgg(LegalityAnalysis data)
        {
            var Info = data.Info;
            var pkm = data.pkm;
            var EncounterMatch = Info.EncounterMatch;
            switch (pkm.Format)
            {
                case 4:
                    if (pkm.IsNicknamed) // gen4 doesn't use the nickname flag for eggs
                        data.AddLine(GetInvalid(LNickFlagEggNo, CheckIdentifier.Egg));
                    break;
                case 7:
                    if (EncounterMatch is EncounterStatic ^ !pkm.IsNicknamed) // gen7 doesn't use for ingame gifts
                        data.AddLine(GetInvalid(pkm.IsNicknamed ? LNickFlagEggNo : LNickFlagEggYes, CheckIdentifier.Egg));
                    break;
                default:
                    if (!pkm.IsNicknamed)
                        data.AddLine(GetInvalid(LNickFlagEggYes, CheckIdentifier.Egg));
                    break;
            }

            if (pkm.Format == 2 && pkm.IsEgg && !SpeciesName.IsNicknamedAnyLanguage(0, pkm.Nickname, 2))
                data.AddLine(GetValid(LNickMatchLanguageEgg, CheckIdentifier.Egg));
            else if (SpeciesName.GetSpeciesNameGeneration(0, pkm.Language, Info.Generation) != pkm.Nickname)
                data.AddLine(GetInvalid(LNickMatchLanguageEggFail, CheckIdentifier.Egg));
            else
                data.AddLine(GetValid(LNickMatchLanguageEgg, CheckIdentifier.Egg));
        }

        private void VerifyNicknameTrade(LegalityAnalysis data, EncounterTrade t)
        {
            switch (data.Info.Generation)
            {
                case 1:
                case 2: VerifyTrade12(data, t); return;
                case 3: VerifyTrade3(data, t); return;
                case 4: VerifyTrade4(data, t); return;
                case 5: VerifyTrade5(data, t); return;
                case 6:
                case 7:
                case 8:
                    VerifyTrade(data, t, data.pkm.Language); return;
            }
        }

        private void VerifyG1NicknameWithinBounds(LegalityAnalysis data, string str)
        {
            var pkm = data.pkm;
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
            else if (pkm.Korean && StringConverter2KOR.GetIsG2Korean(str))
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
            if (t.TID != 0) // Gen2 Trade
                return; // already checked all relevant properties when fetching with getValidEncounterTradeVC2

            if (!EncounterGenerator.IsEncounterTrade1Valid(data.pkm, t))
                data.AddLine(GetInvalid(LEncTradeChangedOT, CheckIdentifier.Trainer));
        }

        private static void VerifyTrade3(LegalityAnalysis data, EncounterTrade t)
        {
            var pkm = data.pkm;
            int lang = pkm.Language;
            if (t.Species == (int)Species.Jynx) // FRLG Jynx
                lang = DetectTradeLanguageG3DANTAEJynx(pkm, lang);
            VerifyTrade(data, t, lang);
        }

        private static void VerifyTrade4(LegalityAnalysis data, EncounterTrade t)
        {
            var pkm = data.pkm;
            if (pkm.TID == 1000)
            {
                VerifyTradeOTOnly(data, t);
                return;
            }
            int lang = pkm.Language;
            switch (t.Species)
            {
                case (int)Species.Pikachu: // HGSS Pikachu
                    lang = DetectTradeLanguageG4SurgePikachu(pkm, t, lang);
                    // flag korean on gen4 saves since the pkm.Language is German
                    FlagKoreanIncompatibleSameGenTrade(data, pkm, lang);
                    break;
                case (int)Species.Magikarp: // DPPt Magikarp
                    lang = DetectTradeLanguageG4MeisterMagikarp(pkm, t, lang);
                    // flag korean on gen4 saves since the pkm.Language is German
                    FlagKoreanIncompatibleSameGenTrade(data, pkm, lang);
                    break;

                default:
                    if (lang == 1 && (pkm.Version == (int)GameVersion.D || pkm.Version == (int)GameVersion.P))
                    {
                        // DP English origin are Japanese lang
                        if (pkm.OT_Name != t.GetOT(1)) // not japanese
                            lang = 2; // English
                    }
                    break;
            }
            VerifyTrade(data, t, lang);
        }

        private static void FlagKoreanIncompatibleSameGenTrade(LegalityAnalysis data, PKM pkm, int lang)
        {
            if (pkm.Format != 4 || lang != (int)Korean)
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

        private static int DetectTradeLanguageG4MeisterMagikarp(PKM pkm, EncounterTrade t, int currentLanguageID)
        {
            if (currentLanguageID == (int)English)
                return (int)German;

            // All have German, regardless of origin version.
            var lang = DetectTradeLanguage(pkm.OT_Name, t, currentLanguageID);
            if (lang == (int)English) // possible collision with FR/ES/DE. Check nickname
                return pkm.Nickname == t.Nicknames[(int)French] ? (int)French : (int)Spanish; // Spanish is same as English

            return lang;
        }

        private static int DetectTradeLanguageG4SurgePikachu(PKM pkm, EncounterTrade t, int currentLanguageID)
        {
            if (currentLanguageID == (int)French)
                return (int)English;

            // All have English, regardless of origin version.
            var lang = DetectTradeLanguage(pkm.OT_Name, t, currentLanguageID);
            if (lang == 2) // possible collision with ES/IT. Check nickname
                return pkm.Nickname == t.Nicknames[(int)Italian] ? (int)Italian : (int)Spanish;

            return lang;
        }

        private static void VerifyTrade5(LegalityAnalysis data, EncounterTrade t)
        {
            var pkm = data.pkm;
            int lang = pkm.Language;
            // Trades for JPN games have language ID of 0, not 1.
            if (pkm.BW)
            {
                if (pkm.Format == 5 && lang == (int)Japanese)
                    data.AddLine(GetInvalid(string.Format(LOTLanguage, 0, Japanese), CheckIdentifier.Language));

                lang = Math.Max(lang, 1);
                VerifyTrade(data, t, lang);
            }
            else // B2W2
            {
                if (t.TID == Encounters5.YancyTID || t.TID == Encounters5.CurtisTID)
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
            var pkm = data.pkm;
            if (pkm.IsNicknamed && pkm.Format < 8)
                return GetInvalid(LEncTradeChangedNickname, CheckIdentifier.Nickname);
            int lang = pkm.Language;
            if (validOT.Count <= lang)
                return GetInvalid(LEncTradeIndexBad, CheckIdentifier.Trainer);
            if (validOT[lang] != pkm.OT_Name)
                return GetInvalid(LEncTradeChangedOT, CheckIdentifier.Trainer);
            return GetValid(LEncTradeUnchanged, CheckIdentifier.Nickname);
        }

        private static void VerifyTrade(LegalityAnalysis data, EncounterTrade t, int language)
        {
            var ot = t.GetOT(language);
            var nick = t.GetNickname(language);
            VerifyTradeOTNick(data, t, nick, ot);
        }

        private static void VerifyTradeOTNick(LegalityAnalysis data, EncounterTrade t, string nick, string OT)
        {
            var pkm = data.pkm;
            // trades that are not nicknamed (but are present in a table with others being named)
            var result = IsNicknameMatch(nick, pkm, t)
                ? GetValid(LEncTradeUnchanged, CheckIdentifier.Nickname)
                : Get(LEncTradeChangedNickname, ParseSettings.NicknamedTrade, CheckIdentifier.Nickname);
            data.AddLine(result);

            if (OT != pkm.OT_Name)
                data.AddLine(GetInvalid(LEncTradeChangedOT, CheckIdentifier.Trainer));
        }

        private static bool IsNicknameMatch(string nick, ILangNick pkm, IEncounterable EncounterMatch)
        {
            if (nick == "Quacklin’" && pkm.Nickname == "Quacklin'")
                return true;
            var trade = (EncounterTrade) EncounterMatch;
            if (trade.IsNicknamed != pkm.IsNicknamed)
                return false;
            if (nick != pkm.Nickname) // if not match, must not be a nicknamed trade && not currently named
                return !trade.IsNicknamed && !pkm.IsNicknamed;
            return true;
        }
    }
}
