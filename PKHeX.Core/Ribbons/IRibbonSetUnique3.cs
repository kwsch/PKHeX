namespace PKHeX.Core;

/// <summary> Ribbons introduced in Generation 3 and were transferred to future Generations (4 and 5 only). </summary>
public interface IRibbonSetUnique3
{
    /// <summary> Ribbon awarded for clearing Hoenn's Battle Tower's Lv. 50 challenge. </summary>
    bool RibbonWinning { get; set; }

    /// <summary> Ribbon awarded for clearing Hoenn's Battle Tower's Lv. 100 challenge. </summary>
    bool RibbonVictory { get; set; }
}
