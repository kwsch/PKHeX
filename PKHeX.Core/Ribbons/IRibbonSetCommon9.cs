namespace PKHeX.Core;

/// <summary> Common Ribbons introduced in Generation 9 </summary>
public interface IRibbonSetCommon9
{
    bool RibbonChampionPaldea { get; set; }
    bool RibbonOnceInALifetime { get; set; }
}

public static partial class RibbonExtensions
{
    public static void CopyRibbonSetCommon9(this IRibbonSetCommon9 set, IRibbonSetCommon9 dest)
    {
        dest.RibbonChampionPaldea = set.RibbonChampionPaldea;
        dest.RibbonOnceInALifetime = set.RibbonOnceInALifetime;
    }
}
