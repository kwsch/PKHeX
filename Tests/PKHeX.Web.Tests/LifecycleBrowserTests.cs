using System.Collections.Concurrent;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Acknowledgements and the session lifecycle in the published app (WEB-LEGAL-003, WEB-SESSION-004/005, WEB-SEC-002, WEB-BROWSER-003):
/// an Invalid result is acknowledged before apply and again before the download that contains it; Reset to original reopens the file as
/// opened; Discard session asks once; and a reload, a back navigation or a page restored from the back-forward cache leaves no session,
/// no warning and nothing stored.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class LifecycleBrowserTests(PublishedAppFixture app, ITestOutputHelper output)
{
    public static TheoryData<string> EngineCases() => [.. PublishedAppFixture.Engines];

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task AnInvalidResultIsAcknowledgedBeforeApplyAndAgainBeforeItsDownload(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // The Zigzagoon is not its trainer's own in an X/Y save, so it is Invalid before and after the edit.
        await Load(page, SaveFixtures.Synthetic(false));
        await Select(page);
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Invalid");
        await Expect(page.Locator("#apply-ack")).ToHaveCountAsync(0);
        await Expect(page.Locator("#export-ack")).ToHaveCountAsync(0);
        await Expect(page.Locator("#download")).ToBeEnabledAsync();

        // Apply waits for the result of the draft as it is now, then for its acknowledgement.
        await page.Locator("#nickname").FillAsync("First");
        await Expect(page.Locator("#apply")).ToBeDisabledAsync();
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Invalid");
        await Expect(page.Locator("#apply-ack-waiting")).ToHaveCountAsync(0);
        await Expect(page.Locator("label[for=apply-ack]")).ToHaveTextAsync(LegalityText.ApplyAcknowledgement(LegalityVerdict.Invalid));
        await Expect(page.Locator("#apply-ack")).Not.ToBeCheckedAsync();
        await Expect(page.Locator("#apply")).ToBeDisabledAsync();
        await page.Locator("#apply-ack").CheckAsync();
        await Expect(page.Locator("#apply")).ToBeEnabledAsync();

        // Any further edit withdraws it: the new result has to be acknowledged afresh.
        await page.Locator("#nickname").FillAsync("Flagged");
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Invalid");
        await Expect(page.Locator("#apply-ack")).Not.ToBeCheckedAsync();
        await Expect(page.Locator("#apply")).ToBeDisabledAsync();
        await page.Locator("#apply-ack").CheckAsync();
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync("Changes applied in memory; download to retain changes.");

        // The download lists the flagged change and waits for its own acknowledgement.
        await Expect(page.Locator("#export-ack-list")).ToHaveTextAsync(SessionStatusText.Flagged(SaveFixtures.FirstBoxSlot, LegalityVerdict.Invalid));
        await Expect(page.Locator("#download")).ToBeDisabledAsync();
        await page.Locator("#export-ack").CheckAsync();
        await Expect(page.Locator("#download")).ToBeEnabledAsync();
        var exported = await DownloadEdited(page);
        Assert.True(SaveFixtures.Open(exported).Select(SaveFixtures.FirstBoxSlot).Nickname == "Flagged", "The acknowledged change was not downloaded.");

        // A later apply makes a download the user has not seen listed, so the acknowledgement is withdrawn, here and in the exit panel.
        await page.Locator("#nickname").FillAsync("Later");
        await Apply(page);
        await Expect(page.Locator("#export-ack")).Not.ToBeCheckedAsync();
        await Expect(page.Locator("#download")).ToBeDisabledAsync();
        await page.Locator("#close-session").ClickAsync();
        await Expect(page.Locator("#exit-export-ack-list")).ToHaveTextAsync(SessionStatusText.Flagged(SaveFixtures.FirstBoxSlot, LegalityVerdict.Invalid));
        await Expect(page.Locator("#exit-export")).ToBeDisabledAsync();
        await Expect(page.Locator("#exit-discard-session")).ToBeEnabledAsync();
        await page.Locator("#exit-export-ack").CheckAsync();
        await Expect(page.Locator("#export-ack")).ToBeCheckedAsync();
        var (fromPanel, _) = await DownloadNamed(page, "#exit-export");
        Assert.True(SaveFixtures.Open(fromPanel).Select(SaveFixtures.FirstBoxSlot).Nickname == "Later", "The panel's download did not hold the later change.");
        await page.Locator("#exit-continue").ClickAsync();
        await Expect(page.Locator("#overview-game")).ToHaveCountAsync(0);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ResetToOriginalReopensTheFileAsOpened(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // In Alpha Sapphire the Zigzagoon is its trainer's own and legal, so nothing asks for an acknowledgement.
        var bytes = SaveFixtures.Synthetic(true);
        var nickname = SaveFixtures.Parse(bytes).GetBoxSlotAtIndex(0, 0).Nickname;
        await Load(page, bytes);
        await Expect(page.Locator("#reset-session")).ToBeDisabledAsync();
        await Select(page);
        await page.Locator("#nickname").FillAsync("Applied");
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Valid");
        await Expect(page.Locator("#apply-ack")).ToHaveCountAsync(0);
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#export-ack")).ToHaveCountAsync(0);

        // The reset offers a download first; Cancel keeps everything and returns focus to the button.
        await page.Locator("#reset-session").ClickAsync();
        await Expect(page.Locator("#exit-title")).ToHaveTextAsync("Reset to the file as opened?");
        await Expect(page.Locator("#exit-title")).ToBeFocusedAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync("Resolve the open session before it is reset.");
        await Expect(page.Locator("#exit-discard-session")).ToHaveTextAsync("Discard changes and reset");
        await Expect(page.Locator("#exit-name")).ToHaveCountAsync(0);
        await page.Locator("#exit-cancel").ClickAsync();
        await Expect(page.Locator("#reset-session")).ToBeFocusedAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");

        // With a draft as well: the draft first, then the session.
        await page.Locator("#nickname").FillAsync("Draft");
        await page.Locator("#reset-session").ClickAsync();
        await page.Locator("#exit-discard-draft").ClickAsync();
        await page.Locator("#exit-discard-session").ClickAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync(SessionStatusText.ResetDone);
        await Expect(page.Locator("#open-title")).ToBeFocusedAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Unmodified session");
        await Expect(page.Locator("#export-state")).ToHaveTextAsync("No changes to download.");
        await Expect(page.Locator("#reset-session")).ToBeDisabledAsync();
        await Expect(page.Locator("#draft-title")).ToHaveCountAsync(0);
        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("Alpha Sapphire");

        await Select(page);
        await Expect(page.Locator("#nickname")).ToHaveValueAsync(nickname);
        var downloaded = await Download(page);
        Assert.True(downloaded.AsSpan().SequenceEqual(bytes), "The reset session did not download the file as opened.");

        // Nothing is left to lose, so leaving does not warn.
        await page.ReloadAsync();
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
        Assert.Empty(session.Dialogs);
        await app.AssertStaticBootAsync(session);
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task DiscardSessionAsksOnceAndNamesWhatIsLost(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, SaveFixtures.Synthetic(true));
        await Select(page);
        await page.Locator("#nickname").FillAsync("Applied");
        await Apply(page);
        await page.Locator("#nickname").FillAsync("Draft");

        await page.Locator("#discard-session").ClickAsync();
        await Expect(page.Locator("#exit-title")).ToHaveTextAsync("Discard this session?");
        await Expect(page.Locator("#exit-title")).ToBeFocusedAsync();
        await Expect(page.Locator("#exit-prompt")).ToHaveTextAsync(SessionStatusText.DiscardPrompt(true, ExportStatus.NotExported));
        await Expect(page.Locator("#exit button")).ToHaveCountAsync(2);
        await page.Locator("#exit-cancel").ClickAsync();
        await Expect(page.Locator("#discard-session")).ToBeFocusedAsync();
        await Expect(page.Locator("#nickname")).ToHaveValueAsync("Draft");

        await page.Locator("#discard-session").ClickAsync();
        await page.Locator("#exit-discard-session").ClickAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync(SessionStatusText.Discarded);
        await Expect(page.Locator("#open-title")).ToBeFocusedAsync();
        await Expect(page.Locator("#overview-game")).ToHaveCountAsync(0);

        // An unchanged session has nothing to lose, so it closes at once.
        await Load(page, SaveFixtures.Synthetic(true));
        await page.Locator("#discard-session").ClickAsync();
        await Expect(page.Locator("#overview-game")).ToHaveCountAsync(0);
        await Expect(page.Locator("#exit")).ToHaveCountAsync(0);
        // The button that asked is gone, so focus must not drop to the page body.
        await Expect(page.Locator("#open-title")).ToBeFocusedAsync();

        await page.ReloadAsync();
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
        Assert.Empty(session.Dialogs);
        await app.AssertStaticBootAsync(session);
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task LeavingAndComingBackLeavesNoSession(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // Records how each document was shown, so the log says whether the engine restored the page from its back-forward cache.
        var shows = new ConcurrentQueue<bool>();
        await page.Context.ExposeBindingAsync("__pkhexPageShow", (BindingSource _, bool persisted) => shows.Enqueue(persisted));
        await page.Context.AddInitScriptAsync("addEventListener('pageshow', e => window.__pkhexPageShow(e.persisted));");

        foreach (var edited in new[] { false, true })
        {
            // Both an unchanged session and an edited one, which also arms the leave warning. No engine restores a page from its back-forward
            // cache under Playwright (persisted stays false, even for a plain static page), so this checks that a back navigation starts empty;
            // the restore itself is covered by ARestoredPageIsHiddenThenReloadedWithoutAWarning.
            await Load(page, SaveFixtures.Synthetic(true));
            await Expect(page.Locator("#overview-game")).ToHaveTextAsync("Alpha Sapphire");
            if (edited)
            {
                await Select(page);
                await page.Locator("#nickname").FillAsync("Applied");
                await Apply(page);
            }
            // A blank page, so the navigation away involves nothing but the app.
            await page.GotoAsync("about:blank");
            await page.GoBackAsync();
            await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
            await Expect(page.Locator("#overview-game")).ToHaveCountAsync(0);
            await Expect(page.Locator("#open-title")).ToHaveTextAsync("Open a save file");
            Assert.False(await page.EvaluateAsync<bool>("() => document.documentElement.hasAttribute('data-session-ended')"), "The returned page is still hidden.");
            await app.AssertStaticBootAsync(session);
        }
        output.WriteLine($"{engine} /{prefix}: pageshow persisted = [{string.Join(", ", shows)}]");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    /// <remarks>
    /// Whether an engine keeps the page in its back-forward cache cannot be controlled from a test (headless Chromium has the cache off), so
    /// the restore is simulated with the events the browser would send. Startup is the same at both paths, so the root is enough.
    /// </remarks>
    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(EngineCases))]
    public async Task ARestoredPageIsHiddenThenReloadedWithoutAWarning(string engine)
    {
        await using var session = await app.BootAsync(engine, "");
        var page = session.Page;
        await Load(page, SaveFixtures.Synthetic(true));
        await Select(page);
        await page.Locator("#nickname").FillAsync("Unsaved");
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("Unapplied draft");

        // Going into the cache hides the page, so a restored copy shows nothing of the session and takes no input.
        await page.EvaluateAsync("() => dispatchEvent(new PageTransitionEvent('pagehide', { persisted: true }))");
        Assert.True(await page.EvaluateAsync<string>("() => getComputedStyle(document.body).visibility") == "hidden", "The page entering the cache was not hidden.");
        await Expect(page.Locator("#nickname")).ToBeHiddenAsync();

        // Being restored reloads it, without asking again: the user already chose to leave.
        await page.RunAndWaitForRequestAsync(
            () => page.EvaluateAsync("() => dispatchEvent(new PageTransitionEvent('pageshow', { persisted: true }))"),
            request => request.ResourceType == "document");
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
        await Expect(page.Locator("#overview-game")).ToHaveCountAsync(0);
        Assert.Empty(session.Dialogs);
        Assert.True(await page.EvaluateAsync<string>("() => getComputedStyle(document.body).visibility") == "visible", "The reloaded page is still hidden.");

        await app.AssertStaticBootAsync(session);
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}
