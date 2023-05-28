namespace PKHeX.Core;

/// <summary>
/// Details about the original encounter.
/// </summary>
/// <param name="Species">Species the encounter originated as</param>
/// <param name="Version">Version the encounter originated on</param>
/// <param name="Generation">Generation the encounter originated in</param>
/// <param name="LevelMin">Minimum level the encounter originated at</param>
/// <param name="LevelMax">Maximum level in final state</param>
/// <param name="SkipChecks">Skip enforcement of legality for evolution criteria</param>
public readonly record struct EvolutionOrigin(ushort Species, byte Version, byte Generation, byte LevelMin, byte LevelMax, bool SkipChecks = false);
