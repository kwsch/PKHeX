namespace PKHeX
{
    public static partial class Legal
    {
        internal static readonly int[] Items_Ball =
        {
            000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012,
            013, 014, 015, 016, 492, 493, 494, 495, 496, 497, 498, 499, 576,
            851
        };
        internal static readonly int[] Items_CommonBall = { 4, 3, 2, 1 };
        internal static readonly int[] Items_UncommonBall =
        {
            7, 576, 13, 492, 497, 14, 495, 493, 496, 494, 11, 498, 8, 6,
            12, 15, 9, 5, 499, 10, 16
        };
        internal static readonly int[] Gen4EncounterTypes = { 1, 2, 4, 5, 7, 9, 10, 12, 23, 24 };
        internal static readonly int[] LightBall = { 25, 26, 172 };
        internal static readonly int[] Fossils = { 138, 140, 142, 345, 347, 408, 410, 564, 566, 696, 698 };
        internal static readonly int[] RotomMoves = { 0, 315, 056, 059, 403, 437 };
        internal static readonly int[] WildForms =
        {
            422, 423, // Shellos
            550, // Basculin
            669, 670, 671 // Flabébé
        };
        internal static readonly int[] SplitBreed =
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
        internal static readonly int[] FormChange = // Pokémon that can change form and retain it
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
            718, // Zygarde
            720, // Hoopa
        };
        internal static readonly int[] FormChangeMoves =
        {
            386, // Deoxys
            487, // Giratina
            492, // Shaymin
            720, // Hoopa
        };
        internal static readonly int[] BreedMaleOnly =
        {
            128, // Tauros
            627, 628, // Rufflet
            236, 106, 107, 237, // Tyrogue
            538, 539, // Sawk & Throh
        };

        #region Legality; unhatchable from egg

        internal static readonly int[] NoHatchFromEgg =
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

        internal static readonly int[] BattleForms =
        {
            351, // Castform
            421, // Cherrim
            555, // Darmanitan
            648, // Meloetta
            681, // Aegislash
            719, // Xerneas
            746, // Wishiwashi
            778, // Mimikyu
        };
        internal static readonly int[] BattleMegas =
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
        internal static readonly int[] BattlePrimals = { 382, 383 };

        internal static readonly int[] Z_Moves =
        {
            622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658,
            695, 696, 697, 698, 699, 700, 701, 702, 703,
            719,
        };

        #region Games

        internal static readonly int[] Games_7sm = { 30, 31 };
        internal static readonly int[] Games_6xy = { 24, 25 };
        internal static readonly int[] Games_6oras = { 26, 27 };
        internal static readonly int[] Games_5 = { 20, 21, 22, 23 };
        internal static readonly int[] Games_4 = { 10, 11, };
        internal static readonly int[] Games_4e = { 12 };
        internal static readonly int[] Games_4r = { 7, 8 };
        internal static readonly int[] Games_3 = { 1, 2 };
        internal static readonly int[] Games_3e = { 3 };
        internal static readonly int[] Games_3r = { 4, 5 };
        internal static readonly int[] Games_3s = { 15 };

        #endregion
    }
}
