namespace PKHeX.Core
{
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

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesOnly3 =
        {
            nameof(IRibbonSetOnly3.RibbonCountG3Cool), nameof(IRibbonSetOnly3.RibbonCountG3Beauty), nameof(IRibbonSetOnly3.RibbonCountG3Cute),
            nameof(IRibbonSetOnly3.RibbonCountG3Smart), nameof(IRibbonSetOnly3.RibbonCountG3Tough),

            nameof(IRibbonSetOnly3.RibbonWorld),
            nameof(IRibbonSetOnly3.Unused1), nameof(IRibbonSetOnly3.Unused2),
            nameof(IRibbonSetOnly3.Unused3), nameof(IRibbonSetOnly3.Unused4),
        };

        internal static string[] RibbonNames(this IRibbonSetOnly3 _) => RibbonSetNamesOnly3;
    }
}
