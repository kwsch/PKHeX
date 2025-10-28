using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Utility to store all possible box manipulation types.
/// </summary>
public static class BoxManipUtil
{
    /// <summary>
    /// Grouped categories of different <see cref="IBoxManip"/>.
    /// </summary>
    public static readonly IReadOnlyList<IBoxManip>[] ManipCategories =
    [
        BoxManipDefaults.ClearCommon,
        BoxManipDefaults.SortCommon,
        BoxManipDefaults.SortAdvanced,
        BoxManipDefaults.ModifyCommon,
    ];

    /// <summary>
    /// Manipulation Group Names to be used for uniquely naming groups of GUI controls.
    /// </summary>
    /// <remarks>
    /// Order should match that of <see cref="ManipCategories"/>.
    /// </remarks>
    public static readonly string[] ManipCategoryNames =
    [
        "Delete",
        "Sort",
        "SortAdvanced",
        "Modify",
    ];

    /// <summary>
    /// Gets a <see cref="IBoxManip"/> reference that carries out the action of the requested <see cref="type"/>.
    /// </summary>
    /// <param name="type">Manipulation type.</param>
    /// <returns>Reference to <see cref="IBoxManip"/>.</returns>
    public static IBoxManip GetManip(this BoxManipType type)
    {
        foreach (var category in ManipCategories)
        {
            foreach (var manip in category)
            {
                if (manip.Type == type)
                    return manip;
            }
        }
        throw new ArgumentOutOfRangeException(nameof(type), type, null);
    }

    /// <summary>
    /// Gets the corresponding name from <see cref="ManipCategoryNames"/> for the requested <see cref="type"/>.
    /// </summary>
    /// <param name="type">Manipulation type.</param>
    /// <param name="name">Category Name</param>
    /// <returns>Category Name</returns>
    public static bool TryGetManipCategoryName(this BoxManipType type, [NotNullWhen(true)] out string? name)
    {
        for (int i = 0; i < ManipCategories.Length; i++)
        {
            foreach (var manip in ManipCategories[i])
            {
                if (manip.Type != type)
                    continue;
                name = ManipCategoryNames[i];
                return true;
            }
        }

        name = null;
        return false;
    }

    /// <summary>
    /// Gets the corresponding name from <see cref="ManipCategoryNames"/> for the requested <see cref="manip"/>.
    /// </summary>
    /// <param name="manip">Manipulation type.</param>
    /// <param name="name">Category Name</param>
    /// <returns>Category Name</returns>
    public static bool GetManipCategoryName(this IBoxManip manip, [NotNullWhen(true)] out string? name)
    {
        for (int i = 0; i < ManipCategories.Length; i++)
        {
            foreach (var m in ManipCategories[i])
            {
                if (m != manip)
                    continue;
                name = ManipCategoryNames[i];
                return true;
            }
        }

        name = null;
        return false;
    }
}
