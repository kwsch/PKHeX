using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms.Controls;

public partial class PokeGrid : UserControl
{
    public PokeGrid()
    {
        InitializeComponent();
    }

    public readonly List<PictureBox> Entries = [];
    public int Slots { get; private set; }

    private int sizeW = 68;
    private int sizeH = 56;

    public bool InitializeGrid(int width, int height, SpriteBuilder info)
    {
        var newCount = width * height;
        if (Slots == newCount)
        {
            if (info.Width == sizeW && info.Height == sizeH)
                return false;
        }

        sizeW = info.Width;
        sizeH = info.Height;
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
        foreach (var c in Entries)
            c.Dispose();
        Entries.Clear();

        int colWidth = sizeW;
        int rowHeight = sizeH;

        Location = Location with { X = 0 }; // prevent auto-expanding parent if position changes (centered)
        for (int row = 0; row < height; row++)
        {
            var y = padEdge + (row * (rowHeight + border));
            for (int column = 0; column < width; column++)
            {
                var name = $"Pokémon Grid Row {row:00} Column {column:00}";
                var x = padEdge + (column * (colWidth + border));
                var pb = GetControl(sizeW, sizeH, name);
                pb.Location = new Point(x, y);
                Entries.Add(pb);
            }
        }

        int w = (2 * padEdge) + border + (width * (colWidth + border));
        int h = (2 * padEdge) + border + (height * (rowHeight + border));
        Size = new Size(w, h);
        Controls.AddRange(Entries.Cast<Control>().ToArray());
        ResumeLayout();
    }

    public void SetBackground(Image img) => BackgroundImage = img;

    public static SelectablePictureBox GetControl(int width, int height, string name) => new()
    {
        AutoSize = false,
        SizeMode = PictureBoxSizeMode.CenterImage,
        BackColor = SlotUtil.GoodDataColor,
        Width = width + (2 * border),
        Height = height + (2 * border),
        Padding = Padding.Empty,
        Margin = Padding.Empty,
        BorderStyle = BorderStyle.FixedSingle,
        AccessibleRole = AccessibleRole.Alert,
        Name = name,
        AccessibleName = name,
    };
}
