using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Session, draft and export rules, run on synthetic saves.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SessionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SyntheticDraftCancelApplyAndRoundTrip(bool oras)
    {
        var source = SaveFixtures.Synthetic(oras);
        var before = source.ToArray();
        var session = SaveFixtures.Open(source);
        source.Should().Equal(before, "parsing must not mutate caller bytes");
        session.GetOriginalBytes().Should().Equal(before);

        var draft = session.Select(0);
        draft.EditNickname("WASM Proof", true);
        draft.IsDirty.Should().BeTrue();
        var exportDirty = () => SaveExporter.Export(session, draft);
        exportDirty.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.DraftUnapplied);

        draft = session.Select(draft.SlotIndex);
        draft.IsDirty.Should().BeFalse();
        session.HasChangesSinceOpen.Should().BeFalse();

        draft.EditNickname("WASM Proof", true);
        session.Apply(draft);
        var output = SaveExporter.Export(session, session.Select(draft.SlotIndex));
        var reloaded = SaveFixtures.Open(output).Select(0);
        reloaded.Nickname.Should().Be("WASM Proof");
        reloaded.IsNicknamed.Should().BeTrue();
        source.Should().Equal(before);

        var native = SaveFixtures.Parse(before);
        var pk = native.GetBoxSlotAtIndex(0);
        pk.Nickname = "WASM Proof";
        pk.IsNicknamed = true;
        native.SetBoxSlotAtIndex(pk, 0, EntityImportSettings.None);
        output.Should().Equal(native.Write().ToArray(), "the session must match the native Core edit");
    }

    [Fact]
    public void RejectsInvalidDraftWithoutMutation()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var draft = session.Select(0);
        var editTooLong = () => draft.EditNickname(new string('a', 13), true);
        editTooLong.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.NicknameTooLong);
        draft.IsDirty.Should().BeFalse();
        session.HasChangesSinceOpen.Should().BeFalse();
    }

    [Theory]
    [InlineData("Bad\nName", SessionError.NicknameInvalidCharacters)]
    [InlineData("\u0007", SessionError.NicknameInvalidCharacters)]
    [InlineData("\uE08E", SessionError.NicknameNotRepresentable)] // Stored as-is but read back as '♂', so the text would change.
    public void RejectsNicknameThatCannotBeStoredUnchanged(string nickname, SessionError expected)
    {
        var draft = SaveFixtures.Open(SaveFixtures.Synthetic(false)).Select(0);
        var before = draft.Nickname;
        var act = () => draft.EditNickname(nickname, true);
        act.Should().Throw<SessionException>().Which.Error.Should().Be(expected);
        draft.Nickname.Should().Be(before);
        draft.IsDirty.Should().BeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void KnownEntityLegalityIsPreserved(bool legal)
    {
        var native = new LegalityAnalysis(new PK6(SaveFixtures.ReadEntity(legal)));
        native.Parsed.Should().BeTrue();
        native.Valid.Should().Be(legal);

        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true, legal));
        session.Select(0).Analyze(session).Verdict.Should().Be(legal ? "Valid" : "Invalid");
    }

    [Fact]
    public void SelectRefusesEmptySlotsAndBrokenEntities()
    {
        var native = SaveFixtures.Parse(SaveFixtures.Synthetic(false));
        // Copy slot 0 into slot 1 and flip its last stored byte: the entity still reads a species, but its checksum fails.
        var source = native.GetBoxSlotOffset(0, 0);
        var target = native.GetBoxSlotOffset(0, 1);
        native.Data.Slice(source, native.SIZE_BOXSLOT).CopyTo(native.Data[target..]);
        native.Data[target + native.SIZE_BOXSLOT - 1] ^= 1;
        var broken = native.GetBoxSlotAtIndex(0, 1);
        broken.Species.Should().NotBe(0);
        broken.ChecksumValid.Should().BeFalse();

        var session = SaveFixtures.Open(native.Write().ToArray());
        var empty = () => session.Select(2);
        empty.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.SlotNotOccupied);
        var corrupt = () => session.Select(1);
        corrupt.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.EntityChecksumInvalid);
    }

    [Fact]
    public void RevisionAdvancesOnlyOnRealApply()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        session.Revision.Should().Be(0);

        session.Apply(session.Select(0));
        session.Revision.Should().Be(0);
        session.HasChangesSinceOpen.Should().BeFalse();

        var draft = session.Select(0);
        draft.EditNickname("Changed", true);
        session.Apply(draft);
        session.Revision.Should().Be(1);
        session.HasChangesSinceOpen.Should().BeTrue();
    }

    [Fact]
    public void ApplyWritesOnlyEditedFields()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var expected = (PK6)session.Working.GetBoxSlotAtIndex(0);
        var draft = session.Select(0);
        draft.EditNickname("Changed", true);
        session.Apply(draft);

        expected.Nickname = "Changed";
        expected.IsNicknamed = true;
        expected.RefreshChecksum();
        session.Working.GetBoxSlotAtIndex(0).Data.ToArray().Should().Equal(expected.Data.ToArray());
    }

    [Fact]
    public void MarkExportedRecordsOnlyRevisionsOfTheSession()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        SaveExporter.Export(session, null);
        session.ExportedRevision.Should().BeNull("producing the bytes is not a download");

        session.MarkExported(session.Revision);
        session.ExportedRevision.Should().Be(0);

        var draft = session.Select(0);
        draft.EditNickname("Changed", true);
        session.Apply(draft);
        session.ExportedRevision.Should().Be(0);
        session.MarkExported(session.Revision);
        session.ExportedRevision.Should().Be(1);

        var markFuture = () => session.MarkExported(2);
        markFuture.Should().Throw<ArgumentOutOfRangeException>();
        var markNegative = () => session.MarkExported(-1);
        markNegative.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void StaleDraftIsRejectedUnlessClean()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var stale = session.Select(0);
        var current = session.Select(0);
        current.EditNickname("First", true);
        session.Apply(current);
        var working = session.Working;

        var exportStale = () => SaveExporter.Export(session, stale);
        exportStale.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.StaleDraft);
        var analyzeStale = () => stale.Analyze(session);
        analyzeStale.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.StaleDraft);
        session.Apply(stale);
        session.Revision.Should().Be(1, "a clean stale draft has nothing to apply");

        stale.EditNickname("Second", true);
        var applyStale = () => session.Apply(stale);
        applyStale.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.StaleDraft);
        session.Working.Should().BeSameAs(working);
        session.Revision.Should().Be(1);
        session.Select(0).Nickname.Should().Be("First");
    }

    [Fact]
    public void ForeignSessionDraftIsRejectedWithoutMutation()
    {
        var owner = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var other = SaveFixtures.Open(SaveFixtures.Synthetic(false, legal: false));
        other.SessionId.Should().NotBe(owner.SessionId);
        var foreign = owner.Select(0);
        foreign.EditNickname("Foreign", true);
        var working = other.Working;
        var nickname = other.Select(0).Nickname;

        var applyForeign = () => other.Apply(foreign);
        applyForeign.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ForeignDraft);
        var analyzeForeign = () => foreign.Analyze(other);
        analyzeForeign.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ForeignDraft);
        var exportForeign = () => SaveExporter.Export(other, owner.Select(0));
        exportForeign.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ForeignDraft);
        other.Working.Should().BeSameAs(working);
        other.Revision.Should().Be(0);
        other.HasChangesSinceOpen.Should().BeFalse();
        other.Select(0).Nickname.Should().Be(nickname);
    }
}
