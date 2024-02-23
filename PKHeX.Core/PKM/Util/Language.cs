using System;
using static PKHeX.Core.LanguageID;

namespace PKHeX.Core;

/// <summary>
/// Logic relating to <see cref="LanguageID"/> values.
/// </summary>
public static class Language
{
    private static ReadOnlySpan<byte> Languages =>
    [
        (byte)Japanese,
        (byte)English,
        (byte)French,
        (byte)Italian,
        (byte)German,

        (byte)Spanish,

        (byte)Korean, // GS

        (byte)ChineseS,
        (byte)ChineseT,
    ];

    // check Korean for the VC case, never possible to match string outside of this case
    private static ReadOnlySpan<byte> Languages_GB => Languages[..7]; // [..KOR]
    private static ReadOnlySpan<byte> Languages_3  => Languages[..6]; // [..KOR)
    private const LanguageID SafeLanguage = English;

    /// <summary>
    /// Returns the available languages for the given generation.
    /// </summary>
    /// <param name="generation">Generation to check.</param>
    /// <returns>Available languages for the given generation.</returns>
    public static ReadOnlySpan<byte> GetAvailableGameLanguages(byte generation = PKX.Generation) => generation switch
    {
        1           => Languages_3, // No KOR
        2           => Languages_GB,
        3           => Languages_3, // No KOR
        4 or 5 or 6 => Languages_GB,
        _           => Languages,
    };

    private static bool HasLanguage(ReadOnlySpan<byte> permitted, byte language) => permitted.Contains(language);

    /// <summary>
    /// Returns the language that is safe to use for the given generation.
    /// </summary>
    /// <param name="generation">Generation to check.</param>
    /// <param name="prefer">Preferred language.</param>
    /// <param name="game">Game version to check.</param>
    /// <returns>Language that is safe to use for the given generation.</returns>
    public static LanguageID GetSafeLanguage(byte generation, LanguageID prefer, GameVersion game = GameVersion.Any) => generation switch
    {
        1 when game == GameVersion.BU => Japanese,
        1           => HasLanguage(Languages_3,  (byte)prefer) ? prefer : SafeLanguage,
        2           => HasLanguage(Languages_GB, (byte)prefer) && (prefer != Korean || game == GameVersion.C) ? prefer : SafeLanguage,
        3           => HasLanguage(Languages_3 , (byte)prefer) ? prefer : SafeLanguage,
        4 or 5 or 6 => HasLanguage(Languages_GB, (byte)prefer) ? prefer : SafeLanguage,
        _           => HasLanguage(Languages,    (byte)prefer) ? prefer : SafeLanguage,
    };

    /// <summary>
    /// Gets the 2-character language name for the given <see cref="LanguageID"/>.
    /// </summary>
    /// <param name="language">Language ID to get the 2-character name for.</param>
    /// <returns>2-character language name.</returns>
    public static string GetLanguage2CharName(this LanguageID language) => language switch
    {
        Japanese => "ja",
        French => "fr",
        Italian => "it",
        German => "de",
        Spanish => "es",
        Korean => "ko",
        ChineseS => "zh",
        ChineseT => "zh2",
        _ => GameLanguage.DefaultLanguage,
    };

    /// <summary>
    /// Gets the Main Series language ID from a GameCube (C/XD) language ID.
    /// </summary>
    /// <param name="value">GameCube (C/XD) language ID.</param>
    /// <returns>Main Series language ID.</returns>
    /// <remarks>If no conversion is possible or maps to the same value, the input <see cref="value"/> is returned.</remarks>
    public static byte GetMainLangIDfromGC(byte value) => (LanguageGC)value switch
    {
        LanguageGC.German =>   (byte)German,
        LanguageGC.French =>   (byte)French,
        LanguageGC.Italian =>  (byte)Italian,
        LanguageGC.Spanish =>  (byte)Spanish,
        LanguageGC.UNUSED_6 => (byte)UNUSED_6,
        _ => value,
    };

    /// <summary>
    /// Gets the GameCube (C/XD) language ID from a Main Series language ID.
    /// </summary>
    /// <param name="value">Main Series language ID.</param>
    /// <returns>GameCube (C/XD) language ID.</returns>
    /// <remarks>If no conversion is possible or maps to the same value, the input <see cref="value"/> is returned.</remarks>
    public static byte GetGCLangIDfromMain(byte value) => (LanguageID)value switch
    {
        French =>   (byte)LanguageGC.French,
        Italian =>  (byte)LanguageGC.Italian,
        German =>   (byte)LanguageGC.German,
        UNUSED_6 => (byte)LanguageGC.UNUSED_6,
        Spanish =>  (byte)LanguageGC.Spanish,
        _ => value,
    };
}
