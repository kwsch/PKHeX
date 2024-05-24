using System;
using static PKHeX.Core.EReaderBerryMatch;

namespace PKHeX.Core;

/// <summary>
/// Stores details about the Gen3 e-Reader Berry values.
/// </summary>
public static class EReaderBerrySettings
{
    /// <summary> e-Reader Berry is Enigma or special berry (from e-Reader data)</summary>
    public static bool IsEnigma { get; set; } = true;

    /// <summary> e-Reader Berry Name </summary>
    private static string Name { get; set; } = string.Empty;

    /// <summary> e-Reader Berry Name formatted in Title Case </summary>
    public static string DisplayName => Util.ToTitleCase(string.Format(LegalityCheckStrings.L_XEnigmaBerry_0, Name));

    private static int Language { get; set; }

    /// <summary>
    /// Checks if the most recently loaded Generation 3 Save File has a proper Enigma Berry that matches known distributions.
    /// </summary>
    public static EReaderBerryMatch GetStatus()
    {
        if (IsEnigma) // no e-Reader Berry data provided, can't hold berry.
            return NoData;

        ReadOnlySpan<char> name = Name;
        if (IsEReaderBerriesNameUSA(name))
        {
            return Language switch
            {
                <= 0 => ValidAny,
                not 1 => ValidUSA,
                _ => InvalidUSA,
            };
        }
        if (IsEReaderBerriesNameJPN(name))
        {
            return Language switch
            {
                <= 0 => ValidAny,
                1 => ValidJPN,
                _ => InvalidJPN,
            };
        }
        return NoMatch;
    }

    public static bool IsEReaderBerriesNameUSA(ReadOnlySpan<char> name) => name is
        "PUMKIN" or
        "DRASH" or
        "EGGANT" or
        "STRIB" or
        "CHILAN" or
        "NUTPEA"
    ;

    public static bool IsEReaderBerriesNameJPN(ReadOnlySpan<char> name) => name is
        "カチャ" or // PUMKIN
        "ブ－カ" or // DRASH
        "ビスナ" or // EGGANT
        "エドマ" or // STRIB
        "ホズ" or // CHILAN
        "ラッカ" or // NUTPEA
        // JP Series 2
        "ギネマ" or // GINEMA
        "クオ" or // KUO
        "ヤゴ" or // YAGO
        "トウガ" or // TOUGA
        "ニニク" or // NINIKU
        "トポ" // TOPO
    ;

    public static void LoadFrom(SAV3 sav3)
    {
        Language = sav3.Japanese ? (int)LanguageID.Japanese : (int)LanguageID.English;
        if (sav3.IsEBerryEngima)
        {
            IsEnigma = true;
            Name = string.Empty;
        }
        else
        {
            IsEnigma = false;
            Name = sav3.EBerryName;
        }
    }
}

/// <summary>
/// For use in validating the current e-Reader Berry settings.
/// </summary>
public enum EReaderBerryMatch : byte
{
    /// <summary> Invalid: Doesn't match either language </summary>
    NoMatch,
    /// <summary> Invalid: No data provided from a save file </summary>
    NoData,
    /// <summary> Invalid: USA berry name on JPN save file </summary>
    InvalidUSA,
    /// <summary> Invalid: JPN berry name on USA save file</summary>
    InvalidJPN,

    /// <summary> Valid: Berry name matches either USA/JPN (Ambiguous Save File Language) </summary>
    ValidAny,
    /// <summary> Valid: Berry name matches a USA berry name </summary>
    ValidUSA,
    /// <summary> Valid: Berry name matches a JPN berry name </summary>
    ValidJPN,
}
