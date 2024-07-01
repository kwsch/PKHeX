using System;
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
        Span<char> nickname = stackalloc char[pk.TrashCharCountNickname];
        int len = pk.LoadString(pk.NicknameTrash, nickname);
        if (len == 0)
        {
            data.AddLine(GetInvalid(LNickLengthShort));
            return;
        }
        nickname = nickname[..len];

        if (pk.Species > SpeciesName.MaxSpeciesID)
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
            if (pk.IsNicknamed)
                return;
        }

        if (pk is { Format: <= 7, IsNicknamed: true }) // can nickname afterward
        {
            if (pk.VC)
                VerifyG1NicknameWithinBounds(data, nickname);
            else if (IsMysteryGiftNoNickname(enc))
                data.AddLine(Get(LEncGiftNicknamed, ParseSettings.Settings.Nickname.NicknamedMysteryGift(enc.Context)));
        }

        if (enc is IFixedTrainer t)
        {
            VerifyNicknameTrade(data, enc);
            if (t is IFixedNickname { IsFixedNickname: true })
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
        if (ParseSettings.Settings.WordFilter.IsEnabled(pk.Format) && pk.IsNicknamed)
        {
            if (WordFilter.IsFiltered(nickname.ToString(), out var badPattern))
                data.AddLine(GetInvalid($"Word Filter: {badPattern}"));
            if (TrainerNameVerifier.ContainsTooManyNumbers(nickname, data.Info.Generation))
                data.AddLine(GetInvalid("Word Filter: Too many numbers."));
        }
    }

    private static bool IsMysteryGiftNoNickname(IEncounterable enc)
    {
        if (enc is not MysteryGift { IsEgg: false })
            return false;
        return enc switch
        {
            PCD pcd => !pcd.Gift.PK.IsNicknamed,
            PGF pgf => !pgf.IsNicknamed,
            WC6 wc6 => !wc6.IsNicknamed && wc6 is not ({ IsOriginalTrainerNameSet: false }
                                                         or { IsLinkGift: true, Species: (int)Species.Glalie or (int)Species.Steelix }), // Can nickname the demo gift
            WC7 wc7 => !wc7.IsNicknamed && wc7 is not { IsOriginalTrainerNameSet: false },
            _ => true,
        };
    }

    private void VerifyFixedNicknameEncounter(LegalityAnalysis data, ILangNicknamedTemplate n, IEncounterTemplate enc, PKM pk, ReadOnlySpan<char> nickname)
    {
        var encounterNickname = n.GetNickname(pk.Language);

        if (string.IsNullOrWhiteSpace(encounterNickname))
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
                foreach (var evo in evos)
                {
                    if (!SpeciesName.IsNicknamedAnyLanguage(evo.Species, nickname, 3))
                        return;
                }
            }

            if (pk.IsNicknamed)
                data.AddLine(Get(LEncGiftNicknamed, Severity.Invalid));
            return;
        }

        if (!pk.IsNicknamed)
        {
            // Check if it had a nickname at all
            var orig = SpeciesName.GetSpeciesNameGeneration(enc.Species, pk.Language, enc.Generation);
            if (orig == encounterNickname)
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
        bool matches = nickname.SequenceEqual(encounterNickname);
        var severity = !matches || !pk.IsNicknamed ? Severity.Invalid : Severity.Valid;
        data.AddLine(Get(LEncGiftNicknamed, severity));
    }

    private void VerifyHomeGiftNickname(LegalityAnalysis data, IEncounterTemplate enc, ILangNick pk, ReadOnlySpan<char> nickname)
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
        if (nickname.SequenceEqual(orig))
            data.AddLine(GetInvalid(LNickMatchLanguageFlag));
    }

    private bool VerifyUnNicknamedEncounter(LegalityAnalysis data, PKM pk, ReadOnlySpan<char> nickname)
    {
        if (pk.IsNicknamed)
        {
            if (data.Info.Generation >= 8 && CanNicknameForeign8Plus(data, pk))
            {
                // Can only nickname if it matches your language.
                // Setting the nickname to the same as the species name does not set the Nickname flag (equals unmodified, no flag)
                if (!SpeciesName.IsNicknamed(pk.Species, nickname, pk.Language, pk.Format))
                {
                    data.AddLine(Get(LNickMatchLanguageFlag, Severity.Invalid));
                    return true;
                }
            }
            foreach (var language in Language.GetAvailableGameLanguages(pk.Format))
            {
                if (!SpeciesName.TryGetSpecies(nickname, language, out var species))
                    continue;
                var msg = species == pk.Species && language != pk.Language ? LNickMatchNoOthersFail : LNickMatchLanguageFlag;
                data.AddLine(Get(msg, ParseSettings.Settings.Nickname.NicknamedAnotherSpecies));
                return true;
            }
            if (pk.Format <= 7 && StringConverter.HasEastAsianScriptCharacters(nickname) && pk is not PB7) // East Asian Scripts
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
        else
        {
            VerifyUnNicknamed(data, pk, nickname);
        }

        return false;
    }

    private void VerifyUnNicknamed(LegalityAnalysis data, PKM pk, ReadOnlySpan<char> nickname)
    {
        if (pk.Format < 3)
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
    }

    private static bool CanNicknameForeign8Plus(LegalityAnalysis data, PKM pk)
    {
        if (data.Info.EvoChainsAllGens.HasVisitedSWSH)
            return true;
        if (pk.Format >= 9)
            return !pk.FatefulEncounter;
        return false;
    }

    private static int GetForeignNicknameLength(PKM pk, IEncounterTemplate match, byte origin)
    {
        // HOME gifts already verified prior to reaching here.
        System.Diagnostics.Debug.Assert(match is not WC8 {IsHOMEGift:true});

        int length = 0;
        if (origin is (4 or 5 or 6 or 7) && match.IsEgg && pk.WasTradedEgg)
            length = Legal.GetMaxLengthNickname(origin, English);

        if (pk.FatefulEncounter)
            return length;

        if (pk.Format < 8 || pk.BDSP)
            return length;

        // Can only nickname if the language matches.
        var future = Legal.GetMaxLengthNickname(pk.Format, (LanguageID)pk.Language);
        return Math.Max(length, future);
    }

    private static bool IsNicknameValid(PKM pk, IEncounterTemplate enc, ReadOnlySpan<char> nickname)
    {
        ushort species = pk.Species;
        byte format = pk.Format;
        int language = pk.Language;

        // Farfetch’d and Sirfetch’d have different apostrophes in HOME, only if transferred from 3DS or GO => HOME.
        // HOME doesn't use the right apostrophe. ' vs ’
        if (format >= 8 && species is (int)Species.Farfetchd or (int)Species.Sirfetchd)
        {
            // Evolving in-game fixes it. Check only un-evolved and affected languages.
            if (species == enc.Species && SpeciesName.IsApostropheFarfetchdLanguage(language))
            {
                if (enc is { Generation: < 8, Context: not EntityContext.Gen7b }) // 3DS -> HOME
                    return nickname is "Farfetch'd";
                if (enc is EncounterSlot8GO) // GO -> HOME
                    return species is (int)Species.Farfetchd ? nickname is "Farfetch'd" : nickname is "Sirfetch'd";
            }
        }

        ReadOnlySpan<char> expect = SpeciesName.GetSpeciesNameGeneration(species, language, format);
        if (nickname.SequenceEqual(expect))
            return true;

        // Can't have another language name if it hasn't evolved or wasn't a language-traded egg.
        // Ash Greninja can be transferred from one demo language to another recipient language, while retaining the demo's name string.
        // Starting in Generation 8, hatched language-traded eggs will take the Language from the trainer that hatched it.
        // Also in Generation 8, evolving in a foreign language game will retain the original language as the source for the newly evolved species name.
        // Transferring from Gen7->Gen8 realigns the Nickname string to the Language, if not nicknamed.
        bool canHaveAnyLanguage = format <= 7 && (enc.Species != species || pk.WasTradedEgg || enc is WC7 {IsAshGreninja: true}) && !pk.GG;
        if (canHaveAnyLanguage && !SpeciesName.IsNicknamedAnyLanguage(species, nickname, format))
            return true;

        if (enc is ILangNick loc && loc.Language != 0 && !loc.IsNicknamed && !SpeciesName.IsNicknamedAnyLanguage(species, nickname, format))
            return true; // fixed language without nickname, nice job event maker!

        if (format == 5 && enc.Generation != 5) // transfer
            return IsMatch45(nickname, species, expect, language, canHaveAnyLanguage);

        return false;
    }

    private static bool IsMatch45(ReadOnlySpan<char> nickname, ushort species, ReadOnlySpan<char> expect, int language, bool canHaveAnyLanguage)
    {
        if (species is (int)Species.Farfetchd)
        {
            if (nickname.Length == expect.Length)
            {
                // Compare as upper -- different apostrophe than Gen4 encoding.
                if (IsMatchUpper45(nickname, expect))
                    return true;
            }
            if (SpeciesName.IsApostropheFarfetchdLanguage(language))
                return false; // must have matched above
        }
        if (canHaveAnyLanguage)
            return !SpeciesName.IsNicknamedAnyLanguage(species, nickname, 4);
        expect = SpeciesName.GetSpeciesNameGeneration(species, language, 4);
        return nickname.SequenceEqual(expect);
    }

    private static bool IsMatchUpper45(ReadOnlySpan<char> nickname, ReadOnlySpan<char> expect)
    {
        for (int i = 0; i < expect.Length; i++)
        {
            if (nickname[i] != char.ToUpperInvariant(expect[i]))
                return false;
        }
        return true;
    }

    private static void VerifyNicknameEgg(LegalityAnalysis data)
    {
        var Info = data.Info;
        var pk = data.Entity;

        bool flagState = EggStateLegality.IsNicknameFlagSet(Info.EncounterMatch, pk);
        if (pk.IsNicknamed != flagState)
            data.AddLine(GetInvalid(flagState ? LNickFlagEggYes : LNickFlagEggNo, CheckIdentifier.Egg));

        Span<char> nickname = stackalloc char[pk.TrashCharCountNickname];
        int len = pk.LoadString(pk.NicknameTrash, nickname);
        nickname = nickname[..len];

        if (pk.Format == 2 && !SpeciesName.IsNicknamedAnyLanguage(0, nickname, 2))
            data.AddLine(GetValid(LNickMatchLanguageEgg, CheckIdentifier.Egg));
        else if (!nickname.SequenceEqual(SpeciesName.GetEggName(pk.Language, Info.Generation)))
            data.AddLine(GetInvalid(LNickMatchLanguageEggFail, CheckIdentifier.Egg));
        else
            data.AddLine(GetValid(LNickMatchLanguageEgg, CheckIdentifier.Egg));
    }

    private static void VerifyNicknameTrade(LegalityAnalysis data, IEncounterTemplate t)
    {
        switch (t)
        {
            case EncounterTrade8b b: VerifyTrade8b(data, b); return;
            case EncounterTrade4PID t4: VerifyTrade4(data, t4); return;
            case EncounterTrade5BW t5:
                VerifyEncounterTrade5(data, t5); return;
            default:
                VerifyTrade(data, t, data.Entity.Language); return;
        }
    }

    private void VerifyG1NicknameWithinBounds(LegalityAnalysis data, ReadOnlySpan<char> str)
    {
        var pk = data.Entity;
        if (StringConverter1.GetIsEnglish(str))
        {
            if (str.Length > 10)
                data.AddLine(GetInvalid(LNickLengthLong));
        }
        else if (StringConverter1.GetIsJapanese(str))
        {
            if (str.Length > 5)
                data.AddLine(GetInvalid(LNickLengthLong));
        }
        else if (pk.Korean && StringConverter2KOR.GetIsKorean(str))
        {
            if (str.Length > 5)
                data.AddLine(GetInvalid(LNickLengthLong));
        }
        else
        {
            data.AddLine(GetInvalid(LG1CharNick));
        }
    }

    private static void VerifyTrade4(LegalityAnalysis data, EncounterTrade4PID t)
    {
        var pk = data.Entity;
        if (t.IsIncorrectEnglish(pk))
            data.AddLine(GetInvalid(string.Format(LOTLanguage, Japanese, English), CheckIdentifier.Language));
        var lang = t.DetectOriginalLanguage(pk);
        VerifyTrade(data, t, lang);
    }

    private static void VerifyTrade8b(LegalityAnalysis data, EncounterTrade8b t)
    {
        var pk = data.Entity;
        int lang = pk.Language;
        if (t.Species == (int)Species.Magikarp)
        {
            if (t.IsMagikarpJapaneseTradedBDSP(pk))
            {
                // Traded replaces the OT Name. Verify only the Nickname now.
                VerifyNickname(data, t, (int)German);
                return;
            }

            Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
            int len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
            trainer = trainer[..len];

            Span<char> nickname = stackalloc char[pk.TrashCharCountNickname];
            len = pk.LoadString(pk.NicknameTrash, nickname);
            nickname = nickname[..len];

            lang = t.DetectMeisterMagikarpLanguage(nickname, trainer, lang);
            if (lang == -1) // err
                data.AddLine(GetInvalid(string.Format(LOTLanguage, $"{Japanese}/{German}", $"{(LanguageID)pk.Language}"), CheckIdentifier.Language));
        }

        if (t.IsPijako(pk))
        {
            // Let it be anything (Nicknamed or not) and just verify the OT.
            VerifyTradeOTOnly(data, t);
            return;
        }
        VerifyTrade(data, t, lang);
    }

    private static void VerifyEncounterTrade5(LegalityAnalysis data, EncounterTrade5BW t)
    {
        var pk = data.Entity;
        var lang = pk.Language;
        if (pk.Format == 5 && lang == (int)Japanese)
            data.AddLine(GetInvalid(string.Format(LOTLanguage, 0, Japanese), CheckIdentifier.Language));

        lang = Math.Max(lang, 1);
        VerifyTrade(data, t, lang);
    }

    private static void VerifyTradeOTOnly(LegalityAnalysis data, IFixedTrainer t)
    {
        var result = CheckTradeOTOnly(data, t);
        data.AddLine(result);
    }

    private static CheckResult CheckTradeOTOnly(LegalityAnalysis data, IFixedTrainer t)
    {
        var pk = data.Entity;
        if (pk.IsNicknamed && (pk.Format < 8 || pk.FatefulEncounter))
            return GetInvalid(LEncTradeChangedNickname, CheckIdentifier.Nickname);
        int lang = pk.Language;

        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
        trainer = trainer[..len];

        if (!t.IsTrainerMatch(pk, trainer, lang))
            return GetInvalid(LEncTradeIndexBad, CheckIdentifier.Trainer);
        return GetValid(LEncTradeUnchanged, CheckIdentifier.Nickname);
    }

    private static void VerifyTrade(LegalityAnalysis data, IEncounterTemplate t, int language)
    {
        if (t is IFixedTrainer { IsFixedTrainer: true } ft)
            VerifyTrainerName(data, ft, language);
        if (t is IFixedNickname { IsFixedNickname: true } fn)
            VerifyNickname(data, fn, language);
    }

    private static void VerifyNickname(LegalityAnalysis data, IFixedNickname fn, int language)
    {
        var pk = data.Entity;
        var result = fn.IsNicknameMatch(pk, pk.Nickname, language)
            ? GetValid(LEncTradeUnchanged, CheckIdentifier.Nickname)
            : Get(LEncTradeChangedNickname, ParseSettings.Settings.Nickname.NicknamedTrade(data.EncounterOriginal.Context), CheckIdentifier.Nickname);
        data.AddLine(result);
    }

    private static void VerifyTrainerName(LegalityAnalysis data, IFixedTrainer ft, int language)
    {
        var pk = data.Entity;
        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
        trainer = trainer[..len];

        if (!ft.IsTrainerMatch(pk, trainer, language))
            data.AddLine(GetInvalid(LEncTradeChangedOT, CheckIdentifier.Trainer));
    }
}
