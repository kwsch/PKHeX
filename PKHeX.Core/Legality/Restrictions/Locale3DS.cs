using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Provides information for <see cref="IRegionOrigin.ConsoleRegion"/> and <see cref="IRegionOrigin.Country"/> data.
    /// </summary>
    public static class Locale3DS
    {
        /// <summary>
        /// Compares the <see cref="IRegionOrigin.ConsoleRegion"/> and <see cref="IRegionOrigin.Country"/> to determine if the country is available within that region.
        /// </summary>
        /// <param name="consoleRegion">Console region.</param>
        /// <param name="country">Country of nationality</param>
        /// <returns>Country is within Console Region</returns>
        public static bool IsConsoleRegionCountryValid(int consoleRegion, int country)
        {
            return consoleRegion switch
            {
                0 => (country == 1), // Japan
                1 => ((8 <= country && country <= 52) || ExtendedAmericas.Contains(country)), // Americas
                2 => ((64 <= country && country <= 127) || ExtendedEurope.Contains(country)), // Europe
                4 => (country == 144 || country == 160), // China
                5 => (country == 136), // Korea
                6 => (country == 144 || country == 128), // Taiwan
                _ => false
            };
        }

        private static readonly HashSet<int> ExtendedAmericas = new() { 153, 156, 168, 174, 186 };
        private static readonly HashSet<int> ExtendedEurope = new() { 169, 184, 185 };
    }
}
