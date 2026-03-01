using System.Drawing;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed class HoverSettings
{
    [LocalizedDescription("Show PKM Slot Preview on Hover")]
    public bool HoverSlotShowPreview { get; set; } = true;

    [LocalizedDescription("Show Encounter Info on Hover")]
    public bool HoverSlotShowEncounter { get; set; } = true;

    [LocalizedDescription("Show all Encounter Info properties on Hover")]
    public bool HoverSlotShowEncounterVerbose { get; set; }

    [LocalizedDescription("Show first Legality Check message if Illegal on Hover")]
    public bool HoverSlotShowLegalityHint { get; set; } = true;

    [LocalizedDescription("Show PKM Slot ToolTip on Hover")]
    public bool HoverSlotShowText { get; set; } = true;

    [LocalizedDescription("Play PKM Slot Cry on Hover")]
    public bool HoverSlotPlayCry { get; set; } = true;

    [LocalizedDescription("Show a Glow effect around the PKM on Hover")]
    public bool HoverSlotGlowEdges { get; set; } = true;

    [LocalizedDescription("Show Showdown Paste in special Preview on Hover")]
    public bool PreviewShowPaste { get; set; } = true;

    [LocalizedDescription("Show a Glow effect around the PKM on Hover")]
    public Point PreviewCursorShift { get; set; } = new(16, 8);
}
