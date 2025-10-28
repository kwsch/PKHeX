using System;

namespace PKHeX.Core;

public interface ISuperTrainRegimen : ISuperTrain
{
    public bool SuperTrain1_HP  { get; set; }
    public bool SuperTrain1_ATK { get; set; }
    public bool SuperTrain1_DEF { get; set; }
    public bool SuperTrain1_SPA { get; set; }
    public bool SuperTrain1_SPD { get; set; }
    public bool SuperTrain1_SPE { get; set; }
    public bool SuperTrain2_HP  { get; set; }
    public bool SuperTrain2_ATK { get; set; }
    public bool SuperTrain2_DEF { get; set; }
    public bool SuperTrain2_SPA { get; set; }
    public bool SuperTrain2_SPD { get; set; }
    public bool SuperTrain2_SPE { get; set; }
    public bool SuperTrain3_HP  { get; set; }
    public bool SuperTrain3_ATK { get; set; }
    public bool SuperTrain3_DEF { get; set; }
    public bool SuperTrain3_SPA { get; set; }
    public bool SuperTrain3_SPD { get; set; }
    public bool SuperTrain3_SPE { get; set; }
    public bool SuperTrain4_1   { get; set; }
    public bool SuperTrain5_1   { get; set; }
    public bool SuperTrain5_2   { get; set; }
    public bool SuperTrain5_3   { get; set; }
    public bool SuperTrain5_4   { get; set; }
    public bool SuperTrain6_1   { get; set; }
    public bool SuperTrain6_2   { get; set; }
    public bool SuperTrain6_3   { get; set; }
    public bool SuperTrain7_1   { get; set; }
    public bool SuperTrain7_2   { get; set; }
    public bool SuperTrain7_3   { get; set; }
    public bool SuperTrain8_1   { get; set; }
    public bool DistSuperTrain1 { get; set; }
    public bool DistSuperTrain2 { get; set; }
    public bool DistSuperTrain3 { get; set; }
    public bool DistSuperTrain4 { get; set; }
    public bool DistSuperTrain5 { get; set; }
    public bool DistSuperTrain6 { get; set; }
}

public static class SuperTrainRegimenExtensions
{
    public const int CountRegimen = 30;
    public const int CountRegimenDistribution = 6;

    public static bool GetRegimenState(this ISuperTrainRegimen sr, int index) => (uint)index switch
    {
        00 => sr.SuperTrain1_HP,
        01 => sr.SuperTrain1_ATK,
        02 => sr.SuperTrain1_DEF,
        03 => sr.SuperTrain1_SPA,
        04 => sr.SuperTrain1_SPD,
        05 => sr.SuperTrain1_SPE,

        06 => sr.SuperTrain2_HP,
        07 => sr.SuperTrain2_ATK,
        08 => sr.SuperTrain2_DEF,
        09 => sr.SuperTrain2_SPA,
        10 => sr.SuperTrain2_SPD,
        11 => sr.SuperTrain2_SPE,

        12 => sr.SuperTrain3_HP,
        13 => sr.SuperTrain3_ATK,
        14 => sr.SuperTrain3_DEF,
        15 => sr.SuperTrain3_SPA,
        16 => sr.SuperTrain3_SPD,
        17 => sr.SuperTrain3_SPE,

        18 => sr.SuperTrain4_1,

        19 => sr.SuperTrain5_1,
        20 => sr.SuperTrain5_2,
        21 => sr.SuperTrain5_3,
        22 => sr.SuperTrain5_4,

        23 => sr.SuperTrain6_1,
        24 => sr.SuperTrain6_2,
        25 => sr.SuperTrain6_3,

        26 => sr.SuperTrain7_1,
        27 => sr.SuperTrain7_2,
        28 => sr.SuperTrain7_3,

        29 => sr.SuperTrain8_1,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public static bool SetRegimenState(this ISuperTrainRegimen sr, int index, bool value) => (uint)index switch
    {
        00 => sr.SuperTrain1_HP = value,
        01 => sr.SuperTrain1_ATK = value,
        02 => sr.SuperTrain1_DEF = value,
        03 => sr.SuperTrain1_SPA = value,
        04 => sr.SuperTrain1_SPD = value,
        05 => sr.SuperTrain1_SPE = value,

        06 => sr.SuperTrain2_HP = value,
        07 => sr.SuperTrain2_ATK = value,
        08 => sr.SuperTrain2_DEF = value,
        09 => sr.SuperTrain2_SPA = value,
        10 => sr.SuperTrain2_SPD = value,
        11 => sr.SuperTrain2_SPE = value,

        12 => sr.SuperTrain3_HP = value,
        13 => sr.SuperTrain3_ATK = value,
        14 => sr.SuperTrain3_DEF = value,
        15 => sr.SuperTrain3_SPA = value,
        16 => sr.SuperTrain3_SPD = value,
        17 => sr.SuperTrain3_SPE = value,

        18 => sr.SuperTrain4_1 = value,

        19 => sr.SuperTrain5_1 = value,
        20 => sr.SuperTrain5_2 = value,
        21 => sr.SuperTrain5_3 = value,
        22 => sr.SuperTrain5_4 = value,

        23 => sr.SuperTrain6_1 = value,
        24 => sr.SuperTrain6_2 = value,
        25 => sr.SuperTrain6_3 = value,

        26 => sr.SuperTrain7_1 = value,
        27 => sr.SuperTrain7_2 = value,
        28 => sr.SuperTrain7_3 = value,

        29 => sr.SuperTrain8_1 = value,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public static string GetRegimenName(int index) => (uint)index switch
    {
        00 => nameof(ISuperTrainRegimen.SuperTrain1_HP),
        01 => nameof(ISuperTrainRegimen.SuperTrain1_ATK),
        02 => nameof(ISuperTrainRegimen.SuperTrain1_DEF),
        03 => nameof(ISuperTrainRegimen.SuperTrain1_SPA),
        04 => nameof(ISuperTrainRegimen.SuperTrain1_SPD),
        05 => nameof(ISuperTrainRegimen.SuperTrain1_SPE),

        06 => nameof(ISuperTrainRegimen.SuperTrain2_HP),
        07 => nameof(ISuperTrainRegimen.SuperTrain2_ATK),
        08 => nameof(ISuperTrainRegimen.SuperTrain2_DEF),
        09 => nameof(ISuperTrainRegimen.SuperTrain2_SPA),
        10 => nameof(ISuperTrainRegimen.SuperTrain2_SPD),
        11 => nameof(ISuperTrainRegimen.SuperTrain2_SPE),

        12 => nameof(ISuperTrainRegimen.SuperTrain3_HP),
        13 => nameof(ISuperTrainRegimen.SuperTrain3_ATK),
        14 => nameof(ISuperTrainRegimen.SuperTrain3_DEF),
        15 => nameof(ISuperTrainRegimen.SuperTrain3_SPA),
        16 => nameof(ISuperTrainRegimen.SuperTrain3_SPD),
        17 => nameof(ISuperTrainRegimen.SuperTrain3_SPE),

        18 => nameof(ISuperTrainRegimen.SuperTrain4_1),

        19 => nameof(ISuperTrainRegimen.SuperTrain5_1),
        20 => nameof(ISuperTrainRegimen.SuperTrain5_2),
        21 => nameof(ISuperTrainRegimen.SuperTrain5_3),
        22 => nameof(ISuperTrainRegimen.SuperTrain5_4),

        23 => nameof(ISuperTrainRegimen.SuperTrain6_1),
        24 => nameof(ISuperTrainRegimen.SuperTrain6_2),
        25 => nameof(ISuperTrainRegimen.SuperTrain6_3),

        26 => nameof(ISuperTrainRegimen.SuperTrain7_1),
        27 => nameof(ISuperTrainRegimen.SuperTrain7_2),
        28 => nameof(ISuperTrainRegimen.SuperTrain7_3),

        29 => nameof(ISuperTrainRegimen.SuperTrain8_1),
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public static bool GetRegimenStateDistribution(this ISuperTrainRegimen sr, int index) => (uint)index switch
    {
        0 => sr.DistSuperTrain1,
        1 => sr.DistSuperTrain2,
        2 => sr.DistSuperTrain3,
        3 => sr.DistSuperTrain4,
        4 => sr.DistSuperTrain5,
        5 => sr.DistSuperTrain6,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public static bool SetRegimenStateDistribution(this ISuperTrainRegimen sr, int index, bool value) => (uint)index switch
    {
        0 => sr.DistSuperTrain1 = value,
        1 => sr.DistSuperTrain2 = value,
        2 => sr.DistSuperTrain3 = value,
        3 => sr.DistSuperTrain4 = value,
        4 => sr.DistSuperTrain5 = value,
        5 => sr.DistSuperTrain6 = value,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public static string GetRegimenNameDistribution(int index) => (uint)index switch
    {
        0 => nameof(ISuperTrainRegimen.DistSuperTrain1),
        1 => nameof(ISuperTrainRegimen.DistSuperTrain2),
        2 => nameof(ISuperTrainRegimen.DistSuperTrain3),
        3 => nameof(ISuperTrainRegimen.DistSuperTrain4),
        4 => nameof(ISuperTrainRegimen.DistSuperTrain5),
        5 => nameof(ISuperTrainRegimen.DistSuperTrain6),
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };
}
