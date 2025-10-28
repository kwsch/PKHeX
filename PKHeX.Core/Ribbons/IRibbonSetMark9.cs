namespace PKHeX.Core;

/// <summary> Marks introduced in Generation 9 </summary>
public interface IRibbonSetMark9
{
    bool RibbonMarkJumbo { get; set; }
    bool RibbonMarkMini { get; set; }
    bool RibbonMarkItemfinder { get; set; }
    bool RibbonMarkPartner { get; set; }
    bool RibbonMarkGourmand { get; set; }
    bool RibbonMarkAlpha { get; set; }
    bool RibbonMarkMightiest { get; set; }
    bool RibbonMarkTitan { get; set; }

    bool HasMarkEncounter9 { get; }
}

public static partial class RibbonExtensions
{
    public static void CopyRibbonSetMark9(this IRibbonSetMark9 set, IRibbonSetMark9 dest)
    {
        dest.RibbonMarkJumbo = set.RibbonMarkJumbo;
        dest.RibbonMarkMini = set.RibbonMarkMini;
        dest.RibbonMarkItemfinder = set.RibbonMarkItemfinder;
        dest.RibbonMarkPartner = set.RibbonMarkPartner;
        dest.RibbonMarkGourmand = set.RibbonMarkGourmand;
        dest.RibbonMarkAlpha = set.RibbonMarkAlpha;
        dest.RibbonMarkMightiest = set.RibbonMarkMightiest;
        dest.RibbonMarkTitan = set.RibbonMarkTitan;
    }
}
