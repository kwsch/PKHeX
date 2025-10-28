using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed class DisplaySettings
{
    [LocalizedDescription("Show Unicode gender symbol characters, or ASCII when disabled.")]
    public bool Unicode { get; set; } = true;

    [LocalizedDescription("Don't show the Legality popup if Legal!")]
    public bool IgnoreLegalPopup { get; set; }

    [LocalizedDescription("Display all properties of the encounter (auto-generated) when exporting a verbose report.")]
    public bool ExportLegalityVerboseProperties { get; set; }

    [LocalizedDescription("Always displays the verbose legality report, and inverts the hotkey behavior to instead disable.")]
    public bool ExportLegalityAlwaysVerbose { get; set; }

    [LocalizedDescription("Always skips the prompt option asking if you would like to export a legality report to clipboard.")]
    public bool ExportLegalityNeverClipboard { get; set; }

    [LocalizedDescription("Flag Illegal Slots in Save File")]
    public bool FlagIllegal { get; set; } = true;

    [LocalizedDescription("Focus border indentation for custom drawn image controls.")]
    public int FocusBorderDeflate { get; set; } = 1;

    [LocalizedDescription("Disables the GUI scaling based on Dpi on program startup, falling back to font scaling.")]
    public bool DisableScalingDpi { get; set; }

    [LocalizedDescription("Skips the context menu hotkey requirement and instead always presents the option to check legality of a slot.")]
    public bool SlotLegalityAlwaysVisible { get; set; }
}
