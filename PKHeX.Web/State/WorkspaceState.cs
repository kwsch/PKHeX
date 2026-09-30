using PKHeX.Web.Services;

namespace PKHeX.Web.State;

/// <summary>
/// What the user is working on in this tab: the open session, a replacement waiting for confirmation, and the unapplied draft.
/// </summary>
/// <remarks>
/// It lives outside the workspace components, so a component fault that is recovered from does not lose the open session.
/// Nothing here is persisted; the state ends with the tab.
/// </remarks>
public sealed class WorkspaceState
{
    /// <summary>The open save, or null before one is opened.</summary>
    public SaveSession? Session { get; private set; }

    /// <summary>A parsed replacement for <see cref="Session"/>, held until the user confirms or cancels the replacement.</summary>
    public SaveSession? Pending { get; private set; }

    /// <summary>The unapplied edit of the selected slot, or null when nothing is selected.</summary>
    public EditorDraft? Draft { get; private set; }

    /// <summary>False while the last draft edit was refused; the draft then no longer matches what the user typed.</summary>
    public bool DraftValid { get; private set; } = true;

    /// <summary>True when the draft differs from its slot.</summary>
    public bool DraftDirty => Draft?.IsDirty == true;

    /// <summary>True when leaving the page would lose work: applied changes, a dirty draft, or a refused draft edit.</summary>
    public bool HasUnsavedWork => Session?.HasChangesSinceOpen == true || DraftDirty || !DraftValid;

    /// <summary>The box shown in the storage browser. It starts at the save's in-game current box.</summary>
    public int CurrentBox { get; private set; }

    /// <summary>True when the party and box are shown as a list rather than as grids. Kept for the tab, across opened saves.</summary>
    public bool ShowAsList { get; set; }

    /// <summary>Raised after any change that can affect <see cref="HasUnsavedWork"/>.</summary>
    public event Action? Changed;

    /// <summary>Makes <paramref name="session"/> the open session, dropping any pending replacement and draft.</summary>
    public void Open(SaveSession session)
    {
        Session = session;
        Pending = null;
        Draft = null;
        DraftValid = true;
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
    /// Takes the result of opening a file. A failure changes nothing: the session, draft and any pending replacement are kept.
    /// A new session is opened at once when nothing would be lost, and otherwise held as the pending replacement, replacing any earlier one.
    /// </summary>
    public OpenDisposition Accept(SaveLoadOutcome outcome)
    {
        if (outcome.Session is not { } candidate)
        {
            return OpenDisposition.Refused;
        }
        if (HasUnsavedWork)
        {
            OfferReplacement(candidate);
            return OpenDisposition.Held;
        }
        Open(candidate);
        return OpenDisposition.Opened;
    }

    /// <summary>Holds <paramref name="candidate"/> until <see cref="ConfirmReplace"/> or <see cref="CancelReplace"/>.</summary>
    public void OfferReplacement(SaveSession candidate)
    {
        Pending = candidate;
        OnChanged();
    }

    /// <summary>Opens the pending replacement, if there is one.</summary>
    public void ConfirmReplace()
    {
        if (Pending is not null)
        {
            Open(Pending);
        }
    }

    /// <summary>Drops the pending replacement and keeps the open session.</summary>
    public void CancelReplace()
    {
        Pending = null;
        OnChanged();
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
            return SlotOpening.AlreadyOpen;
        }
        if (DraftDirty || !DraftValid)
        {
            return SlotOpening.DraftPending;
        }
        try
        {
            SetDraft(session.Select(slot));
            return SlotOpening.Opened;
        }
        catch (SessionException e) when (e.Error is SessionError.SlotNotOccupied or SessionError.EntityInvalid)
        {
            SetDraft(null);
            return e.Error == SessionError.SlotNotOccupied ? SlotOpening.Empty : SlotOpening.Unreadable;
        }
    }

    /// <summary>Replaces the draft, or clears it with null.</summary>
    public void SetDraft(EditorDraft? draft)
    {
        Draft = draft;
        DraftValid = true;
        OnChanged();
    }

    /// <summary>Records whether the last edit of the draft was accepted.</summary>
    public void SetDraftValid(bool valid)
    {
        DraftValid = valid;
        OnChanged();
    }

    /// <summary>Reports a change made through the session or draft themselves, such as an apply, an export or a draft edit.</summary>
    public void NotifyChanged() => OnChanged();

    /// <summary>
    /// Keeps the open session after a component fault, and drops what the fault may have left half-done: the draft and any pending replacement.
    /// </summary>
    /// <remarks>
    /// The session itself is safe to keep: <see cref="SaveSession.Apply"/> stages every write on a clone and swaps it in only after it is verified,
    /// so a fault can interrupt an apply but never leave it partly written. Exports are still validated before download.
    /// </remarks>
    public void RecoverAfterFault()
    {
        Pending = null;
        Draft = null;
        DraftValid = true;
        OnChanged();
    }

    /// <summary>Closes the session and drops everything, as if the page had just loaded.</summary>
    public void Discard()
    {
        Session = null;
        CurrentBox = 0;
        RecoverAfterFault();
    }

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

    /// <summary>The file is held as <see cref="WorkspaceState.Pending"/> until the user confirms or cancels.</summary>
    Held,
}
