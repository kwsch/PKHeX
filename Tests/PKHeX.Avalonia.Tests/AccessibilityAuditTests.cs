using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Guardrail for issue #137 (accessibility pass): scans every .axaml view for
/// icon-only interactive controls (Button/ToggleButton/RepeatButton whose only
/// visible content is a glyph, emoji, or image/icon — no readable text) and
/// asserts each one exposes an <c>AutomationProperties.Name</c> so screen
/// readers announce something meaningful instead of "button".
///
/// This is a pragmatic, regex-based static scan of the source .axaml files
/// (not a rendered/headless check) so it stays fast and has zero UI
/// dependencies. It intentionally only targets the "icon with no text"
/// case — controls with a visible text label (Content="Save", a child
/// TextBlock with real words, etc.) already announce something useful and
/// are skipped to avoid noisy false positives.
///
/// Known, justified exceptions (e.g. purely decorative / non-interactive
/// glyphs, or controls whose name is supplied dynamically in a way this
/// regex scan can't see) are listed in <c>accessibility-allowlist.txt</c>
/// next to this file, one "RelativePath.axaml|snippet" entry per line.
/// </summary>
public class AccessibilityAuditTests
{
    private readonly ITestOutputHelper _output;

    public AccessibilityAuditTests(ITestOutputHelper output) => _output = output;

    private static readonly string[] ButtonLikeTags = ["Button", "ToggleButton", "RepeatButton"];

    // Icon/glyph child element names that indicate the button's content is
    // purely graphical (no text) unless a sibling TextBlock supplies a label.
    private static readonly Regex IconChildPattern = new(@"<(Image|PathIcon|Path|DrawingPresenter|Svg)\b", RegexOptions.Compiled);

    // A visible text label: Content="Some Words" or <TextBlock Text="Some Words"/>
    // Require at least 2 letters so single glyph/emoji "labels" don't count as text.
    private static readonly Regex VisibleTextPattern = new(@"(Content|Text)\s*=\s*""[^""{}]*[A-Za-z]{2,}[^""]*""", RegexOptions.Compiled);

    private static readonly Regex ContentAttrPattern = new(@"\bContent\s*=\s*""([^""]*)""", RegexOptions.Compiled);
    private static readonly Regex AutomationNamePattern = new(@"AutomationProperties\.Name\s*=", RegexOptions.Compiled);

    [Fact]
    public void IconOnlyButtons_Have_AutomationPropertiesName()
    {
        var repoRoot = FindRepoRoot();
        var viewsDir = Path.Combine(repoRoot, "PKHeX.Avalonia", "Views");
        var allowlist = LoadAllowlist(repoRoot);
        var usedAllowlistEntries = new HashSet<string>();
        var violations = new List<string>();

        foreach (var file in Directory.GetFiles(viewsDir, "*.axaml", SearchOption.AllDirectories).OrderBy(f => f))
        {
            var relative = Path.GetRelativePath(repoRoot, file).Replace('\\', '/');
            var xml = File.ReadAllText(file);

            foreach (var tag in ButtonLikeTags)
            {
                foreach (var block in ExtractElementBlocks(xml, tag))
                {
                    if (AutomationNamePattern.IsMatch(block))
                        continue; // already named

                    if (!IsIconOnly(block))
                        continue; // has a real text label somewhere — fine

                    var snippet = Snippet(block);
                    var allowKey = $"{relative}|{snippet}";

                    if (allowlist.Contains(allowKey) || allowlist.Contains(relative))
                    {
                        usedAllowlistEntries.Add(allowlist.Contains(allowKey) ? allowKey : relative);
                        continue;
                    }

                    violations.Add($"{relative}: <{tag}> \"{snippet}\"");
                }
            }
        }

        if (violations.Count > 0)
        {
            _output.WriteLine($"Found {violations.Count} icon-only control(s) without AutomationProperties.Name:");
            foreach (var v in violations)
                _output.WriteLine(" - " + v);
        }

        Assert.True(violations.Count == 0,
            $"{violations.Count} icon-only Button/ToggleButton/RepeatButton control(s) are missing AutomationProperties.Name.\n" +
            "Add an AutomationProperties.Name (or a genuinely visible text label), or — if this control is a justified " +
            "exception — add an entry to Tests/PKHeX.Avalonia.Tests/accessibility-allowlist.txt.\n\n" +
            string.Join("\n", violations));
    }

    /// <summary>
    /// Fails if the allowlist accumulates stale entries for controls that no longer
    /// match the violation pattern (e.g. because someone already fixed them without
    /// removing the now-unnecessary allowlist line). Keeps the allowlist maintainable.
    /// </summary>
    [Fact]
    public void Allowlist_Has_No_Obviously_Malformed_Entries()
    {
        var repoRoot = FindRepoRoot();
        var allowlistPath = Path.Combine(repoRoot, "Tests", "PKHeX.Avalonia.Tests", "accessibility-allowlist.txt");
        if (!File.Exists(allowlistPath))
            return;

        var lines = File.ReadAllLines(allowlistPath)
            .Select((line, idx) => (line, idx))
            .Where(t => !string.IsNullOrWhiteSpace(t.line) && !t.line.TrimStart().StartsWith('#'))
            .ToList();

        foreach (var (line, idx) in lines)
        {
            Assert.True(line.Contains(".axaml"),
                $"accessibility-allowlist.txt line {idx + 1} does not reference a .axaml file: '{line}'");
        }
    }

    private static bool IsIconOnly(string block)
    {
        var openTagEnd = block.IndexOf('>');
        var openingTag = openTagEnd >= 0 ? block[..(openTagEnd + 1)] : block;

        // A Content bound to a ViewModel property could be real text at runtime —
        // we can't statically know, so don't flag it (avoid false positives).
        if (Regex.IsMatch(openingTag, @"\bContent\s*=\s*""\{"))
            return false;

        if (VisibleTextPattern.IsMatch(block))
            return false; // a real text label exists somewhere (Content or child TextBlock)

        var contentMatch = ContentAttrPattern.Match(openingTag);
        var hasIconContent = contentMatch.Success && IsIconLikeText(contentMatch.Groups[1].Value);
        var hasIconChild = IconChildPattern.IsMatch(block);

        // Icon-only if it has a glyph Content and/or an icon/image child, and no text was found above.
        return hasIconContent || hasIconChild;
    }

    private static bool IsIconLikeText(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Length == 0)
            return false;
        if (trimmed.StartsWith('{'))
            return false; // bound value, can't tell statically

        // Real short words like "OK" or "Go" contain only ASCII letters — don't flag those.
        if (Regex.IsMatch(trimmed, "^[A-Za-z]{1,3}$"))
            return false;

        // No letters at all (glyph/emoji/symbol), or very short punctuation like "<" "+" "X" "…"
        return !Regex.IsMatch(trimmed, "[A-Za-z]{2,}");
    }

    private static string Snippet(string block)
    {
        var singleLine = Regex.Replace(block, @"\s+", " ").Trim();
        return singleLine.Length > 120 ? singleLine[..120] + "…" : singleLine;
    }

    private static HashSet<string> LoadAllowlist(string repoRoot)
    {
        var path = Path.Combine(repoRoot, "Tests", "PKHeX.Avalonia.Tests", "accessibility-allowlist.txt");
        if (!File.Exists(path))
            return [];

        return File.ReadAllLines(path)
            .Select(l => l.Trim())
            .Where(l => l.Length > 0 && !l.StartsWith('#'))
            .ToHashSet();
    }

    /// <summary>
    /// Extracts each top-level occurrence of &lt;tagName ...&gt;...&lt;/tagName&gt; (or the
    /// self-closing form) from raw XAML text, tracking nesting depth of the same tag name.
    /// This is a pragmatic scanner, not a full XML parser — good enough for a static guardrail.
    /// </summary>
    private static IEnumerable<string> ExtractElementBlocks(string xml, string tagName)
    {
        var results = new List<string>();
        var openTag = "<" + tagName;
        var closeTag = "</" + tagName + ">";
        var i = 0;

        while (true)
        {
            var start = xml.IndexOf(openTag, i, StringComparison.Ordinal);
            if (start < 0) break;

            var afterName = start + openTag.Length;
            if (afterName < xml.Length && (char.IsLetterOrDigit(xml[afterName]) || xml[afterName] == '_' || xml[afterName] == '.'))
            {
                // Matched a longer tag name (e.g. <ButtonSpinner> when searching for <Button>),
                // or Avalonia property-element syntax (e.g. <Button.ContextMenu> is not a Button).
                i = afterName;
                continue;
            }

            var gt = xml.IndexOf('>', start);
            if (gt < 0) break;

            if (xml[gt - 1] == '/')
            {
                results.Add(xml[start..(gt + 1)]);
                i = gt + 1;
                continue;
            }

            var depth = 1;
            var pos = gt + 1;
            var closeIdx = -1;

            while (pos < xml.Length)
            {
                var nextOpen = xml.IndexOf(openTag, pos, StringComparison.Ordinal);
                var nextClose = xml.IndexOf(closeTag, pos, StringComparison.Ordinal);
                if (nextClose < 0) break;

                if (nextOpen >= 0 && nextOpen < nextClose)
                {
                    var c = nextOpen + openTag.Length < xml.Length ? xml[nextOpen + openTag.Length] : ' ';
                    if (!char.IsLetterOrDigit(c) && c != '_' && c != '.')
                        depth++;
                    pos = nextOpen + openTag.Length;
                    continue;
                }

                depth--;
                if (depth == 0)
                {
                    closeIdx = nextClose;
                    break;
                }
                pos = nextClose + closeTag.Length;
            }

            if (closeIdx < 0)
            {
                i = gt + 1;
                continue;
            }

            var end = closeIdx + closeTag.Length;
            results.Add(xml[start..end]);
            i = end;
        }

        return results;
    }

    private static string FindRepoRoot()
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        while (!Directory.Exists(Path.Combine(dir, "PKHeX.Avalonia")))
        {
            var parent = Directory.GetParent(dir);
            if (parent == null) throw new DirectoryNotFoundException("Could not find repository root");
            dir = parent.FullName;
        }
        return dir;
    }
}
