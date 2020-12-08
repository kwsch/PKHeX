using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic related to breeding.
    /// </summary>
    public static class Breeding
    {
        /// <summary>
        /// Species that have special handling for breeding.
        /// </summary>
        internal static readonly HashSet<int> MixedGenderBreeding = new HashSet<int>
        {
            (int)Species.NidoranF,
            (int)Species.NidoranM,

            (int)Species.Volbeat,
            (int)Species.Illumise,

            (int)Species.Indeedee, // male/female
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

        internal static readonly HashSet<int> SplitBreed_3 = new HashSet<int>
        {
            // Incense
            (int)Species.Marill, (int)Species.Azumarill,
            (int)Species.Wobbuffet,
        };

        /// <summary>
        /// Species that can yield a different baby species when bred.
        /// </summary>
        private static readonly HashSet<int> SplitBreed = new HashSet<int>(SplitBreed_3)
        {
            // Incense
            (int)Species.Chansey, (int)Species.Blissey,
            (int)Species.MrMime, (int)Species.MrRime,
            (int)Species.Snorlax,
            (int)Species.Sudowoodo,
            (int)Species.Mantine,
            (int)Species.Roselia, (int)Species.Roserade,
            (int)Species.Chimecho,
        };

        internal static ICollection<int> GetSplitBreedGeneration(int generation)
        {
            switch (generation)
            {
                case 3: return SplitBreed_3;

                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    return SplitBreed;

                default: return Array.Empty<int>();
            }
        }
    }
}
