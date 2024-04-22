using static PKHeX.Core.LeadRequired;

namespace PKHeX.Core;

/// <summary>
/// Result wrapper for encounter lead information.
/// </summary>
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

    public readonly bool IsNoRequirement() => Lead == None;
    public readonly bool IsNoAbilityLead() => Lead == None;
    public readonly bool IsValid() => Lead != Invalid;

    /// <summary>
    /// Prefers the lead with the most likely value (lowest value).
    /// </summary>
    public readonly bool IsBetterThan(LeadSeed other) => Lead > other.Lead;
}
