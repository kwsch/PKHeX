using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The automated accessibility check (WEB-A11Y-003): axe-core, run in the page by the test, never shipped with the app. It checks the
/// WCAG 2.0, 2.1 and 2.2 A and AA rules, and a test fails on any violation, naming the rule, the elements and what axe says is wrong.
/// </summary>
/// <remarks>
/// No rule is disabled. axe cannot judge everything (focus order, meaningful names, announcements); <c>Incomplete</c> results are left to the
/// manual and screen-reader passes, which the plan records as not yet done.
/// </remarks>
internal static class Accessibility
{
    /// <summary>The axe tags checked: every WCAG A and AA rule up to 2.2.</summary>
    public static readonly IReadOnlyList<string> Tags = ["wcag2a", "wcag2aa", "wcag21a", "wcag21aa", "wcag22aa"];

    /// <summary>Runs axe on <paramref name="page"/> as it is now and fails with every violation found.</summary>
    /// <param name="page">The page.</param>
    /// <param name="state">What the page shows, for the failure message.</param>
    public static async Task AssertNoViolationsAsync(IPage page, string state)
    {
        var options = new AxeRunOptions
        {
            RunOnly = new RunOnlyOptions { Type = "tag", Values = [.. Tags] },
            ResultTypes = [ResultType.Violations],
        };
        var result = await page.RunAxe(options);
        var violations = result.Violations ?? [];
        Assert.True(violations.Length == 0, $"axe found {violations.Length} violation(s) in '{state}':\n{Describe(violations)}");
    }

    /// <summary>One line per violation and element: the rule, its impact and help text, the element's selector and the failed checks.</summary>
    private static string Describe(IEnumerable<AxeResultItem> violations) => string.Join('\n',
        violations.SelectMany(v => (v.Nodes ?? []).Select(n => $"- {v.Id} ({v.Impact}): {v.Help} at {n.Target}: {Checks(n)}")));

    /// <summary>What axe says each failed check of <paramref name="node"/> found.</summary>
    private static string Checks(AxeResultNode node) =>
        string.Join("; ", (node.Any ?? []).Concat(node.All ?? []).Concat(node.None ?? []).Select(c => c.Message));
}
