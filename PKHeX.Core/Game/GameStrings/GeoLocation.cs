using System;

namespace PKHeX.Core;

/// <summary>
/// Geolocation Utility for Generation 6/7 (3DS) Earth location values.
/// </summary>
public static class GeoLocation
{
    private static readonly string[]?[] CountryList = GetCountryList();
    private static readonly string[] lang_geo = ["ja", "en", "fr", "de", "it", "es", "zh", "ko", "zh2"];
    private static readonly string[]?[]?[] RegionList = new string[CountryList.Length][][];

    /// <summary>
    /// Returns the index of which the <see cref="language"/> is in the country/region list.
    /// </summary>
    public static int GetLanguageIndex(string language) => Array.IndexOf(lang_geo, language);
    private static int GetLanguageIndex(LanguageID language) => GetLanguageIndex(language.GetLanguage2CharName());

    private const string INVALID = nameof(INVALID);

    private static string[]?[] GetCountryList()
    {
        var input = Util.GetStringList("countries");
        return UnpackList(input);
    }

    private static string[]?[] GetRegionList(byte country)
    {
        var input = Util.GetStringList($"sr_{country:000}");
        return UnpackList(input);
    }

    private static string[]?[] UnpackList(ReadOnlySpan<string> input)
    {
        var last = GetEntry(input[^1], out var lastIndex);
        string[]?[] list = new string[lastIndex+1][];
        list[lastIndex] = last;
        foreach (var line in input[..^1])
        {
            var entry = GetEntry(line, out var index);
            list[index] = entry;
        }
        return list;
    }

    private static string[] GetEntry(string line, out int index)
    {
        var entries = line.Split('\t');
        index = int.Parse(entries[0]);
        return entries;
    }

    private static string GetCountryName(byte country, int l)
    {
        if (l < 0)
            return INVALID;
        if ((uint)country >= CountryList.Length)
            return INVALID;
        var countryNames = CountryList[country];
        if (countryNames is not null && (uint)l < countryNames.Length)
            return countryNames[l + 1];
        return INVALID;
    }

    private static string GetRegionName(byte country, byte region, int l)
    {
        if (l < 0)
            return INVALID;
        if ((uint)country >= RegionList.Length)
            return INVALID;
        var regionNames = RegionList[country] ??= GetRegionList(country);
        if ((uint)region >= regionNames.Length)
            return INVALID;
        var localized = regionNames[region];
        if (localized is not null && (uint)l < localized.Length)
            return localized[l + 1];
        return INVALID;
    }

    /// <summary>
    /// Gets an array of all country names for the requested <see cref="language"/>.
    /// </summary>
    public static string[]? GetCountryList(string language)
    {
        int index = GetLanguageIndex(language);
        return CountryList[index];
    }

    /// <summary>
    /// Gets the Country string for a given Country ID
    /// </summary>
    /// <param name="language">Language ID</param>
    /// <param name="country">Country ID</param>
    /// <returns>Country ID string</returns>
    public static string GetCountryName(string language, byte country) => GetCountryName(country, GetLanguageIndex(language));

    /// <summary>
    /// Gets the Region string for a specified country ID.
    /// </summary>
    /// <param name="language">Language ID</param>
    /// <param name="country">Country ID</param>
    /// <param name="region">Region ID</param>
    /// <returns>Region ID string</returns>
    public static string GetRegionName(string language, byte country, byte region) => GetRegionName(country, region, GetLanguageIndex(language));

    /// <summary>
    /// Gets the Country string for a given Country ID
    /// </summary>
    /// <param name="language">Language ID</param>
    /// <param name="country">Country ID</param>
    /// <returns>Country ID string</returns>
    public static string GetCountryName(LanguageID language, byte country) => GetCountryName(country, GetLanguageIndex(language));

    /// <summary>
    /// Gets the Region string for a specified country ID.
    /// </summary>
    /// <param name="language">Language ID</param>
    /// <param name="country">Country ID</param>
    /// <param name="region">Region ID</param>
    /// <returns>Region ID string</returns>
    public static string GetRegionName(LanguageID language, byte country, byte region) => GetRegionName(country, region, GetLanguageIndex(language));

    /// <summary>
    /// Checks if the Country and Region exist for selection.
    /// </summary>
    /// <param name="country">Country ID</param>
    /// <param name="region">Region ID</param>
    /// <returns>True if exists</returns>
    public static bool GetIsCountryRegionExist(byte country, byte region)
    {
        if ((uint)country >= RegionList.Length)
            return false;
        var regionNames = RegionList[country] ??= GetRegionList(country);
        return (uint)region < regionNames.Length && regionNames[region] is not null;
    }

    /// <summary>
    /// Gets Country and Region strings for corresponding IDs and language.
    /// </summary>
    /// <param name="country">Country ID</param>
    /// <param name="region">Region ID</param>
    /// <param name="language">Language ID</param>
    /// <returns>Tuple containing country and region</returns>
    public static (string Country, string Region) GetCountryRegionText(byte country, byte region, string language)
    {
        // Get Language we're fetching for
        int lang = Array.IndexOf(lang_geo, language);
        var countryName = GetCountryName(country, lang);
        var regionName = GetRegionName(country, region, lang);
        return (countryName, regionName);
    }
}
