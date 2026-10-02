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
}
