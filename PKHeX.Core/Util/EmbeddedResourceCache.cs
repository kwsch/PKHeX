using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace PKHeX.Core;

/// <summary>
/// Wrapper for reading resources embedded in an assembly with some considerations for faster retrieval and case insensitivity.
/// </summary>
public sealed class EmbeddedResourceCache
{
    private readonly Assembly Assembly;

    /// <summary>
    /// Remember the resource names for quick lookup.
    /// </summary>
    private readonly Dictionary<string, string> ResourceNameMap;

    public EmbeddedResourceCache(Assembly assembly)
    {
        Assembly = assembly;
        ResourceNameMap = BuildLookup(Assembly.GetManifestResourceNames());
    }

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
        System.Diagnostics.Debug.Assert(start != 0); // should have a period in the name

        // text file fetch excludes ".txt" (mixed case...); other extensions are used (all lowercase).
        return resName.EndsWith(".txt", StringComparison.Ordinal) ? resName[start..^4].ToLowerInvariant() : resName[start..];
    }

    /// <summary>
    /// Fetches a string resource from this assembly.
    /// </summary>
    public bool TryGetStringResource(string name, [NotNullWhen(true)] out string? result)
    {
        if (!ResourceNameMap.TryGetValue(name.ToLowerInvariant(), out result))
            return false;

        using var resource = Assembly.GetManifestResourceStream(result);
        if (resource is null)
            return false;
        using var reader = new StreamReader(resource);
        result = reader.ReadToEnd();
        return true;
    }

    /// <summary>
    /// Fetches a byte array resource from this assembly.
    /// </summary>
    public bool TryGetBinaryResource(string name, [NotNullWhen(true)] out byte[]? result)
    {
        result = null;
        if (!ResourceNameMap.TryGetValue(name, out var resName))
            return false;

        using var resource = Assembly.GetManifestResourceStream(resName);
        if (resource is null)
            return false;

        result = new byte[resource.Length];
        resource.ReadExactly(result);
        return true;
    }

    /// <summary>
    /// Fetches a byte array resource from this assembly.
    /// </summary>
    public bool TryGetBinaryResource(string name, ArrayPool<byte> provider, [NotNullWhen(true)] out byte[]? result, out int length)
    {
        length = -1;
        result = null;
        if (!ResourceNameMap.TryGetValue(name, out var resName))
            return false;

        using var resource = Assembly.GetManifestResourceStream(resName);
        if (resource is null)
            return false;

        result = provider.Rent(length = (int)resource.Length);
        resource.ReadExactly(result, 0, length);
        return true;
    }
}
