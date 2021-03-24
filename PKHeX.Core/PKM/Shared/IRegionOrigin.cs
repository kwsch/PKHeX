namespace PKHeX.Core
{
    public interface IRegionOrigin
    {
        /// <summary> Console hardware region. </summary>
        /// <see cref="RegionID"/>
        int ConsoleRegion { get; set; }
        /// <summary> Console's configured Country via System Settings. </summary>
        int Country { get; set; }
        /// <summary> Console's configured Region within <see cref="Country"/> via System Settings. </summary>
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
