namespace PKHeX.Core;

/// <summary>
/// Program generated report used to summarize the <see cref="GameVersion.PLA"/> Pokédex state.
/// </summary>
public sealed record PokedexUpdateInfo8a
{
    public int ProgressPokeNum { get; init; }
    public int ProgressNum { get; init; }
    public int PointsGainedFromProgressPoke { get; init; }
    public int NewCompleteResearchNum { get; init; }
    public int PointsGainedFromCompleteResearch { get; init; }
    public int TotalResearchPointBeforeUpdate { get; init; }
    public int TotalResearchPointAfterUpdate { get; init; }
    public int RankBeforeUpdate { get; init; }
    public int PointsNeededForNextRankBeforeUpdate { get; init; }
    public int PointsNeededForNextRankAfterUpdate { get; init; }
    public int ProgressPercentToNextRankBeforeUpdate { get; init; }
    public int ProgressPercentToNextRankAfterUpdate { get; init; }
    public int TotalResearchPointAfterUpdate_Duplicate { get; init; }
}
