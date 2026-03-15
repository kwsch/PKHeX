namespace PKHeX.Avalonia.Settings;

public sealed class DisplaySettings
{
    public bool Unicode { get; set; } = true;
    public bool IgnoreLegalPopup { get; set; }
    public bool FlagIllegal { get; set; } = true;
    public int FocusBorderDeflate { get; set; } = 1;
    public bool SlotLegalityAlwaysVisible { get; set; }
}
