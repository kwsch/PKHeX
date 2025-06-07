using System;

namespace PKHeX.Core;

/// <summary>
/// Flags to control the export of binary data.
/// </summary>
[Flags]
public enum BinaryExportSetting
{
    /// <summary>
    /// Export the complete binary file with all sections included.
    /// </summary>
    None,

    /// <summary>
    /// Exclude the footer section from the exported file.
    /// </summary>
    ExcludeFooter = 1 << 0,

    /// <summary>
    /// Exclude the header section from the exported file.
    /// </summary>
    ExcludeHeader = 1 << 1,

    /// <summary>
    /// Do not perform finalization steps when exporting.
    /// </summary>
    /// <remarks>
    /// When this flag is set, the export skips any finalization logic such as updating the header or footer segments.
    /// The exact steps skipped depend on the file type and export implementation.
    /// See <see cref="ISaveHandler.Finalize"/> implementations for details.
    /// </remarks>
    ExcludeFinalize = 1 << 2,
}
