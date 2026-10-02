namespace PKHeX.Web.State;

/// <summary>
/// Why a session, draft or export operation was refused. The UI maps each value to text; nothing here is shown directly.
/// </summary>
public enum SessionError
{
    /// <summary>The chosen party position or box slot holds no entity, or does not exist in the save.</summary>
    SlotNotOccupied,

    /// <summary>The entity in the chosen slot is a bad egg: it fails its checksum or its sanity check (<see cref="PKHeX.Core.PKM.Valid"/>).</summary>
    EntityInvalid,

    /// <summary>
    /// The draft is of a party position, and the save's family has no party-stat policy in this release
    /// (<see cref="Services.SupportedFamily.WritesParty"/>), so its party members are inspected only.
    /// </summary>
    PartyApplyNotAvailable,

    /// <summary>
    /// The party member is stored without battle stats. Core would recalculate them on write, which also restores its HP and clears its
    /// status, and a stat edit has no current HP to keep, so it is neither edited for stats nor written.
    /// </summary>
    PartyStatsMissing,

    /// <summary>The draft was taken from another session.</summary>
    ForeignDraft,

    /// <summary>The draft was taken from an earlier revision of the session.</summary>
    StaleDraft,

    /// <summary>An export was requested while the draft still has unapplied changes.</summary>
    DraftUnapplied,

    /// <summary>Core refuses to write the entity to its slot.</summary>
    SlotNotWritable,

    /// <summary>The staged slot write reported failure.</summary>
    StagedWriteFailed,

    /// <summary>The entity read back from the staged slot does not match what was written.</summary>
    StagedEditMismatch,

    /// <summary>The staged write changed the number of party members.</summary>
    PartyCountChanged,

    /// <summary>The staged write changed a party position or box slot other than the one the draft was taken from.</summary>
    UntargetedSlotChanged,

    /// <summary>The save's family does not allow this field to be drafted in this release (see <see cref="SaveCapabilities.Editable"/>).</summary>
    FieldNotEditable,

    /// <summary>The nickname is longer than the format can store.</summary>
    NicknameTooLong,

    /// <summary>The nickname contains control characters.</summary>
    NicknameInvalidCharacters,

    /// <summary>The format would store the nickname with different text.</summary>
    NicknameNotRepresentable,

    /// <summary>The exported bytes do not reopen through <see cref="Services.SaveLoader"/>.</summary>
    ExportRevalidationFailed,

    /// <summary>The reopened export has a different family, version or party count from the session.</summary>
    ExportIdentityMismatch,

    /// <summary>The drafted entity in the reopened export fails its checksum or lost its edit.</summary>
    ExportEntityMismatch,
}

/// <summary>
/// A refused session, draft or export operation. Nothing was changed.
/// </summary>
/// <remarks>The message is the <see cref="SessionError"/> name, never display text.</remarks>
/// <param name="error">Why the operation was refused.</param>
public sealed class SessionException(SessionError error) : InvalidOperationException(error.ToString())
{
    /// <summary>Why the operation was refused.</summary>
    public SessionError Error { get; } = error;
}
