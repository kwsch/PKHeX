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

    /// <summary>
    /// Splits the specified <see cref="ReadOnlySpan{T}"/> of characters into an array of strings, using newline characters ('\n') as delimiters.
    /// </summary>
    /// <remarks>
    /// This method is optimized for performance and avoids unnecessary allocations by working directly with spans.
    /// It is suitable for scenarios where splitting large text data into lines is required.
    /// </remarks>
    /// <param name="s">The span of characters to split. Can include '\n' and '\r\n' as line breaks.</param>
    /// <returns>
    /// An array of strings, where each element represents a line of text from the input span.
    /// Lines ending with a carriage return ('\r') will have the '\r' removed.
    /// Returns an empty array if the input span is empty.
    /// </returns>
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
