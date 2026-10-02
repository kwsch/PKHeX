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

    /// <summary>What this release can do with the save: its family, the fields that can be drafted and the session's Core lists.</summary>
    public SaveCapabilities Capabilities { get; }

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

    /// <param name="source">The bytes the save was opened from; the session keeps them as given.</param>
    /// <param name="save">The parsed save, which becomes the working save.</param>
    /// <param name="fileName">The sanitised file name.</param>
    /// <param name="capabilities">The save's capabilities; worked out from <see cref="Services.SupportMatrix"/> when null. Tests pass their own.</param>
    internal SaveSession(byte[] source, SaveFile save, string fileName, SaveCapabilities? capabilities = null)
    {
        original = source;
        FileName = fileName;
        Working = save;
        Capabilities = capabilities ?? SaveCapabilities.For(save);
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
        return new EditorDraft(SessionId, slot, Revision, entity, Capabilities);
    }

    /// <summary>
    /// Writes the entity to its slot on a staged save during <see cref="Apply"/>. Tests replace it to inject failures; the default is
    /// Core's slot write with <see cref="EntityImportSettings.None"/>.
    /// </summary>
    /// <remarks>
    /// The default import settings would also mark the Pokédex, bump trainer records and rewrite handler data as if the entity were traded in.
    /// </remarks>
    internal Func<SaveFile, ISlotInfo, PKM, bool> StagedWriter { get; set; } = static (save, slot, entity) => slot.WriteTo(save, entity, EntityImportSettings.None);

    /// <summary>
    /// Writes a draft to its source slot. The write is staged on a clone of the working save and swapped in only after it is verified.
    /// </summary>
    /// <remarks>
    /// <para>The stages, in order; a refusal at any of them leaves the session exactly as it was:</para>
    /// <list type="number">
    /// <item>The draft must belong to this session, be taken from the current revision and be of a position this release writes.</item>
    /// <item>A party member must be stored with party stats, which Core would otherwise recalculate on write, restoring its HP and clearing its status.</item>
    /// <item>Core must allow the slot and the entity (<see cref="ISlotInfo.CanWriteTo(SaveFile, PKM)"/>).</item>
    /// <item>The entity is written to a clone of the working save with <see cref="EntityImportSettings.None"/>.</item>
    /// <item>The slot must read back as exactly the drafted entity (<see cref="StoresExactly"/>), party stats, HP and status included.</item>
    /// <item>The party count must be unchanged, and every party position and box slot other than the target must keep every byte (<see cref="SlotImages"/>).</item>
    /// </list>
    /// <para>
    /// The verified clone then replaces the working save and the revision advances. A draft with no changes is not a change: nothing is
    /// written and the revision stays. A changed draft always changes the slot, since it must read back exactly.
    /// </para>
    /// </remarks>
    /// <exception cref="SessionException">
    /// The draft is foreign or stale, its position cannot be written by this release (<see cref="SaveCapabilities.CanApply"/>), the party
    /// member has no stored stats, the slot cannot be written, or the staged write fails verification.
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

        // Checked on the stored member, not the draft: a stat edit gives the draft stats, recalculated from no previous HP.
        if (draft.Slot.IsParty && ReadOccupied(Working, draft.Slot) is not { PartyStatsPresent: true })
        {
            throw new SessionException(SessionError.PartyStatsMissing);
        }
        var entity = draft.ToStoredEntity();
        var candidate = Working.Clone();
        var slot = draft.Slot.ToSlotInfo(candidate);
        if (!slot.CanWriteTo(candidate) || slot.CanWriteTo(candidate, entity) != WriteBlockedMessage.None)
        {
            throw new SessionException(SessionError.SlotNotWritable);
        }

        var before = SlotImages.Of(Working);
        // The writer is handed a copy, so nothing it does to the entity can change what the read-back is compared with.
        if (!StagedWriter(candidate, slot, entity.Clone()))
        {
            throw new SessionException(SessionError.StagedWriteFailed);
        }
        // The slot must now hold exactly the drafted entity: every byte, not only the edited fields.
        if (!StoresExactly(draft.Slot.ToSlotInfo(candidate).Read(candidate), entity, draft.Slot.IsParty))
        {
            throw new SessionException(SessionError.StagedEditMismatch);
        }
        if (candidate.PartyCount != Working.PartyCount)
        {
            throw new SessionException(SessionError.PartyCountChanged);
        }
        if (SlotImages.FirstUntargetedChange(before, SlotImages.Of(candidate), draft.Slot) is not null)
        {
            throw new SessionException(SessionError.UntargetedSlotChanged);
        }

        Working = candidate;
        Revision++;
        HasChangesSinceOpen = true;
    }

    /// <summary>
    /// True when <paramref name="stored"/>, read back from a slot, passes its checksum and holds the same bytes as <paramref name="expected"/>.
    /// </summary>
    /// <remarks>
    /// A party member is compared in the party format, so its battle stats, current HP and status are compared too, and a write that healed
    /// it or recalculated its stats does not match. A boxed entity is compared in the stored format, the only bytes a box slot holds.
    /// </remarks>
    /// <param name="stored">The entity read back from the slot.</param>
    /// <param name="expected">The entity that was written.</param>
    /// <param name="party">True when the slot is a party position.</param>
    internal static bool StoresExactly(PKM stored, PKM expected, bool party)
    {
        if (!stored.ChecksumValid)
        {
            return false;
        }
        var reference = expected.Clone();
        reference.RefreshChecksum();
        var size = party ? reference.SIZE_PARTY : reference.SIZE_STORED;
        return stored.Data.Length >= size && reference.Data.Length >= size && stored.Data[..size].SequenceEqual(reference.Data[..size]);
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
