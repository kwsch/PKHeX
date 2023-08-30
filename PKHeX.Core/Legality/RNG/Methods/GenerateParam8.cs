namespace PKHeX.Core;

/// <summary>
/// Parameters used to generate data for an encounter.
/// </summary>
/// <param name="Species">Species to generate.</param>
/// <param name="GenderRatio">Gender ratio byte.</param>
/// <param name="FlawlessIVs">Count of IVs that are perfect.</param>
/// <param name="Ability">Ability type to generate.</param>
/// <param name="Shiny">PID generation type.</param>
/// <param name="Nature">Nature specification.</param>
/// <param name="IVs">IV specification.</param>
public readonly record struct GenerateParam8(ushort Species, byte GenderRatio, byte FlawlessIVs,
    AbilityPermission Ability = AbilityPermission.Any12, Shiny Shiny = Shiny.Random,
    Nature Nature = Nature.Random, IndividualValueSet IVs = default);
