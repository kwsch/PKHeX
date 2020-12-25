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
                0 => country is 1, // Japan
                1 => (8 <= country && country <= 52) || (country is 153 or 156 or 168 or 174 or 186), // Americas
                2 => (64 <= country && country <= 127) || (country is 169 or 184 or 185), // Europe
                4 => country is 144 or 160, // China
                5 => country is 136, // Korea
                6 => country is 144 or 128, // Taiwan
                _ => false
            };
        }
    }
}
