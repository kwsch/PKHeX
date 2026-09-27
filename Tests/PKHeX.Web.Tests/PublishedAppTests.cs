using PKHeX.Core;
using PKHeX.Web.Services;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Published app in every engine, at the root and under <c>/PKHeX/</c>, with synthetic saves only.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class PublishedAppTests(PublishedAppFixture app)
{
    /// <summary>Most files a Cloudflare Pages deployment may contain.</summary>
    private const int MaxDeployedFiles = 20_000;

    /// <summary>Largest single file a Cloudflare Pages deployment accepts (25 MiB).</summary>
    private const long MaxDeployedFileBytes = 25L * 1024 * 1024;

    /// <summary>Extensions of the precompressed copies the publish places next to an uncompressed asset.</summary>
    private static readonly HashSet<string> PrecompressedExtensions = [".br", ".gz"];

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task BootsWithDeploymentHeadersAndNoPersistence(string engine, string prefix)
    {
        // BootAsync has already checked the headers and static-only requests.
        await using var session = await app.BootAsync(engine, prefix);
        var baseUri = await session.Page.EvaluateAsync<string>("() => document.baseURI");
        Assert.True(new Uri(baseUri).AbsolutePath == "/" + prefix, $"Unexpected base href '{new Uri(baseUri).AbsolutePath}'.");

        // The trimmed publish keeps the build provenance (BuildInfoTests checks the values against git).
        await Expect(session.Page.Locator("#build-label")).ToHaveTextAsync($"PKHeX.Web {BuildInfo.WebVersion} · {BuildInfo.ShortCommit}");
        await Expect(session.Page.Locator("#build-label span")).ToHaveAttributeAsync("title", BuildInfo.SourceCommit);

        // Give late post-boot activity (deferred fetches, service worker registration) time to show up before checking for it.
        await session.Page.WaitForTimeoutAsync(1000);
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred during boot.");
    }

    [TierFact(TestCategory.E2E)]
    public void PublishesLicenseAndNotices()
    {
        var root = SaveFixtures.RepositoryRoot;
        Assert.Equal(File.ReadAllBytes(Path.Combine(root, "LICENSE")), File.ReadAllBytes(Path.Combine(app.Root, "LICENSE.txt")));
        Assert.Equal(File.ReadAllBytes(NoticesInventory.SourcePath), File.ReadAllBytes(Path.Combine(app.Root, "THIRD-PARTY-NOTICES.md")));

        // Exactly the upstream notices named in the table are published, and each matches every package mapped to it.
        var rows = NoticesInventory.Rows();
        var listed = rows.Where(r => r.Section == NoticesInventory.Section.Published).ToList();
        var named = listed.Select(r => r.Notices!).ToHashSet();
        var licenses = Directory.EnumerateFiles(Path.Combine(app.Root, "licenses")).Select(f => "licenses/" + Path.GetFileName(f)).ToHashSet();
        Assert.Equal(named.Order(), licenses.Order());
        foreach (var row in listed)
        {
            var upstream = NoticesInventory.UpstreamNotices(row.Id, row.Version);
            Assert.True(File.ReadAllBytes(Path.Combine(app.Root, row.Notices!)).AsSpan().SequenceEqual(upstream), $"Published {row.Notices} differs from the notices of {row.Id}.");
        }

        // Every published framework file belongs to a package listed as published, other than this repository's own assemblies.
        var owners = NoticesInventory.PackageFileOwners();
        var ownFiles = new[] { "PKHeX.Core.dll", "PKHeX.Web.dll" };
        var publishedIds = listed.Select(r => r.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var withFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in Directory.EnumerateFiles(Path.Combine(app.Root, "_framework")))
        {
            var name = Path.GetFileName(file);
            if (name.EndsWith(".br", StringComparison.Ordinal) || name.EndsWith(".gz", StringComparison.Ordinal))
            {
                continue;
            }
            var candidates = NoticesInventory.PackageFileNames(name).ToList();
            if (candidates.Any(ownFiles.Contains))
            {
                continue;
            }
            var fileOwners = candidates.SelectMany(c => owners.GetValueOrDefault(c) ?? []).ToHashSet(StringComparer.OrdinalIgnoreCase);
            Assert.True(fileOwners.Count > 0, $"Published {name} comes from no restored package.");
            Assert.True(fileOwners.Overlaps(publishedIds), $"Published {name} comes from {string.Join(", ", fileOwners)}, which is not listed as published.");
            withFiles.UnionWith(fileOwners);
        }

        // Each package listed as published has a file in the publish, and none listed as removed by trimming does.
        Assert.Empty(publishedIds.Except(withFiles));
        Assert.Empty(rows.Where(r => r.Section == NoticesInventory.Section.Trimmed && withFiles.Contains(r.Id)).Select(r => r.Id));
    }

    [TierFact(TestCategory.E2E)]
    public void PublishesOnlyStaticDeployableFiles()
    {
        var files = Directory.EnumerateFiles(app.Root, "*", SearchOption.AllDirectories).ToList();
        Assert.True(files.Count <= MaxDeployedFiles, $"The publish has {files.Count} files; the host limit is {MaxDeployedFiles}.");

        foreach (var file in files)
        {
            var name = Path.GetRelativePath(app.Root, file);
            var size = new FileInfo(file).Length;
            Assert.True(size <= MaxDeployedFileBytes, $"Published {name} is {size} bytes; the host limit is {MaxDeployedFileBytes}.");

            // A precompressed copy must sit next to the deployable asset it compresses.
            var asset = file;
            if (PrecompressedExtensions.Contains(Path.GetExtension(file)))
            {
                asset = file[..^Path.GetExtension(file).Length];
                Assert.True(File.Exists(asset), $"Published {name} has no uncompressed asset next to it.");
            }
            Assert.True(IsDeployableAsset(Path.GetRelativePath(app.Root, asset)), $"Published {name} is not an expected static asset in that location.");
        }
    }

    /// <summary>
    /// Whether a published (uncompressed) file is one a Release publish is expected to contain, by folder and type.
    /// Anything else, such as source maps, symbols, sources, project files, save-like or extensionless files like <c>main</c>, must not be deployed.
    /// </summary>
    /// <param name="relativePath">Path relative to the published <c>wwwroot</c>.</param>
    private static bool IsDeployableAsset(string relativePath)
    {
        var directory = Path.GetDirectoryName(relativePath)?.Replace(Path.DirectorySeparatorChar, '/') ?? "";
        var fileName = Path.GetFileName(relativePath);
        var extension = Path.GetExtension(fileName);
        return directory switch
        {
            // Runtime, assemblies and ICU data.
            "_framework" => extension is ".wasm" or ".js" or ".dat",
            // Upstream .NET notices.
            "licenses" => extension is ".txt",
            // App shell, scripts and styles, plus the license and notices.
            "" => extension is ".html" or ".css" or ".js" || fileName is "LICENSE.txt" or "THIRD-PARTY-NOTICES.md",
            _ => false,
        };
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task PublishedFailuresDraftsAndKnownLegality(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;

        // A cancelled draft blocks download while open and leaves the save unchanged.
        var bytes = SaveFixtures.Synthetic(false);
        var expected = SaveFixtures.Parse(bytes).Write().ToArray();
        await Load(page, bytes);
        await Expect(page.Locator("#family")).ToHaveTextAsync("XY");
        await Select(page, 0);
        var originalNickname = await page.Locator("#nickname").InputValueAsync();
        await page.Locator("#nickname").FillAsync("WASM Cancel");
        await page.Locator("#nicknamed").CheckAsync();
        await Expect(page.Locator("#download")).ToBeDisabledAsync();
        await page.Locator("#cancel-draft").ClickAsync();
        Assert.True(await page.Locator("#nickname").InputValueAsync() == originalNickname);
        Assert.True((await Download(page)).AsSpan().SequenceEqual(expected));

        // Rejected files never replace the open session.
        byte[][] invalidInputs =
        [
            [],
            new byte[512],
            bytes[..^1],
            Corrupt(bytes),
            new byte[SaveLoader.MaxInputBytes + 1],
            new SAV5BW().Write().ToArray(),
            new byte[SaveUtil.SIZE_G6ORAS],
        ];
        foreach (var invalid in invalidInputs)
        {
            await Load(page, invalid);
            await Expect(page.Locator("#message")).Not.ToHaveTextAsync("Download started — verify your file. The session remains temporary.");
            await Expect(page.Locator("#family")).ToHaveTextAsync("XY");
            Assert.True((await Download(page)).AsSpan().SequenceEqual(expected), "Rejected replacement changed session.");
        }

        // Cancelling a replacement keeps the pending draft.
        await page.Locator("#nickname").FillAsync("WASM Pending");
        await page.Locator("#nicknamed").CheckAsync();
        await Load(page, SaveFixtures.Synthetic(true));
        await page.Locator("#replace-cancel").ClickAsync();
        Assert.True(await page.Locator("#nickname").InputValueAsync() == "WASM Pending");
        await page.Locator("#cancel-draft").ClickAsync();

        // Known legal and illegal entities get the native verdict and report.
        foreach (var legal in new[] { true, false })
        {
            var sample = SaveFixtures.Synthetic(true, legal);
            await Load(page, sample);
            await Expect(page.Locator("#family")).ToHaveTextAsync("ORAS");
            await Select(page, 0);

            var native = SaveFixtures.Parse(sample);
            await CheckLegality(page, native.GetBoxSlotAtIndex(0), native);
            await Expect(page.Locator("#legality-status")).ToHaveTextAsync(legal ? "Valid" : "Invalid");
            Assert.True((await Download(page)).AsSpan().SequenceEqual(native.Write().Span));
        }

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}
