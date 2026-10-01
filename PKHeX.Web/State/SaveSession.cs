using PKHeX.Core;

namespace PKHeX.Web.State;

/// <summary>
/// One opened save: the untouched original bytes plus the working <see cref="SaveFile"/> that applied edits are written to.
/// </summary>
/// <remarks>
/// Create instances through <see cref="Services.SaveLoader"/>. The working save is internal and is only ever replaced
/// wholesale by a staged, validated clone, so every change goes through <see cref="Apply"/> and its revision tracking,
/// and a failed apply leaves the session unchanged.
/// </remarks>
public sealed class SaveSession
{
    private readonly byte[] original;

    /// <summary>Unique identifier for this opened session.</summary>
    public Guid SessionId { get; } = Guid.NewGuid();

    /// <summary>Working save that reflects every applied edit.</summary>
    internal SaveFile Working { get; private set; }

    /// <summary>Sanitised name the file was opened as, kept apart from the bytes; exports are offered under this name.</summary>
    public string FileName { get; }

    /// <summary>Incremented once for each apply that changed the working save.</summary>
    public int Revision { get; private set; }

    /// <summary>True once any edit has been applied since the file was opened.</summary>
    public bool HasChangesSinceOpen { get; private set; }

    /// <summary>The <see cref="Revision"/> of the last export whose download was started, or null if none.</summary>
    public int? ExportedRevision { get; private set; }

    /// <summary>
    /// Whether the applied changes are covered by a started download. It is separate from <see cref="HasChangesSinceOpen"/>,
    /// which stays true after a download, because the session still differs from the file it was opened from.
    /// </summary>
    public ExportStatus ExportStatus => !HasChangesSinceOpen ? ExportStatus.Unchanged
        : ExportedRevision is not { } exported ? ExportStatus.NotExported
        : exported == Revision ? ExportStatus.ExportedCurrent
        : ExportStatus.ChangedSinceExport;

    internal SaveSession(byte[] source, SaveFile save, string fileName)
    {
        original = source;
        FileName = fileName;
        Working = save;
    }

    /// <summary>Returns a copy of the bytes the session was opened from.</summary>
    public byte[] GetOriginalBytes() => original.ToArray();

    /// <summary>Size of the file the session was opened from, in bytes.</summary>
    public int OriginalLength => original.Length;

    /// <summary>
    /// Starts a draft from an occupied party position or box slot, read from the current revision.
    /// </summary>
    /// <remarks>
    /// The slot is read afresh on every call, so a draft is never taken from an earlier state of the save.
    /// Party positions at or after <see cref="SaveFile.PartyCount"/> count as empty whatever bytes they still hold, as they do in game.
    /// </remarks>
    /// <exception cref="SessionException"><see cref="SessionError.SlotNotOccupied"/> or <see cref="SessionError.EntityInvalid"/>.</exception>
    public EditorDraft Select(SlotRef slot)
    {
        if (ReadOccupied(Working, slot) is not PK6 entity)
        {
            throw new SessionException(SessionError.SlotNotOccupied);
        }
        // PKM.Valid is the checksum plus the sanity flag; the game shows either failure as a bad egg, as does WinForms.
        if (!entity.Valid)
        {
            throw new SessionException(SessionError.EntityInvalid);
        }
        return new EditorDraft(SessionId, slot, Revision, entity, Working.MaxStringLengthNickname);
    }

    /// <summary>
    /// Writes a draft to its source slot. The write is staged on a clone of the working save and swapped in only after it is verified.
    /// </summary>
    /// <remarks>A draft with no changes is ignored and does not count as a change.</remarks>
    /// <exception cref="SessionException">
    /// The draft is foreign or stale, it is of a party position (not yet written by this release), the slot cannot be written,
    /// or the staged write fails verification.
    /// </exception>
    public void Apply(EditorDraft draft)
    {
        EnsureOwns(draft);
        if (!draft.IsDirty)
        {
            return;
        }
        EnsureCurrent(draft);
        if (!draft.CanApply)
        {
            throw new SessionException(SessionError.PartyApplyNotAvailable);
        }

        var entity = draft.ToStoredEntity();
        var candidate = Working.Clone();
        var slot = draft.Slot.ToSlotInfo(candidate);
        if (!slot.CanWriteTo(candidate) || slot.CanWriteTo(candidate, entity) != WriteBlockedMessage.None)
        {
            throw new SessionException(SessionError.SlotNotWritable);
        }
        // The default import settings also mark the Pokédex, bump trainer records and rewrite handler data as if traded in.
        if (!slot.WriteTo(candidate, entity, EntityImportSettings.None))
        {
            throw new SessionException(SessionError.StagedWriteFailed);
        }
        var stored = slot.Read(candidate);
        if (!stored.ChecksumValid || stored.Nickname != entity.Nickname || stored.IsNicknamed != entity.IsNicknamed)
        {
            throw new SessionException(SessionError.StagedEditMismatch);
        }

        Working = candidate;
        Revision++;
        HasChangesSinceOpen = true;
    }

    /// <summary>
    /// Records that the download of an export taken at <paramref name="revision"/> was started.
    /// </summary>
    /// <remarks>Call this only after the download has been handed to the browser, not when the bytes are produced.</remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="revision"/> is not a revision of this session.</exception>
    public void MarkExported(int revision)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(revision);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(revision, Revision);
        ExportedRevision = revision;
    }

    /// <summary>
    /// Rejects a draft that was not taken from this session.
    /// </summary>
    /// <exception cref="SessionException"><see cref="SessionError.ForeignDraft"/>.</exception>
    internal void EnsureOwns(EditorDraft draft)
    {
        if (draft.SessionId != SessionId)
        {
            throw new SessionException(SessionError.ForeignDraft);
        }
    }

    /// <summary>
    /// Rejects a draft taken from an earlier revision.
    /// </summary>
    /// <exception cref="SessionException"><see cref="SessionError.StaleDraft"/>.</exception>
    internal void EnsureCurrent(EditorDraft draft)
    {
        if (draft.SourceRevision != Revision)
        {
            throw new SessionException(SessionError.StaleDraft);
        }
    }

    /// <summary>
    /// The entity at <paramref name="slot"/> of <paramref name="save"/>, or null when the position does not exist or holds nothing.
    /// </summary>
    /// <remarks>Party positions at or after <see cref="SaveFile.PartyCount"/> are empty, even if an earlier member's bytes remain there.</remarks>
    internal static PKM? ReadOccupied(SaveFile save, SlotRef slot)
    {
        if (!slot.IsWithin(save) || (slot.IsParty && slot.Slot >= save.PartyCount))
        {
            return null;
        }
        var entity = slot.ToSlotInfo(save).Read(save);
        return entity.Species == 0 ? null : entity;
    }
}
