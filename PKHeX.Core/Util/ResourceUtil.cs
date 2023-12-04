using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace PKHeX.Core;

public static partial class Util
{
    private static readonly Assembly thisAssembly = typeof(Util).GetTypeInfo().Assembly;
    private static readonly Dictionary<string, string> resourceNameMap = BuildLookup(thisAssembly.GetManifestResourceNames());

    private static Dictionary<string, string> BuildLookup(ReadOnlySpan<string> manifestNames)
    {
        var result = new Dictionary<string, string>(manifestNames.Length);
        foreach (var resName in manifestNames)
        {
            var fileName = GetFileName(resName);
            result.Add(fileName, resName);
        }
        return result;
    }

    private static string GetFileName(string resName)
    {
        var period = resName.LastIndexOf('.', resName.Length - 5);
        var start = period + 1;
        System.Diagnostics.Debug.Assert(start != 0);

        // text file fetch excludes ".txt" (mixed case...); other extensions are used (all lowercase).
        return resName.EndsWith(".txt", StringComparison.Ordinal) ? resName[start..^4].ToLowerInvariant() : resName[start..];
    }

    private static readonly Dictionary<string, string[]> stringListCache = [];

    private static readonly object getStringListLoadLock = new();

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
    public static string[][] GetLanguageStrings7(string fileName) =>
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
    public static string[][] GetLanguageStrings8(string fileName) =>
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
    public static string[][] GetLanguageStrings10(string fileName, string zh2 = "zh") =>
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

    public static string[] GetStringList(string fileName)
    {
        if (IsStringListCached(fileName, out var result))
            return result;
        var txt = GetStringResource(fileName); // Fetch File, \n to list.
        return LoadStringList(fileName, txt);
    }

    public static bool IsStringListCached(string fileName, [NotNullWhen(true)] out string[]? result)
    {
        lock (getStringListLoadLock) // Make sure only one thread can read the cache
            return stringListCache.TryGetValue(fileName, out result);
    }

    /// <summary>
    /// Loads a text <see cref="file"/> into the program with a value of <see cref="txt"/>.
    /// </summary>
    /// <remarks>Caches the result array for future fetches.</remarks>
    public static string[] LoadStringList(string file, string? txt)
    {
        if (txt == null)
            return [];
        string[] raw = FastSplit(txt);

        // Make sure only one thread can write to the cache
        lock (getStringListLoadLock)
            stringListCache.TryAdd(file, raw);
        return raw;
    }

    public static string[] GetStringList(string fileName, string lang2char, string type = "text") => GetStringList(GetFullResourceName(fileName, lang2char, type));

    private static string GetFullResourceName(string fileName, string lang2char, string type) => $"{type}_{fileName}_{lang2char}";

    public static byte[] GetBinaryResource(string name)
    {
        if (!resourceNameMap.TryGetValue(name, out var resName))
            return [];

        using var resource = thisAssembly.GetManifestResourceStream(resName);
        if (resource is null)
            return [];

        var buffer = new byte[resource.Length];
        resource.ReadExactly(buffer);
        return buffer;
    }

    public static string? GetStringResource(string name)
    {
        if (!resourceNameMap.TryGetValue(name.ToLowerInvariant(), out var resourceName))
            return null;

        using var resource = thisAssembly.GetManifestResourceStream(resourceName);
        if (resource is null)
            return null;
        using var reader = new StreamReader(resource);
        return reader.ReadToEnd();
    }

    private static string[] FastSplit(ReadOnlySpan<char> s)
    {
        // Get Count
        if (s.Length == 0)
            return [];

        var count = GetCount(s);
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

    private static int GetCount(ReadOnlySpan<char> s)
    {
        int count = 1;
        while (true)
        {
            var index = s.IndexOf('\n');
            if (index == -1)
                return count;
            count++;
            s = s[(index+1)..];
        }
    }
}
