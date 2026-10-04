using PKHeX.Web.State;

namespace PKHeX.Web.Components;

/// <summary>One edit of the draft from the editor, with the id of the control it came from, so a refusal can be shown on that control.</summary>
/// <param name="FieldId">The id of the editor control (see <see cref="EditorFields"/>).</param>
/// <param name="Edit">The draft edit.</param>
public sealed record DraftEdit(string FieldId, Action<EditorDraft> Edit);
