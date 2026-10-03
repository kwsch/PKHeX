using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Legality findings are warnings with explicit acknowledgement, never passed over silently (WEB-LEGAL-003, PKHeX.Web.md §MVP): a changed draft
/// is applied only with a current result, an Invalid or Unavailable one only once acknowledged, and the download that contains such a change
/// asks again. Entities that were already illegal and were not changed are exported without asking.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class LegalityAcknowledgementTests
{
    /// <summary>Opens a synthetic save with its known entity as the draft: Invalid in X/Y (not its trainer's own), Valid in Alpha Sapphire.</summary>
    private static WorkspaceState OpenDraft(bool oras, WorkspaceState? state = null)
    {
        state ??= SaveFixtures.NewState();
        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(oras)));
        state.OpenSlot(SaveFixtures.FirstBoxSlot).Should().Be(SlotOpening.Opened);
        return state;
    }

    /// <summary>Makes an accepted edit through the same path as the editor.</summary>
    private static void Edit(WorkspaceState state, Action<EditorDraft> edit)
    {
        edit(state.Draft!);
        state.SetDraftValid(true);
    }

    /// <summary>Asserts that <see cref="WorkspaceState.ApplyDraft"/> is refused with <paramref name="error"/> and changes nothing.</summary>
    private static void ApplyIsRefused(WorkspaceState state, SessionError error)
    {
        var session = state.Session!;
        var draft = state.Draft;
        var revision = session.Revision;
        var apply = state.ApplyDraft;
        apply.Should().Throw<SessionException>().Which.Error.Should().Be(error);
        session.Revision.Should().Be(revision);
        session.FlaggedChanges.Should().BeEmpty();
        state.Draft.Should().BeSameAs(draft);
        state.DraftDirty.Should().BeTrue();
    }

    [Fact]
    public async Task AChangedDraftWaitsForAResultOfItAsItIsNow()
    {
        var state = OpenDraft(oras: true);
        Edit(state, d => d.EditNickname("Waiting", true));
        state.Legality.Status.Should().Be(LegalityStatus.Pending, "it waits for the idle delay");
        state.Legality.Gate.Should().Be(LegalityGate.Waiting);
        ApplyIsRefused(state, SessionError.LegalityNotCurrent);

        await state.Legality.RunNowAsync();
        state.Legality.Gate.Should().Be(LegalityGate.Clear);
        Edit(state, d => d.EditNickname("Stale", true));
        state.Legality.Status.Should().Be(LegalityStatus.Stale);
        state.Legality.Gate.Should().Be(LegalityGate.Waiting, "the Valid result was for an earlier edit");
        ApplyIsRefused(state, SessionError.LegalityNotCurrent);

        // A refused edit leaves the draft not matching what was typed, so there is nothing to apply it with.
        await state.Legality.RunNowAsync();
        state.SetDraftValid(false);
        state.Legality.Gate.Should().Be(LegalityGate.Waiting);
        ApplyIsRefused(state, SessionError.LegalityNotCurrent);
    }

    [Fact]
    public async Task AValidResultAppliesWithoutAskingAndFlagsNothing()
    {
        var state = OpenDraft(oras: true);
        Edit(state, d => d.EditNickname("Valid", true));
        await state.Legality.RunNowAsync();
        state.Legality.Gate.Should().Be(LegalityGate.Clear);
        var acknowledge = () => state.AcknowledgeLegality(true);
        acknowledge.Should().Throw<InvalidOperationException>("a Valid result has nothing to acknowledge");

        state.ApplyDraft();
        state.Session!.Revision.Should().Be(1);
        state.Session.FlaggedChanges.Should().BeEmpty();
        state.Session.ExportNeedsAcknowledgement.Should().BeFalse();
        SaveExporter.Export(state.Session, state.Draft).Should().NotBeEmpty();
    }

    [Fact]
    public async Task AnInvalidResultAppliesOnlyOnceAcknowledgedAndIsFlagged()
    {
        var state = OpenDraft(oras: false);
        Edit(state, d => d.EditNickname("Invalid", true));
        await state.Legality.RunNowAsync();
        state.Legality.Status.Should().Be(LegalityStatus.Invalid);
        state.Legality.Gate.Should().Be(LegalityGate.NeedsAcknowledgement);
        ApplyIsRefused(state, SessionError.LegalityNotAcknowledged);

        var changes = 0;
        state.Changed += () => changes++;
        state.AcknowledgeLegality(true);
        state.Legality.Gate.Should().Be(LegalityGate.Acknowledged);
        state.AcknowledgeLegality(false);
        state.Legality.Gate.Should().Be(LegalityGate.NeedsAcknowledgement, "the acknowledgement can be withdrawn");
        ApplyIsRefused(state, SessionError.LegalityNotAcknowledged);
        state.AcknowledgeLegality(true);
        changes.Should().Be(3);

        state.ApplyDraft();
        var session = state.Session!;
        session.Revision.Should().Be(1);
        session.FlaggedChanges.Should().Equal(new Dictionary<SlotRef, LegalityVerdict> { [SaveFixtures.FirstBoxSlot] = LegalityVerdict.Invalid });
        state.Legality.Gate.Should().Be(LegalityGate.Waiting, "the reopened slot is a new draft, and the acknowledgement does not carry over");
    }

    [Fact]
    public async Task AnyEditWithdrawsTheAcknowledgementEvenBackToTheAcknowledgedValues()
    {
        var state = OpenDraft(oras: false);
        Edit(state, d => d.EditNickname("First", true));
        await state.Legality.RunNowAsync();
        state.AcknowledgeLegality(true);

        Edit(state, d => d.EditNickname("Second", true));
        state.Legality.Gate.Should().Be(LegalityGate.Waiting);
        Edit(state, d => d.EditNickname("First", true));
        await state.Legality.RunNowAsync();
        state.Legality.Status.Should().Be(LegalityStatus.Invalid);
        state.Legality.Gate.Should().Be(LegalityGate.NeedsAcknowledgement, "the user acknowledged an earlier result, not this one");
        ApplyIsRefused(state, SessionError.LegalityNotAcknowledged);

        // A new draft forgets it too.
        state.AcknowledgeLegality(true);
        state.SetDraft(state.Session!.Select(SaveFixtures.FirstBoxSlot));
        Edit(state, d => d.EditNickname("First", true));
        await state.Legality.RunNowAsync();
        state.Legality.Gate.Should().Be(LegalityGate.NeedsAcknowledgement);
    }

    [Fact]
    public async Task AnUnavailableResultIsAcknowledgedAndFlaggedLikeAnInvalidOne()
    {
        var state = new WorkspaceState(new FakeTimeProvider(), new LegalityService((_, _, _) => throw new InvalidOperationException("detail")));
        OpenDraft(oras: true, state);
        Edit(state, d => d.EditNickname("Unknown", true));
        await state.Legality.RunNowAsync();
        state.Legality.Status.Should().Be(LegalityStatus.Unavailable);
        state.Legality.Gate.Should().Be(LegalityGate.NeedsAcknowledgement, "an analysis that could not complete is never treated as legal");
        ApplyIsRefused(state, SessionError.LegalityNotAcknowledged);

        state.AcknowledgeLegality(true);
        state.ApplyDraft();
        state.Session!.FlaggedChanges.Should().Equal(new Dictionary<SlotRef, LegalityVerdict> { [SaveFixtures.FirstBoxSlot] = LegalityVerdict.Unavailable });
    }

    [Fact]
    public void AnAcknowledgementNeedsAResultToAcknowledge()
    {
        var state = SaveFixtures.NewState();
        var noDraft = () => state.AcknowledgeLegality(true);
        noDraft.Should().Throw<InvalidOperationException>();

        OpenDraft(oras: false, state);
        Edit(state, d => d.EditNickname("Pending", true));
        var pending = () => state.AcknowledgeLegality(true);
        pending.Should().Throw<InvalidOperationException>("no result describes the draft yet");
        state.AcknowledgeLegality(false);
        state.Legality.Gate.Should().Be(LegalityGate.Waiting, "withdrawing is always allowed");
    }

    [Fact]
    public void ACleanDraftWritesNothingSoNeedsNoResult()
    {
        var state = OpenDraft(oras: false);
        state.Legality.Gate.Should().Be(LegalityGate.Waiting);
        state.ApplyDraft();
        state.Session!.Revision.Should().Be(0);
        state.Session.FlaggedChanges.Should().BeEmpty();
    }

    [Fact]
    public async Task ALaterValidApplyOfTheSameSlotClearsItsFlag()
    {
        var state = OpenDraft(oras: true);
        // Level 1 is below the Zigzagoon's met level, so the legal entity becomes Invalid.
        Edit(state, d => d.EditLevel(1));
        await state.Legality.RunNowAsync();
        state.Legality.Status.Should().Be(LegalityStatus.Invalid);
        state.AcknowledgeLegality(true);
        state.ApplyDraft();
        var session = state.Session!;
        session.FlaggedChanges.Should().ContainKey(SaveFixtures.FirstBoxSlot);

        var original = SaveFixtures.Open(SaveFixtures.Synthetic(true)).Select(SaveFixtures.FirstBoxSlot).Level;
        Edit(state, d => d.EditLevel(original));
        await state.Legality.RunNowAsync();
        state.Legality.Status.Should().Be(LegalityStatus.Valid);
        state.ApplyDraft();
        session.Revision.Should().Be(2);
        session.FlaggedChanges.Should().BeEmpty("the slot's latest change is Valid");
        session.ExportNeedsAcknowledgement.Should().BeFalse();
    }

    [Fact]
    public async Task ADownloadOfFlaggedChangesIsAcknowledgedForItsRevision()
    {
        var state = OpenDraft(oras: false);
        var session = state.Session!;
        session.ExportNeedsAcknowledgement.Should().BeFalse("the Invalid entity was not changed, so it is exported as it was");
        SaveExporter.Export(session, state.Draft).Should().Equal(session.GetOriginalBytes());
        var acknowledgeNothing = () => session.AcknowledgeExport(true);
        acknowledgeNothing.Should().Throw<InvalidOperationException>();

        Edit(state, d => d.EditNickname("Flagged", true));
        await SaveFixtures.ApplyAsync(state);
        session.ExportNeedsAcknowledgement.Should().BeTrue();
        var export = () => SaveExporter.Export(session, state.Draft);
        export.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ExportNotAcknowledged);

        var changes = 0;
        state.Changed += () => changes++;
        state.AcknowledgeExport(true);
        changes.Should().Be(1);
        session.ExportAcknowledgedRevision.Should().Be(1);
        session.ExportNeedsAcknowledgement.Should().BeFalse();
        SaveFixtures.Open(export()).Select(SaveFixtures.FirstBoxSlot).Nickname.Should().Be("Flagged");
        state.AcknowledgeExport(false);
        session.ExportNeedsAcknowledgement.Should().BeTrue("the acknowledgement can be withdrawn");
        state.AcknowledgeExport(true);

        // Another apply makes a download the user has not seen listed, so it asks again, even for a Valid change.
        Edit(state, d => d.EditTrainerFriendship(200));
        await SaveFixtures.ApplyAsync(state);
        session.Revision.Should().Be(2);
        session.ExportNeedsAcknowledgement.Should().BeTrue();
        export.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ExportNotAcknowledged);
    }

    [Fact]
    public async Task AResetSessionHasNoFlaggedChanges()
    {
        var state = OpenDraft(oras: false);
        Edit(state, d => d.EditNickname("Flagged", true));
        await SaveFixtures.ApplyAsync(state);
        state.RequestReset();
        state.DiscardSessionForExit();
        state.Session!.Revision.Should().Be(0);
        state.Session.FlaggedChanges.Should().BeEmpty();
        state.Session.ExportNeedsAcknowledgement.Should().BeFalse();
    }
}
