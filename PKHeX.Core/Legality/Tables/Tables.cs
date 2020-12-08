using System.Collections.Generic;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        /// <summary>
        /// Species that trigger Light Ball yielding Volt Tackle
        /// </summary>
        public static readonly HashSet<int> LightBall = new HashSet<int> { 25, 26, 172 };

        /// <summary>
        /// Species that can change between their forms and get access to form-specific moves.
        /// </summary>
        public static readonly HashSet<int> FormChangeMoves = new HashSet<int>
        {
            386, // Deoxys
            487, // Giratina
            492, // Shaymin
            720, // Hoopa
        };

        /// <summary>
        /// Species that cannot hatch from an egg.
        /// </summary>
        public static readonly HashSet<int> NoHatchFromEgg = new HashSet<int>
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

            803, // Poipole
            804, // Naganadel
            805, // Stakataka
            806, // Blacephalon
            807, // Zeraora

            808, // Meltan
            809, // Melmetal

            (int)Species.Dracozolt,
            (int)Species.Arctozolt,
            (int)Species.Dracovish,
            (int)Species.Arctovish,

            (int)Species.Zacian,
            (int)Species.Zamazenta,
            (int)Species.Eternatus,

            (int)Species.Kubfu,
            (int)Species.Urshifu,
            (int)Species.Zarude,

            (int)Species.Regieleki,
            (int)Species.Regidrago,
            (int)Species.Glastrier,
            (int)Species.Spectrier,
            (int)Species.Calyrex,
        };

        /// <summary>
        /// Generation 3 &amp; 4 Battle Frontier Species banlist. When referencing this in context to generation 4, be sure to disallow <see cref="Species.Pichu"/> with Form 1 (Spiky).
        /// </summary>
        public static readonly HashSet<int> BattleFrontierBanlist = new HashSet<int>
        {
            150, // Mewtwo
            151, // Mew

            249, // Lugia
            250, // Ho-Oh
            251, // Celebi

            382, // Kyogre
            383, // Groudon
            384, // Rayquaza
            385, // Jirachi
            386, // Deoxys

            483, // Dialga
            484, // Palkia
            487, // Giratina
            489, // Phione
            490, // Manaphy
            491, // Darkrai
            492, // Shaymin
            493, // Arceus

            494, // Victini
            643, // Reshiram
            644, // Zekrom
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

            789, // Cosmog
            790, // Cosmoem
            791, // Solgaleo
            792, // Lunala
            800, // Necrozma
            801, // Magearna
            802, // Marshadow
            807, // Zeraora
            808, // Meltan
            809, // Melmetal
        };

        public static readonly HashSet<int> Z_Moves = new HashSet<int>
        {
            622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658,
            695, 696, 697, 698, 699, 700, 701, 702, 703,
            719,
            723, 724, 725, 726, 727, 728
        };

        /// <summary>
        /// Moves that can not be obtained by using Sketch with Smeargle.
        /// </summary>
        internal static readonly HashSet<int> InvalidSketch = new HashSet<int>(Z_Moves)
        {
            // Can't Sketch
            165, // Struggle
            448, // Chatter

            // Unreleased
            617, // Light of Ruin
        };

        /// <summary>
        /// Species classified as "Legend" by the game code.
        /// </summary>
        public static readonly HashSet<int> Legends = new HashSet<int>
        {
            (int)Species.Mewtwo, (int)Species.Mew,
            (int)Species.Lugia, (int)Species.HoOh, (int)Species.Celebi,
            (int)Species.Kyogre, (int)Species.Groudon, (int)Species.Rayquaza, (int)Species.Jirachi, (int)Species.Deoxys,
            (int)Species.Dialga, (int)Species.Palkia, (int)Species.Giratina, (int)Species.Phione, (int)Species.Manaphy, (int)Species.Darkrai, (int)Species.Shaymin, (int)Species.Arceus,
            (int)Species.Victini, (int)Species.Reshiram, (int)Species.Zekrom, (int)Species.Kyurem, (int)Species.Keldeo, (int)Species.Meloetta, (int)Species.Genesect,
            (int)Species.Xerneas, (int)Species.Yveltal, (int)Species.Zygarde, (int)Species.Diancie, (int)Species.Hoopa, (int)Species.Volcanion,
            (int)Species.Cosmog, (int)Species.Cosmoem, (int)Species.Solgaleo, (int)Species.Lunala, (int)Species.Necrozma, (int)Species.Magearna, (int)Species.Marshadow, (int)Species.Zeraora,
            (int)Species.Meltan, (int)Species.Melmetal,
            (int)Species.Zacian, (int)Species.Zamazenta, (int)Species.Eternatus, (int)Species.Zarude, (int)Species.Calyrex,
        };

        /// <summary>
        /// Species classified as "SubLegend" by the game code.
        /// </summary>
        public static readonly HashSet<int> SubLegends = new HashSet<int>
        {
            (int)Species.Articuno, (int)Species.Zapdos, (int)Species.Moltres,
            (int)Species.Raikou, (int)Species.Entei, (int)Species.Suicune,
            (int)Species.Regirock, (int)Species.Regice, (int)Species.Registeel, (int)Species.Latias, (int)Species.Latios,
            (int)Species.Uxie, (int)Species.Mesprit, (int)Species.Azelf, (int)Species.Heatran, (int)Species.Regigigas, (int)Species.Cresselia,
            (int)Species.Cobalion, (int)Species.Terrakion, (int)Species.Virizion, (int)Species.Tornadus, (int)Species.Thundurus, (int)Species.Landorus,
            (int)Species.TypeNull, (int)Species.Silvally, (int)Species.TapuKoko, (int)Species.TapuLele, (int)Species.TapuBulu, (int)Species.TapuFini,
            (int)Species.Nihilego, (int)Species.Buzzwole, (int)Species.Pheromosa, (int)Species.Xurkitree, (int)Species.Celesteela, (int)Species.Kartana, (int)Species.Guzzlord,
            (int)Species.Poipole, (int)Species.Naganadel, (int)Species.Stakataka, (int)Species.Blacephalon,
            (int)Species.Kubfu, (int)Species.Urshifu, (int)Species.Regieleki, (int)Species.Regidrago, (int)Species.Glastrier, (int)Species.Spectrier,
        };

        /// <summary>
        /// Species that evolve from a Bi-Gendered species into a Single-Gender.
        /// </summary>
        public static readonly HashSet<int> FixedGenderFromBiGender = new HashSet<int>
        {
            (int)Species.Nincada,
            (int)Species.Shedinja, // (G)

            (int)Species.Burmy,
            (int)Species.Wormadam, //(F)
            (int)Species.Mothim, // (M)

            (int)Species.Ralts,
            (int)Species.Gallade, // (M)

            (int)Species.Snorunt,
            (int)Species.Froslass, // (F)

            (int)Species.Espurr,
            (int)Species.Meowstic, // (M/F) form specific
        };
    }
}
