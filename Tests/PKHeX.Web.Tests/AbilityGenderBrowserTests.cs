using System.Globalization;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The ability slot and the gender in the published app: Core's slots offered with (1), (2) and (H), the species'
/// genders only, Meowstic's form changed with its gender and the change stated, a party member's battle state kept, and exports
/// byte-identical to native Core.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class AbilityGenderBrowserTests(PublishedAppFixture app)
{
    private const int Burn = 0x10;
    private static readonly SlotRef Boxed = SlotRef.InBox(0, 1);

    private static string Id(int value) => value.ToString(CultureInfo.InvariantCulture);

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ABoxedAbilityAndGenderEditMatchesNativeCore(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        var bytes = AbilityGenderDraftTests.Boxed(change: p => p.RefreshAbility(0));
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = (PK6)changed.GetBoxSlotAtIndex(0, 1);
        var other = (byte)(1 - edited.Gender);
        edited.SetAbilityIndex(2);
        edited.Gender = other;
        changed.SetBoxSlotAtIndex(edited, 0, 1, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        await Load(page, bytes);
        await Select(page, Boxed);
        await Expect(page.Locator("#message")).ToContainTextAsync("ability and gender can be changed");
        var names = GameInfo.Strings.abilitylist;
        var pi = edited.PersonalInfo;
        await Expect(page.Locator("#ability option")).ToHaveTextAsync([$"{names[pi.Ability1]} (1)", $"{names[pi.Ability2]} (2)", $"{names[pi.AbilityH]} (H)"]);
        await Expect(page.Locator("#gender option")).ToHaveTextAsync(["Male", "Female"]);

        await page.Locator("#ability").SelectOptionAsync("2");
        await Expect(page.Locator("#inspect-ability")).ToContainTextAsync($"{names[pi.AbilityH]} (hidden ability)");
        await page.Locator("#gender").SelectOptionAsync(Id(other));
        await Expect(page.Locator("#inspect-gender")).ToContainTextAsync(EditorText.GenderName(other));
        await Expect(page.Locator("#gender-change")).ToBeEmptyAsync();

        await Apply(page);
        await Expect(page.Locator("#message")).ToHaveTextAsync("Changes applied in memory; download to retain changes.");
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(NativeLegality.Of(changed, changed.GetBoxSlotAtIndex(0, 1), StorageSlotType.Box).Verdict);
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native ability and gender output differs.");
        AssertOnlyRangeDiffers(bytes, output, native.GetBoxSlotOffset(0, 1), native.SIZE_BOXSLOT);

        await page.SetViewportSizeAsync(375, 800);
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The editor scrolls horizontally at 375 px.");
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task AMeowsticPartyMemberChangesFormWithItsGenderAndKeepsItsBattleState(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // The leader is a male Meowstic with Prankster, 5 experience points into level 30, injured and burned, with a stored Attack Core
        // would not calculate: a heal, a cleared status or a recalculation would show.
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember("Leader", p =>
        {
            AbilityGenderDraftTests.MaleMeowstic(p);
            p.ResetPartyStats();
            p.Stat_HPCurrent = 7;
            p.Status_Condition = Burn;
            p.Stat_ATK = 1;
        }));
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = changed.GetPartySlotAtIndex(0);
        var storedExp = edited.EXP;
        edited.ChangeSpeciesForm(AbilityGenderDraftTests.Meowstic, 1, changed.Personal, 2);
        edited.Gender = EntityGender.Female;
        changed.SetPartySlotAtIndex(edited, 0, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        var competitive = GameInfo.Strings.abilitylist[(int)Ability.Competitive];
        await Load(page, bytes);
        await Select(page, SlotRef.InParty(0));
        await Expect(page.Locator("#message")).ToContainTextAsync(PartyText.KeptOnEdit);
        await Expect(page.Locator("#gender-note")).ToContainTextAsync("form is its gender");

        await page.Locator("#gender").SelectOptionAsync("1");
        await Expect(page.Locator("#gender-change")).ToHaveTextAsync(
            $"Its form changed with its gender. Its ability is now {competitive} (hidden ability). Its experience points are now {edited.EXP} (level 30), from {storedExp}.");
        await Expect(page.Locator("#ability option").Last).ToHaveTextAsync($"{competitive} (H)");
        await Expect(page.Locator("#inspect-ability")).ToContainTextAsync(competitive);
        await Expect(page.Locator("#exp")).ToHaveValueAsync(Id((int)edited.EXP));
        await Expect(page.Locator("#inspect-hp")).ToContainTextAsync("7 of ");
        await Expect(page.Locator("#party-hp-preview")).ToHaveCountAsync(0);
        await Expect(page.Locator("#inspect-stats-table caption")).Not.ToContainTextAsync("recalculated");

        await Apply(page);
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native Meowstic output differs.");
        var result = SaveFixtures.Parse(output).GetPartySlotAtIndex(0);
        Assert.Equal((Burn, 7, 1, (byte)1, EntityGender.Female), (result.Status_Condition, result.Stat_HPCurrent, result.Stat_ATK, result.Form, result.Gender));

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}
