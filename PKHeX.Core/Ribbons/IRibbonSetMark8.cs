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

internal static partial class RibbonExtensions
{
    public static bool HasWeatherMark(this IRibbonSetMark8 m)
    {
        return m.RibbonMarkCloudy   || m.RibbonMarkRainy || m.RibbonMarkStormy    || m.RibbonMarkSnowy
            || m.RibbonMarkBlizzard || m.RibbonMarkDry   || m.RibbonMarkSandstorm || m.RibbonMarkMisty;
    }

    internal static void CopyRibbonSetMark8(this IRibbonSetMark8 set, IRibbonSetMark8 dest)
    {
        dest.RibbonMarkLunchtime = set.RibbonMarkLunchtime;
        dest.RibbonMarkSleepyTime = set.RibbonMarkSleepyTime;
        dest.RibbonMarkDusk = set.RibbonMarkDusk;
        dest.RibbonMarkDawn = set.RibbonMarkDawn;
        dest.RibbonMarkCloudy = set.RibbonMarkCloudy;
        dest.RibbonMarkRainy = set.RibbonMarkRainy;
        dest.RibbonMarkStormy = set.RibbonMarkStormy;
        dest.RibbonMarkSnowy = set.RibbonMarkSnowy;
        dest.RibbonMarkBlizzard = set.RibbonMarkBlizzard;
        dest.RibbonMarkDry = set.RibbonMarkDry;
        dest.RibbonMarkSandstorm = set.RibbonMarkSandstorm;
        dest.RibbonMarkMisty = set.RibbonMarkMisty;
        dest.RibbonMarkDestiny = set.RibbonMarkDestiny;
        dest.RibbonMarkFishing = set.RibbonMarkFishing;
        dest.RibbonMarkCurry = set.RibbonMarkCurry;
        dest.RibbonMarkUncommon = set.RibbonMarkUncommon;
        dest.RibbonMarkRare = set.RibbonMarkRare;
        dest.RibbonMarkRowdy = set.RibbonMarkRowdy;
        dest.RibbonMarkAbsentMinded = set.RibbonMarkAbsentMinded;
        dest.RibbonMarkJittery = set.RibbonMarkJittery;
        dest.RibbonMarkExcited = set.RibbonMarkExcited;
        dest.RibbonMarkCharismatic = set.RibbonMarkCharismatic;
        dest.RibbonMarkCalmness = set.RibbonMarkCalmness;
        dest.RibbonMarkIntense = set.RibbonMarkIntense;
        dest.RibbonMarkZonedOut = set.RibbonMarkZonedOut;
        dest.RibbonMarkJoyful = set.RibbonMarkJoyful;
        dest.RibbonMarkAngry = set.RibbonMarkAngry;
        dest.RibbonMarkSmiley = set.RibbonMarkSmiley;
        dest.RibbonMarkTeary = set.RibbonMarkTeary;
        dest.RibbonMarkUpbeat = set.RibbonMarkUpbeat;
        dest.RibbonMarkPeeved = set.RibbonMarkPeeved;
        dest.RibbonMarkIntellectual = set.RibbonMarkIntellectual;
        dest.RibbonMarkFerocious = set.RibbonMarkFerocious;
        dest.RibbonMarkCrafty = set.RibbonMarkCrafty;
        dest.RibbonMarkScowling = set.RibbonMarkScowling;
        dest.RibbonMarkKindly = set.RibbonMarkKindly;
        dest.RibbonMarkFlustered = set.RibbonMarkFlustered;
        dest.RibbonMarkPumpedUp = set.RibbonMarkPumpedUp;
        dest.RibbonMarkZeroEnergy = set.RibbonMarkZeroEnergy;
        dest.RibbonMarkPrideful = set.RibbonMarkPrideful;
        dest.RibbonMarkUnsure = set.RibbonMarkUnsure;
        dest.RibbonMarkHumble = set.RibbonMarkHumble;
        dest.RibbonMarkThorny = set.RibbonMarkThorny;
        dest.RibbonMarkVigor = set.RibbonMarkVigor;
        dest.RibbonMarkSlump = set.RibbonMarkSlump;
    }
}
