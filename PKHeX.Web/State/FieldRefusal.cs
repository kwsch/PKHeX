namespace PKHeX.Web.State;

/// <summary>
/// The draft edit that was last refused: the editor control it came from and why. The draft keeps its values from before the edit, so the
/// control shows input the draft does not hold until it is corrected, another edit is accepted, or the draft is replaced.
/// </summary>
/// <param name="FieldId">The id of the editor control whose input was refused (such as <c>level</c> or <c>iv-3</c>).</param>
/// <param name="Error">Why it was refused, or null when the edit failed unexpectedly rather than being refused.</param>
public sealed record FieldRefusal(string FieldId, SessionError? Error);
