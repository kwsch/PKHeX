using System;
using static PKHeX.Core.EntityContext;

namespace PKHeX.Core;

/// <summary>
/// "Context" is an existence island; data format restricts the types of changes that can be made (such as evolving).
/// </summary>
/// <remarks>
/// Starting in the 8th generation games, entities can move between games with wildly different evolution rules.
/// Previous implementations of a "Format Generation" were unable to differentiate if a class object was present in one of these different-rule contexts.
/// The "Format Generation" is still a useful generalization to check if certain fields are present in the entity data, or if certain mutations are possible.
/// </remarks>
public enum EntityContext : byte
{
    None = 0,
    Gen1 = 1,
    Gen2 = 2,
    Gen3 = 3,
    Gen4 = 4,
    Gen5 = 5,
    Gen6 = 6,
    Gen7 = 7,
    Gen8 = 8,

    SplitInvalid,
    Gen7b,
    Gen8a,
    Gen8b,

    MaxInvalid,
}

public static class EntityContextExtensions
{
    public static int Generation(this EntityContext value) => value < SplitInvalid ? (int)value : value switch
    {
        Gen7b => 7,
        Gen8a => 8,
        Gen8b => 8,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static bool IsValid(this EntityContext value) => value is not (0 or SplitInvalid) and < MaxInvalid;

    public static GameVersion GetSingleGameVersion(this EntityContext value) => value switch
    {
        Gen1 => GameVersion.RD,
        Gen2 => GameVersion.C,
        Gen3 => GameVersion.E,
        Gen4 => GameVersion.SS,
        Gen5 => GameVersion.W2,
        Gen6 => GameVersion.AS,
        Gen7 => GameVersion.UM,
        Gen8 => GameVersion.SH,

        Gen7b => GameVersion.GP,
        Gen8a => GameVersion.PLA,
        Gen8b => GameVersion.BD,

        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static EntityContext GetContext(this GameVersion version) => version switch
    {
        GameVersion.GP or GameVersion.GE => Gen7b,
        GameVersion.PLA => Gen8a,
        GameVersion.BD or GameVersion.SP => Gen8b,
        _ => (EntityContext)version.GetGeneration(),
    };
}
