using System.Globalization;
using PKHeX.Web.Services;
using PKHeX.Web.State;

namespace PKHeX.Web.Components;

/// <summary>
/// The text for a session's change and download state, the steps of leaving a session (replace, close, reset, discard), the download name choice
/// and the acknowledgement of flagged changes in a download.
/// </summary>
/// <remarks>
/// No text claims a file was saved: the browser reports only that a download started, so every download message asks the user to verify the file.
/// </remarks>
public static class SessionStatusText
{
    /// <summary>Shown after a download is handed to the browser.</summary>
    public const string DownloadStarted = "Download started — verify your file. The session remains temporary.";

    /// <summary>Whether the session differs from the file it was opened from. It stays "edited" after a download.</summary>
    public static string Changes(SaveSession session) => session.HasChangesSinceOpen ? "Edited in memory" : "Unmodified session";

    /// <summary>Whether the applied changes are covered by a started download.</summary>
    public static string For(ExportStatus status) => status switch
    {
        ExportStatus.Unchanged => "No changes to download.",
        ExportStatus.NotExported => "Changes not downloaded yet.",
        ExportStatus.ExportedCurrent => "Download started for the current changes — verify your file.",
        ExportStatus.ChangedSinceExport => "Changed since the last download.",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
    };

    /// <summary>The heading of the exit panel, naming what the user asked for.</summary>
    public static string ExitTitle(SessionExit exit) => exit.Intent switch
    {
        ExitIntent.Close => "Close this save?",
        ExitIntent.Reset => "Reset to the file as opened?",
        ExitIntent.Discard => "Discard this session?",
        _ => $"Open {exit.Candidate!.FileName}?",
    };

    /// <summary>What the current exit step asks of the user.</summary>
    /// <remarks><see cref="ExitStage.ConfirmDiscard"/> names what will be lost, so its prompt comes from <see cref="DiscardPrompt"/>.</remarks>
    public static string ExitPrompt(ExitStage stage) => stage switch
    {
        ExitStage.ResolveDraft => "The editor has changes that are not applied. Apply them to the save, or discard them.",
        ExitStage.ResolveSession => "This save has changes that have not been downloaded. Download it first, or discard the session and lose them.",
        ExitStage.ConfirmExport => "A download of the current changes was started. Check that the file was saved before you continue; the session will be gone.",
        ExitStage.Ready => "Nothing will be lost.",
        _ => "",
    };

    /// <summary>What confirming a discard loses: the draft, the session's changes, or both, and whether a download holds the changes.</summary>
    /// <param name="draftPending">True when the draft has unapplied or refused changes.</param>
    /// <param name="status">The session's download status.</param>
    public static string DiscardPrompt(bool draftPending, ExportStatus status)
    {
        var changes = status switch
        {
            ExportStatus.NotExported => "the changes applied to this save, which have not been downloaded",
            ExportStatus.ChangedSinceExport => "the changes applied to this save, including some made after the last download",
            ExportStatus.ExportedCurrent => "this session; a download of its current changes was started, so check that file before you discard",
            _ => null,
        };
        var lost = (draftPending, changes) switch
        {
            (true, null) => "the unapplied changes in the editor",
            (true, { } c) => $"the unapplied changes in the editor and {c}",
            (false, { } c) => c,
            _ => "this session",
        };
        var keep = draftPending ? "cancel, apply the draft and download the save first" : "cancel and download the save first";
        return $"Discarding loses {lost}. It cannot be undone. To keep your changes, {keep}.";
    }

    /// <summary>The button that completes the exit once the download was checked, or once nothing would be lost.</summary>
    public static string Continue(SessionExit exit, ExitStage stage) => stage == ExitStage.ConfirmExport
        ? "Continue; I have checked my export"
        : exit.Intent switch
        {
            ExitIntent.Close or ExitIntent.Discard => "Close save",
            ExitIntent.Reset => "Reset to original",
            _ => $"Open {exit.Candidate!.FileName}",
        };

    /// <summary>The destructive button that completes the exit without a download.</summary>
    public static string DiscardSession(SessionExit exit) => exit.Intent switch
    {
        ExitIntent.Close => "Discard session and close",
        ExitIntent.Reset => "Discard changes and reset",
        ExitIntent.Discard => "Discard session",
        _ => $"Discard session and open {exit.Candidate!.FileName}",
    };

    /// <summary>What Reset to original does, shown beside it.</summary>
    public const string ResetNote = "Reset to original reopens the file as you opened it: every applied change, the draft and the download status are cleared. You are offered a download first.";

    /// <summary>What Discard session does, shown beside it.</summary>
    public const string DiscardNote = "Discard session closes this save without offering a download, after one confirmation. Close save offers to apply and download first.";

    /// <summary>Shown once an exit's reset completed.</summary>
    public const string ResetDone = "Reset to the file as opened. The draft, applied changes and download status were cleared.";

    /// <summary>Shown once a discard completed.</summary>
    public const string Discarded = "Session discarded. Choose a save to begin.";

    /// <summary>Introduces the list of flagged changes a download contains.</summary>
    public static string FlaggedIntro(int count) => count == 1
        ? "This download contains 1 applied change that legality reported a problem with:"
        : $"This download contains {count.ToString(CultureInfo.InvariantCulture)} applied changes that legality reported problems with:";

    /// <summary>One flagged change: its position and the verdict it was applied with.</summary>
    public static string Flagged(SlotRef slot, LegalityVerdict verdict) => verdict == LegalityVerdict.Invalid
        ? $"{SlotText.Position(slot)}: Invalid"
        : $"{SlotText.Position(slot)}: legality could not be analysed";

    /// <summary>The acknowledgement that allows the download of flagged changes. It is withdrawn by any later apply.</summary>
    public const string ExportAcknowledgement = "Download them anyway. Nothing is repaired, and games or online services may refuse them.";

    /// <summary>The label of a name choice, showing the name it would give now.</summary>
    public static string NameOption(ExportNameChoice choice, string name) => choice == ExportNameChoice.Edited
        ? $"Edited name: {name} (stamped with the time of the download)"
        : $"Original name: {name}";

    /// <summary>Shown when the chosen name is not the one a console or emulator restores from.</summary>
    public const string RenameToRestore = "To restore this file with a save manager on the console, or in an emulator, rename it to main first.";
}
