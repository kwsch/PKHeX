using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class BoxManipUtil
    {
        /// <summary>
        /// Grouped categories of different <see cref="IBoxManip"/>.
        /// </summary>
        public static readonly IReadOnlyList<IBoxManip>[] ManipCategories =
        {
            BoxManipBase.ClearCommon,
            BoxManipBase.SortCommon,
            BoxManipBase.SortAdvanced,
            BoxManipBase.ModifyCommon,
        };

        public static readonly string[] ManipCategoryNames =
        {
            "Delete",
            "Sort",
            "SortAdvanced",
            "Modify",
        };

        /// <summary>
        /// Gets a <see cref="IBoxManip"/> reference that carries out the action of the requested <see cref="type"/>.
        /// </summary>
        /// <param name="type">Manipulation type.</param>
        /// <returns>Reference to <see cref="IBoxManip"/>.</returns>
        public static IBoxManip GetManip(this BoxManipType type) => ManipCategories.SelectMany(c => c).FirstOrDefault(m => m.Type == type);

        /// <summary>
        /// Gets the corresponding name from <see cref="ManipCategoryNames"/> for the requested <see cref="type"/>.
        /// </summary>
        /// <param name="type">Manipulation type.</param>
        /// <returns>Category Name</returns>
        public static string? GetManipCategoryName(this BoxManipType type)
        {
            for (int i = 0; i < ManipCategories.Length; i++)
            {
                if (ManipCategories[i].Any(z => z.Type == type))
                    return ManipCategoryNames[i];
            }
            return null;
        }

        /// <summary>
        /// Gets the corresponding name from <see cref="ManipCategoryNames"/> for the requested <see cref="manip"/>.
        /// </summary>
        /// <param name="manip">Manipulation type.</param>
        /// <returns>Category Name</returns>
        public static string? GetManipCategoryName(this IBoxManip manip)
        {
            for (int i = 0; i < ManipCategories.Length; i++)
            {
                if (ManipCategories[i].Any(z => z == manip))
                    return ManipCategoryNames[i];
            }
            return null;
        }
    }
}
