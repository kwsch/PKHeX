using System;

namespace PKHeX.Core;

/// <summary>
/// Provides information for Unity Tower player location information.
/// </summary>
/// <remarks>These values were specific to the NDS games (Generation 5)</remarks>
public static class LocaleNDS5
{
    public const int CountryCount = 232;
    public const int Japan = 105;

    public static ReadOnlySpan<byte> LegalCountries =>
    [
        001, 002, 003, 006, 008, 009, 012, 013, 015, 016, 017, 018, 020, 021, 022, 023,
        025, 027, 028, 029, 031, 033, 034, 035, 036, 040, 042, 043, 045, 047, 048, 049,
        051, 053, 054, 058, 060, 061, 062, 063, 064, 071, 072, 073, 074, 076, 079, 080,
        081, 082, 083, 084, 085, 087, 088, 090, 091, 092, 093, 094, 095, 096, 098, 099,
        101, 102, 103, 105, 106, 109, 111, 115, 117, 118, 121, 125, 128, 130, 132, 134,
        138, 139, 141, 145, 147, 148, 149, 150, 151, 155, 156, 157, 160, 161, 163, 164,
        166, 167, 170, 173, 174, 181, 185, 186, 188, 189, 190, 191, 194, 195, 196, 198,
        199, 200, 201, 203, 205, 206, 210, 211, 215, 217, 218, 219, 220, 221, 222, 224,
        226, 227,
    ];

    public static byte GetSubregionCount(byte country) => country switch
    {
        009 => 24, // Argentina
        012 =>  8, // Australia
        028 => 27, // Brazil
        036 => 13, // Canada
        043 => 33, // China
        072 =>  6, // Finland
        073 => 22, // France
        079 => 16, // Germany
        095 => 35, // India
        102 => 20, // Italy
        105 => 50, // Japan
        155 => 22, // Norway
        166 => 16, // Poland
        174 =>  8, // Russian Federation
        195 => 17, // Spain
        200 => 22, // Sweden
        218 => 12, // United Kingdom
        220 => 51, // United States of America
        _ => 0,
    };
}
