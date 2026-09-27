using System.Text.Json;
using System.Text.RegularExpressions;

namespace PKHeX.Web.Tests;

/// <summary>
/// Reads the package tables of <c>PKHeX.Web/THIRD-PARTY-NOTICES.md</c> and the Web restore graph they must describe.
/// </summary>
internal static partial class NoticesInventory
{
    /// <summary>Table headings, by the section they introduce.</summary>
    private static readonly Dictionary<string, Section> Headings = new()
    {
        ["## Published in the browser bundle"] = Section.Published,
        ["## Restored for the browser but removed by trimming"] = Section.Trimmed,
        ["## Build-time only, not distributed"] = Section.BuildOnly,
    };

    /// <summary>Table a package is listed in.</summary>
    public enum Section
    {
        /// <summary>At least one of the package's files is in the Release publish.</summary>
        Published,

        /// <summary>Resolved for the browser, but trimming removes all of its code.</summary>
        Trimmed,

        /// <summary>Used only while building.</summary>
        BuildOnly,
    }

    /// <summary>Target in <c>project.assets.json</c> that the browser publish resolves against.</summary>
    private const string BrowserTarget = "net10.0/browser-wasm";

    /// <summary>One package row of the notices tables.</summary>
    /// <param name="Id">NuGet package ID.</param>
    /// <param name="Version">Exact package version.</param>
    /// <param name="Section">Table the row is in.</param>
    /// <param name="Notices">Published upstream notices file (relative to <c>wwwroot</c>), or <see langword="null"/> for none.</param>
    public sealed record Row(string Id, string Version, Section Section, string? Notices);

    /// <summary>The notices file in the source tree.</summary>
    public static string SourcePath => Path.Combine(SaveFixtures.RepositoryRoot, "PKHeX.Web", "THIRD-PARTY-NOTICES.md");

    /// <summary>Package rows of both tables, in file order. Rows for non-packages (PKHeX.Core) are not included.</summary>
    public static IReadOnlyList<Row> Rows()
    {
        var rows = new List<Row>();
        Section? section = null;
        foreach (var line in File.ReadLines(SourcePath))
        {
            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                section = Headings.TryGetValue(line, out var found) ? found : null;
                continue;
            }
            var match = PackageRow().Match(line);
            if (section is null || !match.Success)
            {
                continue;
            }
            var notices = match.Groups["notices"].Success ? match.Groups["notices"].Value : null;
            rows.Add(new Row(match.Groups["id"].Value, match.Groups["version"].Value, section.Value, notices));
        }
        return rows;
    }

    /// <summary>Every package in the Web restore graph for the browser target, including the runtime pack, as ID → version.</summary>
    public static Dictionary<string, string> RestoredPackages()
    {
        using var assets = JsonDocument.Parse(File.ReadAllBytes(AssetsPath));
        var root = assets.RootElement;
        var packages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var library in root.GetProperty("targets").GetProperty(BrowserTarget).EnumerateObject())
        {
            if (library.Value.GetProperty("type").GetString() == "package")
            {
                var (id, version) = SplitLibrary(library.Name);
                packages.Add(id, version);
            }
        }
        foreach (var framework in root.GetProperty("project").GetProperty("frameworks").EnumerateObject())
        {
            if (!framework.Value.TryGetProperty("downloadDependencies", out var downloads))
            {
                continue;
            }
            foreach (var download in downloads.EnumerateArray())
            {
                // Download dependencies are exact ranges such as "[10.0.12, 10.0.12]".
                var version = download.GetProperty("version").GetString()!.Trim('[', ']').Split(',')[0].Trim();
                packages.TryAdd(download.GetProperty("name").GetString()!, version);
            }
        }
        return packages;
    }

    /// <summary>The upstream <c>THIRD-PARTY-NOTICES</c> file of a restored package, read from the NuGet package folder.</summary>
    public static byte[] UpstreamNotices(string id, string version)
    {
        var directory = PackageDirectory(id, version);
        // File names differ in case between packages (.txt/.TXT).
        var file = Directory.EnumerateFiles(directory).SingleOrDefault(f => Path.GetFileNameWithoutExtension(f).Equals("THIRD-PARTY-NOTICES", StringComparison.OrdinalIgnoreCase));
        return file is not null ? File.ReadAllBytes(file) : throw new FileNotFoundException($"No upstream THIRD-PARTY-NOTICES file found for {id} {version}.");
    }

    /// <summary>
    /// Maps each file name found in any restored package (including the runtime pack) to the IDs of the packages that contain a file of that name.
    /// </summary>
    public static Dictionary<string, HashSet<string>> PackageFileOwners()
    {
        var owners = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        foreach (var (id, version) in RestoredPackages())
        {
            foreach (var file in Directory.EnumerateFiles(PackageDirectory(id, version), "*", SearchOption.AllDirectories))
            {
                var name = Path.GetFileName(file);
                if (!owners.TryGetValue(name, out var set))
                {
                    owners[name] = set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }
                set.Add(id);
            }
        }
        return owners;
    }

    /// <summary>
    /// Names a published <c>_framework</c> file could have had inside its package: with and without the publish fingerprint,
    /// and for a <c>.wasm</c> (Webcil) assembly also as the original <c>.dll</c>.
    /// </summary>
    public static IEnumerable<string> PackageFileNames(string publishedName)
    {
        var fingerprint = Fingerprinted().Match(publishedName);
        var bases = fingerprint.Success ? new[] { publishedName, fingerprint.Groups["name"].Value + fingerprint.Groups["ext"].Value } : [publishedName];
        foreach (var name in bases)
        {
            yield return name;
            if (name.EndsWith(".wasm", StringComparison.Ordinal))
            {
                yield return name[..^".wasm".Length] + ".dll";
            }
        }
    }

    private static string PackageDirectory(string id, string version)
    {
        using var assets = JsonDocument.Parse(File.ReadAllBytes(AssetsPath));
        foreach (var folder in assets.RootElement.GetProperty("packageFolders").EnumerateObject())
        {
            var directory = Path.Combine(folder.Name, id.ToLowerInvariant(), version);
            if (Directory.Exists(directory))
            {
                return directory;
            }
        }
        throw new DirectoryNotFoundException($"Package {id} {version} is not in any restore package folder.");
    }

    private static string AssetsPath
    {
        get
        {
            var path = Path.Combine(SaveFixtures.RepositoryRoot, "PKHeX.Web", "obj", "project.assets.json");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Restore PKHeX.Web before running the notices tests.", path);
            }
            return path;
        }
    }

    private static (string Id, string Version) SplitLibrary(string name)
    {
        var slash = name.IndexOf('/');
        return (name[..slash], name[(slash + 1)..]);
    }

    [GeneratedRegex(@"^\| (?<id>[A-Za-z0-9][A-Za-z0-9.\-]*)(?: \([^|]*\))? \| (?<version>\d+(?:\.\d+)+(?:-[0-9A-Za-z.\-]+)?) \| [^|]+ \| (?:`(?<notices>licenses/[^`]+)`|—) \|$")]
    private static partial Regex PackageRow();

    /// <summary>A publish fingerprint: <c>name.{10 lowercase alphanumerics}.ext</c>.</summary>
    [GeneratedRegex(@"^(?<name>.+)\.[a-z0-9]{10}(?<ext>\.[A-Za-z0-9]+)$")]
    private static partial Regex Fingerprinted();
}
