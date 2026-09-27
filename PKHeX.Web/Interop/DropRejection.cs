namespace PKHeX.Web.Interop;

/// <summary>
/// Why a drop on a file drop zone was refused before anything was read.
/// </summary>
/// <remarks>Values match the codes <c>browser.js</c> reports; see <see cref="BrowserFileService.RegisterDropZoneAsync"/>.</remarks>
public enum DropRejection
{
    /// <summary>More than one file was dropped.</summary>
    MultipleFiles,

    /// <summary>A folder was dropped.</summary>
    Directory,

    /// <summary>The drop held no file, only text or a link.</summary>
    NotAFile,

    /// <summary>The drop zone's file input was disabled because another operation was running.</summary>
    Busy,
}
