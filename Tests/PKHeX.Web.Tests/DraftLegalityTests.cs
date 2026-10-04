using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// When the draft is analysed and which result is shown (WEB-LEGAL-003, WEB-PERF-004): stale at once on an edit, analysed after the idle delay
/// or on request, and never showing a result for a state that has moved on.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class DraftLegalityTests
{
    private readonly FakeTimeProvider clock = new();
    private readonly WorkspaceState state;
    private int analyses;

    /// <summary>Completed when a run reaches its paint wait, which it does at once once its timer has fired.</summary>
    private TaskCompletionSource runStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public DraftLegalityTests()
    {
        // Counts calls into Core, so a test can tell "not analysed" from "analysed and dropped".
        var counting = new LegalityService((pk, table, type) =>
        {
            analyses++;
            return new PKHeX.Core.LegalityAnalysis(pk, table, type);
        });
        state = new WorkspaceState(clock, counting);
        state.Legality.Yield = () =>
        {
            runStarted.TrySetResult();
            return Task.CompletedTask;
        };
    }

    private DraftLegality Legality => state.Legality;

    private static readonly TimeSpan JustBefore = DraftLegality.IdleDelay - TimeSpan.FromMilliseconds(1);

    /// <summary>Opens a synthetic ORAS save (where the known legal entity is its trainer's own) with the legal or illegal entity as the draft.</summary>
    private EditorDraft OpenDraft(bool legal = true)
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true, legal));
        state.Open(session);
        state.OpenSlot(SaveFixtures.FirstBoxSlot).Should().Be(SlotOpening.Opened);
        return state.Draft!;
    }

    /// <summary>Makes an accepted edit through the same path as the editor.</summary>
    private void Edit(string nickname)
    {
        state.Draft!.EditNickname(nickname, true);
        state.AcceptDraftEdit();
    }

    /// <summary>
    /// Advances the clock by <paramref name="time"/> and waits up to 250 ms for a run to start. The fake clock fires timers at once, but their
    /// continuations are posted, so asserting straight after <see cref="FakeTimeProvider.Advance"/> would not see a run that started. A run
    /// that fired reaches its paint wait without calling Core, so it is seen well within the wait even on a slow machine.
    /// </summary>
    private async Task Pass(TimeSpan time)
    {
        runStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        clock.Advance(time);
        if (await Task.WhenAny(runStarted.Task, Task.Delay(250)) == runStarted.Task)
        {
            await Legality.Completion;
        }
    }

    /// <summary>Lets the idle delay pass and waits for the run it starts.</summary>
    private async Task Idle()
    {
        clock.Advance(DraftLegality.IdleDelay);
        await Legality.Completion;
    }

    [Fact]
    public void NoDraftHasNoStatus()
    {
        Legality.Status.Should().Be(LegalityStatus.None);
        Legality.Current.Should().BeNull();
    }

    [Fact]
    public async Task OpeningASlotAnalysesItOnceIdle()
    {
        OpenDraft(legal: false);
        Legality.Status.Should().Be(LegalityStatus.Pending, "an analysis is waiting for the idle delay");

        await Pass(JustBefore);
        analyses.Should().Be(0);
        await Idle();

        analyses.Should().Be(1);
        Legality.Status.Should().Be(LegalityStatus.Invalid);
        Legality.Current!.Tag.Should().Be(LegalityTag.Of(state.Draft!));
    }

    [Fact]
    public async Task AnEditMakesTheResultStaleAtOnceAndIsAnalysedOnceIdle()
    {
        OpenDraft();
        await Idle();
        Legality.Status.Should().Be(LegalityStatus.Valid);

        Edit("Changed");
        Legality.Status.Should().Be(LegalityStatus.Stale);
        Legality.Current.Should().BeNull("a result for an earlier edit is never current");
        Legality.Result.Should().NotBeNull("the earlier result is kept only to tell stale from not analysed");

        await Pass(JustBefore);
        analyses.Should().Be(1);
        await Idle();
        analyses.Should().Be(2);
        Legality.Status.Should().Be(LegalityStatus.Valid);
        Legality.Current!.Tag.EditRevision.Should().Be(1);
    }

    [Fact]
    public async Task EachEditRestartsTheIdleDelay()
    {
        OpenDraft();
        await Idle();
        Edit("A");
        await Pass(JustBefore);
        Edit("AB");
        await Pass(JustBefore);
        Edit("ABC");
        await Pass(JustBefore);
        analyses.Should().Be(1, "typing keeps postponing the analysis");

        await Idle();
        analyses.Should().Be(2, "only the final state is analysed");
        Legality.Current!.Tag.EditRevision.Should().Be(3);
    }

    [Fact]
    public async Task AnEditWhileTheRunWaitsToPaintSupersedesIt()
    {
        OpenDraft();
        var paint = new TaskCompletionSource();
        Legality.Yield = () => paint.Task;
        // Analyze now runs synchronously up to the paint wait, so the run is certainly waiting there.
        var superseded = Legality.RunNowAsync();
        Legality.Status.Should().Be(LegalityStatus.Pending);

        Edit("Later");
        paint.SetResult();
        await superseded;

        analyses.Should().Be(0, "Core is not called for a state that has moved on");
        Legality.Result.Should().BeNull();
        Legality.Status.Should().Be(LegalityStatus.Pending, "the edit scheduled a fresh analysis");

        await Idle();
        Legality.Status.Should().Be(LegalityStatus.Valid);
        Legality.Current!.Tag.EditRevision.Should().Be(1);
    }

    [Fact]
    public async Task ANewDraftDropsTheOldResultAndAPendingRun()
    {
        var first = OpenDraft();
        await Idle();
        Legality.Result!.Tag.Draft.Should().BeSameAs(first);

        var paint = new TaskCompletionSource();
        Legality.Yield = () => paint.Task;
        state.SetDraft(state.Session!.Select(SaveFixtures.FirstBoxSlot));
        Legality.Result.Should().BeNull("the result belongs to the previous draft");
        Legality.Status.Should().Be(LegalityStatus.Pending);
        var running = Legality.RunNowAsync();

        // Opening another save while the run waits to paint.
        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(true)));
        paint.SetResult();
        await running;

        analyses.Should().Be(1);
        Legality.Status.Should().Be(LegalityStatus.None);
    }

    [Fact]
    public async Task AnalyzeNowRunsAtOnceAndCancelsTheWaitingRun()
    {
        OpenDraft();
        Edit("Now");
        await Legality.RunNowAsync();
        analyses.Should().Be(1);
        Legality.Status.Should().Be(LegalityStatus.Valid);

        await Idle();
        analyses.Should().Be(1, "the waiting run was cancelled");
        await Legality.RunNowAsync();
        analyses.Should().Be(1, "a current result is not analysed again");
    }

    [Fact]
    public async Task ARefusedEditIsStaleAndNotAnalysed()
    {
        OpenDraft();
        await Idle();
        state.RefuseDraftEdit(new FieldRefusal("level", SessionError.LevelOutOfRange));

        Legality.Status.Should().Be(LegalityStatus.Stale, "the draft no longer matches what the user entered");
        await Idle();
        await Legality.RunNowAsync();
        analyses.Should().Be(1);

        Edit("Fixed");
        await Idle();
        analyses.Should().Be(2);
        Legality.Status.Should().Be(LegalityStatus.Valid);
    }

    [Fact]
    public async Task ARefusedEditBeforeAnyResultIsNotAnalysed()
    {
        OpenDraft();
        state.RefuseDraftEdit(new FieldRefusal("level", SessionError.LevelOutOfRange));
        Legality.Status.Should().Be(LegalityStatus.NotAnalyzed);
        await Idle();
        analyses.Should().Be(0);
    }

    [Fact]
    public async Task AutoRunOffWaitsForAnalyzeNow()
    {
        Legality.AutoRun = false;
        OpenDraft();
        Legality.Status.Should().Be(LegalityStatus.NotAnalyzed);
        await Idle();
        analyses.Should().Be(0);

        await Legality.RunNowAsync();
        Legality.Status.Should().Be(LegalityStatus.Valid);
        Edit("Manual");
        Legality.Status.Should().Be(LegalityStatus.Stale);
        await Idle();
        analyses.Should().Be(1);
    }

    [Fact]
    public async Task DisposingStopsTheWaitingRun()
    {
        OpenDraft();
        state.Dispose();
        await Idle();
        analyses.Should().Be(0);
    }

    [Fact]
    public async Task ApplyCancelAndRecoveryEachStartFresh()
    {
        OpenDraft();
        Edit("Applied");
        await Idle();
        analyses.Should().Be(1);

        state.ApplyDraft();
        Legality.Result.Should().BeNull("the reopened slot is a new draft");
        Legality.Status.Should().Be(LegalityStatus.Pending, "it is analysed again once idle (refresh after apply)");
        await Idle();
        analyses.Should().Be(2);

        state.SetDraft(state.Session!.Select(SaveFixtures.FirstBoxSlot));
        Legality.Status.Should().Be(LegalityStatus.Pending, "a cancelled draft is reopened and analysed again");
        await Idle();
        analyses.Should().Be(3);

        state.RecoverAfterFault();
        Legality.Status.Should().Be(LegalityStatus.None);
        await Idle();
        analyses.Should().Be(3);
    }

    [Fact]
    public async Task AFailedAnalysisIsUnavailableAndReported()
    {
        var failures = new List<Exception>();
        var throwing = new WorkspaceState(clock, new LegalityService((_, _, _) => throw new InvalidOperationException("detail")));
        throwing.Legality.AnalysisFailed += failures.Add;
        throwing.Open(SaveFixtures.Open(SaveFixtures.Synthetic(false)));
        throwing.OpenSlot(SaveFixtures.FirstBoxSlot);

        await throwing.Legality.RunNowAsync();

        throwing.Legality.Status.Should().Be(LegalityStatus.Unavailable);
        failures.Should().ContainSingle();
    }

    [Fact]
    public async Task AFailingFailureHandlerKeepsTheUnavailableResult()
    {
        var throwing = new WorkspaceState(clock, new LegalityService((_, _, _) => throw new InvalidOperationException("detail")));
        throwing.Legality.AnalysisFailed += _ => throw new InvalidOperationException("handler");
        throwing.Open(SaveFixtures.Open(SaveFixtures.Synthetic(true)));
        throwing.OpenSlot(SaveFixtures.FirstBoxSlot);

        await throwing.Legality.RunNowAsync();

        throwing.Legality.Status.Should().Be(LegalityStatus.Unavailable, "reporting the cause must not undo the result");
    }

    [Fact]
    public async Task ARunSupersededWhileWaitingToPaintIsNotBusy()
    {
        OpenDraft();
        var paint = new TaskCompletionSource();
        Legality.Yield = () => paint.Task;
        var superseded = Legality.RunNowAsync();
        Legality.IsRunning.Should().BeTrue();

        Edit("Later");
        Legality.IsRunning.Should().BeFalse("the waiting run is for an earlier state and will not call Core");
        paint.SetResult();
        await superseded;
        Legality.IsRunning.Should().BeFalse();
    }

    [Fact]
    public async Task ADraftFromAnEarlierRevisionIsUnavailableRatherThanStaleForever()
    {
        // As if reopening the slot after an apply had failed: the open draft belongs to an earlier revision of the session.
        OpenDraft();
        var session = state.Session!;
        var other = session.Select(SaveFixtures.FirstBoxSlot);
        other.EditNickname("Applied", true);
        session.Apply(other);
        Edit("Orphan");

        await Legality.RunNowAsync();

        analyses.Should().Be(0);
        Legality.Status.Should().Be(LegalityStatus.Unavailable);
    }

    [Fact]
    public async Task ChangedIsRaisedWhenARunCompletes()
    {
        OpenDraft();
        var changes = 0;
        Legality.Changed += () => changes++;
        await Idle();
        changes.Should().BeGreaterThanOrEqualTo(2, "once when the run is marked pending and once when it completes");
    }
}
