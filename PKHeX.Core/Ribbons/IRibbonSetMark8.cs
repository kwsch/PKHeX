using System;

namespace PKHeX.Core;

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

    bool HasMarkEncounter8 { get; }
}

public interface IRibbonSetRibbons
{
    int RibbonCount { get; }
}

public interface IRibbonSetMarks
{
    int MarkCount { get; }
    int RibbonMarkCount { get; }
}

public static partial class RibbonExtensions
{
    extension(IRibbonSetMark8 m)
    {
        public bool HasWeatherMark(out RibbonIndex ribbon)
        {
            if (m.RibbonMarkCloudy) { ribbon = RibbonIndex.MarkCloudy; return true; }
            if (m.RibbonMarkRainy) { ribbon = RibbonIndex.MarkRainy; return true; }
            if (m.RibbonMarkStormy) { ribbon = RibbonIndex.MarkStormy; return true; }
            if (m.RibbonMarkSnowy) { ribbon = RibbonIndex.MarkSnowy; return true; }
            if (m.RibbonMarkBlizzard) { ribbon = RibbonIndex.MarkBlizzard; return true; }
            if (m.RibbonMarkDry) { ribbon = RibbonIndex.MarkDry; return true; }
            if (m.RibbonMarkSandstorm) { ribbon = RibbonIndex.MarkSandstorm; return true; }
            if (m.RibbonMarkMisty) { ribbon = RibbonIndex.MarkMisty; return true; }
            ribbon = default;
            return false;
        }

        public void CopyRibbonSetMark8(IRibbonSetMark8 dest)
        {
            dest.RibbonMarkLunchtime = m.RibbonMarkLunchtime;
            dest.RibbonMarkSleepyTime = m.RibbonMarkSleepyTime;
            dest.RibbonMarkDusk = m.RibbonMarkDusk;
            dest.RibbonMarkDawn = m.RibbonMarkDawn;
            dest.RibbonMarkCloudy = m.RibbonMarkCloudy;
            dest.RibbonMarkRainy = m.RibbonMarkRainy;
            dest.RibbonMarkStormy = m.RibbonMarkStormy;
            dest.RibbonMarkSnowy = m.RibbonMarkSnowy;
            dest.RibbonMarkBlizzard = m.RibbonMarkBlizzard;
            dest.RibbonMarkDry = m.RibbonMarkDry;
            dest.RibbonMarkSandstorm = m.RibbonMarkSandstorm;
            dest.RibbonMarkMisty = m.RibbonMarkMisty;
            dest.RibbonMarkDestiny = m.RibbonMarkDestiny;
            dest.RibbonMarkFishing = m.RibbonMarkFishing;
            dest.RibbonMarkCurry = m.RibbonMarkCurry;
            dest.RibbonMarkUncommon = m.RibbonMarkUncommon;
            dest.RibbonMarkRare = m.RibbonMarkRare;
            dest.RibbonMarkRowdy = m.RibbonMarkRowdy;
            dest.RibbonMarkAbsentMinded = m.RibbonMarkAbsentMinded;
            dest.RibbonMarkJittery = m.RibbonMarkJittery;
            dest.RibbonMarkExcited = m.RibbonMarkExcited;
            dest.RibbonMarkCharismatic = m.RibbonMarkCharismatic;
            dest.RibbonMarkCalmness = m.RibbonMarkCalmness;
            dest.RibbonMarkIntense = m.RibbonMarkIntense;
            dest.RibbonMarkZonedOut = m.RibbonMarkZonedOut;
            dest.RibbonMarkJoyful = m.RibbonMarkJoyful;
            dest.RibbonMarkAngry = m.RibbonMarkAngry;
            dest.RibbonMarkSmiley = m.RibbonMarkSmiley;
            dest.RibbonMarkTeary = m.RibbonMarkTeary;
            dest.RibbonMarkUpbeat = m.RibbonMarkUpbeat;
            dest.RibbonMarkPeeved = m.RibbonMarkPeeved;
            dest.RibbonMarkIntellectual = m.RibbonMarkIntellectual;
            dest.RibbonMarkFerocious = m.RibbonMarkFerocious;
            dest.RibbonMarkCrafty = m.RibbonMarkCrafty;
            dest.RibbonMarkScowling = m.RibbonMarkScowling;
            dest.RibbonMarkKindly = m.RibbonMarkKindly;
            dest.RibbonMarkFlustered = m.RibbonMarkFlustered;
            dest.RibbonMarkPumpedUp = m.RibbonMarkPumpedUp;
            dest.RibbonMarkZeroEnergy = m.RibbonMarkZeroEnergy;
            dest.RibbonMarkPrideful = m.RibbonMarkPrideful;
            dest.RibbonMarkUnsure = m.RibbonMarkUnsure;
            dest.RibbonMarkHumble = m.RibbonMarkHumble;
            dest.RibbonMarkThorny = m.RibbonMarkThorny;
            dest.RibbonMarkVigor = m.RibbonMarkVigor;
            dest.RibbonMarkSlump = m.RibbonMarkSlump;
        }

        public bool HasMark8(RibbonIndex index) => index switch
        {
            RibbonIndex.MarkLunchtime => m.RibbonMarkLunchtime,
            RibbonIndex.MarkSleepyTime => m.RibbonMarkSleepyTime,
            RibbonIndex.MarkDusk => m.RibbonMarkDusk,
            RibbonIndex.MarkDawn => m.RibbonMarkDawn,
            RibbonIndex.MarkCloudy => m.RibbonMarkCloudy,
            RibbonIndex.MarkRainy => m.RibbonMarkRainy,
            RibbonIndex.MarkStormy => m.RibbonMarkStormy,
            RibbonIndex.MarkSnowy => m.RibbonMarkSnowy,
            RibbonIndex.MarkBlizzard => m.RibbonMarkBlizzard,
            RibbonIndex.MarkDry => m.RibbonMarkDry,
            RibbonIndex.MarkSandstorm => m.RibbonMarkSandstorm,
            RibbonIndex.MarkMisty => m.RibbonMarkMisty,
            RibbonIndex.MarkDestiny => m.RibbonMarkDestiny,
            RibbonIndex.MarkFishing => m.RibbonMarkFishing,
            RibbonIndex.MarkCurry => m.RibbonMarkCurry,
            RibbonIndex.MarkUncommon => m.RibbonMarkUncommon,
            RibbonIndex.MarkRare => m.RibbonMarkRare,
            RibbonIndex.MarkRowdy => m.RibbonMarkRowdy,
            RibbonIndex.MarkAbsentMinded => m.RibbonMarkAbsentMinded,
            RibbonIndex.MarkJittery => m.RibbonMarkJittery,
            RibbonIndex.MarkExcited => m.RibbonMarkExcited,
            RibbonIndex.MarkCharismatic => m.RibbonMarkCharismatic,
            RibbonIndex.MarkCalmness => m.RibbonMarkCalmness,
            RibbonIndex.MarkIntense => m.RibbonMarkIntense,
            RibbonIndex.MarkZonedOut => m.RibbonMarkZonedOut,
            RibbonIndex.MarkJoyful => m.RibbonMarkJoyful,
            RibbonIndex.MarkAngry => m.RibbonMarkAngry,
            RibbonIndex.MarkSmiley => m.RibbonMarkSmiley,
            RibbonIndex.MarkTeary => m.RibbonMarkTeary,
            RibbonIndex.MarkUpbeat => m.RibbonMarkUpbeat,
            RibbonIndex.MarkPeeved => m.RibbonMarkPeeved,
            RibbonIndex.MarkIntellectual => m.RibbonMarkIntellectual,
            RibbonIndex.MarkFerocious => m.RibbonMarkFerocious,
            RibbonIndex.MarkCrafty => m.RibbonMarkCrafty,
            RibbonIndex.MarkScowling => m.RibbonMarkScowling,
            RibbonIndex.MarkKindly => m.RibbonMarkKindly,
            RibbonIndex.MarkFlustered => m.RibbonMarkFlustered,
            RibbonIndex.MarkPumpedUp => m.RibbonMarkPumpedUp,
            RibbonIndex.MarkZeroEnergy => m.RibbonMarkZeroEnergy,
            RibbonIndex.MarkPrideful => m.RibbonMarkPrideful,
            RibbonIndex.MarkUnsure => m.RibbonMarkUnsure,
            RibbonIndex.MarkHumble => m.RibbonMarkHumble,
            RibbonIndex.MarkThorny => m.RibbonMarkThorny,
            RibbonIndex.MarkVigor => m.RibbonMarkVigor,
            RibbonIndex.MarkSlump => m.RibbonMarkSlump,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
        };
    }
}
