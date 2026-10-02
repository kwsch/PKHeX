using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Drafting the name, language and friendship (WEB-PKM-003, WEB-PKM-006): each edit changes only its own stored bytes, as the same native
/// Core edit does; refused values are never clamped and leave the draft unchanged; eggs are not edited.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class NameAndFriendshipDraftTests
{
    private const int English = (int)LanguageID.English, French = (int)LanguageID.French, German = (int)LanguageID.German;

    private static string ZigzagoonIn(int language) => SpeciesName.GetSpeciesNameGeneration((ushort)Species.Zigzagoon, language, 6);

    /// <summary>The known legal Zigzagoon in box 1, slot 1 (English, not nicknamed, no handling trainer).</summary>
    private static EditorDraft Zigzagoon(bool oras = true) => SaveFixtures.Open(SaveFixtures.Synthetic(oras)).Select(SaveFixtures.FirstBoxSlot);

    /// <summary>The known illegal Ditto in box 1, slot 1: it has a handling trainer, who is its current handler.</summary>
    private static EditorDraft Ditto() => SaveFixtures.Open(SaveFixtures.Synthetic(true, legal: false)).Select(SaveFixtures.FirstBoxSlot);

    /// <summary>
    /// Asserts the draft holds exactly the stored entity changed by <paramref name="nativeEdit"/>, so the edit wrote nothing else.
    /// </summary>
    private static void AssertMatchesNative(EditorDraft draft, PK6 stored, Action<PK6> nativeEdit)
    {
        var expected = (PK6)stored.Clone();
        nativeEdit(expected);
        draft.Preview().Data.ToArray().Should().Equal(expected.Data.ToArray());
    }

    [Fact]
    public void TypingACustomNameSetsTheFlag()
    {
        var draft = Zigzagoon();
        var stored = draft.Preview();

        draft.TypeNickname("Quill");

        draft.IsNicknamed.Should().BeTrue();
        AssertMatchesNative(draft, stored, p =>
        {
            p.Nickname = "Quill";
            p.IsNicknamed = true;
        });
    }

    [Fact]
    public void TypingTheSpeciesNameInAnotherLanguageKeepsTheFlagClear()
    {
        var draft = Zigzagoon();

        draft.TypeNickname(ZigzagoonIn(French));

        draft.IsNicknamed.Should().BeFalse();
        draft.Nickname.Should().Be(ZigzagoonIn(French));
        draft.Name.KeptOtherLanguageName.Should().BeTrue();
    }

    [Fact]
    public void ClearingTheFlagRestoresTheDefaultNameAndItsStoredBytes()
    {
        var draft = Zigzagoon();

        draft.TypeNickname("Quill");
        draft.SetNicknamed(false);

        draft.Nickname.Should().Be(ZigzagoonIn(English));
        draft.IsNicknamed.Should().BeFalse();
        draft.IsDirty.Should().BeFalse("the stored name is restored with its stored bytes");
        draft.EditRevision.Should().Be(2);
    }

    [Fact]
    public void SettingTheFlagKeepsTheName()
    {
        var draft = Zigzagoon();
        var stored = draft.Preview();

        draft.SetNicknamed(true);

        AssertMatchesNative(draft, stored, p => p.IsNicknamed = true);
    }

    [Fact]
    public void ALanguageChangeKeepsASpeciesNameAndReportsIt()
    {
        // The desktop keeps an English species name when the language changes; the note says legality may report it.
        var draft = Zigzagoon();
        var stored = draft.Preview();

        draft.EditLanguage(French);

        draft.Nickname.Should().Be(ZigzagoonIn(English));
        draft.Name.KeptOtherLanguageName.Should().BeTrue();
        draft.Name.DefaultName.Should().Be(ZigzagoonIn(French));
        AssertMatchesNative(draft, stored, p => p.Language = French);
    }

    [Fact]
    public void ALanguageChangeGivesACustomNameThatIsNotNicknamedItsDefault()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            p.Nickname = "Quill";
            p.IsNicknamed = false;
        })));
        var draft = session.Select(SlotRef.InBox(0, 1));
        var stored = draft.Preview();

        draft.EditLanguage(German);

        draft.Nickname.Should().Be(ZigzagoonIn(German));
        draft.IsNicknamed.Should().BeFalse();
        AssertMatchesNative(draft, stored, p =>
        {
            p.Language = German;
            p.Nickname = ZigzagoonIn(German);
        });
    }

    [Fact]
    public void ALanguageChangeKeepsANickname()
    {
        var draft = Zigzagoon();
        draft.TypeNickname("Quill");

        draft.EditLanguage(German);

        draft.Nickname.Should().Be("Quill");
        draft.Name.DefaultName.Should().Be(ZigzagoonIn(German));
    }

    [Fact]
    public void ALanguageRoundTripLeavesTheDraftClean()
    {
        var draft = Zigzagoon();

        draft.EditLanguage(French);
        draft.EditLanguage(English);

        draft.IsDirty.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)] // unused in Generation 6
    [InlineData((int)LanguageID.ChineseS)]
    [InlineData(-1)]
    public void ALanguageOutsideTheGamesListIsRefused(int language)
    {
        var draft = Zigzagoon();
        draft.TypeNickname("Quill");
        var before = draft.Preview();

        var edit = () => draft.EditLanguage(language);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.LanguageNotAvailable);
        draft.Preview().Data.ToArray().Should().Equal(before.Data.ToArray());
        draft.EditRevision.Should().Be(1);
    }

    [Fact]
    public void EveryListedLanguageIsAccepted()
    {
        var draft = Zigzagoon();

        foreach (var language in draft.Capabilities.Lists.Languages)
        {
            draft.EditLanguage(language.Value);
            draft.Language.Should().Be(language.Value);
        }
    }

    [Fact]
    public void ClearingTheFlagKeepsTheNameWhenTheStoredLanguageHasNoDefault()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            p.Language = 6;
            p.Nickname = "Quill";
            p.IsNicknamed = true;
        })));
        var draft = session.Select(SlotRef.InBox(0, 1));
        var stored = draft.Preview();

        draft.SetNicknamed(false);

        draft.Nickname.Should().Be("Quill");
        AssertMatchesNative(draft, stored, p => p.IsNicknamed = false);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(255)]
    [InlineData(123)]
    public void TrainerFriendshipChangesOnlyItsByte(int value)
    {
        var draft = Zigzagoon();
        var stored = draft.Preview();

        draft.EditTrainerFriendship(value);

        draft.TrainerFriendship.Should().Be((byte)value);
        AssertMatchesNative(draft, stored, p => p.OriginalTrainerFriendship = (byte)value);
    }

    [Theory]
    [InlineData(256)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void FriendshipOutsideTheRangeIsRefusedNotClamped(int value)
    {
        var draft = Ditto();
        var before = draft.Preview();

        var trainer = () => draft.EditTrainerFriendship(value);
        var handler = () => draft.EditHandlerFriendship(value);

        trainer.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.FriendshipOutOfRange);
        handler.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.FriendshipOutOfRange);
        draft.Preview().Data.ToArray().Should().Equal(before.Data.ToArray());
        draft.EditRevision.Should().Be(0);
    }

    [Fact]
    public void HandlerFriendshipChangesOnlyItsByteAndKeepsTheCurrentHandler()
    {
        var draft = Ditto();
        var stored = draft.Preview();
        draft.HasHandlingTrainer.Should().BeTrue();
        draft.IsWithHandler.Should().BeTrue();

        draft.EditHandlerFriendship(200);

        draft.HandlerFriendship.Should().Be(200);
        draft.IsWithHandler.Should().BeTrue();
        AssertMatchesNative(draft, stored, p => p.HandlingTrainerFriendship = 200);
    }

    [Fact]
    public void HandlerFriendshipIsRefusedWithoutAHandlingTrainer()
    {
        var draft = Zigzagoon();
        draft.HasHandlingTrainer.Should().BeFalse();
        draft.IsWithHandler.Should().BeFalse();

        var edit = () => draft.EditHandlerFriendship(10);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.NoHandlingTrainer);
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void AnEggIsNotEdited()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            p.IsEgg = true;
            p.IsNicknamed = true;
            p.Nickname = SpeciesName.GetEggName(p.Language, 6);
        })));
        var draft = session.Select(SlotRef.InBox(0, 1));

        draft.IsEgg.Should().BeTrue();
        draft.CanApply.Should().BeTrue();
        draft.Editable.Should().Be(EditableFields.None);
        Action[] edits =
        [
            () => draft.EditNickname("Quill", true),
            () => draft.TypeNickname("Quill"),
            () => draft.SetNicknamed(false),
            () => draft.EditLanguage(French),
            () => draft.EditTrainerFriendship(1),
            () => draft.EditHandlerFriendship(1),
        ];
        foreach (var edit in edits)
        {
            edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.EggNotEditable);
        }
        draft.IsDirty.Should().BeFalse();
        draft.EditRevision.Should().Be(0);
    }

    [Fact]
    public void EachFieldIsGatedOnTheFamily()
    {
        var bytes = SaveFixtures.Synthetic(true);
        var save = SaveFixtures.Parse(bytes);
        var family = SupportMatrix.Find(save)! with { Editable = EditableFields.Nickname };
        var draft = new SaveSession(bytes.ToArray(), save, "fixture.sav", SaveCapabilities.For(save, family)).Select(SaveFixtures.FirstBoxSlot);

        var language = () => draft.EditLanguage(French);
        var friendship = () => draft.EditTrainerFriendship(1);

        language.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.FieldNotEditable);
        friendship.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.FieldNotEditable);
        draft.TypeNickname("Quill");
        draft.IsNicknamed.Should().BeTrue();
    }

    [Fact]
    public void ALanguageChangeThatRenamesNeedsNicknameEdits()
    {
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            p.Nickname = "Quill";
            p.IsNicknamed = false;
        }));
        var save = SaveFixtures.Parse(bytes);
        var family = SupportMatrix.Find(save)! with { Editable = EditableFields.Language };
        var session = new SaveSession(bytes.ToArray(), save, "fixture.sav", SaveCapabilities.For(save, family));

        var renaming = session.Select(SlotRef.InBox(0, 1));
        var rename = () => renaming.EditLanguage(German);
        rename.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.FieldNotEditable);
        renaming.IsDirty.Should().BeFalse();

        var keeping = session.Select(SaveFixtures.FirstBoxSlot);
        keeping.EditLanguage(French);
        keeping.Language.Should().Be(French, "a kept name needs no nickname edit");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PartyEditsKeepTheBattleStateAndMatchNativeCore(bool oras)
    {
        const int burn = 0x10;
        var source = SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithPartyMember("Leader", p =>
        {
            p.Stat_HPCurrent = 7;
            p.Status_Condition = burn;
            // A stored stat Core would not calculate, so a recalculation would show.
            p.Stat_ATK = 1;
        }));
        var session = SaveFixtures.Open(source);
        var draft = session.Select(SlotRef.InParty(0));

        draft.EditLanguage(German);
        draft.EditTrainerFriendship(42);
        draft.HpChange.Should().BeNull("neither edit affects stats");
        session.Apply(draft);

        var output = SaveExporter.Export(session, session.Select(SlotRef.InParty(0)));
        var native = SaveFixtures.Parse(source);
        var pk = native.GetPartySlotAtIndex(0);
        pk.Language = German;
        pk.OriginalTrainerFriendship = 42;
        native.SetPartySlotAtIndex(pk, 0, EntityImportSettings.None);
        output.Should().Equal(native.Write().ToArray(), "the session must match the native Core edit");
        var applied = SaveFixtures.Parse(output).GetPartySlotAtIndex(0);
        applied.Nickname.Should().Be("Leader", "a nickname is kept across a language change");
        applied.Stat_HPCurrent.Should().Be(7);
        applied.Status_Condition.Should().Be(burn);
        applied.Stat_ATK.Should().Be(1, "neither edit recalculates the stored stats");
    }

    [Fact]
    public void BoxEditsApplyAndExportAsNativeCore()
    {
        var source = SaveFixtures.Synthetic(false, legal: false);
        var session = SaveFixtures.Open(source);
        var draft = session.Select(SaveFixtures.FirstBoxSlot);

        draft.TypeNickname("Quill");
        draft.EditLanguage(French);
        draft.EditTrainerFriendship(0);
        draft.EditHandlerFriendship(255);
        session.Apply(draft);

        var output = SaveExporter.Export(session, session.Select(SaveFixtures.FirstBoxSlot));
        var native = SaveFixtures.Parse(source);
        var pk = native.GetBoxSlotAtIndex(0, 0);
        pk.Nickname = "Quill";
        pk.IsNicknamed = true;
        pk.Language = French;
        pk.OriginalTrainerFriendship = 0;
        pk.HandlingTrainerFriendship = 255;
        native.SetBoxSlotAtIndex(pk, 0, 0, EntityImportSettings.None);
        output.Should().Equal(native.Write().ToArray());
    }
}
