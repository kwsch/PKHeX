using System;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

public enum AbilityPermission : sbyte
{
    Any12H = -1,
    Any12 = 0,
    OnlyFirst = 1,
    OnlySecond = 2,
    OnlyHidden = 4,
}

public static class AbilityPermissionExtensions
{
    public static byte GetSingleValue(this AbilityPermission value) => value switch
    {
        OnlyFirst => 0,
        OnlySecond => 1,
        OnlyHidden => 2,
        _ => throw new ArgumentOutOfRangeException(nameof(value)),
    };

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

    public static bool CanBeHidden(this AbilityPermission value) => value switch
    {
        Any12H => true,
        OnlyHidden => true,
        _ => false,
    };
}
