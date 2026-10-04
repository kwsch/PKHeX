using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Previous and Next Pokémon in the published app: party then boxes, skipping empty slots, wrapping, the box shown following the draft,
/// focus kept on the button, unapplied work never replaced, and on a phone the editor kept shown with the return action landing on the
/// stepped-to slot.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class NavigationBrowserTests(PublishedAppFixture app)
{
    private static readonly SlotRef Far = SlotRef.InBox(2, 4);

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task PreviousAndNextWalkTheSaveAndKeepUnappliedWork(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // Party 1, box 1 slot 1 and box 3 slot 5; everything else is empty.
        var bytes = SaveFixtures.Synthetic(false, customize: SaveFixtures.All(SaveFixtures.WithPartyMember(), SaveFixtures.WithBoxEntity(Far.Box, Far.Slot, _ => { })));
        await Load(page, bytes);
        await Select(page, SlotRef.InParty(0));
        var next = page.Locator("#draft-next");
        var previous = page.Locator("#draft-prev");
        await Expect(page.Locator("#draft-steps")).ToHaveAttributeAsync("aria-label", WorkspaceLayout.StepsLabel);

        await next.ClickAsync();
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(SlotText.Position(SaveFixtures.FirstBoxSlot));
        await Expect(page.Locator("#box-select")).ToHaveValueAsync("0");
        await Expect(page.Locator("#message")).ToContainTextAsync($"Opened {SlotText.Position(SaveFixtures.FirstBoxSlot)}");
        await next.PressAsync("Enter");
        // The empty slots between are skipped.
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(SlotText.Position(Far));
        // The box shown follows the draft.
        await Expect(page.Locator("#box-select")).ToHaveValueAsync("2");
        await Expect(next).ToBeFocusedAsync();
        await Expect(page.Locator("#box-grid-4")).ToHaveAttributeAsync("tabindex", "0");
        await next.PressAsync("Enter");
        // The last Pokémon wraps to the first.
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(SlotText.Position(SlotRef.InParty(0)));
        await previous.ClickAsync();
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(SlotText.Position(Far));

        // Unapplied work is kept: the summary says why and leads to Apply.
        await page.Locator("#ot-friendship").FillAsync("100");
        await Expect(next).ToHaveAttributeAsync("aria-disabled", "true");
        // Playwright will not click an aria-disabled button; a keyboard user reaches and activates it.
        await next.FocusAsync();
        await page.Keyboard.PressAsync("Enter");
        await Expect(page.Locator("#step-summary")).ToBeFocusedAsync();
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(SlotText.Position(Far));
        await page.Locator("#step-summary-list a").ClickAsync();
        await Expect(page.Locator("#apply")).ToBeFocusedAsync();
        await page.Locator("#cancel-draft").ClickAsync();
        await Expect(page.Locator("#step-summary")).ToHaveCountAsync(0);

        // On a phone the editor stays shown, and Back lands on the stepped-to slot.
        await page.SetViewportSizeAsync(375, 800);
        // By keyboard: WebKit, as Safari, does not focus a button that is clicked.
        await next.FocusAsync();
        await page.Keyboard.PressAsync("Enter");
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(SlotText.Position(SlotRef.InParty(0)));
        await Expect(page.Locator("#pane-editor")).ToBeVisibleAsync();
        await Expect(next).ToBeFocusedAsync();
        await previous.ClickAsync();
        await page.Locator("#editor-return").ClickAsync();
        await Expect(page.Locator("#box-grid-4")).ToBeFocusedAsync();
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The page scrolls horizontally at 375 px.");
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}
