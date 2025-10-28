namespace PKHeX.Core;

/// <summary>
/// Logic to calculate max counts denoted by the ranch level in My Pokémon Ranch
/// </summary>
public static class RanchLevel
{
    public static int GetMaxMiiCount(int ranchLevel) => ranchLevel switch
    {
        >= 11 => 20,
        >= 08 => 15,
        >= 04 => 10,
        _ => 5,
    };

    public static int GetMaxToyCount(int ranchLevel) => ranchLevel switch
    {
        >= 25 => 6,
        >= 20 => 5,
        >= 15 => 4,
        >= 11 => 3,
        >= 08 => 2,
        _ => 1,
    };

    public static int GetSlotCount(int ranchLevel) => ranchLevel switch
    {
        01 => 020,
        02 => 025,
        03 => 030,

        04 => 040,
        05 => 050,
        06 => 060,

        07 => 080,
        08 => 100,

        09 => 150,
        10 => 200,
        11 => 250,
        12 => 300,
        13 => 350,

        14 => 400,
        15 => 500,
        16 => 600,
        17 => 700,
        18 => 800,
        19 => 900,
        20 => 1000,

        21 => 1000,
        22 => 1000,
        23 => 1000,
        24 => 1000,
        25 => 1000,

        _ => 1500,
    };
}
