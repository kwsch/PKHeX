using System.Text.RegularExpressions;

namespace PKHeX.Web.Tests;

/// <summary>
/// The rules of a published <c>_headers</c> file, applied the way Cloudflare's asset server applies them, so the test host serves exactly
/// what the checked-in file gives a deployment.
/// </summary>
/// <remarks>
/// Only the syntax the app's file uses is accepted: a rule is a path pattern on an unindented line (at most one <c>*</c>, which matches any characters),
/// followed by indented <c>Name: value</c> lines or <c>! Name</c> detaches. <c>#</c> starts a comment line. Anything else, such as placeholders,
/// absolute URLs or a path listed twice, throws, so the file cannot rely on a feature this model does not reproduce.
/// Cloudflare (<c>workers-sdk</c>, <c>pages-shared/asset-server/handler.ts</c>, <c>attachHeaders</c>) applies every matching rule in file order:
/// first the rule's detaches remove the header, then each of its headers is set, or appended with a comma when an earlier rule set it.
/// </remarks>
internal sealed class HostHeaders
{
    private readonly List<Rule> rules;

    private HostHeaders(List<Rule> rules) => this.rules = rules;

    /// <summary>One rule: the path pattern as written, its compiled form, the headers it removes and the headers it sets, in order.</summary>
    internal sealed record Rule(string Pattern, Regex Path, IReadOnlyList<string> Detach, IReadOnlyList<KeyValuePair<string, string>> Set);

    /// <summary>The rules in file order.</summary>
    public IReadOnlyList<Rule> Rules => rules;

    /// <summary>Reads and parses the <c>_headers</c> file in <paramref name="root"/>.</summary>
    /// <exception cref="InvalidOperationException">The file is missing or uses syntax this model does not accept.</exception>
    public static HostHeaders Load(string root)
    {
        var file = Path.Combine(root, "_headers");
        if (!File.Exists(file))
        {
            throw new InvalidOperationException($"{root} has no _headers file; publish PKHeX.Web, which ships one in wwwroot.");
        }
        return Parse(File.ReadAllText(file));
    }

    /// <summary>Parses the text of a <c>_headers</c> file.</summary>
    /// <exception cref="InvalidOperationException">The text uses syntax this model does not accept.</exception>
    public static HostHeaders Parse(string text)
    {
        var parsed = new List<Rule>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        string? pattern = null;
        var detach = new List<string>();
        var set = new List<KeyValuePair<string, string>>();

        void Close()
        {
            if (pattern is null)
            {
                return;
            }
            if (detach.Count == 0 && set.Count == 0)
            {
                throw new InvalidOperationException($"_headers rule '{pattern}' has no headers.");
            }
            parsed.Add(new(pattern, Compile(pattern), [.. detach], [.. set]));
            detach.Clear();
            set.Clear();
        }

        var number = 0;
        foreach (var raw in text.ReplaceLineEndings("\n").Split('\n'))
        {
            number++;
            var line = raw.TrimEnd();
            if (line.Trim().Length == 0 || line.TrimStart().StartsWith('#'))
            {
                continue;
            }
            if (line.Length > 2000)
            {
                throw new InvalidOperationException($"_headers line {number} is over Cloudflare's 2,000-character limit.");
            }

            if (!char.IsWhiteSpace(line[0]))
            {
                Close();
                if (!line.StartsWith('/') || line.Count(c => c == '*') > 1 || line.Contains(':') || line.Any(char.IsWhiteSpace))
                {
                    throw new InvalidOperationException($"_headers line {number}: '{line}' is not a supported path pattern.");
                }
                if (!seen.Add(line))
                {
                    throw new InvalidOperationException($"_headers line {number}: '{line}' is listed twice; Cloudflare keeps only one rule per pattern.");
                }
                pattern = line;
                continue;
            }

            if (pattern is null)
            {
                throw new InvalidOperationException($"_headers line {number} is indented but follows no path pattern.");
            }
            var entry = line.Trim();
            if (entry.StartsWith("! ", StringComparison.Ordinal))
            {
                detach.Add(entry[2..].Trim());
                continue;
            }
            var colon = entry.IndexOf(':');
            if (colon <= 0)
            {
                throw new InvalidOperationException($"_headers line {number}: '{entry}' is not 'Name: value' or '! Name'.");
            }
            set.Add(new(entry[..colon].Trim(), entry[(colon + 1)..].Trim()));
        }
        Close();

        if (parsed.Count > 100)
        {
            throw new InvalidOperationException($"_headers has {parsed.Count} rules; Cloudflare accepts at most 100.");
        }
        return new(parsed);
    }

    /// <summary>
    /// The headers a request for <paramref name="path"/> gets, in the order first set. Names compare case-insensitively.
    /// </summary>
    /// <param name="path">The request path from the deployment root, starting with <c>/</c> (for a subpath deployment, with the subpath removed).</param>
    public IReadOnlyList<KeyValuePair<string, string>> Resolve(string path)
    {
        // Only the file's own headers are modelled, so a header is present here only if an earlier rule set it:
        // absent, Cloudflare's set (or its append after a detach) adds it; present, its append joins the values with a comma.
        var headers = new List<KeyValuePair<string, string>>();
        foreach (var rule in rules.Where(r => r.Path.IsMatch(path)))
        {
            foreach (var name in rule.Detach)
            {
                headers.RemoveAll(h => string.Equals(h.Key, name, StringComparison.OrdinalIgnoreCase));
            }
            foreach (var (name, value) in rule.Set)
            {
                var index = headers.FindIndex(h => string.Equals(h.Key, name, StringComparison.OrdinalIgnoreCase));
                if (index < 0)
                {
                    headers.Add(new(name, value));
                }
                else
                {
                    headers[index] = new(headers[index].Key, headers[index].Value + ", " + value);
                }
            }
        }
        return headers;
    }

    /// <summary>The value of one header for <paramref name="path"/>, or <see langword="null"/> when no rule sets it.</summary>
    public string? Get(string path, string name)
        => Resolve(path).Where(h => string.Equals(h.Key, name, StringComparison.OrdinalIgnoreCase)).Select(h => h.Value).FirstOrDefault();

    /// <summary>The anchored expression Cloudflare compiles from a pattern: literal text, with <c>*</c> matching any characters.</summary>
    private static Regex Compile(string pattern)
        => new("^" + string.Join(".*", pattern.Split('*').Select(Regex.Escape)) + "$", RegexOptions.CultureInvariant);
}
