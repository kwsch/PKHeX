using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Logic for parsing details for <see cref="ShowdownSet"/> objects.
/// </summary>
public static class ShowdownParsing
{
    private static readonly string[] genderForms = ["", "F", ""];

    /// <inheritdoc cref="ShowdownSet.DefaultListAllocation"/>
    private const int DefaultListAllocation = ShowdownSet.DefaultListAllocation;

    /// <summary>
    /// Gets the Form ID from the input <see cref="name"/>.
    /// </summary>
    /// <param name="name">Form name to find the form index of</param>
    /// <param name="strings">Localized string source to fetch with</param>
    /// <param name="species">Species ID the form belongs to</param>
    /// <param name="context">Format the form name should appear in</param>
    /// <returns>Zero (base form) if no form matches the input string.</returns>
    public static byte GetFormFromString(ReadOnlySpan<char> name, GameStrings strings, ushort species, EntityContext context)
    {
        if (name.Length == 0)
            return 0;

        var forms = FormConverter.GetFormList(species, strings.Types, strings.forms, genderForms, context);
        if (forms.Length < 1)
            return 0;

        // Find first matching index that matches any case, ignoring dashes interchanged with spaces.
        for (byte i = 0; i < forms.Length; i++)
        {
            if (IsFormEquivalent(forms[i], name))
                return i;
        }

        // No match, assume default 0 form.
        return 0;
    }

    private static bool IsFormEquivalent(ReadOnlySpan<char> reference, ReadOnlySpan<char> input)
    {
        if (input.Length != reference.Length)
            return false;

        for (int i = 0; i < input.Length; i++)
        {
            var c1 = input[i];
            var c2 = reference[i];
            if (char.ToUpperInvariant(c1) == char.ToUpperInvariant(c2))
                continue;
            if (c1 is ' ' or '-' && c2 is ' ' or '-')
                continue;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Converts a Form ID to string.
    /// </summary>
    /// <param name="form">Form to get the form name of</param>
    /// <param name="strings">Localized string source to fetch with</param>
    /// <param name="species">Species ID the form belongs to</param>
    /// <param name="context">Format the form name should appear in</param>
    public static string GetStringFromForm(byte form, GameStrings strings, ushort species, EntityContext context)
    {
        if (form == 0)
            return string.Empty;

        var forms = FormConverter.GetFormList(species, strings.Types, strings.forms, genderForms, context);
        var result = form >= forms.Length ? string.Empty : forms[form];

        // Showdown uses a non-standard representation for some forms, and uses interstitial dashes instead of spaces.
        return GetShowdownFormName(species, result);
    }

    private const string MiniorFormName = "Meteor";

    /// <summary>
    /// Converts the PKHeX standard form name to Showdown's form name.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">PKHeX form name</param>
    public static string GetShowdownFormName(ushort species, string form)
    {
        if (form.Length == 0)
        {
            return species switch
            {
                (int)Minior => MiniorFormName,
                _ => form,
            };
        }

        return species switch
        {
            (int)Basculin when form is "Blue"           => "Blue-Striped",
            (int)Vivillon when form is "Poké Ball"      => "Pokeball",
            (int)Zygarde                                => form.Replace("-C", string.Empty).Replace("50%", string.Empty),
            (int)Minior   when form.StartsWith("M-", StringComparison.OrdinalIgnoreCase)  => MiniorFormName,
            (int)Minior                                 => form.Replace("C-", string.Empty),
            (int)Necrozma when form is "Dusk"           => $"{form}-Mane",
            (int)Necrozma when form is "Dawn"           => $"{form}-Wings",
            (int)Polteageist or (int)Sinistea           => form == "Antique" ? form : string.Empty,
            (int)Maushold when form is "Family of Four" => "Four",

            (int)Greninja or (int)Rockruff or (int)Koraidon or (int)Miraidon => string.Empty,

            _ => FormInfo.HasTotemForm(species) && form == "Large"
                ? species is (int)Raticate or (int)Marowak ? "Alola-Totem" : "Totem"
                : form.Replace(' ', '-'),
        };
    }

    public static bool IsTotemForm(ReadOnlySpan<char> formName) =>
        formName.Equals("Totem", StringComparison.OrdinalIgnoreCase) ||
        formName.Equals("Alola-Totem", StringComparison.OrdinalIgnoreCase) || 
        formName.Equals("Large", StringComparison.OrdinalIgnoreCase);


    public static bool IsCosplayPikachu(ReadOnlySpan<char> formName, ReadOnlySpan<string> formNames)
        => FormConverter.IsCosplayPikachu(formName, formNames);

    /// <summary>
    /// Converts the Showdown form name to PKHeX's form name.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Showdown form name</param>
    /// <param name="ability">Showdown ability ID</param>
    public static string GetFormNameFromShowdownFormName(ushort species, string form, int ability)
    {
        if (form.Length != 0)
            form = form.Replace(' ', '-'); // inconsistencies are great

        return species switch
        {
            (int)Basculin   when form is "Blue-Striped" => "Blue",
            (int)Vivillon   when form is "Pokeball"     => "Poké Ball",
            (int)Necrozma   when form is "Dusk-Mane"    => "Dusk",
            (int)Necrozma   when form is "Dawn-Wings"   => "Dawn",
            (int)Toxtricity when form is "Low-Key"      => "Low Key",
            (int)Darmanitan when form is "Galar-Zen"    => "Galar Zen",
            (int)Minior     when form is not MiniorFormName => $"C-{form}",
            (int)Zygarde    when form is "Complete"     => form,
            (int)Zygarde    when ability == 211         => $"{(string.IsNullOrWhiteSpace(form) ? "50%" : "10%")}-C",
            (int)Greninja   when ability == 210         => "Ash", // Battle Bond
            (int)Rockruff   when ability == 020         => "Dusk", // Rockruff-1
            (int)Maushold   when form is "Four"         => "Family of Four",
            (int)Urshifu or (int)Pikachu or (int)Alcremie => form.Replace('-', ' '), // Strike and Cosplay

            _ => FormInfo.HasTotemForm(species) && form.EndsWith("Totem", StringComparison.OrdinalIgnoreCase) ? "Large" : form,
        };
    }

    /// <summary>
    /// Fetches <see cref="ShowdownSet"/> data from the input <see cref="lines"/>.
    /// </summary>
    /// <param name="lines">Raw lines containing numerous multi-line set data.</param>
    /// <param name="localization">Localization data for the set.</param>
    /// <returns><see cref="ShowdownSet"/> objects until <see cref="lines"/> is consumed.</returns>
    public static IEnumerable<ShowdownSet> GetShowdownSets(IEnumerable<string> lines, BattleTemplateLocalization localization)
    {
        // exported sets always have >4 moves; new List will always require 1 resizing, allocate 2x to save 1 reallocation.
        // intro, nature, ability, (ivs, evs, shiny, level) 4*moves
        var setLines = new List<string>(DefaultListAllocation);
        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                setLines.Add(line);
                continue;
            }
            if (setLines.Count == 0)
                continue;
            yield return new ShowdownSet(setLines, localization);
            setLines.Clear();
        }
        if (setLines.Count != 0)
            yield return new ShowdownSet(setLines, localization);
    }

    /// <inheritdoc cref="GetShowdownSets(IEnumerable{string},BattleTemplateLocalization)"/>
    public static IEnumerable<ShowdownSet> GetShowdownSets(IEnumerable<string> lines)
    {
        var setLines = new List<string>(DefaultListAllocation);
        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                setLines.Add(line);
                continue;
            }
            if (setLines.Count == 0)
                continue;
            yield return TryParseAnyLanguage(setLines, out var set) ? set : new ShowdownSet(setLines);
            setLines.Clear();
        }
        if (setLines.Count != 0)
            yield return TryParseAnyLanguage(setLines, out var set) ? set : new ShowdownSet(setLines);
    }

    /// <inheritdoc cref="GetShowdownSets(IEnumerable{string},BattleTemplateLocalization)"/>
    public static IEnumerable<ShowdownSet> GetShowdownSets(ReadOnlyMemory<char> text, BattleTemplateLocalization localization)
    {
        int start = 0;
        do
        {
            var span = text.Span;
            var slice = span[start..];
            var set = GetShowdownSet(slice, localization, out int length);
            if (set.Species == 0)
                break;
            yield return set;
            start += length;
        }
        while (start < text.Length);
    }

    /// <inheritdoc cref="GetShowdownSets(IEnumerable{string},BattleTemplateLocalization)"/>
    /// <summary>
    /// Language-unknown version of <see cref="GetShowdownSets(IEnumerable{string},BattleTemplateLocalization)"/>.
    /// </summary>
    public static IEnumerable<ShowdownSet> GetShowdownSets(ReadOnlyMemory<char> text)
    {
        int start = 0;
        do
        {
            var span = text.Span;
            var slice = span[start..];
            var set = GetShowdownSet(slice, out int length);
            if (set.Species == 0)
                break;
            yield return set;
            start += length;
        }
        while (start < text.Length);
    }

    /// <inheritdoc cref="GetShowdownSets(ReadOnlyMemory{char},BattleTemplateLocalization)"/>
    public static IEnumerable<ShowdownSet> GetShowdownSets(string text, BattleTemplateLocalization localization) => GetShowdownSets(text.AsMemory(), localization);

    private static int GetLength(ReadOnlySpan<char> text)
    {
        // Find the end of the Showdown Set lines.
        // The end is implied when:
        // - we see a complete whitespace or empty line
        int length = 0;

        while (true)
        {
            var newline = text.IndexOf('\n');
            if (newline == -1)
                return length + text.Length;

            var slice = text[..newline];
            var used = newline + 1;
            length += used;

            if (slice.IsWhiteSpace())
                return length;
            text = text[used..];
        }
    }

    /// <summary>
    /// Attempts to parse the input <see cref="text"/> into a <see cref="ShowdownSet"/> object.
    /// </summary>
    /// <param name="text">Input string to parse.</param>
    /// <param name="localization">Input localization to use.</param>
    /// <param name="length">Amount of characters consumed from the input string.</param>
    /// <returns>Parsed <see cref="ShowdownSet"/> object if successful, otherwise might be a best-match with some/all unparsed lines.</returns>
    public static ShowdownSet GetShowdownSet(ReadOnlySpan<char> text, BattleTemplateLocalization localization, out int length)
    {
        length = GetLength(text);
        var slice = text[..length];
        var set = new ShowdownSet(slice, localization);
        while (length < text.Length && text[length] is '\r' or '\n' or ' ')
            length++;
        return set;
    }

    /// <inheritdoc cref="GetShowdownSet(ReadOnlySpan{char},BattleTemplateLocalization,out int)"/>
    public static ShowdownSet GetShowdownSet(ReadOnlySpan<char> text, out int length)
    {
        length = GetLength(text);
        var slice = text[..length];
        if (!TryParseAnyLanguage(slice, out var set))
            set = new ShowdownSet(slice); // should never fall back
        while (length < text.Length && text[length] is '\r' or '\n' or ' ')
            length++;
        return set;
    }

    /// <inheritdoc cref="GetShowdownSets(ReadOnlyMemory{char},BattleTemplateLocalization)"/>
    public static IEnumerable<ShowdownSet> GetShowdownSets(string text) => GetShowdownSets(text.AsMemory());

    /// <inheritdoc cref="GetShowdownText(PKM, in BattleTemplateExportSettings)"/>
    public static string GetShowdownText(PKM pk) => GetShowdownText(pk, BattleTemplateExportSettings.Showdown);

    /// <summary>
    /// Converts the <see cref="PKM"/> data into an importable set format for Pokémon Showdown.
    /// </summary>
    /// <param name="pk">PKM to convert to string</param>
    /// <param name="settings">Import localization/style setting</param>
    /// <returns>Multi line set data</returns>
    public static string GetShowdownText(PKM pk, in BattleTemplateExportSettings settings)
    {
        if (pk.Species == 0)
            return string.Empty;
        var set = new ShowdownSet(pk);
        set.InterpretAsPreview(pk);
        return set.GetText(settings);
    }

    /// <summary>
    /// Fetches ShowdownSet lines from the input <see cref="PKM"/> data.
    /// </summary>
    /// <param name="data">Pokémon data to summarize.</param>
    /// <param name="settings">Export localization/style setting</param>
    /// <returns>Consumable list of <see cref="ShowdownSet.Text"/> lines.</returns>
    public static IEnumerable<string> GetShowdownText(IEnumerable<PKM> data, in BattleTemplateExportSettings settings)
    {
        List<string> result = new();
        var sets = GetShowdownSets(data);
        foreach (var set in sets)
            result.Add(set.GetText(settings));
        return result;
    }

    /// <summary>
    /// Fetches ShowdownSet lines from the input <see cref="PKM"/> data.
    /// </summary>
    /// <param name="data">Pokémon data to summarize.</param>
    /// <returns>Consumable list of <see cref="ShowdownSet.Text"/> lines.</returns>
    public static IEnumerable<ShowdownSet> GetShowdownSets(IEnumerable<PKM> data)
    {
        foreach (var pk in data)
        {
            if (pk.Species == 0)
                continue;
            yield return new ShowdownSet(pk);
        }
    }

    /// <inheritdoc cref="GetShowdownSets(IEnumerable{string},BattleTemplateLocalization)"/>
    public static string GetShowdownSets(IEnumerable<PKM> data, string separator) => string.Join(separator, GetShowdownText(data, BattleTemplateExportSettings.Showdown));

    /// <summary>
    /// Fetches ShowdownSet lines from the input <see cref="PKM"/> data, and combines it into one string.
    /// </summary>
    /// <param name="data">Pokémon data to summarize.</param>
    /// <param name="separator">Splitter between each set.</param>
    /// <param name="settings">Import localization/style setting</param>
    /// <returns>Single string containing all <see cref="ShowdownSet.Text"/> lines.</returns>
    public static string GetShowdownSets(IEnumerable<PKM> data, string separator, in BattleTemplateExportSettings settings) => string.Join(separator, GetShowdownText(data, settings));

    /// <summary>
    /// Gets a localized string preview of the provided <see cref="pk"/>.
    /// </summary>
    /// <param name="pk">Pokémon data</param>
    /// <param name="settings">Export settings</param>
    /// <returns>Multi-line string</returns>
    public static string GetLocalizedPreviewText(PKM pk, in BattleTemplateExportSettings settings)
    {
        var set = new ShowdownSet(pk);
        set.InterpretAsPreview(pk);
        return set.GetText(settings);
    }

    /// <summary>
    /// Tries to parse the input string into a <see cref="ShowdownSet"/> object.
    /// </summary>
    /// <param name="message">Input string to parse.</param>
    /// <param name="set">Parsed <see cref="ShowdownSet"/> object if successful, otherwise might be a best-match with some unparsed lines.</param>
    /// <returns>True if the input was parsed successfully, false otherwise.</returns>
    public static bool TryParseAnyLanguage(ReadOnlySpan<char> message, [NotNullWhen(true)] out ShowdownSet? set)
    {
        set = null;
        if (message.Length == 0)
            return false;

        var invalid = int.MaxValue;
        var all = BattleTemplateLocalization.GetAll();
        foreach (var lang in all)
        {
            var local = lang.Value;
            var tmp = new ShowdownSet(message, local);
            var bad = tmp.InvalidLines.Count;
            if (bad == 0)
            {
                set = tmp;
                return true;
            }

            // Check for invalid lines
            if (bad >= invalid)
                continue;

            // Best so far.
            invalid = bad;
            set = tmp;
        }
        if (set is null)
            return false;
        return set.Species != 0;
    }

    /// <inheritdoc cref="TryParseAnyLanguage(ReadOnlySpan{char}, out ShowdownSet?)"/>
    public static bool TryParseAnyLanguage(IReadOnlyList<string> setLines, [NotNullWhen(true)] out ShowdownSet? set)
    {
        set = null;
        if (setLines.Count == 0)
            return false;

        var invalid = int.MaxValue;
        var all = BattleTemplateLocalization.GetAll();
        foreach (var lang in all)
        {
            var local = lang.Value;
            var tmp = new ShowdownSet(setLines, local);
            var bad = tmp.InvalidLines.Count;
            if (bad == 0)
            {
                set = tmp;
                return true;
            }

            // Check for invalid lines
            if (bad >= invalid)
                continue;

            // Best so far.
            invalid = bad;
            set = tmp;
        }
        return false;
    }

    /// <summary>
    /// Tries to translate the input battle template <see cref="message"/> into a localized string.
    /// </summary>
    /// <param name="message">Input string to parse.</param>
    /// <param name="outputSettings">Export settings</param>
    /// <param name="translated">Translated string if successful.</param>
    /// <returns><c>true</c> if the input was translated successfully, <c>false</c> otherwise.</returns>
    public static bool TryTranslate(ReadOnlySpan<char> message, BattleTemplateExportSettings outputSettings, [NotNullWhen(true)] out string? translated)
    {
        translated = null;
        if (!TryParseAnyLanguage(message, out var set))
            return false;
        translated = set.GetText(outputSettings);
        return true;
    }

    /// <inheritdoc cref="TryTranslate(ReadOnlySpan{char}, BattleTemplateExportSettings, out string?)"/>
    public static bool TryTranslate(IReadOnlyList<string> message, BattleTemplateExportSettings outputSettings, [NotNullWhen(true)] out string? translated)
    {
        translated = null;
        if (!TryParseAnyLanguage(message, out var set))
            return false;
        translated = set.GetText(outputSettings);
        return true;
    }
}
