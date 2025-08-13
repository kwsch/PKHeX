using System;

namespace PKHeX.Core;

/// <summary>
/// Word filter contexts used by different <see cref="EntityContext"/>.
/// </summary>
public enum WordFilterType
{
    /// <summary>
    /// No strict filtering is applied.
    /// </summary>
    None,

    /// <summary>
    /// Generation 5 word filter, used for games like Black/White and Black 2/White 2.
    /// </summary>
    /// <remarks>
    /// <see cref="WordFilter5"/>
    /// </remarks>
    Gen5,

    /// <summary>
    /// Generation 6 and 7 word filter, used for games like X/Y, Sun/Moon, Ultra Sun/Ultra Moon, and Sword/Shield.
    /// </summary>
    /// <remarks>
    /// See <see cref="WordFilter3DS"/>
    /// </remarks>
    Nintendo3DS,

    /// <summary>
    /// Generation 8+ word filter.
    /// </summary>
    /// <remarks>
    /// See <see cref="WordFilterNX"/>
    /// </remarks>
    NintendoSwitch,
}

public static class WordFilterTypeExtensions
{
    public static WordFilterType GetName(EntityContext type) => type.GetConsole() switch
    {
        GameConsole.NX => WordFilterType.NintendoSwitch,
        _ => type.Generation() switch
        {
            5 => WordFilterType.Gen5,
            6 or 7 => WordFilterType.Nintendo3DS,
            _ => WordFilterType.None,
        },
    };

    public static Type GetType(WordFilterType type) => type switch
    {
        WordFilterType.Gen5 => typeof(WordFilter5),
        WordFilterType.Nintendo3DS => typeof(WordFilter3DS),
        WordFilterType.NintendoSwitch => typeof(WordFilterNX),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, $"Invalid {nameof(WordFilterType)} value."),
    };
}
