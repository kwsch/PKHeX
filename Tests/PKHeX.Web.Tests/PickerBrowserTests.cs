using System.Globalization;
using Microsoft.Playwright;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The searchable move and item boxes in the published app: typing narrows a box without changing the draft, accents and case are ignored in
/// every engine's runtime, a choice from the narrowed box is an ordinary edit, and the export matches native Core.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class PickerBrowserTests(PublishedAppFixture app)
{
    private static readonly SlotRef Boxed = SlotRef.InBox(0, 1);

    private static string Id(int value) => value.ToString(CultureInfo.InvariantCulture);

    /// <summary>Whether <paramref name="condition"/>, a script returning a boolean, becomes true within Playwright's default timeout.</summary>
    private static async Task<bool> LaidOutAsync(IPage page, string condition)
    {
        try
        {
            await page.WaitForFunctionAsync(condition);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task SearchNarrowsTheBoxesAndAChoiceFromThemMatchesNativeCore(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // Tackle (20 of 49 PP, 2 PP Ups), an empty slot, Surf and Sketch, holding Leftovers.
        var bytes = ItemMoveDraftTests.Moveset();
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = (PK6)changed.GetBoxSlotAtIndex(0, 1);
        edited.HeldItem = (int)Ball.Poke;
        edited.Move1 = (ushort)Move.Thunderbolt;
        edited.Move1_PP = edited.GetMovePP((ushort)Move.Thunderbolt, edited.Move1_PPUps);
        changed.SetBoxSlotAtIndex(edited, 0, 1, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        var names = GameInfo.Strings;
        await Load(page, bytes);
        await Select(page, Boxed);

        // Typing narrows the box to the matches, Tackle (the drafted move) and (None), and changes nothing.
        await page.Locator("#move-0-search").PressSequentiallyAsync("THUN");
        var thunder = new[] { Move.Thunder, Move.ThunderFang, Move.ThunderPunch, Move.ThunderShock, Move.ThunderWave, Move.Thunderbolt };
        await Expect(page.Locator("#move-0 option")).ToHaveCountAsync(thunder.Length + 2);
        await Expect(page.Locator("#move-0")).ToHaveValueAsync(Id((int)Move.Tackle));
        await Expect(page.Locator("#move-0-search-note")).ToContainTextAsync("6 of ");
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");
        await Expect(page.Locator("#move-note")).ToBeEmptyAsync();
        // Each box has its own search.
        await Expect(page.Locator("#move-1 option")).ToHaveCountAsync(SaveFixtures.Open(bytes).Capabilities.Lists.Moves.Count);

        await page.Locator("#move-0").SelectOptionAsync(Id((int)Move.Thunderbolt));
        await Expect(page.Locator("#move-note")).ToHaveTextAsync($"Move 1 is now {names.movelist[(int)Move.Thunderbolt]}, with full PP: {edited.Move1_PP} of {edited.Move1_PP} (2 PP Ups).");

        // Accents and case are folded by the app, not by the browser's collation data.
        await page.Locator("#held-item-search").FillAsync("poke ball");
        var pokeBall = page.Locator("#held-item option", new() { HasText = names.itemlist[(int)Ball.Poke] });
        await Expect(pokeBall).ToHaveCountAsync(1);
        await page.Locator("#held-item").SelectOptionAsync(Id((int)Ball.Poke));
        await Expect(page.Locator("#inspect-item")).ToContainTextAsync(names.itemlist[(int)Ball.Poke]);
        await page.Locator("#held-item-search").FillAsync("zzzz");
        await Expect(page.Locator("#held-item-search-note")).ToHaveTextAsync(EditorText.SearchNote("zzzz", 0, 0, "item"));
        // A search that matches nothing keeps the choice.
        await Expect(page.Locator("#held-item")).ToHaveValueAsync(Id((int)Ball.Poke));

        await Apply(page);
        // The reopened draft starts with no search.
        await Expect(page.Locator("#move-0-search")).ToHaveValueAsync("");
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native picker output differs.");
        AssertOnlyRangeDiffers(bytes, output, native.GetBoxSlotOffset(0, 1), native.SIZE_BOXSLOT);

        // On a phone the search sits above its box, and neither scrolls the page sideways.
        // Firefox can resolve the resize before the page is laid out at the new width, so both checks wait for it rather than measuring once.
        await page.SetViewportSizeAsync(320, 800);
        await page.WaitForFunctionAsync("() => window.innerWidth === 320");
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The editor scrolls horizontally at 320 px.");
        Assert.True(await LaidOutAsync(page, """
            () => {
                const search = document.getElementById('move-0-search')?.getBoundingClientRect();
                const box = document.getElementById('move-0')?.getBoundingClientRect();
                return !!search && !!box && search.height >= 44 && search.bottom <= box.top;
            }
            """), "The move search is not a 44px field above its box at 320 px.");
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}
