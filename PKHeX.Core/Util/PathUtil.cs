using System;
using System.IO;

namespace PKHeX.Core;

public static partial class Util
{
    /// <summary>
    /// Cleans the local <see cref="fileName"/> by removing any invalid filename characters.
    /// </summary>
    /// <returns>New string without any invalid characters.</returns>
    public static string CleanFileName(string fileName)
    {
        Span<char> result = stackalloc char[fileName.Length];
        int ctr = GetCleanFileName(fileName, result);
        if (ctr == fileName.Length)
            return fileName;
        return new string(result[..ctr]);
    }

    private static int GetCleanFileName(ReadOnlySpan<char> input, Span<char> output)
    {
        ReadOnlySpan<char> invalid = Path.GetInvalidFileNameChars();
        int ctr = 0;
        foreach (var c in input)
        {
            if (!invalid.Contains(c))
                output[ctr++] = c;
        }
        return ctr;
    }
}
