namespace PKHeX.Core;

/// <summary> Ribbons that originated in Generation 3 and were only present within that Generation. </summary>
public interface IRibbonSetOnly3
{
    int RibbonCountG3Cool { get; set; }
    int RibbonCountG3Beauty { get; set; }
    int RibbonCountG3Cute { get; set; }
    int RibbonCountG3Smart { get; set; }
    int RibbonCountG3Tough { get; set; }

    bool RibbonWorld { get; set; }
    bool Unused1 { get; set; }
    bool Unused2 { get; set; }
    bool Unused3 { get; set; }
    bool Unused4 { get; set; }
}
