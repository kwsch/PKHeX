namespace PKHeX.Web.State;

/// <summary>How a session's applied changes relate to its downloads; see <see cref="SaveSession.ExportStatus"/>.</summary>
/// <remarks>
/// A started download is the most the app can know: the browser does not report whether the file was kept, so no value means "saved".
/// </remarks>
public enum ExportStatus
{
    /// <summary>Nothing has been applied since the file was opened, whether or not it was downloaded.</summary>
    Unchanged,

    /// <summary>Changes were applied, and no download has been started since.</summary>
    NotExported,

    /// <summary>A download of the current revision was started.</summary>
    ExportedCurrent,

    /// <summary>A download was started, but changes were applied after it.</summary>
    ChangedSinceExport,
}
