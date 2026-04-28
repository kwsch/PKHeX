using System;
using System.ComponentModel;

namespace PKHeX.Core;

public sealed class AdvancedSettings
{
    [LocalizedDescription("Skip the Overwrite prompt when exporting a save file, to always Save As...")]
    public bool SaveExportForceSaveAs { get; set; }

    [LocalizedDescription("Check if the Pokémon in the editor has unsaved changes before exporting the save file.")]
    public bool SaveExportCheckUnsavedEntity { get; set; } = true;

    [LocalizedDescription("Folder path that contains dump(s) of block hash-names. If a specific dump file does not exist, only names defined within the program's code will be loaded.")]
    public string PathBlockKeyList { get; set; } = string.Empty;

    [LocalizedDescription("Hide event variable names for that contain any of the comma-separated substrings below. Removes event values from the GUI that the user doesn't care to view.")]
    public string HideEvent8Contains { get; set; } = string.Empty;

    [LocalizedDescription("Minimum distance threshold that mouse movement must exceed before a drag operation is started from a slot.")]
    public int DragStartThreshold { get; set; } = 0;

    [Browsable(false)]
    public string[] GetExclusionList8() => Array.ConvertAll(HideEvent8Contains.Split(',', StringSplitOptions.RemoveEmptyEntries), z => z.Trim());
}
