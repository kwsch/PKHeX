namespace PKHeX.Application.Abstractions;

/// <summary>
/// Framework-free file-dialog glob patterns. The host turns these into platform picker filters;
/// the Application/Presentation layers only deal in the patterns themselves.
/// </summary>
public static class FileDialogFilters
{
    /// <summary>
    /// Patterns for the "Open Save File" dialog. Covers classic saves (*.sav), raw dumps (*.bin),
    /// Switch saves (*.main, or the literal extensionless file name "main"), and anything else via
    /// wildcard — save files come in many names, so the dialog must never block a selection.
    /// </summary>
    public static readonly string[] OpenSaveFile = ["*.sav", "*.bin", "*.main", "main", "*"];
}
