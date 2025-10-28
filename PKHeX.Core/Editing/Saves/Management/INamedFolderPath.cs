namespace PKHeX.Core;

/// <summary>
/// Describes details about a folder and other special metadata.
/// </summary>
public interface INamedFolderPath
{
    string Path { get; }
    string DisplayText { get; }
    FolderPathGroup Group { get; }
}

/// <summary>
/// Folder path groupings for named folder paths.
/// </summary>
public enum FolderPathGroup
{
    /// <summary>
    /// Path is a standard folder.
    /// </summary>
    None = 0,

    /// <summary>
    /// Path is provided by the user.
    /// </summary>
    Custom = 1,

    /// <summary>
    /// Path is one of the Nintendo 3DS homebrew system folders.
    /// </summary>
    Nintendo3DS = 2,

    /// <summary>
    /// Path is one of the Nintendo Switch homebrew system folders.
    /// </summary>
    NintendoSwitch = 3
}
