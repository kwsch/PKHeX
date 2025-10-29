namespace PKHeX.Core;

/// <summary>
/// Defines the correlation types for Legends: Z-A encounter RNG patterns.
/// </summary>
public enum LumioseCorrelation : byte
{
    /// <summary>
    /// RNG generation pattern is as expected, where all values enter FixInitSpec without any being already set or overwritten after generation.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Discards the initially generated IVs and reapplies them after FixInitSpec with a separate (unrelated) RNG seed.
    /// </summary>
    ReApplyIVs = 1,

    /// <summary>
    /// Prepares the IVs before entering FixInitSpec by deciding a quantity of flawless IV indexes first with a separate (unrelated) RNG seed.
    /// </summary>
    PreApplyIVs = 2,

    /// <summary>
    /// No fake trainer ID is generated; uses the original trainer ID as-is due to it already being provided to FixInitSpec.
    /// </summary>
    SkipTrainer = 3,
}
