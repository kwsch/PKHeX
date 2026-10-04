using FluentAssertions;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The nginx and Apache snippets give every published shape of path the same headers as the shipped <c>_headers</c>, at the root and under a subpath,
/// so the self-hosting guide cannot drift from what Cloudflare gets. <see cref="DeploymentHeadersTests"/> repeats this over every file of a publish.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class HostingSnippetsTests
{
    /// <summary>One path of every shape a publish holds, relative to <c>wwwroot</c>.</summary>
    public static readonly TheoryData<string> Paths = new(
        "index.html",
        "404.html",
        "app.css",
        "boot.js",
        "_framework/PKHeX.Core.qlok0qw4y5.wasm",
        "_framework/dotnet.native.rw4kynp763.wasm",
        "_framework/dotnet.native.b6l13xorvf.js",
        "_framework/dotnet.runtime.v06hirbjsv.js",
        "_framework/icudt_CJK.tjcz0u77k5.dat",
        "_framework/dotnet.js",
        "_framework/blazor.webassembly.js",
        "LICENSE.txt",
        "THIRD-PARTY-NOTICES.md",
        "licenses/dotnet-runtime.txt",
        "sprites/pokemon.f678bd6ad6c2a435.png",
        "sprites/sprites.2c8cbfc95b5dbfe1.css",
        "sprites/manifest.json",
        "sprites/sources.json");

    [Theory]
    [MemberData(nameof(Paths))]
    public void NginxGivesWhatTheHeadersFileGives(string path) => AssertMatches(HostingSnippets.Nginx(), path, []);

    [Theory]
    [MemberData(nameof(Paths))]
    public void ApacheGivesWhatTheHeadersFileGives(string path) => AssertMatches(HostingSnippets.Apache(), path, [".br", ".gz"]);

    [Fact]
    public void TheRootPageGetsTheAppPolicy()
    {
        HostingSnippets.Nginx().Get("/", "Content-Security-Policy").Should().Be(ExpectedHeaders.AppPolicy);
        HostingSnippets.Apache().Get("/PKHeX/", "Content-Security-Policy").Should().Be(ExpectedHeaders.AppPolicy);
    }

    [Fact]
    public void BothSnippetsSendABinaryTypeForTheRuntimeData()
    {
        // The ICU data files (_framework/*.dat) have no entry in either server's standard table: nginx falls back to the http block's default_type
        // (text/plain when none is set) and Apache 2.4 sends no Content-Type at all, so each snippet declares the type itself.
        File.ReadAllText(Path.Combine(HostingSnippets.Folder, "nginx.conf")).Should().MatchRegex(@"(?m)^    default_type application/octet-stream;");
        File.ReadAllText(Path.Combine(HostingSnippets.Folder, "apache.conf")).Should().MatchRegex(@"(?m)^AddType application/octet-stream \.dat\s*$");
    }

    [Fact]
    public void BothSnippetsReadAsRules()
    {
        // A snippet rewritten into a shape HostingSnippets cannot read would otherwise have no overrides to check.
        HostingSnippets.Nginx().Overrides.Should().HaveCount(4);
        HostingSnippets.Apache().Overrides.Should().HaveCount(4);
    }

    /// <summary>
    /// The server's headers for <paramref name="path"/> at the root and under <c>/PKHeX/</c>, and, for Apache, for the precompressed copy the request is
    /// rewritten to, match <see cref="ExpectedHeaders"/>.
    /// </summary>
    internal static void AssertMatches(HostingSnippets.Rules rules, string path, string[] rewrittenSuffixes)
    {
        foreach (var prefix in new[] { "/", "/PKHeX/" })
        {
            foreach (var suffix in rewrittenSuffixes.Prepend(""))
            {
                var requested = prefix + path + suffix;
                rules.Get(requested, "Cache-Control").Should().Be(ExpectedHeaders.CacheControlFor(path), requested);
                rules.Get(requested, "Content-Security-Policy").Should().Be(ExpectedHeaders.PolicyFor(path), requested);
                rules.Get(requested, "X-Content-Type-Options").Should().Be(ExpectedHeaders.ContentTypeOptions, requested);
                rules.Get(requested, "Referrer-Policy").Should().Be(ExpectedHeaders.ReferrerPolicy, requested);
            }
        }
    }
}
