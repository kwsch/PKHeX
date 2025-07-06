using System;
using static PKHeX.Core.LanguageID;

namespace PKHeX.Core;

/// <summary>
/// Logic for replacing the name of a Pokémon in <see cref="EntityContext.Gen9"/>.
/// </summary>
public static class ReplaceTrainerName9
{
    private const EntityContext Context = EntityContext.Gen9;

    /// <summary>
    /// Checks if the original name is a trigger for replacement, and if the current name is a valid replacement.
    /// </summary>
    /// <param name="original">Original name to check for replacement trigger.</param>
    /// <param name="current">Current name to check for valid replacement.</param>
    /// <param name="language">Entity language.</param>
    public static bool IsTriggerAndReplace(ReadOnlySpan<char> original, ReadOnlySpan<char> current, LanguageID language)
    {
        if (!IsTrigger(original, language))
            return false;
        return IsReplace(current, language);
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
    public static bool IsReplace(ReadOnlySpan<char> name, LanguageID language)
    {
        var expect = GetName(language);
        return name.SequenceEqual(expect);
    }

    /// <summary>
    /// Gets the replacement name for <see cref="EntityContext.Gen9"/> in the given language.
    /// </summary>
    public static string GetName(LanguageID language) => language switch
    {
        Japanese => "スカーレット",
        French => "Écarlate",
        Italian => "Scarlatto",
        German => "Karmesin",
        Spanish => "Escarlata",
        Korean => "스칼렛",
        ChineseS or ChineseT => "朱",

        _ => "Scarlet",
    };
}
