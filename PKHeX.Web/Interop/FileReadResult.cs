namespace PKHeX.Web.Interop;

/// <summary>
/// Outcome of reading one user-chosen file into memory.
/// </summary>
/// <remarks>Statuses only describe the read; recognising the save is a separate step. The UI maps each status to text.</remarks>
public enum FileReadStatus
{
    /// <summary>The whole file was read within the size limit.</summary>
    Ok,

    /// <summary>The file has no content.</summary>
    Empty,

    /// <summary>The file is larger than the input limit, by its declared size or by the bytes actually streamed.</summary>
    TooLarge,

    /// <summary>The browser could not read the file, for example because it changed or was removed after it was chosen.</summary>
    ReadFailed,
}

/// <summary>
/// A file read from the browser.
/// </summary>
/// <param name="Status">Whether the read succeeded, and why not.</param>
/// <param name="FileName">The file's name, already passed through <see cref="Services.FileNaming.Sanitize"/>.</param>
/// <param name="Bytes">The file contents when <paramref name="Status"/> is <see cref="FileReadStatus.Ok"/>; otherwise empty.</param>
public sealed record FileReadResult(FileReadStatus Status, string FileName, byte[] Bytes)
{
    /// <summary>A failed read of <paramref name="fileName"/>, carrying no bytes.</summary>
    public static FileReadResult Failed(FileReadStatus status, string fileName) => new(status, fileName, []);
}
