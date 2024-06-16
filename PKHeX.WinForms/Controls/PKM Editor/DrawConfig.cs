using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using PKHeX.Core;

namespace PKHeX.WinForms;

/// <summary>
/// Drawing Configuration for painting and updating controls
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class DrawConfig : IDisposable
{
    private const string PKM = "Pokémon Editor";
    private const string Moves = "Moves";
    private const string Hovering = "Hovering";

    [Category(Hovering), LocalizedDescription("Hovering over a PKM color 1.")]
    public Color GlowInitial { get; set; } = Color.White;

    [Category(Hovering), LocalizedDescription("Hovering over a PKM color 2.")]
    public Color GlowFinal { get; set; } = Color.LightSkyBlue;

    [Category(PKM), LocalizedDescription("Vertical tab selected primary color.")]
    public Color VerticalSelectPrimary { get; set; } = Color.White;

    [Category(PKM), LocalizedDescription("Vertical tab selected secondary color.")]
    public Color VerticalSelectSecondary { get; set; } = Color.LightGray;

    #region PKM

    [Category(PKM), LocalizedDescription("Background color of a ComboBox when the selected item is not valid.")]
    public Color InvalidSelection { get; set; } = Color.DarkSalmon;

    [Category(PKM), LocalizedDescription("Default colored marking.")]
    public Color MarkDefault { get; set; } = Color.Black;

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

    #region Moves

    [Category(Moves), LocalizedDescription("Legal move choice background color.")]
    public Color BackLegal { get; set; } = Color.FromArgb(200, 255, 200);

    [Category(Moves), LocalizedDescription("Legal move choice text color.")]
    public Color TextColor { get; set; } = SystemColors.WindowText;

    [Category(Moves), LocalizedDescription("Illegal Legal move choice background color.")]
    public Color BackColor { get; set; } = SystemColors.Window;

    [Category(Moves), LocalizedDescription("Highlighted move choice background color.")]
    public Color BackHighlighted { get; set; } = SystemColors.Highlight;

    [Category(Moves), LocalizedDescription("Highlighted move choice text color.")]
    public Color TextHighlighted { get; set; } = SystemColors.HighlightText;

    #endregion

    public DrawConfig() => LoadBrushes();

    public Color GetGenderColor(byte gender) => gender switch
    {
        0 => Male,
        1 => Female,
        _ => TextColor,
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

    public Color GetText(bool highlight) => highlight ? TextHighlighted : TextColor;
    public Color GetBackground(bool legal, bool highlight) => highlight ? BackHighlighted : (legal ? BackLegal : BackColor);

    [NonSerialized]
    public readonly BrushSet Brushes = new();

    public void LoadBrushes()
    {
        Brushes.BackLegal = new SolidBrush(BackLegal);
        Brushes.Text = new SolidBrush(TextColor);
        Brushes.BackDefault = new SolidBrush(BackColor);
        Brushes.TextHighlighted = new SolidBrush(TextHighlighted);
        Brushes.BackHighlighted = new SolidBrush(BackHighlighted);
    }

    public void Dispose() => Brushes.Dispose();
}

public sealed class BrushSet : IDisposable
{
    public Brush Text { get; set; } = Brushes.Black;
    public Brush BackLegal { get; set; } = Brushes.DarkSeaGreen;
    public Brush BackDefault { get; set; } = Brushes.White;
    public Brush TextHighlighted { get; set; } = Brushes.White;
    public Brush BackHighlighted { get; set; } = Brushes.Blue;

    public Brush GetText(bool highlight) => highlight ? TextHighlighted : Text;
    public Brush GetBackground(bool legal, bool highlight) => highlight ? BackHighlighted : (legal ? BackLegal : BackDefault);

    public void Dispose()
    {
        Text.Dispose();
        BackLegal.Dispose();
        BackDefault.Dispose();
        TextHighlighted.Dispose();
        BackHighlighted.Dispose();
    }
}
