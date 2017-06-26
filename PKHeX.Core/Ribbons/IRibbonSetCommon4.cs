namespace PKHeX.Core
{
    /// <summary> Common Ribbons introduced in Generation 4 </summary>
    internal interface IRibbonSetCommon4
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
            nameof(IRibbonSetCommon4.RibbonChampionSinnoh), nameof(IRibbonSetCommon4.RibbonAlert), nameof(IRibbonSetCommon4.RibbonShock),
            nameof(IRibbonSetCommon4.RibbonDowncast), nameof(IRibbonSetCommon4.RibbonCareless), nameof(IRibbonSetCommon4.RibbonRelax),
            nameof(IRibbonSetCommon4.RibbonSnooze), nameof(IRibbonSetCommon4.RibbonSmile), nameof(IRibbonSetCommon4.RibbonGorgeous),
            nameof(IRibbonSetCommon4.RibbonRoyal), nameof(IRibbonSetCommon4.RibbonGorgeousRoyal), nameof(IRibbonSetCommon4.RibbonFootprint),
            nameof(IRibbonSetCommon4.RibbonRecord), nameof(IRibbonSetCommon4.RibbonLegend),
        };
        internal static bool[] RibbonBits(this IRibbonSetCommon4 set)
        {
            if (set == null)
                return new bool[15];
            return new[]
            {
                set.RibbonChampionSinnoh,
                set.RibbonAlert,
                set.RibbonShock,
                set.RibbonDowncast,
                set.RibbonCareless,
                set.RibbonRelax,
                set.RibbonSnooze,
                set.RibbonSmile,
                set.RibbonGorgeous,
                set.RibbonRoyal,
                set.RibbonGorgeousRoyal,
                set.RibbonFootprint,
                set.RibbonRecord,
                set.RibbonLegend,
            };
        }
        internal static string[] RibbonNames(this IRibbonSetCommon4 set) => RibbonSetNamesCommon4;
        private static readonly string[] RibbonSetNamesCommon4Only =
        {
            nameof(IRibbonSetCommon4.RibbonChampionSinnoh), nameof(IRibbonSetCommon4.RibbonAlert), nameof(IRibbonSetCommon4.RibbonShock),
            nameof(IRibbonSetCommon4.RibbonDowncast), nameof(IRibbonSetCommon4.RibbonCareless), nameof(IRibbonSetCommon4.RibbonRelax),
            nameof(IRibbonSetCommon4.RibbonSnooze), nameof(IRibbonSetCommon4.RibbonSmile),
            nameof(IRibbonSetCommon4.RibbonRecord), nameof(IRibbonSetCommon4.RibbonLegend),

            // nameof(IRibbonSetCommon4.RibbonGorgeous), nameof(IRibbonSetCommon4.RibbonRoyal), nameof(IRibbonSetCommon4.RibbonGorgeousRoyal), 
            // nameof(IRibbonSetCommon4.RibbonFootprint),
        };
        internal static bool[] RibbonBitsOnly(this IRibbonSetCommon4 set)
        {
            if (set == null)
                return new bool[10];
            return new[]
            {
                set.RibbonChampionSinnoh,
                set.RibbonAlert,
                set.RibbonShock,
                set.RibbonDowncast,
                set.RibbonCareless,
                set.RibbonRelax,
                set.RibbonSnooze,
                set.RibbonSmile,
                set.RibbonRecord,
                set.RibbonLegend,
            };
        }
        internal static string[] RibbonNamesOnly(this IRibbonSetCommon4 set) => RibbonSetNamesCommon4Only;
    }
}
