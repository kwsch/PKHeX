namespace PKHeX.Core
{
    /// <summary> Common Ribbons introduced in Generation 8 </summary>
    public interface IRibbonSetCommon8
    {
        bool RibbonChampionGalar { get; set; }
        bool RibbonTowerMaster { get; set; }
        bool RibbonMasterRank { get; set; }
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesCommon8 =
        {
            nameof(IRibbonSetCommon8.RibbonChampionGalar), nameof(IRibbonSetCommon8.RibbonTowerMaster),
            nameof(IRibbonSetCommon8.RibbonMasterRank),
        };

        internal static bool[] RibbonBits(this IRibbonSetCommon8 set)
        {
            return new[]
            {
                set.RibbonChampionGalar,
                set.RibbonTowerMaster,
                set.RibbonMasterRank,
            };
        }

        internal static string[] RibbonNames(this IRibbonSetCommon8 _) => RibbonSetNamesCommon8;
    }
}