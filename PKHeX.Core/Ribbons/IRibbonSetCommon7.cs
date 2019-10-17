namespace PKHeX.Core
{
    /// <summary> Common Ribbons introduced in Generation 7 </summary>
    public interface IRibbonSetCommon7
    {
        bool RibbonChampionAlola { get; set; }
        bool RibbonBattleRoyale { get; set; }
        bool RibbonBattleTreeGreat { get; set; }
        bool RibbonBattleTreeMaster { get; set; }
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesCommon7 =
        {
            nameof(IRibbonSetCommon7.RibbonChampionAlola), nameof(IRibbonSetCommon7.RibbonBattleRoyale),
            nameof(IRibbonSetCommon7.RibbonBattleTreeGreat), nameof(IRibbonSetCommon7.RibbonBattleTreeMaster)
        };

        internal static bool[] RibbonBits(this IRibbonSetCommon7 set)
        {
            return new[]
            {
                set.RibbonChampionAlola,
                set.RibbonBattleRoyale,
                set.RibbonBattleTreeGreat,
                set.RibbonBattleTreeMaster,
            };
        }

        internal static string[] RibbonNames(this IRibbonSetCommon7 _) => RibbonSetNamesCommon7;
    }
}
