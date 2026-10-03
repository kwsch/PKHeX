using PKHeX.Web.Services;

namespace PKHeX.Web.State;

/// <summary>What the legality panel shows for the open draft.</summary>
public enum LegalityStatus
{
    /// <summary>No draft is open.</summary>
    None,

    /// <summary>The draft has not been analysed and no analysis is waiting.</summary>
    NotAnalyzed,

    /// <summary>
    /// The draft as it is now is being analysed, or, when it has never been analysed, is waiting for the idle delay to analyse it.
    /// </summary>
    Pending,

    /// <summary>
    /// The draft changed after its last analysis (or the last edit was refused), and no analysis of it is running yet; one may be waiting for
    /// the idle delay.
    /// </summary>
    Stale,

    /// <summary>The current draft was analysed: <see cref="LegalityVerdict.Valid"/>.</summary>
    Valid,

    /// <summary>The current draft was analysed: <see cref="LegalityVerdict.Invalid"/>.</summary>
    Invalid,

    /// <summary>The current draft was analysed: <see cref="LegalityVerdict.Unavailable"/>.</summary>
    Unavailable,
}

/// <summary>
/// The legality result of the open draft, and when it is analysed: after <see cref="IdleDelay"/> without further changes, or at once on request.
/// </summary>
/// <remarks>
/// <para>
/// Every result is tagged with the draft state it describes (<see cref="LegalityTag"/>). <see cref="Status"/> is worked out from the live draft
/// every time, so an accepted edit makes the result stale at once, and a result that completes for a superseded state is dropped.
/// </para>
/// <para>
/// Core's analysis is synchronous and runs on the browser's only thread. Before it starts, a run marks itself pending and awaits <see cref="Yield"/>,
/// so the page can show "Pending" first; edits made meanwhile supersede the run, which then does not analyse at all. Nothing runs in the background.
/// </para>
/// </remarks>
public sealed class DraftLegality : IDisposable
{
    /// <summary>How long the draft must stay unchanged before it is analysed automatically.</summary>
    public static readonly TimeSpan IdleDelay = TimeSpan.FromMilliseconds(300);

    private readonly WorkspaceState state;
    private readonly TimeProvider clock;
    private readonly LegalityService service;
    private CancellationTokenSource? timer;

    /// <summary>The draft state waiting for the idle delay, if any.</summary>
    private LegalityTag? scheduled;

    /// <summary>The draft state being analysed, if any.</summary>
    private LegalityTag? running;

    internal DraftLegality(WorkspaceState state, TimeProvider clock, LegalityService service)
    {
        this.state = state;
        this.clock = clock;
        this.service = service;
    }

    /// <summary>The last completed result for the open draft, which may be for an earlier edit; null when the open draft was never analysed.</summary>
    public LegalityResult? Result => result is { } r && state.Draft is { } draft && ReferenceEquals(r.Tag.Draft, draft) ? r : null;

    private LegalityResult? result;

    /// <summary>The result when it describes the draft exactly as it is now; otherwise null.</summary>
    public LegalityResult? Current => Status is LegalityStatus.Valid or LegalityStatus.Invalid or LegalityStatus.Unavailable ? result : null;

    /// <summary>What the panel shows, worked out from the live draft.</summary>
    public LegalityStatus Status
    {
        get
        {
            if (state.Draft is not { } draft)
            {
                return LegalityStatus.None;
            }
            var last = Result;
            if (!state.DraftValid)
            {
                // The draft no longer matches what the user typed, so no result describes it.
                return last is null ? LegalityStatus.NotAnalyzed : LegalityStatus.Stale;
            }
            var tag = LegalityTag.Of(draft);
            if (last is not null && last.Tag == tag)
            {
                return last.Verdict switch
                {
                    LegalityVerdict.Valid => LegalityStatus.Valid,
                    LegalityVerdict.Invalid => LegalityStatus.Invalid,
                    _ => LegalityStatus.Unavailable,
                };
            }
            if (running == tag)
            {
                return LegalityStatus.Pending;
            }
            if (last is not null)
            {
                return LegalityStatus.Stale;
            }
            return scheduled == tag ? LegalityStatus.Pending : LegalityStatus.NotAnalyzed;
        }
    }

    /// <summary>
    /// True from the moment a run of the draft as it is now starts until it completes, including its wait for a paint. A run that an edit has
    /// superseded does not count: it will not call Core.
    /// </summary>
    public bool IsRunning => running is { } tag && IsCurrent(tag);

    /// <summary>The Invalid or Unavailable result the user acknowledged, if any. It counts only while it is the draft's current result.</summary>
    private LegalityTag? acknowledged;

    /// <summary>
    /// What legality asks before the draft as it is now can be applied. It is worked out from the live draft, so an accepted edit, which makes
    /// the result stale, also withdraws an acknowledgement: a new result has to be acknowledged afresh.
    /// </summary>
    public LegalityGate Gate => Status switch
    {
        LegalityStatus.Valid => LegalityGate.Clear,
        LegalityStatus.Invalid or LegalityStatus.Unavailable => acknowledged == result?.Tag ? LegalityGate.Acknowledged : LegalityGate.NeedsAcknowledgement,
        _ => LegalityGate.Waiting,
    };

    /// <summary>
    /// Records (or, with false, withdraws) the user's acknowledgement of the current Invalid or Unavailable result, allowing the draft as it is
    /// now to be applied.
    /// </summary>
    /// <exception cref="InvalidOperationException">The draft as it is now has no Invalid or Unavailable result to acknowledge.</exception>
    public void Acknowledge(bool acknowledge)
    {
        if (!acknowledge)
        {
            acknowledged = null;
        }
        else if (Status is LegalityStatus.Invalid or LegalityStatus.Unavailable)
        {
            acknowledged = result!.Tag;
        }
        else
        {
            throw new InvalidOperationException($"There is no Invalid or Unavailable result to acknowledge; the legality status is {Status}.");
        }
        OnChanged();
    }

    /// <summary>
    /// When false, nothing is analysed automatically and <see cref="Schedule"/> only cancels a waiting run; <see cref="RunNowAsync"/> still works.
    /// </summary>
    public bool AutoRun { get; set; } = true;

    /// <summary>
    /// Awaited between marking a run pending and calling Core, so the page can show the pending state before the synchronous analysis.
    /// The browser sets it to wait for the next paint.
    /// </summary>
    public Func<Task> Yield { get; set; } = async () => await Task.Yield();

    /// <summary>Raised whenever <see cref="Status"/> or <see cref="Result"/> may have changed, including after an awaited step.</summary>
    public event Action? Changed;

    /// <summary>Raised with the exception that made an analysis unavailable. It is for the browser console only, never for the page.</summary>
    public event Action<Exception>? AnalysisFailed;

    /// <summary>The waiting or running analysis, or a completed task when there is none. For tests and for awaiting a run to settle.</summary>
    internal Task Completion { get; private set; } = Task.CompletedTask;

    /// <summary>
    /// Analyses the open draft once it has been unchanged for <see cref="IdleDelay"/>, replacing any waiting analysis. Does nothing more
    /// (beyond cancelling) when there is no draft, the last edit was refused, or <see cref="AutoRun"/> is off.
    /// </summary>
    public void Schedule()
    {
        CancelTimer();
        if (AutoRun && state.Draft is { } draft && state.DraftValid)
        {
            var tag = LegalityTag.Of(draft);
            var source = new CancellationTokenSource();
            timer = source;
            scheduled = tag;
            Completion = WaitThenRunAsync(tag, source.Token);
        }
        OnChanged();
    }

    /// <summary>
    /// Analyses the open draft now, cancelling any waiting analysis. Does nothing when there is no draft, the last edit was refused,
    /// the draft as it is now already has a result, or it is already being analysed.
    /// </summary>
    public Task RunNowAsync()
    {
        CancelTimer();
        if (state.Draft is not { } draft || !state.DraftValid)
        {
            OnChanged();
            return Task.CompletedTask;
        }
        var tag = LegalityTag.Of(draft);
        if (running == tag || Result?.Tag == tag)
        {
            return Completion;
        }
        return Completion = RunAsync(tag);
    }

    /// <summary>
    /// Cancels any waiting analysis and forgets the result and its acknowledgement. A run that is already pending finds its draft gone and does
    /// not analyse.
    /// </summary>
    public void Reset()
    {
        CancelTimer();
        result = null;
        running = null;
        acknowledged = null;
    }

    /// <summary>Cancels any waiting analysis.</summary>
    public void Dispose() => CancelTimer();

    private void CancelTimer()
    {
        scheduled = null;
        timer?.Cancel();
        timer?.Dispose();
        timer = null;
    }

    private async Task WaitThenRunAsync(LegalityTag tag, CancellationToken token)
    {
        try
        {
            await Task.Delay(IdleDelay, clock, token);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        if (token.IsCancellationRequested || scheduled != tag)
        {
            return;
        }
        CancelTimer();
        await RunAsync(tag);
    }

    private async Task RunAsync(LegalityTag tag)
    {
        running = tag;
        OnChanged();
        try
        {
            try
            {
                await Yield();
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                // Waiting for a paint failed (for example, the page is going away); the analysis itself can still run.
            }
            Analyze(tag);
        }
        finally
        {
            if (running == tag)
            {
                running = null;
            }
            OnChanged();
        }
    }

    /// <summary>Calls Core for <paramref name="tag"/> and keeps the result, unless the draft moved on meanwhile.</summary>
    private void Analyze(LegalityTag tag)
    {
        // An edit, a new draft or a closed session while yielding supersedes this run: Core is not called for a state nobody sees.
        if (!IsCurrent(tag) || state.Session is not { } session)
        {
            return;
        }
        LegalityResult completed;
        Exception? failure;
        try
        {
            completed = service.Analyze(session, tag.Draft, out failure);
        }
        catch (SessionException)
        {
            // The open draft was taken from an earlier revision (reopening its slot after an apply failed). It can never be analysed, so say so
            // rather than leaving it stale; it is replaced on the next draft change.
            completed = LegalityResult.Unavailable(tag);
            failure = null;
        }
        // Core is synchronous, so nothing can change the draft during the call; checked again so a stale result can never be kept.
        if (IsCurrent(tag))
        {
            result = completed;
        }
        if (failure is not null)
        {
            ReportFailure(failure);
        }
    }

    /// <summary>Raises <see cref="AnalysisFailed"/>. A failing handler must not undo the result, so its own exception is dropped.</summary>
    private void ReportFailure(Exception failure)
    {
        try
        {
            AnalysisFailed?.Invoke(failure);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            // Reporting is for the console only; there is nowhere left to report this to.
        }
    }

    /// <summary>True when <paramref name="tag"/> is the open draft as it is now and the draft matches what the user entered.</summary>
    private bool IsCurrent(LegalityTag tag) => state.Draft is { } draft && LegalityTag.Of(draft) == tag && state.DraftValid;

    private void OnChanged() => Changed?.Invoke();
}
