namespace PKHeX.Core
{
    /// <summary> Marks introduced in Generation 8 </summary>
    public interface IRibbonSetMark8
    {
        bool RibbonMarkLunchtime { get; set; }
        bool RibbonMarkSleepyTime { get; set; }
        bool RibbonMarkDusk { get; set; }
        bool RibbonMarkDawn { get; set; }
        bool RibbonMarkCloudy { get; set; }
        bool RibbonMarkRainy { get; set; }
        bool RibbonMarkStormy { get; set; }
        bool RibbonMarkSnowy { get; set; }
        bool RibbonMarkBlizzard { get; set; }
        bool RibbonMarkDry { get; set; }
        bool RibbonMarkSandstorm { get; set; }
        bool RibbonMarkMisty { get; set; }
        bool RibbonMarkDestiny { get; set; }
        bool RibbonMarkFishing { get; set; }
        bool RibbonMarkCurry { get; set; }
        bool RibbonMarkUncommon { get; set; }
        bool RibbonMarkRare { get; set; }
        bool RibbonMarkRowdy { get; set; }
        bool RibbonMarkAbsentMinded { get; set; }
        bool RibbonMarkJittery { get; set; }
        bool RibbonMarkExcited { get; set; }
        bool RibbonMarkCharismatic { get; set; }
        bool RibbonMarkCalmness { get; set; }
        bool RibbonMarkIntense { get; set; }
        bool RibbonMarkZonedOut { get; set; }
        bool RibbonMarkJoyful { get; set; }
        bool RibbonMarkAngry { get; set; }
        bool RibbonMarkSmiley { get; set; }
        bool RibbonMarkTeary { get; set; }
        bool RibbonMarkUpbeat { get; set; }
        bool RibbonMarkPeeved { get; set; }
        bool RibbonMarkIntellectual { get; set; }
        bool RibbonMarkFerocious { get; set; }
        bool RibbonMarkCrafty { get; set; }
        bool RibbonMarkScowling { get; set; }
        bool RibbonMarkKindly { get; set; }
        bool RibbonMarkFlustered { get; set; }
        bool RibbonMarkPumpedUp { get; set; }
        bool RibbonMarkZeroEnergy { get; set; }
        bool RibbonMarkPrideful { get; set; }
        bool RibbonMarkUnsure { get; set; }
        bool RibbonMarkHumble { get; set; }
        bool RibbonMarkThorny { get; set; }
        bool RibbonMarkVigor { get; set; }
        bool RibbonMarkSlump { get; set; }

        bool HasMark();
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesMark8 =
        {
            nameof(IRibbonSetMark8.RibbonMarkLunchtime),
            nameof(IRibbonSetMark8.RibbonMarkSleepyTime),
            nameof(IRibbonSetMark8.RibbonMarkDusk),
            nameof(IRibbonSetMark8.RibbonMarkDawn),
            nameof(IRibbonSetMark8.RibbonMarkCloudy),
            nameof(IRibbonSetMark8.RibbonMarkRainy),
            nameof(IRibbonSetMark8.RibbonMarkStormy),
            nameof(IRibbonSetMark8.RibbonMarkSnowy),
            nameof(IRibbonSetMark8.RibbonMarkBlizzard),
            nameof(IRibbonSetMark8.RibbonMarkDry),
            nameof(IRibbonSetMark8.RibbonMarkSandstorm),
            nameof(IRibbonSetMark8.RibbonMarkMisty),
            nameof(IRibbonSetMark8.RibbonMarkDestiny),
            nameof(IRibbonSetMark8.RibbonMarkFishing),
            nameof(IRibbonSetMark8.RibbonMarkCurry),
            nameof(IRibbonSetMark8.RibbonMarkUncommon),
            nameof(IRibbonSetMark8.RibbonMarkRare),
            nameof(IRibbonSetMark8.RibbonMarkRowdy),
            nameof(IRibbonSetMark8.RibbonMarkAbsentMinded),
            nameof(IRibbonSetMark8.RibbonMarkJittery),
            nameof(IRibbonSetMark8.RibbonMarkExcited),
            nameof(IRibbonSetMark8.RibbonMarkCharismatic),
            nameof(IRibbonSetMark8.RibbonMarkCalmness),
            nameof(IRibbonSetMark8.RibbonMarkIntense),
            nameof(IRibbonSetMark8.RibbonMarkZonedOut),
            nameof(IRibbonSetMark8.RibbonMarkJoyful),
            nameof(IRibbonSetMark8.RibbonMarkAngry),
            nameof(IRibbonSetMark8.RibbonMarkSmiley),
            nameof(IRibbonSetMark8.RibbonMarkTeary),
            nameof(IRibbonSetMark8.RibbonMarkUpbeat),
            nameof(IRibbonSetMark8.RibbonMarkPeeved),
            nameof(IRibbonSetMark8.RibbonMarkIntellectual),
            nameof(IRibbonSetMark8.RibbonMarkFerocious),
            nameof(IRibbonSetMark8.RibbonMarkCrafty),
            nameof(IRibbonSetMark8.RibbonMarkScowling),
            nameof(IRibbonSetMark8.RibbonMarkKindly),
            nameof(IRibbonSetMark8.RibbonMarkFlustered),
            nameof(IRibbonSetMark8.RibbonMarkPumpedUp),
            nameof(IRibbonSetMark8.RibbonMarkZeroEnergy),
            nameof(IRibbonSetMark8.RibbonMarkPrideful),
            nameof(IRibbonSetMark8.RibbonMarkUnsure),
            nameof(IRibbonSetMark8.RibbonMarkHumble),
            nameof(IRibbonSetMark8.RibbonMarkThorny),
            nameof(IRibbonSetMark8.RibbonMarkVigor),
            nameof(IRibbonSetMark8.RibbonMarkSlump),
        };

        internal static bool[] RibbonBits(this IRibbonSetMark8 set)
        {
            return new[]
            {
                set.RibbonMarkLunchtime,
                set.RibbonMarkSleepyTime,
                set.RibbonMarkDusk,
                set.RibbonMarkDawn,
                set.RibbonMarkCloudy,
                set.RibbonMarkRainy,
                set.RibbonMarkStormy,
                set.RibbonMarkSnowy,
                set.RibbonMarkBlizzard,
                set.RibbonMarkDry,
                set.RibbonMarkSandstorm,
                set.RibbonMarkMisty,
                set.RibbonMarkDestiny,
                set.RibbonMarkFishing,
                set.RibbonMarkCurry,
                set.RibbonMarkUncommon,
                set.RibbonMarkRare,
                set.RibbonMarkRowdy,
                set.RibbonMarkAbsentMinded,
                set.RibbonMarkJittery,
                set.RibbonMarkExcited,
                set.RibbonMarkCharismatic,
                set.RibbonMarkCalmness,
                set.RibbonMarkIntense,
                set.RibbonMarkZonedOut,
                set.RibbonMarkJoyful,
                set.RibbonMarkAngry,
                set.RibbonMarkSmiley,
                set.RibbonMarkTeary,
                set.RibbonMarkUpbeat,
                set.RibbonMarkPeeved,
                set.RibbonMarkIntellectual,
                set.RibbonMarkFerocious,
                set.RibbonMarkCrafty,
                set.RibbonMarkScowling,
                set.RibbonMarkKindly,
                set.RibbonMarkFlustered,
                set.RibbonMarkPumpedUp,
                set.RibbonMarkZeroEnergy,
                set.RibbonMarkPrideful,
                set.RibbonMarkUnsure,
                set.RibbonMarkHumble,
                set.RibbonMarkThorny,
                set.RibbonMarkVigor,
                set.RibbonMarkSlump,
            };
        }

        internal static string[] RibbonNames(this IRibbonSetMark8 _) => RibbonSetNamesMark8;
    }
}