using static PKHeX.Core.LanguageID;

namespace PKHeX.Core
{
    /// <summary>
    /// Provides information for <see cref="IRegionOrigin.ConsoleRegion"/> and <see cref="IRegionOrigin.Country"/> data.
    /// </summary>
    /// <remarks>These values were specific to the 3DS games (Generations 6 and 7, excluding LGP/E)</remarks>
    public static class Locale3DS
    {
        /// <summary>
        /// Compares the <see cref="IRegionOrigin.ConsoleRegion"/> and <see cref="IRegionOrigin.Country"/> to determine if the country is available within that region.
        /// </summary>
        /// <param name="consoleRegion">Console region.</param>
        /// <param name="country">Country of nationality</param>
        /// <returns>Country is within Console Region</returns>
        public static bool IsConsoleRegionCountryValid(int consoleRegion, int country) => consoleRegion switch
        {
            0 => country is 1, // Japan
            1 => country is (>= 8 and <= 52) or 153 or 156 or 168 or 174 or 186, // Americas
            2 => country is (>= 64 and <= 127) or 169 or 184 or 185, // Europe
            4 => country is 144 or 160, // China
            5 => country is 136, // Korea
            6 => country is 144 or 128, // Taiwan
            _ => false
        };

        /// <summary>
        /// Compares the <see cref="IRegionOrigin.ConsoleRegion"/> and language ID to determine if the language is available within that region.
        /// </summary>
        /// <remarks>
        /// Used to check for Virtual Console language availability. VC Games were region locked to the Console, meaning not all language games are available.
        /// </remarks>
        /// <param name="consoleRegion">Console region.</param>
        /// <param name="language">Language of region</param>
        /// <returns>Language is available within Console Region</returns>
        public static bool IsRegionLockedLanguageValidVC(int consoleRegion, int language) => consoleRegion switch
        {
            0 or 6 => language == 1, // Japan & Taiwan
            1 => language is (int)English or (int)Spanish or (int)French, // Americas
            2 => language is (int)English or (int)Spanish or (int)French or (int)German or (int)Italian, // Europe
            5 => language is (int)Korean, // Korea
            _ => false, // China & Invalid
        };

        /// <summary>
        /// Compares the <see cref="IRegionOrigin.ConsoleRegion"/> and <see cref="IRegionOrigin.Country"/> to determine if the country is available within that region.
        /// </summary>
        /// <remarks>
        /// Used to check for gift availability.
        /// </remarks>
        /// <param name="consoleRegion">Console region.</param>
        /// <param name="language">Language of region</param>
        /// <returns>Language is available within Console Region</returns>
        public static bool IsRegionLockedLanguageValid(int consoleRegion, int language) => consoleRegion switch
        {
            0 => language is (int)Japanese, // Japan & Taiwan
            1 => language is (int)English or (int)Spanish or (int)French, // Americas
            2 => language is (int)English or (int)Spanish or (int)French or (int)German or (int)Italian, // Europe
            4 => language is (int)ChineseS or (int)ChineseT, // China
            5 => language is (int)Korean, // Korea
            6 => language is (int)ChineseS or (int)ChineseT,
            _ => false, // China & Invalid
        };
    }
}
