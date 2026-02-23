namespace PKHeX.Core;

/// <summary>
/// Information wrapper used for Batch Editing to apply suggested values.
/// </summary>
/// <param name="Entity"> Entity to be modified. </param>
public sealed record BatchInfo(PKM Entity)
{
    /// <summary>
    /// Legality analysis of the entity.
    /// </summary>
    /// <remarks>
    /// Eagerly evaluate on ctor, so that the initial state is remembered before any modifications may disturb matching.
    /// </remarks>
    public readonly LegalityAnalysis Legality = new(Entity);

    /// <inheritdoc cref="LegalityAnalysis.Valid"/>
    public bool Legal => Legality.Valid;
}
