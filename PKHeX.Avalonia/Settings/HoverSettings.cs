namespace PKHeX.Avalonia.Settings;

public sealed class HoverSettings
{
    public bool HoverSlotShowPreview { get; set; } = true;
    public bool HoverSlotShowEncounter { get; set; } = true;
    public bool HoverSlotShowEncounterVerbose { get; set; }
    public bool HoverSlotShowLegalityHint { get; set; } = true;
    public bool HoverSlotShowText { get; set; } = true;
    public bool HoverSlotPlayCry { get; set; } = true;
    public bool HoverSlotGlowEdges { get; set; } = true;
    public bool PreviewShowPaste { get; set; } = true;
    public int PreviewCursorShiftX { get; set; } = 16;
    public int PreviewCursorShiftY { get; set; } = 8;
}
