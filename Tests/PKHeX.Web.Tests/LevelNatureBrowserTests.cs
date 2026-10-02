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
/// Level, experience points and nature in the published app (WEB-PKM-005, WEB-PKM-007, WEB-PKM-014): level and experience kept in step,
/// out-of-range values refused as typed, the nature's stat effect shown, a party member's stats recalculated without healing it, and
/// exports byte-identical to native Core.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class LevelNatureBrowserTests(PublishedAppFixture app)
{
    private const int Burn = 0x10;

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ABoxedLevelAndNatureEditMatchesNativeCore(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        var bytes = SaveFixtures.Synthetic(true);
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = changed.GetBoxSlotAtIndex(0, 0);
        var nature = edited.Nature == Nature.Adamant ? Nature.Modest : Nature.Adamant;
        edited.EXP = Experience.GetEXP(50, edited.PersonalInfo.EXPGrowth);
        edited.Nature = nature;
        changed.SetBoxSlotAtIndex(edited, 0, 0, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        await Load(page, bytes);
        await Select(page);
        await Expect(page.Locator("#message")).ToContainTextAsync("level, experience points, nature");

        // A level edit sets the experience points to the start of the level, and the note shows the range.
        await page.Locator("#level").FillAsync("50");
        await Expect(page.Locator("#exp")).ToHaveValueAsync(edited.EXP.ToString(CultureInfo.InvariantCulture));
        await Expect(page.Locator("#level-note")).ToHaveTextAsync(EditorText.LevelNote(LevelProgress.Of(edited)));
        await Expect(page.Locator("#inspect-level")).ToHaveTextAsync("50");

        // Experience past the curve is refused as typed, not clamped, and blocks Apply until corrected.
        var maximum = Experience.GetEXP(Experience.MaxLevel, edited.PersonalInfo.EXPGrowth);
        await page.Locator("#exp").FillAsync((maximum + 1).ToString(CultureInfo.InvariantCulture));
        await Expect(page.Locator("#message")).ToHaveTextAsync(UserMessages.For(SessionError.ExperienceOutOfRange));
        await Expect(page.Locator("#exp")).ToHaveValueAsync((maximum + 1).ToString(CultureInfo.InvariantCulture));
        await Expect(page.Locator("#apply")).ToBeDisabledAsync();
        await page.Locator("#exp").FillAsync(edited.EXP.ToString(CultureInfo.InvariantCulture));
        await Expect(page.Locator("#apply")).ToBeEnabledAsync();
        await Expect(page.Locator("#message")).ToBeEmptyAsync();

        await page.Locator("#nature").SelectOptionAsync(((int)nature).ToString(CultureInfo.InvariantCulture));
        await Expect(page.Locator("#nature-note")).ToHaveTextAsync(EditorText.NatureNote(NatureEffect.Of(nature)));
        var effect = NatureEffect.Of(nature);
        await Expect(page.Locator("#inspect-stats-table")).ToContainTextAsync(InspectorText.StatName(effect.Raised, effect));
        await Expect(page.Locator("#inspect-stats-table")).ToContainTextAsync(InspectorText.StatName(effect.Lowered, effect));

        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#message")).ToHaveTextAsync("Changes applied in memory; download to retain changes.");
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(NativeLegality.Of(changed, changed.GetBoxSlotAtIndex(0, 0), StorageSlotType.Box).Verdict);
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native level and nature output differs.");
        AssertOnlyRangeDiffers(bytes, output, native.GetBoxSlotOffset(0, 0), native.SIZE_BOXSLOT);

        await page.SetViewportSizeAsync(375, 800);
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The editor scrolls horizontally at 375 px.");
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task APartyLevelDropIsPreviewedAndKeepsTheStatus(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // The leader is at level 50, part-way to 51, at full HP and burned, so lowering its level lowers its HP, a heal or cleared status
        // would show, and retyping its level a key at a time passes through level 5.
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember("Leader", p =>
        {
            p.EXP = Experience.GetEXP(50, p.PersonalInfo.EXPGrowth) + 100;
            p.ResetPartyStats();
            p.Status_Condition = Burn;
        }));
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = changed.GetPartySlotAtIndex(0);
        var previousHp = edited.Stat_HPCurrent;
        var previousMax = edited.Stat_HPMax;
        edited.EXP = Experience.GetEXP(2, edited.PersonalInfo.EXPGrowth);
        var stats = edited.GetStats(edited.PersonalInfo);
        edited.SetStats(stats);
        edited.Stat_Level = 2;
        edited.Stat_HPCurrent = Math.Min(previousHp, (int)stats[0]);
        changed.SetPartySlotAtIndex(edited, 0, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        await Load(page, bytes);
        await Select(page, SlotRef.InParty(0));
        await Expect(page.Locator("#message")).ToContainTextAsync(PartyText.KeptOnEdit);
        await Expect(page.Locator("#party-hp-preview")).ToHaveCountAsync(0);

        // Retyping the stored level a key at a time passes through lower levels; the draft must come back clean, HP and EXP included.
        var storedLevel = native.GetPartySlotAtIndex(0).CurrentLevel.ToString(CultureInfo.InvariantCulture);
        Assert.True(storedLevel.Length > 1, "The fixture must pass through a lower level while typing.");
        await page.Locator("#level").FillAsync("");
        await page.Locator("#level").PressSequentiallyAsync(storedLevel);
        await Expect(page.Locator("#level")).ToHaveValueAsync(storedLevel);
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");
        await Expect(page.Locator("#party-hp-preview")).ToHaveCountAsync(0);
        // Typing 55 also passes through 5; HP clamped there must not carry over to level 55, whose maximum is higher.
        await page.Locator("#level").FillAsync("");
        await page.Locator("#level").PressSequentiallyAsync("55");
        await Expect(page.Locator("#inspect-level")).ToHaveTextAsync("55");
        await Expect(page.Locator("#party-hp-preview")).ToHaveCountAsync(0);
        var at55 = native.GetPartySlotAtIndex(0);
        at55.EXP = Experience.GetEXP(55, at55.PersonalInfo.EXPGrowth);
        await Expect(page.Locator("#inspect-hp")).ToHaveTextAsync($"{previousHp} of {at55.GetStats(at55.PersonalInfo)[0]}");

        await page.Locator("#level").FillAsync("2");
        await Expect(page.Locator("#party-hp-preview")).ToHaveTextAsync(PartyText.HpPreview(new PartyHpChange(previousHp, edited.Stat_HPCurrent, previousMax, edited.Stat_HPMax))!);
        await Expect(page.Locator("#inspect-stats-table caption")).ToContainTextAsync("recalculated");
        await Expect(page.Locator("#inspect-hp")).ToContainTextAsync(edited.Stat_HPCurrent.ToString(CultureInfo.InvariantCulture));

        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        await Expect(page.Locator("#inspect-stats-table caption")).ToHaveTextAsync("Stats as stored with this party member");
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native party level output differs.");
        var result = SaveFixtures.Parse(output).GetPartySlotAtIndex(0);
        Assert.Equal(Burn, result.Status_Condition);
        Assert.Equal(edited.Stat_HPCurrent, result.Stat_HPCurrent);
        Assert.True(result.Stat_HPCurrent < previousHp, "The fixture must lower HP.");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}
