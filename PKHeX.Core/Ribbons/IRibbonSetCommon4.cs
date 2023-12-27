namespace PKHeX.Core;

/// <summary> Common Ribbons introduced in Generation 4 </summary>
public interface IRibbonSetCommon4
{
    bool RibbonChampionSinnoh { get; set; }
    bool RibbonAlert { get; set; }
    bool RibbonShock { get; set; }
    bool RibbonDowncast { get; set; }
    bool RibbonCareless { get; set; }
    bool RibbonRelax { get; set; }
    bool RibbonSnooze { get; set; }
    bool RibbonSmile { get; set; }
    bool RibbonGorgeous { get; set; }
    bool RibbonRoyal { get; set; }
    bool RibbonGorgeousRoyal { get; set; }
    bool RibbonFootprint { get; set; }
    bool RibbonRecord { get; set; }
    bool RibbonLegend { get; set; }
}

public static partial class RibbonExtensions
{
    public static void CopyRibbonSetCommon4(this IRibbonSetCommon4 set, IRibbonSetCommon4 dest)
    {
        dest.RibbonChampionSinnoh = set.RibbonChampionSinnoh;
        dest.RibbonAlert = set.RibbonAlert;
        dest.RibbonShock = set.RibbonShock;
        dest.RibbonDowncast = set.RibbonDowncast;
        dest.RibbonCareless = set.RibbonCareless;
        dest.RibbonRelax = set.RibbonRelax;
        dest.RibbonSnooze = set.RibbonSnooze;
        dest.RibbonSmile = set.RibbonSmile;
        dest.RibbonGorgeous = set.RibbonGorgeous;
        dest.RibbonRoyal = set.RibbonRoyal;
        dest.RibbonGorgeousRoyal = set.RibbonGorgeousRoyal;
        dest.RibbonFootprint = set.RibbonFootprint;
        dest.RibbonRecord = set.RibbonRecord;
        dest.RibbonLegend = set.RibbonLegend;
    }
}
