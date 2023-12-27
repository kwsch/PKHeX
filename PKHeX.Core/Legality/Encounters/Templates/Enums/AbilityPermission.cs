using System;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PKM.Ability"/> legality permissions.
/// </summary>
public enum AbilityPermission : sbyte
{
    Any12H = -1,
    Any12 = 0,
    OnlyFirst = 1,
    OnlySecond = 2,
    OnlyHidden = 4,
}

/// <summary>
/// Extension methods for <see cref="AbilityPermission"/>.
/// </summary>
public static class AbilityPermissionExtensions
{
    /// <summary>
    /// Returns the <see cref="AbilityPermission"/> value for the given index.
    /// </summary>
    /// <param name="value">Value to fetch the index for</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static byte GetSingleValue(this AbilityPermission value) => value switch
    {
        OnlyFirst => 0,
        OnlySecond => 1,
        OnlyHidden => 2,
        _ => throw new ArgumentOutOfRangeException(nameof(value)),
    };

    /// <summary>
    /// Returns the <see cref="AbilityPermission"/> value for the given index.
    /// </summary>
    /// <param name="value">Value to fetch the index for</param>
    /// <param name="index">Index to use</param>
    /// <returns>True if single index.</returns>
    public static bool IsSingleValue(this AbilityPermission value, out int index)
    {
        switch (value)
        {
            case OnlyFirst:  index = 0; return true;
            case OnlySecond: index = 1; return true;
            case OnlyHidden: index = 2; return true;
            default: index = 0; return false;
        }
    }

    /// <summary>
    /// Indicates if the given <see cref="AbilityPermission"/> value can be initially obtained with a hidden ability.
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>True if it can be a hidden ability.</returns>
    public static bool CanBeHidden(this AbilityPermission value) => value switch
    {
        Any12H => true,
        OnlyHidden => true,
        _ => false,
    };
}
