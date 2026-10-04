using FluentAssertions;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Why Apply and Download cannot act: every reason, in the order the error summary lists them, and none when they can. The
/// buttons' state and their guards come from here, so the summary never disagrees with what a click does.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class ActionReadinessTests
{
    private static readonly FieldRefusal Refused = new("level", SessionError.LevelOutOfRange);

    [Fact]
    public void AChangedDraftWithAClearResultCanBeApplied()
    {
        ActionReadiness.ForApply(false, true, true, null, false, LegalityGate.Clear).Should().BeEmpty();
        ActionReadiness.ForApply(false, true, true, null, false, LegalityGate.Acknowledged).Should().BeEmpty();
    }

    [Theory]
    [InlineData(LegalityGate.Waiting, ApplyBlocker.LegalityWaiting)]
    [InlineData(LegalityGate.NeedsAcknowledgement, ApplyBlocker.LegalityNotAcknowledged)]
    public void LegalityHoldsBackAChangedDraft(LegalityGate gate, ApplyBlocker expected)
    {
        ActionReadiness.ForApply(false, true, true, null, false, gate).Should().Equal(expected);
    }

    [Fact]
    public void AnUnchangedDraftHasNothingToApplyAndNoResultToWaitFor()
    {
        ActionReadiness.ForApply(false, true, false, null, false, LegalityGate.Waiting).Should().Equal(ApplyBlocker.NoChanges);
    }

    [Fact]
    public void ARefusedEditIsTheReasonNotLegality()
    {
        // A refused draft has no result to wait for, and whether it differs from its slot is not what holds it back.
        ActionReadiness.ForApply(false, true, true, Refused, false, LegalityGate.Waiting).Should().Equal(ApplyBlocker.FieldRefused);
        ActionReadiness.ForApply(false, true, false, Refused, false, LegalityGate.Waiting).Should().Equal(ApplyBlocker.FieldRefused);
    }

    [Fact]
    public void APendingPreviewIsListedWithLegality()
    {
        ActionReadiness.ForApply(false, true, true, null, true, LegalityGate.NeedsAcknowledgement)
            .Should().Equal(ApplyBlocker.SpeciesPreviewPending, ApplyBlocker.LegalityNotAcknowledged);
        ActionReadiness.ForApply(false, true, false, null, true, LegalityGate.Waiting)
            .Should().Equal(ApplyBlocker.NoChanges, ApplyBlocker.SpeciesPreviewPending);
    }

    [Fact]
    public void AnUnwritablePositionIsTheOnlyReasonBesidesBusy()
    {
        ActionReadiness.ForApply(true, false, true, Refused, true, LegalityGate.Waiting).Should().Equal(ApplyBlocker.Busy, ApplyBlocker.NotWritable);
        ActionReadiness.ForApply(false, false, false, null, false, LegalityGate.Clear).Should().Equal(ApplyBlocker.NotWritable);
    }

    [Fact]
    public void BusyComesFirst()
    {
        ActionReadiness.ForApply(true, true, true, null, false, LegalityGate.Clear).Should().Equal(ApplyBlocker.Busy);
    }

    [Fact]
    public void ACleanSessionCanBeDownloaded()
    {
        ActionReadiness.ForDownload(false, false, null, false, false).Should().BeEmpty();
    }

    [Fact]
    public void EveryDownloadReasonIsListedInOrder()
    {
        ActionReadiness.ForDownload(true, true, Refused, true, true).Should().Equal(
            DownloadBlocker.Busy, DownloadBlocker.FieldRefused, DownloadBlocker.SpeciesPreviewPending,
            DownloadBlocker.DraftNotApplied, DownloadBlocker.ExportNotAcknowledged);
    }

    [Theory]
    [InlineData(true, false, false, false, DownloadBlocker.Busy)]
    [InlineData(false, true, false, false, DownloadBlocker.FieldRefused)]
    [InlineData(false, false, true, false, DownloadBlocker.SpeciesPreviewPending)]
    [InlineData(false, false, false, true, DownloadBlocker.ExportNotAcknowledged)]
    public void EachDownloadReasonHoldsItBackAlone(bool busy, bool refused, bool previewing, bool needsAcknowledgement, DownloadBlocker expected)
    {
        ActionReadiness.ForDownload(busy, false, refused ? Refused : null, previewing, needsAcknowledgement).Should().Equal(expected);
    }

    [Fact]
    public void AnUnappliedDraftHoldsBackTheDownload()
    {
        ActionReadiness.ForDownload(false, true, null, false, false).Should().Equal(DownloadBlocker.DraftNotApplied);
    }

    [Fact]
    public void ACleanDraftCanStepToAnotherPokemon()
    {
        ActionReadiness.ForStep(false, false, null, false).Should().BeEmpty();
    }

    [Fact]
    public void EveryStepReasonIsListedInOrder()
    {
        ActionReadiness.ForStep(true, true, Refused, true).Should().Equal(
            StepBlocker.Busy, StepBlocker.FieldRefused, StepBlocker.SpeciesPreviewPending, StepBlocker.DraftNotApplied);
    }

    [Theory]
    [InlineData(true, false, false, false, StepBlocker.Busy)]
    [InlineData(false, true, false, false, StepBlocker.DraftNotApplied)]
    [InlineData(false, false, true, false, StepBlocker.FieldRefused)]
    [InlineData(false, false, false, true, StepBlocker.SpeciesPreviewPending)]
    public void EachStepReasonHoldsItBackAlone(bool busy, bool dirty, bool refused, bool previewing, StepBlocker expected)
    {
        ActionReadiness.ForStep(busy, dirty, refused ? Refused : null, previewing).Should().Equal(expected);
    }
}
