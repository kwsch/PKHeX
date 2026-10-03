using FluentAssertions;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The tab's working state: what counts as unsaved work, leaving a session (replace, close, reset or discard), and what survives a component fault.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class WorkspaceStateTests
{
    private static SaveSession Open(bool oras = false) => SaveFixtures.Open(SaveFixtures.Synthetic(oras));

    /// <summary>A draft of the first box slot with a changed nickname.</summary>
    private static EditorDraft Dirty(SaveSession session, string nickname = "Dirty")
    {
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname(nickname, true);
        return draft;
    }

    /// <summary>A state with <paramref name="session"/> open and one change applied to it.</summary>
    private static async Task<WorkspaceState> WithAppliedChange(SaveSession session, string nickname = "Applied")
    {
        var state = SaveFixtures.NewState();
        state.Open(session);
        state.SetDraft(Dirty(session, nickname));
        await SaveFixtures.ApplyAsync(state);
        return state;
    }

    [Fact]
    public void StartsEmptyWithNothingToLose()
    {
        var state = SaveFixtures.NewState();
        state.Session.Should().BeNull();
        state.Exit.Should().BeNull();
        state.ExitStage.Should().Be(ExitStage.None);
        state.Draft.Should().BeNull();
        state.DraftValid.Should().BeTrue();
        state.HasUnsavedWork.Should().BeFalse();
    }

    [Fact]
    public void UnsavedWorkFollowsAppliedChangesDirtyDraftsAndRefusedEdits()
    {
        var state = SaveFixtures.NewState();
        var changes = 0;
        state.Changed += () => changes++;
        var session = Open();
        state.Open(session);
        state.HasUnsavedWork.Should().BeFalse("an unmodified save has nothing to lose");

        state.SetDraft(session.Select(SaveFixtures.FirstBoxSlot));
        state.HasUnsavedWork.Should().BeFalse("a clean draft has nothing to lose");
        state.Draft!.EditNickname("Draft", true);
        state.NotifyChanged();
        state.HasUnsavedWork.Should().BeTrue("the draft differs from its slot");

        state.SetDraftValid(false);
        state.HasUnsavedWork.Should().BeTrue("a refused edit no longer matches what the user typed");

        session.Apply(state.Draft);
        state.SetDraft(null);
        state.HasUnsavedWork.Should().BeTrue("an applied change is only in memory until downloaded");
        changes.Should().Be(5);
    }

    [Fact]
    public void AcceptOpensWhenNothingIsLostAndOtherwiseHoldsTheCandidate()
    {
        var state = SaveFixtures.NewState();
        var first = Open();
        state.Accept(SaveLoadOutcome.Opened(first)).Should().Be(OpenDisposition.Opened, "there is no session to lose");
        state.Session.Should().BeSameAs(first);

        var clean = Open();
        state.SetDraft(first.Select(SaveFixtures.FirstBoxSlot));
        state.Accept(SaveLoadOutcome.Opened(clean)).Should().Be(OpenDisposition.Opened, "a clean draft has nothing to lose");
        state.Session.Should().BeSameAs(clean);
        state.Draft.Should().BeNull();

        var draft = clean.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Dirty", true);
        state.SetDraft(draft);
        var held = Open(oras: true);
        state.Accept(SaveLoadOutcome.Opened(held)).Should().Be(OpenDisposition.Held, "the dirty draft would be lost");
        state.Session.Should().BeSameAs(clean);
        state.Draft.Should().BeSameAs(draft);
        state.Exit!.Candidate.Should().BeSameAs(held);
        state.ExitStage.Should().Be(ExitStage.ResolveDraft);

        var newer = Open();
        state.Accept(SaveLoadOutcome.Opened(newer)).Should().Be(OpenDisposition.Held);
        state.Exit!.Candidate.Should().BeSameAs(newer, "the latest successfully opened file replaces the earlier candidate");
    }

    public static TheoryData<LoadFailure> Failures => [.. Enum.GetValues<LoadFailure>()];

    [Theory]
    [MemberData(nameof(Failures))]
    public void RefusedOpenChangesNothing(LoadFailure failure)
    {
        var state = SaveFixtures.NewState();
        var session = Open();
        state.Open(session);
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Dirty", true);
        state.SetDraft(draft);
        state.SetDraftValid(false);
        var pending = Open(oras: true);
        state.RequestReplace(pending);
        var changes = 0;
        state.Changed += () => changes++;

        var recognized = new RecognizedSave(typeof(PKHeX.Core.SAV6XY), PKHeX.Core.GameVersion.X, 6);
        var outcome = failure switch
        {
            LoadFailure.RecognizedNotEnabled => SaveLoadOutcome.NotEnabled(recognized),
            LoadFailure.IntegrityFailed => SaveLoadOutcome.IntegrityFailed(recognized, IntegrityProblem.ChecksumsInvalid),
            _ => SaveLoadOutcome.Failed(failure),
        };
        state.Accept(outcome).Should().Be(OpenDisposition.Refused);
        state.Session.Should().BeSameAs(session);
        state.Draft.Should().BeSameAs(draft);
        state.DraftValid.Should().BeFalse();
        state.Exit!.Candidate.Should().BeSameAs(pending);
        changes.Should().Be(0);
    }

    [Fact]
    public void RecoveryKeepsTheAppliedSessionAndDropsHalfDoneWork()
    {
        var state = SaveFixtures.NewState();
        var session = Open();
        state.Open(session);
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Applied", true);
        session.Apply(draft);
        var revision = session.Revision;

        var dirty = session.Select(SaveFixtures.FirstBoxSlot);
        dirty.EditNickname("Unapplied", true);
        state.SetDraft(dirty);
        state.SetDraftValid(false);
        state.RequestReplace(Open(oras: true));

        state.RecoverAfterFault();
        state.Session.Should().BeSameAs(session);
        session.Revision.Should().Be(revision);
        state.Draft.Should().BeNull();
        state.Exit.Should().BeNull();
        state.DraftValid.Should().BeTrue();
        state.HasUnsavedWork.Should().BeTrue("the applied change is still only in memory");
        SaveFixtures.Open(SaveExporter.Export(session, null)).Select(SaveFixtures.FirstBoxSlot).Nickname.Should().Be("Applied", "the kept session still exports its applied state");
    }

    [Fact]
    public void DiscardClearsEverything()
    {
        var state = SaveFixtures.NewState();
        var session = Open();
        state.Open(session);
        state.SetDraft(Dirty(session));
        state.RequestReplace(Open(oras: true));

        state.Discard();
        state.Session.Should().BeNull();
        state.Exit.Should().BeNull();
        state.Draft.Should().BeNull();
        state.HasUnsavedWork.Should().BeFalse();
    }

    [Fact]
    public void BoxNavigationStartsAtTheInGameBoxAndWraps()
    {
        var state = SaveFixtures.NewState();
        var showNoSession = () => state.ShowBox(0);
        showNoSession.Should().Throw<InvalidOperationException>();

        var changes = 0;
        state.Changed += () => changes++;
        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: save => save.CurrentBox = 4)));
        state.CurrentBox.Should().Be(4);

        state.ShowBox(-1);
        state.CurrentBox.Should().Be(30, "previous from the first box is the last");
        state.ShowBox(31);
        state.CurrentBox.Should().Be(0, "next from the last box is the first");
        state.ShowBox(12);
        state.CurrentBox.Should().Be(12);
        changes.Should().Be(1, "navigation cannot affect unsaved work");
    }

    [Fact]
    public void BoxIsKeptOnRecoveryAndResetOnOpenOrDiscard()
    {
        var state = SaveFixtures.NewState();
        var session = Open();
        state.Open(session);
        state.ShowBox(9);
        state.SetDraft(session.Select(SaveFixtures.FirstBoxSlot));

        state.RecoverAfterFault();
        state.CurrentBox.Should().Be(9);
        state.Draft.Should().BeNull();

        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: save => save.CurrentBox = 2)));
        state.CurrentBox.Should().Be(2);

        state.Discard();
        state.CurrentBox.Should().Be(0);
    }

    [Fact]
    public void OpenSlotOpensReadableEntitiesAndClosesTheCleanDraftOtherwise()
    {
        var native = SaveFixtures.Parse(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember()));
        // Box 1, slot 2 holds a copy of slot 1 with one stored byte flipped, so it fails its checksum.
        var target = native.GetBoxSlotOffset(0, 1);
        native.Data.Slice(native.GetBoxSlotOffset(0, 0), native.SIZE_BOXSLOT).CopyTo(native.Data[target..]);
        native.Data[target + native.SIZE_BOXSLOT - 1] ^= 1;
        var state = SaveFixtures.NewState();
        var noSession = () => state.OpenSlot(SaveFixtures.FirstBoxSlot);
        noSession.Should().Throw<InvalidOperationException>();
        state.Open(SaveFixtures.Open(native.Write().ToArray()));

        state.OpenSlot(SlotRef.InParty(0)).Should().Be(SlotOpening.Opened);
        state.Draft!.Slot.Should().Be(SlotRef.InParty(0));
        state.OpenSlot(SlotRef.InBox(0, 2)).Should().Be(SlotOpening.Empty);
        state.Draft.Should().BeNull();
        state.OpenSlot(SaveFixtures.FirstBoxSlot).Should().Be(SlotOpening.Opened);
        state.OpenSlot(SlotRef.InBox(0, 1)).Should().Be(SlotOpening.Unreadable);
        state.Draft.Should().BeNull();
        state.OpenSlot(SlotRef.InParty(1)).Should().Be(SlotOpening.Empty);
    }

    [Fact]
    public void OpenSlotKeepsTheOpenDraftAndNeverReplacesUnappliedWork()
    {
        var state = SaveFixtures.NewState();
        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember())));
        state.OpenSlot(SaveFixtures.FirstBoxSlot).Should().Be(SlotOpening.Opened);
        var draft = state.Draft;

        state.OpenSlot(SaveFixtures.FirstBoxSlot).Should().Be(SlotOpening.AlreadyOpen);
        state.Draft.Should().BeSameAs(draft, "reopening the open slot keeps its draft and legality result");

        draft!.EditNickname("Unapplied", true);
        state.OpenSlot(SlotRef.InParty(0)).Should().Be(SlotOpening.DraftPending);
        state.OpenSlot(SlotRef.InBox(0, 2)).Should().Be(SlotOpening.DraftPending, "an empty slot must not close unapplied work either");
        state.Draft.Should().BeSameAs(draft);

        state.SetDraft(state.Session!.Select(SaveFixtures.FirstBoxSlot));
        state.SetDraftValid(false);
        state.OpenSlot(SlotRef.InParty(0)).Should().Be(SlotOpening.DraftPending);
        state.Draft!.Slot.Should().Be(SaveFixtures.FirstBoxSlot);
    }

    [Fact]
    public void LeavingAnUnchangedSessionNeedsNoConfirmation()
    {
        var state = SaveFixtures.NewState();
        var first = Open();
        state.Open(first);
        state.SetDraft(first.Select(SaveFixtures.FirstBoxSlot));
        var second = Open(oras: true);
        state.RequestReplace(second);
        state.Session.Should().BeSameAs(second, "a clean draft and an unchanged save have nothing to lose");
        state.Exit.Should().BeNull();

        state.RequestClose();
        state.Session.Should().BeNull();
        state.Exit.Should().BeNull();
        var closeNothing = () => state.RequestClose();
        closeNothing.Should().Throw<InvalidOperationException>();

        state.RequestReplace(first);
        state.Session.Should().BeSameAs(first, "with no session open the candidate simply opens");
    }

    [Fact]
    public async Task ReplaceResolvesTheDraftThenTheSessionThenTheDownload()
    {
        var session = Open();
        var state = SaveFixtures.NewState();
        state.Open(session);
        state.SetDraft(Dirty(session, "First"));
        var candidate = Open(oras: true);

        state.Accept(SaveLoadOutcome.Opened(candidate)).Should().Be(OpenDisposition.Held);
        state.Exit.Should().Be(SessionExit.Replace(candidate));
        state.ExitStage.Should().Be(ExitStage.ResolveDraft);
        var continueEarly = () => state.ConfirmExportChecked();
        continueEarly.Should().Throw<InvalidOperationException>("nothing was downloaded");
        var discardEarly = () => state.DiscardSessionForExit();
        discardEarly.Should().Throw<InvalidOperationException>("the draft step comes first");

        await SaveFixtures.ReadyToApply(state);
        state.ApplyDraftForExit();
        state.Session.Should().BeSameAs(session);
        session.Revision.Should().Be(1);
        state.DraftDirty.Should().BeFalse();
        state.ExitStage.Should().Be(ExitStage.ResolveSession, "the applied change is not downloaded");
        var applyAgain = () => state.ApplyDraftForExit();
        applyAgain.Should().Throw<InvalidOperationException>();
        continueEarly.Should().Throw<InvalidOperationException>("nothing was downloaded");

        session.MarkExported(session.Revision);
        state.NotifyChanged();
        state.ExitStage.Should().Be(ExitStage.ConfirmExport);
        state.Session.Should().BeSameAs(session, "a started download never ends the session by itself");

        state.ConfirmExportChecked();
        state.Session.Should().BeSameAs(candidate);
        state.Exit.Should().BeNull();
        state.Draft.Should().BeNull();
    }

    [Fact]
    public async Task AnApplyAfterTheDownloadTakesTheExitBackToTheSessionStep()
    {
        var session = Open();
        var state = await WithAppliedChange(session);
        state.RequestClose();
        state.ExitStage.Should().Be(ExitStage.ResolveSession);
        session.MarkExported(session.Revision);
        state.ExitStage.Should().Be(ExitStage.ConfirmExport);

        // Still in the exit, the user edits and applies again from the editor.
        state.SetDraft(Dirty(session, "Later"));
        state.ExitStage.Should().Be(ExitStage.ResolveDraft, "the new draft must be resolved first");
        await SaveFixtures.ApplyAsync(state);
        state.ExitStage.Should().Be(ExitStage.ResolveSession, "the download does not hold the later change");
        var confirm = () => state.ConfirmExportChecked();
        confirm.Should().Throw<InvalidOperationException>();
        state.Session.Should().BeSameAs(session);
    }

    [Fact]
    public async Task AnEarlierDownloadOfTheCurrentRevisionOnlyNeedsTheConfirmation()
    {
        var session = Open();
        var state = await WithAppliedChange(session);
        session.MarkExported(session.Revision);
        state.RequestClose();
        state.ExitStage.Should().Be(ExitStage.ConfirmExport);
        state.Session.Should().BeSameAs(session);
        state.ConfirmExportChecked();
        state.Session.Should().BeNull();
    }

    [Fact]
    public async Task DiscardingTheDraftKeepsTheSessionAndMovesOn()
    {
        var session = Open();
        var state = SaveFixtures.NewState();
        state.Open(session);
        state.SetDraft(Dirty(session));
        state.SetDraftValid(false);
        var candidate = Open(oras: true);
        state.RequestReplace(candidate);
        state.ExitStage.Should().Be(ExitStage.ResolveDraft);

        state.DiscardDraftForExit();
        state.Session.Should().BeSameAs(candidate, "with the draft gone nothing was left to lose");
        session.Revision.Should().Be(0);
        session.HasChangesSinceOpen.Should().BeFalse();

        var applied = Open();
        state = await WithAppliedChange(applied);
        state.SetDraft(Dirty(applied, "Unapplied"));
        state.RequestClose();
        state.DiscardDraftForExit();
        state.Draft.Should().BeNull();
        state.Session.Should().BeSameAs(applied);
        state.ExitStage.Should().Be(ExitStage.ResolveSession);
        state.DiscardSessionForExit();
        state.Session.Should().BeNull("discarding the session is the explicit destructive choice");
    }

    [Fact]
    public async Task CancelKeepsTheSessionDraftAndEditor()
    {
        var session = Open();
        var state = await WithAppliedChange(session);
        var draft = Dirty(session, "Kept");
        state.SetDraft(draft);
        state.RequestReplace(Open(oras: true));
        var changes = 0;
        state.Changed += () => changes++;

        state.CancelExit();
        state.Exit.Should().BeNull();
        state.ExitStage.Should().Be(ExitStage.None);
        state.Session.Should().BeSameAs(session);
        state.Draft.Should().BeSameAs(draft);
        session.Revision.Should().Be(1);
        changes.Should().Be(1);
    }

    [Fact]
    public async Task AFileOpenedDuringACloseTurnsItIntoAReplaceAtTheSameStep()
    {
        var session = Open();
        var state = await WithAppliedChange(session);
        state.RequestClose();
        session.MarkExported(session.Revision);
        state.ExitStage.Should().Be(ExitStage.ConfirmExport);

        var candidate = Open(oras: true);
        state.Accept(SaveLoadOutcome.Opened(candidate)).Should().Be(OpenDisposition.Held);
        state.Exit.Should().Be(SessionExit.Replace(candidate));
        state.ExitStage.Should().Be(ExitStage.ConfirmExport, "the step depends only on the open session");
        state.Accept(SaveLoadOutcome.Failed(LoadFailure.Unrecognized)).Should().Be(OpenDisposition.Refused);
        state.Exit!.Candidate.Should().BeSameAs(candidate);
    }

    [Fact]
    public void AnExitLeftWithNothingToLoseStillWaitsForTheUser()
    {
        var session = Open();
        var state = SaveFixtures.NewState();
        state.Open(session);
        state.SetDraft(Dirty(session));
        state.RequestClose();
        state.ExitStage.Should().Be(ExitStage.ResolveDraft);

        // Cancelling the draft in the editor must not close the save behind the user's back.
        state.SetDraft(session.Select(SaveFixtures.FirstBoxSlot));
        state.ExitStage.Should().Be(ExitStage.Ready);
        state.Session.Should().BeSameAs(session);

        var candidate = Open(oras: true);
        state.Accept(SaveLoadOutcome.Opened(candidate)).Should().Be(OpenDisposition.Opened, "nothing would be lost");
        state.Session.Should().BeSameAs(candidate);

        state.SetDraft(Dirty(candidate));
        state.RequestClose();
        state.SetDraft(null);
        state.ConfirmExportChecked();
        state.Session.Should().BeNull();
    }

    [Fact]
    public async Task ResetReopensAFreshCopyAndIsResolvedLikeAReplace()
    {
        var session = Open();
        var state = await WithAppliedChange(session);
        session.MarkExported(session.Revision);
        state.ShowBox(7);

        state.RequestReset();
        state.Exit!.Intent.Should().Be(ExitIntent.Reset);
        var fresh = state.Exit.Candidate!;
        fresh.Should().NotBeSameAs(session);
        fresh.GetOriginalBytes().Should().Equal(session.GetOriginalBytes());
        fresh.FileName.Should().Be(session.FileName);
        state.ExitStage.Should().Be(ExitStage.ConfirmExport, "the current revision was downloaded, so only the confirmation is asked");
        state.Session.Should().BeSameAs(session, "nothing is lost before the user confirms");

        state.ConfirmExportChecked();
        state.Session.Should().BeSameAs(fresh);
        state.Exit.Should().BeNull();
        state.Draft.Should().BeNull();
        fresh.Revision.Should().Be(0);
        fresh.HasChangesSinceOpen.Should().BeFalse();
        fresh.ExportStatus.Should().Be(ExportStatus.Unchanged, "the download status belongs to the old session");
        state.HasUnsavedWork.Should().BeFalse();
        state.CurrentBox.Should().Be(StorageView.InitialBox(fresh));
        SaveExporter.Export(fresh, null).Should().Equal(session.GetOriginalBytes(), "the reset session exports the file as it was opened");
        fresh.Select(SaveFixtures.FirstBoxSlot).Nickname.Should().NotBe("Applied");
    }

    [Fact]
    public async Task ResetResolvesTheDraftThenTheSessionAndCanBeCancelledAtEachStep()
    {
        var session = Open();
        var state = await WithAppliedChange(session);
        state.SetDraft(Dirty(session, "Draft"));
        state.RequestReset();
        state.ExitStage.Should().Be(ExitStage.ResolveDraft);
        state.CancelExit();
        state.Session.Should().BeSameAs(session);
        state.Draft!.Nickname.Should().Be("Draft");

        state.RequestReset();
        state.DiscardDraftForExit();
        state.ExitStage.Should().Be(ExitStage.ResolveSession, "the applied change is not downloaded");
        var confirm = () => state.ConfirmExportChecked();
        confirm.Should().Throw<InvalidOperationException>("nothing was downloaded");
        state.CancelExit();
        state.Session.Should().BeSameAs(session);
        session.Revision.Should().Be(1);

        state.RequestReset();
        state.DiscardSessionForExit();
        state.Session.Should().NotBeSameAs(session);
        state.Session!.HasChangesSinceOpen.Should().BeFalse();
    }

    [Fact]
    public void ResetOfAnUnchangedSessionCompletesAtOnce()
    {
        var session = Open();
        var state = SaveFixtures.NewState();
        state.Open(session);
        state.RequestReset();
        state.Exit.Should().BeNull();
        state.Session.Should().NotBeSameAs(session, "a fresh copy is opened");
        state.Session!.GetOriginalBytes().Should().Equal(session.GetOriginalBytes());

        var empty = SaveFixtures.NewState();
        var noSession = () => empty.RequestReset();
        noSession.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public async Task AFailedResetKeepsEverythingAsItWas()
    {
        var session = Open();
        var state = await WithAppliedChange(session);
        var draft = Dirty(session, "Kept");
        state.SetDraft(draft);
        var candidate = Open(oras: true);
        state.RequestReplace(candidate);
        state.Reopen = _ => SaveLoadOutcome.Failed(LoadFailure.ParserFault);
        var changes = 0;
        state.Changed += () => changes++;

        var reset = () => state.RequestReset();
        reset.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ResetFailed);
        state.Session.Should().BeSameAs(session);
        session.Revision.Should().Be(1);
        state.Draft.Should().BeSameAs(draft);
        state.Exit.Should().Be(SessionExit.Replace(candidate), "the exit already in progress is kept");
        changes.Should().Be(0);
    }

    [Fact]
    public void TheResetCandidateIsParsedFromAFreshCopyOfTheOriginalBytes()
    {
        var session = Open();
        var state = SaveFixtures.NewState();
        state.Open(session);
        byte[]? given = null;
        state.Reopen = s =>
        {
            given = s.GetOriginalBytes();
            given[0] ^= 0xFF;
            return SaveLoadOutcome.Failed(LoadFailure.Unrecognized);
        };
        var reset = () => state.RequestReset();
        reset.Should().Throw<SessionException>();
        given.Should().NotBeNull();
        session.GetOriginalBytes()[0].Should().NotBe(given![0], "the session's own original bytes are never handed out");
    }

    [Fact]
    public async Task DiscardAsksOnceAndOffersNoDraftOrDownloadStep()
    {
        var session = Open();
        var state = await WithAppliedChange(session);
        state.SetDraft(Dirty(session, "Draft"));

        state.RequestDiscard();
        state.Exit.Should().Be(SessionExit.Discard);
        state.ExitStage.Should().Be(ExitStage.ConfirmDiscard, "the draft and the session's changes are both at stake, and one confirmation covers them");
        var applyDraft = () => state.ApplyDraftForExit();
        applyDraft.Should().Throw<InvalidOperationException>();
        var discardDraft = () => state.DiscardDraftForExit();
        discardDraft.Should().Throw<InvalidOperationException>();
        var confirmExport = () => state.ConfirmExportChecked();
        confirmExport.Should().Throw<InvalidOperationException>();

        state.CancelExit();
        state.Session.Should().BeSameAs(session);
        state.Draft!.Nickname.Should().Be("Draft");

        state.RequestDiscard();
        state.DiscardSessionForExit();
        state.Session.Should().BeNull();
        state.Draft.Should().BeNull();
        state.Exit.Should().BeNull();
        state.HasUnsavedWork.Should().BeFalse();
    }

    [Fact]
    public async Task DiscardIsConfirmedEvenAfterADownload()
    {
        var session = Open();
        var state = await WithAppliedChange(session);
        session.MarkExported(session.Revision);
        state.RequestDiscard();
        state.ExitStage.Should().Be(ExitStage.ConfirmDiscard, "a started download is not known to be saved");
        state.DiscardSessionForExit();
        state.Session.Should().BeNull();
    }

    [Fact]
    public void DiscardWithNothingToLoseClosesAtOnce()
    {
        var state = SaveFixtures.NewState();
        var session = Open();
        state.Open(session);
        state.SetDraft(session.Select(SaveFixtures.FirstBoxSlot));
        state.RequestDiscard();
        state.Session.Should().BeNull();
        state.Exit.Should().BeNull();
        var noSession = () => state.RequestDiscard();
        noSession.Should().Throw<InvalidOperationException>();

        // Work that goes away during the confirmation leaves the exit waiting for the user, as any exit does.
        state.Open(session);
        state.SetDraft(Dirty(session));
        state.RequestDiscard();
        state.ExitStage.Should().Be(ExitStage.ConfirmDiscard);
        state.SetDraft(session.Select(SaveFixtures.FirstBoxSlot));
        state.ExitStage.Should().Be(ExitStage.Ready);
        state.Session.Should().BeSameAs(session);
        state.ConfirmExportChecked();
        state.Session.Should().BeNull();
    }

    [Fact]
    public void AFileOpenedDuringADiscardTurnsItIntoAReplace()
    {
        var session = Open();
        var state = SaveFixtures.NewState();
        state.Open(session);
        state.SetDraft(Dirty(session));
        state.RequestDiscard();
        var candidate = Open(oras: true);
        state.Accept(SaveLoadOutcome.Opened(candidate)).Should().Be(OpenDisposition.Held);
        state.Exit.Should().Be(SessionExit.Replace(candidate));
        state.ExitStage.Should().Be(ExitStage.ResolveDraft, "a replace offers to apply the draft and download first");
    }
}
