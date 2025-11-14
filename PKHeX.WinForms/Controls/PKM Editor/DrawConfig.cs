using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using PKHeX.Core;

namespace PKHeX.WinForms;

/// <summary>
/// Drawing Configuration for painting and updating controls
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class DrawConfig
{
    private const string PKM = "Pokémon Editor";
    private const string Hovering = "Hovering";

    [Category(Hovering), LocalizedDescription("Hovering over a PKM color 1.")]
    public Color GlowInitial => SystemColors.ActiveCaption;

    [Category(Hovering), LocalizedDescription("Hovering over a PKM color 2.")]
    public Color GlowFinal => SystemColors.Highlight;

    #region PKM

    [Category(PKM), LocalizedDescription("Background color of a ComboBox when the selected item is not valid.")]
    public Color InvalidSelection { get; set; } = Color.DarkSalmon;

    [Category(PKM), LocalizedDescription("Default colored marking.")]
    public Color MarkDefault => SystemColors.ControlText;

    [Category(PKM), LocalizedDescription("Blue colored marking.")]
    public Color MarkBlue { get; set; } = Color.FromArgb(000, 191, 255);

    [Category(PKM), LocalizedDescription("Pink colored marking.")]
    public Color MarkPink { get; set; } = Color.FromArgb(255, 117, 179);

    [Category(PKM), LocalizedDescription("Male gender color.")]
    public Color Male { get; set; } = Color.Blue;

    [Category(PKM), LocalizedDescription("Female gender color.")]
    public Color Female { get; set; } = Color.Red;

    [Category(PKM), LocalizedDescription("Shiny star when using unicode characters.")]
    public string ShinyUnicode { get; set; } = "☆";

    [Category(PKM), LocalizedDescription("Shiny star when not using unicode characters.")]
    public string ShinyDefault { get; set; } = "*";

    #endregion

    public Color GetGenderColor(byte gender) => gender switch
    {
        0 => Male,
        1 => Female,
        _ => SystemColors.WindowText,
    };

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
