namespace PKHeX.Core
{
    /// <summary> Ribbons introduced in Generation 3 and were transferred to future Generations (4 and 5 only). </summary>
    public interface IRibbonSetUnique3
    {
        /// <summary> Ribbon awarded for clearing Hoenn's Battle Tower's Lv. 50 challenge. </summary>
        bool RibbonWinning { get; set; }

        /// <summary> Ribbon awarded for clearing Hoenn's Battle Tower's Lv. 100 challenge. </summary>
        bool RibbonVictory { get; set; }
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesUnique3 =
        {
            nameof(IRibbonSetUnique3.RibbonWinning), nameof(IRibbonSetUnique3.RibbonVictory),
        };

        internal static bool[] RibbonBits(this IRibbonSetUnique3 set)
        {
            return new[]
            {
                set.RibbonWinning,
                set.RibbonVictory,
            };
        }

        internal static string[] RibbonNames(this IRibbonSetUnique3 _) => RibbonSetNamesUnique3;
    }
}
