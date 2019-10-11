namespace PKHeX.Core
{
    /// <summary> Ribbons introduced in Generation 4 for Special Events </summary>
    public interface IRibbonSetEvent4
    {
        bool RibbonClassic { get; set; }
        bool RibbonWishing { get; set; }
        bool RibbonPremier { get; set; }
        bool RibbonEvent { get; set; }
        bool RibbonBirthday { get; set; }
        bool RibbonSpecial { get; set; }
        bool RibbonWorld { get; set; }
        bool RibbonChampionWorld { get; set; }
        bool RibbonSouvenir { get; set; }
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesEvent4 =
        {
            nameof(IRibbonSetEvent4.RibbonClassic), nameof(IRibbonSetEvent4.RibbonWishing), nameof(IRibbonSetEvent4.RibbonPremier),
            nameof(IRibbonSetEvent4.RibbonEvent), nameof(IRibbonSetEvent4.RibbonBirthday), nameof(IRibbonSetEvent4.RibbonSpecial),
            nameof(IRibbonSetEvent4.RibbonWorld), nameof(IRibbonSetEvent4.RibbonChampionWorld), nameof(IRibbonSetEvent4.RibbonSouvenir)
        };

        internal static bool[] RibbonBits(this IRibbonSetEvent4 set)
        {
            return new[]
            {
                set.RibbonClassic,
                set.RibbonWishing,
                set.RibbonPremier,
                set.RibbonEvent,
                set.RibbonBirthday,
                set.RibbonSpecial,
                set.RibbonWorld,
                set.RibbonChampionWorld,
                set.RibbonSouvenir,
            };
        }

        internal static string[] RibbonNames(this IRibbonSetEvent4 _) => RibbonSetNamesEvent4;
    }
}
