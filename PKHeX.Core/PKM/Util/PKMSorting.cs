using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class PKMSorting
    {
        /// <summary>
        /// Sorts an <see cref="Enumerable"/> list of <see cref="PKM"/> objects with ascending <see cref="PKM.Species"/> values.
        /// </summary>
        /// <param name="list">Source list to sort</param>
        /// <returns>Enumerable list that is sorted</returns>
        public static IEnumerable<PKM> OrderBySpecies(this IEnumerable<PKM> list)
        {
            return list.InitialSortBy()
                .ThenBy(p => p.Species)
                .FinalSortBy();
        }

        /// <summary>
        /// Sorts an <see cref="Enumerable"/> list of <see cref="PKM"/> objects with descending <see cref="PKM.Species"/> values.
        /// </summary>
        /// <param name="list">Source list to sort</param>
        /// <returns>Enumerable list that is sorted</returns>
        public static IEnumerable<PKM> OrderByDescendingSpecies(this IEnumerable<PKM> list)
        {
            return list.InitialSortBy()
                .ThenByDescending(p => p.Species)
                .FinalSortBy();
        }

        /// <summary>
        /// Sorts an <see cref="Enumerable"/> list of <see cref="PKM"/> objects with ascending <see cref="PKM.CurrentLevel"/> values.
        /// </summary>
        /// <param name="list">Source list to sort</param>
        /// <returns>Enumerable list that is sorted</returns>
        public static IEnumerable<PKM> OrderByLevel(this IEnumerable<PKM> list)
        {
            return list.InitialSortBy()
                .ThenBy(p => p.CurrentLevel)
                .ThenBy(p => p.Species)
                .FinalSortBy();
        }

        /// <summary>
        /// Sorts an <see cref="Enumerable"/> list of <see cref="PKM"/> objects with descending <see cref="PKM.CurrentLevel"/> values.
        /// </summary>
        /// <param name="list">Source list to sort</param>
        /// <returns>Enumerable list that is sorted</returns>
        public static IEnumerable<PKM> OrderByDescendingLevel(this IEnumerable<PKM> list)
        {
            return list.InitialSortBy()
                .ThenByDescending(p => p.CurrentLevel)
                .ThenBy(p => p.Species)
                .FinalSortBy();
        }

        /// <summary>
        /// Sorts an <see cref="Enumerable"/> list of <see cref="PKM"/> objects by the date they were obtained.
        /// </summary>
        /// <param name="list">Source list to sort</param>
        /// <returns>Enumerable list that is sorted</returns>
        public static IEnumerable<PKM> OrderByDateObtained(this IEnumerable<PKM> list)
        {
            return list.InitialSortBy()
                .ThenBy(p => p.MetDate ?? p.EggMetDate)
                .ThenBy(p => p.Species)
                .FinalSortBy();
        }

        /// <summary>
        /// Sorts an <see cref="Enumerable"/> list of <see cref="PKM"/> objects by the date they were obtained.
        /// </summary>
        /// <param name="list">Source list to sort</param>
        /// <returns>Enumerable list that is sorted</returns>
        public static IEnumerable<PKM> OrderByUsage(this IEnumerable<PKM> list)
        {
            return list.InitialSortBy()
                .ThenByDescending(GetFriendshipDelta) // friendship raised evaluation
                .ThenBy(p => p.Species)
                .FinalSortBy();
        }

        /// <summary>
        /// Sorts an <see cref="Enumerable"/> list of <see cref="PKM"/> objects alphabetically by <see cref="PKM.Species"/> name.
        /// </summary>
        /// <param name="list">Source list to sort</param>
        /// <param name="speciesNames">Names of each species</param>
        /// <returns>Enumerable list that is sorted</returns>
        public static IEnumerable<PKM> OrderBySpeciesName(this IEnumerable<PKM> list, IReadOnlyList<string> speciesNames)
        {
            int max = speciesNames.Count - 1;
            string SpeciesName(int s) => s > max ? string.Empty : speciesNames[s];

            return list.InitialSortBy()
                .ThenBy(p => p.Species > max) // out of range sanity check
                .ThenBy(p => SpeciesName(p.Species))
                .FinalSortBy();
        }

        /// <summary>
        /// Sorts an <see cref="Enumerable"/> list of <see cref="PKM"/> objects to display those originally obtained by the current <see cref="ITrainerInfo"/>.
        /// </summary>
        /// <param name="list">Source list to sort</param>
        /// <param name="trainer">The <see cref="ITrainerInfo"/> requesting the sorted data.</param>
        /// <returns>Enumerable list that is sorted</returns>
        public static IEnumerable<PKM> OrderByOwnership(this IEnumerable<PKM> list, ITrainerInfo trainer)
        {
            return list.InitialSortBy()
                .ThenByDescending(p => trainer.IsOriginalHandler(p, ((GameVersion)trainer.Game).IsValidSavedVersion())) // true first
                .ThenByDescending(p => string.Equals(p.OT_Name, trainer.OT, StringComparison.CurrentCultureIgnoreCase))
                .OrderByTrainer()
                .ThenBy(p => p.Species)
                .FinalSortBy();
        }

        /// <summary>
        /// Sorts an <see cref="Enumerable"/> list of <see cref="PKM"/> objects based on the provided filter operations.
        /// </summary>
        /// <param name="list">Source list to sort</param>
        /// <param name="filters">Filter operations to sort with (sorted with ThenBy after the initial sort).</param>
        /// <returns>Enumerable list that is sorted</returns>
        public static IEnumerable<PKM> OrderByCustom(this IEnumerable<PKM> list, params Func<PKM, IComparable>[] filters)
        {
            var init = list.InitialSortBy();
            return filters.Aggregate(init, (current, f) => current.ThenBy(f))
                .FinalSortBy();
        }

        /// <summary>
        /// Sorts an <see cref="Enumerable"/> list of <see cref="PKM"/> objects based on the provided filter operations.
        /// </summary>
        /// <param name="list">Source list to sort</param>
        /// <param name="filters">Filter operations to sort with (sorted with ThenBy after the initial sort).</param>
        /// <returns>Enumerable list that is sorted</returns>
        /// <remarks>Boolean sort doesn't pair well with <see cref="FinalSortBy"/>, so just keep original sorting order.</remarks>
        public static IEnumerable<PKM> OrderByCustom(this IEnumerable<PKM> list, params Func<PKM, bool>[] filters)
        {
            var init = list.InitialSortBy();
            return filters.Aggregate(init, (current, f) => current.ThenBy(f))
                ;
        }

        /// <summary>
        /// Sorts an <see cref="Enumerable"/> list of <see cref="PKM"/> objects in reverse.
        /// </summary>
        /// <param name="list">Source list to revese sort</param>
        /// <returns>Enumerable list that is sorted</returns>
        public static IEnumerable<PKM> ReverseSort(this IEnumerable<PKM> list)
        {
            int i = 0;
            return list.InitialSortBy()
                .ThenByDescending(_ => i++)
                ; // can't sort further
        }

        /// <summary>
        /// Common pre-filtering of <see cref="PKM"/> data.
        /// </summary>
        /// <param name="list">Input list of <see cref="PKM"/> data.</param>
        private static IOrderedEnumerable<PKM> InitialSortBy(this IEnumerable<PKM> list)
        {
            return list
                .OrderBy(p => p.Species == 0) // empty slots at end
                .ThenBy(p => p.IsEgg); // eggs to the end
        }

        /// <summary>
        /// Common post-filtering of <see cref="PKM"/> data.
        /// </summary>
        /// <param name="result">Output list of <see cref="PKM"/> data.</param>
        private static IOrderedEnumerable<PKM> FinalSortBy(this IOrderedEnumerable<PKM> result)
        {
            var postSorted = result
                .ThenBy(p => p.Form) // forms sorted
                .ThenBy(p => p.Gender) // gender sorted
                .ThenBy(p => p.IsNicknamed);
            return postSorted;
        }

        /// <summary>
        /// Common mid-filtering grouping of PKM data according to the Original Trainer details.
        /// </summary>
        /// <param name="list">Output list of <see cref="PKM"/> data.</param>
        private static IOrderedEnumerable<PKM> OrderByTrainer(this IOrderedEnumerable<PKM> list)
        {
            return list.ThenBy(p => p.OT_Name)
                .ThenBy(p => p.OT_Gender)
                .ThenBy(p => p.TID)
                .ThenBy(p => p.SID);
        }

        /// <summary>
        /// Gets if the current handler is the original trainer.
        /// </summary>
        /// <param name="trainer">The <see cref="ITrainerInfo"/> requesting the check.</param>
        /// <param name="pk">Pokémon data</param>
        /// <param name="checkGame">Toggle to check the game's version or not</param>
        /// <returns>True if OT, false if not OT.</returns>
        public static bool IsOriginalHandler(this ITrainerInfo trainer, PKM pk, bool checkGame)
        {
            if (pk.Format >= 6)
                return pk.CurrentHandler != 1;
            if (checkGame && trainer.Game != pk.Version)
                return false;
            if (trainer.TID != pk.TID || trainer.SID != pk.SID)
                return false;
            if (trainer.Gender != pk.OT_Gender)
                return false;
            return trainer.OT == pk.OT_Name;
        }

        /// <summary>
        /// Gets a Friendship delta rating to indicate how much the <see cref="PKM.CurrentFriendship"/> has been raised vs its base Friendship value.
        /// </summary>
        /// <param name="pk">Pokémon data</param>
        /// <returns>255 if maxed, else the difference between current &amp; base.</returns>
        private static int GetFriendshipDelta(PKM pk)
        {
            var currentFriendship = pk.CurrentFriendship;
            if (currentFriendship == 255)
                return 255;
            var baseFriendship = pk.PersonalInfo.BaseFriendship;
            return currentFriendship - baseFriendship;
        }
    }
}
