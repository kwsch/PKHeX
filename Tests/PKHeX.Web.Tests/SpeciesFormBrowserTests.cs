using System.Globalization;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The species and form in the published app: a choice previewed with the fields Core changes before it is made, cancelled or
/// confirmed, battle-only forms marked, a party member's stats recalculated without healing it, and exports byte-identical to native Core.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class SpeciesFormBrowserTests(PublishedAppFixture app)
{
    private const int Burn = 0x10;
    private static readonly SlotRef Boxed = SlotRef.InBox(0, 1);

    private static string Id(int value) => value.ToString(CultureInfo.InvariantCulture);

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ABoxedSpeciesChangeIsPreviewedAndMatchesNativeCore(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        var bytes = AbilityGenderDraftTests.Boxed();
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = (PK6)changed.GetBoxSlotAtIndex(0, 1);
        var zigzagoon = edited.Nickname;
        edited.ChangeSpeciesForm((ushort)Species.Linoone, 0, changed.Personal);
        changed.SetBoxSlotAtIndex(edited, 0, 1, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        var linoone = GameInfo.Strings.specieslist[(int)Species.Linoone];
        await Load(page, bytes);
        await Select(page, Boxed);
        await Expect(page.Locator("#message")).ToContainTextAsync("Its species, form, nickname");
        await Expect(page.Locator("#form")).ToBeDisabledAsync();

        // A battle-only form is marked and explained, and cancelling leaves the draft as it was.
        await page.Locator("#species").SelectOptionAsync(Id((int)Species.Charizard));
        await Expect(page.Locator("#form option")).ToHaveTextAsync(["Normal", "Mega X (battle only)", "Mega Y (battle only)"]);
        await page.Locator("#form").SelectOptionAsync("1");
        await Expect(page.Locator("#species-preview")).ToContainTextAsync("exists only during battle");
        await page.Locator("#species-cancel").ClickAsync();
        await Expect(page.Locator("#species")).ToBeFocusedAsync();
        await Expect(page.Locator("#species")).ToHaveValueAsync(Id((int)Species.Zigzagoon));
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");

        await page.Locator("#species").SelectOptionAsync(Id((int)Species.Linoone));
        await Expect(page.Locator("#species-preview-title")).ToHaveTextAsync($"Change {GameInfo.Strings.specieslist[(int)Species.Zigzagoon]} to {linoone}?");
        await Expect(page.Locator("#species-preview-changes")).ToContainTextAsync($"Name: {TestText.Isolated(zigzagoon)} to {TestText.Isolated(linoone)}, as it is not nicknamed.");
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");
        await page.Locator("#species-confirm").ClickAsync();
        await Expect(page.Locator("#species-change")).ToContainTextAsync("The species and form were changed.");
        await Expect(page.Locator("#inspect-species")).ToContainTextAsync(linoone);
        await Expect(page.Locator("#nickname")).ToHaveValueAsync(linoone);

        await Apply(page);
        await Expect(page.Locator("#message")).ToHaveTextAsync("Changes applied in memory; download to retain changes.");
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(NativeLegality.Of(changed, changed.GetBoxSlotAtIndex(0, 1), StorageSlotType.Box).Verdict);
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native species output differs.");
        AssertOnlyRangeDiffers(bytes, output, native.GetBoxSlotOffset(0, 1), native.SIZE_BOXSLOT);

        await page.SetViewportSizeAsync(375, 800);
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The editor scrolls horizontally at 375 px.");
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task APartySpeciesChangeRecalculatesItsStatsWithoutHealingIt(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // The leader is injured and burned, with a stored Attack Core would not calculate: a heal, a cleared status or a missed
        // recalculation would show.
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember("Leader", p =>
        {
            p.Stat_HPCurrent = 7;
            p.Status_Condition = Burn;
            p.Stat_ATK = 1;
        }));
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = changed.GetPartySlotAtIndex(0);
        edited.ChangeSpeciesForm((ushort)Species.Chansey, 0, changed.Personal);
        edited.ResetPartyStats();
        edited.Stat_HPCurrent = 7;
        edited.Status_Condition = Burn;
        changed.SetPartySlotAtIndex(edited, 0, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        await Load(page, bytes);
        await Select(page, SlotRef.InParty(0));
        await Expect(page.Locator("#message")).ToContainTextAsync(PartyText.KeptOnEdit);

        await page.Locator("#species").SelectOptionAsync(Id((int)Species.Chansey));
        await Expect(page.Locator("#species-preview-changes")).ToContainTextAsync(string.Create(CultureInfo.InvariantCulture, $"HP {native.GetPartySlotAtIndex(0).Stat_HPMax} to {edited.Stat_HPMax}"));
        await Expect(page.Locator("#species-preview-changes")).ToContainTextAsync("Attack 1 to ");
        await page.Locator("#species-confirm").ClickAsync();
        await Expect(page.Locator("#inspect-stats-table caption")).ToContainTextAsync("recalculated");
        await Expect(page.Locator("#inspect-hp")).ToContainTextAsync(string.Create(CultureInfo.InvariantCulture, $"7 of {edited.Stat_HPMax}"));

        await Apply(page);
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native party species output differs.");
        var result = SaveFixtures.Parse(output).GetPartySlotAtIndex(0);
        Assert.Equal(((ushort)Species.Chansey, Burn, 7), (result.Species, result.Status_Condition, result.Stat_HPCurrent));

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ARefusedPreviewPutsTheBoxBackToTheDraftedSpecies(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        var bytes = PartyApplyTests.WithoutStoredStats();
        var species = SaveFixtures.Parse(bytes).GetPartySlotAtIndex(0).Species;
        await Load(page, bytes);
        await Select(page, SlotRef.InParty(0));

        await page.Locator("#species").SelectOptionAsync(Id((int)Species.Linoone));

        await Expect(page.Locator("#species-preview-refusal")).ToHaveTextAsync(UserMessages.For(SessionError.PartyStatsMissing));
        await Expect(page.Locator("#species")).ToHaveValueAsync(Id(species), new() { Timeout = 2000 });
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task APendingPreviewHoldsBackApplyAndDownload(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, AbilityGenderDraftTests.Boxed());
        await Select(page, Boxed);
        await page.Locator("#ot-friendship").FillAsync("100");
        await ReadyToApply(page);

        await page.Locator("#species").SelectOptionAsync(Id((int)Species.Linoone));

        await Expect(page.Locator("#apply")).ToBeDisabledAsync(new() { Timeout = 2000 });
        await Expect(page.Locator("#species-pending")).ToHaveTextAsync(EditorText.SpeciesFormPending);
        await page.Locator("#species-cancel").ClickAsync();
        await ReadyToApply(page);
        await Expect(page.Locator("#species-pending")).ToHaveCountAsync(0);
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}
