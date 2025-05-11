using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Logic for interacting with Entity file extensions.
/// </summary>
public static class EntityFileExtension
{
    // All side-game formats that don't follow the usual pk* format
    private const string ExtensionSK2 = "sk2";
    private const string ExtensionCK3 = "ck3";
    private const string ExtensionXK3 = "xk3";
    private const string ExtensionBK4 = "bk4";
    private const string ExtensionRK4 = "rk4";
    private const string ExtensionPB7 = "pb7";
    private const string ExtensionPB8 = "pb8";
    private const string ExtensionPA8 = "pa8";
    private const int CountExtra = 8;

    /// <summary>
    /// Valid file extensions that represent <see cref="PKM"/> data, without the leading '.'
    /// </summary>
    private static string[] Extensions => GetExtensions();

    /// <summary>
    /// Gets an array of valid <see cref="PKM"/> file extensions.
    /// </summary>
    /// <param name="maxGeneration">Maximum Generation to permit</param>
    /// <returns>Valid <see cref="PKM"/> file extensions.</returns>
    public static string[] GetExtensions(int maxGeneration = Latest.Generation)
    {
        int min = maxGeneration is <= 2 or >= 7 ? 1 : 3;
        int size = maxGeneration - min + 1 + CountExtra;
        var result = new List<string>(size);
        for (int i = min; i <= maxGeneration; i++)
            result.Add($"pk{i}");
        if (min < 3)
            result.Add(ExtensionSK2); // stadium

        if (maxGeneration >= 3)
        {
            result.Add(ExtensionCK3); // colosseum
            result.Add(ExtensionXK3); // xd
        }
        if (maxGeneration >= 4)
        {
            result.Add(ExtensionBK4); // battle revolution
            result.Add(ExtensionRK4); // My Pokemon Ranch
        }
        if (maxGeneration >= 7)
            result.Add(ExtensionPB7); // let's go
        if (maxGeneration >= 8)
            result.Add(ExtensionPB8); // Brilliant Diamond & Shining Pearl
        if (maxGeneration >= 8)
            result.Add(ExtensionPA8); // Legends: Arceus

        return [.. result];
    }

    /// <summary>
    /// Roughly detects the PKM format from the file's extension.
    /// </summary>
    /// <param name="ext">File extension.</param>
    /// <param name="prefer">Preference if not a valid extension, usually the highest acceptable format.</param>
    /// <returns>Format hint that the file is.</returns>
    public static EntityContext GetContextFromExtension(ReadOnlySpan<char> ext, EntityContext prefer = EntityContext.None)
    {
        if (ext.Length == 0)
            return prefer;

        static bool Is(ReadOnlySpan<char> ext, ReadOnlySpan<char> str) => ext.EndsWith(str, StringComparison.InvariantCultureIgnoreCase);
        if (Is(ext, "a8")) return EntityContext.Gen8a;
        if (Is(ext, "b8")) return EntityContext.Gen8b;
        if (Is(ext, "k8")) return EntityContext.Gen8;
        if (Is(ext, "b7")) return EntityContext.Gen7b;
        if (Is(ext, "k7")) return EntityContext.Gen7;
        if (Is(ext, "k6")) return EntityContext.Gen6;

        return (EntityContext)GetFormatFromExtension(ext[^1], prefer);
    }

    /// <summary>
    /// Roughly detects the PKM format from the file's extension.
    /// </summary>
    /// <param name="last">Last character of the file's extension.</param>
    /// <param name="prefer">Preference if not a valid extension, usually the highest acceptable format.</param>
    /// <returns>Format hint that the file is.</returns>
    private static int GetFormatFromExtension(char last, EntityContext prefer)
    {
        if (last is >= '1' and <= '9')
            return last - '0';
        if (prefer.Generation() <= 7 && last == 'x')
            return 6;
        return (int)prefer;
    }

    public static IReadOnlyList<string> Extensions7b => [ExtensionPB7];
    public static IReadOnlyList<string> GetExtensionsAll() => Extensions;
    public static IReadOnlyList<string> GetExtensionsHOME() => Extensions;
    public static IReadOnlyList<string> GetExtensionsAtOrBelow(int specific)
        => Array.FindAll(Extensions, f => IsAtOrBelow(f, specific));
    public static IReadOnlyList<string> GetExtensionsAtOrBelow(int specific, string exclude)
        => Array.FindAll(Extensions, f => IsAtOrBelow(f, specific) && !exclude.Contains(f));
    private static bool IsAtOrBelow(ReadOnlySpan<char> ext, int specific)
        => IsAtOrBelow(specific, ext[^1] - 0x30);
    private static bool IsAtOrBelow(int specific, int gen) => gen <= specific;
}
