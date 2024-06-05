using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Resources;

namespace PKHeX.Core;

public static partial class Util
{
    public static EmbeddedResourceCache ResourceCache { get; } = new(typeof(Util).Assembly);

    /// <summary>
    /// Expose for plugin use/reuse so that plugins can cache their own strings without needing to manage their own cache.
    /// </summary>
    /// <remarks>
    /// Assume the plugins won't wipe/modify, but if they do, that's on them.
    /// Might enable some tweaks to work like changing species names.
    /// </remarks>
    public static ConcurrentDictionary<string, string[]> CachedStrings { get; } = [];

    #region String Lists

    /// <summary>
    /// Gets a list of all Pokémon species names.
    /// </summary>
    /// <param name="language">Language of the Pokémon species names to select (e.g. "en", "fr", "jp", etc.)</param>
    /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon species name.</returns>
    public static string[] GetSpeciesList(string language) => GetStringList("species", language);

    /// <summary>
    /// Gets a list of all move names.
    /// </summary>
    /// <param name="language">Language of the move names to select (e.g. "en", "fr", "jp", etc.)</param>
    /// <returns>An array of strings whose indexes correspond to the IDs of each move name.</returns>
    public static string[] GetMovesList(string language) => GetStringList("moves", language);

    /// <summary>
    /// Gets a list of all Pokémon ability names.
    /// </summary>
    /// <param name="language">Language of the Pokémon ability names to select (e.g. "en", "fr", "jp", etc.)</param>
    /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon ability name.</returns>
    public static string[] GetAbilitiesList(string language) => GetStringList("abilities", language);

    /// <summary>
    /// Gets a list of all Pokémon nature names.
    /// </summary>
    /// <param name="language">Language of the Pokémon nature names to select (e.g. "en", "fr", "jp", etc.)</param>
    /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon nature name.</returns>
    public static string[] GetNaturesList(string language) => GetStringList("natures", language);

    /// <summary>
    /// Gets a list of all Pokémon form names.
    /// </summary>
    /// <param name="language">Language of the Pokémon form names to select (e.g. "en", "fr", "jp", etc.)</param>
    /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon form name.</returns>
    public static string[] GetFormsList(string language) => GetStringList("forms", language);

    /// <summary>
    /// Gets a list of all Pokémon type names.
    /// </summary>
    /// <param name="language">Language of the Pokémon type names to select (e.g. "en", "fr", "jp", etc.)</param>
    /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon type name.</returns>
    public static string[] GetTypesList(string language) => GetStringList("types", language);

    /// <summary>
    /// Gets a list of all Pokémon characteristic.
    /// </summary>
    /// <param name="language">Language of the Pokémon characteristic to select (e.g. "en", "fr", "jp", etc.)</param>
    /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon characteristic.</returns>
    public static string[] GetCharacteristicsList(string language) => GetStringList("character", language);

    /// <summary>
    /// Gets a list of all items.
    /// </summary>
    /// <param name="language">Language of the items to select (e.g. "en", "fr", "jp", etc.)</param>
    /// <returns>An array of strings whose indexes correspond to the IDs of each item.</returns>
    public static string[] GetItemsList(string language) => GetStringList("items", language);

    /// <summary>
    /// Retrieves the localization index list for all requested strings for the <see cref="fileName"/> through Spanish.
    /// </summary>
    /// <param name="fileName">Base file name</param>
    /// <remarks>Ignores Korean Language.</remarks>
    public static string[][] GetLanguageStrings7([ConstantExpected] string fileName) =>
    [
        [], // 0 - None
        GetStringList(fileName, "ja"), // 1
        GetStringList(fileName, "en"), // 2
        GetStringList(fileName, "fr"), // 3
        GetStringList(fileName, "it"), // 4
        GetStringList(fileName, "de"), // 5
        [], // 6 - None
        GetStringList(fileName, "es"), // 7
    ];

    /// <summary>
    /// Retrieves the localization index list for all requested strings for the <see cref="fileName"/> through Korean.
    /// </summary>
    /// <param name="fileName">Base file name</param>
    public static string[][] GetLanguageStrings8([ConstantExpected] string fileName) =>
    [
        [], // 0 - None
        GetStringList(fileName, "ja"), // 1
        GetStringList(fileName, "en"), // 2
        GetStringList(fileName, "fr"), // 3
        GetStringList(fileName, "it"), // 4
        GetStringList(fileName, "de"), // 5
        [], // 6 - None
        GetStringList(fileName, "es"), // 7
        GetStringList(fileName, "ko"), // 8
    ];

    /// <summary>
    /// Retrieves the localization index list for all requested strings for the <see cref="fileName"/> through Chinese.
    /// </summary>
    /// <param name="fileName">Base file name</param>
    /// <param name="zh2">String to use for the second Chinese localization.</param>
    public static string[][] GetLanguageStrings10([ConstantExpected] string fileName, string zh2 = "zh") =>
    [
        [], // 0 - None
        GetStringList(fileName, "ja"), // 1
        GetStringList(fileName, "en"), // 2
        GetStringList(fileName, "fr"), // 3
        GetStringList(fileName, "it"), // 4
        GetStringList(fileName, "de"), // 5
        [], // 6 - None
        GetStringList(fileName, "es"), // 7
        GetStringList(fileName, "ko"), // 8
        GetStringList(fileName, "zh"), // 9
        GetStringList(fileName, zh2), // 10
    ];

    #endregion

    /// <inheritdoc cref="GetStringList(string, EmbeddedResourceCache)"/>
    public static string[] GetStringList(string fileName)
    {
        if (CachedStrings.TryGetValue(fileName, out var result))
            return result;
        return LoadAndCache(fileName, ResourceCache);
    }

    /// <summary>
    /// Gets a string array from an assembly's resources.
    /// </summary>
    /// <remarks>Caches the result array for future fetches of the same resource.</remarks>
    public static string[] GetStringList(string fileName, EmbeddedResourceCache src)
    {
        if (CachedStrings.TryGetValue(fileName, out var result))
            return result;
        return LoadAndCache(fileName, src);
    }

    private static string[] LoadAndCache(string fileName, EmbeddedResourceCache src)
    {
        if (!src.TryGetStringResource(fileName, out var txt)) // Fetch File, \n to list.
            return []; // Instead of throwing an exception, return empty.
        var result = FastSplit(txt); // could just string.Split but we know ours are \n or \r\n
        CachedStrings.TryAdd(fileName, result);
        return result;
    }

    public static string[] GetStringList(string fileName, string lang2char, [ConstantExpected] string type = "text") => GetStringList(GetFullResourceName(fileName, lang2char, type));

    private static string GetFullResourceName(string fileName, string lang2char, [ConstantExpected] string type) => $"{type}_{fileName}_{lang2char}";

    public static byte[] GetBinaryResource(string name)
    {
        if (!ResourceCache.TryGetBinaryResource(name, out var result))
            throw new MissingManifestResourceException($"Resource not found: {name}");
        return result;
    }

    public static string GetStringResource(string name)
    {
        if (!ResourceCache.TryGetStringResource(name, out var result))
            throw new MissingManifestResourceException($"Resource not found: {name}");
        return result;
    }

    private static string[] FastSplit(ReadOnlySpan<char> s)
    {
        // Get Count
        if (s.Length == 0)
            return [];

        var count = 1 + s.Count('\n');
        var result = new string[count];

        var i = 0;
        while (true)
        {
            var index = s.IndexOf('\n');
            var length = index == -1 ? s.Length : index;
            var slice = s[..length];
            if (slice.Length != 0 && slice[^1] == '\r')
                slice = slice[..^1];

            result[i++] = slice.ToString();
            if (i == count)
                return result;
            s = s[(index + 1)..];
        }
    }
}
