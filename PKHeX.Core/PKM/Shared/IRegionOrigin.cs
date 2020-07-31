namespace PKHeX.Core
{
    public interface IRegionOrigin
    {
        int ConsoleRegion { get; set; }
        int Country { get; set; }
        int Region { get; set; }
    }

    public static partial class Extensions
    {
        public static void SetDefaultRegionOrigins(this IRegionOrigin o)
        {
            o.ConsoleRegion = 1; // North America
            o.Region = 7; // California
            o.Country = 49; // USA
        }
    }
}
