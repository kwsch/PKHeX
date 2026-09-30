using FluentAssertions;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The tab's working state: what counts as unsaved work, replacement, and what survives a component fault.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class WorkspaceStateTests
{
    private static SaveSession Open(bool oras = false) => SaveFixtures.Open(SaveFixtures.Synthetic(oras));

    [Fact]
    public void StartsEmptyWithNothingToLose()
    {
        var state = new WorkspaceState();
        state.Session.Should().BeNull();
        state.Pending.Should().BeNull();
        state.Draft.Should().BeNull();
        state.DraftValid.Should().BeTrue();
        state.HasUnsavedWork.Should().BeFalse();
    }

    [Fact]
    public void UnsavedWorkFollowsAppliedChangesDirtyDraftsAndRefusedEdits()
    {
        var state = new WorkspaceState();
        var changes = 0;
        state.Changed += () => changes++;
        var session = Open();
        state.Open(session);
        state.HasUnsavedWork.Should().BeFalse("an unmodified save has nothing to lose");

        state.SetDraft(session.Select(0));
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
    public void ReplacementIsHeldUntilConfirmedOrCancelled()
    {
        var state = new WorkspaceState();
        var first = Open();
        state.Open(first);
        state.SetDraft(first.Select(0));

        var second = Open(oras: true);
        state.OfferReplacement(second);
        state.Session.Should().BeSameAs(first);
        state.Draft.Should().NotBeNull("offering a replacement leaves the current draft alone");
        state.CancelReplace();
        state.Pending.Should().BeNull();
        state.Session.Should().BeSameAs(first);

        state.OfferReplacement(second);
        state.ConfirmReplace();
        state.Session.Should().BeSameAs(second);
        state.Pending.Should().BeNull();
        state.Draft.Should().BeNull("a draft belongs to the session it was taken from");
    }

    [Fact]
    public void AcceptOpensWhenNothingIsLostAndOtherwiseHoldsTheCandidate()
    {
        var state = new WorkspaceState();
        var first = Open();
        state.Accept(SaveLoadOutcome.Opened(first)).Should().Be(OpenDisposition.Opened, "there is no session to lose");
        state.Session.Should().BeSameAs(first);

        var clean = Open();
        state.SetDraft(first.Select(0));
        state.Accept(SaveLoadOutcome.Opened(clean)).Should().Be(OpenDisposition.Opened, "a clean draft has nothing to lose");
        state.Session.Should().BeSameAs(clean);
        state.Draft.Should().BeNull();

        var draft = clean.Select(0);
        draft.EditNickname("Dirty", true);
        state.SetDraft(draft);
        var held = Open(oras: true);
        state.Accept(SaveLoadOutcome.Opened(held)).Should().Be(OpenDisposition.Held, "the dirty draft would be lost");
        state.Session.Should().BeSameAs(clean);
        state.Draft.Should().BeSameAs(draft);
        state.Pending.Should().BeSameAs(held);

        var newer = Open();
        state.Accept(SaveLoadOutcome.Opened(newer)).Should().Be(OpenDisposition.Held);
        state.Pending.Should().BeSameAs(newer, "the latest successfully opened file replaces the earlier candidate");
    }

    public static TheoryData<LoadFailure> Failures => [.. Enum.GetValues<LoadFailure>()];

    [Theory]
    [MemberData(nameof(Failures))]
    public void RefusedOpenChangesNothing(LoadFailure failure)
    {
        var state = new WorkspaceState();
        var session = Open();
        state.Open(session);
        var draft = session.Select(0);
        draft.EditNickname("Dirty", true);
        state.SetDraft(draft);
        state.SetDraftValid(false);
        var pending = Open(oras: true);
        state.OfferReplacement(pending);
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
        state.Pending.Should().BeSameAs(pending);
        changes.Should().Be(0);
    }

    [Fact]
    public void RecoveryKeepsTheAppliedSessionAndDropsHalfDoneWork()
    {
        var state = new WorkspaceState();
        var session = Open();
        state.Open(session);
        var draft = session.Select(0);
        draft.EditNickname("Applied", true);
        session.Apply(draft);
        var revision = session.Revision;

        var dirty = session.Select(0);
        dirty.EditNickname("Unapplied", true);
        state.SetDraft(dirty);
        state.SetDraftValid(false);
        state.OfferReplacement(Open(oras: true));

        state.RecoverAfterFault();
        state.Session.Should().BeSameAs(session);
        session.Revision.Should().Be(revision);
        state.Draft.Should().BeNull();
        state.Pending.Should().BeNull();
        state.DraftValid.Should().BeTrue();
        state.HasUnsavedWork.Should().BeTrue("the applied change is still only in memory");
        SaveFixtures.Open(SaveExporter.Export(session, null)).Select(0).Nickname.Should().Be("Applied", "the kept session still exports its applied state");
    }

    [Fact]
    public void DiscardClearsEverything()
    {
        var state = new WorkspaceState();
        var session = Open();
        state.Open(session);
        state.SetDraft(session.Select(0));
        state.OfferReplacement(Open(oras: true));

        state.Discard();
        state.Session.Should().BeNull();
        state.Pending.Should().BeNull();
        state.Draft.Should().BeNull();
        state.HasUnsavedWork.Should().BeFalse();
    }
}
