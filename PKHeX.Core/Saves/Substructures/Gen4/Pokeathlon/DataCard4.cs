namespace PKHeX.Core;

/// <summary>
/// Data Card indexes and the stats they retrieve in <see cref="PokeathlonGlobalCounters4"/>.
/// </summary>
public enum DataCard4 : byte
{
    PlacedFirst     = 0, // Pokéathlon 1st Place
    PlacedLast      = 1, // Pokéathlon Last Place
    Dashed          = 2, // Times Pokémon Dashed
    Jumped          = 3, // Times Pokémon Jumped

    FirstHurdle     = 4, // Hurdle Dash 1st Places
    FirstRelay      = 5, // Relay Run 1st Places
    FirstPennant    = 6, // Pennant Capture 1st Places
    FirstBlockSmash = 7, // Block Smash 1st Places
    FirstDiscCatch  = 8, // Disc Catch 1st Places
    FirstSnowThrow  = 9, // Snow Throw 1st Places

    PointsAcquired  = 10, // Pokémon Acquired Points
    Failed          = 11, // Pokémon Failed
    SelfImpeded     = 12, // Times Pokémon Self-Impeded
    Tackled         = 13, // Times Pokémon Tackled
    FellDown        = 14, // Pokémon Fell Down

    FirstRingDrop   = 15, // Ring Drop 1st Places
    FirstLampJump   = 16, // Lamp Jump 1st Places
    FirstCirclePush = 17, // Circle Push 1st Places

    ConnectionFirst = 18, // Connection 1st Places
    ConnectionLast  = 19, // Connection Last Places

    EventFirst      = 20, // Event 1st Places
    EventLast       = 21, // Event Last Places

    Switched        = 22, // Times Pokémon Switched

    FirstGoalRoll   = 23, // Goal Roll 1st Places

    BonusesEarned   = 24, // Bonuses Earned
    Instructions    = 25, // Pokémon Instructions
    TimeSpent       = 26, // Time Spent in Pokéathlon

    Count = 27,
};
