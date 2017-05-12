namespace PKHeX.Core
{
    internal interface IRibbonSet1 // Gen3+
    {
        bool RibbonEarth            { get; set; }
        bool RibbonNational         { get; set; }
        bool RibbonCountry          { get; set; }
        bool RibbonChampionBattle   { get; set; }
        bool RibbonChampionRegional { get; set; }
        bool RibbonChampionNational { get; set; }
    }
    internal interface IRibbonSet2 // Gen4+
    {
        bool RibbonClassic   { get; set; }
        bool RibbonWishing   { get; set; }
        bool RibbonPremier   { get; set; }
        bool RibbonEvent     { get; set; }
        bool RibbonBirthday  { get; set; }
        bool RibbonSpecial   { get; set; }
        bool RibbonWorld     { get; set; }
        bool RibbonChampionWorld { get; set; }
        bool RibbonSouvenir  { get; set; }
    }

    internal static class RibbonSetHelper
    {
        public static readonly string[] RibbonNames1 =
        {
            nameof(IRibbonSet1.RibbonEarth), nameof(IRibbonSet1.RibbonNational), nameof(IRibbonSet1.RibbonCountry),
            nameof(IRibbonSet1.RibbonChampionBattle), nameof(IRibbonSet1.RibbonChampionRegional), nameof(IRibbonSet1.RibbonChampionNational)
        };
        public static bool[] getRibbonBits(IRibbonSet1 set)
        {
            if (set == null)
                return new bool[6];
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
        public static readonly string[] RibbonNames2 =
        {
            nameof(IRibbonSet2.RibbonClassic), nameof(IRibbonSet2.RibbonWishing), nameof(IRibbonSet2.RibbonPremier),
            nameof(IRibbonSet2.RibbonEvent), nameof(IRibbonSet2.RibbonBirthday), nameof(IRibbonSet2.RibbonSpecial),
            nameof(IRibbonSet2.RibbonWorld), nameof(IRibbonSet2.RibbonChampionWorld), nameof(IRibbonSet2.RibbonSouvenir)
        };
        public static bool[] getRibbonBits(IRibbonSet2 set)
        {
            if (set == null)
                return new bool[9];
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

        public static string getRibbonNames(IRibbonSet1 set, int index) => RibbonNames1[index];
        public static string getRibbonNames(IRibbonSet2 set, int index) => RibbonNames2[index];
        public static string[] getRibbonNames(IRibbonSet1 set) => RibbonNames1;
        public static string[] getRibbonNames(IRibbonSet2 set) => RibbonNames2;
    }
}
