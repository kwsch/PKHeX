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

        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("WASM Proof", true);
        draft.IsDirty.Should().BeTrue();
        var exportDirty = () => SaveExporter.Export(session, draft);
        exportDirty.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.DraftUnapplied);

        draft = session.Select(draft.Slot);
        draft.IsDirty.Should().BeFalse();
        session.HasChangesSinceOpen.Should().BeFalse();

        draft.EditNickname("WASM Proof", true);
        session.Apply(draft);
        var output = SaveExporter.Export(session, session.Select(draft.Slot));
        var reloaded = SaveFixtures.Open(output).Select(SaveFixtures.FirstBoxSlot);
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
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
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
        var draft = SaveFixtures.Open(SaveFixtures.Synthetic(false)).Select(SaveFixtures.FirstBoxSlot);
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
        session.Select(SaveFixtures.FirstBoxSlot).Analyze(session).Verdict.Should().Be(legal ? "Valid" : "Invalid");
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
        var empty = () => session.Select(SlotRef.InBox(0, 2));
        empty.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.SlotNotOccupied);
        var corrupt = () => session.Select(SlotRef.InBox(0, 1));
        corrupt.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.EntityInvalid);
    }

    [Fact]
    public void RevisionAdvancesOnlyOnRealApply()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        session.Revision.Should().Be(0);

        session.Apply(session.Select(SaveFixtures.FirstBoxSlot));
        session.Revision.Should().Be(0);
        session.HasChangesSinceOpen.Should().BeFalse();

        var draft = session.Select(SaveFixtures.FirstBoxSlot);
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
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Changed", true);
        session.Apply(draft);

        expected.Nickname = "Changed";
        expected.IsNicknamed = true;
        expected.RefreshChecksum();
        session.Working.GetBoxSlotAtIndex(0).Data.ToArray().Should().Equal(expected.Data.ToArray());
    }

    [Fact]
    public void PreviewIsADetachedCopyOfTheDraft()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Drafted", true);

        var preview = draft.Preview();
        preview.Nickname.Should().Be("Drafted", "the preview shows unapplied edits");
        preview.Nickname = "Tampered";
        preview.HeldItem = 1;

        draft.Nickname.Should().Be("Drafted", "changing a preview must not change the draft");
        draft.Preview().HeldItem.Should().NotBe(1);
        draft.Inspect().Identity.Nickname.Should().Be("Drafted");
        draft.Inspect().Advanced.ChecksumValid.Should().BeTrue("the inspector shows the checksum an apply would store, not the stale in-memory one");
    }

    [Fact]
    public void ReturningToTheStoredNicknameKeepsItsEncodingAndIsClean()
    {
        // Bytes after the terminator are kept by the game; an edit that ends at the stored text must not rewrite them.
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithBoxEntity(0, 1, pk =>
        {
            pk.Nickname = "Ziggy";
            pk.IsNicknamed = true;
            pk.NicknameTrash[16] = 0x7F; // after "Ziggy" and its terminator; the final two bytes stay 0, as Core requires
        })));
        var slot = SlotRef.InBox(0, 1);
        var stored = session.Working.GetBoxSlotAtIndex(0, 1).NicknameTrash.ToArray();
        var draft = session.Select(slot);

        // A longer name overwrites the stored bytes after "Ziggy"; deleting back to "Ziggy" must bring the stored bytes back, not leave remnants.
        draft.EditNickname("Ziggy Longer", true);
        draft.IsDirty.Should().BeTrue();
        draft.EditNickname("Ziggy", true);

        draft.IsDirty.Should().BeFalse("the draft is byte for byte the stored entity again");
        draft.Preview().NicknameTrash.ToArray().Should().Equal(stored);

        draft.EditNickname("Ziggy", false);
        draft.IsDirty.Should().BeTrue();
        session.Apply(draft);
        session.Working.GetBoxSlotAtIndex(0, 1).NicknameTrash.ToArray().Should().Equal(stored, "a flag-only change keeps the stored name bytes");
    }

    [Fact]
    public void ARefusedEditLeavesTheDraftUnchanged()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Kept", true);
        var before = draft.Preview().Data.ToArray();

        var tooLong = () => draft.EditNickname(new string('A', draft.MaxNicknameLength + 1), false);
        tooLong.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.NicknameTooLong);
        var control = () => draft.EditNickname("A\u0007", false);
        control.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.NicknameInvalidCharacters);

        draft.Preview().Data.ToArray().Should().Equal(before);
    }

    [Fact]
    public void AppliedDraftIsStoredByteForByte()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true));
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Exact", true);
        var expected = draft.Preview();
        expected.RefreshChecksum();

        session.Apply(draft);

        var stored = session.Working.GetBoxSlotAtIndex(0);
        stored.Data[..expected.SIZE_STORED].ToArray().Should().Equal(expected.Data[..expected.SIZE_STORED].ToArray());
        SaveSession.StoresExactly(stored, expected).Should().BeTrue();
        var different = expected.Clone();
        different.HeldItem = 1;
        SaveSession.StoresExactly(stored, different).Should().BeFalse("any differing stored byte fails the read-back check");
    }

    [Fact]
    public void MarkExportedRecordsOnlyRevisionsOfTheSession()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        SaveExporter.Export(session, null);
        session.ExportedRevision.Should().BeNull("producing the bytes is not a download");

        session.MarkExported(session.Revision);
        session.ExportedRevision.Should().Be(0);

        var draft = session.Select(SaveFixtures.FirstBoxSlot);
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
    public void ExportStatusSeparatesAppliedChangesFromDownloads()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        session.ExportStatus.Should().Be(ExportStatus.Unchanged);
        session.MarkExported(session.Revision);
        session.ExportStatus.Should().Be(ExportStatus.Unchanged, "downloading an unchanged save changes nothing");

        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Changed", true);
        session.Apply(draft);
        session.ExportStatus.Should().Be(ExportStatus.ChangedSinceExport, "the earlier download holds the original");

        var fresh = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var noOp = fresh.Select(SaveFixtures.FirstBoxSlot);
        fresh.Apply(noOp);
        fresh.ExportStatus.Should().Be(ExportStatus.Unchanged, "a no-op apply is not a change");
        noOp.EditNickname("Changed", true);
        fresh.Apply(noOp);
        fresh.ExportStatus.Should().Be(ExportStatus.NotExported);

        fresh.MarkExported(fresh.Revision);
        fresh.ExportStatus.Should().Be(ExportStatus.ExportedCurrent);
        fresh.HasChangesSinceOpen.Should().BeTrue("the session still differs from the file it was opened from");

        var again = fresh.Select(SaveFixtures.FirstBoxSlot);
        again.EditNickname("Again", true);
        fresh.Apply(again);
        fresh.ExportStatus.Should().Be(ExportStatus.ChangedSinceExport);
        fresh.MarkExported(fresh.Revision);
        fresh.ExportStatus.Should().Be(ExportStatus.ExportedCurrent);
    }

    [Fact]
    public void StaleDraftIsRejectedUnlessClean()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var stale = session.Select(SaveFixtures.FirstBoxSlot);
        var current = session.Select(SaveFixtures.FirstBoxSlot);
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
        session.Select(SaveFixtures.FirstBoxSlot).Nickname.Should().Be("First");
    }

    [Fact]
    public void ForeignSessionDraftIsRejectedWithoutMutation()
    {
        var owner = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var other = SaveFixtures.Open(SaveFixtures.Synthetic(false, legal: false));
        other.SessionId.Should().NotBe(owner.SessionId);
        var foreign = owner.Select(SaveFixtures.FirstBoxSlot);
        foreign.EditNickname("Foreign", true);
        var working = other.Working;
        var nickname = other.Select(SaveFixtures.FirstBoxSlot).Nickname;

        var applyForeign = () => other.Apply(foreign);
        applyForeign.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ForeignDraft);
        var analyzeForeign = () => foreign.Analyze(other);
        analyzeForeign.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ForeignDraft);
        var exportForeign = () => SaveExporter.Export(other, owner.Select(SaveFixtures.FirstBoxSlot));
        exportForeign.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ForeignDraft);
        other.Working.Should().BeSameAs(working);
        other.Revision.Should().Be(0);
        other.HasChangesSinceOpen.Should().BeFalse();
        other.Select(SaveFixtures.FirstBoxSlot).Nickname.Should().Be(nickname);
    }

    [Fact]
    public void PartySelectClonesThePartyMemberAndAnalysesItAsParty()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember("Leader")));
        var draft = session.Select(SlotRef.InParty(0));

        draft.Slot.Should().Be(SlotRef.InParty(0));
        draft.Nickname.Should().Be("Leader", "the party member, not the boxed copy, is opened");
        draft.CanApply.Should().BeFalse();
        draft.Editable.Should().Be(EditableFields.None, "nothing is offered for editing that could never be applied");
        var boxed = session.Select(SaveFixtures.FirstBoxSlot);
        boxed.CanApply.Should().BeTrue();
        boxed.Editable.Should().Be(EditableFields.Nickname);
        draft.Inspect().Stats.Source.Should().Be(StatsSource.Stored, "a party member's stored stats are shown");
        boxed.Inspect().Stats.Source.Should().Be(StatsSource.Calculated);

        var native = new LegalityAnalysis(session.Working.GetPartySlotAtIndex(0), session.Working.Personal, StorageSlotType.Party);
        var result = draft.Analyze(session);
        result.Verdict.Should().Be(ProofPage.Verdict(native));
        result.Report.Should().Be(native.Report());
    }

    [Fact]
    public void PartyApplyIsRefusedWithoutChangingTheSession()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember()));
        var working = session.Working;
        var before = SaveExporter.Export(session, null);
        var draft = session.Select(SlotRef.InParty(0));
        draft.EditNickname("Changed", true);

        var apply = () => session.Apply(draft);

        apply.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.PartyApplyNotAvailable);
        session.Working.Should().BeSameAs(working);
        session.Revision.Should().Be(0);
        session.HasChangesSinceOpen.Should().BeFalse();
        SaveExporter.Export(session, null).Should().Equal(before);
    }

    [Fact]
    public void SelectRefusesPositionsOutsideTheSave()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember()));
        foreach (var slot in new[] { SlotRef.InParty(1), SlotRef.InParty(6), SlotRef.InBox(31, 0), SlotRef.InBox(0, 30) })
        {
            var select = () => session.Select(slot);
            select.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.SlotNotOccupied);
        }
    }

    [Fact]
    public void SelectReadsTheCurrentRevision()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Current", true);
        session.Apply(draft);

        var reopened = session.Select(SaveFixtures.FirstBoxSlot);
        reopened.Nickname.Should().Be("Current");
        reopened.SourceRevision.Should().Be(1);
    }
}
