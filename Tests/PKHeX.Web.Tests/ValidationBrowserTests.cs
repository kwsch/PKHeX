using Microsoft.Playwright;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Validation and announcements in the published app (WEB-A11Y-001/002): a refused field is marked and described by its error without moving
/// focus or speaking through the status message; Apply and Download stay focusable while they cannot act, and activating one focuses a
/// summary whose links focus the controls; legality announces only a final verdict; read-only fields are marked without losing contrast.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class ValidationBrowserTests(PublishedAppFixture app)
{
    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ARefusedFieldLeadsFromTheSummaryBackToItself(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await page.SetViewportSizeAsync(1280, 900);
        await Load(page, SaveFixtures.Synthetic(false));
        await Select(page);
        var message = await page.Locator("#message").TextContentAsync();
        var url = page.Url;
        var level = page.Locator("#level");

        // A refused level: marked, described by its error, focus kept in the field, and the live status message left alone.
        await level.FillAsync("101");
        await Expect(level).ToHaveAttributeAsync("aria-invalid", "true");
        await Expect(level).ToHaveAttributeAsync("aria-describedby", "level-note level-fields-error");
        // The border reinforces the mark; the field's own rule must not override it.
        Assert.True(await level.EvaluateAsync<bool>("""
            e => {
                const probe = document.createElement('span');
                probe.style.color = getComputedStyle(document.documentElement).getPropertyValue('--invalid').trim();
                document.body.append(probe);
                const invalid = getComputedStyle(probe).color;
                probe.remove();
                const style = getComputedStyle(e);
                return style.borderTopWidth === '2px' && style.borderTopColor === invalid;
            }
            """), "The refused field does not draw the invalid border.");
        await Expect(page.Locator("#level-fields-error")).ToHaveTextAsync(UserMessages.For(SessionError.LevelOutOfRange));
        await Expect(level).ToBeFocusedAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync(message!);
        await Expect(page.Locator(".error-summary")).ToHaveCountAsync(0);

        // Apply cannot apply, but it is still reached by keyboard; activating it focuses the summary, whose link focuses the field.
        var apply = page.Locator("#apply");
        await Expect(apply).ToHaveAttributeAsync("aria-disabled", "true");
        Assert.False(await apply.EvaluateAsync<bool>("e => e.disabled"), "Apply is natively disabled, so a keyboard cannot reach it.");
        await apply.FocusAsync();
        await Expect(apply).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Enter");
        var summary = page.Locator("#apply-summary");
        await Expect(summary).ToBeFocusedAsync();
        await Expect(page.Locator("#apply-summary-title")).ToHaveTextAsync(ValidationText.ApplyTitle);
        await Expect(page.Locator("#apply-summary-list")).ToContainTextAsync(UserMessages.For(SessionError.LevelOutOfRange));
        // WebKit, as Safari by default, moves to links only with Option+Tab.
        await page.Keyboard.PressAsync(engine == "webkit" ? "Alt+Tab" : "Tab");
        await Expect(page.Locator("#apply-summary-list a")).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Enter");
        await Expect(level).ToBeFocusedAsync();
        Assert.True(page.Url == url, "The summary link navigated.");

        // Corrected: the mark and the error go, the summary follows, and legality announces only its final verdict.
        await level.FillAsync("50");
        await Expect(level).Not.ToHaveAttributeAsync("aria-invalid", "true");
        await Expect(page.Locator("#level-fields-error")).ToHaveCountAsync(0);
        await Expect(page.Locator("#legality-status")).Not.ToHaveAttributeAsync("role", "status");
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(VerdictPattern);
        var verdict = await page.Locator("#legality-status").TextContentAsync();
        await Expect(page.Locator("#legality-announce")).ToHaveTextAsync($"Legality: {verdict}");
        await Expect(page.Locator("#legality-announce")).ToHaveAttributeAsync("role", "status");
        await Expect(page.Locator("#level-note")).Not.ToHaveAttributeAsync("role", "status");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task DownloadSaysWhyItWaitsAndLinksToApply(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await page.SetViewportSizeAsync(375, 800);
        await Load(page, SaveFixtures.Synthetic(false));
        await Select(page);
        await page.Locator("#nickname").FillAsync("Waiting");
        // Back to the party and boxes: the editor pane, and Apply with it, is hidden on a phone.
        await page.Locator("#editor-return").ClickAsync();
        await Expect(page.Locator("#pane-editor")).ToBeHiddenAsync();

        var download = page.Locator("#download");
        await download.FocusAsync();
        await page.Keyboard.PressAsync("Enter");

        await Expect(page.Locator("#download-summary")).ToBeFocusedAsync();
        await Expect(page.Locator("#download-summary-title")).ToHaveTextAsync(ValidationText.DownloadTitle);
        var link = page.Locator("#download-summary-list a");
        await Expect(link).ToHaveAttributeAsync("href", "#apply");
        // The link shows the editor again, so the control it leads to can take focus.
        await link.ClickAsync();
        await Expect(page.Locator("#pane-editor")).ToBeVisibleAsync();
        await Expect(page.Locator("#apply")).ToBeFocusedAsync();
        await Expect(page.Locator("#message")).Not.ToHaveTextAsync(SessionStatusText.DownloadStarted);

        // Applied: the summary is gone, and Download can act once a flagged change is acknowledged (the synthetic entity is Invalid).
        await Apply(page);
        await Expect(page.Locator("#download-summary")).ToHaveCountAsync(0);
        if (await page.Locator("#export-ack").CountAsync() > 0)
        {
            await Expect(download).ToHaveAttributeAsync("aria-disabled", "true");
            await page.Locator("#export-ack").CheckAsync();
        }
        await Expect(download).ToHaveAttributeAsync("aria-disabled", "false");
        await Expect(page.Locator("#download-summary")).ToHaveCountAsync(0);
        Assert.True(SaveFixtures.Open(await DownloadEdited(page)).Select(SaveFixtures.FirstBoxSlot).Nickname == "Waiting", "The applied change was not downloaded.");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task AFocusedSummaryThatResolvesHandsFocusBackToItsButton(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await page.SetViewportSizeAsync(1280, 900);
        // The ORAS entity stays Valid after a friendship edit, so nothing is left to acknowledge once the result arrives.
        await Load(page, SaveFixtures.Synthetic(true));
        await Select(page);
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(VerdictPattern);

        // Apply pressed at once after typing waits for the result; its summary takes focus.
        await page.Locator("#ot-friendship").FillAsync("100");
        await page.Locator("#apply").FocusAsync();
        await page.Keyboard.PressAsync("Enter");
        await Expect(page.Locator("#apply-summary")).ToBeFocusedAsync();
        await Expect(page.Locator("#apply-summary-list")).ToHaveTextAsync(LegalityText.ApplyWaiting);

        // The result resolves it: the summary goes, and focus returns to Apply, which can now act, rather than to the page body.
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Valid");
        await Expect(page.Locator("#apply-summary")).ToHaveCountAsync(0);
        await Expect(page.Locator("#apply")).ToBeFocusedAsync();
        await Expect(page.Locator("#apply")).ToHaveAttributeAsync("aria-disabled", "false");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ReadOnlyFieldsAreMarkedWithoutLosingContrast(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            p.IsEgg = true;
            p.IsNicknamed = true;
            p.Nickname = SpeciesName.GetEggName(p.Language, 6);
        })));
        await Select(page);
        var editable = await StyleOf(page, "#nickname");
        await Select(page, SlotRef.InBox(0, 1));
        await Expect(page.Locator("#nickname")).Not.ToBeEditableAsync();
        var readOnly = await StyleOf(page, "#nickname");
        var tokens = await page.EvaluateAsync<string[]>("""
            () => ['--bg', '--fg', '--control'].map(t => {
                const probe = document.createElement('span');
                probe.style.color = getComputedStyle(document.documentElement).getPropertyValue(t).trim();
                document.body.append(probe);
                const resolved = getComputedStyle(probe).color;
                probe.remove();
                return resolved;
            })
            """);

        Assert.Equal([tokens[1], tokens[0], "solid"], editable);
        // Its text keeps the full-contrast token; the background and a dashed border mark it, not colour alone.
        Assert.Equal([tokens[1], tokens[2], "dashed"], readOnly);
    }

    /// <summary>The text colour, background colour and border style of the element <paramref name="selector"/> matches.</summary>
    private static Task<string[]> StyleOf(IPage page, string selector) => page.Locator(selector).EvaluateAsync<string[]>(
        "e => { const s = getComputedStyle(e); return [s.color, s.backgroundColor, s.borderTopStyle]; }");
}
