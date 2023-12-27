namespace PKHeX.Core;

/// <summary>
/// Exposes contest stat value setters.
/// </summary>
public interface IContestStats : IContestStatsReadOnly
{
    new byte CNT_Cool   { get; set; }
    new byte CNT_Beauty { get; set; }
    new byte CNT_Cute   { get; set; }
    new byte CNT_Smart  { get; set; }
    new byte CNT_Tough  { get; set; }
    new byte CNT_Sheen  { get; set; }
}
