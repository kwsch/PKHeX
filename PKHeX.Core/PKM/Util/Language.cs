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

        (byte)SpanishL, // ZA: LATAM
    ];

    // check Korean for the VC case, never possible to match string outside of this case
    private static ReadOnlySpan<byte> Languages_GB => Languages[..7]; // [..KOR]
    private static ReadOnlySpan<byte> Languages_3  => Languages[..6]; // [..KOR)
    private static ReadOnlySpan<byte> Languages_9  => Languages[..9]; // [..CHT]
    private const LanguageID SafeLanguage = English;

    /// <summary>
    /// Returns the available languages for the given generation.
    /// </summary>
    /// <param name="context">Generation to check.</param>
    /// <returns>Available languages for the given generation.</returns>
    public static ReadOnlySpan<byte> GetAvailableGameLanguages(EntityContext context = Latest.Context) => context.Generation switch
    {
        1           => Languages_3, // No KOR
        2           => Languages_GB,
        3           => Languages_3, // No KOR
        4 or 5 or 6 => Languages_GB,
        7 or 8      => Languages_9,
        9 when context is EntityContext.Gen9 => Languages_9,
        _           => Languages,
    };

    private static bool HasLanguage(ReadOnlySpan<byte> permitted, LanguageID language) => permitted.Contains((byte)language);

    public static LanguageID GetSafeLanguage1(LanguageID prefer, GameVersion version)
    {
        if (version is GameVersion.BU)
            return Japanese;
        return HasLanguage(Languages_3, prefer) ? prefer : SafeLanguage;
    }

    public static LanguageID GetSafeLanguage2(LanguageID prefer) => HasLanguage(Languages_GB, prefer) ? prefer : SafeLanguage;
    public static LanguageID GetSafeLanguage3(LanguageID prefer) => HasLanguage(Languages_3, prefer) ? prefer : SafeLanguage;
    public static LanguageID GetSafeLanguage456(LanguageID prefer) => HasLanguage(Languages_GB, prefer) ? prefer : SafeLanguage;
    public static LanguageID GetSafeLanguage789(LanguageID prefer) => HasLanguage(Languages_9, prefer) ? prefer : SafeLanguage;
    public static LanguageID GetSafeLanguage9a(LanguageID prefer) => HasLanguage(Languages, prefer) ? prefer : SafeLanguage;

    /// <summary>
    /// Gets the language code for the given <see cref="LanguageID"/>.
    /// </summary>
    /// <param name="language">Language ID to get the language code for.</param>
    /// <returns>Language code.</returns>
    public static string GetLanguageCode(this LanguageID language) => language switch
    {
        Japanese => "ja",
        English => "en",
        French => "fr",
        Italian => "it",
        German => "de",
        Spanish => "es",
        Korean => "ko",
        ChineseS => "zh-Hans",
        ChineseT => "zh-Hant",
        SpanishL => "es-419",
        _ => GameLanguage.DefaultLanguage,
    };

    /// <summary>
    /// Gets the <see cref="LanguageID"/> value from a language code.
    /// </summary>
    /// <param name="language">Language code.</param>
    /// <returns>Language ID.</returns>
    public static LanguageID GetLanguageValue(string language) => language switch
    {
        "ja" => Japanese,
        "en" => English,
        "fr" => French,
        "it" => Italian,
        "de" => German,
        "es" => Spanish,
        "ko" => Korean,
        "zh-Hans" => ChineseS,
        "zh-Hant" => ChineseT,
        "es-419" => SpanishL,
        _ => GetLanguageValue(GameLanguage.DefaultLanguage),
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
