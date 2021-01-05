using System;
using System.Collections.Generic;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic related to breeding.
    /// </summary>
    public static class Breeding
    {
        /// <summary>
        /// Checks if the game has a Daycare, and returns true if it does.
        /// </summary>
        /// <param name="game">Version ID to check for.</param>
        public static bool CanGameGenerateEggs(GameVersion game) => GamesWithEggs.Contains(game);

        private static readonly HashSet<GameVersion> GamesWithEggs = new()
        {
            GD, SV, C,
            R, S, E, FR, LG,
            D, P, Pt, HG, SS,
            B, W, B2, W2,
            X, Y, OR, AS,
            SN, MN, US, UM,
            SW, SH,
        };

        /// <summary>
        /// Species that have special handling for breeding.
        /// </summary>
        internal static readonly HashSet<int> MixedGenderBreeding = new()
        {
            (int)NidoranF,
            (int)NidoranM,

            (int)Volbeat,
            (int)Illumise,

            (int)Indeedee, // male/female
        };

        /// <summary>
        /// Checks if the <see cref="species"/> can be born with inherited moves from the parents.
        /// </summary>
        /// <param name="species">Entity species ID</param>
        /// <returns>True if can inherit moves, false if cannot.</returns>
        internal static bool GetCanInheritMoves(int species)
        {
            if (Legal.FixedGenderFromBiGender.Contains(species)) // Nincada -> Shedinja loses gender causing 'false', edge case
                return true;
            var pi = PKX.Personal[species];
            if (!pi.Genderless && !pi.OnlyMale)
                return true;
            if (MixedGenderBreeding.Contains(species))
                return true;
            return false;
        }

        private static readonly HashSet<int> SplitBreed_3 = new()
        {
            // Incense
            (int)Marill, (int)Azumarill,
            (int)Wobbuffet,
        };

        /// <summary>
        /// Species that can yield a different baby species when bred.
        /// </summary>
        private static readonly HashSet<int> SplitBreed = new(SplitBreed_3)
        {
            // Incense
            (int)Chansey, (int)Blissey,
            (int)MrMime, (int)MrRime,
            (int)Snorlax,
            (int)Sudowoodo,
            (int)Mantine,
            (int)Roselia, (int)Roserade,
            (int)Chimecho,
        };

        internal static ICollection<int> GetSplitBreedGeneration(int generation) => generation switch
        {
            3 => SplitBreed_3,
            4 or 5 or 6 or 7 or 8 => SplitBreed,
            _ => Array.Empty<int>(),
        };

        /// <summary>
        /// Checks if the <see cref="species"/> can be obtained from a daycare egg.
        /// </summary>
        /// <remarks>Chained with the other 2 overloads for incremental checks with different parameters.</remarks>
        public static bool CanHatchAsEgg(int species) => !NoHatchFromEgg.Contains(species);

        /// <summary>
        /// Checks if the <see cref="species"/>-<see cref="form"/> can exist as a hatched egg in the requested <see cref="generation"/>.
        /// </summary>
        /// <remarks>Chained with the other 2 overloads for incremental checks with different parameters.</remarks>
        public static bool CanHatchAsEgg(int species, int form, int generation)
        {
            if (form == 0)
                return true;

            if (FormInfo.IsTotemForm(species, form, generation))
                return false;
            if (species == (int)Pichu)
                return false; // can't get Spiky Ear Pichu eggs
            if (species is (int)Sinistea or (int)Polteageist)
                return false; // can't get Antique eggs

            return true;
        }

        /// <summary>
        /// Checks if the <see cref="species"/>-<see cref="form"/> can exist as a hatched egg in the requested <see cref="game"/>.
        /// </summary>
        /// <remarks>Chained with the other 2 overloads for incremental checks with different parameters.</remarks>
        public static bool CanHatchAsEgg(int species, int form, GameVersion game)
        {
            // Sanity check form for origin
            var gameInfo = GameData.GetPersonal(game);
            var entry = gameInfo.GetFormEntry(species, form);
            return form < entry.FormCount || (species == (int)Rotom && form <= 5);
        }

        /// <summary>
        /// Species that cannot hatch from an egg.
        /// </summary>
        private static readonly HashSet<int> NoHatchFromEgg = new()
        {
            // Gen1
            (int)Ditto,
            (int)Articuno, (int)Zapdos, (int)Moltres,
            (int)Mewtwo, (int)Mew,

            // Gen2
            (int)Unown,
            (int)Raikou, (int)Entei, (int)Suicune,
            (int)Lugia, (int)HoOh, (int)Celebi,

            // Gen3
            (int)Regirock, (int)Regice, (int)Registeel,
            (int)Latias, (int)Latios,
            (int)Kyogre, (int)Groudon, (int)Rayquaza,
            (int)Jirachi, (int)Deoxys,

            // Gen4
            (int)Uxie, (int)Mesprit, (int)Azelf,
            (int)Dialga, (int)Palkia, (int)Heatran,
            (int)Regigigas, (int)Giratina, (int)Cresselia,
            (int)Manaphy, (int)Darkrai, (int)Shaymin, (int)Arceus,

            // Gen5
            (int)Victini,
            (int)Cobalion, (int)Terrakion, (int)Virizion,
            (int)Tornadus, (int)Thundurus,
            (int)Reshiram, (int)Zekrom,
            (int)Landorus, (int)Kyurem,
            (int)Keldeo, (int)Meloetta, (int)Genesect,

            // Gen6
            (int)Xerneas, (int)Yveltal, (int)Zygarde,
            (int)Diancie, (int)Hoopa, (int)Volcanion,

            // Gen7
            (int)TypeNull, (int)Silvally,
            (int)TapuKoko, (int)TapuLele, (int)TapuBulu, (int)TapuFini,
            (int)Cosmog, (int)Cosmoem, (int)Solgaleo, (int)Lunala,
            (int)Nihilego, (int)Buzzwole, (int)Pheromosa, (int)Xurkitree, (int)Celesteela, (int)Kartana, (int)Guzzlord, (int)Necrozma,
            (int)Magearna, (int)Marshadow,
            (int)Poipole, (int)Naganadel, (int)Stakataka, (int)Blacephalon, (int)Zeraora,

            (int)Meltan, (int)Melmetal,

            // Gen8
            (int)Dracozolt, (int)Arctozolt, (int)Dracovish, (int)Arctovish,
            (int)Zacian, (int)Zamazenta, (int)Eternatus,
            (int)Kubfu, (int)Urshifu, (int)Zarude,
            (int)Regieleki, (int)Regidrago,
            (int)Glastrier, (int)Spectrier, (int)Calyrex,
        };
    }
}
