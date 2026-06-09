namespace PKHeX.Core;

/// <summary>
/// Enumeration of the 10 different Pokeathlon events in Gen 4, used for indexing into the event data structures.
/// </summary>
public enum PokeathlonEvent4 : byte
{
    HurdleDash = 0,
    PennantCapture = 1,
    CirclePush = 2,
    BlockSmash = 3,
    DiscCatch = 4,
    LampJump = 5,
    RelayRun = 6,
    RingDrop = 7,
    SnowThrow = 8,
    GoalRoll = 9,

    Count = 10,
}
