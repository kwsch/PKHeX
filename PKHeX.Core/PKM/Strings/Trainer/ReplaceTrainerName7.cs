using System;
using static PKHeX.Core.LanguageID;

namespace PKHeX.Core;

/// <summary>
/// Logic for replacing the name of a Pokémon in <see cref="EntityContext.Gen7"/>.
/// </summary>
public static class ReplaceTrainerName7
{
    private const EntityContext Context = EntityContext.Gen7;

    // Moving to Bank can avoid the replacement, and can therefore move between SN/MN to US/UM with the original name.
    // Therefore, even if traded, it doesn't have to be replaced.

    /// <summary>
    /// Checks if the original name is a trigger for replacement, and if the current name is a valid replacement.
    /// </summary>
    /// <param name="original">Original name to check for replacement trigger.</param>
    /// <param name="current">Current name to check for valid replacement.</param>
    /// <param name="language">Entity language.</param>
    /// <param name="origin">Entity game version.</param>
    public static bool IsTriggerAndReplace(ReadOnlySpan<char> original, ReadOnlySpan<char> current, LanguageID language, GameVersion origin)
    {
        if (!IsTrigger(original, language))
            return false;
        return IsReplace(current, language, origin);
    }

    /// <summary>
    /// Determines whether the specified name should be replaced based on language-specific rules.
    /// </summary>
    /// <remarks>This method checks for undefined characters in the name and applies additional rules for
    /// certain languages. For example, names longer than five characters are flagged for replacement in Asian languages
    /// such as Japanese, Korean, Simplified Chinese, and Traditional Chinese.</remarks>
    /// <param name="name">The name to evaluate, represented as a read-only span of characters.</param>
    /// <param name="language">The language identifier used to apply language-specific rules.</param>
    /// <returns><see langword="true"/> if the name contains undefined characters or violates language-specific constraints;
    /// otherwise, <see langword="false"/>.</returns>
    public static bool IsTrigger(ReadOnlySpan<char> name, LanguageID language)
    {
        bool result = StringFontUtil.HasUndefinedCharacters(name, Context, language, language);
        if (result)
            return true;

        // Skip trash byte checks since nothing is legally generated with them; they'll already be flagged via trash byte checks.

        return false; // OK
    }

    /// <summary>
    /// Checks if the provided name is one of the valid replacement names for the specified language and game version.
    /// </summary>
    /// <param name="name">Current name to check for valid replacement.</param>
    /// <param name="language">Entity language.</param>
    /// <param name="origin">Entity game version.</param>
    public static bool IsReplace(ReadOnlySpan<char> name, LanguageID language, GameVersion origin)
    {
        // Triggering the replacement can be done on SN/MN or US/UM.
        // When triggering in SN/MN and the version is MN, use MN name.
        // When triggering in US/UM and the version is UM, use MN name.
        // Otherwise, use SN name.
        // Note that trading in SN/MN and the version is UM, use SN name. Likewise, trading in US/UM and the version is SN, use SN name.
        // Therefore, everything can be replaced with SN name, and only MN/UM can be replaced with MN name if traded in their respective version sets.
        var sn = GetNameSun(language);
        if (name.SequenceEqual(sn))
            return true;

        if (origin is not (GameVersion.MN or GameVersion.UM))
            return false;

        var mn = GetNameMoon(language);
        if (name.SequenceEqual(mn))
            return true;
        return false;
    }

    /// <summary>
    /// Gets the replacement name for <see cref="GameVersion.SN"/> in the given language.
    /// </summary>
    public static string GetNameSun(LanguageID language) => language switch
    {
        Japanese => "サン．",
        French => "Soleil.",
        Italian => "Sole.",
        German => "Sonne.",
        Spanish => "Sol.",
        Korean => "썬．",
        ChineseS or ChineseT => "Ｓｕｎ．",

        _ => "Sun.",
    };

    /// <summary>
    /// Gets the replacement name for <see cref="GameVersion.MN"/> in the given language.
    /// </summary>
    public static string GetNameMoon(LanguageID language) => language switch
    {
        Japanese => "ムーン．",
        French => "Lune.",
        Italian => "Luna.",
        German => "Mond.",
        Spanish => "Luna.",
        Korean => "문．",
        ChineseS or ChineseT => "Ｍｏｏｎ．",

        _ => "Moon.",
    };
}
