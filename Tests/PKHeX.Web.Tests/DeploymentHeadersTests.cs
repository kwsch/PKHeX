using System.Net;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The shipped <c>_headers</c> against every file of an actual publish, and the responses a host following it gives:
/// fingerprinted files cached for good and nothing else, the license text under its own policy, and a real 404 for a missing file.
/// </summary>
/// <remarks>
/// The boot checks in <see cref="PublishedAppFixture"/> cover the headers of every file a boot requests; these cover the files a boot does not
/// request and the rules that only a deployment's cache sees.
/// </remarks>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class DeploymentHeadersTests(PublishedAppFixture app)
{
    [TierFact(TestCategory.E2E)]
    public void EveryPublishedFileGetsItsCachePolicyAndHeaders() => AssertEveryFile(app.Root);

    [TierFact(TestCategory.E2E)]
    [Trait(TestCategory.Needs, TestCategory.SpritePublish)]
    public void EverySpritePublishFileGetsItsCachePolicyAndHeaders() => AssertEveryFile(app.SpriteRoot);

    /// <summary>
    /// Each file a client can request (the precompressed copies are served under their asset's path, so they are not requested by name) gets the
    /// expected <c>Cache-Control</c>, policy and headers from the published <c>_headers</c>, and from the nginx and Apache snippets.
    /// </summary>
    private static void AssertEveryFile(string root)
    {
        var rules = HostHeaders.Load(root);
        var nginx = HostingSnippets.Nginx();
        var apache = HostingSnippets.Apache();
        var problems = new List<string>();
        var immutable = 0;
        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            var path = Path.GetRelativePath(root, file).Replace('\\', '/');
            if (path is "_headers" || Path.GetExtension(path) is ".br" or ".gz")
            {
                continue;
            }
            var expected = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Cache-Control"] = ExpectedHeaders.CacheControlFor(path),
                ["Content-Security-Policy"] = ExpectedHeaders.PolicyFor(path),
                ["X-Content-Type-Options"] = ExpectedHeaders.ContentTypeOptions,
                ["Referrer-Policy"] = ExpectedHeaders.ReferrerPolicy,
            };
            var served = rules.Resolve("/" + path).ToDictionary(h => h.Key, h => h.Value, StringComparer.OrdinalIgnoreCase);
            if (!served.OrderBy(h => h.Key, StringComparer.OrdinalIgnoreCase).SequenceEqual(expected.OrderBy(h => h.Key, StringComparer.OrdinalIgnoreCase)))
            {
                problems.Add($"{path}: {string.Join("; ", served.Select(h => $"{h.Key}: {h.Value}"))}");
            }
            immutable += ExpectedHeaders.IsFingerprinted(path) ? 1 : 0;
            HostingSnippetsTests.AssertMatches(nginx, path, []);
            HostingSnippetsTests.AssertMatches(apache, path, [".br", ".gz"]);
        }
        Assert.True(problems.Count == 0, $"Published files with the wrong headers:{Environment.NewLine}{string.Join(Environment.NewLine, problems)}");
        Assert.True(immutable > 50, $"Only {immutable} files are fingerprinted; the oracle no longer recognises the publish's names.");
    }

    [TierTheory(TestCategory.E2E)]
    [InlineData("")]
    [InlineData("PKHeX/")]
    public async Task AMissingFileIsANotFoundPageAndNeverTheApp(string prefix)
    {
        foreach (var path in new[] { "_framework/missing.qlok0qw4y5.wasm", "missing/page", "_headers" })
        {
            using var response = await app.FetchAsync(prefix, path);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal(await File.ReadAllTextAsync(Path.Combine(app.Root, StaticHost.NotFoundPage)), body);
            Assert.DoesNotContain("_framework", body);
            Assert.Equal(ExpectedHeaders.AppPolicy, string.Join(", ", response.Headers.GetValues("Content-Security-Policy")));
        }
    }

    /// <summary>
    /// The license opens in the browser's own viewer with no CSP violation: under the app's policy WebKit's viewer broke <c>style-src-attr</c>.
    /// The violation recorder is a context init script, so it runs in the viewer's document too.
    /// </summary>
    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task LicenseTextOpensWithoutAPolicyViolation(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var licenses = Directory.EnumerateFiles(Path.Combine(app.Root, "licenses")).Select(f => "licenses/" + Path.GetFileName(f)).Order().First();
        foreach (var path in new[] { "LICENSE.txt", licenses })
        {
            var response = await session.Page.GotoAsync(session.AppUrl + path);
            Assert.NotNull(response);
            Assert.Equal(ExpectedHeaders.TextPolicy, await response.HeaderValueAsync("content-security-policy"));
            var text = await session.Page.EvaluateAsync<string>("() => document.body.innerText");
            Assert.Contains((await File.ReadAllTextAsync(Path.Combine(app.Root, path))).Trim().Split('\n')[0].Trim(), text);
        }
        await session.AssertNoCspViolationsAsync();
    }
}
