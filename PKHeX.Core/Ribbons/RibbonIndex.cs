using System;
using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Ribbon Indexes for Generation 8
/// </summary>
public enum RibbonIndex : byte
{
    ChampionKalos,
    ChampionG3,
    ChampionSinnoh,
    BestFriends,
    Training,
    BattlerSkillful,
    BattlerExpert,
    Effort,
    Alert,
    Shock,
    Downcast,
    Careless,
    Relax,
    Snooze,
    Smile,
    Gorgeous,
    Royal,
    GorgeousRoyal,
    Artist,
    Footprint,
    Record,
    Legend,
    Country,
    National,
    Earth,
    World,
    Classic,
    Premier,
    Event,
    Birthday,
    Special,
    Souvenir,
    Wishing,
    ChampionBattle,
    ChampionRegional,
    ChampionNational,
    ChampionWorld,
    CountMemoryContest,
    CountMemoryBattle,
    ChampionG6Hoenn,
    ContestStar,
    MasterCoolness,
    MasterBeauty,
    MasterCuteness,
    MasterCleverness,
    MasterToughness,
    ChampionAlola,
    BattleRoyale,
    BattleTreeGreat,
    BattleTreeMaster,
    ChampionGalar,
    TowerMaster,
    MasterRank,

    MarkLunchtime,
    MarkSleepyTime,
    MarkDusk,
    MarkDawn,
    MarkCloudy,
    MarkRainy,
    MarkStormy,
    MarkSnowy,
    MarkBlizzard,
    MarkDry,
    MarkSandstorm,
    MarkMisty,
    MarkDestiny,
    MarkFishing,
    MarkCurry,
    MarkUncommon,
    MarkRare,
    MarkRowdy,
    MarkAbsentMinded,
    MarkJittery,
    MarkExcited,
    MarkCharismatic,
    MarkCalmness,
    MarkIntense,
    MarkZonedOut,
    MarkJoyful,
    MarkAngry,
    MarkSmiley,
    MarkTeary,
    MarkUpbeat,
    MarkPeeved,
    MarkIntellectual,
    MarkFerocious,
    MarkCrafty,
    MarkScowling,
    MarkKindly,
    MarkFlustered,
    MarkPumpedUp,
    MarkZeroEnergy,
    MarkPrideful,
    MarkUnsure,
    MarkHumble,
    MarkThorny,
    MarkVigor,
    MarkSlump,

    Hisui,
    TwinklingStar,

    ChampionPaldea,
    MarkJumbo,
    MarkMini,
    MarkItemfinder,
    MarkPartner,
    MarkGourmand,
    OnceInALifetime,
    MarkAlpha,
    MarkMightiest,
    MarkTitan,
    Partner,

    MAX_COUNT,
}

public static class RibbonIndexExtensions
{
    public const RibbonIndex MAX_G8 = MarkSlump;
    public const RibbonIndex MAX_G8A = Hisui;
    public const RibbonIndex MAX_G8B = TwinklingStar;
    public const RibbonIndex MAX_G9 = Partner;

    public static bool GetRibbonIndex(this IRibbonIndex x, RibbonIndex r) => x.GetRibbon((int)r);
    public static void SetRibbonIndex(this IRibbonIndex x, RibbonIndex r, bool value = true) => x.SetRibbon((int)r, value);
    public static bool IsEncounterMark8(this RibbonIndex r) => r is >= MarkLunchtime and <= MarkSlump;
    public static bool IsEncounterMark9(this RibbonIndex r) => r is >= MarkAlpha and <= MarkTitan;
    public static bool IsRibbon(this RibbonIndex r) => r < MAX_COUNT && !r.IsEncounterMark8() && r.IsEncounterMark9();

    /// <summary>
    /// Checks if the ribbon index is one of the specific wild encounter-only marks. These marks are granted when the encounter spawns in the wild.
    /// </summary>
    public static bool HasEncounterMark(this IRibbonIndex m)
    {
        for (int i = (int)MarkLunchtime; i <= (int)MarkSlump; i++)
        {
            if (m.GetRibbon(i))
                return true;
        }
        return false;
    }

    public static AreaWeather8 GetWeather8(this RibbonIndex x) => x switch
    {
        MarkCloudy => AreaWeather8.Overcast,
        MarkRainy => AreaWeather8.Raining,
        MarkStormy => AreaWeather8.Thunderstorm,
        MarkDry => AreaWeather8.Intense_Sun,
        MarkSnowy => AreaWeather8.Snowing,
        MarkBlizzard => AreaWeather8.Snowstorm,
        MarkSandstorm => AreaWeather8.Sandstorm,
        MarkMisty => AreaWeather8.Heavy_Fog,
        _ => AreaWeather8.None,
    };

    private enum RibbonIndexGroup : byte
    {
        None,
        EncounterMark,
        CountMemory,
        Common3,
        Common4,
        Event3,
        Event4,
        Common6,
        Common7,
        Common8,
        Common9,
    }

    private static RibbonIndexGroup GetGroup(this RibbonIndex r)
    {
        if (r.IsEncounterMark8())
            return RibbonIndexGroup.EncounterMark;
        return r switch
        {
            ChampionG3 => RibbonIndexGroup.Common3,
            Effort => RibbonIndexGroup.Common3,
            Artist => RibbonIndexGroup.Common3,

            ChampionSinnoh => RibbonIndexGroup.Common4,
            Alert => RibbonIndexGroup.Common4,
            Shock => RibbonIndexGroup.Common4,
            Downcast => RibbonIndexGroup.Common4,
            Careless => RibbonIndexGroup.Common4,
            Relax => RibbonIndexGroup.Common4,
            Snooze => RibbonIndexGroup.Common4,
            Smile => RibbonIndexGroup.Common4,
            Gorgeous => RibbonIndexGroup.Common4,
            Royal => RibbonIndexGroup.Common4,
            GorgeousRoyal => RibbonIndexGroup.Common4,
            Footprint => RibbonIndexGroup.Common4,
            Record => RibbonIndexGroup.Common4,
            Legend => RibbonIndexGroup.Common4,

            Country => RibbonIndexGroup.Event3,
            National => RibbonIndexGroup.Event3,
            Earth => RibbonIndexGroup.Event3,
            ChampionBattle => RibbonIndexGroup.Event3,
            ChampionRegional => RibbonIndexGroup.Event3,
            ChampionNational => RibbonIndexGroup.Event3,

            World => RibbonIndexGroup.Event4,
            Classic => RibbonIndexGroup.Event4,
            Premier => RibbonIndexGroup.Event4,
            Event => RibbonIndexGroup.Event4,
            Birthday => RibbonIndexGroup.Event4,
            Special => RibbonIndexGroup.Event4,
            Souvenir => RibbonIndexGroup.Event4,
            Wishing => RibbonIndexGroup.Event4,
            ChampionWorld => RibbonIndexGroup.Event4,

            ChampionKalos => RibbonIndexGroup.Common6,
            BestFriends => RibbonIndexGroup.Common6,
            Training => RibbonIndexGroup.Common6,
            BattlerSkillful => RibbonIndexGroup.Common6,
            BattlerExpert => RibbonIndexGroup.Common6,
            ChampionG6Hoenn => RibbonIndexGroup.Common6,
            ContestStar => RibbonIndexGroup.Common6,
            MasterCoolness => RibbonIndexGroup.Common6,
            MasterBeauty => RibbonIndexGroup.Common6,
            MasterCuteness => RibbonIndexGroup.Common6,
            MasterCleverness => RibbonIndexGroup.Common6,
            MasterToughness => RibbonIndexGroup.Common6,

            CountMemoryContest => RibbonIndexGroup.CountMemory,
            CountMemoryBattle => RibbonIndexGroup.CountMemory,

            ChampionAlola => RibbonIndexGroup.Common7,
            BattleRoyale => RibbonIndexGroup.Common7,
            BattleTreeGreat => RibbonIndexGroup.Common7,
            BattleTreeMaster => RibbonIndexGroup.Common7,

            ChampionGalar => RibbonIndexGroup.Common8,
            TowerMaster => RibbonIndexGroup.Common8,
            MasterRank => RibbonIndexGroup.Common8,
            Hisui => RibbonIndexGroup.Common8,
            TwinklingStar => RibbonIndexGroup.Common8,

            ChampionPaldea => RibbonIndexGroup.Common9,
            MarkJumbo => RibbonIndexGroup.Common9,
            MarkMini => RibbonIndexGroup.Common9,
            MarkItemfinder => RibbonIndexGroup.Common9,
            MarkPartner => RibbonIndexGroup.Common9,
            MarkGourmand => RibbonIndexGroup.Common9,
            OnceInALifetime => RibbonIndexGroup.Common9,
            MarkAlpha => RibbonIndexGroup.Common9,
            MarkMightiest => RibbonIndexGroup.Common9,
            MarkTitan => RibbonIndexGroup.Common9,
            Partner => RibbonIndexGroup.Common9,

            _ => RibbonIndexGroup.None,
        };
    }

    public static void Fix(this RibbonIndex r, in RibbonVerifierArguments args, bool state)
    {
        var pk = args.Entity;
        var group = r.GetGroup();
        switch (group)
        {
            case RibbonIndexGroup.EncounterMark:
                r.FixEncounterMark(pk, state);
                return;
            case RibbonIndexGroup.CountMemory:
                if (pk is not IRibbonSetMemory6 m6)
                    return;
                (byte contest, byte battle) = state ? RibbonRules.GetMaxMemoryCounts(args.History, args.Entity, args.Encounter) : default;
                if (r is CountMemoryContest)
                    m6.RibbonCountMemoryContest = contest;
                else
                    m6.RibbonCountMemoryBattle = battle;
                return;
            case RibbonIndexGroup.Common3:
                if (pk is not IRibbonSetCommon3 c3)
                    return;
                if (r == ChampionG3) c3.RibbonChampionG3 = state;
                else if (r == Effort) c3.RibbonEffort = state;
                else if (r == Artist) c3.RibbonArtist = state;
                return;
            case RibbonIndexGroup.Common4:
                if (pk is not IRibbonSetCommon4 c4)
                    return;
                if (r == ChampionSinnoh) c4.RibbonChampionSinnoh = state;
                else if (r == Alert) c4.RibbonAlert = state;
                else if (r == Shock) c4.RibbonShock = state;
                else if (r == Downcast) c4.RibbonDowncast = state;
                else if (r == Careless) c4.RibbonCareless = state;
                else if (r == Relax) c4.RibbonRelax = state;
                else if (r == Snooze) c4.RibbonSnooze = state;
                else if (r == Smile) c4.RibbonSmile = state;
                else if (r == Gorgeous) c4.RibbonGorgeous = state;
                else if (r == Royal) c4.RibbonRoyal = state;
                else if (r == GorgeousRoyal) c4.RibbonGorgeousRoyal = state;
                else if (r == Footprint) c4.RibbonFootprint = state;
                else if (r == Record) c4.RibbonRecord = state;
                else if (r == Legend) c4.RibbonLegend = state;
                return;
            case RibbonIndexGroup.Event3:
                if (pk is not IRibbonSetEvent3 e3)
                    return;
                if (r == Country) e3.RibbonCountry = state;
                else if (r == National) e3.RibbonNational = state;
                else if (r == Earth) e3.RibbonEarth = state;
                else if (r == ChampionBattle) e3.RibbonChampionBattle = state;
                else if (r == ChampionRegional) e3.RibbonChampionRegional = state;
                else if (r == ChampionNational) e3.RibbonChampionNational = state;
                return;
            case RibbonIndexGroup.Event4:
                if (pk is not IRibbonSetEvent4 e4)
                    return;
                if (r == World) e4.RibbonWorld = state;
                else if (r == Classic) e4.RibbonClassic = state;
                else if (r == Premier) e4.RibbonPremier = state;
                else if (r == Event) e4.RibbonEvent = state;
                else if (r == Birthday) e4.RibbonBirthday = state;
                else if (r == Special) e4.RibbonSpecial = state;
                else if (r == Souvenir) e4.RibbonSouvenir = state;
                else if (r == Wishing) e4.RibbonWishing = state;
                else if (r == ChampionWorld) e4.RibbonChampionWorld = state;
                return;
            case RibbonIndexGroup.Common6:
                if (pk is not IRibbonSetCommon6 c6)
                    return;
                if (r == ChampionKalos) c6.RibbonChampionKalos = state;
                else if (r == BestFriends) c6.RibbonBestFriends = state;
                else if (r == Training) c6.RibbonTraining = state;
                else if (r == BattlerSkillful) c6.RibbonBattlerSkillful = state;
                else if (r == BattlerExpert) c6.RibbonBattlerExpert = state;
                else if (r == ChampionG6Hoenn) c6.RibbonChampionG6Hoenn = state;
                else if (r == ContestStar) c6.RibbonContestStar = state;
                else if (r == MasterCoolness) c6.RibbonMasterCoolness = state;
                else if (r == MasterBeauty) c6.RibbonMasterBeauty = state;
                else if (r == MasterCuteness) c6.RibbonMasterCuteness = state;
                else if (r == MasterCleverness) c6.RibbonMasterCleverness = state;
                else if (r == MasterToughness) c6.RibbonMasterToughness = state;
                return;
            case RibbonIndexGroup.Common7:
                if (pk is not IRibbonSetCommon7 c7)
                    return;
                if (r == ChampionAlola) c7.RibbonChampionAlola = state;
                else if (r == BattleRoyale) c7.RibbonBattleRoyale = state;
                else if (r == BattleTreeGreat) c7.RibbonBattleTreeGreat = state;
                else if (r == BattleTreeMaster) c7.RibbonBattleTreeMaster = state;
                return;
            case RibbonIndexGroup.Common8:
                if (pk is not IRibbonSetCommon8 c8)
                    return;
                if (r == ChampionGalar) c8.RibbonChampionGalar = state;
                else if (r == TowerMaster) c8.RibbonTowerMaster = state;
                else if (r == MasterRank) c8.RibbonMasterRank = state;
                else if (r == Hisui) c8.RibbonHisui = state;
                else if (r == TwinklingStar) c8.RibbonTwinklingStar = state;
                return;
            case RibbonIndexGroup.Common9:
                if (pk is IRibbonSetCommon9 c9)
                {
                    if      (r == ChampionPaldea) c9.RibbonChampionPaldea = state;
                    else if (r == OnceInALifetime) c9.RibbonOnceInALifetime = state;
                    else if (r == Partner) c9.RibbonPartner = state;
                }
                if (pk is IRibbonSetMark9 m9)
                {
                    if      (r == MarkJumbo) m9.RibbonMarkJumbo = state;
                    else if (r == MarkMini) m9.RibbonMarkMini = state;
                    else if (r == MarkItemfinder) m9.RibbonMarkItemfinder = state;
                    else if (r == MarkPartner) m9.RibbonMarkPartner = state;
                    else if (r == MarkGourmand) m9.RibbonMarkGourmand = state;
                    else if (r == MarkAlpha) m9.RibbonMarkAlpha = state;
                    else if (r == MarkMightiest) m9.RibbonMarkMightiest = state;
                    else if (r == MarkTitan) m9.RibbonMarkTitan = state;
                }
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(r), r, null);
        }
    }

    private static void FixEncounterMark(this RibbonIndex r, PKM pk, bool state)
    {
        if (pk is not IRibbonSetMark8 m)
            return;
        _ = r switch
        {
            MarkLunchtime => m.RibbonMarkLunchtime = state,
            MarkSleepyTime => m.RibbonMarkSleepyTime = state,
            MarkDusk => m.RibbonMarkDusk = state,
            MarkDawn => m.RibbonMarkDawn = state,
            MarkCloudy => m.RibbonMarkCloudy = state,
            MarkRainy => m.RibbonMarkRainy = state,
            MarkStormy => m.RibbonMarkStormy = state,
            MarkSnowy => m.RibbonMarkSnowy = state,
            MarkBlizzard => m.RibbonMarkBlizzard = state,
            MarkDry => m.RibbonMarkDry = state,
            MarkSandstorm => m.RibbonMarkSandstorm = state,
            MarkMisty => m.RibbonMarkMisty = state,
            MarkDestiny => m.RibbonMarkDestiny = state,
            MarkFishing => m.RibbonMarkFishing = state,
            MarkCurry => m.RibbonMarkCurry = state,
            MarkUncommon => m.RibbonMarkUncommon = state,
            MarkRare => m.RibbonMarkRare = state,
            MarkRowdy => m.RibbonMarkRowdy = state,
            MarkAbsentMinded => m.RibbonMarkAbsentMinded = state,
            MarkJittery => m.RibbonMarkJittery = state,
            MarkExcited => m.RibbonMarkExcited = state,
            MarkCharismatic => m.RibbonMarkCharismatic = state,
            MarkCalmness => m.RibbonMarkCalmness = state,
            MarkIntense => m.RibbonMarkIntense = state,
            MarkZonedOut => m.RibbonMarkZonedOut = state,
            MarkJoyful => m.RibbonMarkJoyful = state,
            MarkAngry => m.RibbonMarkAngry = state,
            MarkSmiley => m.RibbonMarkSmiley = state,
            MarkTeary => m.RibbonMarkTeary = state,
            MarkUpbeat => m.RibbonMarkUpbeat = state,
            MarkPeeved => m.RibbonMarkPeeved = state,
            MarkIntellectual => m.RibbonMarkIntellectual = state,
            MarkFerocious => m.RibbonMarkFerocious = state,
            MarkCrafty => m.RibbonMarkCrafty = state,
            MarkScowling => m.RibbonMarkScowling = state,
            MarkKindly => m.RibbonMarkKindly = state,
            MarkFlustered => m.RibbonMarkFlustered = state,
            MarkPumpedUp => m.RibbonMarkPumpedUp = state,
            MarkZeroEnergy => m.RibbonMarkZeroEnergy = state,
            MarkPrideful => m.RibbonMarkPrideful = state,
            MarkUnsure => m.RibbonMarkUnsure = state,
            MarkHumble => m.RibbonMarkHumble = state,
            MarkThorny => m.RibbonMarkThorny = state,
            MarkVigor => m.RibbonMarkVigor = state,
            MarkSlump => m.RibbonMarkSlump = state,
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null),
        };
    }

    public static string GetPropertyName(this RibbonIndex r) => r switch
    {
        ChampionKalos => nameof(IRibbonSetCommon6.RibbonChampionKalos),
        ChampionG3 => nameof(IRibbonSetCommon3.RibbonChampionG3),
        ChampionSinnoh => nameof(IRibbonSetCommon4.RibbonChampionSinnoh),
        BestFriends => nameof(IRibbonSetCommon6.RibbonBestFriends),
        Training => nameof(IRibbonSetCommon6.RibbonTraining),
        BattlerSkillful => nameof(IRibbonSetCommon6.RibbonBattlerSkillful),
        BattlerExpert => nameof(IRibbonSetCommon6.RibbonBattlerExpert),
        Effort => nameof(IRibbonSetCommon3.RibbonEffort),
        Alert => nameof(IRibbonSetCommon4.RibbonAlert),
        Shock => nameof(IRibbonSetCommon4.RibbonShock),
        Downcast => nameof(IRibbonSetCommon4.RibbonDowncast),
        Careless => nameof(IRibbonSetCommon4.RibbonCareless),
        Relax => nameof(IRibbonSetCommon4.RibbonRelax),
        Snooze => nameof(IRibbonSetCommon4.RibbonSnooze),
        Smile => nameof(IRibbonSetCommon4.RibbonSmile),
        Gorgeous => nameof(IRibbonSetCommon4.RibbonGorgeous),
        Royal => nameof(IRibbonSetCommon4.RibbonRoyal),
        GorgeousRoyal => nameof(IRibbonSetCommon4.RibbonGorgeousRoyal),
        Artist => nameof(IRibbonSetCommon3.RibbonArtist),
        Footprint => nameof(IRibbonSetCommon4.RibbonFootprint),
        Record => nameof(IRibbonSetCommon4.RibbonRecord),
        Legend => nameof(IRibbonSetCommon4.RibbonLegend),
        Country => nameof(IRibbonSetEvent3.RibbonCountry),
        National => nameof(IRibbonSetEvent3.RibbonNational),
        Earth => nameof(IRibbonSetEvent3.RibbonEarth),
        World => nameof(IRibbonSetEvent4.RibbonWorld),
        Classic => nameof(IRibbonSetEvent4.RibbonClassic),
        Premier => nameof(IRibbonSetEvent4.RibbonPremier),
        Event => nameof(IRibbonSetEvent4.RibbonEvent),
        Birthday => nameof(IRibbonSetEvent4.RibbonBirthday),
        Special => nameof(IRibbonSetEvent4.RibbonSpecial),
        Souvenir => nameof(IRibbonSetEvent4.RibbonSouvenir),
        Wishing => nameof(IRibbonSetEvent4.RibbonWishing),
        ChampionBattle => nameof(IRibbonSetEvent3.RibbonChampionBattle),
        ChampionRegional => nameof(IRibbonSetEvent3.RibbonChampionRegional),
        ChampionNational => nameof(IRibbonSetEvent3.RibbonChampionNational),
        ChampionWorld => nameof(IRibbonSetEvent4.RibbonChampionWorld),
        CountMemoryContest => nameof(IRibbonSetMemory6.RibbonCountMemoryContest),
        CountMemoryBattle => nameof(IRibbonSetMemory6.RibbonCountMemoryBattle),
        ChampionG6Hoenn => nameof(IRibbonSetCommon6.RibbonChampionG6Hoenn),
        ContestStar => nameof(IRibbonSetCommon6.RibbonContestStar),
        MasterCoolness => nameof(IRibbonSetCommon6.RibbonMasterCoolness),
        MasterBeauty => nameof(IRibbonSetCommon6.RibbonMasterBeauty),
        MasterCuteness => nameof(IRibbonSetCommon6.RibbonMasterCuteness),
        MasterCleverness => nameof(IRibbonSetCommon6.RibbonMasterCleverness),
        MasterToughness => nameof(IRibbonSetCommon6.RibbonMasterToughness),
        ChampionAlola => nameof(IRibbonSetCommon7.RibbonChampionAlola),
        BattleRoyale => nameof(IRibbonSetCommon7.RibbonBattleRoyale),
        BattleTreeGreat => nameof(IRibbonSetCommon7.RibbonBattleTreeGreat),
        BattleTreeMaster => nameof(IRibbonSetCommon7.RibbonBattleTreeMaster),
        ChampionGalar => nameof(IRibbonSetCommon8.RibbonChampionGalar),
        TowerMaster => nameof(IRibbonSetCommon8.RibbonTowerMaster),
        MasterRank => nameof(IRibbonSetCommon8.RibbonMasterRank),
        MarkLunchtime => nameof(IRibbonSetMark8.RibbonMarkLunchtime),
        MarkSleepyTime => nameof(IRibbonSetMark8.RibbonMarkSleepyTime),
        MarkDusk => nameof(IRibbonSetMark8.RibbonMarkDusk),
        MarkDawn => nameof(IRibbonSetMark8.RibbonMarkDawn),
        MarkCloudy => nameof(IRibbonSetMark8.RibbonMarkCloudy),
        MarkRainy => nameof(IRibbonSetMark8.RibbonMarkRainy),
        MarkStormy => nameof(IRibbonSetMark8.RibbonMarkStormy),
        MarkSnowy => nameof(IRibbonSetMark8.RibbonMarkSnowy),
        MarkBlizzard => nameof(IRibbonSetMark8.RibbonMarkBlizzard),
        MarkDry => nameof(IRibbonSetMark8.RibbonMarkDry),
        MarkSandstorm => nameof(IRibbonSetMark8.RibbonMarkSandstorm),
        MarkMisty => nameof(IRibbonSetMark8.RibbonMarkMisty),
        MarkDestiny => nameof(IRibbonSetMark8.RibbonMarkDestiny),
        MarkFishing => nameof(IRibbonSetMark8.RibbonMarkFishing),
        MarkCurry => nameof(IRibbonSetMark8.RibbonMarkCurry),
        MarkUncommon => nameof(IRibbonSetMark8.RibbonMarkUncommon),
        MarkRare => nameof(IRibbonSetMark8.RibbonMarkRare),
        MarkRowdy => nameof(IRibbonSetMark8.RibbonMarkRowdy),
        MarkAbsentMinded => nameof(IRibbonSetMark8.RibbonMarkAbsentMinded),
        MarkJittery => nameof(IRibbonSetMark8.RibbonMarkJittery),
        MarkExcited => nameof(IRibbonSetMark8.RibbonMarkExcited),
        MarkCharismatic => nameof(IRibbonSetMark8.RibbonMarkCharismatic),
        MarkCalmness => nameof(IRibbonSetMark8.RibbonMarkCalmness),
        MarkIntense => nameof(IRibbonSetMark8.RibbonMarkIntense),
        MarkZonedOut => nameof(IRibbonSetMark8.RibbonMarkZonedOut),
        MarkJoyful => nameof(IRibbonSetMark8.RibbonMarkJoyful),
        MarkAngry => nameof(IRibbonSetMark8.RibbonMarkAngry),
        MarkSmiley => nameof(IRibbonSetMark8.RibbonMarkSmiley),
        MarkTeary => nameof(IRibbonSetMark8.RibbonMarkTeary),
        MarkUpbeat => nameof(IRibbonSetMark8.RibbonMarkUpbeat),
        MarkPeeved => nameof(IRibbonSetMark8.RibbonMarkPeeved),
        MarkIntellectual => nameof(IRibbonSetMark8.RibbonMarkIntellectual),
        MarkFerocious => nameof(IRibbonSetMark8.RibbonMarkFerocious),
        MarkCrafty => nameof(IRibbonSetMark8.RibbonMarkCrafty),
        MarkScowling => nameof(IRibbonSetMark8.RibbonMarkScowling),
        MarkKindly => nameof(IRibbonSetMark8.RibbonMarkKindly),
        MarkFlustered => nameof(IRibbonSetMark8.RibbonMarkFlustered),
        MarkPumpedUp => nameof(IRibbonSetMark8.RibbonMarkPumpedUp),
        MarkZeroEnergy => nameof(IRibbonSetMark8.RibbonMarkZeroEnergy),
        MarkPrideful => nameof(IRibbonSetMark8.RibbonMarkPrideful),
        MarkUnsure => nameof(IRibbonSetMark8.RibbonMarkUnsure),
        MarkHumble => nameof(IRibbonSetMark8.RibbonMarkHumble),
        MarkThorny => nameof(IRibbonSetMark8.RibbonMarkThorny),
        MarkVigor => nameof(IRibbonSetMark8.RibbonMarkVigor),
        MarkSlump => nameof(IRibbonSetMark8.RibbonMarkSlump),
        Hisui => nameof(IRibbonSetCommon8.RibbonHisui),
        TwinklingStar => nameof(IRibbonSetCommon8.RibbonTwinklingStar),
        ChampionPaldea => nameof(IRibbonSetCommon9.RibbonChampionPaldea),
        MarkJumbo => nameof(IRibbonSetMark9.RibbonMarkJumbo),
        MarkMini => nameof(IRibbonSetMark9.RibbonMarkMini),
        MarkItemfinder => nameof(IRibbonSetMark9.RibbonMarkItemfinder),
        MarkPartner => nameof(IRibbonSetMark9.RibbonMarkPartner),
        MarkGourmand => nameof(IRibbonSetMark9.RibbonMarkGourmand),
        OnceInALifetime => nameof(IRibbonSetCommon9.RibbonOnceInALifetime),
        MarkAlpha => nameof(IRibbonSetMark9.RibbonMarkAlpha),
        MarkMightiest => nameof(IRibbonSetMark9.RibbonMarkMightiest),
        MarkTitan => nameof(IRibbonSetMark9.RibbonMarkTitan),
        Partner => nameof(IRibbonSetCommon9.RibbonPartner),
        _ => throw new ArgumentOutOfRangeException(nameof(r), r, null),
    };
}
