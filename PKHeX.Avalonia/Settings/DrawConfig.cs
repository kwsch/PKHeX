using System.Text.Json.Serialization;
using Avalonia.Media;
using PKHeX.Core;

namespace PKHeX.Avalonia.Settings;

public sealed class DrawConfig
{
    [JsonIgnore] public Color GlowInitial => Color.FromRgb(51, 153, 255);
    [JsonIgnore] public Color GlowFinal => Color.FromRgb(0, 120, 215);
    [JsonIgnore] public Color MarkDefault => Colors.Black;

    public Color InvalidSelection { get; set; } = Color.FromRgb(233, 150, 122); // DarkSalmon
    public Color MarkBlue { get; set; } = Color.FromRgb(000, 191, 255);
    public Color MarkPink { get; set; } = Color.FromRgb(255, 117, 179);

    public string ShinyUnicode { get; set; } = "☆";
    public string ShinyDefault { get; set; } = "*";

    public bool GetMarkingColor(MarkingColor markval, out Color c)
    {
        switch (markval)
        {
            case MarkingColor.Blue: c = MarkBlue; return true;
            case MarkingColor.Pink: c = MarkPink; return true;
            default: c = MarkDefault; return false;
        }
    }
}
