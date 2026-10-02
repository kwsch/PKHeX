using PKHeX.Core;
using PKHeX.Web.Services;

namespace PKHeX.Web.State;

/// <summary>
/// An unapplied edit of one party member or boxed entity. The working save is untouched until <see cref="SaveSession.Apply"/>.
/// </summary>
/// <remarks>
/// The draft owns a private clone of the slot's entity, and its typed edit methods (such as <see cref="EditNickname"/>) are the only
/// code that changes it. Nothing outside the draft is ever handed that clone: readers get a copy through <see cref="Preview"/>, so an
/// apply carries exactly the changes the edit methods made, and every other stored byte stays as it was read.
/// </remarks>
public sealed class EditorDraft
{
    private readonly PK6 baseline;
    private readonly PK6 working;

    /// <summary><see cref="SaveSession.SessionId"/> of the session the draft was taken from.</summary>
    public Guid SessionId { get; }

    /// <summary>The party position or box slot the draft was taken from.</summary>
    public SlotRef Slot { get; }

    /// <summary>What the session's save allows, which decides the fields that can be drafted and whether the draft can be applied.</summary>
    public SaveCapabilities Capabilities { get; }

    /// <summary>
    /// True when <see cref="SaveSession.Apply"/> can write the draft back (see <see cref="SaveCapabilities.CanApply"/>).
    /// Party members can be applied only in families with a party-stat policy (<see cref="SupportedFamily.WritesParty"/>).
    /// </summary>
    public bool CanApply => Capabilities.CanApply(Slot);

    /// <summary>Fields the user can change in this draft: none when it cannot be applied, so nothing is drafted that could never be kept.</summary>
    public EditableFields Editable => CanApply ? Capabilities.Editable : EditableFields.None;

    /// <summary><see cref="SaveSession.Revision"/> the draft was taken from.</summary>
    public int SourceRevision { get; }

    /// <summary>Longest nickname the save format can store.</summary>
    public int MaxNicknameLength => Capabilities.MaxNicknameLength;

    /// <summary>
    /// Number of edits accepted since the draft was taken. It tags legality results (<see cref="LegalityTag"/>), so a result for an
    /// earlier edit is stale at once. A refused edit leaves it unchanged, because it leaves the draft unchanged.
    /// </summary>
    public int EditRevision { get; private set; }

    /// <summary>Drafted nickname text.</summary>
    public string Nickname => working.Nickname;

    /// <summary>Drafted nickname flag.</summary>
    public bool IsNicknamed => working.IsNicknamed;

    /// <summary>True when any stored byte of the draft differs from the slot it was taken from.</summary>
    /// <remarks>Neither copy has its checksum refreshed in memory, so the checksum bytes cannot make an unchanged draft look dirty.</remarks>
    public bool IsDirty => !working.Data.SequenceEqual(baseline.Data);

    /// <summary>
    /// How applying the draft changes a party member's current and maximum HP (see <see cref="PartyStatPolicy"/>), or null for a box slot
    /// or when neither changes. The UI previews any reduction before the draft is applied.
    /// </summary>
    public PartyHpChange? HpChange => Slot.IsParty ? PartyHpChange.Between(baseline, working) : null;

    internal EditorDraft(Guid sessionId, SlotRef slot, int sourceRevision, PK6 source, SaveCapabilities capabilities)
    {
        SessionId = sessionId;
        Slot = slot;
        SourceRevision = sourceRevision;
        Capabilities = capabilities;
        baseline = (PK6)source.Clone();
        working = (PK6)source.Clone();
    }

    /// <summary>
    /// Replaces the draft's nickname fields. On failure the previous values are kept.
    /// </summary>
    /// <remarks>
    /// Text equal to the stored nickname keeps the stored encoding, including any bytes after the terminator, so returning to the
    /// original text (or changing only the flag) never rewrites the name. New text is encoded by Core.
    /// </remarks>
    /// <exception cref="SessionException">
    /// The family does not allow nickname edits, or the text is too long, contains control characters, or cannot be stored unchanged.
    /// </exception>
    public void EditNickname(string nickname, bool isNicknamed)
    {
        // Gated on the family, not the position: a party draft of a family without party writes can still be edited in memory, and Apply refuses it.
        if (!Capabilities.Editable.HasFlag(EditableFields.Nickname))
        {
            throw new SessionException(SessionError.FieldNotEditable);
        }
        if (nickname.Length > MaxNicknameLength)
        {
            throw new SessionException(SessionError.NicknameTooLong);
        }
        if (nickname.Any(char.IsControl))
        {
            throw new SessionException(SessionError.NicknameInvalidCharacters);
        }
        var candidate = (PK6)working.Clone();
        if (nickname == baseline.Nickname)
        {
            baseline.NicknameTrash.CopyTo(candidate.NicknameTrash);
        }
        else
        {
            candidate.Nickname = nickname;
            if (candidate.Nickname != nickname)
            {
                throw new SessionException(SessionError.NicknameNotRepresentable);
            }
        }
        candidate.IsNicknamed = isNicknamed;
        Commit(candidate, affectsStats: false);
    }

    /// <summary>
    /// Applies an arbitrary change to the draft through the same commit path as the typed edit methods.
    /// </summary>
    /// <remarks>
    /// Test-only: no typed edit method changes stats yet, so this is how tests reach the party-stat recalculation path. It is replaced by
    /// the typed stat edits (level/EXP, nature, IVs/EVs, species/form) as they are added.
    /// </remarks>
    /// <param name="change">The change to make on a copy of the drafted entity.</param>
    /// <param name="affectsStats">Whether the change affects calculated stats, as a typed edit method would declare.</param>
    internal void EditForTest(Action<PK6> change, bool affectsStats)
    {
        var candidate = (PK6)working.Clone();
        change(candidate);
        Commit(candidate, affectsStats);
    }

    /// <summary>
    /// Makes a validated <paramref name="candidate"/> the drafted entity and counts the edit.
    /// </summary>
    /// <remarks>
    /// For a party member, an edit that affects stats has its stats recalculated by <see cref="PartyStatPolicy"/>, so status is kept and
    /// HP is never raised; any other edit keeps the stored stats, HP and status as they are.
    /// </remarks>
    /// <param name="candidate">A changed copy of the drafted entity.</param>
    /// <param name="affectsStats">True for an edit of species/form, level/EXP, nature, IVs or EVs.</param>
    /// <exception cref="SessionException">
    /// <see cref="SessionError.PartyStatsMissing"/>: a stat edit of a party member stored without stats, which has no current HP to keep.
    /// </exception>
    private void Commit(PK6 candidate, bool affectsStats)
    {
        if (affectsStats && Slot.IsParty)
        {
            if (!baseline.PartyStatsPresent)
            {
                throw new SessionException(SessionError.PartyStatsMissing);
            }
            PartyStatPolicy.Recalculate(candidate);
        }
        candidate.Data.CopyTo(working.Data);
        EditRevision++;
    }

    /// <summary>A copy of the drafted entity, for read-only use. Changing it does not change the draft.</summary>
    public PK6 Preview() => (PK6)working.Clone();

    /// <summary>The inspector's view of the drafted entity, including unapplied edits.</summary>
    /// <remarks>
    /// Edits do not refresh the draft's checksum; Core refreshes it when the entity is written. The inspected copy is refreshed the
    /// same way, so the checksum shown is the one an apply would store, not a stale one that would read as corruption.
    /// </remarks>
    public EntityInspection Inspect()
    {
        var copy = Preview();
        copy.RefreshChecksum();
        return EntityInspection.From(copy, Slot, Capabilities);
    }

    /// <summary>The entity to store or analyse: a copy of the drafted entity.</summary>
    internal PK6 ToStoredEntity() => Preview();
}
