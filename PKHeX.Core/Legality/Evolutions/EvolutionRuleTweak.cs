namespace PKHeX.Core;

/// <summary>
/// Tweaks to Evolution rules to account for game-specific behaviors.
/// </summary>
public sealed class EvolutionRuleTweak
{
    /// <summary>
    /// Default Evolution logic (no tweaks).
    /// </summary>
    public static readonly EvolutionRuleTweak Default = new();

    /// <summary>
    /// In Scarlet &amp; Violet, level 100 Pokemon can trigger evolution methods via Rare Candy level up.
    /// </summary>
    public static readonly EvolutionRuleTweak Level100 = new() { AllowLevelUpEvolution100 = true };

    /// <summary>
    /// Allow Level Up Evolutions to trigger if already level 100.
    /// </summary>
    public bool AllowLevelUpEvolution100 { get; init; }
}
