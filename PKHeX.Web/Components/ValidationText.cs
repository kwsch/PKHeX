using PKHeX.Web.State;

namespace PKHeX.Web.Components;

/// <summary>One reason in an error summary: a sentence, and where present, a link that moves focus to the control that resolves it.</summary>
/// <param name="Text">The reason.</param>
/// <param name="LinkText">The link's text, or null when there is nowhere to go (the reason resolves itself or cannot be resolved here).</param>
/// <param name="TargetId">The id of the element the link focuses, or null with <paramref name="LinkText"/>.</param>
public sealed record SummaryItem(string Text, string? LinkText = null, string? TargetId = null);

/// <summary>
/// Text for validation (WEB-A11Y-002): a refused field's error line, and the error summary shown when Apply or Download is activated while it
/// cannot act (<see cref="ActionReadiness"/>).
/// </summary>
public static class ValidationText
{
    /// <summary>The error line of a field whose edit failed unexpectedly rather than being refused.</summary>
    public const string FieldFailed = "This change could not be made, so the draft was not changed.";

    /// <summary>The heading of the summary shown when Apply is activated but cannot apply.</summary>
    public const string ApplyTitle = "The changes cannot be applied yet";

    /// <summary>The heading of the summary shown when Download is activated but cannot download.</summary>
    public const string DownloadTitle = "The save cannot be downloaded yet";

    /// <summary>Why <paramref name="refusal"/>'s edit was refused.</summary>
    public static string FieldError(FieldRefusal refusal) => refusal.Error is { } error ? UserMessages.For(error) : FieldFailed;

    /// <summary>The summary entry for <paramref name="blocker"/>.</summary>
    /// <param name="blocker">Why Apply cannot apply.</param>
    /// <param name="refusal">The refused edit, for <see cref="ApplyBlocker.FieldRefused"/>.</param>
    /// <param name="verdict">The verdict to acknowledge, for <see cref="ApplyBlocker.LegalityNotAcknowledged"/>.</param>
    public static SummaryItem For(ApplyBlocker blocker, FieldRefusal? refusal, LegalityVerdict verdict) => blocker switch
    {
        ApplyBlocker.Busy => new(Busy),
        ApplyBlocker.NotWritable => new(UserMessages.For(SessionError.PartyApplyNotAvailable)),
        ApplyBlocker.NoChanges => new("The draft has no changes to apply."),
        ApplyBlocker.FieldRefused => Refused(refusal),
        ApplyBlocker.SpeciesPreviewPending => Pending,
        ApplyBlocker.LegalityWaiting => new(LegalityText.ApplyWaiting),
        ApplyBlocker.LegalityNotAcknowledged => new(
            verdict == LegalityVerdict.Unavailable
                ? "Legality could not analyse this draft. Acknowledge that to apply it without a result, or change the draft."
                : "Legality reports this draft as Invalid. Acknowledge that to apply it anyway, or change the draft.",
            "Go to the acknowledgement", "apply-ack"),
        _ => throw new ArgumentOutOfRangeException(nameof(blocker), blocker, null),
    };

    /// <summary>The summary entry for <paramref name="blocker"/>.</summary>
    /// <param name="blocker">Why Download cannot download.</param>
    /// <param name="refusal">The refused edit, for <see cref="DownloadBlocker.FieldRefused"/>.</param>
    public static SummaryItem For(DownloadBlocker blocker, FieldRefusal? refusal) => blocker switch
    {
        DownloadBlocker.Busy => new(Busy),
        DownloadBlocker.FieldRefused => Refused(refusal),
        DownloadBlocker.SpeciesPreviewPending => Pending,
        DownloadBlocker.DraftNotApplied => new("The editor has changes that are not applied, which the download would not hold. Apply or cancel them first.", "Go to Apply changes", "apply"),
        DownloadBlocker.ExportNotAcknowledged => new(UserMessages.For(SessionError.ExportNotAcknowledged), "Go to the acknowledgement", "export-ack"),
        _ => throw new ArgumentOutOfRangeException(nameof(blocker), blocker, null),
    };

    private const string Busy = "A download is being prepared. Wait for it to finish.";

    private static SummaryItem Pending => new(EditorText.SpeciesFormPending, "Go to the species change", "species-confirm");

    private static SummaryItem Refused(FieldRefusal? refusal) => refusal is null
        ? new(FieldFailed)
        : new($"The last change was refused: {FieldError(refusal)}", "Go to the field", refusal.FieldId);
}
