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

    /// <summary>The language is not one the save's game can give a Pokémon (see <see cref="SaveCapabilities.Lists"/>).</summary>
    LanguageNotAvailable,

    /// <summary>The friendship value is outside 0–255. It is refused, never clamped.</summary>
    FriendshipOutOfRange,

    /// <summary>The Pokémon has no handling trainer, so it has no friendship towards one to edit.</summary>
    NoHandlingTrainer,

    /// <summary>The level is outside 1–100. It is refused, never clamped.</summary>
    LevelOutOfRange,

    /// <summary>
    /// The experience points are negative or above the most the species' growth rate counts (the level 100 threshold). They are refused,
    /// never clamped.
    /// </summary>
    ExperienceOutOfRange,

    /// <summary>The nature is not one of the game's natures (see <see cref="SaveCapabilities.Lists"/>).</summary>
    NatureNotAvailable,

    /// <summary>The individual value is outside 0 to the format's maximum (31). It is refused, never clamped.</summary>
    IvOutOfRange,

    /// <summary>The effort value is outside 0 to the format's maximum for one stat (252). It is refused, never clamped.</summary>
    EvOutOfRange,

    /// <summary>
    /// The effort value would raise the total of all six above the most a Pokémon can hold (510). An edit that lowers the total is accepted
    /// even while it stays above the limit, so a stored total over it can be brought down one stat at a time.
    /// </summary>
    EvTotalAboveLimit,

    /// <summary>The item is not one the save's game lets a Pokémon hold (see <see cref="SaveCapabilities.Lists"/>).</summary>
    ItemNotAvailable,

    /// <summary>The move is not one of the save's game's moves (see <see cref="SaveCapabilities.Lists"/>).</summary>
    MoveNotAvailable,

    /// <summary>The move slot is empty, so it has no PP or PP Ups to change.</summary>
    MoveSlotEmpty,

    /// <summary>The PP is outside 0 to the move's PP with its PP Ups. It is refused, never clamped.</summary>
    PpOutOfRange,

    /// <summary>The number of PP Ups is outside 0–3. It is refused, never clamped.</summary>
    PpUpsOutOfRange,

    /// <summary>PP Ups cannot be used on the slot's move (such as Sketch), so it can have none.</summary>
    PpUpsNotAllowed,

    /// <summary>The ability slot is not one the species and form have (first, second or hidden).</summary>
    AbilitySlotNotAvailable,

    /// <summary>The gender is not one the species can have (such as female for an always-male species).</summary>
    GenderNotAvailable,

    /// <summary>The species is not one the save's game can hold (see <see cref="SaveCapabilities.SpeciesChoices"/>).</summary>
    SpeciesNotAvailable,

    /// <summary>
    /// The form is not one the species' form list offers (see <see cref="EditorDraft.FormChoices"/>); a species without alternate forms
    /// takes only form 0.
    /// </summary>
    FormNotAvailable,

    /// <summary>
    /// The Pokémon is an egg. Its name must be the game's egg name, and its friendship field holds the hatch counter, so this release
    /// edits none of its fields.
    /// </summary>
    EggNotEditable,

    /// <summary>
    /// The draft as it is now has no legality result yet (<see cref="LegalityGate.Waiting"/>), so it cannot be applied: an apply never passes over
    /// a result the user has not seen.
    /// </summary>
    LegalityNotCurrent,

    /// <summary>Legality reported the draft as Invalid or Unavailable, and the user has not acknowledged it (<see cref="LegalityGate.NeedsAcknowledgement"/>).</summary>
    LegalityNotAcknowledged,

    /// <summary>
    /// The session holds applied changes that legality reported as Invalid or Unavailable, and the user has not acknowledged them for the current
    /// revision (<see cref="SaveSession.ExportNeedsAcknowledgement"/>).
    /// </summary>
    ExportNotAcknowledged,

    /// <summary>
    /// The bytes the session was opened from no longer open (<see cref="Services.SaveLoader"/> refused them), so it could not be reset to them.
    /// The session was kept as it was.
    /// </summary>
    ResetFailed,

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
