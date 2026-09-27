using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Playwright;
using PKHeX.Core;
using PKHeX.Web.Services;
using Xunit;
using static Microsoft.Playwright.Assertions;

namespace PKHeX.Web.Tests;

public sealed class BrowserProofTests
{
    public static IEnumerable<object[]> RealCases()
    {
        foreach (var browser in new[] { "chromium", "firefox", "webkit" })
        {
            foreach (var prefix in new[] { "", "PKHeX/" })
            {
                foreach (var family in new[] { "XY", "ORAS" })
                {
                    yield return [browser, prefix, family];
                }
            }
        }
    }

    public static IEnumerable<object[]> BrowserCases()
    {
        foreach (var browser in new[] { "chromium", "firefox", "webkit" })
        {
            foreach (var prefix in new[] { "", "PKHeX/" })
            {
                yield return [browser, prefix];
            }
        }
    }

    [Theory]
    [MemberData(nameof(RealCases))]
    [Trait(TestCategory.Name, TestCategory.RealSave)]
    public async Task RealSavePublishedRoundTrip(string engine, string prefix, string family)
    {
        // Private fixture: validate it natively before any browser run.
        var path = Required(family == "XY" ? "PKHEX_XY_SAVE" : "PKHEX_ORAS_SAVE");
        var bytes = await File.ReadAllBytesAsync(path);
        var originalHash = SHA256.HashData(bytes);
        var native = ProofFixtures.Parse(bytes);
        Assert.True(native is SAV6XY or SAV6AO && native.ChecksumsValid, "Real fixture failed native integrity validation.");
        Assert.True((native is SAV6XY ? "XY" : "ORAS") == family);

        var index = ProofFixtures.WritableSlot(native);
        var nativeSlot = ProofFixtures.Slot(native, index);
        var sourcePk = nativeSlot.Read(native);
        var expectedNoOp = native.Clone().Write().ToArray();

        // Native reference edit: the same nickname change, made directly through Core.
        var changed = native.Clone();
        var editedPk = ProofFixtures.Slot(changed, index).Read(changed);
        var nickname = editedPk.Nickname == "WASM Proof" ? "WASM Test" : "WASM Proof";
        editedPk.Nickname = nickname;
        editedPk.IsNicknamed = true;
        var beforeSet = changed.Data.ToArray();
        Assert.True(ProofFixtures.Slot(changed, index).WriteTo(changed, editedPk, EntityImportSettings.None));

        var start = changed.GetBoxOffset(index / changed.BoxSlotCount) + (index % changed.BoxSlotCount) * changed.SIZE_BOXSLOT;
        var slotSize = changed.SIZE_BOXSLOT;
        Assert.True(changed.PartyCount == native.PartyCount, "Party count changed.");
        Assert.True(beforeSet.AsSpan(0, start).SequenceEqual(changed.Data[..start]), "Native edit changed data before the target slot.");
        Assert.True(beforeSet.AsSpan(start + slotSize).SequenceEqual(changed.Data[(start + slotSize)..]), "Native edit changed data after the target slot (including party/dex/records).");
        var expectedEdited = changed.Clone().Write().ToArray();

        // Boot the published app.
        var published = Required("PKHEX_WEB_PUBLISHED");
        Assert.True(File.Exists(Path.Combine(published, "_framework", "blazor.webassembly.js")), "Point PKHEX_WEB_PUBLISHED to Release publish/wwwroot.");
        using var server = new StaticHost(published);
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await Launch(playwright, engine);
        await using var context = await browser.NewContextAsync(new() { AcceptDownloads = true });

        var requests = new ConcurrentBag<IRequest>();
        context.Request += (_, request) => requests.Add(request);
        var page = await context.NewPageAsync();
        var errors = 0;
        page.PageError += (_, _) => Interlocked.Increment(ref errors);
        page.Dialog += async (_, dialog) => await dialog.AcceptAsync();

        var timer = Stopwatch.StartNew();
        await page.GotoAsync(server.Url + prefix);
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
        var bootMs = timer.ElapsedMilliseconds;
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        AssertOnlyStaticRequests(requests, published, prefix);
        requests.Clear();

        // No-op round trip: open, analyse, download unchanged.
        await Load(page, bytes);
        await Expect(page.Locator("#family")).ToHaveTextAsync(family);
        await Select(page, index);
        await CheckLegality(page, sourcePk, native, nativeSlot.Type);
        var noOp = await Download(page);
        Assert.True(noOp.AsSpan().SequenceEqual(expectedNoOp), "No-op browser/native output differs (bytes withheld).");

        // Edited round trip: reopen the no-op output, edit the nickname, apply, download.
        await Load(page, noOp);
        await Expect(page.Locator("#message")).ToHaveTextAsync("Save loaded locally; checksums valid.");
        await Select(page, index);
        await page.Locator("#nickname").FillAsync(nickname);
        await page.Locator("#nicknamed").CheckAsync();
        await CheckLegality(page, editedPk, changed, nativeSlot.Type);
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        var edited = await Download(page);
        Assert.True(edited.AsSpan().SequenceEqual(expectedEdited), "Edited browser/native output differs (bytes withheld).");

        // The edited export changes only the nickname fields of the target slot.
        var reopened = ProofFixtures.Parse(edited);
        var reopenedPk = ProofFixtures.Slot(reopened, index).Read(reopened);
        Assert.True(reopened.ChecksumsValid && reopenedPk.Nickname == nickname);
        Assert.True(reopened.PartyCount == native.PartyCount, "Export changed party count.");
        Assert.True(PartyBytes(reopened).SequenceEqual(PartyBytes(native)), "Export changed party data.");
        AssertOnlyRangeDiffers(noOp, edited, start, slotSize);
        AssertOnlyNicknameChanged(sourcePk, reopenedPk);

        // Replacing the edited session with its own export shows the edit persisted.
        await Load(page, edited);
        await page.Locator("#replace-confirm").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Unmodified session");
        await Select(page, index);
        Assert.True(await page.Locator("#nickname").InputValueAsync() == nickname, "Downloaded nickname did not survive reopening.");
        await CheckLegality(page, reopenedPk, reopened, nativeSlot.Type);

        // Privacy, runtime errors, and reload clearing the session.
        await CheckPrivacy(context, requests);
        Assert.True(errors == 0, "Browser runtime errors occurred; no private traces retained.");
        await page.ReloadAsync();
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync();
        await Expect(page.Locator("#family")).ToHaveCountAsync(0);
        await Expect(page.Locator("#slot")).ToHaveCountAsync(0);
        Assert.True(SHA256.HashData(await File.ReadAllBytesAsync(path)).AsSpan().SequenceEqual(originalHash), "The original fixture changed.");

        var evidence = Environment.GetEnvironmentVariable("PKHEX_PROOF_EVIDENCE");
        if (evidence is not null)
        {
            Directory.CreateDirectory(evidence);
            var result = new
            {
                family, engine, browserVersion = browser.Version, basePath = "/" + prefix, bootMs, totalMs = timer.ElapsedMilliseconds,
                slotKind = "box", noOpMatchesNative = true, editedMatchesNative = true, reopened = true, originalUnchanged = true, privacyPassed = true,
            };
            var file = $"{family}-{engine}-{(prefix.Length == 0 ? "root" : "subpath")}.json";
            await File.WriteAllTextAsync(Path.Combine(evidence, file), JsonSerializer.Serialize(result));
        }
    }

    [Theory]
    [MemberData(nameof(BrowserCases))]
    [Trait(TestCategory.Name, TestCategory.E2E)]
    public async Task PublishedFailuresDraftsAndKnownLegality(string engine, string prefix)
    {
        // Boot the published app.
        var published = Required("PKHEX_WEB_PUBLISHED");
        using var server = new StaticHost(published);
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await Launch(playwright, engine);
        await using var context = await browser.NewContextAsync(new() { AcceptDownloads = true });

        var requests = new ConcurrentBag<IRequest>();
        context.Request += (_, request) => requests.Add(request);
        var page = await context.NewPageAsync();
        page.Dialog += async (_, dialog) => await dialog.AcceptAsync();

        await page.GotoAsync(server.Url + prefix);
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        AssertOnlyStaticRequests(requests, published, prefix);
        requests.Clear();

        // A cancelled draft blocks download while open and leaves the save unchanged.
        var bytes = ProofFixtures.Synthetic(false);
        var expected = ProofFixtures.Parse(bytes).Write().ToArray();
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
        await Load(page, ProofFixtures.Synthetic(true));
        await page.Locator("#replace-cancel").ClickAsync();
        Assert.True(await page.Locator("#nickname").InputValueAsync() == "WASM Pending");
        await page.Locator("#cancel-draft").ClickAsync();

        // Known legal and illegal entities get the native verdict and report.
        foreach (var legal in new[] { true, false })
        {
            var sample = ProofFixtures.Synthetic(true, legal);
            await Load(page, sample);
            await Expect(page.Locator("#family")).ToHaveTextAsync("ORAS");
            await Select(page, 0);

            var native = ProofFixtures.Parse(sample);
            await CheckLegality(page, native.GetBoxSlotAtIndex(0), native);
            await Expect(page.Locator("#legality-status")).ToHaveTextAsync(legal ? "Valid" : "Invalid");
            Assert.True((await Download(page)).AsSpan().SequenceEqual(native.Write().Span));
        }

        await CheckPrivacy(context, requests);
    }

    private static string Required(string name) => Environment.GetEnvironmentVariable(name)
        ?? throw new InvalidOperationException($"Set {name}; this proof must not silently skip required evidence.");

    private static async Task<IBrowser> Launch(IPlaywright playwright, string name) => await (name switch
    {
        "chromium" => playwright.Chromium,
        "firefox" => playwright.Firefox,
        _ => playwright.Webkit,
    }).LaunchAsync(new() { Headless = true });

    private static async Task Load(IPage page, byte[] bytes)
    {
        await page.Locator("#save-file").SetInputFilesAsync(new FilePayload
        {
            Name = "main", MimeType = "application/octet-stream", Buffer = bytes,
        });
    }

    private static async Task<byte[]> Download(IPage page)
    {
        var download = await page.RunAndWaitForDownloadAsync(() => page.Locator("#download").ClickAsync());
        var path = await download.PathAsync();
        Assert.True(path is not null, "No local download was produced.");
        Assert.True(download.SuggestedFilename == "main", "Unexpected download naming.");
        return await File.ReadAllBytesAsync(path!);
    }

    private static async Task Select(IPage page, int index)
    {
        await page.Locator("#slot").SelectOptionAsync(index.ToString());
        await Expect(page.Locator("#nickname")).ToBeVisibleAsync();
    }

    private static string Verdict(LegalityAnalysis analysis) => analysis.Parsed ? analysis.Valid ? "Valid" : "Invalid" : "Unavailable";

    private static async Task CheckLegality(IPage page, PKM pk, SaveFile save, StorageSlotType type = StorageSlotType.Box)
    {
        var native = new LegalityAnalysis(pk.Clone(), save.Personal, type);
        await page.Locator("#analyze").ClickAsync();
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(Verdict(native));

        // Do not include private reports in assertion output or public test artifacts.
        var actual = await page.Locator("#legality-report").TextContentAsync();
        Assert.True(actual == native.Report(), "Browser/native legality reports differ (contents withheld).");
    }

    /// <summary>
    /// Boot may only fetch published files (plus the root and favicon) from the loopback host, with plain GETs.
    /// </summary>
    private static void AssertOnlyStaticRequests(ConcurrentBag<IRequest> requests, string root, string prefix)
    {
        var allowed = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Select(p => "/" + prefix + Path.GetRelativePath(root, p).Replace('\\', '/'))
            .ToHashSet();
        allowed.Add("/" + prefix);
        allowed.Add("/favicon.ico");

        Assert.False(requests.IsEmpty, "No boot requests were recorded.");
        foreach (var request in requests)
        {
            var uri = new Uri(request.Url);
            var isStatic = uri.Host == "127.0.0.1" && uri.Query.Length == 0 && request.Method == "GET"
                && request.PostData is null && allowed.Contains(uri.AbsolutePath);
            Assert.True(isStatic, "A non-static network request occurred during boot (details withheld).");
        }
    }

    private static async Task CheckPrivacy(IBrowserContext context, ConcurrentBag<IRequest> requests)
    {
        // Requests are cleared once the app has booted, so any request here came from handling the save.
        Assert.True(requests.IsEmpty, "File processing triggered network activity after application boot.");

        // Nothing may be persisted in the browser.
        foreach (var page in context.Pages)
        {
            var empty = await page.EvaluateAsync<bool>("""
                async () => localStorage.length === 0 && sessionStorage.length === 0 &&
                    (await indexedDB.databases()).length === 0 && (await caches.keys()).length === 0 &&
                    (await navigator.serviceWorker.getRegistrations()).length === 0
                """);
            Assert.True(empty, "Browser save persistence was detected.");
        }
        Assert.Empty(await context.CookiesAsync());
    }

    private static byte[] PartyBytes(SaveFile save) => save.PartyData.SelectMany(p => p.Data.ToArray()).ToArray();

    private static void AssertOnlyNicknameChanged(PKM original, PKM result)
    {
        // Restore only nickname storage and its flag, then compare every other entity byte.
        var restored = result.Clone();
        original.NicknameTrash.CopyTo(restored.NicknameTrash);
        restored.IsNicknamed = original.IsNicknamed;
        restored.RefreshChecksum();

        var reference = original.Clone();
        reference.RefreshChecksum();
        Assert.True(restored.Data.SequenceEqual(reference.Data), "Non-nickname entity data changed, including party stats/HP/status/ownership.");
    }

    /// <summary>
    /// Save-level check: every byte differing between the no-op and edited exports lies in the edited
    /// box slot or the Gen 6 block-info footer (block checksums). Party, dex, records and handler data are untouched.
    /// </summary>
    private static void AssertOnlyRangeDiffers(byte[] before, byte[] after, int start, int length)
    {
        Assert.True(before.Length == after.Length, "Export length changed.");
        var footer = before.Length - 0x200;
        for (int i = 0; i < before.Length; i++)
        {
            if (before[i] == after[i] || (i >= start && i < start + length) || i >= footer)
            {
                continue;
            }
            Assert.Fail("Export changed data outside the edited slot and checksum footer (details withheld).");
        }
    }

    private static byte[] Corrupt(byte[] bytes)
    {
        var result = bytes.ToArray();
        result[0] ^= 1;
        return result;
    }
}
