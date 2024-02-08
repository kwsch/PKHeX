namespace PKHeX.Core;

/// <summary>
/// Result wrapper for encounter lead information.
/// </summary>
public struct LeadSeed(uint Seed, LeadRequired Lead)
{
    public static readonly LeadSeed Invalid = new(default, LeadRequired.Invalid);

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

    public readonly bool IsNoRequirement() => Lead == LeadRequired.None;
    public readonly bool IsNoAbilityLead() => Lead == LeadRequired.None;
    public readonly bool RequiresProcFail() => Lead.HasFlag(LeadRequired.Fail);
    public readonly bool IsValid() => Lead != LeadRequired.Invalid;

    /// <summary>
    /// Prefers the lead with the most likely value (lowest value).
    /// </summary>
    public readonly bool IsBetterThan(in LeadSeed other) => Lead < other.Lead;
}
