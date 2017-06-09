using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        public static readonly int[] Items_Ball =
        {
            000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012,
            013, 014, 015, 016, 492, 493, 494, 495, 496, 497, 498, 499, 576,
            851
        };
        public static readonly int[] Items_CommonBall = { 4, 3, 2, 1 };
        public static readonly int[] Items_UncommonBall =
        {
            7, 576, 13, 492, 497, 14, 495, 493, 496, 494, 11, 498, 8, 6,
            12, 15, 9, 5, 499, 10, 16
        };
        public static readonly int[] Gen4EncounterTypes = { 1, 2, 4, 5, 7, 9, 10, 12, 23, 24 };
        public static readonly int[] LightBall = { 25, 26, 172 };
        public static readonly int[] Fossils = { 138, 140, 142, 345, 347, 408, 410, 564, 566, 696, 698 };
        public static readonly int[] RotomMoves = { 0, 315, 056, 059, 403, 437 };
        public static readonly int[] WildForms =
        {
            422, 423, // Shellos
            550, // Basculin
            669, 670, 671 // Flabébé
        };
        public static readonly int[] SplitBreed =
        {
            // Incense
            113, 242, // Chansey
            122, // Mr. Mime
            143, // Snorlax
            183, 184, // Marill
            185, // Sudowoodo
            202, // Wobbuffet
            226, // Mantine
            315, 407, // Roselia
            358, // Chimecho
        };
        public static readonly int[] FormChange = // Pokémon that can change form and retain it
        {
            386, // Deoxys
            412, // Burmy
            421, // Cherrim
            479, // Rotom
            487, // Giratina
            492, // Shaymin
            493, // Arceus
            641, // Tornadus
            642, // Thundurus
            645, // Landorus
            646, // Kyurem
            647, // Keldeo
            649, // Genesect
            676, // Furfrou
            720, // Hoopa
            741, // Oricorio
            773, // Silvally
        };
        public static readonly int[] FormChangeMoves =
        {
            386, // Deoxys
            487, // Giratina
            492, // Shaymin
            720, // Hoopa
        };
        public static readonly int[] BreedMaleOnly =
        {
            128, // Tauros
            627, 628, // Rufflet
            236, 106, 107, 237, // Tyrogue
            538, 539, // Sawk & Throh
        };

        #region Legality; unhatchable from egg

        public static readonly int[] NoHatchFromEgg =
        {
            132, // Ditto
            144, // Articuno
            145, // Zapdos
            146, // Moltres
            150, // Mewtwo
            151, // Mew

            201, // Unown
            243, // Raikou
            244, // Entei
            245, // Suicune
            249, // Lugia
            250, // Ho-Oh
            251, // Celebi

            377, // Regirock
            378, // Regice
            379, // Registeel
            380, // Latias
            381, // Latios
            382, // Kyogre
            383, // Groudon
            384, // Rayquaza
            385, // Jirachi
            386, // Deoxys

            480, // Uxie
            481, // Mesprit
            482, // Azelf
            483, // Dialga
            484, // Palkia
            485, // Heatran
            486, // Regigigas
            487, // Giratina
            488, // Cresselia
            490, // Manaphy
            491, // Darkrai
            492, // Shaymin
            493, // Arceus

            494, // Victini
            638, // Cobalion
            639, // Terrakion
            640, // Virizion
            641, // Tornadus
            642, // Thundurus
            643, // Reshiram
            644, // Zekrom
            645, // Landorus
            646, // Kyurem
            647, // Keldeo
            648, // Meloetta
            649, // Genesect

            716, // Xerneas
            717, // Yveltal
            718, // Zygarde
            719, // Diancie
            720, // Hoopa
            721, // Volcanion

            772, // Type: Null
            773, // Silvally
            785, // Tapu Koko
            786, // Tapu Lele
            787, // Tapu Bulu
            788, // Tapu Fini
            789, // Cosmog
            790, // Cosmoem
            791, // Solgaleo
            792, // Lunala
            793, // Nihilego
            794, // Buzzwole
            795, // Pheromosa
            796, // Xurkitree
            797, // Celesteela
            798, // Kartana
            799, // Guzzlord
            800, // Necrozma
            801, // Magearna
            802, // Marshadow
        };

        #endregion

        public static readonly int[] BattleForms =
        {
            351, // Castform
            421, // Cherrim
            555, // Darmanitan
            648, // Meloetta
            681, // Aegislash
            716, // Xerneas
            746, // Wishiwashi
            778, // Mimikyu
        };
        public static readonly int[] BattleMegas =
        {
            // XY
            3,6,9,65,80,
            115,127,130,142,150,181,
            212,214,229,248,282,
            303,306,308,310,354,359,380,381,
            445,448,460,

            // AO
            15,18,94,
            208,254,257,260,
            302,319,323,334,362,373,376,384,
            428,475,
            531,
            719
        };
        public static readonly int[] BattlePrimals = { 382, 383 };

        public static readonly int[] Z_Moves =
        {
            622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658,
            695, 696, 697, 698, 699, 700, 701, 702, 703,
            719,
        };
        internal static readonly int[] InvalidSketch = new[] { 165, 448 }.Concat(Z_Moves).ToArray(); // Struggle & Chatter

        public static readonly int[] Legends =
        {
            150, 151, 249, 250, 251, 382, 383, 384, 385, 386, 483,
            484, 487, 489, 490, 491, 492, 493, 494, 643, 644, 646,
            647, 648, 649, 716, 717, 718, 719, 720, 721, 789, 790,
            791, 792, 800, 801, 802
        };
        public static readonly int[] SubLegends =
        {
            144, 145, 146, 243, 244, 245, 377, 378, 379, 380, 381,
            480, 481, 482, 485, 486, 488, 638, 639, 640, 641, 642,
            645, 772, 773, 787, 788, 785, 786, 793, 794, 795, 796,
            797, 798, 799
        };

        public static readonly int[] Arceus_Plate = {303, 306, 304, 305, 309, 308, 310, 313, 298, 299, 301, 300, 307, 302, 311, 312, 644};
        public static readonly int[] Arceus_ZCrystal = {782, 785, 783, 784, 788, 787, 789, 792, 777, 778, 780, 779, 786, 781, 790, 791, 793};

        internal static readonly int[] BabyEvolutionWithMove =
        {
            122, // Mr. Mime (Mime Jr with Mimic)
            185, // Sudowoodo (Bonsly with Mimic)
        };
        
        // List of species that evolve from a previous species having a move while leveling up
        internal static readonly int[] SpeciesEvolutionWithMove =
        {
            122, // Mr. Mime (Mime Jr with Mimic)
            185, // Sudowoodo (Bonsly with Mimic)
            424, // Ambipom (Aipom with Double Hit)
            463, // Lickilicky (Lickitung with Rollout)
            465, // Tangrowth (Tangela with Ancient Power)
            469, // Yanmega (Yamma with Ancient Power)
            473, // Mamoswine (Piloswine with Ancient Power)
            700, // Sylveon (Eevee with Fairy Move)
            763, // Tsareena (Steenee with Stomp)
        };
        internal static readonly int[] FairyMoves =
        {
            186, //Sweet Kiss
            204, //Charm
            236, //Moonlight 
            574, //Disarming Voice 
            577, //Draining Kiss 
            578, //Crafty Shield 
            579, //Flower Shield 
            581, //Misty Terrain 
            583, //Play Rough 
            584, //Fairy Wind 
            585, //Moonblast 
            587, //Fairy Lock 
            597, //Aromatic Mist 
            601, //Geomancy 
            605, //Dazzling Gleam 
            608, //Baby-Doll Eyes 
            617, //Light of Ruin 
            656, //Twinkle Tackle 
            657, //Twinkle Tackle 
            666, //Floral Healing 
            698, //Guardian of Alola 
            705, //Fleur Cannon 
            717, //Nature's Madness 
        };
        // Moves that trigger the evolution by move
        internal static readonly int[][] MoveEvolutionWithMove =
        {
            new [] { 102 }, // Mr. Mime (Mime Jr with Mimic)
            new [] { 102 }, // Sudowoodo (Bonsly with Mimic)
            new [] { 458 }, // Ambipom (Aipom with Double Hit)
            new [] { 205 }, // Lickilicky (Lickitung with Rollout)
            new [] { 246 }, // Tangrowth (Tangela with Ancient Power)
            new [] { 246 }, // Yanmega (Yamma with Ancient Power)
            new [] { 246 }, // Mamoswine (Piloswine with Ancient Power)
            FairyMoves, // Sylveon (Eevee with Fairy Move)
            new [] { 023 }, // Tsareena (Steenee with Stomp)
        };
        // Min level for any species for every generation to learn the move for evolution by move
        // 0 means it cant be learned in that generation
        internal static readonly int[][] MinLevelEvolutionWithMove =
        {
            // Mr. Mime (Mime Jr with Mimic)
            new [] { 0, 0, 0, 0, 18, 15, 15, 2 },
            // Sudowoodo (Bonsly with Mimic)
            new [] { 0, 0, 0, 0, 17, 17, 15, 2 },
            // Ambipom (Aipom with Double Hit)
            new [] { 0, 0, 0, 0, 32, 32, 32, 2 },
            // Lickilicky (Lickitung with Rollout)
            new [] { 0, 0, 2, 0, 2, 33, 33, 2 },
            // Tangrowth (Tangela with Ancient Power)
            new [] { 0, 0, 0, 0, 2, 36, 38, 2 },
            // Yanmega (Yanma with Ancient Power)
            new [] { 0, 0, 0, 0, 2, 33, 33, 2 },
            // Mamoswine (Piloswine with Ancient Power)
            new [] { 0, 0, 0, 0, 2, 2, 2, 2 },
            // Sylveon (Eevee with Fairy Move)
            new [] { 0, 0, 0, 0, 0, 29, 9, 2 },
            // Tsareena (Steenee with Stomp)
            new [] { 0, 0, 0, 0, 0, 0, 0, 2 },
        };
        // True -> the pokemon could hatch from an egg with the move for evolution as an egg move
        internal static readonly bool[][] EggMoveEvolutionWithMove =
        {
            // Mr. Mime (Mime Jr with Mimic)
            new [] { false, false, false, false, true, true, true, true },
            // Sudowoodo (Bonsly with Mimic)
            new [] { false, false, false, false, true, true, true, true },
            // Ambipom (Aipom with Double Hit)
            new [] { false, false, false, false, true, true, true, true },
            // Lickilicky (Lickitung with Rollout)
            new [] { false, false, true, false, true, true, true, true },
            // Tangrowth (Tangela with Ancient Power)
            new [] { false, false, false, false, true, true, true, true },
            // Yanmega (Yanma with Ancient Power)
            new [] { false, false, false, false, true, true, true, true },
            // Mamoswine (Piloswine with Ancient Power)
            new [] { false, false, true, true, true, true, true, true },
            // Sylveon (Eevee with Fairy Move)
            new [] { false, false, true, true, true, true, true, true },
            // Tsareena (Steenee with Stomp)
            new [] { false, false, false, false, false, false, false, false },
        };
        internal static readonly int[] MixedGenderBreeding =
        {
            29, // Nidoran♀
            32, // Nidoran♂
            314, // Volbeat
            314, // Illumise
        };
        #region Games

        public static readonly int[] Games_7vc2 = { 39, 40, 41 }; // Gold, Silver, Crystal
        public static readonly int[] Games_7vc1 = { 35, 36, 37, 38 }; // Red, Green, Blue, Yellow
        public static readonly int[] Games_7go = { 34 };
        public static readonly int[] Games_7sm = { 30, 31 };
        public static readonly int[] Games_6xy = { 24, 25 };
        public static readonly int[] Games_6oras = { 26, 27 };
        public static readonly int[] Games_5 = { 20, 21, 22, 23 };
        public static readonly int[] Games_4 = { 10, 11, };
        public static readonly int[] Games_4e = { 12 };
        public static readonly int[] Games_4r = { 7, 8 };
        public static readonly int[] Games_3 = { 1, 2 };
        public static readonly int[] Games_3e = { 3 };
        public static readonly int[] Games_3r = { 4, 5 };
        public static readonly int[] Games_3s = { 15 };

        #endregion
    }
}
