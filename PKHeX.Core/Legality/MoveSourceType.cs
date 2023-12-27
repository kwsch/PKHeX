using System;

namespace PKHeX.Core;

/// <summary>
/// Indicates the source of a <see cref="Move"/> for a <see cref="PKM"/>.
/// </summary>
[Flags]
public enum MoveSourceType
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
    Evolve          = 1 << 8,

    AllTutors = TypeTutor | SpecialTutor | EnhancedTutor,
    AllMachines = Machine | TechnicalRecord,

    Reminder = LevelUp | RelearnMoves | TechnicalRecord | Evolve,
    Encounter = LevelUp | RelearnMoves,
    ExternalSources = Reminder | AllMachines | AllTutors,
    All = ExternalSources | SharedEggMove | RelearnMoves,
}

/// <summary>
/// Extensions for <see cref="MoveSourceType"/>.
/// </summary>
public static class MoveSourceTypeExtensions
{
    /// <summary>
    /// Masks out the flags to only leave those that are possible for eggs.
    /// </summary>
    /// <param name="value">Flags to mask.</param>
    /// <returns>Masked flags.</returns>
    public static MoveSourceType ClearNonEggSources(this MoveSourceType value) => value & MoveSourceType.Encounter;
}
