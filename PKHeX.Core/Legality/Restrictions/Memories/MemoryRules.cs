using System;

namespace PKHeX.Core;

/// <summary>
/// Rules for memory mutation across games.
/// </summary>
public static class MemoryRules
{
    /// <summary>
    /// Get the possible sources of memories for a given <see cref="EvolutionHistory"/>.
    /// </summary>
    /// <returns>A <see cref="MemorySource"/> flag.</returns>
    public static MemorySource GetPossibleSources(EvolutionHistory history)
    {
        var sources = MemorySource.None;
        if (history.HasVisitedGen6)
            sources |= MemorySource.Gen6 | MemorySource.Bank;
        if (history.HasVisitedGen7)
            sources |= MemorySource.Bank; // Trade encounters from Gen7 also come with hardcoded memories.
        if (history.HasVisitedSWSH)
            sources |= MemorySource.Gen8;
        if (history.HasVisitedGen9)
            sources |= MemorySource.Deleted;
        return sources;
    }

    /// <summary>
    /// Revise the possible sources of memories for a given <see cref="PKM"/> and <see cref="MemorySource"/> flag.
    /// </summary>
    /// <param name="pk">Entity to check.</param>
    /// <param name="sources">Possible sources of memories.</param>
    /// <param name="enc">Encounter matched to.</param>
    /// <returns>Revised <see cref="MemorySource"/> flag.</returns>
    public static MemorySource ReviseSourcesHandler(PKM pk, MemorySource sources, IEncounterTemplate enc)
    {
        // No HT Name => no HT Memory
        if (pk.IsUntraded)
        {
            if (enc is { Context: EntityContext.Gen8, EggEncounter: true } && pk is { Context: EntityContext.Gen8, Met_Location: Locations.LinkTrade6 }) // Applies HT memory without HT details
                return sources;
            return MemorySource.None;
        }

        // Any Gen6 or Bank specific memory on Switch must have no HT language or else it would be replaced/erased.
        if (pk is IHandlerLanguage { HT_Language: not 0 }) // Gen8+ Memory Required
            sources &= ~(MemorySource.Gen6 | MemorySource.Bank);

        return sources;
    }
}

/// <summary>
/// Possible sources of memories.
/// </summary>
[Flags]
public enum MemorySource
{
    None,
    Gen6 = 1 << 0,
    Bank = 1 << 1,
    Gen8 = 1 << 2,
    Deleted = 1 << 3,
}
