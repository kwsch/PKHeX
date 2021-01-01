using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        /// <summary>
        /// Species that trigger Light Ball yielding Volt Tackle
        /// </summary>
        public static readonly HashSet<int> LightBall = new() {(int) Pikachu, (int) Raichu, (int) Pichu};

        /// <summary>
        /// Species that can change between their forms and get access to form-specific moves.
        /// </summary>
        public static readonly HashSet<int> FormChangeMoves = new()
        {
            (int)Deoxys,
            (int)Giratina,
            (int)Shaymin,
            (int)Hoopa,
        };

        /// <summary>
        /// Generation 3 &amp; 4 Battle Frontier Species banlist. When referencing this in context to generation 4, be sure to disallow <see cref="Pichu"/> with Form 1 (Spiky).
        /// </summary>
        public static readonly HashSet<int> BattleFrontierBanlist = new()
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

        public static readonly HashSet<int> Z_Moves = new()
        {
            622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658,
            695, 696, 697, 698, 699, 700, 701, 702, 703,
            719,
            723, 724, 725, 726, 727, 728
        };

        /// <summary>
        /// Moves that can not be obtained by using Sketch with Smeargle.
        /// </summary>
        internal static readonly HashSet<int> InvalidSketch = new(Z_Moves)
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
        public static readonly HashSet<int> Legends = new()
        {
            (int)Mewtwo, (int)Mew,
            (int)Lugia, (int)HoOh, (int)Celebi,
            (int)Kyogre, (int)Groudon, (int)Rayquaza, (int)Jirachi, (int)Deoxys,
            (int)Dialga, (int)Palkia, (int)Giratina, (int)Phione, (int)Manaphy, (int)Darkrai, (int)Shaymin, (int)Arceus,
            (int)Victini, (int)Reshiram, (int)Zekrom, (int)Kyurem, (int)Keldeo, (int)Meloetta, (int)Genesect,
            (int)Xerneas, (int)Yveltal, (int)Zygarde, (int)Diancie, (int)Hoopa, (int)Volcanion,
            (int)Cosmog, (int)Cosmoem, (int)Solgaleo, (int)Lunala, (int)Necrozma, (int)Magearna, (int)Marshadow, (int)Zeraora,
            (int)Meltan, (int)Melmetal,
            (int)Zacian, (int)Zamazenta, (int)Eternatus, (int)Zarude, (int)Calyrex,
        };

        /// <summary>
        /// Species classified as "SubLegend" by the game code.
        /// </summary>
        public static readonly HashSet<int> SubLegends = new()
        {
            (int)Articuno, (int)Zapdos, (int)Moltres,
            (int)Raikou, (int)Entei, (int)Suicune,
            (int)Regirock, (int)Regice, (int)Registeel, (int)Latias, (int)Latios,
            (int)Uxie, (int)Mesprit, (int)Azelf, (int)Heatran, (int)Regigigas, (int)Cresselia,
            (int)Cobalion, (int)Terrakion, (int)Virizion, (int)Tornadus, (int)Thundurus, (int)Landorus,
            (int)TypeNull, (int)Silvally, (int)TapuKoko, (int)TapuLele, (int)TapuBulu, (int)TapuFini,
            (int)Nihilego, (int)Buzzwole, (int)Pheromosa, (int)Xurkitree, (int)Celesteela, (int)Kartana, (int)Guzzlord,
            (int)Poipole, (int)Naganadel, (int)Stakataka, (int)Blacephalon,
            (int)Kubfu, (int)Urshifu, (int)Regieleki, (int)Regidrago, (int)Glastrier, (int)Spectrier,
        };

        /// <summary>
        /// Species that evolve from a Bi-Gendered species into a Single-Gender.
        /// </summary>
        public static readonly HashSet<int> FixedGenderFromBiGender = new()
        {
            (int)Nincada,
            (int)Shedinja, // (G)

            (int)Burmy,
            (int)Wormadam, //(F)
            (int)Mothim, // (M)

            (int)Ralts,
            (int)Gallade, // (M)

            (int)Snorunt,
            (int)Froslass, // (F)

            (int)Espurr,
            (int)Meowstic, // (M/F) form specific
        };
    }
}
