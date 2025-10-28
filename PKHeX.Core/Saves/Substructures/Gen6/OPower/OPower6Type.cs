namespace PKHeX.Core;

public enum OPower6Index : byte
{
    Enable = 0,

    Hatching1 = 1,
    Hatching2 = 2,
    Hatching3 = 3,
    HatchingS = 4,
    HatchingMAX = 5,

    Bargain1 = 6,
    Bargain2 = 7,
    Bargain3 = 8,
    BargainS = 9,
    BargainMAX = 10,

    PrizeMoney1 = 11,
    PrizeMoney2 = 12,
    PrizeMoney3 = 13,
    PrizeMoneyS = 14,
    PrizeMoneyMAX = 15,

    Experience1 = 16,
    Experience2 = 17,
    Experience3 = 18,
    ExperienceS = 19,
    ExperienceMAX = 20,

    Capture1 = 21,
    Capture2 = 22,
    Capture3 = 23,
    CaptureS = 24,
    CaptureMAX = 25,

    Encounter1 = 26,
    Encounter2 = 27,
    Encounter3 = 28,

    Stealth1 = 29,
    Stealth2 = 30,
    Stealth3 = 31,

    HPRestoring1 = 32,
    HPRestoring2 = 33,
    HPRestoring3 = 34,

    PPRestoring1 = 35,
    PPRestoring2 = 36,
    PPRestoring3 = 37,

    FullRecovery = 38,

    Befriending1 = 39,
    Befriending2 = 40,
    Befriending3 = 41,
    BefriendingS = 42,
    BefriendingMAX = 43,

    Attack1 = 44,
    Attack2 = 45,
    Attack3 = 46,

    Defense1 = 47,
    Defense2 = 48,
    Defense3 = 49,

    SpecialAttack1 = 50,
    SpecialAttack2 = 51,
    SpecialAttack3 = 52,

    SpecialDefense1 = 53,
    SpecialDefense2 = 54,
    SpecialDefense3 = 55,

    Speed1 = 56,
    Speed2 = 57,
    Speed3 = 58,

    Critical1 = 59,
    Critical2 = 60,
    Critical3 = 61,

    Accuracy1 = 62,
    Accuracy2 = 63,
    Accuracy3 = 64,

    Count = 65,
}

public static class OPowerTypeExtensions
{
    public static OPower6FieldType GetFieldType(this OPower6Index index) => index switch
    {
        0 => OPower6FieldType.Count, // Invalid
        <= OPower6Index.HatchingMAX    => OPower6FieldType.Hatching,
        <= OPower6Index.BargainMAX     => OPower6FieldType.Bargain,
        <= OPower6Index.PrizeMoneyMAX  => OPower6FieldType.PrizeMoney,
        <= OPower6Index.ExperienceMAX  => OPower6FieldType.Experience,
        <= OPower6Index.CaptureMAX     => OPower6FieldType.Capture,
        <= OPower6Index.Encounter3     => OPower6FieldType.Encounter,
        <= OPower6Index.Stealth3       => OPower6FieldType.Stealth,
        <= OPower6Index.HPRestoring3   => OPower6FieldType.HPRestoring,
        <= OPower6Index.PPRestoring3   => OPower6FieldType.PPRestoring,
           OPower6Index.FullRecovery   => OPower6FieldType.Count, // Invalid
        <= OPower6Index.BefriendingMAX => OPower6FieldType.Befriending,
        _ => OPower6FieldType.Count, // Invalid
    };

    public static OPower6BattleType GetBattleType(this OPower6Index index) => index switch
    {
        >= OPower6Index.Attack1 and <= OPower6Index.Accuracy3 => (OPower6BattleType)((index - OPower6Index.Attack1) / 3),
        _ => OPower6BattleType.Count, // Invalid
    };
}
