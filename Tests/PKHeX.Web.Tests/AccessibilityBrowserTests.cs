using Microsoft.Playwright;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The automated WCAG check of the published app (WEB-A11Y-003): axe finds no violation in any state of the journey, at desktop and phone
/// widths, in light and dark colour schemes.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class AccessibilityBrowserTests(PublishedAppFixture app)
{
    /// <summary>An XY save with an entity Core finds illegal (so legality lists findings) and a party member.</summary>
    private static byte[] Fixture() => SaveFixtures.Synthetic(false, legal: false, customize: SaveFixtures.WithPartyMember());

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task EveryStateOfTheJourneyPassesAxe(string engine, string prefix)
    {
        // Contrast depends on the colour scheme, layout on the width; the dark pass at desktop width covers the dark tokens.
        foreach (var (width, height, scheme) in new[] { (1280, 900, ColorScheme.Light), (375, 800, ColorScheme.Light), (1280, 900, ColorScheme.Dark) })
        {
            await using var session = await app.BootAsync(engine, prefix);
            var page = session.Page;
            await page.SetViewportSizeAsync(width, height);
            await page.EmulateMediaAsync(new() { ColorScheme = scheme });
            var pass = $"{width}px {scheme}";

            await Accessibility.AssertNoViolationsAsync(page, $"start, {pass}");
            await AssertControlsUseTokens(page, $"start, {pass}");

            await page.Locator("#about-toggle").ClickAsync();
            await page.Locator("#about-diag-prepare").ClickAsync();
            await Expect(page.Locator("#about-diag-preview")).ToBeVisibleAsync();
            await Accessibility.AssertNoViolationsAsync(page, $"About with a diagnostic report, {pass}");
            await page.Locator("#about-toggle").ClickAsync();

            await Load(page, Fixture());
            await Accessibility.AssertNoViolationsAsync(page, $"loaded, grid, {pass}");
            await page.Locator("#storage-as-list").CheckAsync();
            await Expect(page.Locator("#box-list")).ToBeVisibleAsync();
            await Accessibility.AssertNoViolationsAsync(page, $"loaded, list, {pass}");
            await page.Locator("#storage-as-list").UncheckAsync();

            await Select(page);
            await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Invalid");
            await Expect(page.Locator("#legality-findings li").First).ToBeVisibleAsync();
            await Accessibility.AssertNoViolationsAsync(page, $"editor with an Invalid result, {pass}");
            await AssertControlsUseTokens(page, $"editor, {pass}");

            await page.Locator("#nickname").FillAsync("Axe");
            await page.Locator("#close-session").ClickAsync();
            await Expect(page.Locator("#exit")).ToBeVisibleAsync();
            await Accessibility.AssertNoViolationsAsync(page, $"exit panel, {pass}");
            await AssertControlsUseTokens(page, $"exit panel, {pass}");
            await page.Locator("#exit-cancel").ClickAsync();

            await session.AssertNoNetworkOrPersistenceAsync();
        }
    }

    /// <summary>
    /// Every visible button, select and text field draws its text and background from the stylesheet's colour tokens, not from the browser's
    /// own control colours. Those differ by engine and platform (WebKit on Linux draws dark buttons as white on light grey, 1.81:1), so only
    /// token colours have a contrast this app has checked (<see cref="ContrastTokensTests"/>).
    /// </summary>
    private static async Task AssertControlsUseTokens(IPage page, string state)
    {
        var foreign = await page.EvaluateAsync<string[]>("""
            () => {
                const rgb = value => {
                    const probe = document.createElement('span');
                    probe.style.color = value;
                    document.body.append(probe);
                    const resolved = getComputedStyle(probe).color;
                    probe.remove();
                    return resolved;
                };
                const root = getComputedStyle(document.documentElement);
                const tokens = new Set(['--bg', '--fg', '--muted', '--control'].map(t => rgb(root.getPropertyValue(t).trim())));
                const found = [];
                const controls = document.querySelectorAll('button, select, input:not([type=checkbox]):not([type=radio]):not([type=file])');
                for (const control of controls) {
                    if (control.getClientRects().length === 0) continue;
                    const style = getComputedStyle(control);
                    for (const [what, value] of [['color', style.color], ['background', style.backgroundColor]]) {
                        if (!tokens.has(value)) found.push(`${control.tagName.toLowerCase()}#${control.id || '?'} ${what} ${value}`);
                    }
                }
                return found;
            }
            """);
        Assert.True(foreign.Length == 0, $"Controls drawn in the browser's own colours ({state}): {string.Join(", ", foreign)}");
    }
}
