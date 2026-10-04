using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Keeps the shipped <c>_headers</c> policies, the meta CSP in <c>index.html</c> and the tests' <see cref="ExpectedHeaders"/> in step,
/// so a host that honours <c>_headers</c> and one that has only the meta fallback enforce the same policy, and E2E runs enforce it too.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed partial class ContentSecurityPolicyTests
{
    /// <summary>Directives a meta CSP cannot carry; browsers honour them only as a header.</summary>
    private static readonly string[] HeaderOnly = ["frame-ancestors"];

    private static string WebRoot => Path.Combine(SaveFixtures.RepositoryRoot, "PKHeX.Web", "wwwroot");

    [Fact]
    public void HeaderPolicyMatchesMetaPolicy()
    {
        var html = File.ReadAllText(Path.Combine(WebRoot, "index.html"));
        var match = MetaPolicy().Match(html);
        Assert.True(match.Success, "index.html has no Content-Security-Policy meta element.");

        var meta = Directives(match.Groups["policy"].Value);
        var header = Directives(ExpectedHeaders.AppPolicy);
        var headerShared = header.Where(d => !HeaderOnly.Contains(d.Key)).ToDictionary();

        Assert.Equal(meta, headerShared);
        Assert.DoesNotContain(meta.Keys, HeaderOnly.Contains);
        Assert.Equal("'none'", header["frame-ancestors"]);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/index.html")]
    [InlineData("/404.html")]
    [InlineData("/app.css")]
    [InlineData("/_framework/dotnet.js")]
    [InlineData("/missing/page")]
    public void ShippedHeadersGiveTheAppPolicy(string path)
        => HostHeaders.Load(WebRoot).Get(path, "Content-Security-Policy").Should().Be(ExpectedHeaders.AppPolicy);

    [Theory]
    [InlineData("/LICENSE.txt")]
    [InlineData("/THIRD-PARTY-NOTICES.md")]
    [InlineData("/licenses/dotnet-runtime.txt")]
    public void ShippedHeadersGiveLicenseTextOnlyTheTextPolicy(string path)
        => HostHeaders.Load(WebRoot).Get(path, "Content-Security-Policy").Should().Be(ExpectedHeaders.TextPolicy, "one policy, not the app's joined with it");

    [Fact]
    public void TheTextPolicyAllowsNothingButTheViewersOwnStyleAndIcon()
    {
        var policy = Directives(ExpectedHeaders.TextPolicy);
        policy.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            ["default-src"] = "'none'",
            ["img-src"] = "'self'",
            ["style-src"] = "'unsafe-inline'",
            ["frame-ancestors"] = "'none'",
        });
    }

    [Fact]
    public void TheShellHasNoInlineScriptAndThePolicyAllowsNone()
    {
        var html = File.ReadAllText(Path.Combine(WebRoot, "index.html"));
        var policy = Directives(MetaPolicy().Match(html).Groups["policy"].Value);

        ScriptElement().Matches(html).Should().NotBeEmpty().And.OnlyContain(m => m.Groups["attributes"].Value.Contains("src=\""), "every script is a file");
        EventHandlerAttribute().IsMatch(html).Should().BeFalse("no inline event handler attributes");
        html.Should().NotContain("javascript:", "no script URLs");
        policy["script-src"].Should().NotContain("'unsafe-inline'").And.NotContain("'unsafe-eval'");
        policy["style-src"].Should().NotContain("'unsafe-inline'");
        policy.Should().ContainKey("object-src").WhoseValue.Should().Be("'none'");
    }

    private static Dictionary<string, string> Directives(string policy) => policy
        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(d => d.Split(' ', 2))
        .ToDictionary(d => d[0], d => d.Length > 1 ? d[1] : "");

    [GeneratedRegex(""""<meta\s+http-equiv="Content-Security-Policy"\s+content="(?<policy>[^"]*)"""", RegexOptions.IgnoreCase)]
    private static partial Regex MetaPolicy();

    [GeneratedRegex(@"<script(?<attributes>[^>]*)>(?<body>[\s\S]*?)</script>", RegexOptions.IgnoreCase)]
    private static partial Regex ScriptElement();

    [GeneratedRegex(@"<[^>]*\son[a-z]+\s*=", RegexOptions.IgnoreCase)]
    private static partial Regex EventHandlerAttribute();
}
