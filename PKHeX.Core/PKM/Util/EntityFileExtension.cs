using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public static class EntityFileExtension
{
    private const string ExtensionPB7 = "pb7";
    private const string ExtensionPB8 = "pb8";
    private const string ExtensionPA8 = "pa8";

    public static IReadOnlyList<string> Extensions7b => new[] { ExtensionPB7 };

    /// <summary>
    /// Gets an array of valid <see cref="PKM"/> file extensions.
    /// </summary>
    /// <param name="maxGeneration">Maximum Generation to permit</param>
    /// <returns>Valid <see cref="PKM"/> file extensions.</returns>
    public static string[] GetExtensions(int maxGeneration = PKX.Generation)
    {
        int min = maxGeneration is <= 2 or >= 7 ? 1 : 3;
        int size = maxGeneration - min + 1 + 6;
        var result = new List<string>(size);
        for (int i = min; i <= maxGeneration; i++)
            result.Add($"pk{i}");

        if (maxGeneration >= 3)
        {
            result.Add("ck3"); // colosseum
            result.Add("xk3"); // xd
        }
        if (maxGeneration >= 4)
        {
            result.Add("bk4"); // battle revolution
            result.Add("rk4"); // My Pokemon Ranch
        }
        if (maxGeneration >= 7)
            result.Add(ExtensionPB7); // let's go
        if (maxGeneration >= 8)
            result.Add(ExtensionPB8); // Brilliant Diamond & Shining Pearl
        if (maxGeneration >= 8)
            result.Add(ExtensionPA8); // Legends: Arceus

        return result.ToArray();
    }

    /// <summary>
    /// Roughly detects the PKM format from the file's extension.
    /// </summary>
    /// <param name="ext">File extension.</param>
    /// <param name="prefer">Preference if not a valid extension, usually the highest acceptable format.</param>
    /// <returns>Format hint that the file is.</returns>
    public static EntityContext GetContextFromExtension(string ext, EntityContext prefer = EntityContext.None)
    {
        if (ext.Length == 0)
            return prefer;

        static bool Is(string ext, string str) => ext.EndsWith(str, StringComparison.InvariantCultureIgnoreCase);
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
}
