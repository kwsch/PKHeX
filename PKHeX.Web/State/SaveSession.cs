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

    /// <summary>Incremented once for each apply that changed the working save.</summary>
    public int Revision { get; private set; }

    /// <summary>True once any edit has been applied since the file was opened.</summary>
    public bool HasChangesSinceOpen { get; private set; }

    /// <summary>The <see cref="Revision"/> of the last export whose download was started, or null if none.</summary>
    public int? ExportedRevision { get; private set; }

    /// <summary>Display name of the save family.</summary>
    /// <remarks>Only XY and ORAS saves pass the <see cref="Services.SaveLoader"/> allowlist, so anything not XY is ORAS.</remarks>
    public string Family => Working is SAV6XY ? "XY" : "ORAS";

    /// <summary>Flat box-slot indexes that held an entity when the session was opened.</summary>
    public IReadOnlyList<int> OccupiedSlots { get; }

    internal SaveSession(byte[] source, SaveFile save)
    {
        original = source;
        Working = save;
        OccupiedSlots = Enumerable.Range(0, save.SlotCount)
            .Where(i => GetSlot(save, i).Read(save).Species != 0).ToArray();
    }

    /// <summary>Returns a copy of the bytes the session was opened from.</summary>
    public byte[] GetOriginalBytes() => original.ToArray();

    /// <summary>Human-readable box/slot label for a flat box-slot index.</summary>
    public string SlotLabel(int index) => $"Box {(index / Working.BoxSlotCount) + 1}, slot {(index % Working.BoxSlotCount) + 1}";

    /// <summary>
    /// Starts a draft from an occupied box slot of the current revision.
    /// </summary>
    /// <exception cref="InvalidDataException">The slot is not occupied, or its entity fails its checksum.</exception>
    public EditorDraft Select(int index)
    {
        if (!OccupiedSlots.Contains(index))
        {
            throw new InvalidDataException("Choose an occupied box slot.");
        }
        var entity = (PK6)GetSlot(Working, index).Read(Working);
        if (!entity.ChecksumValid)
        {
            throw new InvalidDataException("The selected Pokémon has an invalid checksum.");
        }
        return new EditorDraft(SessionId, index, Revision, entity, Working.MaxStringLengthNickname);
    }

    /// <summary>
    /// Writes a draft to its source slot. The write is staged on a clone of the working save and swapped in only after it is verified.
    /// </summary>
    /// <remarks>A draft with no changes is ignored and does not count as a change.</remarks>
    /// <exception cref="InvalidDataException">The draft belongs to another session or is stale, the slot cannot be written, or verification fails.</exception>
    public void Apply(EditorDraft draft)
    {
        EnsureOwns(draft);
        if (!draft.IsDirty)
        {
            return;
        }
        EnsureCurrent(draft);

        var entity = draft.ToStoredEntity();
        var candidate = Working.Clone();
        var slot = GetSlot(candidate, draft.SlotIndex);
        if (!slot.CanWriteTo(candidate) || slot.CanWriteTo(candidate, entity) != WriteBlockedMessage.None)
        {
            throw new InvalidDataException("The selected slot cannot be edited.");
        }
        // The default import settings also mark the Pokédex, bump trainer records and rewrite handler data as if traded in.
        if (!slot.WriteTo(candidate, entity, EntityImportSettings.None))
        {
            throw new InvalidDataException("The staged slot write failed.");
        }
        var stored = slot.Read(candidate);
        if (!stored.ChecksumValid || stored.Nickname != entity.Nickname || stored.IsNicknamed != entity.IsNicknamed)
        {
            throw new InvalidDataException("The staged edit failed validation.");
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
    /// <exception cref="InvalidDataException">The draft does not belong to this session.</exception>
    internal void EnsureOwns(EditorDraft draft)
    {
        if (draft.SessionId != SessionId)
        {
            throw new InvalidDataException("The draft does not belong to the open save. Select the Pokémon again.");
        }
    }

    /// <summary>
    /// Rejects a draft taken from an earlier revision.
    /// </summary>
    /// <exception cref="InvalidDataException">The draft is stale.</exception>
    internal void EnsureCurrent(EditorDraft draft)
    {
        if (draft.SourceRevision != Revision)
        {
            throw new InvalidDataException("The draft is out of date. Select the Pokémon again.");
        }
    }

    /// <summary>Resolves a flat box-slot index to its slot on <paramref name="save"/>.</summary>
    internal static SlotInfoBox GetSlot(SaveFile save, int index) => new(index / save.BoxSlotCount, index % save.BoxSlotCount, save);
}
