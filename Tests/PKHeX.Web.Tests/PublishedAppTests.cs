using PKHeX.Core;
using PKHeX.Web.Components;
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

        var page = session.Page;
        await Expect(page.Locator("#privacy-statement")).ToHaveTextAsync("Your save is processed entirely on this device and is never uploaded.");
        await Expect(page.Locator("#privacy-hosting")).ToContainTextAsync("may keep access logs");

        // About is a disclosure: collapsed at start, and the trimmed publish keeps the build provenance (BuildInfoTests checks the values against git).
        var toggle = page.Locator("#about-toggle");
        await Expect(toggle).ToHaveAttributeAsync("aria-expanded", "false");
        await Expect(page.Locator("#about")).ToBeHiddenAsync();
        await toggle.ClickAsync();
        await Expect(toggle).ToHaveAttributeAsync("aria-expanded", "true");
        await Expect(page.Locator("#about")).ToBeVisibleAsync();
        await Expect(page.Locator("#about-version")).ToHaveTextAsync(BuildInfo.WebVersion);
        await Expect(page.Locator("#about-commit")).ToHaveTextAsync(BuildInfo.SourceCommit);
        await Expect(page.Locator("#support-matrix tbody tr")).ToHaveCountAsync(SupportMatrix.Families.Count);
        await Expect(page.Locator("#about-source")).ToHaveAttributeAsync("href", "https://github.com/kwsch/PKHeX");
        await Expect(page.Locator("#about-source")).ToHaveAttributeAsync("target", "_blank");
        await AssertLicenseLinksAsync(session);
        await toggle.ClickAsync();
        await Expect(page.Locator("#about")).ToBeHiddenAsync();

        // Give late post-boot activity (deferred fetches, service worker registration) time to show up before checking for it.
        await session.Page.WaitForTimeoutAsync(1000);
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred during boot.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task OverviewShowsTheOpenSave(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, SaveFixtures.Synthetic(false, customize: SaveOverviewTests.SetKnownTrainer), "Serena's <save>");

        // The strings are pinned here, as in OverviewTextTests, not built from the mapping under test.
        var expected = new Dictionary<string, string>
        {
            ["#overview-game"] = "X",
            ["#overview-family"] = "Pokémon X and Y",
            ["#overview-trainer"] = "Serena",
            ["#overview-language"] = "FRA (Français)",
            ["#overview-tid"] = "00042",
            ["#overview-sid"] = "54321",
            ["#overview-playtime"] = "123 h 04 min 05 s",
            ["#overview-money"] = "1,234,567 Pokédollars",
            ["#overview-last-saved"] = "2024-05-06 07:08",
            ["#overview-file"] = "Serena's _save_",
            ["#overview-size"] = "415,232 bytes (405.5 KiB)",
            ["#overview-format"] = "Raw Generation 6 save",
            ["#overview-integrity"] = "Checksums valid and unchanged round trip verified when opened. Every download is revalidated.",
        };
        foreach (var (selector, text) in expected)
        {
            await Expect(page.Locator(selector)).ToHaveTextAsync(text);
        }

        // At phone width the overview stacks instead of scrolling sideways.
        await page.SetViewportSizeAsync(375, 800);
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The page scrolls horizontally at 375 px.");
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    /// <summary>
    /// Every published license and notices file is linked from About, and every link resolves under the hosting path to that file,
    /// served with its type, in a new tab so the session in this one survives.
    /// </summary>
    private async Task AssertLicenseLinksAsync(AppSession session)
    {
        var links = await session.Page.EvaluateAsync<LicenseLink[]>("() => [...document.querySelectorAll('#about-licenses a')].map(a => ({ href: a.href, target: a.target, rel: a.rel }))");
        var expected = Directory.EnumerateFiles(Path.Combine(app.Root, "licenses")).Select(f => "licenses/" + Path.GetFileName(f))
            .Append("LICENSE.txt").Append("THIRD-PARTY-NOTICES.md").Order().ToList();
        Assert.True(links.All(l => l.Href.StartsWith(session.AppUrl, StringComparison.Ordinal)), "A license link does not resolve under the hosting path.");
        Assert.Equal(expected, links.Select(l => l.Href[session.AppUrl.Length..]).Order().ToList());
        foreach (var link in links)
        {
            var path = link.Href[session.AppUrl.Length..];
            Assert.True(link.Target == "_blank" && link.Rel.Split(' ').Contains("noopener"), $"{path} does not open in a new tab without an opener.");
            using var response = await app.FetchAsync(session.Prefix, path);
            Assert.True(response.IsSuccessStatusCode, $"{path}: status {(int)response.StatusCode}.");
            Assert.Equal(path.EndsWith(".md", StringComparison.Ordinal) ? "text/markdown" : "text/plain", response.Content.Headers.ContentType?.MediaType);
            Assert.True((await response.Content.ReadAsByteArrayAsync()).AsSpan().SequenceEqual(File.ReadAllBytes(Path.Combine(app.Root, path))), $"{path} is not the published file.");
        }
    }

    /// <summary>A license link as the browser resolved it. Playwright needs settable properties to deserialize it.</summary>
    private sealed class LicenseLink
    {
        public string Href { get; set; } = "";
        public string Target { get; set; } = "";
        public string Rel { get; set; } = "";
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
        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("X");
        await Select(page, 0);
        var originalNickname = await page.Locator("#nickname").InputValueAsync();
        await page.Locator("#nickname").FillAsync("WASM Cancel");
        await page.Locator("#nicknamed").CheckAsync();
        await Expect(page.Locator("#download")).ToBeDisabledAsync();
        await page.Locator("#cancel-draft").ClickAsync();
        Assert.True(await page.Locator("#nickname").InputValueAsync() == originalNickname);
        Assert.True((await Download(page)).AsSpan().SequenceEqual(expected));

        // Rejected files never replace the open session, and each is reported with its own reason.
        (byte[] Bytes, LoadFailure Failure)[] invalidInputs =
        [
            ([], LoadFailure.Empty),
            (new byte[512], LoadFailure.Unrecognized),
            (bytes[..^1], LoadFailure.Unrecognized),
            (Corrupt(bytes), LoadFailure.IntegrityFailed),
            (new byte[SaveLoader.MaxInputBytes + 1], LoadFailure.TooLarge),
            (new SAV5BW().Write().ToArray(), LoadFailure.RecognizedNotEnabled),
            (new byte[SaveUtil.SIZE_G6ORAS], LoadFailure.Unrecognized),
        ];
        foreach (var (invalid, failure) in invalidInputs)
        {
            await Load(page, invalid);
            await Expect(page.Locator("#message")).ToHaveTextAsync(Refusal(invalid, failure));
            await Expect(page.Locator("#overview-game")).ToHaveTextAsync("X");
            Assert.True((await Download(page)).AsSpan().SequenceEqual(expected), "Rejected replacement changed session.");
        }

        // A pending replacement survives a rejected file, and cancelling it keeps the draft.
        await page.Locator("#nickname").FillAsync("WASM Pending");
        await page.Locator("#nicknamed").CheckAsync();
        await Load(page, SaveFixtures.Synthetic(true), "pending-main");
        await Expect(page.Locator("#replace-name")).ToHaveTextAsync("pending-main");
        await Load(page, new byte[512]);
        await Expect(page.Locator("#message")).ToHaveTextAsync(Refusal(new byte[512], LoadFailure.Unrecognized) + " pending-main is still waiting to replace it.");
        await Expect(page.Locator("#replace-name")).ToHaveTextAsync("pending-main");
        await page.Locator("#replace-cancel").ClickAsync();
        Assert.True(await page.Locator("#nickname").InputValueAsync() == "WASM Pending");
        await page.Locator("#cancel-draft").ClickAsync();

        // Known legal and illegal entities get the native verdict and report.
        foreach (var legal in new[] { true, false })
        {
            var sample = SaveFixtures.Synthetic(true, legal);
            await Load(page, sample);
            await Expect(page.Locator("#overview-game")).ToHaveTextAsync("Omega Ruby");
            await Select(page, 0);

            var native = SaveFixtures.Parse(sample);
            await CheckLegality(page, native.GetBoxSlotAtIndex(0), native);
            await Expect(page.Locator("#legality-status")).ToHaveTextAsync(legal ? "Valid" : "Invalid");
            Assert.True((await Download(page)).AsSpan().SequenceEqual(native.Write().Span));
        }

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    /// <summary>The text shown when <paramref name="bytes"/> is refused while a session is open, checking it is refused for <paramref name="failure"/>.</summary>
    private static string Refusal(byte[] bytes, LoadFailure failure)
    {
        var outcome = SaveLoader.Load(bytes);
        Assert.True(outcome.Failure == failure, $"Fixture is refused as {outcome.Failure}, not {failure}.");
        return UserMessages.For(outcome) + " The previous session was retained.";
    }
}
