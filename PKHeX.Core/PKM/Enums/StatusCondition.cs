using System;

namespace PKHeX.Core;

/// <summary>
/// Status condition flags for Generation 1-4.
/// </summary>
/// <remarks>
/// Bad poison is only stored outside of battle in Generation 3 and 4.
/// </remarks>
[Flags]
public enum StatusCondition : byte
{
    None = 0,
#pragma warning disable RCS1191 // Declare enum value as combination of names
    // Sleep (if present) indicates the number of turns remaining
    Sleep1 = 1,
    Sleep2 = 2,
    Sleep3 = 3,
    Sleep4 = 4,
    Sleep5 = 5,
    Sleep6 = 6,
    Sleep7 = 7,
#pragma warning restore RCS1191 // Declare enum value as combination of names
    Poison = 1 << 3,
    Burn = 1 << 4,
    Freeze = 1 << 5,
    Paralysis = 1 << 6,
    PoisonBad = 1 << 7,
}

/// <summary>
/// Status condition enum for Generation 5+.
/// </summary>
public enum StatusType : byte
{
    None = 0,
    Paralysis = 1,
    Sleep = 2,
    Freeze = 3,
    Burn = 4,
    Poison = 5,
}

public static class StatusConditionUtil
{
    public static StatusType GetStatusType(this PKM pk)
    {
        var value = (StatusCondition)pk.Status_Condition;
        return GetStatusType(value);
    }

    public static StatusType GetStatusType(this StatusCondition value)
    {
        if (value == StatusCondition.None)
            return StatusType.None;
        if (value <= StatusCondition.Sleep7)
            return StatusType.Sleep;

        if ((value & StatusCondition.Paralysis) != 0)
            return StatusType.Paralysis;
        if ((value & StatusCondition.Burn) != 0)
            return StatusType.Burn;
        if ((value & (StatusCondition.Poison | StatusCondition.PoisonBad)) != 0)
            return StatusType.Poison;
        if ((value & StatusCondition.Freeze) != 0)
            return StatusType.Freeze;

        return StatusType.None;
    }
}
