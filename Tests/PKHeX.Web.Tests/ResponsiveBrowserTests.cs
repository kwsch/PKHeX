using PKHeX.Core;
using Microsoft.Playwright;
using PKHeX.Web.Components;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The responsive shell in the published app (WEB-APP-003, WEB-A11Y-003): side-by-side panes on a desktop, a foldable storage pane on a
/// tablet and one pane at a time on a phone, with no loss of the selection or draft on resize; reflow at 400% zoom, 44px targets and
/// reduced motion.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class ResponsiveBrowserTests(PublishedAppFixture app)
{
    /// <summary>An XY save with a party member, so the party grid and its stats are shown too.</summary>
    private static byte[] Fixture() => SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember());

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task PanesFollowTheWidthWithoutLosingTheDraft(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        var storage = page.Locator("#pane-storage");
        var editor = page.Locator("#pane-editor");
        var back = page.Locator("#editor-return");
        var fold = page.Locator("#storage-toggle");

        // Desktop: both panes side by side; focus stays on the slot that was opened.
        await page.SetViewportSizeAsync(1280, 900);
        await Load(page, Fixture());
        await Select(page);
        await Expect(page.Locator("#box-grid-0")).ToBeFocusedAsync();
        await Expect(storage).ToBeVisibleAsync();
        await Expect(editor).ToBeVisibleAsync();
        await Expect(back).ToBeHiddenAsync();
        await Expect(fold).ToBeHiddenAsync();
        await Expect(page.Locator("#editor-resume")).ToBeHiddenAsync();
        var (left, right) = (await Box(storage), await Box(editor));
        Assert.True(right.X >= left.X + left.Width, $"The editor ({right.X}) is not beside the storage pane (ends at {left.X + left.Width}).");
        await page.Locator("#nickname").FillAsync("Resized");
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("Unapplied draft");
        // A resize that hides the element with focus (here the box navigation, in the storage pane a phone hides while the editor is open)
        // moves focus to the heading of the pane shown, not to the page body.
        await page.Locator("#box-next").FocusAsync();
        await page.SetViewportSizeAsync(375, 800);
        await Expect(page.Locator("#draft-title")).ToBeFocusedAsync();
        await page.SetViewportSizeAsync(1280, 900);
        await Expect(page.Locator("#draft-title")).ToBeFocusedAsync();

        // Phone: one pane at a time. The draft survives the resize, and the return action brings back the selected slot with focus on it.
        await page.SetViewportSizeAsync(375, 800);
        await Expect(storage).ToBeHiddenAsync();
        await Expect(editor).ToBeVisibleAsync();
        await Expect(page.Locator("#nickname")).ToHaveValueAsync("Resized");
        await Expect(back).ToHaveTextAsync(WorkspaceLayout.ReturnToStorage);
        await back.ClickAsync();
        await Expect(editor).ToBeHiddenAsync();
        await Expect(storage).ToBeVisibleAsync();
        await Expect(page.Locator("#box-grid-0")).ToBeFocusedAsync();
        await Expect(page.Locator("#box-grid [role=gridcell][aria-selected=true] button")).ToHaveIdAsync("box-grid-0");
        // Opening the selected slot again shows its editor, focused, with the draft as it was.
        await page.Keyboard.PressAsync("Enter");
        await Expect(editor).ToBeVisibleAsync();
        await Expect(storage).ToBeHiddenAsync();
        await Expect(page.Locator("#draft-title")).ToBeFocusedAsync();
        await Expect(page.Locator("#nickname")).ToHaveValueAsync("Resized");

        // Browsed to another box, every slot refuses while the draft has changes, and the draft's own slot is not shown: the resume action
        // leads back to the editor.
        await back.ClickAsync();
        await Expect(page.Locator("#editor-resume")).ToHaveTextAsync(WorkspaceLayout.ResumeEditor);
        await page.Locator("#box-next").ClickAsync();
        await page.Locator("#party-grid-0").ClickAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync("Apply or cancel the draft before opening another slot.");
        await Expect(storage).ToBeVisibleAsync();
        await page.Locator("#editor-resume").ClickAsync();
        await Expect(editor).ToBeVisibleAsync();
        await Expect(page.Locator("#draft-title")).ToBeFocusedAsync();
        await Expect(page.Locator("#editor-resume")).ToBeHiddenAsync();
        // With another box shown, the return action lands on the storage heading, since the selected slot is not on screen.
        await back.ClickAsync();
        await Expect(page.Locator("#storage-title")).ToBeFocusedAsync();
        await page.Locator("#box-prev").ClickAsync();

        // Tablet: stacked, and the storage pane folds away while the editor is open; a wide screen ignores the fold.
        await page.SetViewportSizeAsync(800, 900);
        await Expect(storage).ToBeVisibleAsync();
        await Expect(editor).ToBeVisibleAsync();
        await Expect(back).ToBeHiddenAsync();
        (left, right) = (await Box(storage), await Box(editor));
        Assert.True(right.Y >= left.Y + left.Height, "The panes are not stacked at tablet width.");
        await Expect(fold).ToHaveAttributeAsync("aria-expanded", "true");
        await Expect(fold).ToHaveAttributeAsync("aria-controls", "pane-storage");
        await fold.ClickAsync();
        await Expect(fold).ToHaveAttributeAsync("aria-expanded", "false");
        await Expect(storage).ToBeHiddenAsync();
        await page.SetViewportSizeAsync(1280, 900);
        await Expect(storage).ToBeVisibleAsync();
        await Expect(fold).ToBeHiddenAsync();
        await page.SetViewportSizeAsync(800, 900);
        await Expect(storage).ToBeHiddenAsync();
        await fold.ClickAsync();
        await Expect(storage).ToBeVisibleAsync();

        // Nothing was lost on the way: the draft, the selected slot and the box shown.
        await Expect(page.Locator("#nickname")).ToHaveValueAsync("Resized");
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(SlotText.Position(SaveFixtures.FirstBoxSlot));
        await Expect(page.Locator("#box-select")).ToHaveValueAsync("0");

        // The phone's pane choice outlived the detour through wider layouts; cancelling the draft keeps the clean draft, and its editor, open.
        await page.SetViewportSizeAsync(375, 800);
        await Expect(editor).ToBeHiddenAsync();
        await page.Locator("#box-grid-0").ClickAsync();
        await Expect(editor).ToBeVisibleAsync();
        await page.Locator("#cancel-draft").ClickAsync();
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");
        await Expect(editor).ToBeVisibleAsync();
        await session.AssertNoNetworkOrPersistenceAsync();
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ReflowsAtFourHundredPercentWithTargetsAndNoMotion(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;

        // 1280px at 400% zoom is 320 CSS pixels: nothing scrolls sideways, in any state.
        await page.SetViewportSizeAsync(320, 256);
        await AssertNoSidewaysScroll(page, "start");
        await page.Locator("#about-toggle").ClickAsync();
        await page.Locator("#about-diag-prepare").ClickAsync();
        await AssertNoSidewaysScroll(page, "About with a diagnostic report");
        await page.Locator("#about-toggle").ClickAsync();
        // Adamant lowers Sp. Atk, so the inspector's stats table has its longest row header ("Sp. Atk (lowered by nature)").
        await Load(page, SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember(battle: pk => pk.Nature = Nature.Adamant)));
        await AssertNoSidewaysScroll(page, "loaded");
        await Select(page, PKHeX.Web.State.SlotRef.InParty(0));
        await AssertNoSidewaysScroll(page, "party member in the editor");
        // Download stays reachable: scrolled to, nothing covers it.
        await AssertReachable(page, "#download");
        await AssertReachable(page, "#apply");

        // Targets: every control is at least 44px tall, on the editor and the storage pane alike.
        await page.SetViewportSizeAsync(375, 800);
        await AssertTargets(page, "editor");
        await page.Locator("#editor-return").ClickAsync();
        await AssertTargets(page, "storage");
        await AssertNoMotion(page, "storage grid");
        await page.Locator("#storage-as-list").CheckAsync();
        await AssertTargets(page, "storage as a list");
        await page.Locator("#about-toggle").ClickAsync();
        await AssertTargets(page, "About with a diagnostic report");

        // No motion, with or without the reduced-motion preference.
        await AssertNoMotion(page, "default");
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await AssertNoMotion(page, "reduced motion");
        await session.AssertNoNetworkOrPersistenceAsync();
    }

    private static async Task<LocatorBoundingBoxResult> Box(ILocator locator) =>
        await locator.BoundingBoxAsync() ?? throw new InvalidOperationException("The element has no box.");

    private static async Task AssertNoSidewaysScroll(IPage page, string state)
    {
        var overflow = await page.EvaluateAsync<int>("() => document.documentElement.scrollWidth - document.documentElement.clientWidth");
        Assert.True(overflow <= 0, $"The page scrolls sideways by {overflow}px ({state}).");
    }

    /// <summary>Scrolls <paramref name="selector"/> into view and checks that it, not something on top of it, is under its centre.</summary>
    private static async Task AssertReachable(IPage page, string selector)
    {
        var reachable = await page.EvaluateAsync<bool>("""
            selector => {
                const element = document.querySelector(selector);
                element.scrollIntoView({ block: 'center' });
                const box = element.getBoundingClientRect();
                const hit = document.elementFromPoint(box.left + box.width / 2, box.top + box.height / 2);
                return hit !== null && (hit === element || element.contains(hit));
            }
            """, selector);
        Assert.True(reachable, $"{selector} is covered or out of reach.");
    }

    /// <summary>
    /// Every visible control is at least 44px tall: buttons, fields, selects, disclosures and links that stand alone. A checkbox or radio is
    /// measured by its label, which is its target; links inside a sentence are exempt (WCAG 2.5.8).
    /// </summary>
    private static async Task AssertTargets(IPage page, string state)
    {
        var small = await page.EvaluateAsync<string[]>("""
            () => {
                const visible = e => e.getClientRects().length > 0 && getComputedStyle(e).visibility !== 'hidden';
                const controls = document.querySelectorAll('button, input, select, summary, a[href]');
                const found = [];
                for (const control of controls) {
                    if (!visible(control) || control.closest('.visually-hidden')) continue;
                    if (control.tagName === 'A' && control.closest('p')) continue;
                    const target = control.type === 'checkbox' || control.type === 'radio' ? control.closest('label') : control;
                    const height = target.getBoundingClientRect().height;
                    if (height < 43.5) found.push(`${control.tagName.toLowerCase()}#${control.id || '?'} (${Math.round(height)}px)`);
                }
                return found;
            }
            """);
        Assert.True(small.Length == 0, $"Targets under 44px ({state}): {string.Join(", ", small)}");
    }

    /// <summary>
    /// Nothing moves: no rule in any stylesheet declares a transition or animation (so it holds for elements not on screen), and no element
    /// shown has one, nor smooth scrolling. Under a reduced-motion preference the stylesheet's override would also stop any that were added.
    /// </summary>
    private static async Task AssertNoMotion(IPage page, string state)
    {
        var moving = await page.EvaluateAsync<string[]>("""
            () => {
                const moving = [];
                const seconds = list => list.split(',').map(v => parseFloat(v) * (v.trim().endsWith('ms') ? 0.001 : 1));
                const moves = style => [...seconds(style.transitionDuration || '0s'), ...seconds(style.animationDuration || '0s')].some(d => d > 0);
                const walk = rules => {
                    for (const rule of rules) {
                        if (rule.cssRules) walk(rule.cssRules);
                        if (rule.style && moves(rule.style)) moving.push('rule ' + rule.selectorText);
                    }
                };
                for (const sheet of document.styleSheets) walk(sheet.cssRules);
                for (const element of document.querySelectorAll('*')) {
                    const style = getComputedStyle(element);
                    if (moves(style) && (style.animationName !== 'none' || seconds(style.transitionDuration).some(d => d > 0))) {
                        moving.push(element.tagName.toLowerCase() + '#' + (element.id || '?'));
                    }
                }
                if (getComputedStyle(document.documentElement).scrollBehavior === 'smooth') moving.push('smooth scrolling');
                return moving;
            }
            """);
        Assert.True(moving.Length == 0, $"Motion found ({state}): {string.Join(", ", moving)}");
    }
}
