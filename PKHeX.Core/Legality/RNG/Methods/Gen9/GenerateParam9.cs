namespace PKHeX.Core;

/// <summary>
/// Parameters used to generate data for an encounter.
/// </summary>
/// <param name="Species">Species to generate.</param>
/// <param name="GenderRatio">Gender ratio byte.</param>
/// <param name="FlawlessIVs">Count of IVs that are perfect.</param>
/// <param name="RollCount">Count of shiny rolls allowed for the PID calculation.</param>
/// <param name="Height">Height value to generate. If zero, full random.</param>
/// <param name="Weight">Weight value to generate. If zero, full random.</param>
/// <param name="Scale">Scale value to generate. If zero, full random.</param>
/// <param name="Ability">Ability type to generate.</param>
/// <param name="Shiny">PID generation type.</param>
/// <param name="Nature">Nature specification.</param>
/// <param name="IVs">IV specification.</param>
public readonly record struct GenerateParam9(ushort Species, byte GenderRatio, byte FlawlessIVs, byte RollCount, byte Height,
    byte Weight, SizeType9 ScaleType, byte Scale, AbilityPermission Ability = AbilityPermission.Any12, Shiny Shiny = Shiny.Random,
    Nature Nature = Nature.Random, IndividualValueSet IVs = default);
