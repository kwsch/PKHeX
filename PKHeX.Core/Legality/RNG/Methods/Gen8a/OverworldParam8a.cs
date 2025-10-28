namespace PKHeX.Core;

/// <summary>
/// Parameters used to generate data for an encounter.
/// </summary>
/// <param name="IsAlpha">The encounter is an Alpha Pokémon.</param>
/// <param name="GenderRatio">Gender ratio byte.</param>
/// <param name="FlawlessIVs">Count of IVs that are perfect.</param>
/// <param name="RollCount">Count of shiny rolls allowed for the PID calculation.</param>
/// <param name="Shiny">PID generation type.</param>
public readonly record struct OverworldParam8a(bool IsAlpha, byte GenderRatio, byte FlawlessIVs, byte RollCount, Shiny Shiny = Shiny.Random);
