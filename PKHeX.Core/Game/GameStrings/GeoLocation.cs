using System;

namespace PKHeX.Core
{
    public static class GeoLocation
    {
        private static readonly string[][] CountryList = GetCountryList();
        private static readonly string[] lang_geo = { "ja", "en", "fr", "de", "it", "es", "zh", "ko" };
        private static readonly string[][][] RegionList = new string[CountryList.Length][][];

        private const string INVALID = nameof(INVALID);

        private static string[][] GetCountryList()
        {
            var input = Util.GetStringList("countries");
            return UnpackList(input);
        }

        private static string[][] GetRegionList(int country)
        {
            var input = Util.GetStringList($"sr_{country:000}");
            return UnpackList(input);
        }

        private static string[][] UnpackList(string[] input)
        {
            var last = GetEntry(input[input.Length - 1], out var lastIndex);
            var list = new string[lastIndex+1][];
            list[lastIndex] = last;
            for (int i = 1; i < input.Length - 1; i++)
            {
                var line = input[i];
                var entry = GetEntry(line, out var index);
                list[index] = entry;
            }
            return list;
        }

        private static string[] GetEntry(string line, out int index)
        {
            var entries = line.Split(',');
            index = int.Parse(entries[0]);
            return entries;
        }

        private static string GetCountryName(int countryID, int l)
        {
            if (l < 0)
                return INVALID;
            if (countryID >= CountryList.Length)
                return INVALID;
            var countryNames = CountryList[countryID];
            if (l < countryNames.Length)
                return countryNames[l + 1];
            return INVALID;
        }

        private static string GetRegionName(int countryID, int regionID, int l)
        {
            if (l < 0)
                return INVALID;
            if (countryID >= RegionList.Length)
                return INVALID;
            var regionstrs = RegionList[countryID] ?? (RegionList[countryID] = GetRegionList(countryID));
            if (regionID >= regionstrs.Length)
                return INVALID;
            var localized = regionstrs[regionID];
            if (l < localized.Length)
                return localized[l + 1];
            return INVALID;
        }

        /// <summary>
        /// Gets the Country string for a given Country ID
        /// </summary>
        /// <param name="language">Language ID</param>
        /// <param name="country">Country ID</param>
        /// <returns>Country ID string</returns>
        public static string GetCountryName(string language, int country) => GetCountryName(country, GetLanguageIndex(language));

        /// <summary>
        /// Gets the Region string for a specified country ID.
        /// </summary>
        /// <param name="language">Language ID</param>
        /// <param name="country">Country ID</param>
        /// <param name="region">Region ID</param>
        /// <returns>Region ID string</returns>
        public static string GetRegionName(string language, int country, int region) => GetRegionName(country, region, GetLanguageIndex(language));

        /// <summary>
        /// Gets the Country string for a given Country ID
        /// </summary>
        /// <param name="language">Language ID</param>
        /// <param name="country">Country ID</param>
        /// <returns>Country ID string</returns>
        public static string GetCountryName(LanguageID language, int country) => GetCountryName(country, GetLanguageIndex(language));

        /// <summary>
        /// Gets the Region string for a specified country ID.
        /// </summary>
        /// <param name="language">Language ID</param>
        /// <param name="country">Country ID</param>
        /// <param name="region">Region ID</param>
        /// <returns>Region ID string</returns>
        public static string GetRegionName(LanguageID language, int country, int region) => GetRegionName(country, region, GetLanguageIndex(language));

        /// <summary>
        /// Gets Country and Region strings for corresponding IDs and language.
        /// </summary>
        /// <param name="countryID">Country ID</param>
        /// <param name="regionID">Region ID</param>
        /// <param name="language">Language ID</param>
        /// <returns>Tuple containing country and region</returns>
        public static Tuple<string, string> GetCountryRegionText(int countryID, int regionID, string language)
        {
            // Get Language we're fetching for
            int lang = Array.IndexOf(lang_geo, language);
            var country = GetCountryName(countryID, lang);
            var region = GetRegionName(countryID, regionID, lang);
            return new Tuple<string, string>(country, region); // country, region
        }

        private static int GetLanguageIndex(string language) => Array.IndexOf(lang_geo, language);
        private static int GetLanguageIndex(LanguageID language) => GetLanguageIndex(language.GetLanguage2CharName());
    }
}
