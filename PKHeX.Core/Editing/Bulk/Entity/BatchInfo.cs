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
    public LegalityAnalysis Legality => field ??= new LegalityAnalysis(Entity);

    /// <inheritdoc cref="LegalityAnalysis.Valid"/>
    public bool Legal => Legality.Valid;
}
