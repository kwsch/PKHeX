using System.Globalization;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;

namespace PKHeX.Web.Components;

/// <summary>The text and icons of the legality panel. Findings and reports are Core's own text; this class only labels and counts them.</summary>
/// <remarks>Nothing here reads Core's localized text to decide anything: every status comes from <see cref="DraftLegality.Status"/>.</remarks>
public static class LegalityText
{
    /// <summary>The status word, which is also the content of <c>#legality-status</c>.</summary>
    public static string Status(LegalityStatus status) => status switch
    {
        LegalityStatus.None or LegalityStatus.NotAnalyzed => "Not analyzed",
        LegalityStatus.Pending => "Pending",
        LegalityStatus.Stale => "Stale",
        LegalityStatus.Valid => "Valid",
        LegalityStatus.Invalid => "Invalid",
        LegalityStatus.Unavailable => "Unavailable",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
    };

    /// <summary>
    /// What a screen reader is told about the legality status: the verdict once the draft as it is now has one, and nothing while it is not
    /// analysed, pending or stale, which come and go as the user types.
    /// </summary>
    public static string Announcement(LegalityStatus status) => status switch
    {
        LegalityStatus.Valid or LegalityStatus.Invalid or LegalityStatus.Unavailable => $"Legality: {Status(status)}",
        _ => "",
    };

    /// <summary>The decorative icon shown before the status word. The word itself always carries the meaning.</summary>
    public static string Icon(LegalityStatus status) => status switch
    {
        LegalityStatus.Valid => "✓",
        LegalityStatus.Invalid => "✗",
        LegalityStatus.Unavailable => "?",
        LegalityStatus.Pending => "…",
        LegalityStatus.Stale => "↻",
        _ => "–",
    };

    /// <summary>The CSS class that colours the status icon.</summary>
    public static string IconClass(LegalityStatus status) => "legality-icon legality-" + status.ToString().ToLowerInvariant();

    /// <summary>One line under the status word.</summary>
    /// <param name="status">The panel's status.</param>
    /// <param name="current">The result for the draft as it is now, if any.</param>
    /// <param name="autoRun">Whether drafts are analysed automatically once idle (<see cref="DraftLegality.AutoRun"/>).</param>
    /// <param name="draftValid">False while the last edit was refused (<see cref="WorkspaceState.DraftValid"/>).</param>
    public static string Summary(LegalityStatus status, LegalityResult? current, bool autoRun, bool draftValid) => status switch
    {
        _ when !draftValid && status is LegalityStatus.Stale or LegalityStatus.NotAnalyzed
            => "The last edit was refused, so the draft does not match the editor. Correct or cancel it to analyse again.",
        LegalityStatus.None or LegalityStatus.NotAnalyzed => "Choose Analyze now to check this Pokémon.",
        LegalityStatus.Pending => "Analysing the drafted Pokémon…",
        LegalityStatus.Stale => autoRun
            ? "The draft changed after the last analysis. It is analysed again once you stop typing."
            : "The draft changed after the last analysis. Choose Analyze now to check it again.",
        LegalityStatus.Unavailable => "Analysis could not complete. This does not mean the Pokémon is legal. No changes were made.",
        LegalityStatus.Valid or LegalityStatus.Invalid when current is not null => Counts(current),
        _ => "",
    };

    /// <summary>"No problems found.", or the number of problems and warnings (e.g. "No problems, 1 warning.").</summary>
    public static string Counts(LegalityResult result)
    {
        if (result.Problems == 0 && result.Warnings == 0)
        {
            return "No problems found.";
        }
        var problems = result.Problems == 0 ? "No problems" : Plural(result.Problems, "problem");
        return result.Warnings == 0 ? $"{problems}." : $"{problems}, {Plural(result.Warnings, "warning")}.";
    }

    /// <summary>The decorative icon before a finding.</summary>
    public static string FindingIcon(Severity severity) => severity == Severity.Invalid ? "✗" : "!";

    /// <summary>The inspector heading id and title a finding can link to, or null when the inspector does not show what it is about.</summary>
    public static (string Id, string Title)? Section(CheckIdentifier identifier) => LegalitySections.For(identifier) switch
    {
        InspectorArea.Identity => ("inspect-identity", "Identity"),
        InspectorArea.Stats => ("inspect-stats", "Stats"),
        InspectorArea.Moves => ("inspect-moves", "Moves"),
        InspectorArea.Origin => ("inspect-origin", "Origin and trainer"),
        InspectorArea.Advanced => ("inspect-advanced", "Advanced"),
        _ => null,
    };

    /// <summary>The Core build that produced the results.</summary>
    public static string Engine => $"Analysed by PKHeX.Core {BuildInfo.CoreVersion}, source commit {Abbreviate(BuildInfo.SourceCommit)}.";

    /// <summary>The note on what a legality result means.</summary>
    public const string Diagnostic = "Legality is diagnostic. Applying does not legalize or change ownership, dex entries, or trainer records.";

    /// <summary>The note that an offline result is not an acceptance promise.</summary>
    public const string NotAGuarantee = "This is not an online acceptance guarantee: games and online services may still refuse a Pokémon this check accepts.";

    /// <summary>Shown while a changed draft waits for the legality result of the draft as it is now, which Apply needs.</summary>
    public const string ApplyWaiting = "Apply is available once legality has analysed the draft as it is now.";

    /// <summary>The acknowledgement that allows applying a draft whose current result is Invalid or Unavailable; any accepted edit withdraws it.</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="verdict"/> is Valid, which needs no acknowledgement.</exception>
    public static string ApplyAcknowledgement(LegalityVerdict verdict) => verdict switch
    {
        LegalityVerdict.Invalid => "Legality reports this draft as Invalid. Apply it anyway; nothing is repaired.",
        LegalityVerdict.Unavailable => "Legality could not analyse this draft. Apply it anyway, without a result.",
        _ => throw new ArgumentOutOfRangeException(nameof(verdict), verdict, null),
    };

    private static string Plural(int count, string noun) => count.ToString(CultureInfo.InvariantCulture) + " " + noun + (count == 1 ? "" : "s");

    private static string Abbreviate(string commit) => commit.Length > 12 ? commit[..12] : commit;
}
