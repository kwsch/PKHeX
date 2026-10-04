using PKHeX.Web.State;

namespace PKHeX.Web.Components;

/// <summary>
/// Where a refused edit is shown in the editor (WEB-A11Y-002): the refused control is marked invalid, and its fieldset's error line, placed
/// after the fieldset's controls, says why. Every control of a fieldset shares one line, since only the last edit can be refused.
/// </summary>
public static class EditorFields
{
    /// <summary>
    /// The id of the fieldset holding the editor control <paramref name="fieldId"/>, as <see cref="DraftEditor"/> renders it, or null for an
    /// id the editor does not have.
    /// </summary>
    public static string? FieldsetOf(string fieldId) => fieldId switch
    {
        "species" or "form" => "species-fields",
        "nickname" or "nicknamed" or "language" => "name-fields",
        "ot-friendship" or "ht-friendship" => "friendship-fields",
        "level" or "exp" => "level-fields",
        "nature" => "nature-fields",
        "ability" or "gender" => "ability-fields",
        "held-item" => "item-fields",
        _ when IsIndexed(fieldId, "iv-") || IsIndexed(fieldId, "ev-") => "stat-fields",
        _ when IsIndexed(fieldId, "move-") || IsIndexed(fieldId, "pp-") || IsIndexed(fieldId, "ppups-") => "move-fields",
        _ => null,
    };

    /// <summary>The id of the error line of the fieldset <paramref name="fieldset"/>.</summary>
    public static string ErrorId(string fieldset) => $"{fieldset}-error";

    /// <summary>The id of the error line that describes <paramref name="refusal"/>'s control, or null when the editor has no such control.</summary>
    public static string? ErrorIdFor(FieldRefusal refusal) => FieldsetOf(refusal.FieldId) is { } fieldset ? ErrorId(fieldset) : null;

    private static bool IsIndexed(string id, string prefix) =>
        id.Length == prefix.Length + 1 && id.StartsWith(prefix, StringComparison.Ordinal) && char.IsAsciiDigit(id[^1]);
}
