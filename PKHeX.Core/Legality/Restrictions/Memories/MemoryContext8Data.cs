using System.Collections.Generic;

namespace PKHeX.Core
{
    public partial class MemoryContext8
    {
        private static readonly int[] Memory_NotSWSH =
        {
            // There's probably more. Send a pull request!
            20, // {0} surfed across the water, carrying {1} on its back. {4} that {3}.
            24, // {0} flew, carrying {1} on its back, to {2}. {4} that {3}.
            35, // {0} proudly used Strength at {1}’s instruction in... {2}. {4} that {3}.
            36, // {0} proudly used Cut at {1}’s instruction in... {2}. {4} that {3}.
            37, // {0} shattered rocks to its heart’s content at {1}’s instruction in... {2}. {4} that {3}.
            38, // {0} used Waterfall while carrying {1} on its back in... {2}. {4} that {3}.
            69, // {1} asked {0} to dive. Down it went, deep into the ocean, to explore the bottom of the sea. {4} that {3}.
        };

        private static readonly Dictionary<int, ushort[]> KeyItemMemoryArgsGen8 = new()
        {
            {(int) Species.Rotom, new ushort[] {1278}}, // Rotom Catalog
            {(int) Species.Kyurem, new ushort[] {628, 629}}, // DNA Splicers
            {(int) Species.Necrozma, new ushort[] {943, 944, 945, 946}}, // N-Lunarizer / N-Solarizer
            {(int) Species.Calyrex, new ushort[] {1590, 1591}}, // Reigns of Unity
        };

        private static readonly HashSet<ushort> PurchaseableItemSWSH = new(Legal.TR_SWSH)
        {
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
        };

        // {met, values allowed}
        private static readonly Dictionary<byte, byte[]> MultiGenLocAreas = new()
        {
            // town of Postwick
            // first town, at home, friend's house
            {6, new byte[] {1, 2, 3}},

            // town of Wedgehurst
            // someone's house, boutique, simple town, Pokémon Center
            {14, new byte[] {4, 6, 8, 9}},

            // Route 2
            // lab, tranquil road, lakeside road
            {18, new byte[] {16, 44, 71}},

            // city of Motostoke
            // boutique, Pokémon Center, hotel, Pokémon Gym, stylish café, hair salon, town with a mysterious air
            {20, new byte[] {6, 9, 11, 20, 24, 30, 35}},

            // Motostoke Stadium
            // Pokémon Gym, stadium
            {24, new byte[] {20, 73}},

            // town of Turffield
            // Pokémon Center, town with a mysterious air
            {34, new byte[] {9, 12}},

            // Route 5
            // tranquil road, Pokémon Nursery
            {40, new byte[] {44, 74}},

            // town of Hulbury
            // someone’s house, Pokémon Center, restaurant, seaside town
            {44, new byte[] {4, 9, 31, 33}},

            // city of Hammerlocke
            // someone’s house, boutique, Pokémon Center, Pokémon Gym, stylish café, hair salon, town with a mysterious air
            {56, new byte[] {4, 6, 9, 20, 24, 30, 35}},

            // town of Stow-on-Side
            // someone’s house, Pokémon Center, town in the mountains
            {70, new byte[] {4, 9, 76}},

            // town of Ballonlea
            // someone’s house, Pokémon Center, town with a mysterious air
            {78, new byte[] {4, 9, 12}},

            // town of Circhester
            // someone’s house, boutique, Pokémon Center, hotel, hair salon, restaurant, snowcapped town
            {96, new byte[] {4, 6, 9, 11, 30, 31, 37}},

            // town of Spikemuth
            // Pokémon Center, run-down town
            {102, new byte[] {9, 77}},

            // city of Wyndon
            // someone’s house, boutique, Pokémon Center, hotel, large town, stylish café, hair salon
            {110, new byte[] {4, 6, 9, 11, 22, 24, 30}},

            // town of Freezington
            // someone’s house, snowcapped town
            {206, new byte[] {04, 37}},
        };

        // {met, value allowed}
        private static readonly Dictionary<byte, byte> SingleGenLocAreas = new()
        {
            {008, 41}, // Slumbering Weald, forest
            {012, 44}, // Route 1, tranquil road
            {016, 28}, // Wedgehurst Station, train station
            {022, 28}, // Motostoke Station, train station
            {028, 44}, // Route 3, tranquil road
            {030, 75}, // Galar Mine, mine
            {032, 44}, // Route 4, tranquil road
            {036, 73}, // Turffield Stadium, stadium
            {046, 28}, // Hulbury Station, train station
            {048, 73}, // Hulbury Stadium, stadium
            {052, 35}, // Motostoke Outskirts, town with a mysterious air
            {054, 75}, // Galar Mine No. 2, mine
            {058, 28}, // Hammerlocke Station, train station
            {060, 73}, // Hammerlocke Stadium, stadium
            {064, 79}, // Energy Plant, dangerous place
            {066, 79}, // the tower summit, dangerous place
            {068, 47}, // Route 6, rugged mountain pass
            {072, 73}, // Stow-on-Side Stadium, stadium
            {076, 41}, // Glimwood Tangle, forest
            {080, 73}, // Ballonlea Stadium, stadium
            {084, 44}, // Route 7, tranquil road
            {086, 47}, // Route 8, rugged mountain pass
            {088, 53}, // Route 8 (on Steamdrift Way), snow-swept road
            {090, 53}, // Route 9, snow-swept road
            {092, 53}, // Route 9 (in Circhester Bay), snow-swept road
            {094, 53}, // Route 9 (in Outer Spikemuth), snow-swept road
            {098, 73}, // Circhester Stadium, stadium
            {104, 78}, // Route 9 Tunnel, tunnel
            {106, 53}, // Route 10, snow-swept road
            {108, 28}, // White Hill Station, train station
            {112, 28}, // Wyndon Station, train station
            {114, 38}, // Wyndon Stadium (at the Pokémon League HQ), Pokémon League
            {116, 38}, // Wyndon Stadium (in a locker room), Pokémon League
            {120, 44}, // Meetup Spot, tranquil road
            {122, 72}, // Rolling Fields, vast field
            {124, 72}, // Dappled Grove, vast field
            {126, 72}, // Watchtower Ruins, vast field
            {128, 72}, // East Lake Axewell, vast field
            {130, 72}, // West Lake Axewell, vast field
            {132, 72}, // Axew’s Eye, vast field
            {134, 72}, // South Lake Miloch, vast field
            {136, 72}, // near the Giant’s Seat, vast field
            {138, 72}, // North Lake Miloch, vast field
            {140, 72}, // Motostoke Riverbank, vast field
            {142, 72}, // Bridge Field, vast field
            {144, 72}, // Stony Wilderness, vast field
            {146, 72}, // Dusty Bowl, vast field
            {148, 72}, // around the Giant’s Mirror, vast field
            {150, 72}, // the Hammerlocke Hills, vast field
            {152, 72}, // near the Giant’s Cap, vast field
            {154, 72}, // Lake of Outrage, vast field
            {156, 28}, // Wild Area Station, train station
            {158, 34}, // Battle Tower, tall building
            {160, 34}, // Rose Tower, tall building
            {164, 72}, // Fields of Honor, vast field
            {166, 50}, // Soothing Wetlands, muddy road
            {168, 41}, // Forest of Focus, forest
            {170, 49}, // Challenge Beach, seaside road
            {172, 40}, // Brawlers’ Cave, cave
            {174, 76}, // Challenge Road, town in the mountains
            {176, 40}, // Courageous Cavern, cave
            {178, 49}, // Loop Lagoon, seaside road
            {180, 72}, // Training Lowlands, vast field
            {182, 40}, // Warm-Up Tunnel, cave
            {184, 51}, // Potbottom Desert, sand-swept road
            {186, 49}, // Workout Sea, seaside road
            {188, 49}, // Stepping-Stone Sea, seaside road
            {190, 49}, // Insular Sea, seaside road
            {192, 49}, // Honeycalm Sea, seaside road
            {194, 49}, // Honeycalm Island, seaside road
            {196, 29}, // Master Dojo, battling spot
            {198, 34}, // Tower of Darkness, tall building
            {200, 34}, // Tower of Waters, tall building
            {202, 28}, // Armor Station, train station
            {204, 53}, // Slippery Slope, snow-swept road
            {208, 53}, // Frostpoint Field, snow-swept road
            {210, 72}, // Giant’s Bed, vast field
            {212, 48}, // Old Cemetery, stone-lined area
            {214, 53}, // Snowslide Slope, snow-swept road
            {216, 40}, // Tunnel to the Top, cave
            {218, 53}, // the Path to the Peak, snow-swept road
            {220, 65}, // Crown Shrine, mystical place
            {222, 44}, // Giant’s Foot, tranquil road
            {224, 40}, // Roaring-Sea Caves, cave
            {226, 49}, // Frigid Sea, seaside road
            {228, 72}, // Three-Point Pass, vast field
            {230, 72}, // Ballimere Lake, vast field
            {232, 40}, // Lakeside Cave, cave
            {234, 72}, // Dyna Tree Hill, vast field
            {236, 65}, // Rock Peak Ruins, mystical place
            {238, 65}, // Iceberg Ruins, mystical place
            {240, 65}, // Iron Ruins, mystical place
            {242, 65}, // Split-Decision Ruins, mystical place
            {244, 40}, // Max Lair, cave
            {246, 28}, // Crown Tundra Station, train station
        };

        private static readonly HashSet<byte> PossibleGeneralLocations8 = new()
        {
            01, 02, 03, 04, 06, 08, 09,
            11, 12, 16,
            20, 22, 24, 28, 29,
            30, 31, 33, 34, 35, 37, 38,
            40, 41, 44, 47, 48, 49,
            50, 51, 53,
            65,
            71, 72, 73, 74, 75, 76, 77, 78, 79,
        };
    }
}
