using System.Collections.Generic;

namespace PKHeX.Core
{
    public static class Memories
    {
        #region Tables
        internal static readonly int[] MemoryItems =
        {
            1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,
            50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,
            100,101,102,103,104,105,106,107,108,109,110,111,112,116,117,118,119,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,
            150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,
            200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,
            250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,
            300,301,302,303,304,305,306,307,308,309,310,311,312,313,314,315,316,317,318,319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,346,347,348,349,
            350,351,352,353,354,355,356,357,358,359,360,361,362,363,364,365,366,367,368,369,370,371,372,373,374,375,376,377,378,379,380,381,382,383,384,385,386,387,388,389,390,391,392,393,394,395,396,397,398,399,
            400,401,402,403,404,405,406,407,408,409,410,411,412,413,414,415,416,417,418,419,420,421,422,423,424,425,428,429,430,431,432,433,434,435,436,437,438,439,440,441,442,443,444,445,446,447,448,449,
            450,451,452,453,454,455,456,457,458,459,460,461,462,463,464,465,466,467,468,469,470,471,472,473,474,475,476,477,478,479,480,481,482,483,484,485,486,487,488,489,490,491,492,493,494,495,496,497,498,499,
            500,501,502,503,504,505,506,507,508,509,510,511,512,513,514,515,516,517,518,519,520,521,522,523,524,525,526,527,528,529,530,531,532,533,534,535,536,537,538,539,540,541,542,543,544,545,546,547,548,549,
            550,551,552,553,554,555,556,557,558,559,560,561,562,563,564,565,566,567,568,569,570,571,572,573,574,575,576,577,578,579,580,581,582,583,584,585,586,587,588,589,590,591,592,593,594,595,596,597,598,599,
            600,601,602,603,604,605,606,607,608,609,610,611,612,613,614,615,616,617,618,619,620,621,623,624,625,626,627,628,629,630,631,632,633,634,635,636,637,638,639,640,641,642,643,644,645,646,647,648,649,
            650,651,652,653,654,655,656,657,658,659,660,661,662,663,664,665,666,667,668,669,670,671,672,673,674,675,676,677,678,679,680,681,682,683,684,685,686,687,688,689,690,691,692,693,694,695,696,697,698,699,
            700,701,702,703,704,705,706,707,708,709,710,711,712,713,714,715,716,717,
            /* ORAS */
            718,719,720,737,738,739,740,741,742,752,753,754,755,756,757,758,759,760,761,762,763,764,765,767,768, 769,770,775
        };

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

        internal static readonly int[] LocationsWithPKCenter =
        {
            // Kalos locations with a PKMN CENTER
            18,  // Santalune City
            22,  // Lumiose City
            30,  // Camphrier Town
            40,  // Cyllage City
            44,  // Ambrette Town
            52,  // Geosenge Towny
            58,  // Shalour City
            64,  // Coumarine City
            70,  // Laverre City
            76,  // Dendemille Town
            86,  // Anistar City
            90,  // Couriway Town
            94,  // Snowbelle City
            106, // Pokémon League (X/Y)

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

        internal static GameVersion GetGameVersionForPokeCenterIndex(int index)
        {
            return PokeCenterVersion[index] == 0 ? GameVersion.XY : GameVersion.ORAS;
        }

        private static readonly int[] PokeCenterVersion = // Region matching
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

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

        private static readonly HashSet<int> MemoryGeneral = new HashSet<int> { 1, 2, 3, 4, 19, 24, 31, 32, 33, 35, 36, 37, 38, 39, 42, 52, 59 };
        private static readonly HashSet<int> MemorySpecific = new HashSet<int> { 6 };
        private static readonly HashSet<int> MemoryMove = new HashSet<int> { 12, 16, 48, 49 };
        private static readonly HashSet<int> MemoryItem = new HashSet<int> { 5, 15, 26, 34, 40, 51 };
        private static readonly HashSet<int> MemorySpecies = new HashSet<int> { 7, 9, 13, 14, 17, 21, 18, 25, 29, 44, 45, 50, 60 };

        public static MemoryArgType GetMemoryArgType(int memory)
        {
            if (MemoryGeneral.Contains(memory)) return MemoryArgType.GeneralLocation;
            if (MemorySpecific.Contains(memory)) return MemoryArgType.SpecificLocation;
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
            while (true)
            {
                int feel = Util.Rand.Next(max);
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

        public static MemoryVariableSet Read(PKM pkm) => Read(pkm, pkm.CurrentHandler);

        public static MemoryVariableSet Read(PKM pkm, int handler)
        {
            switch (handler)
            {
                case 0: // OT
                    return new MemoryVariableSet(LegalityCheckStrings.L_XOT,
                        pkm.OT_Memory, pkm.OT_TextVar,
                        pkm.OT_Intensity, pkm.OT_Feeling);
                case 1: // HT
                    return new MemoryVariableSet(LegalityCheckStrings.L_XOT,
                        pkm.HT_Memory, pkm.HT_TextVar,
                        pkm.HT_Intensity, pkm.HT_Feeling);
                default:
                    return new MemoryVariableSet(LegalityCheckStrings.L_XOT, 0, 0, 0, 0);
            }
        }
    }
}
