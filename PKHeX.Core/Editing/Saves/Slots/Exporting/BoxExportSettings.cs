using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Settings for exporting boxes
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed record BoxExportSettings
{
    /// <summary>
    /// Default settings for regular box exports.
    /// </summary>
    public static BoxExportSettings Default => new();

    /// <summary>
    /// Scope of the export -- all boxes or just the current box
    /// </summary>
    [Category("Export")]
    public BoxExportScope Scope { get; set; }

    /// <summary>
    /// Folder creation mode -- no folders, or a folder for each box
    /// </summary>
    [Category("Folder")]
    public BoxExportFolderMode FolderCreation { get; set; }

    /// <summary>
    /// Naming mode for folders
    /// </summary>
    /// <remarks>
    /// only used if <see cref="FolderCreation"/> is set to <see cref="BoxExportFolderMode.FolderEachBox"/>
    /// </remarks>
    [Category("Folder")]
    public BoxExportFolderNaming FolderPrefix { get; set; }

    /// <summary>
    /// Empty slot mode -- skip empty slots or include them in the export
    /// </summary>
    [Category("File")]
    public BoxExportEmptySlots EmptySlots { get; set; }

    /// <summary>
    /// File index prefix mode -- no prefix, or a prefix for each file
    /// </summary>
    [Category("File")]
    public BoxExportIndexPrefix FileIndexPrefix { get; set; }

    /// <summary>
    /// Export notification settings -- whether to notify the user of the export
    /// </summary>
    [Category("Export")]
    public BoxExportNofify Notify { get; set; }
}

/// <summary>
/// Export scope for boxes
/// </summary>
public enum BoxExportScope : byte
{
    /// <summary>
    /// All boxes will be exported
    /// </summary>
    All = 0,

    /// <summary>
    /// Only the current box will be exported
    /// </summary>
    Current = 1,
}

/// <summary>
/// Export folder creation mode
/// </summary>
public enum BoxExportFolderMode : byte
{
    /// <summary>
    /// No folders will be created; all files will be in the same directory
    /// </summary>
    None = 0,

    /// <summary>
    /// A folder will be created for each box
    /// </summary>
    /// <remarks>
    /// Settings will reference the associated <see cref="BoxExportFolderNaming"/> value
    /// </remarks>
    FolderEachBox = 1,
}

/// <summary>
/// Export folder naming mode
/// </summary>
/// <remarks>
/// only used if <see cref="BoxExportFolderMode.FolderEachBox"/> is selected
/// </remarks>
public enum BoxExportFolderNaming : byte
{
    /// <summary>
    /// The folder will be named after the box name
    /// </summary>
    BoxName = 0,

    /// <summary>
    /// The folder will be named after the box index
    /// </summary>
    Index = 1,

    /// <summary>
    /// The folder will be named after the box index and box name
    /// </summary>
    IndexBoxName = 2,
}

/// <summary>
/// Export empty slots mode
/// </summary>
public enum BoxExportEmptySlots : byte
{
    /// <summary>
    /// Empty/Invalid slots will be skipped
    /// </summary>
    Skip = 0,

    /// <summary>
    /// Empty/Invalid slots will be included in the export
    /// </summary>
    Include = 1,
}

/// <summary>
/// Export file index prefix mode
/// </summary>
public enum BoxExportIndexPrefix : byte
{
    /// <summary>
    /// No prefix will be added to the file name
    /// </summary>
    None = 0,

    /// <summary>
    /// The file name will be prefixed with the box index
    /// </summary>
    InBox = 1,

    /// <summary>
    /// The file name will be prefixed with the box index and slot index
    /// </summary>
    InAll = 2,
}

/// <summary>
/// Export notification mode
/// </summary>
public enum BoxExportNofify : byte
{
    /// <summary>
    /// Notify the user of the export result
    /// </summary>
    NotifyResult = 0,

    /// <summary>
    /// Do not notify the user of the export result
    /// </summary>
    Silent = 1,
}
