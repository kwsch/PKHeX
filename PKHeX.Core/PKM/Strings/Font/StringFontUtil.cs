using System;
using static PKHeX.Core.EntityContext;
using static PKHeX.Core.LanguageID;
using G3GC = PKHeX.Core.StringFont3GC;
using G5 = PKHeX.Core.StringFont5;
using G6 = PKHeX.Core.StringFont6;
using G7 = PKHeX.Core.StringFont7;
using G8 = PKHeX.Core.StringFont8;
using G8a = PKHeX.Core.StringFont8a;
using G8b = PKHeX.Core.StringFont8b;

namespace PKHeX.Core;

/// <summary>
/// Utility Logic for checking whether a Unicode character is defined in a font.
/// </summary>
public static class StringFontUtil
{
    private static char GetUndefinedChar(EntityContext context) => context switch
    {
        Gen3 => '⬛', // 0x2B1B
        Gen7 => ' ', // 0x0020
        Gen8b => '☐', // 0x2610
        _ => '?', // 0x003F
    };

    /// <summary>
    /// Determines if a string contains undefined characters in the font for the given <see cref="EntityContext"/> and <see cref="LanguageID"/>.
    /// </summary>
    /// <param name="str">The input string to check for undefined characters.</param>
    /// <param name="context">The context to get the font for.</param>
    /// <param name="pkLanguage">The language of the Pokémon.</param>
    /// <param name="saveLanguage">The language of the save file.</param>
    /// <returns>True if the input string contains undefined characters; otherwise, false.</returns>
    public static bool HasUndefinedCharacters(ReadOnlySpan<char> str, EntityContext context, LanguageID pkLanguage, LanguageID saveLanguage)
    {
        foreach (var c in str)
        {
            if (!IsDefined(c, context, pkLanguage, saveLanguage))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Replaces any undefined characters in the font for the given <see cref="EntityContext"/> and <see cref="LanguageID"/> within a string with a fallback character.
    /// </summary>
    /// <param name="str">The input string to replace any undefined characters in.</param>
    /// <param name="context">The context to get the font for.</param>
    /// <param name="pkLanguage">The language of the Pokémon.</param>
    /// <param name="saveLanguage">The language of the save file.</param>
    /// <returns>The resulting string.</returns>
    public static string ReplaceUndefinedCharacters(ReadOnlySpan<char> str,
        EntityContext context, LanguageID pkLanguage, LanguageID saveLanguage)
    {
        Span<char> result = stackalloc char[str.Length];
        var length = ReplaceUndefinedCharacters(str, result, context, pkLanguage, saveLanguage);
        if (length == 0)
            return string.Empty;
        return new string(result[..length]);
    }

    /// <inheritdoc cref="ReplaceUndefinedCharacters(ReadOnlySpan{char}, EntityContext, LanguageID, LanguageID)"/>
    public static int ReplaceUndefinedCharacters(ReadOnlySpan<char> str, Span<char> result,
        EntityContext context, LanguageID pkLanguage, LanguageID saveLanguage)
    {
        var undefined = GetUndefinedChar(context);

        // In BD/SP, an empty string is shown in place of text in the save language with any undefined characters
        var blankIfUndefined = context is Gen8b && pkLanguage == saveLanguage;
        int i = 0;
        for (; i < str.Length; i++)
        {
            var c = str[i];
            var defined = IsDefined(c, context, pkLanguage, saveLanguage);
            if (!defined && blankIfUndefined)
                return 0; // Empty

            result[i] = defined ? c : undefined;
        }
        return i;
    }

    /// <summary>
    /// Determines if a character is defined in the font for the given <see cref="EntityContext"/> and <see cref="LanguageID"/>.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <param name="context">The context to get the font for.</param>
    /// <param name="pkLanguage">The language of the Pokémon.</param>
    /// <param name="saveLanguage">The language of the save file.</param>
    /// <returns>True if the character is defined; otherwise, false.</returns>
    public static bool IsDefined(char c, EntityContext context, LanguageID pkLanguage, LanguageID saveLanguage) => context switch
    {
        // Gen5/6/7 display names based on the save language, Gen7b/8/9 display names based on the entity's language
        Gen3                  => HasChar(c, G3GC.Defined),
        Gen5  or Gen4         => HasChar(c, G5.Defined),
        Gen6                  => HasChar(c, G6.Defined),
        Gen7  or Gen1 or Gen2 => IsDefined7(c, saveLanguage),
        Gen7b or Gen8 or Gen9 => IsDefined8(c, pkLanguage),
        Gen8a                 => IsDefined8a(c, pkLanguage),
        Gen8b                 => IsDefined8b(c, pkLanguage, saveLanguage),
        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };

    private static bool IsDefined7(char c, LanguageID language)
    {
        if (c is < '\u4E00' or > '\u9FFF')
            return HasChar(c, G7.Defined) || (language is ChineseT && HasChar(c, G7.DefinedCHTOnly));

        return IsDefined7Regular(c, language);
    }

    private static bool IsDefined7Regular(char c, LanguageID language) => language switch
    {
        ChineseT => HasChar(c - '\u4E00', G7.DefinedCHT),
        ChineseS => HasChar(c - '\u4E00', G7.DefinedCHS),
        _ => HasChar(c, G7.Defined),
    };

    public static bool IsDefined8(char c, LanguageID pkLanguage)
    {
        if (IsDefined8Regular(c, pkLanguage))
            return true;
        return HasChar(c, G8.DefinedPrivate);
    }

    private static bool IsDefined8Regular(char c, LanguageID pkLanguage) => pkLanguage switch
    {
        Japanese => HasChar(c, G8.DefinedJPN),
        Korean   => HasChar(c, G8.DefinedKOR) || HasChar(c, G8.DefinedKORAdded),
        ChineseS => HasChar(c, G8.DefinedCHS) || HasChar(c, G8.DefinedCHSExt) || HasChar(c, G8.DefinedCHSAdded),
        ChineseT => HasChar(c, G8.DefinedCHT),
        _ => HasChar(c, G8.DefinedINT),
    };

    // BD/SP uses the following in order:
    // 1. Font for Pokémon's language
    // 2. Font for save language
    // 3. Private use font
    // 4. Liberation Sans
    private static bool IsDefined8b(char c, LanguageID pkLanguage, LanguageID saveLanguage)
    {
        if (IsDefined8b(c, pkLanguage))
            return true;
        if (pkLanguage != saveLanguage && IsDefined8b(c, saveLanguage))
            return true;
        if (HasChar(c, G8b.DefinedLiberationSans))
            return true;
        return HasChar(c, G8.DefinedPrivate); // do last, slowest
    }

    private static bool IsDefined8b(char c, LanguageID entry) => entry switch
    {
        Japanese => HasChar(c, G8.DefinedJPN),
        Korean   => HasChar(c, G8.DefinedKOR),
        ChineseS => HasChar(c, G8.DefinedCHS) || HasChar(c, G8b.DefinedCHSExt),
        ChineseT => HasChar(c, G8.DefinedCHT),
        _ => HasChar(c, G8.DefinedINT),
    };

    private static bool IsDefined8a(char c, LanguageID entry)
        => IsDefined8(c, entry) || HasChar(c, G8a.DefinedPrivate);

    private static bool HasChar(char c, ReadOnlySpan<char> list) => list.BinarySearch(c) >= 0;
    private static bool HasChar(int i, ReadOnlySpan<byte> table) => ((table[i >> 3] >> (i & 7)) & 1) != 0;
}
