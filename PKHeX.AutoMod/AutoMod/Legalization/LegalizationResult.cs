namespace PKHeX.Core.AutoMod;

/// <summary>
/// Indication for how a <see cref="PKM"/> is/was legalized.
/// </summary>
public enum LegalizationResult
{
    /// <summary>
    /// Successfully regenerated from <see cref="IEncounterTemplate"/> data.
    /// </summary>
    Regenerated,

    /// <summary>
    /// Failed to generate.
    /// </summary>
    Failed,

    /// <summary>
    /// Timed out while generating
    /// </summary>
    Timeout,

    /// <summary>
    /// Version mismatch with base PKHeX
    /// </summary>
    VersionMismatch,
}
