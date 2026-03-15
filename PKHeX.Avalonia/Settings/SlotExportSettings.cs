using PKHeX.Core;

namespace PKHeX.Avalonia.Settings;

public sealed class SlotExportSettings
{
    public BoxExportSettings BoxExport { get; set; } = new();
    public string DefaultBoxExportNamer { get; set; } = "";
    public bool AllowBoxDataDrop { get; set; }
}
