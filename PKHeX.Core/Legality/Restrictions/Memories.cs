using System.Collections.Generic;

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

        private static readonly HashSet<int> MemoryGeneral = new() { 1, 2, 3, 4, 19, 24, 31, 32, 33, 35, 36, 37, 38, 39, 42, 52, 59, 70, 86 };
        private static readonly HashSet<int> MemorySpecific = new() { 6 };
        private static readonly HashSet<int> MemoryMove = new() { 12, 16, 48, 49, 80, 81, 89 };
        private static readonly HashSet<int> MemoryItem = new() { 5, 15, 26, 34, 40, 51, 84, 88 };
        private static readonly HashSet<int> MemorySpecies = new() { 7, 9, 13, 14, 17, 21, 18, 25, 29, 44, 45, 50, 60, 70, 71, 72, 75, 82, 83, 87 };

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
            1 => new MemoryVariableSet(LegalityCheckStrings.L_XOT, pkm.HT_Memory, pkm.HT_TextVar, pkm.HT_Intensity, pkm.HT_Feeling), // HT
            _ => new MemoryVariableSet(LegalityCheckStrings.L_XOT, 0, 0, 0, 0)
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
