namespace PKHeX.Core;

/// <summary>
/// Logic to calculate max counts denoted by the ranch level in My Pokemon Ranch
/// </summary>
public static class RanchLevel
{
    public static int GetLevel(byte levelIndex) => levelIndex + 1;

    public static int GetMaxMiis(byte levelIndex) => levelIndex switch
    {
        >= 11 => 20,
        >= 08 => 15,
        >= 04 => 10,
        _ => 5,
    };

    public static int GetMaxToys(byte levelIndex) => levelIndex switch
    {
        >= 25 => 6,
        >= 20 => 5,
        >= 15 => 4,
        >= 11 => 3,
        >= 08 => 2,
        _ => 1,
    };

    public static int GetSlotCount(byte levelIndex) => levelIndex switch
    {
        00 => 020,
        01 => 025,
        02 => 030,
        03 => 040,
        04 => 050,
        05 => 060,
        06 => 080,

        07 => 100,
        08 => 150,
        09 => 200,
        10 => 250,
        11 => 300,
        12 => 350,

        13 => 400,
        14 => 500,
        15 => 600,
        16 => 700,
        17 => 800,
        18 => 900,
        19 => 1000,

        20 => 1000,
        21 => 1000,
        22 => 1000,
        23 => 1000,
        24 => 1000,

        _ => 1500,
    };
}
