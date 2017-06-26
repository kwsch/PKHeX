namespace PKHeX.Core
{
    /// <summary> Ribbons that originated in Generation 3 and were only present within that Generation. </summary>
    internal interface IRibbonSetOnly3
    {
        int RibbonCountG3Cool { get; set; }
        int RibbonCountG3Beauty { get; set; }
        int RibbonCountG3Cute { get; set; }
        int RibbonCountG3Smart { get; set; }
        int RibbonCountG3Tough { get; set; }
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesOnly3 =
        {
            nameof(IRibbonSetOnly3.RibbonCountG3Cool), nameof(IRibbonSetOnly3.RibbonCountG3Beauty), nameof(IRibbonSetOnly3.RibbonCountG3Cute),
            nameof(IRibbonSetOnly3.RibbonCountG3Smart), nameof(IRibbonSetOnly3.RibbonCountG3Tough),
        };
        internal static int[] RibbonCounts(this IRibbonSetOnly3 set)
        {
            if (set == null)
                return new int[5];
            return new[]
            {
                set.RibbonCountG3Cool,
                set.RibbonCountG3Beauty,
                set.RibbonCountG3Cute,
                set.RibbonCountG3Smart,
                set.RibbonCountG3Tough,
            };
        }
        internal static string[] RibbonNames(this IRibbonSetOnly3 set) => RibbonSetNamesOnly3;
    }
}
