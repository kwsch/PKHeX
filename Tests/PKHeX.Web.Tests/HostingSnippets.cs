using System.Text.RegularExpressions;

namespace PKHeX.Web.Tests;

/// <summary>
/// Reads the header rules out of the self-hosting snippets in <c>PKHeX.Web/hosting/</c>, so tests can check that a server configured with them
/// gives every path what the shipped <c>_headers</c> gives it.
/// </summary>
/// <remarks>
/// Only the shapes the snippets use are read: in <c>nginx.conf</c>, the <c>map $uri</c> blocks for the cache and policy values; in <c>apache.conf</c>,
/// the top-level <c>Header always set</c> lines and the <c>&lt;If "%{REQUEST_URI} =~ m#…#"&gt;</c> blocks that override them. A snippet that
/// stops matching these shapes yields no rules and fails the tests rather than passing them.
/// </remarks>
internal static partial class HostingSnippets
{
    /// <summary>The folder holding the snippets.</summary>
    public static string Folder => Path.Combine(SaveFixtures.RepositoryRoot, "PKHeX.Web", "hosting");

    /// <summary>One server's rules: a default value per header, and pattern overrides applied in order (the last match wins).</summary>
    public sealed record Rules(IReadOnlyDictionary<string, string> Defaults, IReadOnlyList<(string Header, Regex Path, string Value)> Overrides)
    {
        /// <summary>The value the server sends for <paramref name="header"/> on <paramref name="path"/>, or <see langword="null"/> when it sends none.</summary>
        public string? Get(string path, string header)
        {
            var value = Defaults.GetValueOrDefault(header);
            foreach (var (name, pattern, overriding) in Overrides)
            {
                if (string.Equals(name, header, StringComparison.OrdinalIgnoreCase) && pattern.IsMatch(path))
                {
                    value = overriding;
                }
            }
            return value;
        }
    }

    /// <summary>The rules of <c>nginx.conf</c>: its <c>map $uri</c> blocks, whose variables its <c>add_header</c> lines send, and its literal headers.</summary>
    public static Rules Nginx()
    {
        var text = File.ReadAllText(Path.Combine(Folder, "nginx.conf"));
        var defaults = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var overrides = new List<(string, Regex, string)>();
        foreach (Match add in NginxAddHeader().Matches(text))
        {
            var header = add.Groups["name"].Value;
            var value = add.Groups["value"].Value;
            if (!value.StartsWith('$'))
            {
                defaults[header] = value.Trim('"');
                continue;
            }
            var map = NginxMap().Matches(text).Single(m => m.Groups["variable"].Value == value);
            foreach (Match entry in NginxMapEntry().Matches(map.Groups["body"].Value))
            {
                if (entry.Groups["default"].Success)
                {
                    defaults[header] = entry.Groups["value"].Value;
                }
                else
                {
                    overrides.Add((header, new Regex(entry.Groups["pattern"].Value, RegexOptions.CultureInvariant), entry.Groups["value"].Value));
                }
            }
        }
        return new(defaults, overrides);
    }

    /// <summary>The rules of <c>apache.conf</c>: its top-level <c>Header always set</c> lines, then its <c>&lt;If&gt;</c> overrides in order.</summary>
    public static Rules Apache()
    {
        var text = File.ReadAllText(Path.Combine(Folder, "apache.conf"));
        var defaults = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match set in ApacheTopLevelHeader().Matches(text))
        {
            defaults[set.Groups["name"].Value] = set.Groups["value"].Value;
        }
        var overrides = new List<(string, Regex, string)>();
        foreach (Match block in ApacheIf().Matches(text))
        {
            var pattern = new Regex(block.Groups["pattern"].Value, RegexOptions.CultureInvariant);
            foreach (Match set in ApacheNestedHeader().Matches(block.Groups["body"].Value))
            {
                overrides.Add((set.Groups["name"].Value, pattern, set.Groups["value"].Value));
            }
        }
        return new(defaults, overrides);
    }

    [GeneratedRegex(@"^\s*add_header\s+(?<name>[A-Za-z-]+)\s+(?<value>""[^""]*""|\$\w+)\s+always;", RegexOptions.Multiline)]
    private static partial Regex NginxAddHeader();

    [GeneratedRegex(@"^map\s+\$uri\s+(?<variable>\$\w+)\s*\{(?<body>[^}]*)\}", RegexOptions.Multiline)]
    private static partial Regex NginxMap();

    [GeneratedRegex(@"^\s*(?:(?<default>default)|""~(?<pattern>[^""]+)"")\s+""(?<value>[^""]*)"";", RegexOptions.Multiline)]
    private static partial Regex NginxMapEntry();

    [GeneratedRegex(@"^Header always set (?<name>[A-Za-z-]+) ""(?<value>[^""]*)""", RegexOptions.Multiline)]
    private static partial Regex ApacheTopLevelHeader();

    [GeneratedRegex(@"^<If ""%\{REQUEST_URI\} =~ m#(?<pattern>[^#]+)#"">(?<body>.*?)^</If>", RegexOptions.Multiline | RegexOptions.Singleline)]
    private static partial Regex ApacheIf();

    [GeneratedRegex(@"^\s+Header always set (?<name>[A-Za-z-]+) ""(?<value>[^""]*)""", RegexOptions.Multiline)]
    private static partial Regex ApacheNestedHeader();
}
