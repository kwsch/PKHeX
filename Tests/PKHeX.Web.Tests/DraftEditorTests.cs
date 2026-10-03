using System.Globalization;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The draft editor (WEB-PKM-003–007, WEB-PKM-009–013): labelled name, language, friendship, level, experience, nature, ability, gender,
/// IV, EV, held item, move, PP and PP Ups fields that turn input into typed draft edits, show what the draft holds after an
/// accepted edit, keep refused input as typed, and offer nothing for an egg.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class DraftEditorTests : IDisposable
{
    private const int French = (int)LanguageID.French, German = (int)LanguageID.German;

    private readonly BunitContext context = new();

    /// <summary>The refusal of the last edit, or null when it was accepted.</summary>
    private SessionError? refused;

    public void Dispose() => context.Dispose();

    private IRenderedComponent<DraftEditor> Render(EditorDraft draft) => context.Render<DraftEditor>(p => p
        .Add(c => c.Draft, draft)
        .Add(c => c.OnEdit, EventCallback.Factory.Create<Action<EditorDraft>>(this, edit =>
        {
            refused = null;
            try
            {
                edit(draft);
            }
            catch (SessionException ex)
            {
                refused = ex.Error;
            }
        })));

    private static EditorDraft Open(byte[] bytes, SlotRef? slot = null) => SaveFixtures.Open(bytes).Select(slot ?? SaveFixtures.FirstBoxSlot);

    private static string ZigzagoonIn(int language) => SpeciesName.GetSpeciesNameGeneration((ushort)Species.Zigzagoon, language, 6);

    /// <summary>Core's display name of <paramref name="language"/>, as the language box lists it.</summary>
    private static string LanguageName(EditorDraft draft, int language) => draft.Capabilities.Lists.Languages.Single(l => l.Value == language).Text;

    [Fact]
    public void FriendshipLabelsNameEachTrainerAndTheCurrentOne()
    {
        // The Ditto has a handling trainer, who holds it now.
        var draft = Open(SaveFixtures.Synthetic(true, legal: false));
        var editor = Render(draft);

        editor.Find("label[for=ot-friendship]").TextContent.Should().Be(EditorText.TrainerFriendshipLabel(draft.TrainerName));
        editor.Find("label[for=ot-friendship]").TextContent.Should().Contain(draft.TrainerName);
        editor.Find("label[for=ht-friendship]").TextContent.Should().Contain(draft.HandlerName);
        editor.Find("#friendship-note").TextContent.Should().StartWith("The game currently uses the handling trainer's value.");
        editor.Find("#ot-friendship").GetAttribute("value").Should().Be(draft.TrainerFriendship.ToString(CultureInfo.InvariantCulture));
        editor.Find("#ht-friendship").HasAttribute("readonly").Should().BeFalse();
        editor.FindAll("#ht-note").Should().BeEmpty();
    }

    [Fact]
    public void HandlerFriendshipIsReadOnlyWithoutAHandlingTrainer()
    {
        var editor = Render(Open(SaveFixtures.Synthetic(true)));

        editor.Find("#ht-friendship").HasAttribute("readonly").Should().BeTrue();
        editor.Find("#ht-friendship").GetAttribute("aria-describedby").Should().Be("ht-note");
        editor.Find("#ht-note").TextContent.Should().Be(EditorText.NoHandler);
        editor.Find("label[for=ht-friendship]").TextContent.Should().Be("Friendship with handling trainer (none stored)");
        editor.Find("#friendship-note").TextContent.Should().StartWith("The game currently uses the original trainer's value.");
        editor.Find("#ot-friendship").HasAttribute("readonly").Should().BeFalse();
    }

    [Fact]
    public void AnEggOffersNothing()
    {
        var draft = Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            p.IsEgg = true;
            p.IsNicknamed = true;
            p.Nickname = SpeciesName.GetEggName(p.Language, 6);
        })), SlotRef.InBox(0, 1));
        var editor = Render(draft);

        editor.Find("#egg-note").TextContent.Should().Be(EditorText.EggReadOnly);
        editor.Find("#nickname").HasAttribute("readonly").Should().BeTrue();
        editor.Find("#nicknamed").HasAttribute("disabled").Should().BeTrue();
        editor.Find("#language").HasAttribute("disabled").Should().BeTrue();
        editor.Find("#ot-friendship").HasAttribute("readonly").Should().BeTrue();
        editor.Find("#ht-friendship").HasAttribute("readonly").Should().BeTrue();
        editor.Find("#level").HasAttribute("readonly").Should().BeTrue();
        editor.Find("#exp").HasAttribute("readonly").Should().BeTrue();
        editor.Find("#nature").HasAttribute("disabled").Should().BeTrue();
        for (var stat = 0; stat < EditorDraft.StatCount; stat++)
        {
            editor.Find($"#iv-{stat}").HasAttribute("readonly").Should().BeTrue();
            editor.Find($"#ev-{stat}").HasAttribute("readonly").Should().BeTrue();
        }
        editor.Find("#held-item").HasAttribute("disabled").Should().BeTrue();
        for (var slot = 0; slot < EditorDraft.MoveCount; slot++)
        {
            editor.Find($"#move-{slot}").HasAttribute("disabled").Should().BeTrue();
            editor.Find($"#pp-{slot}").HasAttribute("readonly").Should().BeTrue();
            editor.Find($"#ppups-{slot}").HasAttribute("disabled").Should().BeTrue();
        }
        editor.Find("#ability").HasAttribute("disabled").Should().BeTrue();
        editor.Find("#gender").HasAttribute("disabled").Should().BeTrue();
        editor.Find("#species").HasAttribute("disabled").Should().BeTrue();
        editor.Find("#form").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ALevelEditShowsTheExperienceItSet()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);

        editor.Find("#level").Input("50");

        refused.Should().BeNull();
        editor.Find("#exp").GetAttribute("value").Should().Be("125000");
        editor.Find("#level-note").TextContent.Should().Be(
            "Level 50 starts at 125000 experience points; 7651 more reach level 51 at 132651. Changing the level sets the experience points to the start of the new level.");
        editor.Find("label[for=exp]").TextContent.Should().Be("Experience points (0–1000000)");
        editor.Find("#level-note").GetAttribute("role").Should().Be("status");
    }

    [Fact]
    public void AnExperienceEditShowsTheLevelItReached()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);

        editor.Find("#exp").Input("1000000");

        editor.Find("#level").GetAttribute("value").Should().Be("100");
        editor.Find("#level-note").TextContent.Should().StartWith("Level 100 starts at 1000000 experience points and is the highest level.");
    }

    [Theory]
    [InlineData("#level", "101", SessionError.LevelOutOfRange)]
    [InlineData("#level", "0", SessionError.LevelOutOfRange)]
    [InlineData("#level", "", SessionError.LevelOutOfRange)]
    [InlineData("#level", "99999999999999999999", SessionError.LevelOutOfRange)]
    [InlineData("#exp", "1000001", SessionError.ExperienceOutOfRange)]
    [InlineData("#exp", "1e6", SessionError.ExperienceOutOfRange)]
    [InlineData("#exp", "-1", SessionError.ExperienceOutOfRange)]
    public void RefusedLevelOrExperienceIsKeptAsTyped(string field, string typed, SessionError error)
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);
        var level = draft.Level;

        editor.Find(field).Input(typed);

        refused.Should().Be(error);
        draft.Level.Should().Be(level);
        draft.IsDirty.Should().BeFalse();
        editor.Find(field).GetAttribute("value").Should().Be(typed);
    }

    [Fact]
    public void ALevelTypedWithALeadingZeroIsKept()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);

        editor.Find("#level").Input("050");

        draft.Level.Should().Be(50);
        editor.Find("#level").GetAttribute("value").Should().Be("050", "text that reads as the drafted value is not rewritten under the cursor");
    }

    [Fact]
    public void TheNatureBoxListsTheGamesNaturesAndNotesTheirEffect()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);

        editor.FindAll("#nature option").Select(o => o.TextContent).Should().Equal(draft.Capabilities.Lists.Natures.Select(n => n.Text));
        editor.Find("#nature").Change(((int)Nature.Adamant).ToString(CultureInfo.InvariantCulture));

        draft.Nature.Should().Be(Nature.Adamant);
        editor.Find("#nature-note").TextContent.Should().Be(
            "This nature raises Attack and lowers Sp. Atk. In this game the nature is stored apart from the PID, so changing it does not change shininess, gender or ability.");
        editor.Find("#nature").Change(((int)Nature.Hardy).ToString(CultureInfo.InvariantCulture));
        editor.Find("#nature-note").TextContent.Should().StartWith("This nature does not raise or lower any stat.");
    }

    [Fact]
    public void TheNatureBoxShowsAnUnlistedStoredValue()
    {
        var draft = Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p => p.Nature = (Nature)30)), SlotRef.InBox(0, 1));
        var editor = Render(draft);

        var options = editor.FindAll("#nature option");
        options.Should().HaveCount(draft.Capabilities.Lists.Natures.Count + 1);
        options[0].TextContent.Should().Be(EditorText.UnlistedNature(30));
        options[0].HasAttribute("selected").Should().BeTrue();
        options[0].HasAttribute("disabled").Should().BeTrue();
        editor.Find("#nature-note").TextContent.Should().StartWith("This nature does not raise or lower any stat.");
    }

    [Fact]
    public void LevelAndExperienceAreWrittenWithInvariantDigits()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-SA");
            var draft = Open(SaveFixtures.Synthetic(true));
            var editor = Render(draft);

            editor.Find("#level").Input("50");

            editor.Find("#exp").GetAttribute("value").Should().Be("125000");
            editor.Find("#level-note").TextContent.Should().Contain("125000");
            editor.Find("label[for=exp]").TextContent.Should().Be("Experience points (0–1000000)");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void TypingACustomNameTicksTheFlag()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);

        editor.Find("#nickname").Input("Quill");

        refused.Should().BeNull();
        draft.Nickname.Should().Be("Quill");
        editor.Find("#nicknamed").HasAttribute("checked").Should().BeTrue();
        editor.Find("#name-note").TextContent.Should().Be($"Default name in {LanguageName(draft, (int)LanguageID.English)}: {ZigzagoonIn((int)LanguageID.English)}.");
    }

    [Fact]
    public void ClearingTheFlagShowsTheDefaultNameItGave()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);
        editor.Find("#nickname").Input("Quill");

        editor.Find("#nicknamed").Change(false);

        editor.Find("#nickname").GetAttribute("value").Should().Be(ZigzagoonIn((int)LanguageID.English));
        editor.Find("#name-note").TextContent.Should().Be($"Not nicknamed, so its name changed from {TestText.Isolated("Quill")} to its {LanguageName(draft, (int)LanguageID.English)} default, {ZigzagoonIn((int)LanguageID.English)}.");
    }

    [Fact]
    public void ALanguageChangeExplainsAKeptName()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);

        editor.Find("#language").Change(French.ToString(CultureInfo.InvariantCulture));

        draft.Language.Should().Be(French);
        editor.Find("#nickname").GetAttribute("value").Should().Be(ZigzagoonIn((int)LanguageID.English));
        editor.Find("#name-note").TextContent.Should().Be($"Not nicknamed: its name was kept because it is the species' name in another language. The {LanguageName(draft, French)} default is {ZigzagoonIn(French)}; legality analysis may report the difference.");
    }

    [Fact]
    public void ALanguageChangeShowsTheNewDefaultName()
    {
        var draft = Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            p.Nickname = "Quill";
            p.IsNicknamed = false;
        })), SlotRef.InBox(0, 1));
        var editor = Render(draft);
        editor.Find("#name-note").TextContent.Should().Be($"Not nicknamed, but the name differs from the {LanguageName(draft, (int)LanguageID.English)} default, {ZigzagoonIn((int)LanguageID.English)}.");

        editor.Find("#language").Change(German.ToString(CultureInfo.InvariantCulture));

        editor.Find("#nickname").GetAttribute("value").Should().Be(ZigzagoonIn(German));
        editor.Find("#name-note").TextContent.Should().Be($"Not nicknamed, so its name changed from {TestText.Isolated("Quill")} to its {LanguageName(draft, German)} default, {ZigzagoonIn(German)}.");
    }

    [Fact]
    public void AStoredLanguageWithoutADefaultNameSaysTheNameIsKept()
    {
        var draft = Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p => p.Language = 6)), SlotRef.InBox(0, 1));
        var editor = Render(draft);

        editor.Find("#name-note").TextContent.Should().Be(EditorText.NoDefaultName);
    }

    [Fact]
    public void FriendshipFieldsAreTextSoPartialInputIsKept()
    {
        var editor = Render(Open(SaveFixtures.Synthetic(true)));

        editor.Find("#ot-friendship").GetAttribute("type").Should().Be("text");
        editor.Find("#ht-friendship").GetAttribute("type").Should().Be("text");
        editor.Find("#ot-friendship").GetAttribute("inputmode").Should().Be("numeric");
    }

    [Fact]
    public void TheFontWarningShowsWhatTheGameWouldDisplay()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);

        editor.Find("#nickname").Input("Ab😀");

        editor.Find("#name-note").TextContent.Should().EndWith($"The game's font cannot show some of these characters; it would show the name as {TestText.Isolated(draft.Name.Displayed)}.");
    }

    [Theory]
    [InlineData("256")]
    [InlineData("-1")]
    [InlineData("")]
    [InlineData("1.5")]
    [InlineData("1e2")]
    public void RefusedFriendshipIsKeptAsTypedAndNotClamped(string text)
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var stored = draft.TrainerFriendship;
        var editor = Render(draft);

        editor.Find("#ot-friendship").Input(text);

        refused.Should().Be(SessionError.FriendshipOutOfRange);
        draft.TrainerFriendship.Should().Be(stored);
        draft.IsDirty.Should().BeFalse();
        editor.Find("#ot-friendship").GetAttribute("value").Should().Be(text);
    }

    [Fact]
    public void AnAcceptedEditReplacesEarlierRefusedInput()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);
        editor.Find("#ot-friendship").Input("300");

        editor.Find("#nickname").Input("Quill");

        editor.Find("#ot-friendship").GetAttribute("value").Should().Be(draft.TrainerFriendship.ToString(CultureInfo.InvariantCulture), "the field shows what the draft holds");
    }

    [Fact]
    public void AcceptedFriendshipKeepsTheTypedText()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);

        editor.Find("#ot-friendship").Input("071");

        draft.TrainerFriendship.Should().Be(71);
        editor.Find("#ot-friendship").GetAttribute("value").Should().Be("071", "the field is not rewritten under the cursor");
    }

    [Fact]
    public void FriendshipIsWrittenWithInvariantDigits()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-SA");
            var draft = Open(SaveFixtures.Synthetic(true));
            var editor = Render(draft);

            editor.Find("#ot-friendship").Input("123");

            draft.TrainerFriendship.Should().Be(123);
            editor.Find("#ot-friendship").GetAttribute("value").Should().Be("123");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void TheLanguageBoxListsTheGamesLanguagesAndAnUnlistedStoredValue()
    {
        var draft = Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p => p.Language = 6)), SlotRef.InBox(0, 1));
        var editor = Render(draft);

        var options = editor.FindAll("#language option");
        options.Should().HaveCount(draft.Capabilities.Lists.Languages.Count + 1);
        options[0].TextContent.Should().Be(EditorText.UnlistedLanguage(6));
        options[0].HasAttribute("disabled").Should().BeTrue();
        options[0].HasAttribute("selected").Should().BeTrue();
        options.Skip(1).Select(o => o.TextContent).Should().Equal(draft.Capabilities.Lists.Languages.Select(l => l.Text));
    }

    [Fact]
    public void TheNameNoteIsAlwaysALiveRegion()
    {
        var editor = Render(Open(SaveFixtures.Synthetic(true)));

        editor.Find("#name-note").GetAttribute("role").Should().Be("status");
        editor.Find("#name-note").TextContent.Should().BeEmpty("the stored default name needs no note");
    }

    [Fact]
    public void ANewDraftReloadsEveryField()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);
        editor.Find("#ot-friendship").Input("300");
        editor.Find("#iv-2").Input("99");
        editor.Find("#ev-2").Input("999");
        var other = Open(SaveFixtures.Synthetic(true, legal: false));

        editor.Render(p => p.Add(c => c.Draft, other));

        editor.Find("#nickname").GetAttribute("value").Should().Be(other.Nickname);
        editor.Find("#ot-friendship").GetAttribute("value").Should().Be(other.TrainerFriendship.ToString(CultureInfo.InvariantCulture));
        editor.Find("#iv-2").GetAttribute("value").Should().Be(other.Ivs[2].ToString(CultureInfo.InvariantCulture));
        editor.Find("#ev-2").GetAttribute("value").Should().Be(other.Evs[2].ToString(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EditableSummaryListsTheFields()
    {
        EditorText.EditableSummary(EditableFields.Nickname | EditableFields.Language | EditableFields.Friendship)
            .Should().Be("Its nickname, language and friendship can be changed.");
        EditorText.EditableSummary(EditableFields.Nickname | EditableFields.Friendship).Should().Be("Its nickname and friendship can be changed.");
        EditorText.EditableSummary(EditableFields.Language).Should().Be("Its language can be changed.");
        EditorText.EditableSummary(EditableFields.None).Should().Be("No fields can be changed.");
        EditorText.EditableSummary(EditableFields.Nickname | EditableFields.Language | EditableFields.Friendship | EditableFields.Level | EditableFields.Nature)
            .Should().Be("Its nickname, language, friendship, level, experience points and nature can be changed.");
        EditorText.EditableSummary(EditableFields.Nature | EditableFields.Ivs | EditableFields.Evs).Should().Be("Its nature, IVs and EVs can be changed.");
        EditorText.EditableSummary(EditableFields.Evs | EditableFields.HeldItem | EditableFields.Moves | EditableFields.Pp)
            .Should().Be("Its EVs, held item, moves, PP and PP Ups can be changed.");
        EditorText.EditableSummary(EditableFields.Pp | EditableFields.Ability | EditableFields.Gender)
            .Should().Be("Its PP, PP Ups, ability and gender can be changed.");
        EditorText.EditableSummary(EditableFields.Species | EditableFields.Nickname).Should().Be("Its species, form and nickname can be changed.");
    }

    /// <summary>A box 1, slot 2 Pokémon with the given EVs, in the summary order the fields use (HP, Attack, Defense, Sp. Atk, Sp. Def, Speed).</summary>
    private static EditorDraft WithEvs(params int[] evs) => Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1,
        p => p.SetEVs([evs[0], evs[1], evs[2], evs[5], evs[3], evs[4]]))), SlotRef.InBox(0, 1));

    [Fact]
    public void EachIvAndEvFieldIsNamedByItsStatAndColumn()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        draft.EditNature((int)Nature.Adamant);
        var editor = Render(draft);

        editor.Find("#iv-head").TextContent.Should().Be("IV (0–31)");
        editor.Find("#ev-head").TextContent.Should().Be("EV (0–252)");
        editor.Find("#stat-1").TextContent.Should().Be("Attack (raised by nature)");
        editor.Find("#stat-3").TextContent.Should().Be("Sp. Atk (lowered by nature)");
        for (var stat = 0; stat < EditorDraft.StatCount; stat++)
        {
            var iv = editor.Find($"#iv-{stat}");
            iv.GetAttribute("aria-labelledby").Should().Be($"stat-{stat} iv-head");
            iv.GetAttribute("inputmode").Should().Be("numeric");
            iv.GetAttribute("value").Should().Be(draft.Ivs[stat].ToString(CultureInfo.InvariantCulture));
            iv.HasAttribute("readonly").Should().BeFalse();
            var ev = editor.Find($"#ev-{stat}");
            ev.GetAttribute("aria-labelledby").Should().Be($"stat-{stat} ev-head");
            ev.GetAttribute("value").Should().Be(draft.Evs[stat].ToString(CultureInfo.InvariantCulture));
            ev.HasAttribute("readonly").Should().BeFalse();
        }
    }

    [Fact]
    public void IvAndEvFieldsFollowTheirOwnFlag()
    {
        var bytes = SaveFixtures.Synthetic(true);
        var save = SaveFixtures.Parse(bytes);
        var family = PKHeX.Web.Services.SupportMatrix.Find(save)! with { Editable = EditableFields.Ivs };
        var editor = Render(new SaveSession(bytes.ToArray(), save, "fixture.sav", SaveCapabilities.For(save, family)).Select(SaveFixtures.FirstBoxSlot));

        for (var stat = 0; stat < EditorDraft.StatCount; stat++)
        {
            editor.Find($"#iv-{stat}").HasAttribute("readonly").Should().BeFalse();
            editor.Find($"#ev-{stat}").HasAttribute("readonly").Should().BeTrue();
        }
    }

    [Fact]
    public void AnIvEditUpdatesTheNoteAndAnEvEditTheTotal()
    {
        var draft = WithEvs(0, 0, 0, 0, 0, 0);
        var editor = Render(draft);

        editor.Find("#iv-5").Input("31");
        editor.Find("#ev-5").Input("252");

        refused.Should().BeNull();
        (draft.Ivs[5], draft.Evs[5]).Should().Be((31, 252));
        editor.Find("#iv-note").TextContent.Should().Be(EditorText.IvNote(draft.IvTotal, 31, GameInfo.Strings.HiddenPowerTypes[draft.HiddenPowerType]));
        editor.Find("#ev-note").TextContent.Should().Be("EV total 252 of 510; 258 remaining.");
        editor.Find("#iv-note").GetAttribute("role").Should().Be("status");
        editor.Find("#ev-note").GetAttribute("role").Should().Be("status");
    }

    [Theory]
    [InlineData("#iv-0", "32", SessionError.IvOutOfRange)]
    [InlineData("#iv-0", "-1", SessionError.IvOutOfRange)]
    [InlineData("#iv-0", "", SessionError.IvOutOfRange)]
    [InlineData("#iv-2", "99999999999999999999", SessionError.IvOutOfRange)]
    [InlineData("#ev-0", "253", SessionError.EvOutOfRange)]
    [InlineData("#ev-4", "1e2", SessionError.EvOutOfRange)]
    [InlineData("#ev-4", "", SessionError.EvOutOfRange)]
    [InlineData("#ev-3", "7", SessionError.EvTotalAboveLimit)]
    public void RefusedIvsAndEvsAreKeptAsTyped(string field, string typed, SessionError error)
    {
        var draft = WithEvs(252, 252, 0, 0, 0, 0); // EV total 504, so 7 more is over 510
        var editor = Render(draft);

        editor.Find(field).Input(typed);

        refused.Should().Be(error);
        draft.IsDirty.Should().BeFalse();
        editor.Find(field).GetAttribute("value").Should().Be(typed);
    }

    [Fact]
    public void AnIvTypedWithALeadingZeroIsKept()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);

        editor.Find("#iv-1").Input("031");

        draft.Ivs[1].Should().Be(31);
        editor.Find("#iv-1").GetAttribute("value").Should().Be("031", "text that reads as the drafted value is not rewritten under the cursor");
    }

    [Fact]
    public void TheEvNoteFollowsCoresGrading()
    {
        var draft = WithEvs(252, 252, 0, 0, 0, 0);
        var editor = Render(draft);

        editor.Find("#ev-2").Input("4");
        editor.Find("#ev-note").TextContent.Should().StartWith("EV total 508 of 510; 2 remaining. Every EV that changes a stat is used");
        editor.Find("#ev-2").Input("6");
        editor.Find("#ev-note").TextContent.Should().Be("EV total 510 of 510; 0 remaining. This is the most a Pokémon can have.");
    }

    [Fact]
    public void TheNotesNameAnUnknownHiddenPowerTypeAndEachGrade()
    {
        EditorText.IvNote(186, 31, null).Should().Be("IV total 186 of 186. Hidden Power type: unknown. The IVs decide the Hidden Power type and the characteristic.");
        EditorText.IvNote(0, 31, "Fighting").Should().StartWith("IV total 0 of 186. Hidden Power type: Fighting.");
        EditorText.EvNote(0, 510).Should().Be("EV total 0 of 510; 510 remaining.");
        EditorText.EvNote(509, 510).Should().Be("EV total 509 of 510; 1 remaining.");
        EditorText.EvNote(511, 510).Should().StartWith("EV total 511 of 510: 1 over the limit. Lower an EV");
    }

    [Fact]
    public void AStoredTotalOverTheLimitIsReported()
    {
        var editor = Render(WithEvs(252, 252, 252, 0, 0, 0));

        editor.Find("#ev-note").TextContent.Should().Be(
            "EV total 756 of 510: 246 over the limit. Lower an EV to bring the total within the limit; an EV cannot be raised until it is.");
    }

    [Fact]
    public void IvsAndEvsAreWrittenWithInvariantDigits()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-SA");
            var draft = WithEvs(0, 0, 0, 0, 0, 0);
            var editor = Render(draft);

            editor.Find("#ev-0").Input("100");
            editor.Find("#iv-0").Input("0");

            editor.Find("#ev-0").GetAttribute("value").Should().Be("100");
            editor.Find("#ev-note").TextContent.Should().Be("EV total 100 of 510; 410 remaining.");
            editor.Find("#iv-note").TextContent.Should().Contain($"IV total {draft.IvTotal} of 186.");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void AnAcceptedEditReplacesRefusedIvAndEvInput()
    {
        var draft = WithEvs(0, 0, 0, 0, 0, 0);
        var editor = Render(draft);

        editor.Find("#iv-0").Input("40");
        editor.Find("#ev-0").Input("300");
        editor.Find("#ev-1").Input("8");

        refused.Should().BeNull();
        editor.Find("#iv-0").GetAttribute("value").Should().Be(draft.Ivs[0].ToString(CultureInfo.InvariantCulture));
        editor.Find("#ev-0").GetAttribute("value").Should().Be("0");
    }

    /// <summary>The moveset of <see cref="ItemMoveDraftTests.Moveset"/>: Tackle (20 of 49 PP, 2 PP Ups), an empty slot, Surf (24 of 24, 3 PP Ups) and Sketch.</summary>
    private static EditorDraft Moveset(Action<PK6>? change = null) => Open(ItemMoveDraftTests.Moveset(change: change), SlotRef.InBox(0, 1));

    private static string MoveName(EditorDraft draft, Move move) => draft.Capabilities.Lists.Moves.Single(m => m.Value == (int)move).Text;

    [Fact]
    public void EachMoveFieldIsNamedByItsSlotAndColumn()
    {
        var draft = Moveset();
        var editor = Render(draft);

        editor.Find("#ppups-head").TextContent.Should().Be("PP Ups (0–3)");
        for (var slot = 0; slot < EditorDraft.MoveCount; slot++)
        {
            editor.Find($"#slot-{slot}").TextContent.Should().Be($"Move {slot + 1}");
            editor.Find($"#move-{slot}").GetAttribute("aria-labelledby").Should().Be($"slot-{slot} move-head");
            editor.Find($"#pp-{slot}").GetAttribute("aria-labelledby").Should().Be($"slot-{slot} pp-head");
            editor.Find($"#pp-{slot}").GetAttribute("inputmode").Should().Be("numeric");
            editor.Find($"#ppups-{slot}").GetAttribute("aria-labelledby").Should().Be($"slot-{slot} ppups-head");
            editor.Find($"#maxpp-{slot}").TextContent.Should().Be(draft.Moves[slot].MaxPp.ToString(CultureInfo.InvariantCulture));
        }
        editor.Find("#pp-0").GetAttribute("value").Should().Be("20");
        editor.Find("#move-list-note").TextContent.Should().Be(EditorText.MoveListNote);
        editor.Find("#move-note").GetAttribute("role").Should().Be("status");
        editor.Find("#move-note").TextContent.Should().BeEmpty();
        editor.Find("#held-item").GetAttribute("aria-describedby").Should().Be("item-note");
        editor.Find("#item-note").TextContent.Should().Be(EditorText.ItemNote);
    }

    [Fact]
    public void TheMoveAndItemBoxesListCoresChoices()
    {
        var draft = Moveset();
        var editor = Render(draft);
        var lists = draft.Capabilities.Lists;

        editor.FindAll("#move-0 option").Select(o => o.GetAttribute("value")).Should().Equal(lists.Moves.Select(m => m.Value.ToString(CultureInfo.InvariantCulture)));
        // Core lists Pretty Feather (571) in two pouches; the box offers each value once.
        lists.Items.Count(i => i.Value == 571).Should().Be(2, "the fixture must have Core's duplicate");
        editor.FindAll("#held-item option").Select(o => o.GetAttribute("value")).Should().Equal(lists.Items.Select(i => i.Value).Distinct().Select(v => v.ToString(CultureInfo.InvariantCulture)));
        editor.Find("#move-0 option[selected]").TextContent.Should().Be(MoveName(draft, Move.Tackle));
        editor.Find("#move-1 option[selected]").GetAttribute("value").Should().Be("0");
        editor.Find("#held-item option[selected]").GetAttribute("value").Should().Be(ItemMoveDraftTests.Leftovers.ToString(CultureInfo.InvariantCulture));
        editor.FindAll("#ppups-0 option").Select(o => o.TextContent).Should().Equal("0", "1", "2", "3");
    }

    [Fact]
    public void AnEmptySlotAndAMoveWithoutPpUpsOfferNothingToChange()
    {
        var editor = Render(Moveset());

        editor.Find("#pp-1").HasAttribute("readonly").Should().BeTrue();
        editor.Find("#ppups-1").HasAttribute("disabled").Should().BeTrue();
        editor.Find("#pp-3").HasAttribute("readonly").Should().BeFalse("Sketch's PP can be lowered");
        editor.Find("#ppups-3").HasAttribute("disabled").Should().BeTrue();
        editor.FindAll("#ppups-3 option").Select(o => o.TextContent).Should().Equal("0");
        editor.Find("#pp-0").HasAttribute("readonly").Should().BeFalse();
        editor.Find("#ppups-0").HasAttribute("disabled").Should().BeFalse();
    }

    [Fact]
    public void StoredPpUpsOnAMoveWithoutThemCanOnlyBeRemoved()
    {
        var draft = Moveset(p => p.Move4_PPUps = 2);
        var editor = Render(draft);

        editor.Find("#ppups-3").HasAttribute("disabled").Should().BeFalse();
        editor.FindAll("#ppups-3 option").Select(o => o.TextContent).Should().Equal("0", "2");

        editor.Find("#ppups-3").Change("0");

        refused.Should().BeNull();
        draft.Moves[3].PpUps.Should().Be(0);
        editor.Find("#ppups-3").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void AMoveChangeShowsThePpItSet()
    {
        var draft = Moveset();
        var editor = Render(draft);

        editor.Find("#move-0").Change(((int)Move.Thunderbolt).ToString(CultureInfo.InvariantCulture));

        refused.Should().BeNull();
        draft.Moves[0].Move.Should().Be((ushort)Move.Thunderbolt);
        editor.Find("#pp-0").GetAttribute("value").Should().Be("21");
        editor.Find("#maxpp-0").TextContent.Should().Be("21");
        editor.Find("#move-note").TextContent.Should().Be($"Move 1 is now {MoveName(draft, Move.Thunderbolt)}, with full PP: 21 of 21 (2 PP Ups).");
    }

    [Fact]
    public void TheMoveNoteNamesEachKindOfChange()
    {
        var draft = Moveset();
        var editor = Render(draft);
        var tackle = MoveName(draft, Move.Tackle);
        var sketch = MoveName(draft, Move.Sketch);

        editor.Find("#move-2").Change(((int)Move.Sketch).ToString(CultureInfo.InvariantCulture));
        editor.Find("#move-note").TextContent.Should().Be($"Move 3 is now {sketch}. PP Ups cannot be used on it, so its 3 PP Ups were removed; its PP is set to 1 of 1.");

        editor.Find("#move-0").Change("0");
        editor.Find("#move-note").TextContent.Should().Be("Move 1 is now empty, so its PP and PP Ups are 0.");
        editor.Find("#pp-0").HasAttribute("readonly").Should().BeTrue();

        editor.Find("#move-0").Change(((int)Move.Tackle).ToString(CultureInfo.InvariantCulture));
        editor.Find("#move-note").TextContent.Should().Be($"Move 1 is back to its stored move, {tackle}, with its stored PP (20 of 49) and 2 PP Ups.");

        editor.Find("#ppups-0").Change("1");
        editor.Find("#move-note").TextContent.Should().Be($"Move 1, {tackle}, now has 1 PP Up; its PP is 42 of 42.");
        editor.Find("#pp-0").GetAttribute("value").Should().Be("42");

        editor.Find("#pp-0").Input("40");
        editor.Find("#move-note").TextContent.Should().BeEmpty("a PP edit shows in its own field, and the last move change is no longer the last edit");
    }

    [Fact]
    public void AHeldItemChoiceIsDrafted()
    {
        var draft = Moveset();
        var editor = Render(draft);

        editor.Find("#held-item").Change(ItemMoveDraftTests.ChoiceScarf.ToString(CultureInfo.InvariantCulture));

        refused.Should().BeNull();
        draft.HeldItem.Should().Be(ItemMoveDraftTests.ChoiceScarf);
        editor.Find("#held-item").Change("0");
        draft.HeldItem.Should().Be(0);
    }

    [Theory]
    [InlineData("50")]
    [InlineData("-1")]
    [InlineData("")]
    [InlineData("1e2")]
    [InlineData("99999999999999999999")]
    public void RefusedPpIsKeptAsTyped(string typed)
    {
        var draft = Moveset();
        var editor = Render(draft);

        editor.Find("#pp-0").Input(typed);

        refused.Should().Be(SessionError.PpOutOfRange);
        draft.IsDirty.Should().BeFalse();
        editor.Find("#pp-0").GetAttribute("value").Should().Be(typed);
    }

    [Fact]
    public void AnAcceptedEditReplacesRefusedPpInput()
    {
        var draft = Moveset();
        var editor = Render(draft);

        editor.Find("#pp-0").Input("50");
        editor.Find("#pp-2").Input("05");

        refused.Should().BeNull();
        editor.Find("#pp-0").GetAttribute("value").Should().Be("20");
        editor.Find("#pp-2").GetAttribute("value").Should().Be("05", "text that reads as the drafted value is not rewritten under the cursor");
    }

    [Fact]
    public void StoredValuesOutsideTheListsAreShownAsStored()
    {
        var draft = Moveset(p =>
        {
            p.HeldItem = 0x7FFF;
            p.Move2 = 0x7FFE;
            p.Move1_PPUps = 9;
        });
        var editor = Render(draft);

        editor.Find("#held-item option[selected]").TextContent.Should().Be("Unknown (stored value 32767)");
        editor.Find("#move-1 option[selected]").TextContent.Should().Be("Unknown (stored value 32766)");
        editor.Find("#move-1").GetAttribute("value").Should().Be("32766", "the value is set on the select, which Firefox honours after a user's choice");
        editor.Find("#ppups-0 option[selected]").TextContent.Should().Be("9 (stored; above the maximum)");
        editor.FindAll("#held-item option[disabled]").Should().ContainSingle();
    }

    [Fact]
    public void MoveAndPpFieldsFollowTheirOwnFlag()
    {
        var bytes = ItemMoveDraftTests.Moveset();
        var save = SaveFixtures.Parse(bytes);
        var family = PKHeX.Web.Services.SupportMatrix.Find(save)! with { Editable = EditableFields.Moves };
        var editor = Render(new SaveSession(bytes.ToArray(), save, "fixture.sav", SaveCapabilities.For(save, family)).Select(SlotRef.InBox(0, 1)));

        editor.Find("#move-0").HasAttribute("disabled").Should().BeFalse();
        editor.Find("#pp-0").HasAttribute("readonly").Should().BeTrue();
        editor.Find("#ppups-0").HasAttribute("disabled").Should().BeTrue();
        editor.Find("#held-item").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void MoveFieldsAreWrittenWithInvariantDigits()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-SA");
            var draft = Moveset();
            var editor = Render(draft);

            editor.Find("#ppups-0").Change("3");

            editor.Find("#pp-0").GetAttribute("value").Should().Be("56");
            editor.Find("#maxpp-0").TextContent.Should().Be("56");
            editor.FindAll("#ppups-0 option").Select(o => o.TextContent).Should().Equal("0", "1", "2", "3");
            editor.Find("#move-note").TextContent.Should().EndWith("now has 3 PP Ups; its PP is 56 of 56.");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void ANewDraftReloadsTheMoveFieldsAndClearsTheNote()
    {
        var bytes = ItemMoveDraftTests.Moveset();
        var session = SaveFixtures.Open(bytes);
        var draft = session.Select(SlotRef.InBox(0, 1));
        var editor = Render(draft);
        editor.Find("#move-0").Change("0");
        editor.Find("#pp-2").Input("99");

        var other = session.Select(SaveFixtures.FirstBoxSlot);
        editor.Render(p => p.Add(c => c.Draft, other));

        editor.Find("#move-note").TextContent.Should().BeEmpty();
        editor.Find("#pp-2").GetAttribute("value").Should().Be(other.Moves[2].Pp.ToString(CultureInfo.InvariantCulture));
        editor.Find("#move-0 option[selected]").GetAttribute("value").Should().Be(other.Moves[0].Move.ToString(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void AMoveOrItemFromTheOtherGameIsNamedAsTheInspectorNamesIt()
    {
        // A Pokémon traded from Omega Ruby into X may know Dragon Ascent (620) and hold Audinite, which X's lists leave out.
        const int audinite = 757; // line 758 of Core's English item list, which starts at 0
        var bytes = SaveFixtures.Synthetic(false, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            p.Move2 = (ushort)Move.DragonAscent;
            p.HeldItem = audinite;
        }));
        var draft = Open(bytes, SlotRef.InBox(0, 1));
        draft.Capabilities.Lists.Items.Any(i => i.Value == audinite).Should().BeFalse("the fixture needs an item outside X's list");
        var editor = Render(draft);

        editor.Find("#move-1 option[selected]").TextContent.Should().Be($"{GameInfo.Strings.movelist[(int)Move.DragonAscent]} (not available in this game)");
        editor.Find("#held-item option[selected]").TextContent.Should().Be($"{GameInfo.Strings.itemlist[audinite]} (not available in this game)");
        editor.Find("#move-1 option[selected]").TextContent.Should().Be(InspectorText.Move(draft.Inspect().Moves.Moves[1].Move));
    }

    [Fact]
    public void AKeystrokeElsewhereDoesNotRenderTheLongListsAgain()
    {
        var draft = Moveset();
        var editor = Render(draft);
        var boxes = editor.FindComponents<ChoiceSelect>();
        boxes.Should().HaveCount(EditorDraft.MoveCount + 3, "the species, the ability, the held item and each move");
        var counts = boxes.Select(b => b.RenderCount).ToArray();

        editor.Find("#ot-friendship").Input("100");
        editor.Find("#ev-0").Input("4");

        refused.Should().BeNull();
        boxes.Select(b => b.RenderCount).Should().Equal(counts, "nothing the species, ability, move and item boxes show changed");
        editor.Find("#move-0").Change(((int)Move.Thunderbolt).ToString(CultureInfo.InvariantCulture));
        boxes[3].RenderCount.Should().Be(counts[3] + 1, "the changed move box renders its new value");
        boxes[2].RenderCount.Should().Be(counts[2], "the item box did not change");
        boxes[1].RenderCount.Should().Be(counts[1], "the ability box did not change");
        boxes[0].RenderCount.Should().Be(counts[0], "the species box did not change");
    }

    [Fact]
    public void TheMoveNoteSaysWhenPpUpsAreCarriedOverOrDropped()
    {
        var draft = Moveset(p => p.Move1_PPUps = 9);
        var editor = Render(draft);
        var surf = MoveName(draft, Move.Surf);
        var sketch = MoveName(draft, Move.Sketch);

        editor.Find("#move-0").Change(((int)Move.Thunderbolt).ToString(CultureInfo.InvariantCulture));
        editor.Find("#move-note").TextContent.Should().EndWith("(no PP Ups). The 9 PP Ups stored before, more than a move can take, were not carried over.");

        editor.Find("#move-2").Change(((int)Move.Sketch).ToString(CultureInfo.InvariantCulture));
        editor.Find("#move-2").Change(((int)Move.Tackle).ToString(CultureInfo.InvariantCulture));
        editor.Find("#move-note").TextContent.Should().Be($"Move 3 is now {MoveName(draft, Move.Tackle)}, with full PP: 56 of 56 (3 PP Ups). It keeps the 3 PP Ups this slot had before {sketch}.");

        editor.Find("#move-1").Change(((int)Move.Surf).ToString(CultureInfo.InvariantCulture));
        editor.Find("#move-note").TextContent.Should().Be($"Move 2 is now {surf}, with full PP: 15 of 15 (no PP Ups).");
    }

    /// <summary>A box 1, slot 2 copy of the known legal Zigzagoon, changed by <paramref name="change"/>.</summary>
    private static EditorDraft Boxed(Action<PK6> change) => Open(AbilityGenderDraftTests.Boxed(change: change), SlotRef.InBox(0, 1));

    [Fact]
    public void TheAbilityBoxListsTheSlotsFromCoresPersonalData()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);
        var box = editor.Find("#ability");

        editor.Find("label[for=ability]").TextContent.Should().Be("Ability");
        box.GetAttribute("aria-describedby").Should().Be("ability-note");
        box.GetAttribute("value").Should().Be((draft.AbilityNumber >> 1).ToString(CultureInfo.InvariantCulture));
        editor.FindAll("#ability option").Select(o => o.GetAttribute("value")).Should().Equal("0", "1", "2");
        editor.FindAll("#ability option").Select(o => o.TextContent).Should().Equal(draft.AbilityChoices.Select(c => c.Text));
        editor.FindAll("#ability option").Select(o => o.TextContent).Should().Equal(
            draft.Capabilities.Lists.GetAbilityList(draft.Preview().PersonalInfo).Select(c => c.Text), "the names are Core's, with its (1), (2) and (H)");
        editor.Find("#ability-note").TextContent.Should().Be(EditorText.AbilityNote(regularSlotsSame: false, storedUnmatched: false));
        editor.Find("#ability-note").TextContent.Should().Contain("legality analysis checks");
        editor.Find("#ability-note").GetAttribute("role").Should().Be("status");
    }

    [Fact]
    public void AnAbilityChoiceIsDrafted()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);

        editor.Find("#ability").Change("2");

        refused.Should().BeNull();
        draft.AbilitySlot.Should().Be(2);
        draft.AbilityNumber.Should().Be(4);
        editor.Find("#ability").GetAttribute("value").Should().Be("2");
    }

    [Fact]
    public void TwoSlotsWithTheSameAbilityAreBothOfferedAndNoted()
    {
        var editor = Render(Boxed(p =>
        {
            p.Species = (ushort)Species.Gastly;
            p.RefreshAbility(1);
        }));

        editor.FindAll("#ability option").Should().HaveCount(3, "the slots stay apart even with the same ability");
        editor.Find("#ability").GetAttribute("value").Should().Be("1");
        editor.Find("#ability-note").TextContent.Should().Contain("Both regular abilities are the same, but the slot is still stored");
    }

    [Theory]
    [InlineData(3)]
    [InlineData(0)]
    public void AStoredAbilityAndSlotThatDoNotMatchAreShownAsStored(int number)
    {
        var draft = Boxed(p => p.AbilityNumber = number);
        var editor = Render(draft);
        var ability = GameInfo.Strings.abilitylist[draft.Ability];

        editor.Find("#ability option[disabled]").TextContent.Should().Be($"{ability} (stored; unknown slot, stored value {number})");
        editor.Find("#ability option[selected]").TextContent.Should().Be($"{ability} (stored; unknown slot, stored value {number})");
        editor.Find("#ability-note").TextContent.Should().Contain("do not match one of this species' slots");

        editor.Find("#ability").Change("0");

        editor.FindAll("#ability option[disabled]").Should().BeEmpty();
        editor.Find("#ability").GetAttribute("value").Should().Be("0");
        editor.Find("#ability-note").TextContent.Should().NotContain("do not match");
    }

    [Fact]
    public void TheGenderBoxOffersBothGendersForASpeciesWithBoth()
    {
        var draft = Open(SaveFixtures.Synthetic(true));
        var editor = Render(draft);
        var other = 1 - draft.Gender;

        editor.FindAll("#gender option").Select(o => o.TextContent).Should().Equal("Male", "Female");
        editor.Find("#gender").GetAttribute("value").Should().Be(draft.Gender.ToString(CultureInfo.InvariantCulture));
        editor.Find("#gender").HasAttribute("disabled").Should().BeFalse();
        editor.Find("#gender").GetAttribute("aria-describedby").Should().Be("gender-note");
        editor.Find("#gender-note").TextContent.Should().Be(EditorText.GenderNote(GenderRule.Either, formFollowsGender: false));

        editor.Find("#gender").Change(other.ToString(CultureInfo.InvariantCulture));

        refused.Should().BeNull();
        draft.Gender.Should().Be((byte)other);
        editor.Find("#gender").GetAttribute("value").Should().Be(other.ToString(CultureInfo.InvariantCulture));
        editor.Find("#gender-change").TextContent.Should().BeEmpty("nothing else changed");
    }

    [Theory]
    [InlineData(Species.Tauros, EntityGender.Male, "This species is always male.")]
    [InlineData(Species.Chansey, EntityGender.Female, "This species is always female.")]
    [InlineData(Species.Magnemite, EntityGender.Genderless, "This species is genderless.")]
    public void ASingleGenderSpeciesShowsItsOnlyGenderReadOnly(Species species, byte gender, string note)
    {
        var editor = Render(Boxed(p =>
        {
            p.Species = (ushort)species;
            p.Gender = gender;
            p.RefreshAbility(0);
        }));

        editor.FindAll("#gender option").Select(o => o.TextContent).Should().Equal(EditorText.GenderName(gender));
        editor.Find("#gender").HasAttribute("disabled").Should().BeTrue("there is nothing else to choose");
        editor.Find("#gender-note").TextContent.Should().Be(note);
    }

    [Fact]
    public void AWrongStoredGenderIsShownAsStoredAndCanBeCorrected()
    {
        var draft = Boxed(p =>
        {
            p.Species = (ushort)Species.Tauros;
            p.Gender = EntityGender.Female;
            p.RefreshAbility(0);
        });
        var editor = Render(draft);

        editor.Find("#gender option[disabled]").TextContent.Should().Be("Female");
        editor.Find("#gender option[selected]").TextContent.Should().Be("Female");
        editor.Find("#gender").HasAttribute("disabled").Should().BeFalse();

        editor.Find("#gender").Change("0");

        draft.Gender.Should().Be(EntityGender.Male);
        editor.FindAll("#gender option").Select(o => o.TextContent).Should().Equal("Male");
        editor.Find("#gender").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void AStoredGenderValueOutsideTheRangeIsShownAsUnknown()
    {
        var editor = Render(Boxed(p => p.Gender = 3));

        editor.Find("#gender option[selected]").TextContent.Should().Be("Unknown (stored value 3)");
        editor.Find("#gender option[disabled]").GetAttribute("value").Should().Be("3");
    }

    [Fact]
    public void AMeowsticGenderChangeSaysWhatItChangedWithTheForm()
    {
        var draft = Boxed(AbilityGenderDraftTests.MaleMeowstic);
        var stored = draft.Preview();
        var editor = Render(draft);
        editor.Find("#gender-note").TextContent.Should().Contain("form is its gender");
        editor.FindAll("#ability option").Select(o => o.TextContent).Last().Should().StartWith(GameInfo.Strings.abilitylist[(int)Ability.Prankster]);

        editor.Find("#gender").Change("1");

        refused.Should().BeNull();
        var competitive = GameInfo.Strings.abilitylist[(int)Ability.Competitive];
        editor.Find("#gender-change").TextContent.Should().Be(
            $"Its form changed with its gender. Its ability is now {competitive} (hidden ability). Its experience points are now {draft.Experience} (level 30), from {stored.EXP}.");
        editor.Find("#gender-change").GetAttribute("role").Should().Be("status");
        editor.FindAll("#ability option").Select(o => o.TextContent).Last().Should().Be($"{competitive} (H)", "the box lists the new form's slots");
        editor.Find("#ability").GetAttribute("value").Should().Be("2");
        editor.Find("#exp").GetAttribute("value").Should().Be(draft.Experience.ToString(CultureInfo.InvariantCulture));

        editor.Find("#ot-friendship").Input("100");

        editor.Find("#gender-change").TextContent.Should().BeEmpty("another edit clears the note");
    }

    [Fact]
    public void AbilityAndGenderFollowTheirOwnFlag()
    {
        var bytes = SaveFixtures.Synthetic(true);
        var save = SaveFixtures.Parse(bytes);
        EditorDraft Only(EditableFields fields) => new SaveSession(bytes.ToArray(), save, "fixture.sav",
            SaveCapabilities.For(save, PKHeX.Web.Services.SupportMatrix.Find(save)! with { Editable = fields })).Select(SaveFixtures.FirstBoxSlot);

        var ability = Render(Only(EditableFields.Ability));
        ability.Find("#ability").HasAttribute("disabled").Should().BeFalse();
        ability.Find("#gender").HasAttribute("disabled").Should().BeTrue();
        var gender = Render(Only(EditableFields.Gender));
        gender.Find("#ability").HasAttribute("disabled").Should().BeTrue();
        gender.Find("#gender").HasAttribute("disabled").Should().BeFalse();
    }

    [Fact]
    public void ANewDraftReloadsTheAbilityAndGenderAndClearsTheNote()
    {
        var session = SaveFixtures.Open(AbilityGenderDraftTests.Boxed(change: AbilityGenderDraftTests.MaleMeowstic));
        var editor = Render(session.Select(SlotRef.InBox(0, 1)));
        editor.Find("#gender").Change("1");
        editor.Find("#ability").Change("0");

        var other = session.Select(SaveFixtures.FirstBoxSlot);
        editor.Render(p => p.Add(c => c.Draft, other));

        editor.Find("#gender-change").TextContent.Should().BeEmpty();
        editor.Find("#gender").GetAttribute("value").Should().Be(other.Gender.ToString(CultureInfo.InvariantCulture));
        editor.Find("#ability").GetAttribute("value").Should().Be((other.AbilityNumber >> 1).ToString(CultureInfo.InvariantCulture));
        editor.FindAll("#ability option").Select(o => o.TextContent).Should().Equal(other.AbilityChoices.Select(c => c.Text));
    }

    [Fact]
    public void TheSpeciesBoxListsTheGamesSpeciesAndTheFormBoxSaysWhenThereAreNoForms()
    {
        var draft = Boxed(_ => { });
        var editor = Render(draft);

        editor.Find("label[for=species]").TextContent.Should().Be("Species");
        editor.Find("#species").GetAttribute("value").Should().Be(draft.Species.ToString(CultureInfo.InvariantCulture));
        editor.FindAll("#species option").Select(o => o.GetAttribute("value")).Should().Equal(draft.Capabilities.SpeciesChoices.Select(s => s.Value.ToString(CultureInfo.InvariantCulture)));
        editor.FindAll("#species option").Select(o => o.TextContent).Should().Equal(draft.Capabilities.SpeciesChoices.Select(s => s.Text), "the names are Core's");
        editor.FindAll("#form option").Select(o => o.TextContent).Should().Equal(EditorText.NoAlternateForms);
        editor.Find("#form").HasAttribute("disabled").Should().BeTrue();
        editor.Find("#species-note").TextContent.Should().Be(EditorText.SpeciesNote);
        editor.Find("#species-preview").TextContent.Should().BeEmpty();
        editor.FindAll("#species-confirm").Should().BeEmpty();
    }

    [Fact]
    public void TheFormBoxListsCoresFormsAndMarksBattleOnlyOnes()
    {
        var draft = Boxed(p => p.Species = (ushort)Species.Charizard);
        var editor = Render(draft);

        editor.FindAll("#form option").Select(o => o.TextContent).Should().Equal("Normal", "Mega X (battle only)", "Mega Y (battle only)");
        editor.Find("#form").HasAttribute("disabled").Should().BeFalse();
        editor.Find("#form").GetAttribute("value").Should().Be("0");
        EditorText.FormName(new FormChoice(1, "Rock Star", false, false)).Should().Be("Rock Star (not in this game)");
        EditorText.FormName(new FormChoice(3, "", false, true)).Should().Be("Form 3");
    }

    [Fact]
    public void AStoredFormOutsideTheListIsShownAsStored()
    {
        var editor = Render(Boxed(p => p.Form = 3));

        editor.FindAll("#form option").Select(o => o.TextContent).Should().Equal(EditorText.UnlistedForm(3));
        editor.Find("#form").GetAttribute("value").Should().Be("3");
    }

    [Fact]
    public void ChoosingASpeciesPreviewsTheChangeWithoutMakingIt()
    {
        var draft = Boxed(p => p.EXP = Experience.GetEXP(50, p.PersonalInfo.EXPGrowth) + 10);
        var stored = draft.Preview();
        var editor = Render(draft);
        var magikarp = (int)Species.Magikarp;

        editor.Find("#species").Change(magikarp.ToString(CultureInfo.InvariantCulture));

        draft.EditRevision.Should().Be(0, "nothing is changed until it is confirmed");
        refused.Should().BeNull();
        var preview = draft.PreviewSpeciesForm(magikarp, 0);
        var names = GameInfo.Strings.specieslist;
        editor.Find("#species-preview").GetAttribute("role").Should().Be("status");
        editor.Find("#species-preview-title").TextContent.Should().Be($"Change {names[(int)Species.Zigzagoon]} to {names[magikarp]}?");
        var lines = editor.FindAll("#species-preview-changes li").Select(li => li.TextContent).ToArray();
        lines.Should().Contain($"Species: {names[(int)Species.Zigzagoon]} to {names[magikarp]}.");
        lines.Should().Contain(string.Create(CultureInfo.InvariantCulture,
            $"Experience points: {stored.EXP} to {preview.After.Experience}, so the level goes from 50 to {preview.After.Level} on the new growth rate."));
        lines.Should().Contain(l => l.StartsWith("Ability: ", StringComparison.Ordinal));
        lines.Should().Contain(l => l.StartsWith("Stats: HP ", StringComparison.Ordinal));
        lines.Should().Contain($"Name: {TestText.Isolated(stored.Nickname)} to {TestText.Isolated(names[magikarp])}, as it is not nicknamed.");
        editor.Find("#species").GetAttribute("value").Should().Be(magikarp.ToString(CultureInfo.InvariantCulture), "the box holds the previewed choice");
        editor.Find("#species-confirm").TextContent.Should().Be(EditorText.ConfirmSpeciesForm);

        editor.Find("#species-confirm").Click();

        refused.Should().BeNull();
        draft.Species.Should().Be((ushort)magikarp);
        draft.EditRevision.Should().Be(1);
        editor.FindAll("#species-confirm").Should().BeEmpty();
        editor.Find("#species-preview").TextContent.Should().BeEmpty();
        editor.Find("#species-change").TextContent.Should().Be(EditorText.SpeciesFormChanged(lines));
        editor.Find("#exp").GetAttribute("value").Should().Be(draft.Experience.ToString(CultureInfo.InvariantCulture), "every field shows the changed draft");
        editor.Find("#nickname").GetAttribute("value").Should().Be(names[magikarp]);

        editor.Find("#ot-friendship").Input("100");

        editor.Find("#species-change").TextContent.Should().BeEmpty("another edit clears the note");
    }

    [Fact]
    public void CancellingAPreviewShowsTheDraftAgain()
    {
        var draft = Boxed(_ => { });
        var editor = Render(draft);
        editor.Find("#species").Change(((int)Species.Charizard).ToString(CultureInfo.InvariantCulture));
        editor.FindAll("#form option").Should().HaveCount(3, "the form box lists the previewed species' forms");
        editor.Find("#form").Change("2");
        editor.Find("#species-preview-title").TextContent.Should().EndWith("(Mega Y (battle only))?");
        editor.FindAll("#species-preview-changes li").Select(li => li.TextContent).Should().Contain("This form exists only during battle, so legality analysis reports a Pokémon stored in it.");

        editor.Find("#species-cancel").Click();

        draft.IsDirty.Should().BeFalse();
        editor.Find("#species").GetAttribute("value").Should().Be(((int)Species.Zigzagoon).ToString(CultureInfo.InvariantCulture));
        editor.FindAll("#form option").Select(o => o.TextContent).Should().Equal(EditorText.NoAlternateForms);
        editor.Find("#species-preview").TextContent.Should().BeEmpty();
        editor.FindAll("#species-cancel").Should().BeEmpty();
    }

    [Fact]
    public void ChoosingTheDraftedSpeciesAgainEndsThePreview()
    {
        var editor = Render(Boxed(_ => { }));
        editor.Find("#species").Change(((int)Species.Linoone).ToString(CultureInfo.InvariantCulture));

        editor.Find("#species").Change(((int)Species.Zigzagoon).ToString(CultureInfo.InvariantCulture));

        editor.FindAll("#species-confirm").Should().BeEmpty();
        editor.Find("#species-preview").TextContent.Should().BeEmpty();
    }

    [Fact]
    public void ReselectingTheDraftedSpeciesWithAStoredFormOutsideTheListIsNoRefusal()
    {
        var draft = Boxed(p => p.Form = 3);
        var editor = Render(draft);
        editor.Find("#species").Change(((int)Species.Linoone).ToString(CultureInfo.InvariantCulture));

        editor.Find("#species").Change(((int)Species.Zigzagoon).ToString(CultureInfo.InvariantCulture));

        editor.Find("#species-preview").TextContent.Should().BeEmpty("going back to the drafted species is no change, not a refused form");
        editor.FindAll("#species-confirm").Should().BeEmpty();
        editor.Find("#form").GetAttribute("value").Should().Be("3");
    }

    [Fact]
    public void AnotherEditEndsThePreview()
    {
        var draft = Boxed(_ => { });
        var editor = Render(draft);
        editor.Find("#species").Change(((int)Species.Linoone).ToString(CultureInfo.InvariantCulture));

        editor.Find("#ot-friendship").Input("100");

        editor.FindAll("#species-confirm").Should().BeEmpty("the preview described the draft before the edit");
        editor.Find("#species").GetAttribute("value").Should().Be(((int)Species.Zigzagoon).ToString(CultureInfo.InvariantCulture));
        draft.Species.Should().Be((ushort)Species.Zigzagoon);
    }

    [Fact]
    public void ARefusedPreviewIsReportedAndTheBoxesShowTheDraft()
    {
        var draft = Open(PartyApplyTests.WithoutStoredStats(), SlotRef.InParty(0));
        var editor = Render(draft);

        editor.Find("#species").Change(((int)Species.Linoone).ToString(CultureInfo.InvariantCulture));

        editor.Find("#species-preview-refusal").TextContent.Should().Be(UserMessages.For(SessionError.PartyStatsMissing));
        editor.FindAll("#species-confirm").Should().BeEmpty();
        editor.Find("#species").GetAttribute("value").Should().Be(draft.Species.ToString(CultureInfo.InvariantCulture));
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void APartyPreviewNamesTheRecalculatedStatsAndKeptHp()
    {
        // At full HP: Magikarp's maximum is lower than Zigzagoon's, so the current HP comes down to it.
        var draft = Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember("Leader")), SlotRef.InParty(0));
        var full = draft.Preview().Stat_HPCurrent;
        var editor = Render(draft);

        editor.Find("#species").Change(((int)Species.Magikarp).ToString(CultureInfo.InvariantCulture));

        var preview = draft.PreviewSpeciesForm((int)Species.Magikarp, 0);
        var lines = editor.FindAll("#species-preview-changes li").Select(li => li.TextContent).ToArray();
        preview.After.Stats[0].Should().BeLessThan(full);
        lines.Should().Contain(string.Create(CultureInfo.InvariantCulture, $"Current HP: {full} to {preview.After.Stats[0]}; it is never raised, and its status is kept."));
        var hpStat = string.Create(CultureInfo.InvariantCulture, $"Stats: HP {full} to {preview.After.Stats[0]}");
        lines.Should().Contain(l => l.StartsWith(hpStat, StringComparison.Ordinal));
    }

    [Fact]
    public void TheSpeciesAndFormFollowTheirOwnFlag()
    {
        var bytes = SaveFixtures.Synthetic(true);
        var save = SaveFixtures.Parse(bytes);
        EditorDraft Only(EditableFields fields) => new SaveSession(bytes.ToArray(), save, "fixture.sav",
            SaveCapabilities.For(save, PKHeX.Web.Services.SupportMatrix.Find(save)! with { Editable = fields })).Select(SaveFixtures.FirstBoxSlot);

        Render(Only(EditableFields.Species)).Find("#species").HasAttribute("disabled").Should().BeFalse();
        Render(Only(EditableFields.Gender)).Find("#species").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ANewDraftClearsThePreviewAndTheNote()
    {
        var session = SaveFixtures.Open(AbilityGenderDraftTests.Boxed());
        var editor = Render(session.Select(SlotRef.InBox(0, 1)));
        editor.Find("#species").Change(((int)Species.Linoone).ToString(CultureInfo.InvariantCulture));
        editor.Find("#species-confirm").Click();
        editor.Find("#species").Change(((int)Species.Pikachu).ToString(CultureInfo.InvariantCulture));

        var other = session.Select(SaveFixtures.FirstBoxSlot);
        editor.Render(p => p.Add(c => c.Draft, other));

        editor.Find("#species-change").TextContent.Should().BeEmpty();
        editor.FindAll("#species-confirm").Should().BeEmpty();
        editor.Find("#species").GetAttribute("value").Should().Be(other.Species.ToString(CultureInfo.InvariantCulture));
    }
}
