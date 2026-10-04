using Microsoft.Playwright;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The exit dialog in the published app: a native modal that keeps focus and pointer input inside it while
/// it is open, cancels on Escape at any step with focus back on the button that opened it, and fits a 320px screen.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class ModalExitBrowserTests(PublishedAppFixture app)
{
    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task TheExitDialogHoldsFocusAndEscapeCancelsIt(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await page.SetViewportSizeAsync(1280, 900);
        await Load(page, SaveFixtures.Synthetic(false));
        await Select(page);
        await page.Locator("#nickname").FillAsync("Leaving");
        var dialog = page.Locator("#exit");

        await page.Locator("#close-session").ClickAsync();
        await Expect(dialog).ToBeVisibleAsync();
        Assert.True(await dialog.EvaluateAsync<bool>("e => e.matches(':modal')"), "The exit dialog is not modal.");
        await Expect(page.Locator("#exit-title")).ToBeFocusedAsync();

        // Tab and Shift+Tab stay inside the dialog (or leave the page for the browser's own controls), never reaching the page behind it.
        var reached = 0;
        foreach (var key in new[] { "Tab", "Shift+Tab" })
        {
            for (var i = 0; i < 8; i++)
            {
                await page.Keyboard.PressAsync(key);
                var where = await page.EvaluateAsync<string>("""
                    () => {
                        const active = document.activeElement;
                        if (!active || active === document.body) return 'none';
                        return document.getElementById('exit').contains(active) ? 'inside' : `outside: ${active.id || active.tagName}`;
                    }
                    """);
                Assert.True(where != "outside" && !where.StartsWith("outside", StringComparison.Ordinal), $"{key} moved focus behind the dialog ({where}).");
                reached += where == "inside" ? 1 : 0;
            }
        }
        Assert.True(reached > 0, "Tab never reached the dialog's controls.");

        // A click on the page behind the dialog does nothing.
        await page.Locator("#download").ClickAsync(new LocatorClickOptions { Force = true });
        await Expect(dialog).ToBeVisibleAsync();
        await Expect(page.Locator("#download-summary")).ToHaveCountAsync(0);

        // Escape cancels: the dialog goes, focus returns to Close save, and the draft is kept.
        await page.Keyboard.PressAsync("Escape");
        await Expect(dialog).ToHaveCountAsync(0);
        await Expect(page.Locator("#close-session")).ToBeFocusedAsync();
        await Expect(page.Locator("#nickname")).ToHaveValueAsync("Leaving");
        await Expect(page.Locator("#message")).ToHaveTextAsync("Current session retained.");

        // A close the browser does not let the page refuse (Android back, repeated Escape) cancels as Escape does.
        await page.Locator("#close-session").ClickAsync();
        await Expect(dialog).ToBeVisibleAsync();
        await dialog.EvaluateAsync("d => d.close()");
        await Expect(dialog).ToHaveCountAsync(0);
        await Expect(page.Locator("#message")).ToHaveTextAsync("Current session retained.");
        await Expect(page.Locator("#close-session")).ToBeFocusedAsync();

        // At a later step (the download), Escape still cancels.
        await Apply(page);
        await page.Locator("#nickname").FillAsync("Again");
        await page.Locator("#close-session").ClickAsync();
        await page.Locator("#exit-discard-draft").ClickAsync();
        await Expect(page.Locator("#exit-export")).ToBeVisibleAsync();
        await Expect(page.Locator("#exit-status")).ToHaveTextAsync("Draft discarded.");
        await page.Keyboard.PressAsync("Escape");
        await Expect(dialog).ToHaveCountAsync(0);
        await Expect(page.Locator("#close-session")).ToBeFocusedAsync();
        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("X");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task TheExitDialogFitsANarrowScreen(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await page.SetViewportSizeAsync(320, 480);
        await Load(page, SaveFixtures.Synthetic(false));
        await Select(page);
        await page.Locator("#nickname").FillAsync("Narrow");
        await Apply(page);
        await page.Locator("#editor-return").ClickAsync();
        await page.Locator("#discard-session").ScrollIntoViewIfNeededAsync();
        await page.Locator("#close-session").ClickAsync();
        await Expect(page.Locator("#exit")).ToBeVisibleAsync();

        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"),
            "The page scrolls sideways with the exit dialog open.");
        var buttons = page.Locator("#exit button");
        var count = await buttons.CountAsync();
        Assert.True(count >= 2, "The dialog shows too few choices.");
        for (var i = 0; i < count; i++)
        {
            var button = buttons.Nth(i);
            await button.ScrollIntoViewIfNeededAsync();
            var box = await button.BoundingBoxAsync();
            Assert.True(box is { X: >= 0, Y: >= 0 } && box.X + box.Width <= 320 && box.Y + box.Height <= 480,
                $"Exit choice {i} is not fully on screen when scrolled to ({box?.X}, {box?.Y}, {box?.Width}, {box?.Height}).");
        }
        await page.Locator("#exit-cancel").ClickAsync();
        await Expect(page.Locator("#exit")).ToHaveCountAsync(0);
    }
}
