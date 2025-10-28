namespace PKHeX.Core;

/// <summary>
/// Information wrapper used for Batch Editing to apply suggested values.
/// </summary>
/// <param name="Entity"> Entity to be modified. </param>
public sealed record BatchInfo(PKM Entity)
{
    private LegalityAnalysis? la; // c# 14 replace with get-field

    /// <summary>
    /// Legality analysis of the entity.
    /// </summary>
    public LegalityAnalysis Legality => la ??= new LegalityAnalysis(Entity);

    /// <inheritdoc cref="LegalityAnalysis.Valid"/>
    public bool Legal => Legality.Valid;
}
