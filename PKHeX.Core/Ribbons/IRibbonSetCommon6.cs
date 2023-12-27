namespace PKHeX.Core;

/// <summary> Common Ribbons introduced in Generation 6 </summary>
public interface IRibbonSetCommon6
{
    bool RibbonChampionKalos { get; set; }
    bool RibbonChampionG6Hoenn { get; set; }
    bool RibbonBestFriends { get; set; }
    bool RibbonTraining { get; set; }
    bool RibbonBattlerSkillful { get; set; }
    bool RibbonBattlerExpert { get; set; }

    bool RibbonContestStar { get; set; }
    bool RibbonMasterCoolness { get; set; }
    bool RibbonMasterBeauty { get; set; }
    bool RibbonMasterCuteness { get; set; }
    bool RibbonMasterCleverness { get; set; }
    bool RibbonMasterToughness { get; set; }
}

public static partial class RibbonExtensions
{
    /// <summary>
    /// Checks if the <see cref="set"/> has all five contest stat ribbons true.
    /// </summary>
    public static bool HasAllContestRibbons(this IRibbonSetCommon6 set) => set is { RibbonMasterCoolness: true, RibbonMasterBeauty: true, RibbonMasterCuteness: true, RibbonMasterCleverness: true, RibbonMasterToughness: true };

    public static void CopyRibbonSetCommon6(this IRibbonSetCommon6 set, IRibbonSetCommon6 dest)
    {
        dest.RibbonChampionKalos = set.RibbonChampionKalos;
        dest.RibbonChampionG6Hoenn = set.RibbonChampionG6Hoenn;
        dest.RibbonBestFriends = set.RibbonBestFriends;
        dest.RibbonTraining = set.RibbonTraining;
        dest.RibbonBattlerSkillful = set.RibbonBattlerSkillful;
        dest.RibbonBattlerExpert = set.RibbonBattlerExpert;
        dest.RibbonContestStar = set.RibbonContestStar;
        dest.RibbonMasterCoolness = set.RibbonMasterCoolness;
        dest.RibbonMasterBeauty = set.RibbonMasterBeauty;
        dest.RibbonMasterCuteness = set.RibbonMasterCuteness;
        dest.RibbonMasterCleverness = set.RibbonMasterCleverness;
        dest.RibbonMasterToughness = set.RibbonMasterToughness;
    }
}
