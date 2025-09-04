using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class ReportGridSettings
{
    [LocalizedDescription("Extra entity properties to try and show in addition to the default properties displayed.")]
    public List<string> ExtraProperties { get; set; } = [];

    [LocalizedDescription("Properties to hide from the report grid.")]
    public List<string> HiddenProperties { get; set; } = [];
}
