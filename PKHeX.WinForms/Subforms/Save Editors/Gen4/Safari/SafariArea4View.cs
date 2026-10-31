using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

/// <summary>
/// Draws the objects of a Safari Zone area on top of the map's terrain.
/// </summary>
/// <remarks>
/// Objects are colored by type and shaded by row index. Overlapping objects are hatched one way, objects resting on unplaceable terrain another way.
/// The horizontal axis is X, the vertical axis is Z; Y is the height off the ground.
/// </remarks>
public sealed partial class SafariArea4View : UserControl
{
    private const int World = SafariZoneMap4.WorldSize;
    private const int MaxObjects = SafariZoneArea4.BlockCount;

    private static readonly Color Unplaceable = Color.FromArgb(60, 60, 60);
    private static readonly Color Land = Color.FromArgb(190, 220, 160);
    private static readonly Color Water = Color.FromArgb(150, 190, 235);
    private static readonly Color Grid = Color.FromArgb(40, 0, 0, 0);

    private readonly SafariZoneObject4[] Objects = new SafariZoneObject4[MaxObjects];
    private readonly SafariZonePlacement4[] Placements = new SafariZonePlacement4[MaxObjects];
    private SafariZoneType4 Map;

    /// <summary> Raised with the row index of the object that was clicked. </summary>
    public event Action<int>? ObjectClicked;

    /// <summary> Raised with the row index of the object that was Alt-clicked for deletion. </summary>
    public event Action<int>? ObjectDeleteRequested;

    /// <summary> Raised with the X, Z, and terrain height (Y) of a tile that was clicked while no object occupies it. </summary>
    public event Action<int, int, int>? EmptyTileClicked;

    public SafariArea4View()
    {
        InitializeComponent();
        DoubleBuffered = true;
    }

    /// <summary> Changes the terrain shown, and re-evaluates all objects. </summary>
    public void SetMap(SafariZoneType4 map)
    {
        Map = map;
        Reevaluate();
    }

    /// <summary> Replaces all objects (bulk update). </summary>
    public void SetObjects(ReadOnlySpan<SafariZoneObject4> objects)
    {
        Array.Clear(Objects);
        objects[..Math.Min(objects.Length, MaxObjects)].CopyTo(Objects);
        Reevaluate();
    }

    /// <summary> Updates a single object. </summary>
    public void SetObject(int index, SafariZoneObject4 obj)
    {
        if ((uint)index >= MaxObjects)
            return;
        Objects[index] = obj;
        Reevaluate();
    }

    private void Reevaluate()
    {
        SafariZoneMap4.Evaluate(Map, Objects, Placements);
        PB_Map.Invalidate();
    }

    private (int Size, int X, int Y) GetCanvas()
    {
        var client = PB_Map.ClientSize;
        var size = Math.Max(World, Math.Min(client.Width, client.Height) / World * World);
        return (size, Math.Max(0, (client.Width - size) / 2), Math.Max(0, (client.Height - size) / 2));
    }

    private void PB_Map_Resize(object? sender, EventArgs e) => PB_Map.Invalidate();

    private void PB_Map_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        var (size, ox, oy) = GetCanvas();
        int cell = size / World;

        DrawTerrain(g, ox, oy, cell);
        for (int i = 0; i < Objects.Length; i++)
        {
            var obj = Objects[i];
            if (!obj.IsEmpty)
                DrawObject(g, i, obj, ox, oy, cell);
        }
    }

    private void DrawTerrain(Graphics g, int ox, int oy, int cell)
    {
        using var land = new SolidBrush(Land);
        using var water = new SolidBrush(Water);
        using var none = new SolidBrush(Unplaceable);
        using var grid = new Pen(Grid);
        for (int z = 0; z < World; z++)
        {
            for (int x = 0; x < World; x++)
            {
                var brush = SafariZoneMap4.IsWater(Map, x, z) ? water : SafariZoneMap4.IsLand(Map, x, z) ? land : none;
                var rect = new Rectangle(ox + (x * cell), oy + (z * cell), cell, cell);
                g.FillRectangle(brush, rect);
                g.DrawRectangle(grid, rect);
            }
        }
    }

    private void DrawObject(Graphics g, int index, SafariZoneObject4 obj, int ox, int oy, int cell)
    {
        var (w, d) = SafariZoneMap4.GetSize(obj.Type);
        var rect = new Rectangle(ox + (obj.X * cell), oy + (obj.Z * cell), w * cell, d * cell);
        var color = GetColor(obj.Type, index);
        var flags = Placements[index];

        using (var fill = new SolidBrush(color))
            g.FillRectangle(fill, rect);

        if (flags != SafariZonePlacement4.None)
        {
            var style = flags switch
            {
                SafariZonePlacement4.Overlap => HatchStyle.WideUpwardDiagonal,
                SafariZonePlacement4.Unplaceable => HatchStyle.Cross,
                _ => HatchStyle.DiagonalCross,
            };
            using var hatch = new HatchBrush(style, Color.Black, Color.Transparent);
            g.FillRectangle(hatch, rect);
        }

        using var pen = new Pen(Color.Black);
        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

        if (cell < 8)
            return;
        using var font = new Font(Font.FontFamily, Math.Max(6f, cell * 0.45f), FontStyle.Bold, GraphicsUnit.Pixel);
        using var format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;
        using var text = new SolidBrush(color.GetBrightness() > 0.55f ? Color.Black : Color.White);
        g.DrawString((index + 1).ToString(), font, text, rect, format);
    }

    /// <summary> Hue is chosen by object type, lightness by row index. </summary>
    private static Color GetColor(SafariZoneObjectType4 type, int index)
    {
        var hue = (int)type * 47 % 360;
        var lightness = 0.35 + (0.07 * (index % 6));
        return FromHsl(hue, 0.7, lightness);
    }

    private static Color FromHsl(double h, double s, double l)
    {
        double c = (1 - Math.Abs((2 * l) - 1)) * s;
        double x = c * (1 - Math.Abs((h / 60 % 2) - 1));
        double m = l - (c / 2);
        var (r, g, b) = (int)(h / 60) switch
        {
            0 => (c, x, 0d),
            1 => (x, c, 0d),
            2 => (0d, c, x),
            3 => (0d, x, c),
            4 => (x, 0d, c),
            _ => (c, 0d, x),
        };
        return Color.FromArgb((int)Math.Round((r + m) * 255), (int)Math.Round((g + m) * 255), (int)Math.Round((b + m) * 255));
    }

    private bool TryGetTile(Point location, out int x, out int z)
    {
        var (size, ox, oy) = GetCanvas();
        int cell = size / World;
        x = (location.X - ox) / cell;
        z = (location.Y - oy) / cell;
        return location.X >= ox && location.Y >= oy && x < World && z < World;
    }

    private int FindObject(int x, int z)
    {
        // Later rows are drawn on top, so prefer them.
        for (int i = Objects.Length - 1; i >= 0; i--)
        {
            var obj = Objects[i];
            if (obj.IsEmpty)
                continue;
            var (w, d) = SafariZoneMap4.GetSize(obj.Type);
            if (x >= obj.X && x < obj.X + w && z >= obj.Z && z < obj.Z + d)
                return i;
        }
        return -1;
    }

    private void PB_Map_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!TryGetTile(e.Location, out var x, out var z))
        {
            ResetCoordinates();
            return;
        }

        var y = SafariZoneMap4.GetHeight(Map, x, z);
        var yText = y < 0 ? "--" : y.ToString();
        var row = FindObject(x, z);
        var rowText = row < 0 ? string.Empty : $"  (#{row + 1})";
        L_Coordinates.Text = @$"X: {x}  Z: {z}  Y: {yText}{rowText}";
    }

    private void PB_Map_MouseLeave(object? sender, EventArgs e) => ResetCoordinates();

    private void ResetCoordinates() => L_Coordinates.Text = @"X: --  Z: --  Y: --";

    private void PB_Map_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || !TryGetTile(e.Location, out var x, out var z))
            return;
        var row = FindObject(x, z);
        if ((ModifierKeys & Keys.Alt) != 0)
        {
            if (row >= 0)
                ObjectDeleteRequested?.Invoke(row);
        }
        else if (row >= 0)
            ObjectClicked?.Invoke(row);
        else
            EmptyTileClicked?.Invoke(x, z, Math.Max(0, SafariZoneMap4.GetHeight(Map, x, z)));
    }
}
