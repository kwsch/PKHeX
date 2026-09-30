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
    private static SaveSession Open(bool oras = false) => SaveLoader.Load(SaveFixtures.Synthetic(oras));

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
        SaveLoader.Load(SaveExporter.Export(session, null)).Select(0).Nickname.Should().Be("Applied", "the kept session still exports its applied state");
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
