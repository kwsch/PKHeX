using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The party strip and box grid in the published app: navigation, keyboard use, the list alternative, and what each slot opens.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class StorageBrowserTests(PublishedAppFixture app)
{
    /// <summary>Box 1's stored name: markup, to show stored names are rendered as text.</summary>
    private const string HostileBoxName = "<i>K</i>";

    /// <summary>An XY save with a party member, box 1 renamed, and box 3 as the in-game current box.</summary>
    private static byte[] Fixture() => SaveFixtures.Synthetic(false, customize: save =>
    {
        SaveFixtures.WithPartyMember("Leader")(save);
        ((IBoxDetailName)save).SetBoxName(0, HostileBoxName);
        save.CurrentBox = 2;
    });

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task BrowsesPartyAndBoxesByPointerKeyboardAndList(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        var bytes = Fixture();
        var opened = SaveFixtures.Open(bytes);
        await Load(page, bytes);

        // Box navigation: Core's 31 boxes, starting at the in-game current box, wrapping at both ends.
        await Expect(page.Locator("#box-select option")).ToHaveCountAsync(31);
        await Expect(page.Locator("#box-select")).ToHaveValueAsync("2");
        await Expect(page.Locator("#box-title")).ToHaveTextAsync(SlotText.BoxOption(2, StorageView.BoxName(opened, 2)));
        await page.Locator("#box-prev").ClickAsync();
        await page.Locator("#box-prev").ClickAsync();
        await Expect(page.Locator("#box-select")).ToHaveValueAsync("0");
        await Expect(page.Locator("#box-title")).ToHaveTextAsync("1. " + TestText.Isolated(HostileBoxName));
        await Expect(page.Locator("#box-title i")).ToHaveCountAsync(0);
        await page.Locator("#box-prev").ClickAsync();
        await Expect(page.Locator("#box-select")).ToHaveValueAsync("30");
        await page.Locator("#box-next").ClickAsync();
        await Expect(page.Locator("#box-select")).ToHaveValueAsync("0");
        await Expect(page.Locator("#box-grid button")).ToHaveCountAsync(30);
        await Expect(page.Locator("#party-grid button")).ToHaveCountAsync(6);
        await Expect(page.Locator("#party-title")).ToHaveTextAsync("Party (1 of 6)");

        // Keyboard only: one tab stop, arrows move focus and their default (page scrolling) is cancelled, and Tab leaves the grid.
        // Focus moving to a slot may still scroll it into view, so the check is on the key events, recorded after grid-keys.js ran.
        await page.SetViewportSizeAsync(375, 500);
        await Expect(page.Locator("#box-grid button[tabindex='0']")).ToHaveCountAsync(1);
        await page.EvaluateAsync("() => { window.gridKeys = []; window.addEventListener('keydown', e => window.gridKeys.push(e.key + ':' + e.defaultPrevented)); }");
        await page.Locator("#box-grid-0").FocusAsync();
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(page.Locator("#box-grid-1")).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("ArrowDown");
        await Expect(page.Locator("#box-grid-7")).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Home");
        await Expect(page.Locator("#box-grid-6")).ToBeFocusedAsync();
        Assert.Equal(["ArrowRight:true", "ArrowDown:true", "Home:true"], await page.EvaluateAsync<string[]>("() => window.gridKeys"));
        await page.Keyboard.PressAsync("Control+Home");
        await Expect(page.Locator("#box-grid-0")).ToBeFocusedAsync();
        await Expect(page.Locator("#box-grid button[tabindex='0']")).ToHaveIdAsync("box-grid-0");
        await page.Keyboard.PressAsync("Tab");
        Assert.Equal("Tab:false", (await page.EvaluateAsync<string[]>("() => window.gridKeys"))[^1]);
        Assert.False(await page.EvaluateAsync<bool>("() => document.activeElement.closest('[role=grid]') !== null"), "Tab did not leave the grid.");
        await page.Keyboard.PressAsync("Shift+Tab");
        await Expect(page.Locator("#box-grid-0")).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Enter");
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(SlotText.Position(SaveFixtures.FirstBoxSlot));
        var boxed = StorageView.Box(opened, 0).Slots[0];
        await Expect(page.Locator("#message")).ToHaveTextAsync($"Opened {SlotText.Label(boxed)}. {EditorText.EditableSummary(opened.Capabilities.Editable)}");
        await Expect(page.Locator("#box-grid [role=gridcell][aria-selected=true] button")).ToHaveIdAsync("box-grid-0");
        // At this width the editor replaces the party and boxes, so focus moves to its heading; the return action brings the slot back.
        await Expect(page.Locator("#draft-title")).ToBeFocusedAsync();
        await page.Locator("#editor-return").ClickAsync();
        await Expect(page.Locator("#box-grid-0")).ToBeFocusedAsync();

        // An empty slot opens nothing and closes the clean draft.
        await page.Locator("#box-grid-1").ClickAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync("Box 1, slot 2 (row 1, column 2) is empty.");
        await Expect(page.Locator("#nickname")).ToHaveCountAsync(0);

        // A party member opens for editing, says its battle state is kept, and is analysed as a party member.
        await Select(page, SlotRef.InParty(0));
        var leader = StorageView.Party(opened)[0];
        await Expect(page.Locator("#message")).ToHaveTextAsync($"Opened {SlotText.Label(leader)}. {EditorText.EditableSummary(opened.Capabilities.Editable)} {PartyText.KeptOnEdit}");
        await Expect(page.Locator("#nickname")).ToHaveValueAsync("Leader");
        await Expect(page.Locator("#nickname")).ToBeEditableAsync();
        await Expect(page.Locator("#party-note")).ToHaveCountAsync(0);
        var native = SaveFixtures.Parse(bytes);
        await CheckLegality(page, native.GetPartySlotAtIndex(0), native, StorageSlotType.Party);

        // An unapplied draft is never replaced by opening another slot.
        await Select(page);
        await page.Locator("#nickname").FillAsync("Unapplied");
        // At this width the editor replaces the party and boxes; the return action brings them back, keeping the draft.
        await page.Locator("#editor-return").ClickAsync();
        await page.Locator("#party-grid-0").ClickAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync("Apply or cancel the draft before opening another slot.");
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(SlotText.Position(SaveFixtures.FirstBoxSlot));
        await page.Locator("#box-grid-0").ClickAsync();
        await page.Locator("#cancel-draft").ClickAsync();
        await page.Locator("#editor-return").ClickAsync();

        // The list alternative shows the same slots, with Open only where there is something to open.
        await page.Locator("#storage-as-list").CheckAsync();
        await Expect(page.Locator("#box-grid")).ToHaveCountAsync(0);
        await Expect(page.Locator("#box-list tbody tr")).ToHaveCountAsync(30);
        await Expect(page.Locator("#party-list-1")).ToHaveCountAsync(0);
        await page.Locator("#party-list-0").ClickAsync();
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(SlotText.Position(SlotRef.InParty(0)));
        await Expect(page.Locator("#party-list tr[aria-current=true] th")).ToHaveTextAsync("Party position 1");
        await page.Locator("#editor-return").ClickAsync();
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The list scrolls horizontally at 375 px.");
        await page.Locator("#storage-as-list").UncheckAsync();
        await Expect(page.Locator("#box-grid")).ToBeVisibleAsync();
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The grid scrolls horizontally at 375 px.");

        // Browsing is local: no requests, storage or page errors.
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}
