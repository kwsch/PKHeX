namespace PKHeX.Core;

/// <summary> Common Ribbons introduced in Generation 8 </summary>
public interface IRibbonSetCommon8
{
    bool RibbonChampionGalar { get; set; }
    bool RibbonTowerMaster { get; set; }
    bool RibbonMasterRank { get; set; }
    bool RibbonTwinklingStar { get; set; }
    bool RibbonPioneer { get; set; }
}

internal static partial class RibbonExtensions
{
    internal static void CopyRibbonSetCommon8(this IRibbonSetCommon8 set, IRibbonSetCommon8 dest)
    {
        dest.RibbonChampionGalar = set.RibbonChampionGalar;
        dest.RibbonTowerMaster = set.RibbonTowerMaster;
        dest.RibbonMasterRank = set.RibbonMasterRank;
        dest.RibbonTwinklingStar = set.RibbonTwinklingStar;
        dest.RibbonPioneer = set.RibbonPioneer;
    }
}
