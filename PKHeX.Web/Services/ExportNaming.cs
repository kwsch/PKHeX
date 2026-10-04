using PKHeX.Web.State;

namespace PKHeX.Web.Services;

/// <summary>Which name a download is offered under.</summary>
public enum ExportNameChoice
{
    /// <summary>The opened name with a modified marker and the local date-time of the download; see <see cref="FileNaming.EditedName"/>.</summary>
    Edited,

    /// <summary>The name the file was opened as, as a console save manager or emulator expects to find it.</summary>
    Original,
}

/// <summary>Chooses the suggested name of a download. The browser may still adjust it when saving.</summary>
public static class ExportNaming
{
    /// <summary>
    /// The choice used until the user picks one: <see cref="ExportNameChoice.Edited"/> once changes have been applied, so an edited download is
    /// never mistaken for the untouched original, and <see cref="ExportNameChoice.Original"/> while the session is unchanged, because its bytes are
    /// the original's.
    /// </summary>
    public static ExportNameChoice DefaultFor(SaveSession session) => session.HasChangesSinceOpen ? ExportNameChoice.Edited : ExportNameChoice.Original;

    /// <summary>The suggested name of a download of <paramref name="session"/> started at <paramref name="localTime"/>.</summary>
    public static string NameFor(SaveSession session, ExportNameChoice choice, DateTime localTime) => choice == ExportNameChoice.Original
        ? session.FileName
        : FileNaming.EditedName(session.FileName, localTime);

    /// <summary>
    /// True when a file saved as <paramref name="name"/> must be renamed before it is restored: XY and ORAS keep their save as <c>main</c>,
    /// and save managers and emulators look for it by that exact name.
    /// </summary>
    public static bool NeedsRenameToRestore(string name) => !string.Equals(name, FileNaming.DefaultSaveName, StringComparison.Ordinal);
}
