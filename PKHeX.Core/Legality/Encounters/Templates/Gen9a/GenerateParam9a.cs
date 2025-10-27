namespace PKHeX.Core;

/// <summary>
/// Parameters used to generate data for an encounter.
/// </summary>
/// <param name="GenderRatio">Gender ratio byte.</param>
/// <param name="FlawlessIVs">Count of IVs that are perfect.</param>
/// <param name="RollCount">Count of shiny rolls allowed for the PID calculation.</param>
/// <param name="Shiny">PID generation type.</param>
public readonly record struct GenerateParam9a(byte GenderRatio, byte FlawlessIVs, byte RollCount,
    LumioseCorrelation Correlation = LumioseCorrelation.Normal,
    SizeType9 SizeType = SizeType9.RANDOM, byte Scale = 0, // random
    Nature Nature = Nature.Random,
    AbilityPermission Ability = AbilityPermission.Any12,
    Shiny Shiny = Shiny.Random, IndividualValueSet IVs = default);
