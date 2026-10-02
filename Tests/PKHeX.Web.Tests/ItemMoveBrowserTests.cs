using System.Globalization;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The held item, moves, PP and PP Ups in the published app (WEB-PKM-010, WEB-PKM-011, WEB-PKM-012): Core's lists offered, a move change's
/// PP effect stated, PP out of range refused as typed, a party member's battle state kept, and exports byte-identical to native Core.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class ItemMoveBrowserTests(PublishedAppFixture app)
{
    private const int Burn = 0x10;
    private static readonly SlotRef Boxed = SlotRef.InBox(0, 1);

    private static string Id(int value) => value.ToString(CultureInfo.InvariantCulture);

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ABoxedItemAndMoveEditMatchesNativeCore(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // Tackle (20 of 49 PP, 2 PP Ups), an empty slot, Surf (24 of 24, 3 PP Ups) and Sketch, holding Leftovers.
        var bytes = ItemMoveDraftTests.Moveset();
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = (PK6)changed.GetBoxSlotAtIndex(0, 1);
        edited.HeldItem = ItemMoveDraftTests.ChoiceScarf;
        edited.Move1 = (ushort)Move.Thunderbolt;
        edited.Move1_PP = 12;
        edited.Move2 = (ushort)Move.Protect;
        edited.Move2_PPUps = 0;
        edited.Move2_PP = edited.GetMovePP((ushort)Move.Protect, 0);
        edited.Move3_PPUps = 1;
        edited.Move3_PP = edited.GetMovePP((ushort)Move.Surf, 1);
        edited.Move4 = 0;
        edited.Move4_PP = 0;
        changed.SetBoxSlotAtIndex(edited, 0, 1, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        var names = GameInfo.Strings;
        await Load(page, bytes);
        await Select(page, Boxed);
        await Expect(page.Locator("#message")).ToContainTextAsync("held item, moves, PP and PP Ups can be changed");
        await Expect(page.Locator("#move-list-note")).ToHaveTextAsync(EditorText.MoveListNote);

        await page.Locator("#held-item").SelectOptionAsync(Id(ItemMoveDraftTests.ChoiceScarf));
        await Expect(page.Locator("#inspect-item")).ToContainTextAsync(names.itemlist[ItemMoveDraftTests.ChoiceScarf]);

        // A move change gets full PP for the PP Ups it keeps, and the note says so.
        await page.Locator("#move-0").SelectOptionAsync(Id((int)Move.Thunderbolt));
        await Expect(page.Locator("#move-note")).ToHaveTextAsync($"Move 1 is now {names.movelist[(int)Move.Thunderbolt]}, with full PP: 21 of 21 (2 PP Ups).");
        await Expect(page.Locator("#pp-0")).ToHaveValueAsync("21");
        await Expect(page.Locator("#inspect-moves-table")).ToContainTextAsync(names.movelist[(int)Move.Thunderbolt]);

        // PP above the maximum is refused as typed and blocks Apply; PP typed a key at a time within it is accepted.
        await page.Locator("#pp-0").FillAsync("22");
        await Expect(page.Locator("#message")).ToHaveTextAsync(UserMessages.For(SessionError.PpOutOfRange));
        await Expect(page.Locator("#pp-0")).ToHaveValueAsync("22");
        await Expect(page.Locator("#apply")).ToBeDisabledAsync();
        await page.Locator("#pp-0").FillAsync("");
        await page.Locator("#pp-0").PressSequentiallyAsync("12");
        await Expect(page.Locator("#message")).ToBeEmptyAsync();
        await Expect(page.Locator("#move-note")).ToBeEmptyAsync();

        // An empty slot filled, a PP Ups change that refills PP, and a slot emptied.
        await Expect(page.Locator("#pp-1")).ToHaveAttributeAsync("readonly", "");
        await page.Locator("#move-1").SelectOptionAsync(Id((int)Move.Protect));
        await Expect(page.Locator("#pp-1")).ToHaveValueAsync(Id(edited.Move2_PP));
        await page.Locator("#ppups-2").SelectOptionAsync("1");
        await Expect(page.Locator("#pp-2")).ToHaveValueAsync(Id(edited.Move3_PP));
        await Expect(page.Locator("#move-note")).ToHaveTextAsync($"Move 3, {names.movelist[(int)Move.Surf]}, now has 1 PP Up; its PP is {edited.Move3_PP} of {edited.Move3_PP}.");
        await page.Locator("#move-3").SelectOptionAsync("0");
        await Expect(page.Locator("#move-note")).ToHaveTextAsync("Move 4 is now empty, so its PP and PP Ups are 0.");
        await Expect(page.Locator("#ppups-3")).ToBeDisabledAsync();
        await Expect(page.Locator("#apply")).ToBeEnabledAsync();

        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync("Changes applied in memory; download to retain changes.");
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(NativeLegality.Of(changed, changed.GetBoxSlotAtIndex(0, 1), StorageSlotType.Box).Verdict);
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native item and move output differs.");
        AssertOnlyRangeDiffers(bytes, output, native.GetBoxSlotOffset(0, 1), native.SIZE_BOXSLOT);

        await page.SetViewportSizeAsync(375, 800);
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The editor scrolls horizontally at 375 px.");
        // The fixed number columns must leave the move box room for a move's name, as the phone layout gives it a line of its own.
        var moveBox = await page.Locator("#move-0").BoundingBoxAsync();
        Assert.True(moveBox is { Width: >= 150 }, "The move box is too narrow at 375 px.");
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task APartyMoveEditKeepsTheBattleState(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // The leader is injured and burned, with a stored Attack Core would not calculate: a heal, a cleared status or a recalculation would show.
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember("Leader", p =>
        {
            p.Stat_HPCurrent = 7;
            p.Status_Condition = Burn;
            p.Stat_ATK = 1;
        }));
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = changed.GetPartySlotAtIndex(0);
        edited.Move2 = (ushort)Move.Surf;
        edited.Move2_PP = edited.GetMovePP((ushort)Move.Surf, edited.Move2_PPUps);
        edited.HeldItem = ItemMoveDraftTests.Leftovers;
        changed.SetPartySlotAtIndex(edited, 0, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        await Load(page, bytes);
        await Select(page, SlotRef.InParty(0));
        await Expect(page.Locator("#message")).ToContainTextAsync(PartyText.KeptOnEdit);

        await page.Locator("#move-1").SelectOptionAsync(Id((int)Move.Surf));
        await page.Locator("#held-item").SelectOptionAsync(Id(ItemMoveDraftTests.Leftovers));
        await Expect(page.Locator("#inspect-hp")).ToContainTextAsync("7 of ");
        await Expect(page.Locator("#party-hp-preview")).ToHaveCountAsync(0);
        await Expect(page.Locator("#inspect-stats-table caption")).Not.ToContainTextAsync("recalculated");

        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native party move output differs.");
        var result = SaveFixtures.Parse(output).GetPartySlotAtIndex(0);
        Assert.Equal((Burn, 7, 1), (result.Status_Condition, result.Stat_HPCurrent, result.Stat_ATK));

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task TheBoxesShowWhatTheDraftHolds(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, ItemMoveDraftTests.Moveset());
        await Select(page, Boxed);

        // Typing a name into a closed move box picks a move per keystroke: "sky" passes Sketch, which cannot take PP Ups, on the way to Sky
        // Attack; Surf's 3 PP Ups must reach Sky Attack.
        var surf = page.Locator("#move-2");
        await surf.FocusAsync();
        await page.Keyboard.TypeAsync("sky", new() { Delay = 50 });
        await Expect(surf).ToHaveValueAsync(Id((int)Move.SkyAttack));
        await Expect(page.Locator("#ppups-2")).ToHaveValueAsync("3");
        await Expect(page.Locator("#pp-2")).ToHaveValueAsync("8");

        // A box the user changed must still follow the draft: Tackle's stored 2 PP Ups come back with Tackle. Firefox ignores an option's
        // selected attribute once the user has chosen, so the value must be set on the select.
        await page.Locator("#ppups-0").SelectOptionAsync("0");
        await page.Locator("#move-0").SelectOptionAsync(Id((int)Move.Thunderbolt));
        await page.Locator("#move-0").SelectOptionAsync(Id((int)Move.Tackle));
        await Expect(page.Locator("#move-note")).ToContainTextAsync("is back to its stored move");
        await Expect(page.Locator("#ppups-0")).ToHaveValueAsync("2");
        await Expect(page.Locator("#pp-0")).ToHaveValueAsync("20");

        // The same for the next Pokémon: its stored nature, not the one chosen for this one.
        var nature = await page.Locator("#nature").InputValueAsync();
        await page.Locator("#nature").SelectOptionAsync(nature == "3" ? "4" : "3");
        await page.Locator("#cancel-draft").ClickAsync();
        await Select(page);
        var stored = new PK6(SaveFixtures.ReadEntity(true));
        await Expect(page.Locator("#nature")).ToHaveValueAsync(Id((int)stored.Nature));
        await Expect(page.Locator("#move-0")).ToHaveValueAsync(Id(stored.Move1));

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}
