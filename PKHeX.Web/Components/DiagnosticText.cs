namespace PKHeX.Web.Components;

/// <summary>
/// The text of the diagnostic report panel. The report itself is built by <see cref="Services.Diagnostics.DiagnosticReport"/>.
/// </summary>
public static class DiagnosticText
{
    /// <summary>The panel's heading.</summary>
    public const string Title = "Diagnostic report";

    /// <summary>What a report is for, what it holds and what it never holds; shown before the user asks for one.</summary>
    public const string Intro = "If something went wrong, a diagnostic report helps identify it. It names this build, your browser and the problems recorded in this tab, by code only. "
        + "It never includes your save, its file name, Pokémon or trainer names, or IDs, and nothing is sent anywhere: you choose whether to copy or download it, and where to share it.";

    /// <summary>The button that builds the report and shows it.</summary>
    public const string Prepare = "Prepare a diagnostic report";

    /// <summary>Shown above the preview.</summary>
    public const string PreviewNote = "This is the whole report. Check it before sharing it, and do not attach your save file or screenshots that show names.";

    /// <summary>The copy button.</summary>
    public const string Copy = "Copy report";

    /// <summary>The download button.</summary>
    public const string Download = "Download report";

    /// <summary>The button that forgets the recorded problems.</summary>
    public const string Clear = "Clear recorded problems";

    /// <summary>The button that hides the preview.</summary>
    public const string Close = "Close report";

    /// <summary>The suggested name of a downloaded report.</summary>
    public const string FileName = "pkhex-web-diagnostics.txt";

    /// <summary>Shown after the report was copied.</summary>
    public const string Copied = "Report copied.";

    /// <summary>Shown when the browser refused to copy: the preview is selected instead.</summary>
    public const string CopyRefused = "Your browser did not allow copying. The report is selected; copy it with your keyboard or the browser's menu.";

    /// <summary>Shown after the report's download was handed to the browser.</summary>
    public const string Downloaded = "Report download started.";

    /// <summary>Shown when the download could not be started.</summary>
    public const string DownloadFailed = "The report could not be downloaded. Copy it instead.";

    /// <summary>Shown after the recorded problems were cleared.</summary>
    public const string Cleared = "Recorded problems cleared.";
}
