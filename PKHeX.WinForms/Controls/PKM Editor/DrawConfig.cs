using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text.Json.Serialization;
using PKHeX.Core;

namespace PKHeX.WinForms;

/// <summary>
/// Drawing Configuration for painting and updating controls
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class DrawConfig
{
    private const string PKM = "Pokémon Editor";

    [JsonIgnore, Browsable(false)] public Color GlowInitial => SystemColors.ActiveCaption;
    [JsonIgnore, Browsable(false)] public Color GlowFinal => SystemColors.Highlight;
    [JsonIgnore, Browsable(false)] public Color MarkDefault => SystemColors.ControlText;

    #region PKM

    [Category(PKM), LocalizedDescription("Background color of a ComboBox when the selected item is not valid.")]
    public Color InvalidSelection { get; set; } = Color.DarkSalmon;

    [Category(PKM), LocalizedDescription("Blue colored marking.")]
    public Color MarkBlue { get; set; } = Color.FromArgb(000, 191, 255);

    [Category(PKM), LocalizedDescription("Pink colored marking.")]
    public Color MarkPink { get; set; } = Color.FromArgb(255, 117, 179);

    [Category(PKM), LocalizedDescription("Shiny star when using unicode characters.")]
    public string ShinyUnicode { get; set; } = "☆";

    [Category(PKM), LocalizedDescription("Shiny star when not using unicode characters.")]
    public string ShinyDefault { get; set; } = "*";

    #endregion

    public bool GetMarkingColor(MarkingColor markval, out Color c)
    {
        switch (markval)
        {
            case MarkingColor.Blue: c = MarkBlue; return true;
            case MarkingColor.Pink: c = MarkPink; return true;
            default: c = MarkDefault; return false; // recolor not required
        }
    }
}
