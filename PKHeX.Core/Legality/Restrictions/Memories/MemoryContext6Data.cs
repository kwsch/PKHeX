using System;

namespace PKHeX.Core;

// data tables stored separately!
public partial class MemoryContext6
{
    private static ReadOnlySpan<byte> Memory_NotXY =>
    [
        65, // {0} was with {1} when (he/she) built a Secret Base. {4} that {3}.
        66, // {0} participated in a contest with {1} and impressed many people. {4} that {3}.
        67, // {0} participated in a contest with {1} and won the title. {4} that {3}.
        68, // {0} soared through the sky with {1} and went to many different places. {4} that {3}.
        69, // {1} asked {0} to dive. Down it went, deep into the ocean, to explore the bottom of the sea. {4} that {3}.
    ];

    private static ReadOnlySpan<byte> Memory_NotAO =>
    [
        11, // {0} went clothes shopping with {1}. {4} that {3}.
        43, // {0} was impressed by the speed of the train it took with {1}. {4} that {3}.
        44, // {0} encountered {2} with {1} using the Poké Radar. {4} that {3}.
        56, // {0} was with {1} when (he/she) went to a boutique and tried on clothes, but (he/she) left the boutique without buying anything. {4} that {3}.
        57, // {0} went to a nice restaurant with {1} and ate until it got totally full. {4} that {3}.
        62, // {0} saw itself in a mirror in a mirror cave that it went to with {1}. {4} that {3}.
    ];

    internal static ReadOnlySpan<byte> MoveSpecificMemoryHM => // Ordered by HM index for bitflag checks.
    [
        36, // {0} proudly used Cut at {1}’s instruction in... {2}. {4} that {3}.
        24, // {0} flew, carrying {1} on its back, to {2}. {4} that {3}.
        20, // {0} surfed across the water, carrying {1} on its back. {4} that {3}.
        35, // {0} proudly used Strength at {1}’s instruction in... {2}. {4} that {3}.
        38, // {0} used Waterfall while carrying {1} on its back in... {2}. {4} that {3}.
        37, // {0} shattered rocks to its heart’s content at {1}’s instruction in... {2}. {4} that {3}.
        69, // {1} asked {0} to dive. Down it went, deep into the ocean, to explore the bottom of the sea. {4} that {3}.
    ];

    /// <summary>
    /// Kalos locations with a Pokémon Center
    /// </summary>
    private static ReadOnlySpan<byte> LocationsWithPokeCenter_XY =>
    [
        // Kalos locations with a PKMN CENTER
        018, // Santalune City
        022, // Lumiose City
        030, // Camphrier Town
        040, // Cyllage City
        044, // Ambrette Town
        052, // Geosenge Town
        058, // Shalour City
        064, // Coumarine City
        070, // Laverre City
        076, // Dendemille Town
        086, // Anistar City
        090, // Couriway Town
        094, // Snowbelle City
        106, // Pokémon League (X/Y)
    ];

    /// <summary>
    /// Hoenn locations with a Pokémon Center
    /// </summary>
    private static ReadOnlySpan<byte> LocationsWithPokeCenter_AO =>
    [
        // Hoenn locations with a PKMN CENTER
        172, // Oldale Town
        174, // Dewford Town
        176, // Lavaridge Town
        178, // Fallarbor Town
        180, // Verdanturf Town
        182, // Pacifidlog Town
        184, // Petalburg City
        186, // Slateport City
        188, // Mauville City
        190, // Rustboro City
        192, // Fortree City
        194, // Lilycove City
        196, // Mossdeep City
        198, // Sootopolis City
        200, // Ever Grande City
        202, // Pokémon League (OR/AS)
    ];

    private static ReadOnlySpan<byte> MemoryMinIntensity =>
    [
        0, 1, 1, 1, 1, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 3, 3, 3, 3, 4, 4,
        3, 3, 3, 3, 3, 3, 3, 4, 5, 5,
        5, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        3, 3, 1, 3, 2, 2, 4, 3, 4, 4,
        4, 4, 2, 4, 2, 4, 3, 3, 4, 2,
        3, 3, 3, 3, 3, 2, 3, 4, 4, 2,
    ];

    private static ReadOnlySpan<byte> MemoryRandChance =>
    [
        000, 100, 100, 100, 100, 005, 005, 005, 005, 005,
        005, 005, 005, 005, 010, 020, 010, 001, 050, 030,
        005, 005, 020, 005, 005, 005, 001, 050, 100, 050,
        050, 002, 002, 005, 005, 005, 005, 005, 005, 002,
        020, 020, 005, 010, 001, 001, 050, 030, 020, 020,
        010, 010, 001, 010, 001, 050, 030, 030, 030, 002,
        050, 020, 020, 020, 020, 010, 010, 050, 020, 005,
    ];

    /// <summary>
    /// 24bits of flags allowing certain feelings for a given memory index.
    /// </summary>
    private static ReadOnlySpan<int> MemoryFeelings =>
    [
        0x000000, 0x04CBFD, 0x004BFD, 0x04CBFD, 0x04CBFD, 0xFFFBFB, 0x84FFF9, 0x47FFFF, 0xBF7FFA, 0x7660B0,
        0x80BDF9, 0x88FB7A, 0x083F79, 0x0001FE, 0xCFEFFF, 0x84EBAF, 0xB368B0, 0x091F7E, 0x0320A0, 0x080DDD,
        0x081A7B, 0x404030, 0x0FFFFF, 0x9A08BC, 0x089A7B, 0x0032AA, 0x80FF7A, 0x0FFFFF, 0x0805FD, 0x098278,
        0x0B3FFF, 0x8BBFFA, 0x8BBFFE, 0x81A97C, 0x8BB97C, 0x8BBF7F, 0x8BBF7F, 0x8BBF7F, 0x8BBF7F, 0xAC3ABE,
        0xBFFFFF, 0x8B837C, 0x848AFA, 0x88FFFE, 0x8B0B7C, 0xB76AB2, 0x8B1FFF, 0xBE7AB8, 0xB77EB8, 0x8C9FFD,
        0xBF9BFF, 0xF408B0, 0xBCFE7A, 0x8F3F72, 0x90DB7A, 0xBCEBFF, 0xBC5838, 0x9C3FFE, 0x9CFFFF, 0x96D83A,
        0xB770B0, 0x881F7A, 0x839F7A, 0x839F7A, 0x839F7A, 0x53897F, 0x41BB6F, 0x0C35FF, 0x8BBF7F, 0x8BBF7F,
    ];

    private static ReadOnlySpan<ushort> KeyItemMemoryArgsAnySpecies =>
    [
        466, // Gracidea
        628, 629, // DNA Splicers
        638, // Reveal Glass
        718, // Mach Bike
        719, // Acro Bike
        765, // Prison Bottle
    ];

    private static bool IsKeyItemMemoryArgValid(ushort species, ushort arg) => species switch
    {
        (int)Species.Shaymin => arg is 466, // Gracidea
        (int)Species.Tornadus => arg is 638, // Reveal Glass
        (int)Species.Thundurus => arg is 638, // Reveal Glass
        (int)Species.Landorus => arg is 638, // Reveal Glass
        (int)Species.Kyurem => arg is 628 or 629, // DNA Splicers
        (int)Species.Hoopa => arg is 765, // Prison Bottle
        _ => false,
    };

    /// <summary>
    /// Only item that can be observed by other Pokémon in party, to show up as a memory arg.
    /// </summary>
    private const ushort KeyItemUsableObserveEonFlute = 775; // Eon Flute

    private static ReadOnlySpan<ushort> PurchaseableItemXY =>
    [
        002, 003, 004, 006, 007, 008, 009, 010, 011, 012,
        013, 014, 015, 017, 018, 019, 020, 021, 022, 023,
        024, 025, 026, 027, 028, 034, 035, 036, 037, 045,
        046, 047, 048, 049, 052, 055, 056, 057, 058, 059,
        060, 061, 062, 076, 077, 078, 079, 082, 084, 085,
        254, 255, 314, 315, 316, 317, 318, 319, 320, 334,
        338, 341, 342, 343, 345, 347, 352, 355, 360, 364,
        365, 377, 379, 395, 402, 403, 405, 411, 618,
    ];

    private static ReadOnlySpan<ushort> PurchaseableItemAO =>
    [
        002, 003, 004, 006, 007, 008, 009, 010, 011, 013,
        014, 015, 017, 018, 019, 020, 021, 022, 023, 024,
        025, 026, 027, 028, 034, 035, 036, 037, 045, 046,
        047, 048, 049, 052, 055, 056, 057, 058, 059, 060,
        061, 062, 063, 064, 076, 077, 078, 079, 254, 255,
        314, 315, 316, 317, 318, 319, 320, 328, 336, 341,
        342, 343, 344, 347, 352, 360, 365, 367, 369, 374,
        379, 384, 395, 398, 400, 403, 405, 409, 577, 692,
        694,
    ];

    private static ReadOnlySpan<ushort> LotoPrizeXYAO =>
    [
        0001, 0033, 0050, 0051, 0053,
    ];

    /// <summary>
    /// Bitflag array with set bits indicating if the species ID can be captured in <see cref="GameVersion.X"/>.
    /// </summary>
    public static ReadOnlySpan<byte> CaptureFlagsX =>
    [
        0xB6, 0x75, 0xE1, 0x7B, 0xCB, 0x5A, 0x4A, 0xF5, 0x6C, 0xAD, 0x9C, 0xAA, 0x65, 0x93, 0xFF, 0xFF,
        0x3F, 0xD4, 0x5F, 0x00, 0x3A, 0x8D, 0x8C, 0xCF, 0x4E, 0xEC, 0xEA, 0xEF, 0x9B, 0x58, 0x82, 0x00,
        0xC0, 0x98, 0x54, 0x53, 0x64, 0xDE, 0xCB, 0xFF, 0xF9, 0xF1, 0x7F, 0x0A, 0xB7, 0xCA, 0x9E, 0x00,
        0x00, 0xB0, 0x51, 0x95, 0x8A, 0x06, 0xA6, 0x9E, 0xFB, 0x1C, 0x00, 0x80, 0x00, 0x00, 0x00, 0xC6,
        0x2A, 0xB9, 0x2C, 0xAD, 0xD1, 0xB0, 0x52, 0x9B, 0x76, 0xDC, 0x34, 0x81, 0xED, 0xED, 0xA9, 0x1D,
        0x00, 0x6C, 0x7B, 0x7F, 0xB7, 0x54, 0x73, 0xE5, 0x7B, 0x55,
    ];

    /// <summary>
    /// Bitflag array with set bits indicating if the species ID can be captured in <see cref="GameVersion.Y"/>.
    /// </summary>
    public static ReadOnlySpan<byte> CaptureFlagsY =>
    [
        0xB6, 0x7D, 0xE1, 0x7B, 0xCB, 0x5A, 0x4A, 0xF5, 0x6C, 0xAD, 0x9C, 0xAE, 0x65, 0x93, 0xFF, 0xFC,
        0x3F, 0xD4, 0x5F, 0x00, 0x3A, 0x8D, 0x8C, 0xCF, 0x4E, 0xEC, 0xEA, 0xEF, 0x8B, 0x58, 0xC2, 0x00,
        0xC0, 0x98, 0x54, 0x53, 0x64, 0xDE, 0xE8, 0xFF, 0xF9, 0xF1, 0x7F, 0x0A, 0xB7, 0x4A, 0x9F, 0x00,
        0x00, 0xB0, 0x51, 0x95, 0x8A, 0x06, 0xA6, 0x9E, 0xFB, 0x1C, 0x00, 0x80, 0x00, 0x00, 0x00, 0xC6,
        0x2A, 0xB9, 0x2C, 0xAD, 0xD1, 0xB0, 0x52, 0x9B, 0x76, 0xDC, 0x34, 0x81, 0xED, 0xED, 0xA9, 0x1D,
        0x00, 0x6C, 0x7B, 0x7F, 0xB7, 0x54, 0x4F, 0xE5, 0x7B, 0x65,
    ];

    /// <summary>
    /// Bitflag array with set bits indicating if the species ID can be captured in <see cref="GameVersion.AS"/>.
    /// </summary>
    public static ReadOnlySpan<byte> CaptureFlagsAS =>
    [
        0x00, 0x40, 0x1A, 0x0A, 0xA8, 0x5E, 0x66, 0x85, 0x04, 0xAF, 0xD2, 0x81, 0x36, 0xA0, 0xF4, 0x81,
        0x36, 0x56, 0x00, 0x49, 0x00, 0x8D, 0x84, 0xC1, 0x40, 0x2F, 0x40, 0xC4, 0x0F, 0x95, 0x39, 0x92,
        0xA4, 0xD7, 0xD0, 0xB9, 0x64, 0x5B, 0xBB, 0xDF, 0xBD, 0x29, 0xFB, 0xAB, 0xEB, 0x5B, 0x4E, 0x7E,
        0x4D, 0x02, 0x14, 0x05, 0x60, 0x8A, 0x11, 0x0F, 0x08, 0x05, 0x00, 0x00, 0xEF, 0x81, 0x24, 0x04,
        0xA0, 0x38, 0x9C, 0x18, 0x94, 0xF8, 0x58, 0x55, 0x00, 0x02, 0x8D, 0x24, 0x24, 0x20, 0x3C, 0xD2,
        0x75, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x05, 0x18,
    ];

    /// <summary>
    /// Bitflag array with set bits indicating if the species ID can be captured in <see cref="GameVersion.OR"/>.
    /// </summary>
    public static ReadOnlySpan<byte> CaptureFlagsOR =>
    [
        0x00, 0x40, 0x1A, 0x0A, 0xA8, 0x5E, 0x66, 0x85, 0x04, 0xAF, 0xD2, 0x81, 0x36, 0xA0, 0xF4, 0x81,
        0x36, 0x56, 0x00, 0x49, 0x00, 0x8D, 0x84, 0xC1, 0x40, 0x2F, 0x40, 0xC4, 0x0F, 0x95, 0x39, 0x94,
        0xA4, 0x17, 0xD6, 0xB9, 0x64, 0x9B, 0xBB, 0xDF, 0xBD, 0xA9, 0xFC, 0xAB, 0xEB, 0x5B, 0x4E, 0xBE,
        0x4D, 0x02, 0x14, 0x05, 0x60, 0x8A, 0x11, 0x0F, 0x08, 0x05, 0x00, 0x00, 0xF7, 0x81, 0x24, 0x04,
        0xA0, 0x38, 0x9C, 0x14, 0x94, 0xF8, 0x58, 0x55, 0x00, 0x02, 0x8D, 0x24, 0x24, 0x20, 0x3C, 0xD2,
        0x6B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x05, 0x18,
    ];
}
