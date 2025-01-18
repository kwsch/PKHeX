namespace PKHeX.Core;

/// <summary>
/// List of Pokewalker Courses for HeartGold/SoulSilver
/// </summary>
public enum PokewalkerCourse4 : byte
{
    RefreshingField = 0,
    NoisyForest = 1,
    RuggedRoad = 2,
    BeautifulBeach = 3,
    SuburbanArea = 4,
    DimCave = 5,
    BlueLake = 6,
    TownOutskirts = 7,
    HoennField = 8,
    WarmBeach = 9,
    VolcanoPath = 10,
    Treehouse = 11,
    ScaryCave = 12,
    SinnohField = 13,
    IcyMountainRoad = 14,
    BigForest = 15,
    WhiteLake = 16,
    StormyBeach = 17,
    Resort = 18,
    QuietCave = 19,
    BeyondTheSea = 20,
    NightSkysEdge = 21,
    YellowForest = 22,

    // A Pokéwalker can send Pokémon to a copy of HG/SS it is not paired with.
    // The captured Pokémon are not locked to the mentioned language, but the course unlock flag is.
    Rally = 23, // JPN Exclusive
    Sightseeing = 24, // JPN/KOR Exclusive
    WinnersPath = 25,
    AmityMeadow = 26, // JPN Exclusive

    MAX_COUNT = 27,
}
