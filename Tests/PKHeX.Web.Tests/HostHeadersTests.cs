using FluentAssertions;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The <c>_headers</c> model the test host serves from (<see cref="HostHeaders"/>), and the shipped file's cache and header rules for representative paths.
/// <see cref="DeploymentHeadersTests"/> checks every file of an actual publish.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class HostHeadersTests
{
    private static HostHeaders Shipped => HostHeaders.Load(Path.Combine(SaveFixtures.RepositoryRoot, "PKHeX.Web", "wwwroot"));

    [Fact]
    public void RulesApplyInFileOrderWithDetachesBeforeTheirOwnHeaders()
    {
        var rules = HostHeaders.Parse("""
            # comment
            /*
              Cache-Control: no-cache
              X-A: one

            /assets/*.js
              ! Cache-Control
              Cache-Control: immutable
              X-A: two
            """);

        rules.Rules.Select(r => r.Pattern).Should().Equal("/*", "/assets/*.js");
        rules.Resolve("/assets/a.js").Should().Equal(
            new KeyValuePair<string, string>("X-A", "one, two"),
            new KeyValuePair<string, string>("Cache-Control", "immutable"));
        rules.Resolve("/assets/a.css").Should().Equal(
            new KeyValuePair<string, string>("Cache-Control", "no-cache"),
            new KeyValuePair<string, string>("X-A", "one"));
    }

    [Fact]
    public void AHeaderSetTwiceWithoutADetachIsJoined()
        => HostHeaders.Parse("/*\n  Cache-Control: no-cache\n/a\n  Cache-Control: immutable\n").Get("/a", "cache-control").Should().Be("no-cache, immutable");

    [Fact]
    public void ADetachInALaterRuleRemovesTheHeader()
        => HostHeaders.Parse("/*\n  X-A: one\n/a\n  ! X-A\n").Get("/a", "X-A").Should().BeNull();

    [Theory]
    [InlineData("/_framework/*.wasm", "/_framework/a.b.wasm", true)]
    [InlineData("/_framework/*.wasm", "/_framework/sub/a.wasm", true)]
    [InlineData("/_framework/*.wasm", "/_framework/a.wasm.br", false)]
    [InlineData("/_framework/dotnet.*.js", "/_framework/dotnet.js", false)]
    [InlineData("/_framework/dotnet.*.js", "/_framework/dotnet.runtime.v06hirbjsv.js", true)]
    [InlineData("/LICENSE.txt", "/LICENSE.txt", true)]
    [InlineData("/LICENSE.txt", "/LICENSEXtxt", false)]
    [InlineData("/*", "/", true)]
    [InlineData("/LICENSE.txt", "/sub/LICENSE.txt", false)]
    [InlineData("/licenses/*", "/sub/licenses/a.txt", false)]
    public void ASplatMatchesAnyCharactersAndTheRestIsLiteral(string pattern, string path, bool matches)
        => (HostHeaders.Parse($"{pattern}\n  X-A: 1\n").Get(path, "X-A") is not null).Should().Be(matches);

    [Theory]
    [InlineData("  X-A: 1\n")]
    [InlineData("/a\n")]
    [InlineData("/a\n  NoColon\n")]
    [InlineData("/:name\n  X-A: 1\n")]
    [InlineData("https://example.com/*\n  X-A: 1\n")]
    [InlineData("/*/*\n  X-A: 1\n")]
    [InlineData("/a\n  X-A: 1\n/a\n  X-B: 1\n")]
    public void SyntaxTheModelDoesNotReproduceIsRefused(string text)
        => FluentActions.Invoking(() => HostHeaders.Parse(text)).Should().Throw<InvalidOperationException>();

    [Fact]
    public void ALineOverCloudflaresLimitIsRefused()
        => FluentActions.Invoking(() => HostHeaders.Parse("/a\n  X-A: " + new string('x', 2000) + "\n")).Should().Throw<InvalidOperationException>();

    [Fact]
    public void AMissingFileIsRefused()
        => FluentActions.Invoking(() => HostHeaders.Load(Path.GetTempPath() + Guid.NewGuid().ToString("N"))).Should().Throw<InvalidOperationException>();

    [Theory]
    [InlineData("_framework/PKHeX.Core.qlok0qw4y5.wasm")]
    [InlineData("_framework/dotnet.native.rw4kynp763.wasm")]
    [InlineData("_framework/dotnet.native.b6l13xorvf.js")]
    [InlineData("_framework/dotnet.runtime.v06hirbjsv.js")]
    [InlineData("_framework/icudt_CJK.tjcz0u77k5.dat")]
    [InlineData("_framework/dotnet.js")]
    [InlineData("_framework/blazor.webassembly.js")]
    [InlineData("index.html")]
    [InlineData("404.html")]
    [InlineData("app.css")]
    [InlineData("boot.js")]
    [InlineData("LICENSE.txt")]
    [InlineData("sprites/pokemon.f678bd6ad6c2a435.png")]
    [InlineData("sprites/sprites.2c8cbfc95b5dbfe1.css")]
    [InlineData("sprites/manifest.json")]
    [InlineData("sprites/sources.json")]
    public void ShippedRulesCacheOnlyFingerprintedFilesForever(string path)
    {
        Shipped.Get("/" + path, "Cache-Control").Should().Be(ExpectedHeaders.CacheControlFor(path));
        Shipped.Get("/" + path, "X-Content-Type-Options").Should().Be(ExpectedHeaders.ContentTypeOptions);
        Shipped.Get("/" + path, "Referrer-Policy").Should().Be(ExpectedHeaders.ReferrerPolicy);
    }

    [Fact]
    public void TheOracleSeesTheLoadersAsUnfingerprinted()
    {
        ExpectedHeaders.IsFingerprinted("_framework/dotnet.js").Should().BeFalse();
        ExpectedHeaders.IsFingerprinted("_framework/blazor.webassembly.js").Should().BeFalse();
        ExpectedHeaders.IsFingerprinted("_framework/System.Collections.o7cz0jc1o2.wasm.br").Should().BeFalse();
        ExpectedHeaders.IsFingerprinted("PKHeX.Core.qlok0qw4y5.wasm").Should().BeFalse();
        ExpectedHeaders.IsFingerprinted("_framework/PKHeX.Core.qlok0qw4y5.wasm").Should().BeTrue();
    }
}
