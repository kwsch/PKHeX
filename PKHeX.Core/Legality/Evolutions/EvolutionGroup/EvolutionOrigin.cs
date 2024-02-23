using System;

namespace PKHeX.Core;

/// <summary>
/// Details about the original encounter.
/// </summary>
/// <param name="Species">Species the encounter originated as</param>
/// <param name="Version">Version the encounter originated on</param>
/// <param name="Generation">Generation the encounter originated in</param>
/// <param name="LevelMin">Minimum level the encounter originated at</param>
/// <param name="LevelMax">Maximum level in final state</param>
/// <param name="Options">Options to toggle logic when using this data</param>
public readonly record struct EvolutionOrigin(ushort Species, GameVersion Version, byte Generation, byte LevelMin, byte LevelMax, OriginOptions Options = 0)
{
    /// <summary>
    /// Checks if evolution checks against the Entity should be skipped when devolving or devolving.
    /// </summary>
    public bool SkipChecks => Options.HasFlag(OriginOptions.SkipChecks);

    /// <summary>
    /// Internally used to enforce Gen1 origin encounters NOT jumping to Gen2 to continue devolving.
    /// </summary>
    public bool IsDiscardRequired(byte format) => format <= 2 && Options.HasFlag(OriginOptions.ForceDiscard);
}

[Flags]
public enum OriginOptions : byte
{
    None = 0,
    SkipChecks = 1 << 0,
    ForceDiscard = 1 << 1,

    EncounterTemplate = SkipChecks | ForceDiscard,
}
