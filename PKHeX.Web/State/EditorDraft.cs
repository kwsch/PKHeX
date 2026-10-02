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
    /// Party members are inspected only until this release has the party-stat policy (stored stats, HP and status).
    /// </summary>
    public bool CanApply => Capabilities.CanApply(Slot);

    /// <summary>Fields the user can change in this draft: none when it cannot be applied, so nothing is drafted that could never be kept.</summary>
    public EditableFields Editable => CanApply ? Capabilities.Editable : EditableFields.None;

    /// <summary><see cref="SaveSession.Revision"/> the draft was taken from.</summary>
    public int SourceRevision { get; }

    /// <summary>Longest nickname the save format can store.</summary>
    public int MaxNicknameLength => Capabilities.MaxNicknameLength;

    /// <summary>Drafted nickname text.</summary>
    public string Nickname => working.Nickname;

    /// <summary>Drafted nickname flag.</summary>
    public bool IsNicknamed => working.IsNicknamed;

    /// <summary>True when any stored byte of the draft differs from the slot it was taken from.</summary>
    /// <remarks>Neither copy has its checksum refreshed in memory, so the checksum bytes cannot make an unchanged draft look dirty.</remarks>
    public bool IsDirty => !working.Data.SequenceEqual(baseline.Data);

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
        // Gated on the family, not the position: a party draft can still be edited in memory, and Apply refuses it.
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
        candidate.Data.CopyTo(working.Data);
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

    /// <summary>
    /// Runs Core legality analysis on the drafted entity, in the context of <paramref name="session"/>'s save and the draft's slot type
    /// (party or box).
    /// </summary>
    /// <exception cref="SessionException">The draft is foreign or stale.</exception>
    public LegalityResult Analyze(SaveSession session)
    {
        session.EnsureOwns(this);
        session.EnsureCurrent(this);
        var save = session.Working;
        var analysis = new LegalityAnalysis(ToStoredEntity(), save.Personal, Slot.ToSlotInfo(save).Type);
        var verdict = !analysis.Parsed ? "Unavailable" : analysis.Valid ? "Valid" : "Invalid";
        return new(verdict, analysis.Report());
    }

    /// <summary>The entity to store: a copy of the drafted entity.</summary>
    internal PK6 ToStoredEntity() => Preview();
}
