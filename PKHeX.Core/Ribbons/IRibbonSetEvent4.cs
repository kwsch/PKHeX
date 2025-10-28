namespace PKHeX.Core;

/// <summary> Ribbons introduced in Generation 4 for Special Events </summary>
public interface IRibbonSetEvent4
{
    bool RibbonClassic { get; set; }
    bool RibbonWishing { get; set; }
    bool RibbonPremier { get; set; }
    bool RibbonEvent { get; set; }
    bool RibbonBirthday { get; set; }
    bool RibbonSpecial { get; set; }
    bool RibbonWorld { get; set; }
    bool RibbonChampionWorld { get; set; }
    bool RibbonSouvenir { get; set; }
}

public static partial class RibbonExtensions
{
    public static void CopyRibbonSetEvent4(this IRibbonSetEvent4 set, IRibbonSetEvent4 dest)
    {
        dest.RibbonClassic = set.RibbonClassic;
        dest.RibbonWishing = set.RibbonWishing;
        dest.RibbonPremier = set.RibbonPremier;
        dest.RibbonEvent = set.RibbonEvent;
        dest.RibbonBirthday = set.RibbonBirthday;
        dest.RibbonSpecial = set.RibbonSpecial;
        dest.RibbonWorld = set.RibbonWorld;
        dest.RibbonChampionWorld = set.RibbonChampionWorld;
        dest.RibbonSouvenir = set.RibbonSouvenir;
    }
}
