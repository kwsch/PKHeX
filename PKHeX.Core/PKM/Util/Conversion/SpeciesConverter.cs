using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a National Pokédex Species ID to/from generation specific values.
/// </summary>
/// <remarks>Specific generations store their Species IDs in different orders from the National Dex ID.</remarks>
public static class SpeciesConverter
{
    /// <summary>
    /// Converts a National Dex ID to Generation 1 internal species ID.
    /// </summary>
    /// <param name="species">National Dex ID.</param>
    /// <returns>Generation 1 species ID.</returns>
    public static byte GetInternal1(ushort species) => species >= Table1NationalToInternal.Length ? (byte)0 : Table1NationalToInternal[species];

    /// <summary>
    /// Converts a Generation 1 internal species ID to National Dex ID.
    /// </summary>
    /// <param name="raw">Generation 1 species ID.</param>
    /// <returns>National Dex ID.</returns>
    public static byte GetNational1(byte raw) => Table1InternalToNational[raw];

    /// <summary>
    /// Converts a National Dex ID to Generation 3 internal species ID.
    /// </summary>
    /// <param name="species">National Dex ID</param>
    /// <returns>Generation 3 species ID.</returns>
    public static ushort GetInternal3(ushort species)
    {
        var shift = species - FirstUnalignedNational3;
        var table = Table3NationalToInternal;
        if ((uint)shift >= table.Length)
            return species;
        return (ushort)(species + table[shift]);
    }

    /// <summary>
    /// Converts a Generation 3 internal species ID to National Dex ID.
    /// </summary>
    /// <param name="raw">Generation 3 species ID.</param>
    /// <returns>National Dex ID.</returns>
    public static ushort GetNational3(ushort raw)
    {
        if (raw < FirstUnalignedNational3)
            return raw;
        var shift = raw - FirstUnalignedInternal3;
        var table = Table3InternalToNational;
        if ((uint)shift >= table.Length)
            return 0;
        return (ushort)(raw + table[shift]);
    }

    /// <summary>
    /// Converts a National Dex ID to Generation 9 internal species ID.
    /// </summary>
    /// <param name="species">National Dex ID</param>
    /// <returns>Generation 9 species ID.</returns>
    public static ushort GetInternal9(ushort species)
    {
        var shift = species - FirstUnalignedNational9;
        var table = Table9NationalToInternal;
        if ((uint)shift >= table.Length)
            return species;
        return (ushort)(species + table[shift]);
    }

    /// <summary>
    /// Converts a Generation 9 internal species ID to National Dex ID.
    /// </summary>
    /// <param name="raw">Generation 9 species ID.</param>
    /// <returns>National Dex ID.</returns>
    public static ushort GetNational9(ushort raw)
    {
        var table = Table9InternalToNational;
        var shift = raw - FirstUnalignedInternal9;
        if ((uint)shift >= table.Length)
            return raw;
        return (ushort)(raw + table[shift]);
    }

    // both tables are 0x100 bytes to eliminate bounds checks when requesting byte indexes
    private static ReadOnlySpan<byte> Table1NationalToInternal => [ 0x00, 0x99, 0x09, 0x9A, 0xB0, 0xB2, 0xB4, 0xB1, 0xB3, 0x1C, 0x7B, 0x7C, 0x7D, 0x70, 0x71, 0x72, 0x24, 0x96, 0x97, 0xA5, 0xA6, 0x05, 0x23, 0x6C, 0x2D, 0x54, 0x55, 0x60, 0x61, 0x0F, 0xA8, 0x10, 0x03, 0xA7, 0x07, 0x04, 0x8E, 0x52, 0x53, 0x64, 0x65, 0x6B, 0x82, 0xB9, 0xBA, 0xBB, 0x6D, 0x2E, 0x41, 0x77, 0x3B, 0x76, 0x4D, 0x90, 0x2F, 0x80, 0x39, 0x75, 0x21, 0x14, 0x47, 0x6E, 0x6F, 0x94, 0x26, 0x95, 0x6A, 0x29, 0x7E, 0xBC, 0xBD, 0xBE, 0x18, 0x9B, 0xA9, 0x27, 0x31, 0xA3, 0xA4, 0x25, 0x08, 0xAD, 0x36, 0x40, 0x46, 0x74, 0x3A, 0x78, 0x0D, 0x88, 0x17, 0x8B, 0x19, 0x93, 0x0E, 0x22, 0x30, 0x81, 0x4E, 0x8A, 0x06, 0x8D, 0x0C, 0x0A, 0x11, 0x91, 0x2B, 0x2C, 0x0B, 0x37, 0x8F, 0x12, 0x01, 0x28, 0x1E, 0x02, 0x5C, 0x5D, 0x9D, 0x9E, 0x1B, 0x98, 0x2A, 0x1A, 0x48, 0x35, 0x33, 0x1D, 0x3C, 0x85, 0x16, 0x13, 0x4C, 0x66, 0x69, 0x68, 0x67, 0xAA, 0x62, 0x63, 0x5A, 0x5B, 0xAB, 0x84, 0x4A, 0x4B, 0x49, 0x58, 0x59, 0x42, 0x83, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 ];
    private static ReadOnlySpan<byte> Table1InternalToNational => [ 0x00, 0x70, 0x73, 0x20, 0x23, 0x15, 0x64, 0x22, 0x50, 0x02, 0x67, 0x6C, 0x66, 0x58, 0x5E, 0x1D, 0x1F, 0x68, 0x6F, 0x83, 0x3B, 0x97, 0x82, 0x5A, 0x48, 0x5C, 0x7B, 0x78, 0x09, 0x7F, 0x72, 0x00, 0x00, 0x3A, 0x5F, 0x16, 0x10, 0x4F, 0x40, 0x4B, 0x71, 0x43, 0x7A, 0x6A, 0x6B, 0x18, 0x2F, 0x36, 0x60, 0x4C, 0x00, 0x7E, 0x00, 0x7D, 0x52, 0x6D, 0x00, 0x38, 0x56, 0x32, 0x80, 0x00, 0x00, 0x00, 0x53, 0x30, 0x95, 0x00, 0x00, 0x00, 0x54, 0x3C, 0x7C, 0x92, 0x90, 0x91, 0x84, 0x34, 0x62, 0x00, 0x00, 0x00, 0x25, 0x26, 0x19, 0x1A, 0x00, 0x00, 0x93, 0x94, 0x8C, 0x8D, 0x74, 0x75, 0x00, 0x00, 0x1B, 0x1C, 0x8A, 0x8B, 0x27, 0x28, 0x85, 0x88, 0x87, 0x86, 0x42, 0x29, 0x17, 0x2E, 0x3D, 0x3E, 0x0D, 0x0E, 0x0F, 0x00, 0x55, 0x39, 0x33, 0x31, 0x57, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x44, 0x00, 0x37, 0x61, 0x2A, 0x96, 0x8F, 0x81, 0x00, 0x00, 0x59, 0x00, 0x63, 0x5B, 0x00, 0x65, 0x24, 0x6E, 0x35, 0x69, 0x00, 0x5D, 0x3F, 0x41, 0x11, 0x12, 0x79, 0x01, 0x03, 0x49, 0x00, 0x76, 0x77, 0x00, 0x00, 0x00, 0x00, 0x4D, 0x4E, 0x13, 0x14, 0x21, 0x1E, 0x4A, 0x89, 0x8E, 0x00, 0x51, 0x00, 0x00, 0x04, 0x07, 0x05, 0x08, 0x06, 0x00, 0x00, 0x00, 0x00, 0x2B, 0x2C, 0x2D, 0x45, 0x46, 0x47, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 ];

    // Use a static span to avoid allocations -- these delta tables shift between the two representations.
    // Since most values are A<->A, we only need to store table values for those that are non-zero delta values.
    private const int FirstUnalignedNational3 = Legal.MaxSpeciesID_2 + 1; // 252
    private const int FirstUnalignedInternal3 = 277;

    private const int FirstUnalignedNational9 = 917;
    private const int FirstUnalignedInternal9 = FirstUnalignedNational9;

    /// <summary>
    /// Difference of National Dex IDs (index) and the associated Gen3 Species IDs (value)
    /// </summary>
    private static ReadOnlySpan<sbyte> Table3NationalToInternal =>
    [
                  025, 025, 025, 025, 025, 025, 025, 025,
        025, 025, 025, 025, 025, 025, 025, 025, 025, 025,
        025, 025, 025, 025, 025, 025, 028, 028, 031, 031,
        112, 112, 112, 028, 028, 021, 021, 077, 077, 077,
        011, 011, 011, 077, 077, 077, 039, 039, 052, 021,
        015, 015, 020, 052, 078, 078, 078, 049, 049, 028,
        028, 042, 042, 073, 073, 048, 051, 051, 012, 012,
        -07, -07, 017, 017, -03, 026, 026, -19, 004, 004,
        004, 013, 013, 025, 025, 045, 043, 011, 011, -16,
        -16, -15, -15, -25, -25, 043, 043, 043, 043, -21,
        -21, 034, -35, 024, 024, 006, 006, 012, 053, 017,
        000, -15, -15, -22, -22, -22, 007, 007, 007, 012,
        -45, 024, 024, 024, 024, 024, 024, 024, 024, 024,
        027, 027, 022, 022, 022, 024, 024,
    ];

    /// <summary>
    /// Difference of Gen3 Species IDs (index) and the associated National Dex IDs (value)
    /// </summary>
    private static ReadOnlySpan<sbyte> Table3InternalToNational =>
    [
                                           -25, -25, -25,
        -25, -25, -25, -25, -25, -25, -25, -25, -25, -25,
        -25, -25, -25, -25, -25, -25, -25, -25, -25, -25,
        -25, -11, -11, -11, -28, -28, -21, -21, 019, -31,
        -31, -28, -28, 007, 007, -15, -15, 035, 025, 025,
        -21, 003, -20, 016, 016, 045, 015, 015, 021, 021,
        -12, -12, -04, -04, -04, -39, -39, -28, -28, -17,
        -17, 022, 022, 022, -13, -13, 015, 015, -11, -11,
        -52, -26, -26, -42, -42, -52, -49, -49, -25, -25,
        000, -06, -06, -48, -77, -77, -77, -51, -51, -12,
        -77, -77, -77, -07, -07, -07, -17, -24, -24, -43,
        -45, -12, -78, -78, -78, -34, -73, -73, -43, -43,
        -43, -43,-112,-112,-112, -24, -24, -24, -24, -24,
        -24, -24, -24, -24, -22, -22, -22, -27, -27, -24,
        -24, -53,
    ];

    /// <summary>
    /// Difference of National Dex IDs (index) and the associated Gen9 Species IDs (value)
    /// </summary>
    private static ReadOnlySpan<sbyte> Table9NationalToInternal =>
    [
                                           001, 001, 001,
        001, 033, 033, 033, 021, 021, 044, 044, 007, 007,
        007, 029, 031, 031, 031, 068, 068, 068, 002, 002,
        017, 017, 030, 030, 024, 024, 028, 028, 058, 058,
        012, -13, -13, -31, -31, -29, -29, 043, 043, 043,
        -31, -31, -03, -30, -30, -23, -23, -14, -24, -03,
        -03, -47, -47, -12, -27, -27, -44, -46, -26, 031,
        029, -53, -65, 025, -06, -03, -07, -04, -04, -08,
        -04, 001, -03, -03, -06, -04, -47, -47, -47, -23,
        -23, -05, -07, -09, -07, -20, -13, -09, -09, -29,
        -23, 001, 012, 012, 000, 000, 000, -06,  005, -06,
        -03, -03, -02, -04, -03, -03,
    ];

    /// <summary>
    /// Difference of Gen9 Species IDs (index) and the associated National Dex IDs (value)
    /// </summary>
    private static ReadOnlySpan<sbyte> Table9InternalToNational =>
    [
                                           065, -01, -01,
        -01, -01, 031, 031, 047, 047, 029, 029, 053, 031,
        031, 046, 044, 030, 030, -07, -07, -07, 013, 013,
        -02, -02, 023, 023, 024, -21, -21, 027, 027, 047,
        047, 047, 026, 014, -33, -33, -33, -17, -17, 003,
        -29, 012, -12, -31, -31, -31, 003, 003, -24, -24,
        -44, -44, -30, -30, -28, -28, 023, 023, 006, 007,
        029, 008, 003, 004, 004, 020, 004, 023, 006, 003,
        003, 004, -01, 013, 009, 007, 005, 007, 009, 009,
        -43, -43, -43, -68, -68, -68, -58, -58, -25, -29,
        -31, 006, -01, 006, 000, 000, 000, 003, 003, 004,
        002, 003, 003, -05, -12, -12,
    ];
}
