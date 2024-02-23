using System;

namespace PKHeX.Core;

public partial class MemoryContext8
{
    private static ReadOnlySpan<byte> Memory_NotSWSH =>
    [
        10, // {0} got treats from {1}. {4} that {3}.
        17, // {0} battled at {1}’s side and beat {2}. {4} that {3}.
        20, // {0} surfed across the water, carrying {1} on its back. {4} that {3}.
        21, // {0} saw {2} carrying {1} on its back. {4} that {3}.
        23, // When {1} challenged the Battle Maison, {0} got really nervous. {4} that {3}.
        24, // {0} flew, carrying {1} on its back, to {2}. {4} that {3}.
        26, // {0} saw {1} using {2}. {4} that {3}.
        31, // {0} searched for hidden items with {1} using the Dowsing Machine {2}. {4} that {3}.
        34, // {0} planted {2} with {1} and imagined a big harvest. {4} that {3}.
        35, // {0} proudly used Strength at {1}’s instruction in... {2}. {4} that {3}.
        36, // {0} proudly used Cut at {1}’s instruction in... {2}. {4} that {3}.
        37, // {0} shattered rocks to its heart’s content at {1}’s instruction in... {2}. {4} that {3}.
        38, // {0} used Waterfall while carrying {1} on its back in... {2}. {4} that {3}.
        41, // {0} headed for Victory Road with {1}. {4} that {3}.
        44, // {0} encountered {2} with {1} using the Poké Radar. {4} that {3}.
        45, // When {2} jumped out, {0} was surprised and ran away with {1}. {4} that {3}.
        46, // {0} got a high score at the Battle Institute where it challenged others with {1}. {4} that {3}.
        47, // {0} was stared at by the Judge when it met him with {1}. {4} that {3}.
        50, // {0} was taken to the Pokémon Day Care by {1} and left with {2}. {4} that {3}.
        52, // {0} was there when {1} used a repellent {2}. {4} that {3}.
        54, // {0} took an elevator with {1}. {4} that {3}.
        58, // {0} was taken to a nice lady by {1} and pampered. {4} that {3}.
        59, // {0} checked a bin with {1} {2}. {4} that {3}.
        61, // {0} went to a tall tower with {1} and looked down on the world. {4} that {3}.
        62, // {0} saw itself in a mirror inside a mirror cave that it explored with {1}. {4} that {3}.
        64, // {0} went to a factory with {1} and saw a lot of machines that looked very complicated. {4} that {3}.
        65, // {0} was there when {1} created a Secret Base. {4} that {3}.
        66, // {0} participated in a contest with {1} and impressed many people. {4} that {3}.
        67, // {0} participated in a contest with {1} and won the title. {4} that {3}.
        68, // {0} soared through the sky with {1} and went to many different places. {4} that {3}.
        69, // {1} asked {0} to dive. Down it went, deep into the ocean, to explore the bottom of the sea. {4} that {3}.
        87, // {0} got in a fight with the {2} that it was in a Box with about {1}. {4} that {3}.
        88, // When {0} was in a Box, it thought about the reason why {1} had it hold the {2}. {4} that {3}.
        89, // When {0} was in a Box, it had a weird dream in which {1} was using the move {2}. {4} that {3}.
    ];

    private static ReadOnlySpan<ushort> KeyItemMemoryArgsAnySpecies =>
    [
        628, 629, // DNA Splicers
        943, 944, 945, 946, // N-Lunarizer / N-Solarizer
        1278, // Rotom Catalog
        1590, 1591, // Reigns of Unity
    ];

    private static bool IsKeyItemMemoryArgValid(ushort species, ushort arg) => species switch
    {
        (int)Species.Rotom => arg == 1278, // Rotom Catalog
        (int)Species.Kyurem => arg is 628 or 629, // DNA Splicers
        (int)Species.Necrozma => arg is 943 or 944 or 945 or 946, // N-Lunarizer / N-Solarizer
        (int)Species.Calyrex => arg is 1590 or 1591, // Reigns of Unity
        _ => false,
    };

    /// <summary>
    /// Includes all purchase-able items except for TRs (separate span).
    /// </summary>
    private static ReadOnlySpan<ushort> PurchaseItemsNoTR =>
    [
        0002, 0003, 0004, 0006, 0007, 0008, 0009, 0010, 0011, 0013,
        0014, 0015, 0017, 0018, 0019, 0020, 0021, 0022, 0023, 0024,
        0025, 0026, 0027, 0028, 0033, 0034, 0035, 0036, 0037, 0042,
        0045, 0046, 0047, 0048, 0049, 0050, 0051, 0052, 0055, 0056,
        0057, 0058, 0059, 0060, 0061, 0062, 0063, 0076, 0077, 0079,
        0089, 0149, 0150, 0151, 0155, 0214, 0215, 0219, 0220, 0254,
        0255, 0269, 0270, 0271, 0275, 0280, 0287, 0289, 0290, 0291,
        0292, 0293, 0294, 0297, 0314, 0315, 0316, 0317, 0318, 0319,
        0320, 0321, 0325, 0326, 0541, 0542, 0545, 0546, 0547, 0639,
        0640, 0645, 0646, 0647, 0648, 0649, 0795, 0846, 0879, 1084,
        1087, 1088, 1089, 1090, 1091, 1094, 1095, 1097, 1098, 1099,
        1118, 1119, 1121, 1122, 1231, 1232, 1233, 1234, 1235, 1236,
        1237, 1238, 1239, 1240, 1241, 1242, 1243, 1244, 1245, 1246,
        1247, 1248, 1249, 1250, 1251, 1252, 1256, 1257, 1258, 1259,
        1260, 1261, 1262, 1263,
    ];

    private static ReadOnlySpan<byte> LotoPrizeSWSH => [0001, 0033, 0050, 0051, 0053];

    // {met, values allowed}
    private static ReadOnlySpan<byte> MultiLoc006 => [ 1, 2, 3 ]; // town of Postwick: first town, at home, friend's house
    private static ReadOnlySpan<byte> MultiLoc014 => [ 4, 6, 8, 9 ]; // town of Wedgehurst: someone's house, boutique, simple town, Pokémon Center
    private static ReadOnlySpan<byte> MultiLoc018 => [ 16, 44, 71 ]; // Route 2: lab, tranquil road, lakeside road
    private static ReadOnlySpan<byte> MultiLoc020 => [ 6, 9, 11, 20, 24, 30, 35 ]; // city of Motostoke: boutique, Pokémon Center, hotel, Pokémon Gym, stylish café, hair salon, town with a mysterious air
    private static ReadOnlySpan<byte> MultiLoc024 => [ 20, 73 ];// Motostoke Stadium: Pokémon Gym, stadium
    private static ReadOnlySpan<byte> MultiLoc034 => [ 9, 12 ]; // town of Turffield: Pokémon Center, town with a mysterious air
    private static ReadOnlySpan<byte> MultiLoc040 => [ 44, 74 ]; // Route 5: tranquil road, Pokémon Nursery
    private static ReadOnlySpan<byte> MultiLoc044 => [ 4, 9, 31, 33 ]; // town of Hulbury: someone’s house, Pokémon Center, restaurant, seaside town
    private static ReadOnlySpan<byte> MultiLoc056 => [ 4, 6, 9, 20, 24, 30, 35 ]; // city of Hammerlocke: someone’s house, boutique, Pokémon Center, Pokémon Gym, stylish café, hair salon, town with a mysterious air
    private static ReadOnlySpan<byte> MultiLoc070 => [ 4, 9, 76 ]; // town of Stow-on-Side: someone’s house, Pokémon Center, town in the mountains
    private static ReadOnlySpan<byte> MultiLoc078 => [ 4, 9, 12 ]; // town of Ballonlea: someone’s house, Pokémon Center, town with a mysterious air
    private static ReadOnlySpan<byte> MultiLoc096 => [ 4, 6, 9, 11, 30, 31, 37 ]; // town of Circhester: someone’s house, boutique, Pokémon Center, hotel, hair salon, restaurant, snowcapped town
    private static ReadOnlySpan<byte> MultiLoc102 => [ 9, 77 ]; // town of Spikemuth: Pokémon Center, run-down town
    private static ReadOnlySpan<byte> MultiLoc110 => [ 4, 6, 9, 11, 22, 24, 30 ]; // city of Wyndon: someone’s house, boutique, Pokémon Center, hotel, large town, stylish café, hair salon
    private static ReadOnlySpan<byte> MultiLoc206 => [ 04, 37 ]; // town of Freezington: someone’s house, snowcapped town
    private static ReadOnlySpan<byte> MultiLoc220 => [ 53, 65 ];// at the Crown Shrine: on a snow-swept road, in a mystical place

    private static bool IsMultiGenLocArea(ushort location, out ReadOnlySpan<byte> expect)
    {
        expect = GetMultiGenLocAreaList(location);
        return expect.Length != 0;
    }

    private static ReadOnlySpan<byte> GetMultiGenLocAreaList(ushort location) => location switch
    {
        006 => MultiLoc006,
        014 => MultiLoc014,
        018 => MultiLoc018,
        020 => MultiLoc020,
        024 => MultiLoc024,
        034 => MultiLoc034,
        040 => MultiLoc040,
        044 => MultiLoc044,
        056 => MultiLoc056,
        070 => MultiLoc070,
        078 => MultiLoc078,
        096 => MultiLoc096,
        102 => MultiLoc102,
        110 => MultiLoc110,
        206 => MultiLoc206,
        220 => MultiLoc220,
        _ => [],
    };

    private static bool IsSingleGenLocArea(ushort location, out byte expect)
    {
        var arr = SingleGenLocAreas;
        if ((location & 1) != 0 || (uint)(location >>= 1) >= arr.Length)
        {
            expect = 0;
            return false;
        }
        expect = arr[location];
        return expect != 0;
    }

    // {met, value allowed}
    private static ReadOnlySpan<byte> SingleGenLocAreas =>
    [
        // {008, 41}, // Slumbering Weald, forest
        // {012, 44}, // Route 1, tranquil road
        // {016, 28}, // Wedgehurst Station, train station
        // {022, 28}, // Motostoke Station, train station
        // {028, 44}, // Route 3, tranquil road
        // {030, 75}, // Galar Mine, mine
        // {032, 44}, // Route 4, tranquil road
        // {036, 73}, // Turffield Stadium, stadium
        // {046, 28}, // Hulbury Station, train station
        // {048, 73}, // Hulbury Stadium, stadium
        // {052, 35}, // Motostoke Outskirts, town with a mysterious air
        // {054, 75}, // Galar Mine No. 2, mine
        // {058, 28}, // Hammerlocke Station, train station
        // {060, 73}, // Hammerlocke Stadium, stadium
        // {064, 79}, // Energy Plant, dangerous place
        // {066, 79}, // the tower summit, dangerous place
        // {068, 47}, // Route 6, rugged mountain pass
        // {072, 73}, // Stow-on-Side Stadium, stadium
        // {076, 41}, // Glimwood Tangle, forest
        // {080, 73}, // Ballonlea Stadium, stadium
        // {084, 44}, // Route 7, tranquil road
        // {086, 47}, // Route 8, rugged mountain pass
        // {088, 53}, // Route 8 (on Steamdrift Way), snow-swept road
        // {090, 53}, // Route 9, snow-swept road
        // {092, 53}, // Route 9 (in Circhester Bay), snow-swept road
        // {094, 53}, // Route 9 (in Outer Spikemuth), snow-swept road
        // {098, 73}, // Circhester Stadium, stadium
        // {104, 78}, // Route 9 Tunnel, tunnel
        // {106, 53}, // Route 10, snow-swept road
        // {108, 28}, // White Hill Station, train station
        // {112, 28}, // Wyndon Station, train station
        // {114, 38}, // Wyndon Stadium (at the Pokémon League HQ), Pokémon League
        // {116, 38}, // Wyndon Stadium (in a locker room), Pokémon League
        // {120, 44}, // Meetup Spot, tranquil road
        // {122, 72}, // Rolling Fields, vast field
        // {124, 72}, // Dappled Grove, vast field
        // {126, 72}, // Watchtower Ruins, vast field
        // {128, 72}, // East Lake Axewell, vast field
        // {130, 72}, // West Lake Axewell, vast field
        // {132, 72}, // Axew’s Eye, vast field
        // {134, 72}, // South Lake Miloch, vast field
        // {136, 72}, // near the Giant’s Seat, vast field
        // {138, 72}, // North Lake Miloch, vast field
        // {140, 72}, // Motostoke Riverbank, vast field
        // {142, 72}, // Bridge Field, vast field
        // {144, 72}, // Stony Wilderness, vast field
        // {146, 72}, // Dusty Bowl, vast field
        // {148, 72}, // around the Giant’s Mirror, vast field
        // {150, 72}, // the Hammerlocke Hills, vast field
        // {152, 72}, // near the Giant’s Cap, vast field
        // {154, 72}, // Lake of Outrage, vast field
        // {156, 28}, // Wild Area Station, train station
        // {158, 34}, // Battle Tower, tall building
        // {160, 34}, // Rose Tower, tall building
        // {164, 72}, // Fields of Honor, vast field
        // {166, 50}, // Soothing Wetlands, muddy road
        // {168, 41}, // Forest of Focus, forest
        // {170, 49}, // Challenge Beach, seaside road
        // {172, 40}, // Brawlers’ Cave, cave
        // {174, 76}, // Challenge Road, town in the mountains
        // {176, 40}, // Courageous Cavern, cave
        // {178, 49}, // Loop Lagoon, seaside road
        // {180, 72}, // Training Lowlands, vast field
        // {182, 40}, // Warm-Up Tunnel, cave
        // {184, 51}, // Potbottom Desert, sand-swept road
        // {186, 49}, // Workout Sea, seaside road
        // {188, 49}, // Stepping-Stone Sea, seaside road
        // {190, 49}, // Insular Sea, seaside road
        // {192, 49}, // Honeycalm Sea, seaside road
        // {194, 49}, // Honeycalm Island, seaside road
        // {196, 29}, // Master Dojo, battling spot
        // {198, 34}, // Tower of Darkness, tall building
        // {200, 34}, // Tower of Waters, tall building
        // {202, 28}, // Armor Station, train station
        // {204, 53}, // Slippery Slope, snow-swept road
        // {208, 53}, // Frostpoint Field, snow-swept road
        // {210, 72}, // Giant’s Bed, vast field
        // {212, 48}, // Old Cemetery, stone-lined area
        // {214, 53}, // Snowslide Slope, snow-swept road
        // {216, 40}, // Tunnel to the Top, cave
        // {218, 53}, // the Path to the Peak, snow-swept road
        // {222, 44}, // Giant’s Foot, tranquil road
        // {224, 40}, // Roaring-Sea Caves, cave
        // {226, 49}, // Frigid Sea, seaside road
        // {228, 72}, // Three-Point Pass, vast field
        // {230, 72}, // Ballimere Lake, vast field
        // {232, 40}, // Lakeside Cave, cave
        // {234, 72}, // Dyna Tree Hill, vast field
        // {236, 65}, // Rock Peak Ruins, mystical place
        // {238, 65}, // Iceberg Ruins, mystical place
        // {240, 65}, // Iron Ruins, mystical place
        // {242, 65}, // Split-Decision Ruins, mystical place
        // {244, 40}, // Max Lair, cave
        // {246, 28}, // Crown Tundra Station, train station
        00, 00, 00, 00, 41, 00, 44, 00, 28, 00,
        00, 28, 00, 00, 44, 75, 44, 00, 73, 00,
        00, 00, 00, 28, 73, 00, 35, 75, 00, 28,
        73, 00, 79, 79, 47, 00, 73, 00, 41, 00,
        73, 00, 44, 47, 53, 53, 53, 53, 00, 73,
        00, 00, 78, 53, 28, 00, 28, 38, 38, 00,
        44, 72, 72, 72, 72, 72, 72, 72, 72, 72,
        72, 72, 72, 72, 72, 72, 72, 72, 28, 34,
        34, 00, 72, 50, 41, 49, 40, 76, 40, 49,
        72, 40, 51, 49, 49, 49, 49, 49, 29, 34,
        34, 28, 53, 00, 53, 72, 48, 53, 40, 53,
        00, 44, 40, 49, 72, 72, 40, 72, 65, 65,
        65, 65, 40, 28,
    ];

    public static bool IsGeneralLocation8(byte location) => PossibleGeneralLocations8.BinarySearch(location) >= 0;

    private static ReadOnlySpan<byte> PossibleGeneralLocations8 =>
    [
        01, 02, 03, 04, 06, 08, 09,
        11, 12, 16,
        20, 22, 24, 28, 29,
        30, 31, 33, 34, 35, 37, 38,
        40, 41, 44, 47, 48, 49,
        50, 51, 53,
        65,
        71, 72, 73, 74, 75, 76, 77, 78, 79,
    ];

    private static ReadOnlySpan<byte> MemoryMinIntensity =>
    [
        0, 1, 1, 1, 1, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 3, 3, 3, 3, 4, 4,
        3, 3, 3, 3, 3, 3, 3, 4, 5, 5,
        5, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        3, 3, 1, 3, 2, 2, 4, 3, 4, 4,
        4, 4, 2, 4, 2, 4, 3, 3, 4, 2,
        3, 3, 3, 3, 3, 2, 3, 4, 4, 2, // same as Gen6
        2, 3, 3, 3, 3, 4, 5, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
    ];

#if FALSE
    // [0,99]+1 >= CHANCE -> abort
    // this silly +1 causes:
    // 100% memories to fail 1% of the time
    // 1% memories to fail 100% of the time (!!!)
    // SW/SH v1.3 sub_71013B0C40
    private static ReadOnlySpan<byte> MemoryRandChance =>
    [
        000, 100, 100, 100, 100, 005, 005, 005, 005, 005,
        005, 005, 005, 005, 010, 020, 010, 001, 050, 030,
        005, 005, 020, 005, 005, 005, 001, 050, 100, 050,
        050, 002, 002, 005, 005, 005, 005, 005, 005, 002,
        020, 020, 005, 010, 001, 001, 050, 030, 020, 020,
        010, 010, 001, 010, 001, 050, 030, 030, 030, 002,
        050, 020, 020, 020, 020, 010, 010, 050, 020, 005, // same as Gen6
        005, 010, 010, 020, 020, 010, 100, 010, 005, 010,
        010, 010, 010, 010, 010, 010, 010, 001, 001, 001,
    ];
#endif

    /// <summary>
    /// 24bits of flags allowing certain feelings for a given memory index.
    /// </summary>
    /// <remarks>Beware, there was an off-by-one error in the game that made Feeling 0 unobtainable, and thus the Happy feeling bit (rightmost) is omitted.</remarks>
    private static ReadOnlySpan<int> MemoryFeelings =>
    [
        0x000000, 0x04CBFD, 0x004BFD, 0x04CBFD, 0x04CBFD, 0xFFFBFB, 0x84FFF9, 0x47FFFF, 0xBF7FFA, 0x7660B0,
        0x80BDF9, 0x88FB7A, 0x083F79, 0x0001FE, 0xCFEFFF, 0x84EBAF, 0xB368B0, 0x091F7E, 0x0320A0, 0x080DDD,
        0x081A7B, 0x404030, 0x0FFFFF, 0x9A08BC, 0x089A7B, 0x0032AA, 0x80FF7A, 0x0FFFFF, 0x0805FD, 0x098278,
        0x0B3FFF, 0x8BBFFA, 0x8BBFFE, 0x81A97C, 0x8BB97C, 0x8BBF7F, 0x8BBF7F, 0x8BBF7F, 0x8BBF7F, 0xAC3ABE,
        0xBFFFFF, 0x8B837C, 0x848AFA, 0x88FFFE, 0x8B0B7C, 0xB76AB2, 0x8B1FFF, 0xBE7AB8, 0xB77EB8, 0x8C9FFD,
        0xBF9BFF, 0xF408B0, 0xBCFE7A, 0x8F3F72, 0x90DB7A, 0xBCEBFF, 0xBC5838, 0x9C3FFE, 0x9CFFFF, 0x96D83A,
        0xB770B0, 0x881F7A, 0x839F7A, 0x839F7A, 0x839F7A, 0x53897F, 0x41BB6F, 0x0C35FF, 0x8BBF7F, 0x8BBF7F, // same as Gen6
        0x90CC7E, 0x2FBF7F, 0x2FBF7F, 0xB797FF, 0x3FB7FF, 0xBFFFFF, 0xCC8BFF, 0xF69F7F, 0x37FDFF, 0x2B277F,
        0x8FFBFA, 0x8CDFFA, 0xFCE9EF, 0x8F6F7B, 0x826AB0, 0x866AB0, 0x8C69FE, 0x776AB0, 0x8CFB7A, 0x0CFEBA,
    ];

    /// <summary>
    /// Bitflag array with set bits indicating if the species ID can be captured in <see cref="GameVersion.SW"/>.
    /// </summary>
    public static ReadOnlySpan<byte> CaptureFlagsSW =>
    [
        0xFE, 0x1F, 0x00, 0xFE, 0xFF, 0x3F, 0xFC, 0xFC, 0x1F, 0xE3, 0x0F, 0xFC, 0xCC, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0x7F, 0x00, 0x18, 0xFE, 0xC7, 0x07, 0xBC, 0x44, 0xF9, 0xF0, 0x4F, 0xF0, 0xFF, 0x67,
        0x9B, 0xC1, 0xCF, 0x07, 0xFC, 0xC4, 0x67, 0xC8, 0x13, 0x67, 0xFE, 0x7F, 0x98, 0x3F, 0xFA, 0xFF,
        0x01, 0x00, 0xF8, 0x80, 0xF1, 0x1E, 0xFC, 0xFD, 0x7F, 0xFC, 0xDF, 0xEA, 0xFF, 0x01, 0x00, 0x7C,
        0xE0, 0xF3, 0xFF, 0x8F, 0xFF, 0xFF, 0xFF, 0xFF, 0xCF, 0xF9, 0xFB, 0xE3, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0x00, 0xF8, 0x00, 0xEC, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x24, 0x01, 0xC7, 0xFF, 0xFF, 0xFF,
        0x1F, 0xF7, 0xBF, 0xFF, 0x69, 0x24, 0xF9, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xCD,
        0x07,
    ];

    /// <summary>
    /// Bitflag array with set bits indicating if the species ID can be captured in <see cref="GameVersion.SH"/>.
    /// </summary>
    public static ReadOnlySpan<byte> CaptureFlagsSH =>
    [
        0xFE, 0x1F, 0x00, 0xFE, 0xFF, 0x3F, 0xFC, 0xFC, 0x1F, 0xE3, 0x0F, 0xFC, 0xCC, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0x7F, 0x00, 0x18, 0xFE, 0xC7, 0x07, 0xBC, 0x44, 0xF9, 0xF0, 0x4F, 0xF0, 0xFF, 0x67,
        0x9B, 0xC1, 0xCF, 0x07, 0xFC, 0xC4, 0x67, 0xC8, 0x13, 0x67, 0xFE, 0x7F, 0x98, 0x3F, 0xFA, 0xFF,
        0x01, 0x00, 0xF8, 0x80, 0xF1, 0x1E, 0xFC, 0xFD, 0x7F, 0xFC, 0xDF, 0xEA, 0xFF, 0x01, 0x00, 0x7C,
        0xE0, 0xF3, 0xFF, 0x8F, 0xFF, 0xFF, 0xFF, 0xFF, 0xCF, 0xF9, 0xFB, 0xE3, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0x00, 0xF8, 0x00, 0xEC, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x24, 0x01, 0xC7, 0xFF, 0xFF, 0xFF,
        0x1F, 0xF7, 0xBF, 0xFF, 0x69, 0x24, 0xF9, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xCE,
        0x07,
    ];
}
