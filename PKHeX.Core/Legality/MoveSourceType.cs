using System;

namespace PKHeX.Core;

[Flags]
#pragma warning disable RCS1154 // Sort enum members.
public enum MoveSourceType
#pragma warning restore RCS1154 // Sort enum members.
{
    None,
    LevelUp         = 1 << 0,
    RelearnMoves    = 1 << 1,
    Machine         = 1 << 2,
    TypeTutor       = 1 << 3,
    SpecialTutor    = 1 << 4,
    EnhancedTutor   = 1 << 5,
    SharedEggMove   = 1 << 6,
    TechnicalRecord = 1 << 7,

    AllTutors = TypeTutor | SpecialTutor | EnhancedTutor,
    AllMachines = Machine | TechnicalRecord,

    Reminder = LevelUp | RelearnMoves | TechnicalRecord,
    Encounter = LevelUp | RelearnMoves,
    ExternalSources = Reminder | AllMachines | AllTutors,
    All = ExternalSources | SharedEggMove | RelearnMoves,
}

public static class MoveSourceTypeExtensions
{
    public static bool HasFlagFast(this MoveSourceType value, MoveSourceType flag) => (value & flag) != 0;
    public static MoveSourceType ClearNonEggSources(this MoveSourceType value) => value & MoveSourceType.Encounter;
}
