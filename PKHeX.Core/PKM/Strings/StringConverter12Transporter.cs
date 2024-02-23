using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> from Generation 1 &amp; 2 games to Generation 7.
/// </summary>
public static class StringConverter12Transporter
{
    private const ushort Terminator = 0;

    /// <summary>
    /// Converts Generation 1/2 encoded data the same way Bank converts.
    /// </summary>
    /// <param name="data">Generation 1 encoded data.</param>
    /// <param name="jp">Data source is Japanese.</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data, bool jp)
    {
        Span<char> result = stackalloc char[data.Length];
        int length = LoadString(data, result, jp);
        return new string(result[..length]);
    }

    /// <inheritdoc cref="GetString(ReadOnlySpan{byte},bool)"/>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <param name="jp">Data source is Japanese.</param>
    /// <returns>Character count loaded.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result, bool jp)
    {
        var table = jp ? CharTableJPN : CharTableINT;
        int i = 0;
        for (; i < data.Length; i++)
        {
            var b = data[i];
            if (b == 0)
                break;

            var value = table[b];
            if (value == Terminator)
                break;

            result[i] = (char)value;
        }
        CheckKata(result[..i]);
        return i;
    }

    private static void CheckKata(Span<char> chars)
    {
        bool isAnyKata = IsAnyKataRemap(chars);
        if (!isAnyKata)
            return;

        if (IsAnyKataOnly(chars))
            return;

        foreach (ref var c in chars)
        {
            if (Katakana.Contains(c))
                c -= (char)0x60; // shift to Hiragana
        }
    }

    /// <summary>
    /// Checks if any char is from the clashing Katakana range.
    /// </summary>
    private static bool IsAnyKataRemap(ReadOnlySpan<char> chars) => chars.ContainsAny(Katakana);

    private static bool IsAnyKataOnly(ReadOnlySpan<char> chars)
    {
        foreach (var c in chars)
        {
            if (c - 0x3041u < 0x53)
                return false; // Hiragana
            if (c - 0x30A1u < 0x56)
                return true;
        }
        return false;
    }

    // ベ (U+30D9), ペ (U+30DA), ヘ (U+30D8), and リ (U+30EA)
    private const string Katakana = "ベペヘリ";
    // べ (U+3079), ぺ (U+307A), へ (U+3078), and り (U+308A)
  //private const string Hiragana = "べぺへり";

    /// <summary>
    /// International 1/2->7 character translation table
    /// </summary>
    /// <remarks>
    /// Exported from Gen1's VC string transferring, with manual modifications for the two permitted accent marks:
    /// <br>0xC1 at arr[0xBF] = Á (Spanish In-game Trade Voltorb, FALCÁN)</br>
    /// <br>0xCD at arr[0xC9] = Í (Spanish In-game Trade Shuckle, MANÍA)</br>
    /// <br>All other new language sensitive re-mapping (or lack thereof) are inaccessible via the character entry screen.</br>
    /// </remarks>
    private static ReadOnlySpan<ushort> CharTableINT =>
    [
        0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 0
        0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 1
        0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 2
        0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 3
        0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 4
        0x0000, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 5
        0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 6
        0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 7
        0x0041, 0x0042, 0x0043, 0x0044, 0x0045, 0x0046, 0x0047, 0x0048, 0x0049, 0x004A, 0x004B, 0x004C, 0x004D, 0x004E, 0x004F, 0x0050, // 8
        0x0051, 0x0052, 0x0053, 0x0054, 0x0055, 0x0056, 0x0057, 0x0058, 0x0059, 0x005A, 0x0028, 0x0029, 0x003A, 0x003B, 0x0028, 0x0029, // 9
        0x0061, 0x0062, 0x0063, 0x0064, 0x0065, 0x0066, 0x0067, 0x0068, 0x0069, 0x006A, 0x006B, 0x006C, 0x006D, 0x006E, 0x006F, 0x0070, // A
        0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077, 0x0078, 0x0079, 0x007A, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x00C1, // B
        0x00C4, 0x00D6, 0x00DC, 0x00E4, 0x00F6, 0x00FC, 0x0020, 0x0020, 0x0020, 0x00CD, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // C
        0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // D
        0x0020, 0x0050, 0x004D, 0x002D, 0x0020, 0x0020, 0x003F, 0x0021, 0x002D, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0xE08E, // E
        0x0020, 0x0078, 0x002E, 0x002F, 0x002C, 0xE08F, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // F
    ];

    /// <summary>
    /// Japanese 1/2->7 character translation table
    /// </summary>
    /// <remarks>Full-width 0-9 removed from the Japanese table as these glyphs are inaccessible via the character entry screen.</remarks>
    private static ReadOnlySpan<ushort> CharTableJPN =>
    [
        0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30AC, 0x30AE, 0x30B0, 0x30B2, 0x30B4, 0x30B6, 0x30B8, 0x30BA, 0x30BC, 0x30BE, 0x30C0, // 0
        0x30C2, 0x30C5, 0x30C7, 0x30C9, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30D0, 0x30D3, 0x30D6, 0x30DC, 0x3000, 0x3000, 0x3000, // 1
        0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x304C, 0x304E, 0x3050, 0x3052, 0x3054, 0x3056, 0x3058, 0x305A, 0x305C, 0x305E, // 2
        0x3060, 0x3062, 0x3065, 0x3067, 0x3069, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3070, 0x3073, 0x3076, 0x30D9, 0x307C, 0x3000, // 3
        0x30D1, 0x30D4, 0x30D7, 0x30DD, 0x3071, 0x3074, 0x3077, 0x30DA, 0x307D, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, // 4
        0x0000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, // 5
        0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, // 6
        0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, // 7
        0x30A2, 0x30A4, 0x30A6, 0x30A8, 0x30AA, 0x30AB, 0x30AD, 0x30AF, 0x30B1, 0x30B3, 0x30B5, 0x30B7, 0x30B9, 0x30BB, 0x30BD, 0x30BF, // 8
        0x30C1, 0x30C4, 0x30C6, 0x30C8, 0x30CA, 0x30CB, 0x30CC, 0x30CD, 0x30CE, 0x30CF, 0x30D2, 0x30D5, 0x30DB, 0x30DE, 0x30DF, 0x30E0, // 9
        0x30E1, 0x30E2, 0x30E4, 0x30E6, 0x30E8, 0x30E9, 0x30EB, 0x30EC, 0x30ED, 0x30EF, 0x30F2, 0x30F3, 0x30C3, 0x30E3, 0x30E5, 0x30E7, // A
        0x30A3, 0x3042, 0x3044, 0x3046, 0x3048, 0x304A, 0x304B, 0x304D, 0x304F, 0x3051, 0x3053, 0x3055, 0x3057, 0x3059, 0x305B, 0x305D, // B
        0x305F, 0x3061, 0x3064, 0x3066, 0x3068, 0x306A, 0x306B, 0x306C, 0x306D, 0x306E, 0x306F, 0x3072, 0x3075, 0x30D8, 0x307B, 0x307E, // C
        0x307F, 0x3080, 0x3081, 0x3082, 0x3084, 0x3086, 0x3088, 0x3089, 0x30EA, 0x308B, 0x308C, 0x308D, 0x308F, 0x3092, 0x3093, 0x3063, // D
        0x3083, 0x3085, 0x3087, 0x30FC, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30A1, 0x30A5, 0x30A7, 0x3000, 0x3000, 0x3000, 0x2642, // E
        0x3000, 0x3000, 0x3000, 0x3000, 0x30A9, 0x2640, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, // F
    ];

    /// <summary>
    /// Gets the Trainer Name for a Generation 1 in-game trade (NPC).
    /// </summary>
    /// <param name="language">Language to localize with</param>
    public static string GetTradeNameGen1(int language) => language switch
    {
        1 => "トレーナー",
        2 => "Trainer",
        3 => "Dresseur",
        4 => "Allenatore",
        5 => "Trainer",
        7 => "Entrenador",
      //8 => "트레이너",
      //9 => "训练家",
      //10 => "訓練家",
        _ => string.Empty,
    };

    /// <summary>
    /// Gets a "safe" Trainer Name for a Generation 1 or 2 trainer, in the event the original was naughty.
    /// </summary>
    /// <param name="language">Language to localize with</param>
    /// <param name="version">Version transferred from to Bank</param>
    public static string GetFilteredOT(int language, GameVersion version) => version switch
    {
        GameVersion.RD => language switch
        {
            1 => "レッド．",
            2 => "Red*",
            3 => "Rouge*",
            4 => "Rosso*",
            5 => "Rot*",
            7 => "Rojo*",
            _ => string.Empty,
        },
        GameVersion.GN => language switch
        {
            1 => "グリーン．",
            2 => "Blue*",
            3 => "Bleu*",
            4 => "Blu*",
            5 => "Blau*",
            7 => "Azul*",
            _ => string.Empty,
        },
        GameVersion.BU => language switch
        {
            1 => "ブルー．",
            _ => string.Empty,
        },
        GameVersion.YW => language switch
        {
            1 => "イエロー．",
            2 => "Yellow*",
            3 => "Jaune*",
            4 => "Giallo*",
            5 => "Gelb*",
            7 => "Amarillo*",
            _ => string.Empty,
        },
        GameVersion.GD => language switch
        {
            1 => "ゴールド．",
            2 => "Gold*",
            3 => "Or*",
            4 => "Oro*",
            5 => "Gold*",
            7 => "Oro*",
            8 => "금.",
            _ => string.Empty,
        },
        GameVersion.SI => language switch
        {
            1 => "シルバー．",
            2 => "Silver*",
            3 => "Argent*",
            4 => "Argento*",
            5 => "Silber*",
            7 => "Plata*",
            8 => "은.",
            _ => string.Empty,
        },
        GameVersion.C => language switch
        {
            1 => "クリスタル．",
            2 => "Crystal*",
            3 => "Cristal*",
            4 => "Cristallo*",
            5 => "Kristall*",
            7 => "Cristal*",
            _ => string.Empty,
        },
        _ => string.Empty,
    };
}
