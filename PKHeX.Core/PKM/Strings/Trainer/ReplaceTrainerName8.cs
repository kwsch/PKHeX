using System;
using static PKHeX.Core.LanguageID;

namespace PKHeX.Core;

/// <summary>
/// Logic for replacing the name of a Pokémon in <see cref="EntityContext.Gen8"/>.
/// </summary>
public static class ReplaceTrainerName8
{
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
        bool result = !IsValid(name, language);
        if (result)
            return true;

        // Skip CheckNgWords: Numbers, whitespace, whitewords, nn::ngc -- implicitly flagged by our WordFilter. No legitimate events trigger this.

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
    /// Gets the replacement name for <see cref="EntityContext.Gen8"/> in the given language.
    /// </summary>
    public static string GetName(LanguageID language) => language switch
    {
        Japanese => "ソード.",
        French => "Épée.",
        Italian => "Spada.",
        German => "Schwert.",
        Spanish => "Espada.",
        Korean => "소드.",
        ChineseS => "剑.",
        ChineseT => "劍.",

        _ => "Sword.",
    };

    /// <summary>
    /// Checks if the character is in the CJK Unified Ideographs range (Chinese, Japanese, Korean).
    /// </summary>
    /// <remarks>
    /// Range only for Common and uncommon kanji, excluding rare kanji and symbols. Likely GB2312.
    /// </remarks>
    private static bool IsCJK(char c)      => c is (>= (char)0x4E00 and <= (char)0x9FA0) and not (char)0x4EDD;

    // General full-width character checking methods
    private static bool IsHiragana(char c) => c is (>= (char)0x3041 and <= (char)0x3090);
    private static bool IsKatakana(char c) => c is (>= (char)0x30A1 and <= (char)0x30FA);
    private static bool IsKanji(char c)    => c is (>= (char)0x4E00 and <= (char)0x9FCC); // version 6.1
    private static bool IsHangul(char c)   => c is (>= (char)0xAC00 and <= (char)0xD7A3);

    /// <summary>
    /// Checks if the entered text is a valid string for the specified language.
    /// </summary>
    /// <param name="name">Input string to validate.</param>
    /// <param name="language">Entity language to validate against.</param>
    /// <returns><see langword="true"/> if the string is valid for the specified language; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(ReadOnlySpan<char> name, LanguageID language)
    {
        // Check if fullwidth is used, and if it doesn't exceed 6 chars.
        // Japanese has a special check, disallowing CJK range (except for a symbol char).
        if (language is Japanese)
        {
            foreach (var c in name)
            {
                if (IsCJK(c))
                    return false;
            }
        }

        if (IsAnyFullWidthLengthTooLong(name, out _))
            return false;

        return true;
    }

    /// <summary>
    /// Checks if any full-width characters are entered, and if so, ensures the total length does not exceed 6 characters.
    /// </summary>
    /// <param name="name">Input string to validate.</param>
    /// <param name="anyFullWidth">Indicates if any full-width characters were found in the input.</param>
    /// <returns><see langword="true"/> if any full-width characters are found in a too-long string.</returns>
    public static bool IsAnyFullWidthLengthTooLong(ReadOnlySpan<char> name, out bool anyFullWidth)
    {
        // disassembled logic scans each character, and if a full-width char has been found at any index, checks if current index > 6
        // we already know the length, so we can just check if any full-width exists
        anyFullWidth = false;
        foreach (var c in name)
        {
            anyFullWidth = IsHiragana(c) || IsKatakana(c) || IsKanji(c) || IsHangul(c);
            if (anyFullWidth)
                return name.Length > Legal.MaxLengthTrainerAsian;
        }
        return false;
    }
}
