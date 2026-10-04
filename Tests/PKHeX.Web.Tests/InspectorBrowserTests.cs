using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The read-only inspector in the published app: the same values as native Core, unapplied edits shown, party members' stored stats,
/// and no horizontal scroll on a phone.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class InspectorBrowserTests(PublishedAppFixture app)
{
    /// <summary>Values checked against native Core: one or more from each section.</summary>
    private static readonly string[] CheckedIds =
    [
        "inspect-species", "inspect-nickname", "inspect-level", "inspect-exp", "inspect-nature", "inspect-ability", "inspect-item",
        "inspect-ev-total", "inspect-characteristic", "inspect-hidden-power", "inspect-relearn-1",
        "inspect-ot", "inspect-tid", "inspect-origin-game", "inspect-met-location", "inspect-ball",
        "inspect-pid", "inspect-ec", "inspect-ribbons", "inspect-checksum",
    ];

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ShowsTheDraftedEntityAsNativeCoreReadsIt(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember("Leader"));
        var native = SaveFixtures.Open(bytes);
        await Load(page, bytes);

        // A boxed Pokémon: five headed sections, every checked value as native Core gives it, stats calculated.
        await Select(page);
        var boxed = InspectorText.Sections(native.Select(SaveFixtures.FirstBoxSlot).Inspect());
        await Expect(page.Locator("#inspector section h3")).ToHaveTextAsync(boxed.Select(s => s.Title).ToArray());
        await ExpectValues(page, boxed);
        await Expect(page.Locator("#inspect-stats-table tbody tr")).ToHaveCountAsync(6);
        await Expect(page.Locator("#inspect-stats-table caption")).ToContainTextAsync("calculated");
        await Expect(page.Locator("#inspect-moves-table tbody tr")).ToHaveCountAsync(4);
        await Expect(page.Locator("#inspect-hp")).ToHaveCountAsync(0);

        // An unapplied nickname edit shows at once; a refused one leaves the last accepted value.
        await page.Locator("#nickname").FillAsync("Drafted");
        await Expect(page.Locator("#inspect-nickname")).ToHaveTextAsync("Drafted");
        await Expect(page.Locator("#inspect-checksum")).ToHaveTextAsync("Valid");
        await page.Locator("#nickname").EvaluateAsync("(e, v) => { e.value = v; e.dispatchEvent(new Event('input', { bubbles: true })); }", "Drafted\u0007");
        await Expect(page.Locator("#inspect-nickname")).ToHaveTextAsync("Drafted");
        await page.Locator("#cancel-draft").ClickAsync();
        await Expect(page.Locator("#inspect-nickname")).ToHaveTextAsync(boxed.SelectMany(s => s.Rows).Single(r => r.Id == "inspect-nickname").Value);

        // A party member: stored stats, HP and status.
        await Select(page, SlotRef.InParty(0));
        var party = InspectorText.Sections(native.Select(SlotRef.InParty(0)).Inspect());
        await ExpectValues(page, party);
        await Expect(page.Locator("#inspect-stats-table caption")).ToContainTextAsync("stored");
        await Expect(page.Locator("#inspect-hp")).ToHaveTextAsync(party.SelectMany(s => s.Rows).Single(r => r.Id == "inspect-hp").Value);
        await Expect(page.Locator("#inspect-nickname")).ToHaveTextAsync("Leader");

        // The inspector fits a phone without horizontal scrolling.
        await page.SetViewportSizeAsync(375, 800);
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The inspector scrolls horizontally at 375 px.");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    /// <summary>Checks each of <see cref="CheckedIds"/> shows the value native Core gives it.</summary>
    private static async Task ExpectValues(Microsoft.Playwright.IPage page, IReadOnlyList<InspectorSection> expected)
    {
        var rows = expected.SelectMany(s => s.Rows).ToDictionary(r => r.Id, r => r.Value);
        foreach (var id in CheckedIds)
        {
            await Expect(page.Locator($"#{id}")).ToHaveTextAsync(rows[id]);
        }
    }
}
