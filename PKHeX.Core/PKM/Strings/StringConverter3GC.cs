using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.StringConverter3;
using static PKHeX.Core.LanguageID;
using static PKHeX.Core.GCRegion;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for Generation 3 GameCube games.
/// </summary>
public static class StringConverter3GC
{
    public const char TerminatorBigEndian = (char)0; // GC

    /// <summary>Converts Big Endian encoded data to decoded string.</summary>
    /// <param name="data">Encoded data</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data)
    {
        Span<char> result = stackalloc char[data.Length];
        int length = LoadString(data, result);
        return new string(result[..length]);
    }

    /// <inheritdoc cref="GetString(ReadOnlySpan{byte})"/>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <returns>Character count loaded.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result)
    {
        int i = 0;
        for (; i < data.Length; i += 2)
        {
            var value = (char)ReadUInt16BigEndian(data[i..]);
            if (value == TerminatorBigEndian)
                break;
            result[i/2] = value;
        }
        return i/2;
    }

    /// <summary>Gets the bytes for a Big Endian string.</summary>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="option">Option to clear the buffer. Only <see cref="StringConverterOption.ClearZero"/> is recognized.</param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength,
        StringConverterOption option)
    {
        if (value.Length > maxLength)
            value = value[..maxLength]; // Hard cap

        if (option is StringConverterOption.ClearZero)
            destBuffer.Clear();

        for (int i = 0; i < value.Length; i++)
        {
            var c = value[i];
            WriteUInt16BigEndian(destBuffer[(i * 2)..], c);
        }

        if (destBuffer.Length == value.Length * 2)
            return value.Length * 2;
        WriteUInt16BigEndian(destBuffer[(value.Length * 2)..], TerminatorBigEndian);
        return (value.Length * 2) + 2;
    }

    #region GC/GBA Conversion

    private const byte SaveInvalidAs = 0xB7; // '$' (INT), '円' (JP)

    /// <summary>
    /// Remaps GBA Glyphs to GC Glyphs.
    /// </summary>
    public static void RemapGlyphs3GC(ReadOnlySpan<byte> data, GCRegion region, int language, Span<byte> dest)
    {
        bool jpn = region == NTSC_J || language == (int)Japanese;
        bool palFG = region == PAL && language is ((int)French or (int)German);

        var table = jpn ? CharTableJPN : CharTableINT;
        int i = 0;
        for (; i < data.Length; i++)
        {
            var b = data[i];
            var value = table[b];
            if (value == TerminatorBigEndian)
                break;

            // Quotation marks are displayed differently based on the Gen3 game language.
            // PAL region copies handle them specially only if the PKM language is French/German.
            if (palFG)
            {
                value = b switch
                {
                    QuoteLeftByte => GetQuoteLeft(language),
                    QuoteRightByte => GetQuoteRight(language),
                    _ => value,
                };
            }

            WriteUInt16BigEndian(dest[(i * 2)..], value);
        }
        dest[(i * 2)..].Clear();
    }

    /// <summary>
    /// Remaps GC Glyphs between regions.
    /// </summary>
    /// <remarks>
    /// GC games cannot trade with each other directly, only with a GBA game as an intermediary.
    /// </remarks>
    public static void RemapGlyphsBetweenRegions3GC(Span<byte> data, GCRegion oldRegion, GCRegion newRegion, int language)
    {
        if (oldRegion == newRegion)
            return; // No changes needed.

        // Transfer to GBA then back to GC.
        Span<byte> temp = stackalloc byte[data.Length / 2];
        RemapGlyphs3GBA(data, oldRegion, language, temp);
        RemapGlyphs3GC(temp, newRegion, language, data);
    }

    /// <summary>
    /// Remaps GC Glyphs to GBA Glyphs.
    /// </summary>
    public static void RemapGlyphs3GBA(ReadOnlySpan<byte> data, GCRegion region, int language, Span<byte> dest)
    {
        bool jpn = region == NTSC_J || language == (int)Japanese;
        bool pfg = region == PAL && language is ((int)French or (int)German);

        var table = jpn ? CharTableJPN : CharTableINT;
        int i = 0;
        for (; i < data.Length; i++)
        {
            var c = (char)ReadUInt16BigEndian(data[(i * 2)..]);
            if (c == TerminatorBigEndian)
                break;

            var value = table.IndexOf(c);
            if (value == -1)
                value = SaveInvalidAs;

            // Quotation marks are displayed differently based on the Gen3 game language.
            // PAL region copies handle them specially only if the PKM language is French/German.
            if (pfg)
            {
                value = c switch
                {
                    '«' or '„' => QuoteLeftByte,
                    '»' or '“' => QuoteRightByte,
                    _ => value,
                };
            }

            dest[i] = (byte)value;
        }
        if (i < dest.Length)
        {
            dest[i] = TerminatorByte;
            dest[(i+1)..].Clear();
        }
    }

    /// <summary>
    /// Japanese GC/GBA character translation table
    /// </summary>
    private static ReadOnlySpan<ushort> CharTableJPN =>
    [
        0x3000, 0x3042, 0x3044, 0x3046, 0x3048, 0x304A, 0x304B, 0x304D, 0x304F, 0x3051, 0x3053, 0x3055, 0x3057, 0x3059, 0x305B, 0x305D, // 0
        0x305F, 0x3061, 0x3064, 0x3066, 0x3068, 0x306A, 0x306B, 0x306C, 0x306D, 0x306E, 0x306F, 0x3072, 0x3075, 0x3078, 0x307B, 0x307E, // 1
        0x307F, 0x3080, 0x3081, 0x3082, 0x3084, 0x3086, 0x3088, 0x3089, 0x308A, 0x308B, 0x308C, 0x308D, 0x308F, 0x3092, 0x3093, 0x3041, // 2
        0x3043, 0x3045, 0x3047, 0x3049, 0x3083, 0x3085, 0x3087, 0x304C, 0x304E, 0x3050, 0x3052, 0x3054, 0x3056, 0x3058, 0x305A, 0x305C, // 3
        0x305E, 0x3060, 0x3062, 0x3065, 0x3067, 0x3069, 0x3070, 0x3073, 0x3076, 0x3079, 0x307C, 0x3071, 0x3074, 0x3077, 0x307A, 0x307D, // 4
        0x3063, 0x30A2, 0x30A4, 0x30A6, 0x30A8, 0x30AA, 0x30AB, 0x30AD, 0x30AF, 0x30B1, 0x30B3, 0x30B5, 0x30B7, 0x30B9, 0x30BB, 0x30BD, // 5
        0x30BF, 0x30C1, 0x30C4, 0x30C6, 0x30C8, 0x30CA, 0x30CB, 0x30CC, 0x30CD, 0x30CE, 0x30CF, 0x30D2, 0x30D5, 0x30D8, 0x30DB, 0x30DE, // 6
        0x30DF, 0x30E0, 0x30E1, 0x30E2, 0x30E4, 0x30E6, 0x30E8, 0x30E9, 0x30EA, 0x30EB, 0x30EC, 0x30ED, 0x30EF, 0x30F2, 0x30F3, 0x30A1, // 7
        0x30A3, 0x30A5, 0x30A7, 0x30A9, 0x30E3, 0x30E5, 0x30E7, 0x30AC, 0x30AE, 0x30B0, 0x30B2, 0x30B4, 0x30B6, 0x30B8, 0x30BA, 0x30BC, // 8
        0x30BE, 0x30C0, 0x30C2, 0x30C5, 0x30C7, 0x30C9, 0x30D0, 0x30D3, 0x30D6, 0x30D9, 0x30DC, 0x30D1, 0x30D4, 0x30D7, 0x30DA, 0x30DD, // 9
        0x30C3, 0xFF10, 0xFF11, 0xFF12, 0xFF13, 0xFF14, 0xFF15, 0xFF16, 0xFF17, 0xFF18, 0xFF19, 0xFF01, 0xFF1F, 0x3002, 0x30FC, 0x30FB, // A
        0x2025, 0x300E, 0x300F, 0x300C, 0x300D, 0x2642, 0x2640, 0x5186, 0xFF0E, 0x00D7, 0xFF0F, 0xFF21, 0xFF22, 0xFF23, 0xFF24, 0xFF25, // B
        0xFF26, 0xFF27, 0xFF28, 0xFF29, 0xFF2A, 0xFF2B, 0xFF2C, 0xFF2D, 0xFF2E, 0xFF2F, 0xFF30, 0xFF31, 0xFF32, 0xFF33, 0xFF34, 0xFF35, // C
        0xFF36, 0xFF37, 0xFF38, 0xFF39, 0xFF3A, 0xFF41, 0xFF42, 0xFF43, 0xFF44, 0xFF45, 0xFF46, 0xFF47, 0xFF48, 0xFF49, 0xFF4A, 0xFF4B, // D
        0xFF4C, 0xFF4D, 0xFF4E, 0xFF4F, 0xFF50, 0xFF51, 0xFF52, 0xFF53, 0xFF54, 0xFF55, 0xFF56, 0xFF57, 0xFF58, 0xFF59, 0xFF5A, 0x0000, // E
        0xFF1A, 0x00C4, 0x00D6, 0x00DC, 0x00E4, 0x00F6, 0x00FC, 0x2191, 0x2193, 0x2190, 0x2192, 0xFF0B, 0x0000, 0x0000, 0x0000, 0x0000, // F
    ];

    /// <summary>
    /// International GC/GBA character translation table
    /// </summary>
    private static ReadOnlySpan<ushort> CharTableINT =>
    [
        0x0020, 0x00C0, 0x00C1, 0x00C2, 0x00C7, 0x00C8, 0x00C9, 0x00CA, 0x00CB, 0x00CC, 0x0000, 0x00CE, 0x00CF, 0x00D2, 0x00D3, 0x00D4, // 0
        0x0152, 0x00D9, 0x00DA, 0x00DB, 0x00D1, 0x00DF, 0x00E0, 0x00E1, 0x0000, 0x00E7, 0x00E8, 0x00E9, 0x00EA, 0x00EB, 0x00EC, 0x0000, // 1
        0x00EE, 0x00EF, 0x00F2, 0x00F3, 0x00F4, 0x0153, 0x00F9, 0x00FA, 0x00FB, 0x00F1, 0x00BA, 0x00AA, 0xFEFE, 0x0026, 0x002B, 0x0000, // 2
        0x0000, 0x0000, 0x0000, 0x0000, 0xFEFE, 0x003D, 0x003B, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, // 3
        0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, // 4
        0x0000, 0x00BF, 0x00A1, 0xFEFE, 0xFEFE, 0xFEFE, 0xFEFE, 0xFEFE, 0xFEFE, 0xFEFE, 0x00CD, 0x0025, 0x0028, 0x0029, 0x0000, 0x0000, // 5
        0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x00E2, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x00ED, // 6
        0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x2191, 0x2193, 0x2190, 0xFF0B, 0x0000, 0x0000, 0x0000, // 7
        0x0000, 0x0000, 0x0000, 0x0000, 0xFEFE, 0x003C, 0x003E, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, // 8
        0x30BE, 0x30C0, 0x30C2, 0x30C5, 0x30C7, 0x30C9, 0x30D0, 0x30D3, 0x30D6, 0x30D9, 0x30DC, 0x30D1, 0x30D4, 0x30D7, 0x30DA, 0x30DD, // 9
        0xFEFE, 0x0030, 0x0031, 0x0032, 0x0033, 0x0034, 0x0035, 0x0036, 0x0037, 0x0038, 0x0039, 0x0021, 0x003F, 0x002E, 0x002D, 0x30FB, // A
        0x2030, 0x201C, 0x201D, 0x2018, 0x0027, 0x2642, 0x2640, 0x0024, 0x002C, 0x00D7, 0x002F, 0x0041, 0x0042, 0x0043, 0x0044, 0x0045, // B
        0x0046, 0x0047, 0x0048, 0x0049, 0x004A, 0x004B, 0x004C, 0x004D, 0x004E, 0x004F, 0x0050, 0x0051, 0x0052, 0x0053, 0x0054, 0x0055, // C
        0x0056, 0x0057, 0x0058, 0x0059, 0x005A, 0x0061, 0x0062, 0x0063, 0x0064, 0x0065, 0x0066, 0x0067, 0x0068, 0x0069, 0x006A, 0x006B, // D
        0x006C, 0x006D, 0x006E, 0x006F, 0x0070, 0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077, 0x0078, 0x0079, 0x007A, 0x0000, // E
        0x003A, 0x00C4, 0x00D6, 0x00DC, 0x00E4, 0x00F6, 0x00FC, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, // F
    ];

    #endregion
}
