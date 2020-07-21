namespace PKHeX.Core
{
    /// <summary> Common Ribbons introduced in Generation 6 </summary>
    public interface IRibbonSetCommon6
    {
        bool RibbonChampionKalos { get; set; }
        bool RibbonChampionG6Hoenn { get; set; }
        bool RibbonBestFriends { get; set; }
        bool RibbonTraining { get; set; }
        bool RibbonBattlerSkillful { get; set; }
        bool RibbonBattlerExpert { get; set; }

        bool RibbonContestStar { get; set; }
        bool RibbonMasterCoolness { get; set; }
        bool RibbonMasterBeauty { get; set; }
        bool RibbonMasterCuteness { get; set; }
        bool RibbonMasterCleverness { get; set; }
        bool RibbonMasterToughness { get; set; }

        int RibbonCountMemoryContest { get; set; }
        int RibbonCountMemoryBattle { get; set; }
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesCommon6Bool =
        {
            nameof(IRibbonSetCommon6.RibbonChampionKalos), nameof(IRibbonSetCommon6.RibbonChampionG6Hoenn), // nameof(IRibbonSetCommon6.RibbonBestFriends),
            nameof(IRibbonSetCommon6.RibbonTraining), nameof(IRibbonSetCommon6.RibbonBattlerSkillful), nameof(IRibbonSetCommon6.RibbonBattlerExpert),
            nameof(IRibbonSetCommon6.RibbonContestStar), nameof(IRibbonSetCommon6.RibbonMasterCoolness), nameof(IRibbonSetCommon6.RibbonMasterBeauty),
            nameof(IRibbonSetCommon6.RibbonMasterCuteness), nameof(IRibbonSetCommon6.RibbonMasterCleverness), nameof(IRibbonSetCommon6.RibbonMasterToughness),
        };

        private static readonly string[] RibbonSetNamesCommon6Contest =
        {
            nameof(IRibbonSetCommon6.RibbonMasterCoolness), nameof(IRibbonSetCommon6.RibbonMasterBeauty),
            nameof(IRibbonSetCommon6.RibbonMasterCuteness), nameof(IRibbonSetCommon6.RibbonMasterCleverness),
            nameof(IRibbonSetCommon6.RibbonMasterToughness),
        };

        internal static bool[] RibbonBits(this IRibbonSetCommon6 set)
        {
            return new[]
            {
                set.RibbonChampionKalos,
                set.RibbonChampionG6Hoenn,
                //set.RibbonBestFriends,
                set.RibbonTraining,
                set.RibbonBattlerSkillful,
                set.RibbonBattlerExpert,

                set.RibbonContestStar,
                set.RibbonMasterCoolness,
                set.RibbonMasterBeauty,
                set.RibbonMasterCuteness,
                set.RibbonMasterCleverness,
                set.RibbonMasterToughness,
            };
        }

        internal static bool[] RibbonBitsContest(this IRibbonSetCommon6 set)
        {
            return new[]
            {
                set.RibbonMasterCoolness,
                set.RibbonMasterBeauty,
                set.RibbonMasterCuteness,
                set.RibbonMasterCleverness,
                set.RibbonMasterToughness,
            };
        }

        internal static string[] RibbonNamesBool(this IRibbonSetCommon6 _) => RibbonSetNamesCommon6Bool;
        internal static string[] RibbonNamesContest(this IRibbonSetCommon6 _) => RibbonSetNamesCommon6Contest;
    }
}
