using System.Globalization;
using System.Text;

namespace PKHeX.Web.Services;

/// <summary>
/// Turns untrusted file names (from the picker, a drop, or a stored session) into names that are safe to display and to offer as a download.
/// </summary>
/// <remarks>
/// The rules do not depend on the host OS: <see cref="Path.GetInvalidFileNameChars"/> only lists <c>/</c> and <c>\0</c> on Unix and WASM,
/// but a downloaded file can end up on any filesystem. Extensions are never added or changed, because consoles restore saves by exact name
/// (XY/ORAS use the extensionless <c>main</c>).
/// </remarks>
public static class FileNaming
{
    /// <summary>Name of the raw XY/ORAS save file, used when no usable name is available.</summary>
    public const string DefaultSaveName = "main";

    /// <summary>Longest sanitised name, in UTF-16 code units.</summary>
    public const int MaxLength = 120;

    /// <summary>Longest extension (including the dot) that is kept intact when a name is shortened.</summary>
    private const int MaxKeptExtension = 16;

    /// <summary>Characters reserved on Windows, replaced rather than removed so that word boundaries survive.</summary>
    private const string Reserved = "<>:\"|?*";

    /// <summary>Windows device names, which cannot be used as a file name with or without an extension.</summary>
    private static readonly HashSet<string> DeviceNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL",
        "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
        "COM\u00B9", "COM\u00B2", "COM\u00B3", "LPT\u00B9", "LPT\u00B2", "LPT\u00B3", "CONIN$", "CONOUT$",
    };

    /// <summary>Characters that never belong in a name: controls, invisible formatting (including bidirectional overrides) and line or paragraph separators.</summary>
    private static bool IsRemoved(UnicodeCategory category) => category is UnicodeCategory.Control or UnicodeCategory.Format
        or UnicodeCategory.LineSeparator or UnicodeCategory.ParagraphSeparator;

    /// <summary>
    /// Returns a display- and download-safe version of <paramref name="raw"/>.
    /// </summary>
    /// <remarks>
    /// Keeps only the last path segment, removes control, format and line-separator characters (including bidirectional overrides)
    /// and unpaired surrogates, replaces Windows-reserved characters with <c>_</c>, trims surrounding whitespace and dots,
    /// prefixes Windows device names with <c>_</c> and caps the length at <see cref="MaxLength"/>, keeping a short extension and never
    /// splitting a surrogate pair. The result is stable: sanitising it again returns it unchanged.
    /// </remarks>
    /// <param name="raw">Untrusted name, possibly null or empty.</param>
    /// <param name="fallback">Returned when nothing usable remains. It is not sanitised.</param>
    public static string Sanitize(string? raw, string fallback = DefaultSaveName)
    {
        if (string.IsNullOrEmpty(raw))
        {
            return fallback;
        }

        var segment = raw[(raw.LastIndexOfAny(['/', '\\']) + 1)..];
        var result = new StringBuilder(segment.Length);
        for (int i = 0; i < segment.Length; i++)
        {
            var c = segment[i];
            if (char.IsSurrogate(c))
            {
                // Keep whole pairs only; browsers replace an unpaired surrogate, so the downloaded name would differ from the one shown.
                if (char.IsHighSurrogate(c) && i + 1 < segment.Length && char.IsLowSurrogate(segment[i + 1]))
                {
                    result.Append(c).Append(segment[++i]);
                }
                continue;
            }
            if (IsRemoved(char.GetUnicodeCategory(c)))
            {
                continue;
            }
            result.Append(Reserved.Contains(c) ? '_' : c);
        }

        var name = Shorten(TrimEnds(result.ToString()));
        // Checked after shortening, since a cut can expose a device name. A stem starting with '_' can never be one, so this runs once.
        if (DeviceNames.Contains(name.Split('.')[0].TrimEnd()))
        {
            name = Shorten("_" + name);
        }
        return name.Length == 0 ? fallback : name;
    }

    /// <summary>Removes surrounding whitespace and dots, repeating until neither is left.</summary>
    /// <remarks>Browsers strip leading dots when saving a download, so a kept leading dot would make the offered name differ from the saved one.</remarks>
    private static string TrimEnds(string name)
    {
        string previous;
        do
        {
            previous = name;
            name = name.Trim().Trim('.');
        }
        while (name != previous);
        return name;
    }

    /// <summary>Caps <paramref name="name"/> at <see cref="MaxLength"/>, cutting the stem so that a short extension survives.</summary>
    /// <remarks>If cutting would leave no stem, the extension is not kept, so a name never shrinks to a bare extension.</remarks>
    private static string Shorten(string name)
    {
        if (name.Length <= MaxLength)
        {
            return name;
        }

        var dot = name.LastIndexOf('.');
        var extension = dot > 0 && name.Length - dot <= MaxKeptExtension ? name[dot..] : "";
        var stem = TrimEnds(Cut(name[..(name.Length - extension.Length)], MaxLength - extension.Length));
        return stem.Length == 0 ? TrimEnds(Cut(name, MaxLength)) : TrimEnds(stem + extension);
    }

    /// <summary>The first <paramref name="length"/> code units of <paramref name="text"/>, one fewer if that would split a surrogate pair.</summary>
    private static string Cut(string text, int length) => char.IsHighSurrogate(text[length - 1]) ? text[..(length - 1)] : text[..length];
}
