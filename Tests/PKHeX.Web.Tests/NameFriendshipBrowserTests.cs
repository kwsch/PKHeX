using System.Globalization;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Name, language and friendship in the published app (WEB-PKM-003, WEB-PKM-006): the desktop's name rules, a language change shown with
/// its default name, labelled friendship refused rather than clamped, eggs left alone, and exports byte-identical to native Core.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class NameFriendshipBrowserTests(PublishedAppFixture app)
{
    private const int German = (int)LanguageID.German;

    private static string ZigzagoonIn(int language) => SpeciesName.GetSpeciesNameGeneration((ushort)Species.Zigzagoon, language, 6);

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ALanguageChangeRenamesAPokemonThatIsNotNicknamedAndMatchesNativeCore(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // Box 1, slot 2 holds a Zigzagoon that is not nicknamed but has a custom name, so a language change gives it its German default.
        var slot = SlotRef.InBox(0, 1);
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            p.Nickname = "Quill";
            p.IsNicknamed = false;
        }));
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = changed.GetBoxSlotAtIndex(0, 1);
        edited.Language = German;
        edited.Nickname = ZigzagoonIn(German);
        edited.OriginalTrainerFriendship = 200;
        changed.SetBoxSlotAtIndex(edited, 0, 1, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        await Load(page, bytes);
        await Select(page, slot);
        await Expect(page.Locator("#message")).ToContainTextAsync(EditorText.EditableSummary(SaveFixtures.Open(bytes).Capabilities.Editable));

        await page.Locator("#language").SelectOptionAsync(German.ToString(CultureInfo.InvariantCulture));
        await Expect(page.Locator("#nickname")).ToHaveValueAsync(ZigzagoonIn(German));
        await Expect(page.Locator("#nicknamed")).Not.ToBeCheckedAsync();
        await Expect(page.Locator("#name-change")).ToContainTextAsync($"its name changed from {TestText.Isolated("Quill")} to its");
        await Expect(page.Locator("#name-change")).ToContainTextAsync(ZigzagoonIn(German));

        // Out-of-range friendship is refused as typed, not clamped, and blocks Apply until corrected.
        await page.Locator("#ot-friendship").FillAsync("256");
        await Expect(page.Locator("#friendship-fields-error")).ToHaveTextAsync(UserMessages.For(SessionError.FriendshipOutOfRange));
        await Expect(page.Locator("#ot-friendship")).ToHaveValueAsync("256");
        await Expect(page.Locator("#apply")).ToBeDisabledAsync();
        // Partly typed text is refused and shown as typed; a number field would report it as empty and the field would be wiped.
        await page.Locator("#ot-friendship").FillAsync("");
        await page.Locator("#ot-friendship").PressSequentiallyAsync("1e");
        await Expect(page.Locator("#ot-friendship")).ToHaveValueAsync("1e");
        await Expect(page.Locator("#friendship-fields-error")).ToHaveTextAsync(UserMessages.For(SessionError.FriendshipOutOfRange));
        await page.Locator("#ot-friendship").FillAsync("200");
        await ReadyToApply(page);
        await Expect(page.Locator("#friendship-fields-error")).ToHaveCountAsync(0);
        await Expect(page.Locator("#inspect-ot-friendship")).ToContainTextAsync("200");

        await Apply(page);
        await Expect(page.Locator("#message")).ToHaveTextAsync("Changes applied in memory; download to retain changes.");
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(NativeLegality.Of(changed, changed.GetBoxSlotAtIndex(0, 1), StorageSlotType.Box).Verdict);
        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native language and friendship output differs.");
        AssertOnlyRangeDiffers(bytes, output, native.GetBoxSlotOffset(0, 1), native.SIZE_BOXSLOT);

        await page.SetViewportSizeAsync(375, 800);
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The editor scrolls horizontally at 375 px.");
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task TheNameFollowsTheFlagAsOnTheDesktop(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, SaveFixtures.Synthetic(true));
        await Select(page);
        var english = ZigzagoonIn((int)LanguageID.English);

        // A custom name ticks the flag; clearing the flag gives back the default name, so the draft is clean again.
        await page.Locator("#nickname").FillAsync("Quill");
        await Expect(page.Locator("#nicknamed")).ToBeCheckedAsync();
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("Unapplied draft");
        await page.Locator("#nicknamed").UncheckAsync();
        await Expect(page.Locator("#nickname")).ToHaveValueAsync(english);
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");

        // A language change keeps a species name and says the default differs.
        await page.Locator("#language").SelectOptionAsync(((int)LanguageID.French).ToString(CultureInfo.InvariantCulture));
        await Expect(page.Locator("#nickname")).ToHaveValueAsync(english);
        await Expect(page.Locator("#name-note")).ToContainTextAsync("its name was kept because it is the species' name in another language");
        var french = SaveFixtures.Open(SaveFixtures.Synthetic(true)).Capabilities.Lists.Languages.Single(l => l.Value == (int)LanguageID.French).Text;
        await Expect(page.Locator("#inspect-language")).ToContainTextAsync(french);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task PartyFriendshipIsLabelledPerTrainerAndKeepsBattleState(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // The leader has a handling trainer who holds it now, and is injured and burned, so a heal or a cleared status would show.
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember("Leader", p =>
        {
            p.HandlingTrainerName = "Holder";
            p.CurrentHandler = 1;
            p.HandlingTrainerFriendship = 50;
            p.Stat_HPCurrent = 7;
            p.Status_Condition = 0x10;
        }));
        var native = SaveFixtures.Parse(bytes);
        var changed = native.Clone();
        var edited = changed.GetPartySlotAtIndex(0);
        edited.OriginalTrainerFriendship = 0;
        edited.HandlingTrainerFriendship = 255;
        changed.SetPartySlotAtIndex(edited, 0, EntityImportSettings.None);
        var expected = changed.Write().ToArray();
        await Load(page, bytes);
        await Select(page, SlotRef.InParty(0));

        await Expect(page.Locator("label[for=ot-friendship]")).ToHaveTextAsync(EditorText.TrainerFriendshipLabel(edited.OriginalTrainerName));
        await Expect(page.Locator("label[for=ht-friendship]")).ToHaveTextAsync(EditorText.HandlerFriendshipLabel("Holder"));
        await Expect(page.Locator("#friendship-note")).ToHaveTextAsync(EditorText.CurrentFriendship(true));
        await page.Locator("#ot-friendship").FillAsync("0");
        await page.Locator("#ht-friendship").FillAsync("255");
        await Expect(page.Locator("#party-hp-preview")).ToHaveCountAsync(0);
        await Apply(page);
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");

        var output = await DownloadEdited(page);
        Assert.True(output.AsSpan().SequenceEqual(expected), "Browser/native friendship output differs.");
        var result = SaveFixtures.Parse(output).GetPartySlotAtIndex(0);
        Assert.Equal(7, result.Stat_HPCurrent);
        Assert.Equal(0x10, result.Status_Condition);
        Assert.Equal(1, result.CurrentHandler);
        AssertOnlyRangeDiffers(bytes, output, native.GetPartyOffset(0), native.SIZE_PARTY);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task AnEggAndAPokemonWithoutAHandlerOfferOnlyWhatCanBeChanged(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            p.IsEgg = true;
            p.IsNicknamed = true;
            p.Nickname = SpeciesName.GetEggName(p.Language, 6);
        })));

        // The Zigzagoon has never left its original trainer: only the handling trainer's friendship is read-only.
        await Select(page);
        await Expect(page.Locator("#ht-friendship")).Not.ToBeEditableAsync();
        await Expect(page.Locator("#ht-note")).ToHaveTextAsync(EditorText.NoHandler);
        await Expect(page.Locator("#ot-friendship")).ToBeEditableAsync();

        await Select(page, SlotRef.InBox(0, 1));
        await Expect(page.Locator("#message")).ToContainTextAsync(UserMessages.For(SessionError.EggNotEditable));
        await Expect(page.Locator("#egg-note")).ToHaveTextAsync(EditorText.EggReadOnly);
        await Expect(page.Locator("#nickname")).Not.ToBeEditableAsync();
        await Expect(page.Locator("#nicknamed")).ToBeDisabledAsync();
        await Expect(page.Locator("#language")).ToBeDisabledAsync();
        await Expect(page.Locator("#ot-friendship")).Not.ToBeEditableAsync();
        await Expect(page.Locator("#apply")).ToBeDisabledAsync();

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}
