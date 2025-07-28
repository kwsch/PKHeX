using System;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

/// <summary>
/// Value object that contains the context for localization of legality checks.
/// </summary>
public readonly ref struct LegalityLocalizationContext
{
    public required LegalityAnalysis Analysis { get; init; }
    public required LegalityLocalizationSet Settings { get; init; }

    public GameStrings Strings => Settings.Strings;

    /// <summary>
    /// Creates a complete <see cref="LegalityLocalizationContext"/> with proper localization initialization.
    /// </summary>
    /// <param name="la">Legality analysis</param>
    /// <param name="settings">Export settings</param>
    /// <returns>Fully initialized localization context</returns>
    public static LegalityLocalizationContext Create(LegalityAnalysis la, LegalityLocalizationSet settings) => new()
    {
        Analysis = la,
        Settings = settings,
    };

    /// <summary>
    /// Creates a complete <see cref="LegalityLocalizationContext"/> using the specified language.
    /// </summary>
    /// <param name="la">Legality analysis</param>
    /// <param name="language">Language code</param>
    /// <returns>Fully initialized localization context</returns>
    public static LegalityLocalizationContext Create(LegalityAnalysis la, string language = GameLanguage.DefaultLanguage)
        => Create(la, LegalityLocalizationSet.GetLocalization(language));

    public string GetRibbonMessage() => RibbonVerifier.GetMessage(Analysis, Strings.Ribbons);
    public string GetStatName(int displayIndex) => GetSafe(Settings.General.StatNames, displayIndex);
    public string GetMoveName(ushort move) => GetSafe(Strings.movelist, move);
    public string GetSpeciesName(ushort species) => GetSafe(Strings.specieslist, species);
    public string GetConsoleRegion3DS(int index) => GetSafe(Strings.console3ds, index);
    public string GetRibbonName(RibbonIndex index) => Strings.Ribbons.GetNameSafe($"Ribbon{index}", out var result) ? result : index.ToString();
    public string GetLanguageName(int index) => GetSafe(Strings.languageNames, index);

    private static string GetSafe(ReadOnlySpan<string> arr, int index)
    {
        if ((uint)index >= arr.Length)
            return string.Empty;
        return arr[index];
    }

    public string GetTrainer(int index) => index switch
    {
        0 => Settings.General.OriginalTrainer,
        1 => Settings.General.HandlingTrainer,
        _ => throw new ArgumentOutOfRangeException(nameof(index), $"Invalid Trainer argument: {index}"),
    };

    /// <summary>
    /// Converts a <see cref="LegalityCheckResultCode"/> to its corresponding localized string,
    /// applying formatting with the provided argument if needed.
    /// </summary>
    /// <param name="chk">Raw check value for formatting the string.</param>
    /// <param name="verbose">Include Identifier</param>
    /// <returns>The localized string from <see cref="LegalityCheckLocalization"/>, with formatting applied if needed</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the enum value doesn't have a corresponding string</exception>
    public string Humanize(in CheckResult chk, bool verbose = false)
    {
        var str = GetInternalString(chk);
        var format = Settings.Lines.F0_1;
        if (verbose)
            str = string.Format(format, chk.Identifier.ToString(), str);
        return string.Format(format, Settings.Description(chk.Judgement), str);
    }

    private string GetInternalString(CheckResult chk)
    {
        var code = chk.Result;
        if (code is External)
            return ExternalLegalityCheck.Localize(chk, Settings);
        var template = code.GetTemplate(Settings.Lines);
        if (code < FirstWithArgument)
            return template;
        if (code.IsArgument())
            return string.Format(template, chk.Argument);
        if (code.IsMove())
            return string.Format(template, GetMoveName(chk.Argument));
        if (code.IsLanguage())
            return string.Format(template, GetLanguageName(chk.Argument), GetLanguageName(Analysis.Entity.Language));
        if (code.IsMemory())
            return GetMemory(chk, template, code);

        // Complex codes may require additional context or arguments.
        return GetComplex(chk, template, code);
    }

    private string GetMemory(CheckResult chk, string template, LegalityCheckResultCode code)
    {
        if (code < FirstMemoryWithValue)
            return string.Format(template, GetTrainer(chk.Argument));
        if (code is MemoryArgBadItem_H1)
            return string.Format(template, GetTrainer(chk.Argument), GetSpeciesName(chk.Argument2));
        if (code is MemoryArgBadMove_H1)
            return string.Format(template, GetTrainer(chk.Argument), GetMoveName(chk.Argument2));
        if (code is MemoryArgBadSpecies_H1)
            return string.Format(template, GetTrainer(chk.Argument), GetSpeciesName(chk.Argument2));
        return string.Format(template, GetTrainer(chk.Argument), chk.Argument2);
    }

    private string GetComplex(CheckResult chk, string format, LegalityCheckResultCode code) => code switch
    {
        < FirstComplex => format, // why are you even here?
        RibbonsInvalid_A => string.Format(format, GetRibbonMessage()),
        WordFilterFlaggedPattern_01 => string.Format(format, WordFilter.GetPattern((WordFilterType)chk.Argument, chk.Argument2), (WordFilterType)chk.Argument),
        WordFilterInvalidCharacter_0 => string.Format(format, chk.Argument, chk.Argument.ToString("X4")),

        AwakenedStatGEQ_01 => string.Format(format, chk.Argument, GetStatName(chk.Argument2)),
        GanbaruStatLEQ_01 => string.Format(format, chk.Argument, GetStatName(chk.Argument2)),

        OTLanguageCannotTransferToConsoleRegion_0 => string.Format(format, GetConsoleRegion3DS(chk.Argument)),
        EncTradeShouldHaveEvolvedToSpecies_0 => string.Format(format, GetSpeciesName(chk.Argument)),
        MoveEvoFCombination_0 => string.Format(format, GetSpeciesName(chk.Argument)),
        HintEvolvesToSpecies_0 => string.Format(format, GetSpeciesName(chk.Argument)),

        RibbonMarkingInvalid_0 => string.Format(format, GetRibbonName((RibbonIndex)chk.Argument)),
        RibbonMarkingAffixed_0 => string.Format(format, GetRibbonName((RibbonIndex)chk.Argument)),
        RibbonsMissing_A => string.Format(format, GetRibbonName((RibbonIndex)chk.Argument)),

        StoredSlotSourceInvalid_0 => string.Format(format, (StorageSlotSource)chk.Argument),
        HintEvolvesToRareForm_0 => string.Format(format, chk.Argument == 1),
        FormInvalidRangeLEQ_0F => string.Format(format, chk.Argument, Analysis.Entity.Form),

        OTLanguageShouldBe_0or1 => string.Format(format, GetLanguageName(chk.Argument), GetLanguageName(chk.Argument2), GetLanguageName(Analysis.Entity.Language)),

        >= MAX => throw new ArgumentOutOfRangeException(nameof(code), code, null),
    };

    public string FormatMove(in MoveResult move, int index, byte currentFormat)
    {
        var result = Format(move, index, Settings.Moves.FormatMove);
        var gen = move.Generation;
        if (currentFormat != gen && gen != 0)
            result += $" [Gen{gen}]";
        return result;
    }

    public string FormatRelearn(in MoveResult move, int index) => Format(move, index, Settings.Moves.FormatRelearn);
    private string Format(in MoveResult move, int index, string format) => string.Format(format, Settings.Description(move.Judgement), index, move.Summary(this));
}
