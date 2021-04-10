namespace PKHeX.Core
{
    /// <summary> Common Ribbons introduced in Generation 3 </summary>
    public interface IRibbonSetCommon3
    {
        bool RibbonChampionG3 { get; set; }
        bool RibbonArtist { get; set; }
        bool RibbonEffort { get; set; }
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesCommon3 =
        {
            nameof(IRibbonSetCommon3.RibbonChampionG3), nameof(IRibbonSetCommon3.RibbonArtist), nameof(IRibbonSetCommon3.RibbonEffort)
        };

        internal static bool[] RibbonBits(this IRibbonSetCommon3 set)
        {
            return new[]
            {
                set.RibbonChampionG3,
                set.RibbonArtist,
                set.RibbonEffort,
            };
        }

        internal static string[] RibbonNames(this IRibbonSetCommon3 _) => RibbonSetNamesCommon3;
    }
}
