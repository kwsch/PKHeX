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
}
