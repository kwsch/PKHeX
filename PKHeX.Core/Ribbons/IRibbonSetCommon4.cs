namespace PKHeX.Core
{
    /// <summary> Common Ribbons introduced in Generation 4 </summary>
    public interface IRibbonSetCommon4
    {
        bool RibbonChampionSinnoh { get; set; }
        bool RibbonAlert { get; set; }
        bool RibbonShock { get; set; }
        bool RibbonDowncast { get; set; }
        bool RibbonCareless { get; set; }
        bool RibbonRelax { get; set; }
        bool RibbonSnooze { get; set; }
        bool RibbonSmile { get; set; }
        bool RibbonGorgeous { get; set; }
        bool RibbonRoyal { get; set; }
        bool RibbonGorgeousRoyal { get; set; }
        bool RibbonFootprint { get; set; }
        bool RibbonRecord { get; set; }
        bool RibbonLegend { get; set; }
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesCommon4 =
        {
            nameof(IRibbonSetCommon4.RibbonGorgeous), nameof(IRibbonSetCommon4.RibbonRoyal), nameof(IRibbonSetCommon4.RibbonGorgeousRoyal),
        };

        internal static bool[] RibbonBitsCosmetic(this IRibbonSetCommon4 set)
        {
            return new[]
            {
                set.RibbonGorgeous,
                set.RibbonRoyal,
                set.RibbonGorgeousRoyal,
            };
        }

        internal static string[] RibbonNamesCosmetic(this IRibbonSetCommon4 _) => RibbonSetNamesCommon4;

        private static readonly string[] RibbonSetNamesCommon4Only =
        {
            nameof(IRibbonSetCommon4.RibbonRecord), nameof(IRibbonSetCommon4.RibbonChampionSinnoh), nameof(IRibbonSetCommon4.RibbonLegend),
        };

        internal static bool[] RibbonBitsOnly(this IRibbonSetCommon4 set)
        {
            return new[]
            {
                set.RibbonRecord,
                set.RibbonChampionSinnoh,
                set.RibbonLegend,
            };
        }

        internal static string[] RibbonNamesOnly(this IRibbonSetCommon4 _) => RibbonSetNamesCommon4Only;

        private static readonly string[] RibbonSetNamesCommon4Daily =
        {
            nameof(IRibbonSetCommon4.RibbonAlert), nameof(IRibbonSetCommon4.RibbonShock),
            nameof(IRibbonSetCommon4.RibbonDowncast), nameof(IRibbonSetCommon4.RibbonCareless), nameof(IRibbonSetCommon4.RibbonRelax),
            nameof(IRibbonSetCommon4.RibbonSnooze), nameof(IRibbonSetCommon4.RibbonSmile),
        };

        internal static bool[] RibbonBitsDaily(this IRibbonSetCommon4 set)
        {
            return new[]
            {
                set.RibbonAlert,
                set.RibbonShock,
                set.RibbonDowncast,
                set.RibbonCareless,
                set.RibbonRelax,
                set.RibbonSnooze,
                set.RibbonSmile,
            };
        }

        internal static string[] RibbonNamesDaily(this IRibbonSetCommon4 _) => RibbonSetNamesCommon4Daily;
    }
}
