namespace PKHeX.Core;

/// <summary> Common Ribbons introduced in Generation 7 </summary>
public interface IRibbonSetCommon7
{
    bool RibbonChampionAlola { get; set; }
    bool RibbonBattleRoyale { get; set; }
    bool RibbonBattleTreeGreat { get; set; }
    bool RibbonBattleTreeMaster { get; set; }
}

public static partial class RibbonExtensions
{
    public static void CopyRibbonSetCommon7(this IRibbonSetCommon7 set, IRibbonSetCommon7 dest)
    {
        dest.RibbonChampionAlola = set.RibbonChampionAlola;
        dest.RibbonBattleRoyale = set.RibbonBattleRoyale;
        dest.RibbonBattleTreeGreat = set.RibbonBattleTreeGreat;
        dest.RibbonBattleTreeMaster = set.RibbonBattleTreeMaster;
    }
}
