using global::Avalonia.Platform.Storage;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// Builds <see cref="FilePickerFileType"/> lists for file dialogs.
/// </summary>
/// <remarks>
/// Avalonia's macOS native dialog ignores glob patterns: it extracts ".ext" suffixes from
/// each pattern and silently drops the rest ("main", "*"), and only the literal "*.*"
/// pattern marks a filter as "allow any file". Glob-based pickers (Windows/Linux) have the
/// opposite quirk: "*.*" does not match extensionless files, only "*" does. Filters built
/// here therefore carry wildcard intent in both forms.
/// </remarks>
public static class FileDialogFilterFactory
{

    private static readonly string[] AnyFilePatterns = ["*", "*.*"];

    public static List<FilePickerFileType> BuildOpenFileTypes(string[]? filters)
    {
        var fileTypes = new List<FilePickerFileType>
        {
            new("All Files") { Patterns = AnyFilePatterns }
        };

        if (filters is not null)
        {
            fileTypes.Insert(0, new FilePickerFileType("Supported Files") { Patterns = WithBothWildcardForms(filters) });
        }

        return fileTypes;
    }

    private static string[] WithBothWildcardForms(string[] patterns)
    {
        bool hasGlobAny = patterns.Contains("*");
        bool hasMacAny = patterns.Contains("*.*");

        if (hasGlobAny && !hasMacAny)
            return [.. patterns, "*.*"];
        if (hasMacAny && !hasGlobAny)
            return [.. patterns, "*"];
        return patterns;
    }
}
