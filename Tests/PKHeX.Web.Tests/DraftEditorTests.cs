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
/// The draft editor (WEB-PKM-003, WEB-PKM-005, WEB-PKM-006, WEB-PKM-007): labelled name, language, friendship, level, experience and
/// nature fields that turn input into typed draft edits, show what the draft holds after an accepted edit, keep refused input as typed,
/// and offer nothing for an egg.
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
        editor.Find("#name-note").TextContent.Should().Be($"Not nicknamed, so its name changed from Quill to its {LanguageName(draft, (int)LanguageID.English)} default, {ZigzagoonIn((int)LanguageID.English)}.");
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
        editor.Find("#name-note").TextContent.Should().Be($"Not nicknamed, so its name changed from Quill to its {LanguageName(draft, German)} default, {ZigzagoonIn(German)}.");
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

        editor.Find("#name-note").TextContent.Should().EndWith($"The game's font cannot show some of these characters; it would show the name as {draft.Name.Displayed}.");
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
        var other = Open(SaveFixtures.Synthetic(true, legal: false));

        editor.Render(p => p.Add(c => c.Draft, other));

        editor.Find("#nickname").GetAttribute("value").Should().Be(other.Nickname);
        editor.Find("#ot-friendship").GetAttribute("value").Should().Be(other.TrainerFriendship.ToString(CultureInfo.InvariantCulture));
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
    }
}
