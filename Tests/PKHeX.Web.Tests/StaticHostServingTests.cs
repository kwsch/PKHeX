using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Responses of the test host over HTTP, in its deployment-caching mode used by the boot baseline and its default mode used by the other browser tests.
/// </summary>
/// <remarks>
/// These open an <see cref="HttpListener"/> on <c>127.0.0.1</c>, which on Windows needs administrator rights or a URL reservation,
/// so they are in the opt-in <see cref="TestCategory.E2E"/> tier with the other tests that start the host, not in Unit.
/// </remarks>
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class StaticHostServingTests : IDisposable
{
    private readonly string root = Directory.CreateTempSubdirectory("pkhex-host-").FullName;

    public StaticHostServingTests()
    {
        Directory.CreateDirectory(Path.Combine(root, "_framework"));
        File.WriteAllText(Path.Combine(root, "index.html"), "<base href=\"/\" />");
        File.WriteAllText(Path.Combine(root, "_framework", "dotnet.js"), "raw");
        File.WriteAllText(Path.Combine(root, "_framework", "dotnet.js.br"), "brotli");
        File.WriteAllText(Path.Combine(root, "_framework", "dotnet.js.gz"), "gzip");
        File.WriteAllText(Path.Combine(root, "_framework", "PKHeX.Core.qlok0qw4y5.wasm"), "wasm");
        File.WriteAllText(Path.Combine(root, StaticHost.NotFoundPage), "not found");
        File.WriteAllText(Path.Combine(root, "_headers"), """
            /*
              X-Test: all
              Cache-Control: no-cache
            /_framework/*.wasm
              ! Cache-Control
              Cache-Control: immutable
            """);
    }

    public void Dispose() => Directory.Delete(root, true);

    [TierFact(TestCategory.E2E)]
    public async Task CachingModeServesPrecompressedRepresentationsAndRevalidates()
    {
        using var host = new StaticHost(root, deploymentCaching: true);
        using var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.None });

        var brotli = await GetAsync(client, host.Url + "_framework/dotnet.js", "br, gzip");
        Assert.Equal("brotli", await brotli.Content.ReadAsStringAsync());
        Assert.Equal(["br"], brotli.Content.Headers.ContentEncoding);
        Assert.Equal("no-cache", brotli.Headers.CacheControl?.ToString());
        Assert.Equal(["all"], brotli.Headers.GetValues("X-Test"));
        Assert.Contains("Accept-Encoding", brotli.Headers.Vary);
        var brotliTag = brotli.Headers.ETag!;

        var gzip = await GetAsync(client, host.Url + "_framework/dotnet.js", "gzip");
        Assert.Equal("gzip", await gzip.Content.ReadAsStringAsync());
        Assert.NotEqual(brotliTag, gzip.Headers.ETag);

        var raw = await GetAsync(client, host.Url + "_framework/dotnet.js", null);
        Assert.Equal("raw", await raw.Content.ReadAsStringAsync());
        Assert.Empty(raw.Content.Headers.ContentEncoding);

        var revalidated = await GetAsync(client, host.Url + "_framework/dotnet.js", "br", brotliTag);
        Assert.Equal(HttpStatusCode.NotModified, revalidated.StatusCode);
        Assert.Equal(brotliTag, revalidated.Headers.ETag);

        var wasm = await GetAsync(client, host.Url + "_framework/PKHeX.Core.qlok0qw4y5.wasm", "br");
        Assert.Equal("immutable", wasm.Headers.CacheControl?.ToString());
        Assert.Empty(wasm.Content.Headers.ContentEncoding);

        // The subpath page is rewritten and served uncompressed, under its own tag.
        var subpath = await GetAsync(client, host.Url + "PKHeX/", "br");
        Assert.Equal("<base href=\"/PKHeX/\" />", await subpath.Content.ReadAsStringAsync());
        var rootPage = await GetAsync(client, host.Url, "br");
        Assert.NotEqual(rootPage.Headers.ETag, subpath.Headers.ETag);

        // A missing file, or the rules themselves, get the 404 page, the rules' headers and Cloudflare's no-store.
        foreach (var path in new[] { "missing.wasm", "_headers", "PKHeX/_headers" })
        {
            var missing = await GetAsync(client, host.Url + path, "br");
            Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
            Assert.Equal("not found", await missing.Content.ReadAsStringAsync());
            Assert.Equal("text/html", missing.Content.Headers.ContentType?.MediaType);
            Assert.Equal(StaticHost.NotFoundCacheControl, missing.Headers.CacheControl?.ToString());
            Assert.Equal(["all"], missing.Headers.GetValues("X-Test"));
        }

        var log = host.TakeLog();
        Assert.Equal([200, 200, 200, 304, 200, 200, 200, 404, 404, 404], log.Select(r => r.Status));
        Assert.Equal(["br", "gzip", null, "br", null, null, null, null, null, null], log.Select(r => r.Encoding));
        Assert.Equal(Encoding.UTF8.GetByteCount("brotli"), log[0].BodyBytes);
        Assert.Equal(0, log[3].BodyBytes);
        Assert.Empty(host.TakeLog());
    }

    [TierFact(TestCategory.E2E)]
    public async Task DefaultModeServesRawFilesWithoutCachingHeaders()
    {
        using var host = new StaticHost(root);
        using var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.None });

        var response = await GetAsync(client, host.Url + "_framework/dotnet.js", "br, gzip", new EntityTagHeaderValue("\"any\""));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("raw", await response.Content.ReadAsStringAsync());
        Assert.Empty(response.Content.Headers.ContentEncoding);
        Assert.Null(response.Headers.ETag);
        Assert.Null(response.Headers.CacheControl);
        Assert.Equal(["all"], response.Headers.GetValues("X-Test"));

        var missing = await GetAsync(client, host.Url + "missing.js", null);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Equal("not found", await missing.Content.ReadAsStringAsync());
        Assert.Null(missing.Headers.CacheControl);
        Assert.Equal([200, 404], host.TakeLog().Select(r => r.Status));
    }

    [TierFact(TestCategory.E2E)]
    public void AHostWithoutHeadersRulesDoesNotStart()
    {
        File.Delete(Path.Combine(root, "_headers"));
        Assert.Throws<InvalidOperationException>(() => new StaticHost(root));
    }

    private static async Task<HttpResponseMessage> GetAsync(HttpClient client, string url, string? acceptEncoding, EntityTagHeaderValue? ifNoneMatch = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (acceptEncoding is not null)
        {
            request.Headers.TryAddWithoutValidation("Accept-Encoding", acceptEncoding);
        }
        if (ifNoneMatch is not null)
        {
            request.Headers.IfNoneMatch.Add(ifNoneMatch);
        }
        return await client.SendAsync(request);
    }
}
