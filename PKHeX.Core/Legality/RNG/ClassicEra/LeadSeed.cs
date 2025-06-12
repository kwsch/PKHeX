using static PKHeX.Core.LeadRequired;

namespace PKHeX.Core;

/// <summary>
/// Result wrapper for Player's party lead and encounter seed information.
/// </summary>
/// <remarks>
/// Mutable struct to allow for some ref mutations, like shifting the seed to an earlier frame, to not require passing the entire struct.
/// </remarks>
public struct LeadSeed(uint Seed, LeadRequired Lead)
{
    /// <summary>
    /// Seed the encounter was triggered from.
    /// </summary>
    public uint Seed = Seed;

    /// <summary>
    /// Lead condition required for the encounter.
    /// </summary>
    public LeadRequired Lead = Lead;

    public readonly void Deconstruct(out uint seed, out LeadRequired lead)
    {
        seed = Seed;
        lead = Lead;
    }

    /// <summary>
    /// Checks if the player's lead Pokémon ability is not one of the abilities that impacts encounter generation.
    /// </summary>
    /// <remarks>
    /// Most often, the player's lead Pokémon does not have an ability that affects the encounter.
    /// </remarks>
    public readonly bool IsNoRequirement() => Lead == None;

    /// <summary>
    /// Checks if the player's lead Pokémon ability is recognized as a valid lead condition.
    /// </summary>
    /// <remarks>
    /// Syntax sugar for checking it is not <see cref="Invalid"/>, in the event logic needs to be extended in the future.
    /// </remarks>
    public readonly bool IsValid() => Lead != Invalid;

    /// <summary>
    /// Prefers the lead with the most likely value (lowest value).
    /// </summary>
    public readonly bool IsBetterThan(LeadSeed other) => Lead > other.Lead;
}
