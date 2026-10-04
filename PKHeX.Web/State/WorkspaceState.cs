using PKHeX.Web.Services;

namespace PKHeX.Web.State;

/// <summary>
/// What the user is working on in this tab: the open session, a request to leave it (replace, close, reset or discard), and the unapplied draft.
/// </summary>
/// <remarks>
/// It lives outside the workspace components, so a component fault that is recovered from does not lose the open session.
/// Nothing here is persisted; the state ends with the tab.
/// </remarks>
public sealed class WorkspaceState : IDisposable
{
    /// <summary>Creates the state for one tab.</summary>
    /// <param name="clock">The clock that times the legality idle delay (<see cref="DraftLegality.IdleDelay"/>).</param>
    public WorkspaceState(TimeProvider clock) : this(clock, LegalityService.Default)
    {
    }

    internal WorkspaceState(TimeProvider clock, LegalityService legality) => Legality = new DraftLegality(this, clock, legality);

    /// <summary>The legality result of the draft, and when it is analysed.</summary>
    public DraftLegality Legality { get; }

    /// <summary>The open save, or null before one is opened.</summary>
    public SaveSession? Session { get; private set; }

    /// <summary>A request to leave the open session, or null when none is in progress.</summary>
    public SessionExit? Exit { get; private set; }

    /// <summary>
    /// What <see cref="Exit"/> still waits for. It is worked out from the live session and draft every time, never stored,
    /// so an apply made after a download (which advances the revision) takes the exit back to <see cref="ExitStage.ResolveSession"/>,
    /// and the user's confirmation can never cover changes the download does not hold.
    /// </summary>
    /// <remarks>A discard asks only to confirm what it loses, so it is <see cref="ExitStage.ConfirmDiscard"/> while there is anything to lose.</remarks>
    public ExitStage ExitStage => Exit is null || Session is not { } session ? ExitStage.None
        : Exit.Intent == ExitIntent.Discard && HasUnsavedWork ? ExitStage.ConfirmDiscard
        : DraftDirty || !DraftValid ? ExitStage.ResolveDraft
        : session.ExportStatus switch
        {
            ExportStatus.Unchanged => ExitStage.Ready,
            ExportStatus.ExportedCurrent => ExitStage.ConfirmExport,
            _ => ExitStage.ResolveSession,
        };

    /// <summary>The unapplied edit of the selected slot, or null when nothing is selected.</summary>
    public EditorDraft? Draft { get; private set; }

    /// <summary>The last draft edit, when it was refused, or null; the draft then no longer matches what the user entered in that control.</summary>
    public FieldRefusal? DraftRefusal { get; private set; }

    /// <summary>False while the last draft edit was refused (<see cref="DraftRefusal"/>).</summary>
    public bool DraftValid => DraftRefusal is null;

    /// <summary>True when the draft differs from its slot.</summary>
    public bool DraftDirty => Draft?.IsDirty == true;

    /// <summary>True when leaving the page would lose work: applied changes, a dirty draft, or a refused draft edit.</summary>
    public bool HasUnsavedWork => Session?.HasChangesSinceOpen == true || DraftDirty || !DraftValid;

    /// <summary>The box shown in the storage browser. It starts at the save's in-game current box.</summary>
    public int CurrentBox { get; private set; }

    /// <summary>True when the party and box are shown as a list rather than as grids. Kept for the tab, across opened saves.</summary>
    public bool ShowAsList { get; set; }

    /// <summary>Which pane a narrow layout shows, and whether a medium one folds the storage pane away. Reset with every new session and closed draft.</summary>
    public WorkspaceView View { get; } = new();

    /// <summary>Raised after any change that can affect <see cref="HasUnsavedWork"/> or <see cref="ExitStage"/>.</summary>
    public event Action? Changed;

    /// <summary>Makes <paramref name="session"/> the open session, dropping any exit in progress and the draft.</summary>
    public void Open(SaveSession session)
    {
        Session = session;
        Exit = null;
        Draft = null;
        DraftRefusal = null;
        View.Reset();
        Legality.Reset();
        CurrentBox = StorageView.InitialBox(session);
        OnChanged();
    }

    /// <summary>
    /// Shows box <paramref name="box"/>, wrapping past either end as the game's box navigation does, so -1 is the last box.
    /// </summary>
    /// <remarks>
    /// Navigation never touches the draft: an unapplied edit stays open while other boxes are browsed.
    /// It does not raise <see cref="Changed"/>, because it cannot affect <see cref="HasUnsavedWork"/>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">No session is open.</exception>
    public void ShowBox(int box)
    {
        var count = Session is { } session ? StorageView.BoxCount(session) : throw new InvalidOperationException("No session is open.");
        CurrentBox = ((box % count) + count) % count;
    }

    /// <summary>
    /// Takes the result of opening a file. A failure changes nothing: the session, draft and any exit in progress are kept.
    /// A new session is opened at once when nothing would be lost. Otherwise it waits as the candidate of a replace
    /// (see <see cref="RequestReplace"/>), replacing any earlier candidate and turning a close in progress into a replace.
    /// </summary>
    public OpenDisposition Accept(SaveLoadOutcome outcome)
    {
        if (outcome.Session is not { } candidate)
        {
            return OpenDisposition.Refused;
        }
        if (Exit is null && !HasUnsavedWork)
        {
            Open(candidate);
            return OpenDisposition.Opened;
        }
        RequestReplace(candidate);
        return Exit is null ? OpenDisposition.Opened : OpenDisposition.Held;
    }

    /// <summary>
    /// Starts (or retargets) a replace of the open session with <paramref name="candidate"/>. It completes at once when nothing would be lost;
    /// otherwise it waits on <see cref="ExitStage"/>. The stage is kept when the candidate changes, since it depends only on the open session.
    /// </summary>
    /// <remarks>With no session open, the candidate is simply opened.</remarks>
    public void RequestReplace(SaveSession candidate)
    {
        if (Session is null)
        {
            Open(candidate);
            return;
        }
        Exit = SessionExit.Replace(candidate);
        Advance();
    }

    /// <summary>Starts a close of the open session. It completes at once when nothing would be lost; otherwise it waits on <see cref="ExitStage"/>.</summary>
    /// <exception cref="InvalidOperationException">No session is open.</exception>
    public void RequestClose()
    {
        if (Session is null)
        {
            throw new InvalidOperationException("No session is open.");
        }
        Exit = SessionExit.Close;
        Advance();
    }

    /// <summary>
    /// Parses the bytes the open session was opened from again, for <see cref="RequestReset"/>. Tests replace it to make the parse fail;
    /// the default is <see cref="SaveLoader"/> on a fresh copy, under the same file name.
    /// </summary>
    /// <remarks>
    /// XY and ORAS saves open with no interpretation choices (edition or language) to repeat; a family that asks for them must pass them here.
    /// </remarks>
    internal Func<SaveSession, SaveLoadOutcome> Reopen { get; set; } = static session => SaveLoader.Load(session.GetOriginalBytes(), session.FileName);

    /// <summary>
    /// Starts a reset of the open session to the file as it was opened. A fresh copy of the original bytes is parsed and checked first, and
    /// waits as the exit's candidate, so the exit is resolved like a replace: the draft, then the session's changes, then a download, each of
    /// which can be cancelled. Completing it opens the fresh session, which clears the draft, every applied change and the download status.
    /// </summary>
    /// <remarks>It completes at once when nothing would be lost. A failed parse changes nothing, not even an exit already in progress.</remarks>
    /// <exception cref="InvalidOperationException">No session is open.</exception>
    /// <exception cref="SessionException"><see cref="SessionError.ResetFailed"/>: the original bytes did not open again.</exception>
    public void RequestReset()
    {
        var session = Session ?? throw new InvalidOperationException("No session is open.");
        if (Reopen(session).Session is not { } fresh)
        {
            throw new SessionException(SessionError.ResetFailed);
        }
        Exit = SessionExit.Reset(fresh);
        Advance();
    }

    /// <summary>
    /// Starts a discard of the open session: the explicitly destructive way out, which offers no draft or download step. It completes at once
    /// when nothing would be lost; otherwise it waits at <see cref="ExitStage.ConfirmDiscard"/> for <see cref="DiscardSessionForExit"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">No session is open.</exception>
    public void RequestDiscard()
    {
        if (Session is null)
        {
            throw new InvalidOperationException("No session is open.");
        }
        Exit = SessionExit.Discard;
        Advance();
    }

    /// <summary>
    /// Applies the draft (see <see cref="ApplyDraft"/>) as the exit's draft step, then moves the exit on.
    /// </summary>
    /// <exception cref="InvalidOperationException">The exit is not at <see cref="ExitStage.ResolveDraft"/>.</exception>
    /// <exception cref="SessionException">The apply was refused; the exit stays where it was.</exception>
    public void ApplyDraftForExit()
    {
        RequireStage(ExitStage.ResolveDraft);
        ApplyDraft();
        Advance();
    }

    /// <summary>Drops the draft (closing the editor) as the exit's draft step, then moves the exit on. The session is unchanged.</summary>
    /// <exception cref="InvalidOperationException">The exit is not at <see cref="ExitStage.ResolveDraft"/>.</exception>
    public void DiscardDraftForExit()
    {
        RequireStage(ExitStage.ResolveDraft);
        Draft = null;
        View.Reset();
        DraftRefusal = null;
        Legality.Reset();
        Advance();
    }

    /// <summary>
    /// Records the user's confirmation that they checked the download of the current revision, and completes the exit.
    /// </summary>
    /// <exception cref="InvalidOperationException">The exit is not at <see cref="ExitStage.ConfirmExport"/> or <see cref="ExitStage.Ready"/>.</exception>
    public void ConfirmExportChecked()
    {
        RequireStage(ExitStage.ConfirmExport, ExitStage.Ready);
        Complete();
    }

    /// <summary>
    /// Completes the exit without a download, losing the session's changes. This is the explicitly destructive choice.
    /// </summary>
    /// <remarks>It is also how a discard's confirmation (<see cref="ExitStage.ConfirmDiscard"/>) is given; the draft is lost with the session.</remarks>
    /// <exception cref="InvalidOperationException">No exit is in progress, or its draft step is unresolved.</exception>
    public void DiscardSessionForExit()
    {
        RequireStage(ExitStage.ResolveSession, ExitStage.ConfirmExport, ExitStage.Ready, ExitStage.ConfirmDiscard);
        Complete();
    }

    /// <summary>Abandons the exit. The session, the draft and the editor are kept as they are; a waiting candidate is dropped.</summary>
    public void CancelExit()
    {
        Exit = null;
        OnChanged();
    }

    /// <summary>
    /// Applies the draft to the session, then reopens the same slot from the new revision as a clean draft.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A changed draft is applied only with a legality result for it as it is now: a Valid one, or an Invalid or Unavailable one the user
    /// acknowledged (<see cref="DraftLegality.Gate"/>). The verdict is recorded with the change (<see cref="SaveSession.FlaggedChanges"/>).
    /// A draft without changes writes nothing, so it needs no result.
    /// </para>
    /// <para><see cref="Changed"/> is raised straight after the apply, so the leave warning is armed even if reopening the slot fails.</para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">No session or draft is open.</exception>
    /// <exception cref="SessionException">
    /// The legality result is missing (<see cref="SessionError.LegalityNotCurrent"/>) or not acknowledged (<see cref="SessionError.LegalityNotAcknowledged"/>),
    /// or the apply or the reselect was refused.
    /// </exception>
    public void ApplyDraft()
    {
        var session = Session ?? throw new InvalidOperationException("No session is open.");
        var draft = Draft ?? throw new InvalidOperationException("No draft is open.");
        if (draft.IsDirty)
        {
            var verdict = Legality.Gate switch
            {
                LegalityGate.Clear or LegalityGate.Acknowledged => Legality.Current!.Verdict,
                LegalityGate.NeedsAcknowledgement => throw new SessionException(SessionError.LegalityNotAcknowledged),
                _ => throw new SessionException(SessionError.LegalityNotCurrent),
            };
            session.Apply(draft, verdict);
        }
        else
        {
            session.EnsureOwns(draft);
        }
        OnChanged();
        SetDraft(session.Select(draft.Slot));
    }

    /// <summary>Records (or withdraws) the user's acknowledgement of the draft's Invalid or Unavailable result; see <see cref="DraftLegality.Acknowledge"/>.</summary>
    /// <exception cref="InvalidOperationException">There is no such result to acknowledge.</exception>
    public void AcknowledgeLegality(bool acknowledge)
    {
        Legality.Acknowledge(acknowledge);
        OnChanged();
    }

    /// <summary>Records (or withdraws) the user's acknowledgement of the session's flagged changes for this download; see <see cref="SaveSession.AcknowledgeExport"/>.</summary>
    /// <exception cref="InvalidOperationException">No session is open, or it has no flagged changes.</exception>
    public void AcknowledgeExport(bool acknowledge)
    {
        var session = Session ?? throw new InvalidOperationException("No session is open.");
        session.AcknowledgeExport(acknowledge);
        OnChanged();
    }

    /// <summary>Completes the exit when nothing is left to resolve, and otherwise reports the new stage.</summary>
    private void Advance()
    {
        if (Exit is not null && !HasUnsavedWork)
        {
            Complete();
            return;
        }
        OnChanged();
    }

    /// <summary>Carries out the exit: opens the candidate, or closes the session.</summary>
    private void Complete()
    {
        if (Exit is { Candidate: { } candidate })
        {
            Open(candidate);
        }
        else
        {
            Discard();
        }
    }

    /// <summary>Rejects an exit step that does not belong to the current <see cref="ExitStage"/>.</summary>
    /// <exception cref="InvalidOperationException">The stage is not one of <paramref name="allowed"/>.</exception>
    private void RequireStage(params ReadOnlySpan<ExitStage> allowed)
    {
        var stage = ExitStage;
        foreach (var candidate in allowed)
        {
            if (candidate == stage)
            {
                return;
            }
        }
        throw new InvalidOperationException($"The exit is at {stage}.");
    }

    /// <summary>
    /// Opens the entity at <paramref name="slot"/> as the draft, read from the session's current revision.
    /// </summary>
    /// <remarks>
    /// An unapplied or refused draft is never replaced, and the slot already open is left as it is, so its legality result stays.
    /// An empty position or a bad egg opens nothing and closes the clean draft, so the editor never shows an entity the chosen
    /// position does not hold. The decision is made on the live save, not on what the page last showed.
    /// </remarks>
    /// <exception cref="InvalidOperationException">No session is open.</exception>
    public SlotOpening OpenSlot(SlotRef slot)
    {
        var session = Session ?? throw new InvalidOperationException("No session is open.");
        if (Draft?.Slot == slot)
        {
            View.ShowEditor();
            return SlotOpening.AlreadyOpen;
        }
        if (DraftDirty || !DraftValid)
        {
            return SlotOpening.DraftPending;
        }
        try
        {
            SetDraft(session.Select(slot));
            View.ShowEditor();
            return SlotOpening.Opened;
        }
        catch (SessionException e) when (e.Error is SessionError.SlotNotOccupied or SessionError.EntityInvalid)
        {
            SetDraft(null);
            return e.Error == SessionError.SlotNotOccupied ? SlotOpening.Empty : SlotOpening.Unreadable;
        }
    }

    /// <summary>Replaces the draft, or clears it with null. A new draft is analysed once it has been left unchanged (see <see cref="DraftLegality.Schedule"/>).</summary>
    public void SetDraft(EditorDraft? draft)
    {
        Draft = draft;
        DraftRefusal = null;
        if (draft is null)
        {
            View.Reset();
        }
        Legality.Reset();
        Legality.Schedule();
        OnChanged();
    }

    /// <summary>
    /// Records that the last edit of the draft was accepted, which ends any earlier refusal. It makes the legality result stale and schedules
    /// a new analysis.
    /// </summary>
    public void AcceptDraftEdit() => SetDraftRefusal(null);

    /// <summary>
    /// Records that the last edit of the draft was refused, and where. It cancels any waiting analysis, since the draft no longer matches what
    /// the user entered.
    /// </summary>
    public void RefuseDraftEdit(FieldRefusal refusal) => SetDraftRefusal(refusal);

    private void SetDraftRefusal(FieldRefusal? refusal)
    {
        DraftRefusal = refusal;
        Legality.Schedule();
        OnChanged();
    }

    /// <summary>Reports a change made through the session or draft themselves, such as an apply, an export or a draft edit.</summary>
    public void NotifyChanged() => OnChanged();

    /// <summary>
    /// Keeps the open session after a component fault, and drops what the fault may have left half-done: the draft and any exit in progress.
    /// </summary>
    /// <remarks>
    /// The session itself is safe to keep: <see cref="SaveSession.Apply(EditorDraft, LegalityVerdict)"/> stages every write on a clone and swaps it in only after it is verified,
    /// so a fault can interrupt an apply but never leave it partly written. Exports are still validated before download.
    /// </remarks>
    public void RecoverAfterFault()
    {
        Exit = null;
        Draft = null;
        DraftRefusal = null;
        View.Reset();
        Legality.Reset();
        OnChanged();
    }

    /// <summary>Closes the session and drops everything, as if the page had just loaded.</summary>
    public void Discard()
    {
        Session = null;
        CurrentBox = 0;
        RecoverAfterFault();
    }

    /// <summary>Cancels any waiting legality analysis when the tab's scope ends.</summary>
    public void Dispose() => Legality.Dispose();

    private void OnChanged() => Changed?.Invoke();
}

/// <summary>What <see cref="WorkspaceState.OpenSlot"/> did.</summary>
public enum SlotOpening
{
    /// <summary>The slot's entity is now the draft.</summary>
    Opened,

    /// <summary>The slot is already the draft's; nothing changed.</summary>
    AlreadyOpen,

    /// <summary>The draft has unapplied or refused changes, so it was kept; nothing changed.</summary>
    DraftPending,

    /// <summary>The position holds nothing; the clean draft, if any, was closed.</summary>
    Empty,

    /// <summary>The entity is a bad egg (fails its checksum or sanity check); the clean draft, if any, was closed.</summary>
    Unreadable,
}

/// <summary>What <see cref="WorkspaceState.Accept"/> did with an opened file.</summary>
public enum OpenDisposition
{
    /// <summary>The file was not opened; nothing changed.</summary>
    Refused,

    /// <summary>The file is now the open session.</summary>
    Opened,

    /// <summary>The file waits as the candidate of <see cref="WorkspaceState.Exit"/> until the user resolves or cancels the exit.</summary>
    Held,
}
