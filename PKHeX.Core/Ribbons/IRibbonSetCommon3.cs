namespace PKHeX.Core;

/// <summary> Common Ribbons introduced in Generation 3 </summary>
public interface IRibbonSetCommon3
{
    bool RibbonChampionG3 { get; set; }
    bool RibbonArtist { get; set; }
    bool RibbonEffort { get; set; }
}

public static partial class RibbonExtensions
{
    public static void CopyRibbonSetCommon3(this IRibbonSetCommon3 set, IRibbonSetCommon3 dest)
    {
        dest.RibbonChampionG3 = set.RibbonChampionG3;
        dest.RibbonArtist = set.RibbonArtist;
        dest.RibbonEffort = set.RibbonEffort;
    }
}
