using System;
using static PKHeX.Core.EntityContext;
using static PKHeX.Core.LanguageID;
using static PKHeX.Core.PersonalTable;

namespace PKHeX.Core;

/// <summary>
/// Logic for replacing the name of a Pokémon in any game adjacent to HOME.
/// </summary>
public static class ReplaceTrainerNameHOME
{
    private const EntityContext Context = Gen9;

    /// <summary>
    /// Gets the replacement name for trades within the HOME context.
    /// </summary>
    public const string ReplaceName = "HOME";

    /// <summary>
    /// Checks if the original name is a trigger for replacement, and if the current name is a valid replacement.
    /// </summary>
    /// <param name="original">Original name to check for replacement trigger.</param>
    /// <param name="current">Current name to check for valid replacement.</param>
    /// <param name="language">Entity language.</param>
    /// <param name="origin">Entity game version.</param>
    /// <param name="species">Original species to check for game presence.</param>
    /// <param name="form">Original form to check for game presence.</param>
    public static EntityContext IsTriggerAndReplace(ReadOnlySpan<char> original, ReadOnlySpan<char> current, LanguageID language, GameVersion origin, ushort species, byte form)
    {
        // Use the encounter species-form to determine if the Pokémon can visit the game.
        if (SWSH.IsPresentInGame(species, form) && ReplaceTrainerName8 .IsTriggerAndReplace(original, current, language))
            return Gen8;
        if (LA  .IsPresentInGame(species, form) && ReplaceTrainerName8a.IsTriggerAndReplace(original, current, language))
            return Gen8a;
        if (BDSP.IsPresentInGame(species, form) && ReplaceTrainerName8b.IsTriggerAndReplace(original, current, language, origin))
            return Gen8b;
        if (SV  .IsPresentInGame(species, form) && ReplaceTrainerName9 .IsTriggerAndReplace(original, current, language))
            return Gen9;
        if (IsTrigger(original, language) && IsReplace(current))
            return Context;
        return EntityContext.None; // No replacement
    }

    /// <inheritdoc cref="IsTriggerAndReplace(ReadOnlySpan{char},ReadOnlySpan{char},LanguageID,GameVersion,ushort,byte)"/>
    public static EntityContext IsTriggerAndReplace(ReadOnlySpan<char> original, ReadOnlySpan<char> current, LanguageID language, GameVersion origin, EvolutionHistory history)
    {
        if (history.HasVisitedSWSH && ReplaceTrainerName8 .IsTriggerAndReplace(original, current, language))
            return Gen8;
        if (history.HasVisitedPLA  && ReplaceTrainerName8a.IsTriggerAndReplace(original, current, language))
            return Gen8a;
        if (history.HasVisitedBDSP && ReplaceTrainerName8b.IsTriggerAndReplace(original, current, language, origin))
            return Gen8b;
        if (history.HasVisitedGen9 && ReplaceTrainerName9 .IsTriggerAndReplace(original, current, language))
            return Gen9;
        if (IsTrigger(original, language) && IsReplace(current))
            return Context;
        return EntityContext.None; // No replacement
    }

    /// <summary>
    /// Checks if the current string is a replaced string for the given game version and language.
    /// </summary>
    /// <param name="current">The current trainer name as a read-only span of characters.</param>
    /// <param name="language">The language identifier for the trainer name.</param>
    /// <param name="origin">The game version where the Pokémon originated.</param>
    /// <param name="species">Original species to check for game presence.</param>
    /// <param name="form">Original form to check for game presence.</param>
    public static EntityContext IsReplace(ReadOnlySpan<char> current, LanguageID language, GameVersion origin, ushort species, byte form)
    {
        // Use the encounter species-form to determine if the Pokémon can visit the game.
        if (SWSH.IsPresentInGame(species, form) && ReplaceTrainerName8 .IsReplace(current, language))
            return Gen8;
        if (LA  .IsPresentInGame(species, form) && ReplaceTrainerName8a.IsReplace(current, language))
            return Gen8a;
        if (BDSP.IsPresentInGame(species, form) && ReplaceTrainerName8b.IsReplace(current, language, origin))
            return Gen8b;
        if (SV  .IsPresentInGame(species, form) && ReplaceTrainerName9 .IsReplace(current, language))
            return Gen9;
        if (current.SequenceEqual(ReplaceName))
            return Context;
        return EntityContext.None; // No replacement
    }

    /// <inheritdoc cref="IsReplace(ReadOnlySpan{char},LanguageID,GameVersion,ushort,byte)"/>
    public static EntityContext IsReplace(ReadOnlySpan<char> current, LanguageID language, GameVersion origin, EvolutionHistory history)
    {
        if (history.HasVisitedSWSH && ReplaceTrainerName8 .IsReplace(current, language))
            return Gen8;
        if (history.HasVisitedPLA  && ReplaceTrainerName8a.IsReplace(current, language))
            return Gen8a;
        if (history.HasVisitedBDSP && ReplaceTrainerName8b.IsReplace(current, language, origin))
            return Gen8b;
        if (history.HasVisitedGen9 && ReplaceTrainerName9 .IsReplace(current, language))
            return Gen9;
        if (IsReplace(current))
            return Context;
        return EntityContext.None; // No replacement
    }

    /// <summary>
    /// Checks if the provided name is one of the valid replacement names for the specified language and game version.
    /// </summary>
    /// <param name="name">Current name to check for valid replacement.</param>
    public static bool IsReplace(ReadOnlySpan<char> name) => name is ReplaceName;

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
        // Undefined Chars - Patrick's Pelipper gift has \u200b (zero width space)
        // Other triggers:
        // Traded eggs from another language - ENG origin, KOR hatched => KOR OT, reset to HOME.
        // ---- This is because it fails the "IsDefined" char check for the origin language.
        bool result = StringFontUtil.HasUndefinedCharacters(name, Context, language, language);
        if (result)
            return true;

        // Too-long OT names from length-limited languages (Japanese, Korean, Chinese)
        // ---- Simple length check. Usually flagged by other legality checks, but can legally trigger for some event gifts.
        if (language is (Japanese or Korean or ChineseS or ChineseT) && ReplaceTrainerName8.IsAnyFullWidthLengthTooLong(name, out _))
            return true;

        // Tested, but does not trigger (unlike other environments):
        // NO: Mixed full-width & half-width entry
        // --- [ァaaァァ] as flagged in Gen7. Probably no longer flagged because software keyboards might allow blending.
        // NO: Using Full-width chars from half-width language, exceeding full-width entry length.
        // --- [ァァァァァァァ] as flagged in Gen8. Probably removed for same reasons above/future-proofing.

        // Skip checks that never occur for fixed-string encounters (like events).
        // We can flag them separately, and the user can trigger sanitization via arbitrary text entry.
        // Skip CheckNgWords: Numbers, whitespace, whitewords, nn::ngc -- implicitly flagged by our WordFilter. 
        // Skip trash byte checks.

        return false; // OK
    }
}
