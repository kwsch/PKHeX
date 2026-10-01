using PKHeX.Web.Services;
using PKHeX.Web.State;

namespace PKHeX.Web.Components;

/// <summary>The text for a session's change and download state, the steps of leaving a session, and the download name choice.</summary>
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
    public static string ExitTitle(SessionExit exit) => exit.Intent == ExitIntent.Close
        ? "Close this save?"
        : $"Open {exit.Candidate!.FileName}?";

    /// <summary>What the current exit step asks of the user.</summary>
    public static string ExitPrompt(ExitStage stage) => stage switch
    {
        ExitStage.ResolveDraft => "The editor has changes that are not applied. Apply them to the save, or discard them.",
        ExitStage.ResolveSession => "This save has changes that have not been downloaded. Download it first, or discard the session and lose them.",
        ExitStage.ConfirmExport => "A download of the current changes was started. Check that the file was saved before you continue; the session will be gone.",
        ExitStage.Ready => "Nothing will be lost.",
        _ => "",
    };

    /// <summary>The button that completes the exit once the download was checked, or once nothing would be lost.</summary>
    public static string Continue(SessionExit exit, ExitStage stage) => stage == ExitStage.ConfirmExport
        ? "Continue; I have checked my export"
        : exit.Intent == ExitIntent.Close ? "Close save" : $"Open {exit.Candidate!.FileName}";

    /// <summary>The destructive button that completes the exit without a download.</summary>
    public static string DiscardSession(SessionExit exit) => exit.Intent == ExitIntent.Close
        ? "Discard session and close"
        : $"Discard session and open {exit.Candidate!.FileName}";

    /// <summary>The label of a name choice, showing the name it would give now.</summary>
    public static string NameOption(ExportNameChoice choice, string name) => choice == ExportNameChoice.Edited
        ? $"Edited name: {name} (stamped with the time of the download)"
        : $"Original name: {name}";

    /// <summary>Shown when the chosen name is not the one a console or emulator restores from.</summary>
    public const string RenameToRestore = "To restore this file with a save manager on the console, or in an emulator, rename it to main first.";
}
