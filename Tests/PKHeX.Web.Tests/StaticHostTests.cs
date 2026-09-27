using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The decisions behind the test host's deployment-caching mode, without opening a listener. <see cref="StaticHostServingTests"/> covers the served responses.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class StaticHostTests
{
    [Theory]
    [InlineData("_framework/PKHeX.Core.qlok0qw4y5.wasm", true)]
    [InlineData("_framework/dotnet.native.b6l13xorvf.js", true)]
    [InlineData("_framework/icudt_CJK.tjcz0u77k5.dat", true)]
    [InlineData("_framework/dotnet.js", false)]
    [InlineData("_framework/blazor.webassembly.js", false)]
    [InlineData("_framework/System.Collections.o7cz0jc1o2.wasm.br", false)]
    [InlineData("PKHeX.Core.qlok0qw4y5.wasm", false)]
    [InlineData("index.html", false)]
    [InlineData("app.css", false)]
    public void FingerprintedFrameworkFilesAreImmutable(string path, bool fingerprinted)
    {
        Assert.Equal(fingerprinted, StaticHost.IsFingerprinted(path));
        Assert.Equal(fingerprinted ? StaticHost.ImmutableCacheControl : StaticHost.RevalidateCacheControl, StaticHost.CacheControlFor(path));
    }

    [Theory]
    [InlineData("gzip, deflate, br, zstd", true, true, "br")]
    [InlineData("gzip, deflate", true, true, "gzip")]
    [InlineData("br;q=0, gzip", true, true, "gzip")]
    [InlineData("BR", true, true, "br")]
    [InlineData("br", false, true, null)]
    [InlineData("gzip, br", false, true, "gzip")]
    [InlineData("identity", true, true, null)]
    [InlineData(null, true, true, null)]
    [InlineData("", true, true, null)]
    public void ChoosesBrotliThenGzipAmongAcceptedCodings(string? accept, bool hasBrotli, bool hasGzip, string? expected)
        => Assert.Equal(expected, StaticHost.ChooseEncoding(accept, hasBrotli, hasGzip));

    [Theory]
    [InlineData("\"abc\"", true)]
    [InlineData("W/\"abc\"", true)]
    [InlineData("\"x\", \"abc\"", true)]
    [InlineData("*", true)]
    [InlineData("\"abcd\"", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void MatchesIfNoneMatch(string? header, bool expected) => Assert.Equal(expected, StaticHost.MatchesIfNoneMatch(header, "\"abc\""));
}
