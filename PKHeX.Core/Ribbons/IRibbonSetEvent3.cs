namespace PKHeX.Core;

/// <summary> Ribbons introduced in Generation 3 for Special Events </summary>
public interface IRibbonSetEvent3
{
    bool RibbonEarth { get; set; }
    bool RibbonNational { get; set; }
    bool RibbonCountry { get; set; }
    bool RibbonChampionBattle { get; set; }
    bool RibbonChampionRegional { get; set; }
    bool RibbonChampionNational { get; set; }
}

public static partial class RibbonExtensions
{
    public static void CopyRibbonSetEvent3(this IRibbonSetEvent3 set, IRibbonSetEvent3 dest)
    {
        dest.RibbonEarth            = set.RibbonEarth;
        dest.RibbonNational         = set.RibbonNational;
        dest.RibbonCountry          = set.RibbonCountry;
        dest.RibbonChampionBattle   = set.RibbonChampionBattle;
        dest.RibbonChampionRegional = set.RibbonChampionRegional;
        dest.RibbonChampionNational = set.RibbonChampionNational;
    }
}
