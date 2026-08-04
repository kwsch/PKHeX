using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Guardrail for issue #132 (UI localization): keeps user-facing English string literals out of the
/// Avalonia shell so every visible string flows through the localization table
/// (<c>LocalizedStrings</c> / the <c>{loc:Loc Key}</c> markup extension) and can be translated.
///
/// Two static, regex-based scans (fast, no rendering, mirroring <see cref="AccessibilityAuditTests"/>):
///   1. <b>.axaml views</b> — literal <c>Text/Content/Header/Watermark/Title/ToolTip.Tip</c> values
///      that are not bindings (<c>{...}</c>) / <c>{loc:...}</c> and contain real words.
///   2. <b>ViewModels</b> — user-facing message literals passed to dialog-service calls
///      (<c>ShowErrorAsync</c>, <c>ShowDialogAsync</c>, …). Localization keys routed through
///      <c>LocalizedStrings</c>/<c>T(...)</c> are single tokens with no spaces and are ignored.
///
/// Newly added views/ViewModels are enforced by default. The existing, not-yet-migrated backlog is
/// enumerated in <c>localization-allowlist.txt</c> (one <c>RelativePath</c> per line for a whole
/// deferred file, or <c>RelativePath|snippet</c> for a single justified technical literal). Migrating
/// a file means removing its allowlist line — the second test fails on stale/malformed entries.
/// </summary>
public class LocalizationAuditTests
{
    private readonly ITestOutputHelper _output;

    public LocalizationAuditTests(ITestOutputHelper output) => _output = output;

    // User-facing text-bearing attributes in Avalonia XAML.
    private static readonly Regex XamlLiteralAttr = new(
        @"(?:Text|Content|Header|Watermark|Title|ToolTip\.Tip)\s*=\s*""([^""]*)""",
        RegexOptions.Compiled);

    // Dialog-service sinks that render a user-facing string in a ViewModel.
    private static readonly Regex DialogCall = new(
        @"\b(?:ShowErrorAsync|ShowInfoAsync|ShowWarningAsync|ShowMessageAsync|ShowConfirmationAsync|ShowDialogAsync)\s*\(",
        RegexOptions.Compiled);

    // A C# string literal (optionally verbatim/interpolated).
    private static readonly Regex CSharpStringLiteral = new(@"[@$]*""([^""\\]*(?:\\.[^""\\]*)*)""", RegexOptions.Compiled);

    private static readonly Regex TwoLetters = new(@"[A-Za-z]{2,}", RegexOptions.Compiled);

    [Fact]
    public void Xaml_Has_No_Unlocalized_UserFacing_Literals()
    {
        var repoRoot = FindRepoRoot();
        var viewsDir = Path.Combine(repoRoot, "PKHeX.Avalonia", "Views");
        var allowlist = LoadAllowlist(repoRoot);
        var violations = new List<string>();

        foreach (var file in Directory.GetFiles(viewsDir, "*.axaml", SearchOption.AllDirectories).OrderBy(f => f))
        {
            var relative = Path.GetRelativePath(repoRoot, file).Replace('\\', '/');
            if (allowlist.Contains(relative))
                continue; // whole file deferred

            var xml = File.ReadAllText(file);
            foreach (Match m in XamlLiteralAttr.Matches(xml))
            {
                var value = m.Groups[1].Value;
                if (!IsUserFacing(value))
                    continue;

                var snippet = Snippet(m.Value);
                if (allowlist.Contains($"{relative}|{snippet}"))
                    continue;

                violations.Add($"{relative}: {snippet}");
            }
        }

        Report(violations, "XAML views");
        Assert.True(violations.Count == 0,
            $"{violations.Count} unlocalized user-facing literal(s) found in .axaml. Use {{loc:Loc Key}} and add the key to " +
            "PKHeX.Presentation/Localization/Strings/en.json (+ the other languages), or allowlist a justified technical " +
            "literal in Tests/PKHeX.Avalonia.Tests/localization-allowlist.txt.\n\n" + string.Join("\n", violations));
    }

    [Fact]
    public void ViewModels_Have_No_Unlocalized_Dialog_Message_Literals()
    {
        var repoRoot = FindRepoRoot();
        var vmDir = Path.Combine(repoRoot, "PKHeX.Presentation", "ViewModels");
        var allowlist = LoadAllowlist(repoRoot);
        var violations = new List<string>();

        foreach (var file in Directory.GetFiles(vmDir, "*.cs", SearchOption.AllDirectories).OrderBy(f => f))
        {
            var relative = Path.GetRelativePath(repoRoot, file).Replace('\\', '/');
            if (allowlist.Contains(relative))
                continue;

            var code = File.ReadAllText(file);
            foreach (Match call in DialogCall.Matches(code))
            {
                // Scan the argument window after the opening paren for a user-facing string literal.
                var start = call.Index + call.Length;
                var window = code.Substring(start, Math.Min(400, code.Length - start));
                foreach (Match lit in CSharpStringLiteral.Matches(window))
                {
                    var value = lit.Groups[1].Value;
                    if (!IsUserFacingMessage(value))
                        continue;

                    var snippet = Snippet(value);
                    if (allowlist.Contains($"{relative}|{snippet}"))
                        continue;

                    violations.Add($"{relative}: \"{snippet}\"");
                }
            }
        }

        Report(violations, "ViewModels");
        Assert.True(violations.Count == 0,
            $"{violations.Count} unlocalized dialog-message literal(s) found in ViewModels. Route the string through " +
            "LocalizedStrings.Instance[\"Key\"] (add the key to en.json + the other languages), or allowlist the file/line " +
            "in Tests/PKHeX.Avalonia.Tests/localization-allowlist.txt.\n\n" + string.Join("\n", violations));
    }

    [Fact]
    public void Allowlist_Has_No_Obviously_Malformed_Entries()
    {
        var repoRoot = FindRepoRoot();
        var path = Path.Combine(repoRoot, "Tests", "PKHeX.Avalonia.Tests", "localization-allowlist.txt");
        if (!File.Exists(path))
            return;

        var lines = File.ReadAllLines(path)
            .Select((line, idx) => (line, idx))
            .Where(t => !string.IsNullOrWhiteSpace(t.line) && !t.line.TrimStart().StartsWith('#'))
            .ToList();

        foreach (var (line, idx) in lines)
        {
            Assert.True(line.Contains(".axaml") || line.Contains(".cs"),
                $"localization-allowlist.txt line {idx + 1} does not reference a .axaml or .cs file: '{line}'");
        }
    }

    // A XAML attribute value is user-facing if it is not a binding/markup and contains real words.
    private static bool IsUserFacing(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Length == 0 || trimmed.StartsWith('{'))
            return false; // binding, {loc:...}, or {}-escaped
        return TwoLetters.IsMatch(trimmed);
    }

    // A ViewModel string literal is a user-facing *message* (not a localization key or identifier) if
    // it contains whitespace and real words. Keys like "Dialog_About"/"Msg_X" never contain spaces,
    // so T("...")/LocalizedStrings["..."] lookups are correctly ignored.
    private static bool IsUserFacingMessage(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Length == 0)
            return false;
        if (!trimmed.Contains(' '))
            return false; // single token: localization key, format token, enum name, etc.
        return TwoLetters.IsMatch(trimmed);
    }

    private static string Snippet(string block)
    {
        var singleLine = Regex.Replace(block, @"\s+", " ").Trim();
        return singleLine.Length > 120 ? singleLine[..120] + "…" : singleLine;
    }

    private void Report(List<string> violations, string label)
    {
        if (violations.Count == 0)
            return;
        _output.WriteLine($"Found {violations.Count} unlocalized literal(s) in {label}:");
        foreach (var v in violations)
            _output.WriteLine(" - " + v);
    }

    private static HashSet<string> LoadAllowlist(string repoRoot)
    {
        var path = Path.Combine(repoRoot, "Tests", "PKHeX.Avalonia.Tests", "localization-allowlist.txt");
        if (!File.Exists(path))
            return [];
        return File.ReadAllLines(path)
            .Select(l => l.Trim())
            .Where(l => l.Length > 0 && !l.StartsWith('#'))
            .ToHashSet();
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
