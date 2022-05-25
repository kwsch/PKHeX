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
public enum EntityContext
{
    Invalid,
    Gen1,
    Gen2,
    Gen3,
    Gen4,
    Gen5,
    Gen6,
    Gen7,
    Gen8,

    SplitInvalid,
    Gen7b,
    Gen8a,
    Gen8b,
}

public static class EntityContextExtensions
{
    public static int Generation(this EntityContext value)
    {
        if (value < SplitInvalid)
            return (int)value;
        return value switch
        {
            Gen7b => 7,
            Gen8a => 8,
            Gen8b => 8,
            _ => throw new IndexOutOfRangeException(nameof(value)),
        };
    }
}
