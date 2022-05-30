namespace PKHeX.Core
{
    /// <summary> Common Ribbons introduced in Generation 8 </summary>
    public interface IRibbonSetCommon8
    {
        bool RibbonChampionGalar { get; set; }
        bool RibbonTowerMaster { get; set; }
        bool RibbonMasterRank { get; set; }
        bool RibbonTwinklingStar { get; set; }
        bool RibbonPioneer { get; set; }
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesCommon8 =
        {
            nameof(IRibbonSetCommon8.RibbonChampionGalar), nameof(IRibbonSetCommon8.RibbonTowerMaster),
            nameof(IRibbonSetCommon8.RibbonMasterRank),
            nameof(IRibbonSetCommon8.RibbonTwinklingStar), nameof(IRibbonSetCommon8.RibbonPioneer),
        };

        internal static bool[] RibbonBits(this IRibbonSetCommon8 set)
        {
            return new[]
            {
                set.RibbonChampionGalar,
                set.RibbonTowerMaster,
                set.RibbonMasterRank,
                set.RibbonTwinklingStar,
                set.RibbonPioneer,
            };
        }

        internal static string[] RibbonNames(this IRibbonSetCommon8 _) => RibbonSetNamesCommon8;

        internal static void CopyRibbonSetCommon8(this IRibbonSetCommon8 set, IRibbonSetCommon8 dest)
        {
            dest.RibbonChampionGalar = set.RibbonChampionGalar;
            dest.RibbonTowerMaster = set.RibbonTowerMaster;
            dest.RibbonMasterRank = set.RibbonMasterRank;
            dest.RibbonTwinklingStar = set.RibbonTwinklingStar;
            dest.RibbonPioneer = set.RibbonPioneer;
        }
    }
}