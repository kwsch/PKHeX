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

    /// <summary>The draft is of a party position; this release inspects party members but does not write them yet.</summary>
    PartyApplyNotAvailable,

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

    /// <summary>The nickname is longer than the format can store.</summary>
    NicknameTooLong,

    /// <summary>The nickname contains control characters.</summary>
    NicknameInvalidCharacters,

    /// <summary>The format would store the nickname with different text.</summary>
    NicknameNotRepresentable,

    /// <summary>The exported bytes do not reopen through <see cref="Services.SaveLoader"/>.</summary>
    ExportRevalidationFailed,

    /// <summary>The reopened export has a different family or version from the session.</summary>
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
