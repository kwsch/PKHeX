using System;
using System.Drawing;

namespace PKHeX.WinForms.Controls
{
    /// <summary>
    /// Drawing Configuration for painting and updating controls
    /// </summary>
    [Serializable]
    public class DrawConfig : IDisposable
    {
        public Color InvalidSelection { get; set; } = Color.DarkSalmon;
        public Color MarkBlue { get; set; } = Color.FromArgb(000, 191, 255);
        public Color MarkPink { get; set; } = Color.FromArgb(255, 117, 179);
        public Color MarkDefault { get; set; } = Color.Black;
        public Color BackLegal { get; set; } = Color.FromArgb(200, 255, 200);
        public Color TextColor { get; set; } = SystemColors.WindowText;
        public Color BackColor { get; set; } = SystemColors.Window;
        public Color TextHighlighted { get; set; } = SystemColors.HighlightText;
        public Color BackHighlighted { get; set; } = SystemColors.Highlight;
        public Color GlowInitial { get; set; } = Color.White;
        public Color GlowFinal { get; set; } = Color.LightSkyBlue;

        public string ShinyDefault { get; set; } = "*";
        public string ShinyUnicode { get; set; } = "☆";

        public DrawConfig() => LoadBrushes();

        public Color GetGenderColor(int gender)
        {
            switch (gender)
            {
                case 0: return Color.Blue;
                case 1: return Color.Red;
                default: return TextColor;
            }
        }

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

        public readonly BrushSet Brushes = new BrushSet();

        public void LoadBrushes()
        {
            Brushes.BackLegal = new SolidBrush(BackLegal);
            Brushes.Text = new SolidBrush(TextColor);
            Brushes.BackDefault = new SolidBrush(BackColor);
            Brushes.TextHighlighted = new SolidBrush(TextHighlighted);
            Brushes.BackHighlighted = new SolidBrush(BackHighlighted);
        }

        public void Dispose() => Brushes.Dispose();

        public class BrushSet : IDisposable
        {
            public Brush Text { get; set; }
            public Brush BackLegal { get; set; }
            public Brush BackDefault { get; set; }
            public Brush TextHighlighted { get; set; }
            public Brush BackHighlighted { get; set; }

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
}
