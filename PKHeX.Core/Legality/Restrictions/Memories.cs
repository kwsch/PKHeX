using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 Memory parameters &amp; validation
    /// </summary>
    public static class Memories
    {
        internal const int MAX_MEMORY_ID_XY = 64;
        internal const int MAX_MEMORY_ID_AO = 69;
        internal const int MAX_MEMORY_ID_SWSH = 89;

        #region Tables
        internal static readonly int[] Memory_NotXY =
        {
            65, // {0} was with {1} when (he/she) built a Secret Base. {4} that {3}.
            66, // {0} participated in a contest with {1} and impressed many people. {4} that {3}.
            67, // {0} participated in a contest with {1} and won the title. {4} that {3}.
            68, // {0} soared through the sky with {1} and went to many different places. {4} that {3}.
            69, // {1} asked {0} to dive. Down it went, deep into the ocean, to explore the bottom of the sea. {4} that {3}.
        };

        internal static readonly int[] Memory_NotAO =
        {
            11, // {0} went clothes shopping with {1}. {4} that {3}.
            43, // {0} was impressed by the speed of the train it took with {1}. {4} that {3}.
            44, // {0} encountered {2} with {1} using the Poké Radar. {4} that {3}.
            56, // {0} was with {1} when (he/she) went to a boutique and tried on clothes, but (he/she) left the boutique without buying anything. {4} that {3}.
            57, // {0} went to a nice restaurant with {1} and ate until it got totally full. {4} that {3}.
            62, // {0} saw itself in a mirror in a mirror cave that it went to with {1}. {4} that {3}.
        };

        internal static readonly int[] Memory_NotSWSH =
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

        internal static readonly int[][] MoveSpecificMemories =
        {
            new[] {
                20, // {0} surfed across the water, carrying {1} on its back. {4} that {3}.
                24, // {0} flew, carrying {1} on its back, to {2}. {4} that {3}.
                35, // {0} proudly used Strength at {1}’s instruction in... {2}. {4} that {3}.
                36, // {0} proudly used Cut at {1}’s instruction in... {2}. {4} that {3}.
                37, // {0} shattered rocks to its heart’s content at {1}’s instruction in... {2}. {4} that {3}.
                38, // {0} used Waterfall while carrying {1} on its back in... {2}. {4} that {3}.
                69, // {1} asked {0} to dive. Down it went, deep into the ocean, to explore the bottom of the sea. {4} that {3}.
            },
            new[] { 57, 19, 70, 15, 249, 127, 291}, // Move IDs
        };

        internal static readonly int[] LocationsWithPokeCenter_XY =
        {
            // Kalos locations with a PKMN CENTER
            018, // Santalune City
            022, // Lumiose City
            030, // Camphrier Town
            040, // Cyllage City
            044, // Ambrette Town
            052, // Geosenge Towny
            058, // Shalour City
            064, // Coumarine City
            070, // Laverre City
            076, // Dendemille Town
            086, // Anistar City
            090, // Couriway Town
            094, // Snowbelle City
            106, // Pokémon League (X/Y)
        };

        internal static readonly int[] LocationsWithPokeCenter_AO =
        {
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
        };

        private static ICollection<int> GetPokeCenterLocations(GameVersion game)
        {
            return GameVersion.XY.Contains(game) ? LocationsWithPokeCenter_XY : LocationsWithPokeCenter_AO;
        }

        public static bool GetHasPokeCenterLocation(GameVersion game, int loc)
        {
            if (game == GameVersion.Gen6)
                return GetHasPokeCenterLocation(GameVersion.X, loc) || GetHasPokeCenterLocation(GameVersion.AS, loc);
            return GetPokeCenterLocations(game).Contains(loc);
        }

        private static readonly byte[] MemoryMinIntensity =
        {
            0, 1, 1, 1, 1, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 3, 3, 3, 3, 4, 4,
            3, 3, 3, 3, 3, 3, 3, 4, 5, 5,
            5, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            3, 3, 1, 3, 2, 2, 4, 3, 4, 4,
            4, 4, 2, 4, 2, 4, 3, 3, 4, 2,
            3, 3, 3, 3, 3, 2, 3, 4, 4, 2,
        };

        public static int GetMemoryRarity(int memory) => (uint) memory >= MemoryRandChance.Length ? -1 : MemoryRandChance[memory];

        private static readonly byte[] MemoryRandChance =
        {
            000, 100, 100, 100, 100, 005, 005, 005, 005, 005,
            005, 005, 005, 005, 010, 020, 010, 001, 050, 030,
            005, 005, 020, 005, 005, 005, 001, 050, 100, 050,
            050, 002, 002, 005, 005, 005, 005, 005, 005, 002,
            020, 020, 005, 010, 001, 001, 050, 030, 020, 020,
            010, 010, 001, 010, 001, 050, 030, 030, 030, 002,
            050, 020, 020, 020, 020, 010, 010, 050, 020, 005,
        };

        /// <summary>
        /// 24bits of flags allowing certain feelings for a given memory index.
        /// </summary>
        private static readonly uint[] MemoryFeelings =
        {
            0x000000, 0x04CBFD, 0x004BFD, 0x04CBFD, 0x04CBFD, 0xFFFBFB, 0x84FFF9, 0x47FFFF, 0xBF7FFA, 0x7660B0,
            0x80BDF9, 0x88FB7A, 0x083F79, 0x0001FE, 0xCFEFFF, 0x84EBAF, 0xB368B0, 0x091F7E, 0x0320A0, 0x080DDD,
            0x081A7B, 0x404030, 0x0FFFFF, 0x9A08BC, 0x089A7B, 0x0032AA, 0x80FF7A, 0x0FFFFF, 0x0805FD, 0x098278,
            0x0B3FFF, 0x8BBFFA, 0x8BBFFE, 0x81A97C, 0x8BB97C, 0x8BBF7F, 0x8BBF7F, 0x8BBF7F, 0x8BBF7F, 0xAC3ABE,
            0xBFFFFF, 0x8B837C, 0x848AFA, 0x88FFFE, 0x8B0B7C, 0xB76AB2, 0x8B1FFF, 0xBE7AB8, 0xB77EB8, 0x8C9FFD,
            0xBF9BFF, 0xF408B0, 0xBCFE7A, 0x8F3F72, 0x90DB7A, 0xBCEBFF, 0xBC5838, 0x9C3FFE, 0x9CFFFF, 0x96D83A,
            0xB770B0, 0x881F7A, 0x839F7A, 0x839F7A, 0x839F7A, 0x53897F, 0x41BB6F, 0x0C35FF, 0x8BBF7F, 0x8BBF7F,
        };

        #endregion

        #region Tables for Gen8

        public static bool IsInvalidGenLoc8(int memory, int loc, int egg, int variable)
        {
            if (variable > 255)
                return true;
            var arg = (byte)variable;
            if (loc > 255) // gift
                return memory != 3 || !PossibleGeneralLocations.Contains(arg);
            if (memory == 2 && egg == 0)
                return true;
            if (loc is Encounters8Nest.SharedNest)
                return !PossibleGeneralLocations.Contains(arg) || arg is 79; // dangerous place - all locations are Y-Comm locked
            if (SingleGenLocAreas.TryGetValue((byte)loc, out var val))
                return arg != val;
            if (MultiGenLocAreas.TryGetValue((byte)loc, out var arr))
                return !arr.Contains(arg);
            return false;
        }

        public static bool IsInvalidGenLoc8Other(int memory, int variable)
        {
            if (variable > byte.MaxValue)
                return true;
            var arg = (byte)variable;
            return memory switch
            {
                _ => !PossibleGeneralLocations.Contains(arg),
            };
        }

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

        private static readonly HashSet<byte> PossibleGeneralLocations = new()
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
        #endregion

        private static bool IsGeneralLocationMemoryMet(int memory) => memory is (1 or 2 or 3);
        internal static readonly HashSet<int> MemoryGeneral = new() { 1, 2, 3, 4, 19, 24, 31, 32, 33, 35, 36, 37, 38, 39, 42, 52, 59, 70, 86 };
        private static readonly HashSet<int> MemorySpecific = new() { 6 };
        private static readonly HashSet<int> MemoryMove = new() { 12, 16, 48, 49, 80, 81, 89 };
        private static readonly HashSet<int> MemoryItem = new() { 5, 15, 26, 34, 40, 51, 84, 88 };
        private static readonly HashSet<int> MemorySpecies = new() { 7, 9, 13, 14, 17, 21, 18, 25, 29, 44, 45, 50, 60, 70, 71, 72, 75, 82, 83, 87 };

        internal static readonly ushort[] KeyItemUsableObserve6 =
        {
            775, // Eon Flute
        };

        internal static readonly Dictionary<int, ushort[]> KeyItemMemoryArgsGen6 = new()
        {
            {(int) Species.Shaymin, new ushort[] {466}}, // Gracidea
            {(int) Species.Tornadus, new ushort[] {638}}, // Reveal Glass
            {(int) Species.Thundurus, new ushort[] {638}}, // Reveal Glass
            {(int) Species.Landorus, new ushort[] {638}}, // Reveal Glass
            {(int) Species.Kyurem, new ushort[] {628, 629}}, // DNA Splicers
            {(int) Species.Hoopa, new ushort[] {765}}, // Prison Bottle
        };

        internal static readonly Dictionary<int, ushort[]> KeyItemMemoryArgsGen8 = new()
        {
            {(int) Species.Rotom, new ushort[] {1278}}, // Rotom Catalog
            {(int) Species.Kyurem, new ushort[] {628, 629}}, // DNA Splicers
            {(int) Species.Necrozma, new ushort[] {943, 944, 945, 946}}, // N-Lunarizer / N-Solarizer
            {(int) Species.Calyrex, new ushort[] {1590, 1591}}, // Reigns of Unity
        };

        public static IEnumerable<ushort> KeyItemArgValues => KeyItemUsableObserve6.Concat(KeyItemMemoryArgsGen6.Values.Concat(KeyItemMemoryArgsGen8.Values).SelectMany(z => z)).Distinct();

        public static MemoryArgType GetMemoryArgType(int memory, int format)
        {
            if (MemoryGeneral.Contains(memory)) return MemoryArgType.GeneralLocation;
            if (MemorySpecific.Contains(memory) && format == 6) return MemoryArgType.SpecificLocation;
            if (MemoryItem.Contains(memory)) return MemoryArgType.Item;
            if (MemoryMove.Contains(memory)) return MemoryArgType.Move;
            if (MemorySpecies.Contains(memory)) return MemoryArgType.Species;

            return MemoryArgType.None;
        }

        public static bool CanHaveFeeling(int memory, int feeling)
        {
            if (memory >= MemoryFeelings.Length)
                return false;
            return (MemoryFeelings[memory] & (1 << feeling)) != 0;
        }

        public static bool CanHaveIntensity(int memory, int intensity)
        {
            if (memory >= MemoryFeelings.Length)
                return false;
            return MemoryMinIntensity[memory] <= intensity;
        }

        public static int GetRandomFeeling(int memory, int max = 24)
        {
            var bits = MemoryFeelings[memory];
            var rnd = Util.Rand;
            while (true)
            {
                int feel = rnd.Next(max);
                if ((bits & (1 << feel)) != 0)
                    return feel;
            }
        }

        public static int GetMinimumIntensity(int memory)
        {
            if (memory > MemoryMinIntensity.Length)
                return -1;
            return MemoryMinIntensity[memory];
        }

        public static IEnumerable<ushort> GetMemoryItemParams(int format)
        {
            return format switch
            {
                6 or 7 => Legal.HeldItem_AO.Distinct()
                    .Concat(KeyItemArgValues)
                    .Concat(Legal.TMHM_AO.Take(100).Select(z => (ushort)z))
                    .Where(z => z < Legal.MaxItemID_6_AO),
                8 => Legal.HeldItem_AO.Concat(Legal.HeldItems_SWSH).Distinct()
                    .Concat(KeyItemArgValues)
                    .Concat(Legal.TMHM_AO.Take(100).Select(z => (ushort)z))
                    .Where(z => z < Legal.MaxItemID_8_R2),
                _ => System.Array.Empty<ushort>(),
            };
        }
    }

    public readonly struct MemoryVariableSet
    {
        public readonly string Handler;

        public readonly int MemoryID;
        public readonly int Variable;
        public readonly int Intensity;
        public readonly int Feeling;

        private MemoryVariableSet(string handler, int m, int v, int i, int f)
        {
            Handler = handler;
            MemoryID = m;
            Variable = v;
            Intensity = i;
            Feeling = f;
        }

        public static MemoryVariableSet Read(ITrainerMemories pkm, int handler) => handler switch
        {
            0 => new MemoryVariableSet(LegalityCheckStrings.L_XOT, pkm.OT_Memory, pkm.OT_TextVar, pkm.OT_Intensity, pkm.OT_Feeling), // OT
            1 => new MemoryVariableSet(LegalityCheckStrings.L_XHT, pkm.HT_Memory, pkm.HT_TextVar, pkm.HT_Intensity, pkm.HT_Feeling), // HT
            _ => new MemoryVariableSet(LegalityCheckStrings.L_XOT, 0, 0, 0, 0),
        };

        public bool Equals(MemoryVariableSet v) => v.Handler == Handler
                                                   && v.MemoryID == MemoryID
                                                   && v.Variable == Variable
                                                   && v.Intensity == Intensity
                                                   && v.Feeling == Feeling;

        public override bool Equals(object obj) => obj is MemoryVariableSet v && Equals(v);
        public override int GetHashCode() => -1;
        public static bool operator ==(MemoryVariableSet left, MemoryVariableSet right) => left.Equals(right);
        public static bool operator !=(MemoryVariableSet left, MemoryVariableSet right) => !(left == right);
    }
}
