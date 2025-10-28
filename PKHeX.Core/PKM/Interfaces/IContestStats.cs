namespace PKHeX.Core;

/// <summary>
/// Exposes contest stat value setters.
/// </summary>
public interface IContestStats : IContestStatsReadOnly
{
    new byte ContestCool   { get; set; }
    new byte ContestBeauty { get; set; }
    new byte ContestCute   { get; set; }
    new byte ContestSmart  { get; set; }
    new byte ContestTough  { get; set; }
    new byte ContestSheen  { get; set; }
}
