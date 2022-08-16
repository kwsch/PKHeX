namespace PKHeX.Core;

/// <summary>
/// Wraps details used for parsing ribbon states.
/// </summary>
/// <param name="Entity">Entity to parse</param>
/// <param name="Encounter">Encounter originated as</param>
/// <param name="History">History of visitation</param>
/// <remarks>For Generation 1/2 encounters, use the transferred encounter data object.</remarks>
public readonly record struct RibbonVerifierArguments(PKM Entity, IEncounterTemplate Encounter, EvolutionHistory History);
