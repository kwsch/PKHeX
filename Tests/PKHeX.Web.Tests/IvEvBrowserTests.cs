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
/// IVs and EVs in the published app (WEB-PKM-013): values out of range and EVs over the total refused as typed, the totals and Hidden Power
/// shown, a party member's stats recalculated without healing it, and exports byte-identical to native Core.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class IvEvBrowserTests(PublishedAppFixture app)
{
    private const int Burn = 0x10;

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ABoxedIvAndEvEditMatchesNativeCore(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // The known legal Zigzagoon, with no EVs, so the edits below reach the 510 total exactly.
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 0, p => p.SetEVs([0, 0, 0, 0, 0, 0])));
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = changed.GetBoxSlotAtIndex(0, 0);
        var speedIv = edited.IV_SPE == 31 ? 30 : 31;
        edited.IV_SPE = speedIv;
        edited.EV_ATK = 252;
        edited.EV_SPE = 252;
        edited.EV_HP = 6;
        changed.SetBoxSlotAtIndex(edited, 0, 0, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        await Load(page, bytes);
        await Select(page);
        await Expect(page.Locator("#message")).ToContainTextAsync("nature, IVs and EVs can be changed");

        // An IV out of range is refused as typed and blocks Apply until corrected.
        var speed = page.Locator("#iv-5");
        await speed.FillAsync("32");
        await Expect(page.Locator("#message")).ToHaveTextAsync(UserMessages.For(SessionError.IvOutOfRange));
        await Expect(speed).ToHaveValueAsync("32");
        await Expect(page.Locator("#apply")).ToBeDisabledAsync();
        await speed.FillAsync(speedIv.ToString(CultureInfo.InvariantCulture));
        await Expect(page.Locator("#message")).ToBeEmptyAsync();
        var hiddenPower = GameInfo.Strings.HiddenPowerTypes[edited.HPType];
        await Expect(page.Locator("#iv-note")).ToHaveTextAsync(EditorText.IvNote(edited.IVTotal, 31, hiddenPower));
        await Expect(page.Locator("#inspect-hidden-power")).ToHaveTextAsync(hiddenPower);

        // EVs typed a key at a time, up to the 510 total; one more is refused as typed.
        await TypeAsync(page.Locator("#ev-1"), "252");
        await TypeAsync(page.Locator("#ev-5"), "252");
        await Expect(page.Locator("#ev-note")).ToHaveTextAsync("EV total 504 of 510; 6 remaining.");
        await page.Locator("#ev-0").FillAsync("7");
        await Expect(page.Locator("#message")).ToHaveTextAsync(UserMessages.For(SessionError.EvTotalAboveLimit));
        await Expect(page.Locator("#ev-0")).ToHaveValueAsync("7");
        await Expect(page.Locator("#apply")).ToBeDisabledAsync();
        await page.Locator("#ev-0").FillAsync("6");
        await Expect(page.Locator("#ev-note")).ToHaveTextAsync(EditorText.EvNote(510, 510));
        await Expect(page.Locator("#inspect-ev-total")).ToContainTextAsync("510");
        await Expect(page.Locator("#apply")).ToBeEnabledAsync();

        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync("Changes applied in memory; download to retain changes.");
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(NativeLegality.Of(changed, changed.GetBoxSlotAtIndex(0, 0), StorageSlotType.Box).Verdict);
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native IV and EV output differs.");
        AssertOnlyRangeDiffers(bytes, output, native.GetBoxSlotOffset(0, 0), native.SIZE_BOXSLOT);

        await page.SetViewportSizeAsync(375, 800);
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The editor scrolls horizontally at 375 px.");
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task APartyIvAndEvEditRecalculatesWithoutHealing(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // The leader is at level 50 with an HP IV of 31 and no EVs, at full HP and burned: more HP EVs raise its maximum but must not heal
        // it, and an HP IV of 0 lowers its maximum below its current HP.
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember("Leader", p =>
        {
            p.EXP = Experience.GetEXP(50, p.PersonalInfo.EXPGrowth);
            p.IV_HP = 31;
            p.SetEVs([0, 0, 0, 0, 0, 0]);
            p.ResetPartyStats();
            p.Status_Condition = Burn;
        }));
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = changed.GetPartySlotAtIndex(0);
        var previousHp = edited.Stat_HPCurrent;
        var previousMax = edited.Stat_HPMax;
        edited.IV_HP = 0;
        var stats = edited.GetStats(edited.PersonalInfo);
        edited.SetStats(stats);
        edited.Stat_HPCurrent = Math.Min(previousHp, (int)stats[0]);
        changed.SetPartySlotAtIndex(edited, 0, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        await Load(page, bytes);
        await Select(page, SlotRef.InParty(0));
        await Expect(page.Locator("#message")).ToContainTextAsync(PartyText.KeptOnEdit);

        // HP EVs typed a key at a time raise the maximum but never the current HP.
        await TypeAsync(page.Locator("#ev-0"), "252");
        var raised = native.GetPartySlotAtIndex(0);
        raised.EV_HP = 252;
        var raisedMax = raised.GetStats(raised.PersonalInfo)[0];
        Assert.True(raisedMax > previousMax, "The fixture must raise the maximum HP.");
        await Expect(page.Locator("#inspect-hp")).ToHaveTextAsync($"{previousHp} of {raisedMax}");
        await Expect(page.Locator("#inspect-stats-table caption")).ToContainTextAsync("recalculated");
        await Expect(page.Locator("#party-hp-preview")).ToHaveCountAsync(0);

        // Back to no EVs, then an HP IV of 0, lowers the maximum below the current HP, which is previewed.
        await TypeAsync(page.Locator("#ev-0"), "0");
        await TypeAsync(page.Locator("#iv-0"), "0");
        await Expect(page.Locator("#party-hp-preview")).ToHaveTextAsync(PartyText.HpPreview(new PartyHpChange(previousHp, edited.Stat_HPCurrent, previousMax, edited.Stat_HPMax))!);

        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native party IV output differs.");
        var result = SaveFixtures.Parse(output).GetPartySlotAtIndex(0);
        Assert.Equal(Burn, result.Status_Condition);
        Assert.True(result.Stat_HPCurrent < previousHp, "The fixture must lower HP.");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    /// <summary>Clears <paramref name="field"/> and types <paramref name="text"/> a key at a time, so every intermediate value is an edit.</summary>
    private static async Task TypeAsync(Microsoft.Playwright.ILocator field, string text)
    {
        await field.FillAsync("");
        await field.PressSequentiallyAsync(text);
        await Expect(field).ToHaveValueAsync(text);
    }
}
