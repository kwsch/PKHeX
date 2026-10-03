using PKHeX.Web.Components;
using PKHeX.Web.Services;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Export and leaving a session in the published app (WEB-EXP-001–003, WEB-SESSION-003/005/007), in every engine and at both paths.
/// </summary>
/// <remarks>Expected text comes from the strings pinned in <see cref="SessionStatusTextTests"/>.</remarks>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class ExportFlowTests(PublishedAppFixture app)
{
    /// <summary>
    /// UTC+14 all year, so a stamp taken in UTC, or in the machine's own zone, cannot match by chance.
    /// </summary>
    private const string FarTimeZone = "Pacific/Kiritimati";

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task DownloadsAreTrackedNamedInLocalTimeAndReopenUnchanged(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix, timezoneId: FarTimeZone);
        var page = session.Page;

        // Open → no-op download → reopen gives native bytes, and downloading the reopened file gives them again.
        var bytes = SaveFixtures.Synthetic(false);
        var native = SaveFixtures.Parse(bytes).Write().ToArray();
        await Load(page, bytes);
        await Expect(page.Locator("#export-state")).ToHaveTextAsync("No changes to download.");
        await Expect(page.Locator("#name-original")).ToBeCheckedAsync();
        await Expect(page.Locator("#rename-note")).ToHaveCountAsync(0);
        var noOp = await Download(page);
        Assert.True(noOp.AsSpan().SequenceEqual(native), "No-op browser/native output differs.");
        await Expect(page.Locator("#message")).ToHaveTextAsync(SessionStatusText.DownloadStarted);
        await Expect(page.Locator("#export-state")).ToHaveTextAsync("No changes to download.");
        await Load(page, noOp);
        await Expect(page.Locator("#message")).ToHaveTextAsync("Save loaded locally; checksums valid.");
        Assert.True((await Download(page)).AsSpan().SequenceEqual(native), "Reopened no-op output differs.");

        // An applied change is tracked apart from the download of the unchanged save, and the default name becomes the edited one.
        await Select(page);
        await page.Locator("#nickname").FillAsync("Exported");
        await Apply(page);
        await Expect(page.Locator("#export-state")).ToHaveTextAsync("Changed since the last download.");
        await Expect(page.Locator("#name-edited")).ToBeCheckedAsync();
        await Expect(page.Locator("#rename-note")).ToHaveTextAsync(SessionStatusText.RenameToRestore);

        var zone = TimeZoneInfo.FindSystemTimeZoneById(FarTimeZone);
        var before = DateTime.UtcNow;
        var (edited, name) = await DownloadNamed(page);
        var after = DateTime.UtcNow;
        Assert.True(StampedWithin(name, zone, before, after), $"The edited name '{name}' is not stamped with the browser's local time.");
        Assert.True(SaveFixtures.Open(edited).Select(SaveFixtures.FirstBoxSlot).Nickname == "Exported");
        await Expect(page.Locator("#message")).ToHaveTextAsync(SessionStatusText.DownloadStarted);
        await Expect(page.Locator("#export-state")).ToHaveTextAsync("Download started for the current changes — verify your file.");
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");

        // The original name can still be chosen; it is what a console restores from.
        await page.Locator("#name-original").CheckAsync();
        await Expect(page.Locator("#rename-note")).ToHaveCountAsync(0);
        Assert.True((await Download(page)).AsSpan().SequenceEqual(edited), "The same revision downloaded different bytes.");

        // A later change is not covered by the download.
        await page.Locator("#nickname").FillAsync("Later");
        await Apply(page);
        await Expect(page.Locator("#export-state")).ToHaveTextAsync("Changed since the last download.");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");

        // A download does not disarm the leave warning: the session is still only in memory.
        // Wait for the download itself: Firefox treats it as a navigation, and reloading while it is starting fails.
        await DownloadNamed(page);
        await Expect(page.Locator("#export-state")).ToHaveTextAsync("Download started for the current changes — verify your file.");
        await page.ReloadAsync();
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
        Assert.Equal(["beforeunload"], session.Dialogs);
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task LeavingAChangedSessionNeedsAnExportOrAnExplicitDiscard(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;

        // An applied change plus an unapplied draft: closing first asks about the draft, and Cancel keeps everything.
        await Load(page, SaveFixtures.Synthetic(false));
        await Select(page);
        await page.Locator("#nickname").FillAsync("Applied");
        await Apply(page);
        await Expect(page.Locator("#export-state")).ToHaveTextAsync("Changes not downloaded yet.");
        await page.Locator("#nickname").FillAsync("Draft");
        await page.Locator("#close-session").ClickAsync();
        await Expect(page.Locator("#exit-title")).ToHaveTextAsync("Close this save?");
        await Expect(page.Locator("#exit-title")).ToBeFocusedAsync();
        await Expect(page.Locator("#exit-prompt")).ToHaveTextAsync("The editor has changes that are not applied. Apply them to the save, or discard them.");
        await Expect(page.Locator("#exit-continue")).ToHaveCountAsync(0);
        await page.Locator("#exit-cancel").ClickAsync();
        await Expect(page.Locator("#exit")).ToHaveCountAsync(0);
        await Expect(page.Locator("#close-session")).ToBeFocusedAsync();
        Assert.True(await page.Locator("#nickname").InputValueAsync() == "Draft");
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");

        // Opening another file waits; discarding the draft leads to the session step, where Continue is not offered before a download.
        await Load(page, SaveFixtures.Synthetic(true), "other-main");
        await Expect(page.Locator("#message")).ToHaveTextAsync("Resolve the open session before the new file is opened.");
        await Expect(page.Locator("#exit-name")).ToHaveTextAsync("other-main");
        await page.Locator("#exit-discard-draft").ClickAsync();
        await Expect(page.Locator("#exit-prompt")).ToHaveTextAsync("This save has changes that have not been downloaded. Download it first, or discard the session and lose them.");
        await Expect(page.Locator("#exit-title")).ToBeFocusedAsync();
        await Expect(page.Locator("#exit-continue")).ToHaveCountAsync(0);
        await Expect(page.Locator("#exit-discard-session")).ToHaveTextAsync("Discard session and open other-main");

        // Download from the panel; only then is the confirmation offered. The session is still open until it is given.
        var (exported, _) = await DownloadNamed(page, "#exit-export");
        Assert.True(SaveFixtures.Open(exported).Select(SaveFixtures.FirstBoxSlot).Nickname == "Applied", "The draft was discarded, so only the applied change is exported.");
        await Expect(page.Locator("#exit-continue")).ToHaveTextAsync("Continue; I have checked my export");
        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("X");

        // Cancel, then a further change: the earlier download no longer covers the session, so the confirmation is gone.
        await page.Locator("#exit-cancel").ClickAsync();
        await Select(page);
        await page.Locator("#nickname").FillAsync("Later");
        await Apply(page);
        await page.Locator("#close-session").ClickAsync();
        await Expect(page.Locator("#exit-discard-session")).ToHaveTextAsync("Discard session and close");
        await Expect(page.Locator("#exit-continue")).ToHaveCountAsync(0);
        await page.Locator("#exit-discard-session").ClickAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync("Save closed. Choose a save to begin.");
        await Expect(page.Locator("#open-title")).ToBeFocusedAsync();
        await Expect(page.Locator("#overview-game")).ToHaveCountAsync(0);
        await Expect(page.Locator("#open-title")).ToHaveTextAsync("Open a save file");

        // An unchanged session closes at once.
        await Load(page, SaveFixtures.Synthetic(false));
        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("X");
        await page.Locator("#close-session").ClickAsync();
        await Expect(page.Locator("#overview-game")).ToHaveCountAsync(0);
        await Expect(page.Locator("#open-title")).ToBeFocusedAsync();

        // Replace through Apply draft → download → Continue opens the waiting file.
        await Load(page, SaveFixtures.Synthetic(false));
        await Select(page);
        await page.Locator("#nickname").FillAsync("Replaced");
        await Load(page, SaveFixtures.Synthetic(true), "other-main");
        // The Zigzagoon is not its trainer's own in an X/Y save, so it is Invalid: the panel's Apply draft waits for its acknowledgement.
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Invalid");
        await Expect(page.Locator("#exit-apply-draft")).ToBeDisabledAsync();
        await page.Locator("#exit-apply-ack").CheckAsync();
        await page.Locator("#exit-apply-draft").ClickAsync();
        await Expect(page.Locator("#exit-export")).ToHaveTextAsync("Download save");
        var (replaced, _) = await DownloadNamed(page, "#exit-export");
        Assert.True(SaveFixtures.Open(replaced).Select(SaveFixtures.FirstBoxSlot).Nickname == "Replaced");
        await page.Locator("#exit-continue").ClickAsync();
        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("Alpha Sapphire");
        await Expect(page.Locator("#open-title")).ToBeFocusedAsync();
        await Expect(page.Locator("#overview-file")).ToHaveTextAsync("other-main");
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Unmodified session");
        await Expect(page.Locator("#exit")).ToHaveCountAsync(0);

        // Untrusted names of the full 120 characters, without a break, wrap in the name choice and the exit panel at 375 px.
        await page.SetViewportSizeAsync(375, 800);
        await Load(page, SaveFixtures.Synthetic(false), new string('x', 116) + ".sav");
        await Select(page);
        await page.Locator("#nickname").FillAsync("Long");
        await Apply(page);
        await Load(page, SaveFixtures.Synthetic(true), new string('y', 120));
        await Expect(page.Locator("#exit-discard-session")).ToBeVisibleAsync();
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The page scrolls horizontally at 375 px.");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    /// <summary>True when <paramref name="name"/> is <c>main</c>'s edited name for some second between <paramref name="from"/> and <paramref name="to"/>, in <paramref name="zone"/>.</summary>
    private static bool StampedWithin(string name, TimeZoneInfo zone, DateTime from, DateTime to)
    {
        for (var utc = from.AddTicks(-(from.Ticks % TimeSpan.TicksPerSecond)); utc <= to; utc = utc.AddSeconds(1))
        {
            if (name == FileNaming.EditedName("main", TimeZoneInfo.ConvertTimeFromUtc(utc, zone)))
            {
                return true;
            }
        }
        return false;
    }
}
