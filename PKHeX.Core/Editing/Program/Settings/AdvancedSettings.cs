using System;
using System.ComponentModel;

namespace PKHeX.Core;

public sealed class AdvancedSettings
{
    [LocalizedDescription("Folder path that contains dump(s) of block hash-names. If a specific dump file does not exist, only names defined within the program's code will be loaded.")]
    public string PathBlockKeyList { get; set; } = string.Empty;

    [LocalizedDescription("Hide event variables below this event type value. Removes event values from the GUI that the user doesn't care to view.")]
    public NamedEventType HideEventTypeBelow { get; set; }

    [LocalizedDescription("Hide event variable names for that contain any of the comma-separated substrings below. Removes event values from the GUI that the user doesn't care to view.")]
    public string HideEvent8Contains { get; set; } = string.Empty;

    [Browsable(false)]
    public string[] GetExclusionList8() => Array.ConvertAll(HideEvent8Contains.Split(',', StringSplitOptions.RemoveEmptyEntries), z => z.Trim());
}
