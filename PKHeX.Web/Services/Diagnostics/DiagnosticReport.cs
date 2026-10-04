using System.Globalization;
using System.Text;

namespace PKHeX.Web.Services.Diagnostics;

/// <summary>What a diagnostic report describes besides the recorded failures. Nothing here comes from the save.</summary>
/// <param name="WebVersion">See <see cref="BuildInfo.WebVersion"/>.</param>
/// <param name="SourceCommit">See <see cref="BuildInfo.SourceCommit"/>.</param>
/// <param name="CoreVersion">See <see cref="BuildInfo.CoreVersion"/>.</param>
/// <param name="SpritesIncluded">See <see cref="BuildInfo.SpritesIncluded"/>.</param>
/// <param name="Browser">The browser's user agent string, or null when it could not be read.</param>
/// <param name="OpenFamily">The display name of the family the open save was opened as (<see cref="SupportedFamily.Games"/>), or null when none is open.</param>
/// <param name="Created">When the report was made, in UTC.</param>
public sealed record DiagnosticContext(string WebVersion, string SourceCommit, string CoreVersion, bool SpritesIncluded, string? Browser, string? OpenFamily, DateTimeOffset Created)
{
    /// <summary>The running build's identity, with the given browser, family and time.</summary>
    public static DiagnosticContext ForBuild(string? browser, string? openFamily, DateTimeOffset created) =>
        new(BuildInfo.WebVersion, BuildInfo.SourceCommit, BuildInfo.CoreVersion, BuildInfo.SpritesIncluded, browser, openFamily, created);
}

/// <summary>
/// Builds the plain-text diagnostic report the user previews before copying or downloading it.
/// </summary>
/// <remarks>
/// The report holds the build, the browser, the open family, and the recorded failures as redacted <see cref="DiagnosticCode"/>s. It never holds
/// save bytes, the file name, sizes, Pokémon or trainer names, IDs or exception messages, and nothing adds them: what the preview shows is
/// the whole report.
/// </remarks>
public static class DiagnosticReport
{
    /// <summary>Longest browser string kept, in UTF-16 code units.</summary>
    public const int MaxBrowserLength = 512;

    /// <summary>The report's first line.</summary>
    public const string Title = "PKHeX Web diagnostic report";

    /// <summary>States, inside the report itself, what it leaves out.</summary>
    public const string Scope = "This report holds no save data, file names, Pokémon or trainer names, or IDs. It is everything that will be shared.";

    /// <summary>Shown in place of the failures when none was recorded.</summary>
    public const string NoEntries = "No problems were recorded in this tab since it was loaded.";

    /// <summary>The report text, with Windows-independent <c>\n</c> line ends.</summary>
    public static string Build(DiagnosticContext context, IReadOnlyList<DiagnosticEntry> entries)
    {
        var text = new StringBuilder();
        Line(text, Title);
        Line(text, Scope);
        Line(text);
        Line(text, $"Web version: {context.WebVersion}");
        Line(text, $"Source commit: {context.SourceCommit}");
        Line(text, $"PKHeX.Core version: {context.CoreVersion}");
        Line(text, $"Sprites: {(context.SpritesIncluded ? "included" : "not included")}");
        Line(text, $"Browser: {Browser(context.Browser)}");
        Line(text, $"Open save: {context.OpenFamily ?? "none"}");
        Line(text, $"Created (UTC): {Time(context.Created)}");
        Line(text);
        if (entries.Count == 0)
        {
            Line(text, NoEntries);
            return text.ToString();
        }
        Line(text, string.Create(CultureInfo.InvariantCulture, $"Recorded problems, oldest first ({entries.Count}; at most {DiagnosticLog.Capacity} are kept):"));
        for (int i = 0; i < entries.Count; i++)
        {
            var (time, operation, code) = entries[i];
            Line(text, string.Create(CultureInfo.InvariantCulture, $"{i + 1}. {Time(time)} {operation}: {code.Name}"));
            if (code.SaveType is { } saveType)
            {
                Line(text, $"   Recognised as: {saveType}");
            }
            if (code.ExceptionTypes.Count > 0)
            {
                Line(text, $"   Exception: {string.Join(" <- ", code.ExceptionTypes)}");
            }
            foreach (var frame in code.Frames)
            {
                Line(text, $"   at {frame}");
            }
        }
        return text.ToString();
    }

    /// <summary>The browser string with control characters removed and its length capped, or "unknown".</summary>
    internal static string Browser(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return BuildInfo.Unknown;
        }
        var kept = new string([.. userAgent.Where(c => !char.IsControl(c))]).Trim();
        if (kept.Length == 0)
        {
            return BuildInfo.Unknown;
        }
        return kept.Length > MaxBrowserLength ? kept[..MaxBrowserLength] : kept;
    }

    private static string Time(DateTimeOffset time) => time.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);

    private static void Line(StringBuilder text, string line = "") => text.Append(line).Append('\n');
}
