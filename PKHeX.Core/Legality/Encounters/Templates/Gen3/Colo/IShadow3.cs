using System;

namespace PKHeX.Core;

/// <summary>
/// Shadow Encounter
/// </summary>
public interface IShadow3
{
    /// <summary>
    /// Sub-Version game differentiator
    /// </summary>
    GameVersion Version { get; }

    /// <summary>
    /// Shadow Pokémon Team Details containing info about team members generated prior to the shadow Pokémon.
    /// </summary>
    ReadOnlyMemory<TeamLock> PartyPrior { get; }
}
