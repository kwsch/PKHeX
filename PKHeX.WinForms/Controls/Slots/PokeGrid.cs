using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PKHeX.WinForms.Controls
{
    public partial class PokeGrid : UserControl
    {
        public PokeGrid()
        {
            InitializeComponent();
        }

        public List<PictureBox> Entries = new List<PictureBox>();
        public int Slots { get; private set; }

        private const int sizeW = 40;
        private const int sizeH = 30;

        public bool InitializeGrid(int width, int height)
        {
            var newCount = width * height;
            if (Slots == newCount)
                return false;

            Generate(width, height);
            Slots = newCount;

            return true;
        }

        private const int padEdge = 1; // edges
        private const int border = 1; // between

        private void Generate(int width, int height)
        {
            SuspendLayout();
            Controls.Clear();
            Entries.Clear();

            const int colWidth = sizeW;
            const int rowHeight = sizeH;

            for (int row = 0; row < height; row++)
            {
                var y = padEdge + (row * (rowHeight + border));
                for (int column = 0; column < width; column++)
                {
                    var x = padEdge + (column * (colWidth + border));
                    var pb = GetControl(sizeW, sizeH);
                    Controls.Add(pb);
                    pb.Location = new Point(x, y);
                    Entries.Add(pb);
                }
            }

            Width = (2 * padEdge) + border + (width * (colWidth + border));
            Height = (2 * padEdge) + border + (height * (rowHeight + border));
            ResumeLayout();
        }

        public void SetBackground(Image img) => BackgroundImage = img;

        public static PictureBox GetControl(int width, int height)
        {
            return new PictureBox
            {
                AutoSize = false,
                BackColor = Color.Transparent,
                Width = width + (2 * 1),
                Height = height + (2 * 1),
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                BorderStyle = BorderStyle.FixedSingle,
            };
        }
    }
}
