using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Applying a party member in the published app (WEB-SESSION-002, WEB-PKM-014): a nickname edit keeps the member's stored stats, HP and
/// status, the export matches native Core and changes only that party position, and the applied member is analysed as a party member.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class PartyApplyBrowserTests(PublishedAppFixture app)
{
    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task APartyNicknameEditKeepsBattleStateAndMatchesNativeCore(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // Two party members; the first is injured and burned, so a heal or a cleared status would show in the export.
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.All(
            SaveFixtures.WithPartyMember("Leader", p =>
            {
                p.Stat_HPCurrent = 7;
                p.Status_Condition = 0x10;
            }),
            SaveFixtures.WithPartyMember("Second", position: 1)));
        var native = SaveFixtures.Parse(bytes);
        var source = native.GetPartySlotAtIndex(0);
        var changed = native.Clone();
        var edited = changed.GetPartySlotAtIndex(0);
        edited.Nickname = "Renamed";
        edited.IsNicknamed = true;
        changed.SetPartySlotAtIndex(edited, 0, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        await Load(page, bytes);

        await Select(page, SlotRef.InParty(0));
        await Expect(page.Locator("#message")).ToContainTextAsync(PartyText.KeptOnEdit);
        await Expect(page.Locator("#party-note")).ToHaveCountAsync(0);
        await page.Locator("#nickname").FillAsync("Renamed");
        await page.Locator("#nicknamed").CheckAsync();
        await Expect(page.Locator("#party-hp-preview")).ToHaveCountAsync(0);
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#message")).ToContainTextAsync("applied in memory");
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");

        // The applied member is analysed again, as a party member.
        var party = NativeLegality.Of(changed, changed.GetPartySlotAtIndex(0), StorageSlotType.Party);
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(party.Verdict);
        Assert.True(await page.Locator("#legality-report-verbose").TextContentAsync() == party.VerboseReport, "The party report differs from native Core.");

        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native party edit output differs.");
        var reopened = SaveFixtures.Parse(output);
        Assert.True(reopened.PartyCount == 2, "Export changed party count.");
        var result = reopened.GetPartySlotAtIndex(0);
        Assert.Equal(7, result.Stat_HPCurrent);
        Assert.Equal(0x10, result.Status_Condition);
        AssertOnlyNicknameChanged(source, result);
        Assert.True(reopened.GetPartySlotAtIndex(1).Data.SequenceEqual(native.GetPartySlotAtIndex(1).Data), "The other party member changed.");
        AssertOnlyRangeDiffers(bytes, output, native.GetPartyOffset(0), native.SIZE_PARTY);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}
