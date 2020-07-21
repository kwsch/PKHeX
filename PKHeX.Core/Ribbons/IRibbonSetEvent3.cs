namespace PKHeX.Core
{
    /// <summary> Ribbons introduced in Generation 3 for Special Events </summary>
    public interface IRibbonSetEvent3
    {
        bool RibbonEarth { get; set; }
        bool RibbonNational { get; set; }
        bool RibbonCountry { get; set; }
        bool RibbonChampionBattle { get; set; }
        bool RibbonChampionRegional { get; set; }
        bool RibbonChampionNational { get; set; }
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesEvent3 =
        {
            nameof(IRibbonSetEvent3.RibbonEarth), nameof(IRibbonSetEvent3.RibbonNational), nameof(IRibbonSetEvent3.RibbonCountry),
            nameof(IRibbonSetEvent3.RibbonChampionBattle), nameof(IRibbonSetEvent3.RibbonChampionRegional), nameof(IRibbonSetEvent3.RibbonChampionNational)
        };

        internal static bool[] RibbonBits(this IRibbonSetEvent3 set)
        {
            return new[]
            {
                set.RibbonEarth,
                set.RibbonNational,
                set.RibbonCountry,
                set.RibbonChampionBattle,
                set.RibbonChampionRegional,
                set.RibbonChampionNational,
            };
        }

        internal static string[] RibbonNames(this IRibbonSetEvent3 _) => RibbonSetNamesEvent3;
    }
}
