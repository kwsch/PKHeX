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
    [Theory]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task BootsWithDeploymentHeadersAndNoPersistence(string engine, string prefix)
    {
        // BootAsync has already checked the headers and static-only requests.
        await using var session = await app.BootAsync(engine, prefix);
        var baseUri = await session.Page.EvaluateAsync<string>("() => document.baseURI");
        Assert.True(new Uri(baseUri).AbsolutePath == "/" + prefix, $"Unexpected base href '{new Uri(baseUri).AbsolutePath}'.");

        // Give late post-boot activity (deferred fetches, service worker registration) time to show up before checking for it.
        await session.Page.WaitForTimeoutAsync(1000);
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred during boot.");
    }

    [Theory]
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
