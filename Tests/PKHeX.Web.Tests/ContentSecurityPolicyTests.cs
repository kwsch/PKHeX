using System.Text.RegularExpressions;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Keeps the test host's CSP header in step with the app's own meta CSP, so E2E runs enforce the policy the app ships with.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed partial class ContentSecurityPolicyTests
{
    /// <summary>Directives a meta CSP cannot carry; browsers honour them only as a header.</summary>
    private static readonly string[] HeaderOnly = ["frame-ancestors"];

    [Fact]
    public void HostHeaderMatchesMetaPolicy()
    {
        var html = File.ReadAllText(Path.Combine(SaveFixtures.RepositoryRoot, "PKHeX.Web", "wwwroot", "index.html"));
        var match = MetaPolicy().Match(html);
        Assert.True(match.Success, "index.html has no Content-Security-Policy meta element.");

        var meta = Directives(match.Groups["policy"].Value);
        var header = Directives(StaticHost.ContentSecurityPolicy);
        var headerShared = header.Where(d => !HeaderOnly.Contains(d.Key)).ToDictionary();

        Assert.Equal(meta, headerShared);
        Assert.DoesNotContain(meta.Keys, HeaderOnly.Contains);
    }

    private static Dictionary<string, string> Directives(string policy) => policy
        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(d => d.Split(' ', 2))
        .ToDictionary(d => d[0], d => d.Length > 1 ? d[1] : "");

    [GeneratedRegex(""""<meta\s+http-equiv="Content-Security-Policy"\s+content="(?<policy>[^"]*)"""", RegexOptions.IgnoreCase)]
    private static partial Regex MetaPolicy();
}
