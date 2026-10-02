namespace PKHeX.Web.State;

/// <summary>
/// Entity fields this release lets the user draft. Every other field is shown read-only and kept exactly as stored.
/// </summary>
/// <remarks>Later editor chunks add a value per field group, together with the draft's typed edit method and its tests.</remarks>
[Flags]
public enum EditableFields
{
    /// <summary>Nothing can be drafted.</summary>
    None = 0,

    /// <summary>The nickname text and the nickname flag (<see cref="EditorDraft.EditNickname"/>).</summary>
    Nickname = 1 << 0,

    /// <summary>The language (<see cref="EditorDraft.EditLanguage"/>), which also decides the default name of a Pokémon that is not nicknamed.</summary>
    Language = 1 << 1,

    /// <summary>
    /// Friendship towards the original trainer and towards the handling trainer, each labelled
    /// (<see cref="EditorDraft.EditTrainerFriendship"/>, <see cref="EditorDraft.EditHandlerFriendship"/>).
    /// </summary>
    Friendship = 1 << 2,

    /// <summary>
    /// The level and experience points, kept in step through Core's growth-rate tables (<see cref="EditorDraft.EditLevel"/>,
    /// <see cref="EditorDraft.EditExperience"/>). Both affect calculated stats.
    /// </summary>
    Level = 1 << 3,

    /// <summary>The nature (<see cref="EditorDraft.EditNature"/>), which affects calculated stats. In Generation 6 it is stored apart from the PID.</summary>
    Nature = 1 << 4,

    /// <summary>
    /// The six individual values (<see cref="EditorDraft.EditIv"/>), each refused outside 0 to the format's maximum. They affect calculated
    /// stats, the Hidden Power type and the characteristic.
    /// </summary>
    Ivs = 1 << 5,

    /// <summary>
    /// The six effort values (<see cref="EditorDraft.EditEv"/>), each refused outside 0 to the format's maximum, and refused when an edit
    /// raises their total above the most a Pokémon can hold. They affect calculated stats.
    /// </summary>
    Evs = 1 << 6,

    /// <summary>The held item (<see cref="EditorDraft.EditHeldItem"/>), chosen from Core's list of items the game lets a Pokémon hold, or none.</summary>
    HeldItem = 1 << 7,

    /// <summary>
    /// The four moves (<see cref="EditorDraft.EditMove"/>), each chosen from Core's list of the game's moves, or left empty. A move change
    /// sets that slot's PP as the desktop editor does; no other slot changes.
    /// </summary>
    Moves = 1 << 8,

    /// <summary>
    /// The current PP and PP Ups of each move (<see cref="EditorDraft.EditPp"/>, <see cref="EditorDraft.EditPpUps"/>), within Core's PP for the
    /// move. Neither can be edited for an empty slot.
    /// </summary>
    Pp = 1 << 9,
}
