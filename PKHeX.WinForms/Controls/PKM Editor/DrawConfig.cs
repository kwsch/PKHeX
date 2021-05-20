using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    /// <summary>
    /// Drawing Configuration for painting and updating controls
    /// </summary>
    [Serializable]
    public sealed class DrawConfig : IDisposable
    {
        private const string PKM = "Pokémon Editor";
        private const string Moves = "Moves";
        private const string Hovering = "Hovering";

        [Category(Hovering), Description("Hovering over a PKM color 1.")]
        public Color GlowInitial { get; set; } = Color.White;

        [Category(Hovering), Description("Hovering over a PKM color 2.")]
        public Color GlowFinal { get; set; } = Color.LightSkyBlue;

        #region PKM

        [Category(PKM), Description("Background color of a ComboBox when the selected item is not valid.")]
        public Color InvalidSelection { get; set; } = Color.DarkSalmon;

        [Category(PKM), Description("Default colored marking.")]
        public Color MarkDefault { get; set; } = Color.Black;

        [Category(PKM), Description("Blue colored marking.")]
        public Color MarkBlue { get; set; } = Color.FromArgb(000, 191, 255);

        [Category(PKM), Description("Pink colored marking.")]
        public Color MarkPink { get; set; } = Color.FromArgb(255, 117, 179);

        [Category(PKM), Description("Male gender color.")]
        public Color Male { get; set; } = Color.Blue;

        [Category(PKM), Description("Female gender color.")]
        public Color Female { get; set; } = Color.Red;

        [Category(PKM), Description("Shiny star when using unicode characters.")]
        public string ShinyUnicode { get; set; } = "☆";

        [Category(PKM), Description("Shiny star when not using unicode characters.")]
        public string ShinyDefault { get; set; } = "*";

        #endregion

        #region Moves

        [Category(Moves), Description("Legal move choice background color.")]
        public Color BackLegal { get; set; } = Color.FromArgb(200, 255, 200);

        [Category(Moves), Description("Legal move choice text color.")]
        public Color TextColor { get; set; } = SystemColors.WindowText;

        [Category(Moves), Description("Illegal Legal move choice background color.")]
        public Color BackColor { get; set; } = SystemColors.Window;

        [Category(Moves), Description("Highlighted move choice background color.")]
        public Color BackHighlighted { get; set; } = SystemColors.Highlight;

        [Category(Moves), Description("Highlighted move choice text color.")]
        public Color TextHighlighted { get; set; } = SystemColors.HighlightText;

        #endregion

        public DrawConfig() => LoadBrushes();

        public Color GetGenderColor(int gender) => gender switch
        {
            0 => Male,
            1 => Female,
            _ => TextColor
        };

        public bool GetMarkingColor(int markval, out Color c)
        {
            switch (markval)
            {
                case 1: c = MarkBlue; return true;
                case 2: c = MarkPink; return true;
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

        public override string ToString()
        {
            var props = ReflectUtil.GetAllPropertyInfoCanWritePublic(typeof(DrawConfig));
            var lines = new List<string>();
            foreach (var p in props)
            {
                if (p.PropertyType == typeof(BrushSet))
                    continue;

                var name = p.Name;
                var value = p.PropertyType == typeof(Color) ? ((Color)p.GetValue(this)!).ToArgb() : p.GetValue(this);
                lines.Add($"{name}\t{value}");
            }
            return string.Join("\n", lines);
        }
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
}
