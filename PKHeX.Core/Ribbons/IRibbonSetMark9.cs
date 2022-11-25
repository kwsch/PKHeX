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

    bool HasMark();
}

internal static partial class RibbonExtensions
{
    private static readonly string[] RibbonSetNamesMark9 =
    {
        nameof(IRibbonSetMark9.RibbonMarkJumbo),
        nameof(IRibbonSetMark9.RibbonMarkMini),
        nameof(IRibbonSetMark9.RibbonMarkItemfinder),
        nameof(IRibbonSetMark9.RibbonMarkPartner),
        nameof(IRibbonSetMark9.RibbonMarkGourmand),
        nameof(IRibbonSetMark9.RibbonMarkAlpha),
        nameof(IRibbonSetMark9.RibbonMarkMightiest),
        nameof(IRibbonSetMark9.RibbonMarkTitan),
    };

    internal static bool[] RibbonBits(this IRibbonSetMark9 set)
    {
        return new[]
        {
            set.RibbonMarkJumbo,
            set.RibbonMarkMini,
            set.RibbonMarkItemfinder,
            set.RibbonMarkPartner,
            set.RibbonMarkGourmand,
            set.RibbonMarkAlpha,
            set.RibbonMarkMightiest,
            set.RibbonMarkTitan,
        };
    }

    internal static string[] RibbonNames(this IRibbonSetMark9 _) => RibbonSetNamesMark9;

    internal static void CopyRibbonSetMark9(this IRibbonSetMark9 set, IRibbonSetMark9 dest)
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
