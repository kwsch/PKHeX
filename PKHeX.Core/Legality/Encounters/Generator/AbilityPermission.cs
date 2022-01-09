using System;

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
    public static int GetSingleValue(this AbilityPermission value) => value switch
    {
        AbilityPermission.OnlyFirst => 0,
        AbilityPermission.OnlySecond => 1,
        AbilityPermission.OnlyHidden => 2,
        _ => throw new ArgumentOutOfRangeException(nameof(value)),
    };

    public static bool IsSingleValue(this AbilityPermission value, out int index)
    {
        switch (value)
        {
            case AbilityPermission.OnlyFirst:  index = 0; return true;
            case AbilityPermission.OnlySecond: index = 1; return true;
            case AbilityPermission.OnlyHidden: index = 2; return true;
            default: index = 0; return false;
        }
    }

    public static bool CanBeHidden(this AbilityPermission value) => value switch
    {
        AbilityPermission.Any12H => true,
        AbilityPermission.OnlyHidden => true,
        _ => false,
    };
}
